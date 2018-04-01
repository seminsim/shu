using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHU
{
	namespace Debugging
	{
		public class DeveloperConsole : MonoBehaviour
		{
			readonly string bindingsGroup = "devConsole";
			Canvas dbgCanvas;
			GameObject parentObj;
			GameObject animationParent;
			Text txt;
			Text inputTxt;
			Image bg;
			int maxChars = 2000;
			bool shown = false;

			bool animating = false;
			float animationTime = 0;
			Vector3 start;
			Vector3 end;

			Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
			List<string> commandNames = new List<string>();

			int LogLevel = 0;

			public static bool IS_DEBUG_MODE = false;

			public void RegisterCommand(string commandName, ConsoleCommand newCommand)
			{
				commandName = commandName.Trim().ToLower();

				if (commandName.Contains(" "))
				{
					Log("[ERROR] Tried to register command containing whitespace.");
				}
				else if (!commands.ContainsKey(commandName))
				{
					commands.Add(commandName, newCommand);
					commandNames.Add(commandName);
					commandNames.Sort();
				}
				else
				{
					Log("[ERROR] Tried to register command \"" + commandName + "\" but it is already registered.");
				}
			}

			void Toggle()
			{
				Vector3 up = new Vector3(0, Screen.height, 0);
				Vector3 down = new Vector3(0, Screen.height / 2, 0);

				shown = !shown;

				if (shown)
				{
					//InputBinding.SetBindingGroup(bindingsGroup);
				}
				else
				{
					//InputBinding.SetBindingGroup("default");
				}

				start = shown ? up : down;
				end = shown ? down : up;

				if (!animating)
				{
					animating = true;
					StartCoroutine(Animate());
				}
				else
				{
					animationTime = 1 - animationTime;
					start = shown ? down : up;
					end = shown ? up : down;
				}
			}

			void Awake()
			{
				parentObj = new GameObject("DeveloperConsole", typeof(Canvas), typeof(CanvasScaler));
				Canvas dbgCanvas = parentObj.GetComponent<Canvas>();
				dbgCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
				dbgCanvas.pixelPerfect = true;
				dbgCanvas.sortingOrder = 1337; //make highest to make sure console stays on top
				parentObj.transform.SetParent(transform);

				animationParent = new GameObject("AnimationParent", typeof(Canvas));
				animationParent.transform.SetParent(parentObj.transform);
				animationParent.GetComponent<RectTransform>().anchorMin = Vector2.zero;
				animationParent.GetComponent<RectTransform>().anchorMax = Vector2.one;
				animationParent.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
				animationParent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				animationParent.transform.localPosition = new Vector3(0, Screen.height, 0);

				GameObject bgObj = new GameObject("DeveloperConsoleBG", typeof(Image));
				bgObj.transform.SetParent(animationParent.transform);
				bg = bgObj.GetComponent<Image>();
				bg.rectTransform.anchorMin = Vector2.zero;
				bg.rectTransform.anchorMax = Vector2.one;
				bg.rectTransform.sizeDelta = Vector2.zero;
				bg.rectTransform.anchoredPosition = Vector2.zero;
				bg.color = new Color(0, 0, 0, 0.5f);

				GameObject textObj = new GameObject("DeveloperConsoleText", typeof(Text), typeof(Outline));
				textObj.transform.SetParent(animationParent.transform);
				txt = textObj.GetComponent<Text>();
				txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				txt.alignment = TextAnchor.LowerLeft;
				txt.verticalOverflow = VerticalWrapMode.Overflow;
				txt.rectTransform.anchorMin = Vector2.zero;
				txt.rectTransform.anchorMax = Vector2.one;
				txt.rectTransform.sizeDelta = new Vector2(0, -35);
				txt.rectTransform.anchoredPosition = Vector2.zero;
				textObj.GetComponent<Outline>().effectColor = Color.black;
				txt.text = "DeveloperConsole initialized.";

				GameObject inputTextObj = new GameObject("DeveloperConsoleText", typeof(Text), typeof(Outline));
				inputTextObj.transform.SetParent(animationParent.transform);
				inputTxt = inputTextObj.GetComponent<Text>();
				inputTxt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				inputTxt.alignment = TextAnchor.LowerLeft;
				inputTxt.verticalOverflow = VerticalWrapMode.Overflow;
				inputTxt.rectTransform.anchorMin = Vector2.zero;
				inputTxt.rectTransform.anchorMax = Vector2.one;
				inputTxt.rectTransform.sizeDelta = Vector2.zero;
				inputTxt.rectTransform.anchoredPosition = Vector2.zero;
				textObj.GetComponent<Outline>().effectColor = Color.black;
				inputTxt.text = ">";
			}

			void Start()
			{
				RegisterBasicCommands();
			}

			void Update()
			{
				if (shown)
				{
					if (Input.inputString.Contains("\n") || Input.inputString.Contains("\r"))
					{
						ProcessInput();
					}
					else if (Input.inputString.Contains("\b"))
					{
						inputTxt.text = inputTxt.text.Substring(0, Mathf.Max(inputTxt.text.Length - 1, 1));
					}
					else if (Input.inputString != "")
					{
						inputTxt.text += Input.inputString;
					}
				}

				if (Input.GetKeyDown (KeyCode.BackQuote)) 
				{
					Toggle ();
				}
			}

			StringBuilder builder = new StringBuilder();
			public void Log(object obj)
			{
				if (builder.Length > maxChars)
				{
					builder.Remove (0, 1500);
				}

				builder.Append ("\n" + obj.ToString ());

				txt.text = builder.ToString ();
			}

			IEnumerator Animate()
			{
				animationTime = 0;
				while (animationTime < 1)
				{
					animationTime += Time.deltaTime * 4;
					if (animationTime > 1) animationTime = 1;
					animationParent.transform.localPosition = Vector3.LerpUnclamped(start, end, animationTime* animationTime *(3f - 2f * animationTime)); //smooth step

					yield return null;
				}
				animating = false;
			}

			void ProcessInput()
			{
				string[] inputArr = inputTxt.text.Substring(1).Split(' ');
				List<string> parameters = new List<string>();
				string command = inputArr[0].ToLower();
				Log("]" + inputTxt.text.Substring(1));

				for (int i = 1; i < inputArr.Length; i++)
				{
					if (inputArr[i] == "") continue;
					else if (inputArr[i].StartsWith("\""))
					{
						string quoteParam = inputArr[i];
						while (!inputArr[i].EndsWith("\"") && i < inputArr.Length)
						{
							quoteParam += " " + inputArr[i];
							i++;
						}
						parameters.Add(quoteParam);
					}
					else
					{
						parameters.Add(inputArr[i]);
					}

				}

				if (commands.ContainsKey(command.ToLower()))
				{
					if (parameters.Count > 0 && parameters[0] == "?")
					{
						Log(commands[command].HelpMessage);
					}
					else
					{
						Log(commands[command].Execute(parameters.ToArray()));
					}
				}
				else
				{
					Log("Unrecognized command \"" + command + "\"");
				}

				inputTxt.text = ">";
			}

			void RegisterBasicCommands()
			{
				RegisterCommand("Debugging", new ConsoleCommand((x) => { IS_DEBUG_MODE = !IS_DEBUG_MODE; return "Debugging mode " + (IS_DEBUG_MODE ? "ON" : "OFF"); }, "Toggles debugging mode."));
				RegisterCommand("version", new ConsoleCommand((x) => { return "Developer Console v0.8.1"; }, "version - Displays the version of the Developer Console."));
				RegisterCommand("help", new ConsoleCommand(
					(x) =>
					{
						int idx = 0;
						if (x.Length > 0)
						{
							int.TryParse(x[0], out idx);
							idx -= 1;
						}

						int commandCount = commands.Count;

						if (idx > commandCount)
						{
							idx = commandCount - 10;
						}
						else if (idx < 0)
						{
							idx = 0;
						}

						string helpString = "Showing commands " + (idx + 1) + " - " + Mathf.Min(idx + 10, commandCount) + " of " + commandCount + ":";

						for (int i = idx; i < idx + 10 && i < commandCount; i++)
						{
							helpString += "\n  " + (i + 1) + " - " + commandNames[i];
						}

						helpString += "\nType \"?\" after a command for more details.";

						return helpString;
					},
					"help <number> - Displays a list of commands."
				));
				RegisterCommand("Logging", new ConsoleCommand(
					(x) =>
					{
						int logLevel = -1;

						if (x.Length > 0)
						{
							if (int.TryParse(x[0], out logLevel))
							{
								LogLevel = Mathf.Clamp(logLevel, 0, 3);

								if (LogLevel == 0)
								{
									Application.logMessageReceived -= Application_logMessageReceived;
								}
								else
								{
									Application.logMessageReceived += Application_logMessageReceived;
								}

								return "Logging level set to " + LogLevel;
							}
						}

						return "Logging level is " + LogLevel;
					},
					"Sets logging level for the console.\n 0 - No logging.\n1 - Errors only.\n2 - Errors and Warnings.\n3 - Errors, Warnings and Debugs."
				));
			}

			private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
			{
				if (type == LogType.Log && LogLevel < 3) return;
				if (type == LogType.Warning && LogLevel < 2) return;

				Log(type + "\n" + condition + "\n" + stackTrace);
			}
		}
	}
}