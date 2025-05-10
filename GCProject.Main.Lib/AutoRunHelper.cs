using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace GCProject.Main.Lib;

public class AutoRunHelper
{
	public const string AutoRunKeyName = "BACHIndustrieMesstechnologie";

	public const string AutoRunKeyPath = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run";

	public static void SetAutoRun()
	{
		RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
		if (key == null)
		{
			key = Registry.LocalMachine.CreateSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
		}
		key.SetValue("BACHIndustrieMesstechnologie", GetRunProgramPath(), RegistryValueKind.String);
		key.Close();
	}

	public static string GetRunProgramPath()
	{
		string exe = Assembly.GetExecutingAssembly().GetName().Name + ".exe";
		return Path.Combine(Library.GetCurrentPath(), exe);
	}
}
