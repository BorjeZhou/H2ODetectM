using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Configs;

namespace GCProject.Main.Windows;

public partial class RunModeWindow : Window, IComponentConnector
{
	private ConfigModel ImportConfigModel;

	public RunModeWindow()
	{
		InitializeComponent();
		base.Closing += RunModeWindow_Closing;
	}

	private void RunModeWindow_Closing(object sender, CancelEventArgs e)
	{
		if (!base.DialogResult.HasValue)
		{
			base.DialogResult = false;
		}
	}

	private void UseDefaultClick(object sender, RoutedEventArgs e)
	{
		rb_RunMode.IsEnabled = true;
		ImportConfigModel = null;
	}

	private void OKClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
		ConfigModel.InitConfigs(NewCreate: true);
		ConfigModel.Instance.ParamSettingConfig.RunMode = (rb_Is24G.IsChecked.Value ? RunModeEnum.Mode_24G : RunModeEnum.Mode_868M);
		ConfigModel.Instance.Save();
	}

	private void CloseClick(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}
}
