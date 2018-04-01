using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour {  	
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
	private bool isConnected = false;
	private bool disconnected = false;
	private System.Object thisLock = new System.Object();  
	#endregion

	public delegate void OnDisconnectHandler();

	// Declare the event.
	public event OnDisconnectHandler OnDisconnectEvent;
	ConcurrentQueue<byte[]> RecvedPackets = new ConcurrentQueue<byte[]>();

	// Use this for initialization 	
	void Start () { 
	}  	
	// Update is called once per frame
	void Update () { 
		if (isConnected) {
			lock (thisLock) {
				if (disconnected) {
					socketConnection = null;
					OnDisconnectEvent.Invoke();
					isConnected = false;
					disconnected = false;
				}
			}
		}
	}  	

	public void Connect (string host, int port) { 		
		try {  		
			isConnected = true;
			socketConnection = new TcpClient(host, port);
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start(); 
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
			lock (thisLock) {
				disconnected = true;
			}
		} 	
	}  	

	private void ListenForData() { 		
		try { 						
			Byte[] bytes = new Byte[1024];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
						OnRecvData(bytes, length);
					} 				
				} 			
			}         
		}         
		catch (Exception exception) {             
			Debug.Log("Socket exception: " + exception);   
			lock (thisLock) {
				disconnected = true;
			}
		}     
	}

	bool readSize = true;
	Byte[] sizeBuf = new byte[2];
	Byte[] dataBuf;
	int readPos = 0;
	UInt16 dataSize = 0;

	private void OnRecvData(Byte[] bytes, int length) {
		int idx = 0;
		while (idx < length) {
			if (readSize) {
				for (; idx < length && readPos < 2; idx++) {
					sizeBuf[readPos] = bytes[idx];
					readPos++;
				}
			
				if (readPos == 2) {
					if (!BitConverter.IsLittleEndian) {
						Array.Reverse (sizeBuf);
					}
					dataSize = BitConverter.ToUInt16 (sizeBuf, 0);
					dataBuf = new byte[dataSize];
					readPos = 0;
					readSize = false;
				}
			} else {
				for (; idx < length && readPos < dataSize; idx++) {
					dataBuf[readPos] = bytes[idx];
					readPos++;
				}
				if (readPos == dataSize) {
					RecvedPackets.Enqueue (dataBuf);
					Debug.Log("Recved Packet: " + Encoding.UTF8.GetString(dataBuf));  
					readSize = true;
					readPos = 0;
				}

			}
		}
	}

	public void Send(byte[] data) {         
		if (socketConnection == null) {             
			return;         
		}
		byte[] newArr = new byte[2 + data.Length];
		byte[] lenBuf = BitConverter.GetBytes ((UInt16)(data.Length));
		if (!BitConverter.IsLittleEndian) {
			Array.Reverse (lenBuf);
		}
		for (int i = 0; i < 2; i++) {
			newArr [i] = lenBuf [i];
		}
		for (int i = 0; i < data.Length; i++) {
			newArr [i + 2] = data [i];
		}
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
				// Write byte array to socketConnection stream.                 
				stream.Write(newArr, 0, newArr.Length);
			}         
		} 		
		catch (Exception exception) {             
			Debug.Log("Socket exception: " + exception);         
		}     
	}

	public void Close() {
		if (socketConnection != null) {             
			socketConnection.Close ();
			socketConnection = null;
		}  
	}
}