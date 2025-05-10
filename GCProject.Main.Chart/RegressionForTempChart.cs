using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GCProject.Main.Chart;

public partial class RegressionForTempChart : UserControl, IComponentConnector
{
	public PlotModel MainChart { get; set; } = new PlotModel();


	private List<LineSeries> LineSeries { get; set; } = new List<LineSeries>();


	private LineSeries CorrectionValueSeries { get; set; }

	private List<DataPoint> CorrectionValues { get; set; } = new List<DataPoint>();


	public RegressionForTempChart()
	{
		InitializeComponent();
		init();
	}

	private void init()
	{
		base.DataContext = this;
		Axis X_Axis = new LinearAxis
		{
			Title = "Temperature(℃)",
			Position = AxisPosition.Bottom,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			MinimumMajorStep = 5.0
		};
		Axis Y_Axis = new LinearAxis
		{
			Title = "H₂O correction value(%)",
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "Y_Axis",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			MinimumMajorStep = 1.0
		};
		MainChart.Axes.Add(X_Axis);
		MainChart.Axes.Add(Y_Axis);
		CorrectionValueSeries = new LineSeries
		{
			Selectable = false,
			ItemsSource = CorrectionValues,
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
		MainChart.Series.Add(CorrectionValueSeries);
	}

	public void Clear()
	{
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	public void LoadCorrectionData(List<DataPoint> CorrectionDatas, List<List<DataPoint>> RegressionLines, List<double> splitTemps = null)
	{
		LineSeries.Clear();
		MainChart.Series.Clear();
		MainChart.Annotations.Clear();
		if (splitTemps != null && splitTemps.Count > 0)
		{
			foreach (double splitTemp in splitTemps)
			{
				LineAnnotation lineAnnotation = new LineAnnotation
				{
					ClipByXAxis = false,
					ClipByYAxis = false,
					Color = OxyColors.Red,
					LineStyle = LineStyle.DashDashDot,
					StrokeThickness = 1.0,
					Type = LineAnnotationType.Vertical,
					X = splitTemp
				};
				MainChart.Annotations.Add(lineAnnotation);
			}
		}
		CorrectionValues.Clear();
		CorrectionDatas.ForEach(delegate(DataPoint c)
		{
			CorrectionValues.Add(new DataPoint(c.X, c.Y));
		});
		MainChart.Series.Add(CorrectionValueSeries);
		foreach (List<DataPoint> line in RegressionLines)
		{
			LineSeries newLine = new LineSeries
			{
				ItemsSource = line,
				Selectable = false,
				Color = OxyColors.Green,
				MarkerType = MarkerType.Circle,
				MarkerSize = 0.0,
				CanTrackerInterpolatePoints = false,
				SelectionMode = OxyPlot.SelectionMode.Single,
				StrokeThickness = 1.0,
				MarkerStrokeThickness = 1.0,
				YAxisKey = "Y_Axis",
				XAxisKey = "X_Axis"
			};
			LineSeries.Add(newLine);
			MainChart.Series.Add(newLine);
		}
	}

	public void UpdateAll(bool UpdateDatas = true)
	{
		MainChart.InvalidatePlot(UpdateDatas);
	}

	public void LoadNewData(List<DataPoint> newDatas)
	{
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	public void LoadNewDataEx(Dictionary<int, List<DataPoint>> newDatas)
	{
		AdjustRange();
		MainChart.InvalidatePlot(updateData: true);
	}

	private void AdjustRange()
	{
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
