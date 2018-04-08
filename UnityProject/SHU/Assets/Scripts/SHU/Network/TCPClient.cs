using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SHU {
  namespace Network {
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
      public ConcurrentQueue<PacketData> RecvedPackets = new ConcurrentQueue<PacketData>();
      public ReadWriter readWriter;

      public TCPClient() {
        readWriter = new SizeReadWriter (new PacketReadWriter (
          (packet) => {
            Debug.Log(string.Format("Recved Packet ID:{0} size:{1} msg:{2}", 
              packet.packetID, packet.data.Length, Encoding.UTF8.GetString(packet.data)));    
            RecvedPackets.Enqueue(packet);
          }));

      }

    	// Use this for initialization 	
    	void Start () { 
    	}  	
    	// Update is called once per frame
    	void Update () { 
    		if (isConnected) {
    			lock (thisLock) {
    				if (disconnected) {
    					socketConnection = null;
              RecvedPackets.Clear ();
              if (OnDisconnectEvent != null) {
                OnDisconnectEvent.Invoke ();
              }
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
 
    	private void OnRecvData(Byte[] bytes, int length) {
        readWriter.Read(new BufData(length, bytes));
    	}

    	public void Send(object data) {         
    		if (socketConnection == null) {             
    			return;         
    		}
        byte[] newArr = readWriter.Write (data) as byte[];
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
  }
}