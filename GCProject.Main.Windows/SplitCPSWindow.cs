using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class SplitCPSWindow : Window, IComponentConnector
{
	public SplitCPSViewModel ViewModel { get; set; }

	public bool? Result { get; set; }

	public SplitCPSWindow(SplitCPSViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		base.DataContext = ViewModel;
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void useCPSClick(object sender, RoutedEventArgs e)
	{
		ViewModel.UscCPSSplitType = true;
		Result = true;
		base.DialogResult = true;
	}

	private void useH20Click(object sender, RoutedEventArgs e)
	{
		ViewModel.UscCPSSplitType = false;
		Result = true;
		base.DialogResult = true;
	}
}
