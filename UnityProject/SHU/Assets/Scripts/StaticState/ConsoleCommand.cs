using System;

namespace SHU {
  public class ConsoleCommand {
  	public readonly Func<string[], string> Execute;
  	public readonly string HelpMessage;

  	public ConsoleCommand(Func<string[], string> ExecuteMethod, string HelpMessage) {
  		Execute = ExecuteMethod;
  		this.HelpMessage = HelpMessage;
  	}
  }
}