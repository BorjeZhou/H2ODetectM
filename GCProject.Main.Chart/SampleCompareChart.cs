using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.LangResource;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace GCProject.Main.Chart;

public partial class SampleCompareChart : UserControl, IComponentConnector
{
	private Axis X_Axis;

	private Axis Y_Axis;

	public PlotModel MainChart { get; private set; }

	public List<DataPoint> LaborDatas { get; set; } = new List<DataPoint>();


	public List<DataPoint> MeassDatas { get; set; } = new List<DataPoint>();


	public SampleCompareChart()
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
			Title = GCProject.Main.LangResource.Lang.SampleCompareChart_SampleIndex,
			Position = AxisPosition.Bottom,
			Minimum = 0.0,
			MinimumMajorStep = 1.0,
			MinorStep = double.NaN,
			Maximum = 20.0,
			MinorTickSize = 0.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		Y_Axis = new LinearAxis
		{
			Title = "H₂O(%)",
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Minimum = 0.0,
			Maximum = 100.0,
			Key = "Y_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		MainChart.Axes.Add(X_Axis);
		MainChart.Axes.Add(Y_Axis);
		LineSeries LaborSeries = new LineSeries
		{
			Title = GCProject.Main.LangResource.Lang.SampleCompareChart_LaberValue,
			ItemsSource = LaborDatas,
			Selectable = false,
			MarkerType = MarkerType.Diamond,
			LineStyle = LineStyle.DashDashDot,
			MarkerSize = 3.0,
			Color = OxyColors.DarkBlue,
			CanTrackerInterpolatePoints = false,
			LabelFormatString = "{1:" + ConfigModel.DecimalStr + "}",
			StrokeThickness = 1.0,
			MarkerStrokeThickness = 0.6,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		LineSeries MeassSeries = new LineSeries
		{
			Title = GCProject.Main.LangResource.Lang.SampleCompareChart_MeasureValue,
			ItemsSource = MeassDatas,
			Selectable = false,
			MarkerType = MarkerType.Diamond,
			LineStyle = LineStyle.DashDashDot,
			MarkerSize = 3.0,
			Color = OxyColors.Red,
			CanTrackerInterpolatePoints = false,
			LabelFormatString = "{1:" + ConfigModel.DecimalStr + "}",
			StrokeThickness = 1.0,
			MarkerStrokeThickness = 0.6,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		MainChart.Series.Add(LaborSeries);
		MainChart.Series.Add(MeassSeries);
	}

	public void SetXAxisRange(int Min, int Max)
	{
		X_Axis.Minimum = Min;
		X_Axis.Maximum = Max;
		MainChart.InvalidatePlot(updateData: true);
	}

	public void SetYAxisRange(int Min, int Max)
	{
		Y_Axis.Minimum = Min;
		Y_Axis.Maximum = Max;
		MainChart.InvalidatePlot(updateData: true);
	}

	public void SetDatas(List<double> laborDatas, List<double> meassDatas)
	{
		LaborDatas.Clear();
		MeassDatas.Clear();
		int count = (int)Math.Ceiling((double)laborDatas.Count * 1.1);
		SetXAxisRange(0, count);
		for (int i = 0; i < laborDatas.Count; i++)
		{
			LaborDatas.Add(new DataPoint(i + 1, laborDatas[i]));
			MeassDatas.Add(new DataPoint(i + 1, meassDatas[i]));
		}
		MainChart.InvalidatePlot(updateData: true);
	}
}
