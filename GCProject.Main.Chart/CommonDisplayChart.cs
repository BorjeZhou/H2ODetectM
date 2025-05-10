using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace GCProject.Main.Chart;

public partial class CommonDisplayChart : UserControl, INotifyPropertyChanged, IComponentConnector
{
	public const int XAxis = 1;

	public const int LeftYAxis = 2;

	public const int RightYAxis = 3;

	public OxyColor LeftColor = OxyColors.Blue;

	public OxyColor RightColor = OxyColors.Red;

	public PlotModel MainChart { get; private set; }

	public List<DataPoint> LeftYAxisDatas { get; set; } = new List<DataPoint>();


	public List<DataPoint> RightYAxisDatas { get; set; } = new List<DataPoint>();


	public Axis X_Axis { get; private set; }

	public Axis Y_Axis_Left { get; private set; }

	public Axis Y_Axis_Right { get; private set; }

	public LineSeries LeftYAxisSeries { get; set; }

	public LineSeries RightYAxisSeries { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public void UpdateChart()
	{
		LeftYAxisSeries.ItemsSource = null;
		LeftYAxisSeries.ItemsSource = LeftYAxisDatas;
		RightYAxisSeries.ItemsSource = null;
		RightYAxisSeries.ItemsSource = RightYAxisDatas;
		MainChart.InvalidatePlot(updateData: true);
	}

	public CommonDisplayChart()
	{
		InitializeComponent();
		base.DataContext = this;
		Legend legend = new Legend
		{
			LegendPosition = LegendPosition.RightTop
		};
		MainChart = new PlotModel
		{
			TitleFontWeight = 0.7,
			TitleFontSize = 13.0
		};
		MainChart.Legends.Add(legend);
		X_Axis = new LinearAxis
		{
			Position = AxisPosition.Bottom,
			MinorTickSize = 0.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		Y_Axis_Left = new LinearAxis
		{
			TextColor = LeftColor,
			TitleColor = LeftColor,
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "Y_Axis_Left",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		Y_Axis_Right = new LinearAxis
		{
			TextColor = RightColor,
			TitleColor = RightColor,
			Position = AxisPosition.Right,
			Key = "Y_Axis_Right",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		MainChart.Axes.Add(X_Axis);
		MainChart.Axes.Add(Y_Axis_Left);
		MainChart.Axes.Add(Y_Axis_Right);
		LeftYAxisSeries = new LineSeries
		{
			ItemsSource = LeftYAxisDatas,
			Selectable = false,
			LineStyle = LineStyle.Automatic,
			MarkerSize = 2.0,
			MarkerType = MarkerType.Circle,
			MarkerFill = OxyColors.Transparent,
			MarkerStroke = LeftColor,
			Color = LeftColor,
			CanTrackerInterpolatePoints = false,
			StrokeThickness = 1.0,
			MarkerStrokeThickness = 0.6,
			YAxisKey = "Y_Axis_Left",
			XAxisKey = "X_Axis"
		};
		RightYAxisSeries = new LineSeries
		{
			ItemsSource = RightYAxisDatas,
			Selectable = false,
			LineStyle = LineStyle.Automatic,
			MarkerSize = 2.0,
			MarkerType = MarkerType.Circle,
			MarkerFill = OxyColors.Transparent,
			MarkerStroke = RightColor,
			Color = RightColor,
			CanTrackerInterpolatePoints = false,
			StrokeThickness = 1.0,
			MarkerStrokeThickness = 0.6,
			YAxisKey = "Y_Axis_Right",
			XAxisKey = "X_Axis"
		};
		MainChart.Series.Add(LeftYAxisSeries);
		MainChart.Series.Add(RightYAxisSeries);
	}

	public void SetRange(int AxisName, double min, double max)
	{
		switch (AxisName)
		{
		case 1:
			X_Axis.Minimum = min;
			X_Axis.Maximum = max;
			break;
		case 2:
			Y_Axis_Left.Minimum = min;
			Y_Axis_Left.Maximum = max;
			break;
		default:
			Y_Axis_Right.Minimum = min;
			Y_Axis_Right.Maximum = max;
			break;
		}
	}

	public void SetDatas(List<Tuple<double, double>> h20_cps_Datas)
	{
		MainChart.InvalidatePlot(updateData: true);
	}
}
