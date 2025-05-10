using System.Diagnostics;

namespace GCProject.Main.Lib;

public class KeyboardHelper
{
	public static void ShowKeyboard()
	{
		Process process = new Process();
		process.StartInfo.UseShellExecute = true;
		process.StartInfo.FileName = "C:\\Windows\\System32\\osk.exe";
		process.Start();
	}
}
