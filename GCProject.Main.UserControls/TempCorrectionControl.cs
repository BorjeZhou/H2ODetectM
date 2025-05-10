using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;

namespace GCProject.Main.UserControls;

public partial class TempCorrectionControl : UserControl, IComponentConnector
{
	public RegressionForTempConfig ViewModel { get; set; } = new RegressionForTempConfig();


	public TempCorrectionControl()
	{
		InitializeComponent();
		base.DataContext = ViewModel;
	}
}
