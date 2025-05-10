using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using GCProject.Main.Configs;
using GCProject.Main.Lib;
using GCProject.Main.Log;

namespace GCProject.Main;

public class Program
{
	[STAThread]
	public static void Main()
	{
		if (!InRunningCheck(3000))
		{
			MessageBox.Show("The program is already running！");
			return;
		}
		try
		{
			GCLogger.log.Info("Run Program v" + Library.CurrentVersion);
			new SplashScreen("resource/PMT9000_Agent.png").Show(autoClose: true);
			if (!MacAddressHelper.CheckLicence())
			{
				MessageBox.Show("Unauthorized Equipment");
				return;
			}
			ConfigModel.CheckOrCreateConfig();
			AutoRunHelper.SetAutoRun();
			ConfigModel.InitConfigs();
			App app = new App();
			app.InitializeComponent();
			app.Run();
			GCLogger.log.Info("Exit Program");
		}
		catch (Exception ex)
		{
			GCLogger.log.Error(ex.Message);
			GCLogger.log.Error(ex.StackTrace);
		}
	}

	public static bool InRunningCheck(int timeout_ms)
	{
		while (timeout_ms > 0)
		{
			if (Process.GetProcessesByName("BACHIndustrieMesstechnologie").Length < 2)
			{
				return true;
			}
			timeout_ms -= 500;
			Thread.Sleep(500);
		}
		return false;
	}
}
