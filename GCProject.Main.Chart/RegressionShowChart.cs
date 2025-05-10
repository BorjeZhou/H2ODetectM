using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GCProject.Main.Chart;

public partial class RegressionShowChart : UserControl, IComponentConnector
{
	private LineSeries OldSeries;

	public PlotModel MainChart { get; set; }

	private LineSeries NewSeries { get; set; }

	private LineSeries NewSeries2 { get; set; }

	private LineSeries NewSeries3 { get; set; }

	public RegressionShowChart()
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
			MinimumMajorStep = 5.0
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
			MinimumMajorStep = 5.0
		};
		MainChart.Axes.Add(X_Axis);
		MainChart.Axes.Add(Y_Axis);
		LineAnnotation lineAnnotation = new LineAnnotation
		{
			ClipByXAxis = false,
			ClipByYAxis = false,
			Color = OxyColors.DarkGreen,
			LineStyle = LineStyle.Solid,
			StrokeThickness = 1.0,
			Type = LineAnnotationType.LinearEquation,
			Intercept = 0.0,
			Slope = 1.0
		};
		MainChart.Annotations.Add(lineAnnotation);
		OldSeries = new LineSeries
		{
			Selectable = false,
			MarkerType = MarkerType.Star,
			MarkerStroke = OxyColors.Gray,
			MarkerFill = OxyColors.Gray,
			MarkerSize = 3.0,
			CanTrackerInterpolatePoints = false,
			SelectionMode = OxyPlot.SelectionMode.Single,
			StrokeThickness = 0.0,
			MarkerStrokeThickness = 1.0,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		NewSeries = new LineSeries
		{
			Selectable = false,
			MarkerType = MarkerType.Circle,
			MarkerStroke = OxyColors.Red,
			MarkerFill = OxyColors.Red,
			MarkerSize = 3.0,
			CanTrackerInterpolatePoints = false,
			SelectionMode = OxyPlot.SelectionMode.Single,
			StrokeThickness = 0.0,
			MarkerStrokeThickness = 1.0,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		NewSeries2 = new LineSeries
		{
			Selectable = false,
			MarkerType = MarkerType.Circle,
			MarkerStroke = OxyColors.Blue,
			MarkerFill = OxyColors.Blue,
			MarkerSize = 3.0,
			CanTrackerInterpolatePoints = false,
			SelectionMode = OxyPlot.SelectionMode.Single,
			StrokeThickness = 0.0,
			MarkerStrokeThickness = 1.0,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		NewSeries3 = new LineSeries
		{
			Selectable = false,
			MarkerType = MarkerType.Circle,
			MarkerStroke = OxyColors.Green,
			MarkerFill = OxyColors.Green,
			MarkerSize = 3.0,
			CanTrackerInterpolatePoints = false,
			SelectionMode = OxyPlot.SelectionMode.Single,
			StrokeThickness = 0.0,
			MarkerStrokeThickness = 1.0,
			YAxisKey = "Y_Axis",
			XAxisKey = "X_Axis"
		};
		MainChart.Series.Add(OldSeries);
		MainChart.Series.Add(NewSeries);
		MainChart.Series.Add(NewSeries2);
		MainChart.Series.Add(NewSeries3);
	}

	public void Clear()
	{
		OldSeries.ItemsSource = null;
		NewSeries.ItemsSource = null;
		NewSeries2.ItemsSource = null;
		NewSeries3.ItemsSource = null;
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	public void LoadOldData(List<DataPoint> OldDatas)
	{
		OldSeries.ItemsSource = null;
		OldSeries.ItemsSource = OldDatas;
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	public void LoadNewData(List<DataPoint> newDatas)
	{
		NewSeries.ItemsSource = null;
		NewSeries.ItemsSource = newDatas;
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	public void LoadNewDataEx(Dictionary<int, List<DataPoint>> newDatas)
	{
		NewSeries3.ItemsSource = null;
		NewSeries2.ItemsSource = null;
		NewSeries.ItemsSource = null;
		if (newDatas.ContainsKey(3))
		{
			NewSeries3.ItemsSource = newDatas[3];
		}
		if (newDatas.ContainsKey(2))
		{
			NewSeries2.ItemsSource = newDatas[2];
		}
		if (newDatas.ContainsKey(1))
		{
			NewSeries.ItemsSource = newDatas[1];
		}
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	private void AdjustRange()
	{
		double maxR = 0.0;
		double minR = 100.0;
		if (OldSeries.ItemsSource != null && OldSeries.ItemsSource is List<DataPoint> { Count: >0 } oldDatas)
		{
			double X_min1 = oldDatas.Select((DataPoint x) => x.X).Min();
			double X_max1 = oldDatas.Select((DataPoint x) => x.X).Max();
			double Y_min1 = oldDatas.Select((DataPoint x) => x.Y).Min();
			double Y_max1 = oldDatas.Select((DataPoint x) => x.Y).Max();
			double min1 = Min(X_min1, Y_min1);
			double max1 = Max(X_max1, Y_max1);
			minR = Min(minR, min1);
			maxR = Max(maxR, max1);
		}
		if (NewSeries.ItemsSource != null && NewSeries.ItemsSource is List<DataPoint> { Count: >0 } newDatas)
		{
			double X_min2 = newDatas.Select((DataPoint x) => x.X).Min();
			double X_max2 = newDatas.Select((DataPoint x) => x.X).Max();
			double Y_min2 = newDatas.Select((DataPoint x) => x.Y).Min();
			double Y_max2 = newDatas.Select((DataPoint x) => x.Y).Max();
			double min2 = Min(X_min2, Y_min2);
			double max2 = Max(X_max2, Y_max2);
			minR = Min(minR, min2);
			maxR = Max(maxR, max2);
		}
		if (NewSeries2.ItemsSource != null && NewSeries2.ItemsSource is List<DataPoint> { Count: >0 } newDatas2)
		{
			double X_min3 = newDatas2.Select((DataPoint x) => x.X).Min();
			double X_max3 = newDatas2.Select((DataPoint x) => x.X).Max();
			double Y_min3 = newDatas2.Select((DataPoint x) => x.Y).Min();
			double Y_max3 = newDatas2.Select((DataPoint x) => x.Y).Max();
			double min3 = Min(X_min3, Y_min3);
			double max3 = Max(X_max3, Y_max3);
			minR = Min(minR, min3);
			maxR = Max(maxR, max3);
		}
		if (NewSeries3.ItemsSource != null && NewSeries3.ItemsSource is List<DataPoint> { Count: >0 } newDatas3)
		{
			double X_min4 = newDatas3.Select((DataPoint x) => x.X).Min();
			double X_max4 = newDatas3.Select((DataPoint x) => x.X).Max();
			double Y_min4 = newDatas3.Select((DataPoint x) => x.Y).Min();
			double Y_max4 = newDatas3.Select((DataPoint x) => x.Y).Max();
			double min4 = Min(X_min4, Y_min4);
			double max4 = Max(X_max4, Y_max4);
			minR = Min(minR, min4);
			maxR = Max(maxR, max4);
		}
		minR *= 0.8;
		maxR *= 1.2;
		if (maxR - minR < 10.0)
		{
			double num = 100.0 - maxR;
			double minCount = minR;
			if (num > minCount)
			{
				maxR = minR + 10.0;
			}
			else
			{
				minR = maxR - 10.0;
			}
		}
		minR = Max(minR, 0.0);
		maxR = Min(maxR, 100.0);
		foreach (Axis axis in MainChart.Axes)
		{
			axis.Minimum = minR;
			axis.Maximum = maxR;
		}
	}

	private double Min(double v1, double v2)
	{
		if (!(v1 - v2 > 0.0))
		{
			return v1;
		}
		return v2;
	}

	private double Max(double v1, double v2)
	{
		if (!(v1 - v2 > 0.0))
		{
			return v2;
		}
		return v1;
	}
}
