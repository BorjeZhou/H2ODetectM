using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using GCProject.Main.Configs;
using GCProject.Main.LangResource;
using GCProject.Main.Windows;

namespace GCProject.Main.Lib;

public class PwdGuard
{
	private static readonly Timer CountDownTimer = new Timer(CountDownCallback, null, -1, -1);

	private static bool RequriedPwd { get; set; } = true;


	public static string MD5Encrypt16(string input)
	{
		return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(input)), 4, 8).Replace("-", "");
	}

	public static bool CheckPwd(string input, string encryptPwd)
	{
		return MD5Encrypt16(input) == encryptPwd;
	}

	private static void CountDownCallback(object p)
	{
		RequriedPwd = true;
	}

	public static void StartCountDown(int minute = 2)
	{
		RequriedPwd = false;
		CountDownTimer.Change(minute * 60 * 1000, -1);
	}

	public static void PerformChangePwd(Window Owner)
	{
		PasswordWindow window = new PasswordWindow(GCProject.Main.LangResource.Lang.PwdGuard_InputPassword);
		window.Owner = Owner;
		if (!window.ShowDialog().Value)
		{
			return;
		}
		if (!CheckPwd(window.InputValue, ConfigModel.Instance.EncyptPwd))
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.PwdGuard_PasswordIncorrect, GCProject.Main.LangResource.Lang.Error, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		PasswordWindow newPwd = new PasswordWindow(GCProject.Main.LangResource.Lang.PwdGuard_InputNewPassword, "", showPassword: true);
		newPwd.Owner = Owner;
		if (newPwd.ShowDialog().Value)
		{
			string newPwdValue = MD5Encrypt16(newPwd.InputValue);
			ConfigModel.Instance.EncyptPwd = newPwdValue;
			ConfigModel.Instance.Save();
		}
	}

	public static bool PerformInputPwd(Window Owner)
	{
		if (!RequriedPwd)
		{
			return true;
		}
		PasswordWindow window = new PasswordWindow(GCProject.Main.LangResource.Lang.PwdGuard_InputPassword);
		window.Owner = Owner;
		bool? res = window.ShowDialog();
		if (res.HasValue && !res.Value)
		{
			return false;
		}
		if (!CheckPwd(window.InputValue, ConfigModel.Instance.EncyptPwd))
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.PwdGuard_PasswordIncorrect, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return false;
		}
		StartCountDown();
		return true;
	}
}
