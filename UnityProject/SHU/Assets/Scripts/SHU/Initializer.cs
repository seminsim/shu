﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SHU {
  public class Initializer : MonoBehaviour {

  	// Use this for initialization
  	void Awake () {
  		var instance = StaticComponent<SHU.DeveloperConsole>.Instance;
      var serverTime = StaticComponent<Network.ServerTime>.Instance;
  		instance.RegisterCommand("Connect", new ConsoleCommand(
  			(x) => {
  				if (x.Length < 2)
  				{
  					return "Error - Connect <IP> <Port>";
  				}
  				int port;
  				if (!int.TryParse(x[1], out port))
  				{
  					return "Error - Connect <IP> <Port:number>";
  				}

  				StaticComponent<Network.TCPClient>.Instance.Connect(x[0], port);
  				return "Connect to " + x[0] + ":" + x[1];
  			},
  			"Connect <IP> <Port> - Connect To Server"
  		));

  		instance.RegisterCommand ("Send", new ConsoleCommand (
  			(x) => {
  				if (x.Length < 1) {
  					return "Error - Send <Msg>";
  				}

          StaticComponent<Network.TCPClient>.Instance.Send(new Network.PacketData(PacketID.Log, Encoding.UTF8.GetBytes(x[0])));
  				return "Send Msg:" + x [0];
  			},
  			"Send <Msg> - Send msg to server"
  		));
  	}

  	void OnDestroy() {
      StaticComponent<Network.TCPClient>.Instance.Close ();
  	}
  }
}
