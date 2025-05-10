using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Interface;

namespace GCProject.Main.Windows;

public partial class RegressionAdjustWindow : Window, IWindow, IComponentConnector
{
	public RegressionAdjustWindow()
	{
		InitializeComponent();
		Init(null);
	}

	public void Init(object args)
	{
	}
}
