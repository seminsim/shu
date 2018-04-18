// C# example.
#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;

public class ScriptBatch 
{
  [MenuItem("MyTools/Windows Build With Postprocess")]
  public static void BuildGame ()
  {
    // Get filename.
    string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
    string[] levels = new string[] {"Assets/Scenes/Main.unity"};

    // Build player.
    BuildPipeline.BuildPlayer(levels, path + "/BuiltGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

    // Copy a file from the project folder to the build folder, alongside the built game.
    //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

    // Run the game (Process class from System.Diagnostics).
    Process proc = new Process();
    proc.StartInfo.FileName = path + "/BuiltGame.exe";
    proc.Start();
  }

  static void PerformBuild ()
  {
    string outDir = string.Empty;
    string configFile = string.Empty;
    outDir = CommandLineReader.GetCustomArgument("out");
    configFile = CommandLineReader.GetCustomArgument ("config");

    string[] scenes = { "Assets/Scenes/Main.unity" };
    BuildPipeline.BuildPlayer(scenes, outDir + "/BuiltGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

    // Run the game (Process class from System.Diagnostics).
    Process proc = new Process();
    proc.StartInfo.FileName = outDir + "/BuiltGame.exe";
    proc.StartInfo.Arguments = "-config " + configFile + " -name player1";
    proc.Start();

    Process proc2 = new Process();
    proc2.StartInfo.FileName = outDir + "/BuiltGame.exe";
    proc2.StartInfo.Arguments = "-config " + configFile + " -name player2";
    proc2.Start();
  }
}
#endif