using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Interface;

namespace GCProject.Main.Windows;

public partial class BusWindow : Window, IWindow, IClose, IComponentConnector
{
	public BusWindow()
	{
		InitializeComponent();
	}

	public void Init(object args)
	{
		base.DataContext = null;
		MainWindow.BusDataProxy.ShowFashRawData = true;
		base.DataContext = MainWindow.BusDataProxy;
	}

	private void CloseBusWindowClick(object sender, RoutedEventArgs e)
	{
		SelfClose();
		e.Handled = true;
	}

	public void SelfClose()
	{
		MainWindow.BusDataProxy.ShowFashRawData = false;
		base.DataContext = null;
		Hide();
	}
}
