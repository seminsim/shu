using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Initializer : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		var instance = StaticComponent<SHU.Debugging.DeveloperConsole>.Instance;
		instance.RegisterCommand("Connect", new ConsoleCommand(
			(x) =>
			{
				if (x.Length < 2)
				{
					return "Error - Connect <IP> <Port>";
				}
				int port;
				if (!int.TryParse(x[1], out port))
				{
					return "Error - Connect <IP> <Port:number>";
				}

				StaticComponent<TCPClient>.Instance.Connect(x[0], port);
				return "Connect to " + x[0] + ":" + x[1];
			},
			"Connect <IP> <Port> - Connect To Server"
		));

		instance.RegisterCommand ("Send", new ConsoleCommand (
			(x) => {
				if (x.Length < 1) {
					return "Error - Send <Msg>";
				}

				StaticComponent<TCPClient>.Instance.Send(Encoding.UTF8.GetBytes(x[0]));
				return "Send Msg:" + x [0];
			},
			"Send <Msg> - Send msg to server"
		));
	}

	void OnDestroy() {
		StaticComponent<TCPClient>.Instance.Close ();
	}
}
