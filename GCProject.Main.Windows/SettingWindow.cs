using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using GCProject.Main.SerialPorts;

namespace GCProject.Main.Windows;

public partial class SettingWindow : Window, IWindow, IComponentConnector
{
	public string[] COMS { get; set; }

	public SettingWindow()
	{
		InitializeComponent();
	}

	private void SettingWindowClose(object sender, RoutedEventArgs e)
	{
		Hide();
		e.Handled = true;
	}

	public void Init(object args)
	{
		base.DataContext = ConfigModel.Instance;
		COMS = GCSerialPortManager.GetCOMList();
		cmbx_bus.ItemsSource = null;
		cmbx_bus.ItemsSource = COMS;
		if (COMS.Contains(ConfigModel.Instance.GCBusSerialPort.ComName))
		{
			cmbx_bus.SelectedValue = ConfigModel.Instance.GCBusSerialPort.ComName;
		}
		cmbx_board.ItemsSource = null;
		cmbx_board.ItemsSource = COMS;
		if (COMS.Contains(ConfigModel.Instance.GCBoardSerialPort.ComName))
		{
			cmbx_board.SelectedValue = ConfigModel.Instance.GCBoardSerialPort.ComName;
		}
		cmbx_plc.ItemsSource = null;
		cmbx_plc.ItemsSource = COMS;
		if (COMS.Contains(ConfigModel.Instance.GCPlcSerialPort.ComName))
		{
			cmbx_plc.SelectedValue = ConfigModel.Instance.GCPlcSerialPort.ComName;
		}
	}

	private void ChangePasswordClick(object sender, RoutedEventArgs e)
	{
		PwdGuard.PerformChangePwd(this);
	}

	private void SaveClick(object sender, RoutedEventArgs e)
	{
		if (!IsValid())
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SomeFieldError, GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			return;
		}
		string bus = cmbx_bus.SelectedValue as string;
		string plc = cmbx_plc.SelectedValue as string;
		string board = cmbx_board.SelectedValue as string;
		if (!string.IsNullOrEmpty(bus) && !string.IsNullOrEmpty(plc) && bus == plc)
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.SettingWindow_SamePortError, GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		if (!string.IsNullOrEmpty(board) && !string.IsNullOrEmpty(plc) && board == plc)
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.SettingWindow_SamePortError, GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		ConfigModel.Instance.GCBusSerialPort.ComName = cmbx_bus.SelectedValue as string;
		ConfigModel.Instance.GCBoardSerialPort.ComName = cmbx_board.SelectedValue as string;
		ConfigModel.Instance.GCPlcSerialPort.ComName = cmbx_plc.SelectedValue as string;
		ConfigModel.Instance.Save();
		GCSerialPortManager.OpenPort();
		MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SaveSuccess, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	private bool IsValid()
	{
		try
		{
			WindowsHelper.GetTextBoxsError(MainGrid);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}
}
