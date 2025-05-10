using System.Windows.Controls;
using System.Windows.Markup;
using OxyPlot;
using OxyPlot.Axes;

namespace GCProject.Main.Chart;

public partial class RegressionAdjustChart : UserControl, IComponentConnector
{
	public PlotModel MainChart { get; set; }

	public RegressionAdjustChart()
	{
		InitializeComponent();
		init();
	}

	private void init()
	{
		base.DataContext = this;
		MainChart = new PlotModel();
		Axis X_Axis = new LinearAxis
		{
			Title = "Labor H₂O Value(%)",
			TitlePosition = 0.9,
			Position = AxisPosition.Bottom,
			Minimum = 0.0,
			Maximum = 100.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			MajorStep = 20.0
		};
		Axis Y_Axis = new LinearAxis
		{
			Title = "Meas H₂O Value(%)",
			TitlePosition = 0.9,
			Position = AxisPosition.Left,
			Minimum = 0.0,
			Maximum = 100.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "Y_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			MajorStep = 20.0
		};
		MainChart.Axes.Add(X_Axis);
		MainChart.Axes.Add(Y_Axis);
	}
}
