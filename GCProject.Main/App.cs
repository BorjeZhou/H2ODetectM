using System.Windows;

namespace GCProject.Main;

public partial class App : Application
{
	protected override void OnStartup(StartupEventArgs e)
	{
		FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
		base.OnStartup(e);
	}
}
