using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace GCProject.Main.Lib;

public class MacAddressHelper
{
	public const string Manufacturer = "SOFTWARE\\BACH";

	public const string Encryt = "233DF9C2B033937DEAC6D22AD501E468";

	public const string LicenceFile = "licence";

	public static void CreateLicence(string file)
	{
		string EncrytNet = GetLicence();
		RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BACH", writable: true);
		registryKey.SetValue("licence", EncrytNet, RegistryValueKind.String);
		registryKey.Close();
	}

	private static string GetLicence()
	{
		string NetAddres = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault((NetworkInterface n) => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet).Id;
		if (string.IsNullOrEmpty(NetAddres))
		{
			NetAddres = "233DF9C2B033937DEAC6D22AD501E468";
		}
		return EncryptHelper.Get32MD5One(NetAddres);
	}

	private static string GetKey(string file)
	{
		string res = string.Empty;
		using StreamReader sr = new StreamReader(file);
		return sr.ReadToEnd();
	}

	private static bool Check(string file)
	{
		return EncryptHelper.Get32MD5One(GetKey(file)) == "233DF9C2B033937DEAC6D22AD501E468";
	}

	public static bool CheckLicence1()
	{
		string licence = Path.Combine(Library.GetCurrentPath(), "licence");
		if (File.Exists(licence))
		{
			if (!Check(licence))
			{
				return false;
			}
			CreateLicence(licence);
			return true;
		}
		string licence2 = GetLicence();
		RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\BACH");
		string reg = registryKey?.GetValue("licence") as string;
		registryKey?.Close();
		return licence2 == reg;
	}

	public static bool CheckLicence()
	{
		return Registry.LocalMachine.OpenSubKey("SOFTWARE\\BACH", writable: true) != null;
	}
}
