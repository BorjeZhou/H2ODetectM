using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class AboutWindow : Window, IComponentConnector
{
	public AboutWindow()
	{
		InitializeComponent();
		lb_verson.Text = Library.CurrentVersion;
	}
}
