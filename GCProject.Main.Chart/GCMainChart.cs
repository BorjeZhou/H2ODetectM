using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.LangResource;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GCProject.Main.Chart;

public partial class GCMainChart : UserControl, IComponentConnector
{
	private BarSeries BarSeries;

	private LineAnnotation HighAlramLine;

	private LineAnnotation LowAlramLine;

	private OxyColor[] BarColors = new OxyColor[2]
	{
		OxyColor.FromRgb(223, 110, 110),
		OxyColor.FromRgb(168, 217, 63)
	};

	private int colorindex;

	public PlotModel WaveChart { get; private set; }

	public Axis X_Axis_wave { get; private set; }

	public Axis Y_Axis_Wave { get; private set; }

	public PlotModel BarChart { get; set; }

	public Axis X_Axis_bar { get; private set; }

	public Axis Y_Axis_bar { get; private set; }

	public Axis X_Axis_bar_Display { get; private set; }

	public List<DataPoint> WaveData { get; set; }

	public List<BarItem> ColumnDatas { get; set; }

	public GCMainChart()
	{
		InitializeComponent();
		OxyChart_Loaded(null, null);
	}

	private void OxyChart_Loaded(object sender, RoutedEventArgs e)
	{
		base.DataContext = this;
		InitWaveChart();
		InitBarChart();
	}

	private void InitWaveChart()
	{
		HighAlramLine = new LineAnnotation
		{
			StrokeThickness = 1.0,
			Color = OxyColors.Red,
			Type = LineAnnotationType.Horizontal,
			Y = -1.0,
			LineStyle = LineStyle.Solid
		};
		LowAlramLine = new LineAnnotation
		{
			StrokeThickness = 1.0,
			Color = OxyColors.Red,
			Type = LineAnnotationType.Horizontal,
			Y = -1.0,
			LineStyle = LineStyle.Solid
		};
		WaveChart = new PlotModel
		{
			Title = "   ",
			TitleFontWeight = 0.7,
			TitleFontSize = 13.0
		};
		WaveChart.Annotations.Add(HighAlramLine);
		WaveChart.Annotations.Add(LowAlramLine);
		X_Axis_wave = new LinearAxis
		{
			Position = AxisPosition.Bottom,
			Minimum = 0.0,
			Maximum = 20.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis_wave",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		Y_Axis_Wave = new LinearAxis
		{
			Title = "H₂O %",
			Position = AxisPosition.Left,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Minimum = 0.0,
			Maximum = 100.0,
			Key = "Y_Axis_Wave",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		WaveChart.Axes.Add(X_Axis_wave);
		WaveChart.Axes.Add(Y_Axis_Wave);
		WaveData = new List<DataPoint>();
		LineSeries lineSeries = new LineSeries
		{
			ItemsSource = WaveData,
			Selectable = false,
			MarkerType = MarkerType.Circle,
			MarkerSize = 2.0,
			MarkerFill = OxyColors.Transparent,
			MarkerStroke = OxyColors.DarkBlue,
			Color = OxyColors.DarkBlue,
			CanTrackerInterpolatePoints = false,
			SelectionMode = OxyPlot.SelectionMode.Single,
			StrokeThickness = 1.0,
			YAxisKey = "Y_Axis_Wave",
			XAxisKey = "X_Axis_wave"
		};
		WaveChart.Series.Add(lineSeries);
	}

	private void InitBarChart()
	{
		BarChart = new PlotModel
		{
			TitleFontWeight = 0.7,
			TitleFontSize = 13.0,
			Padding = new OxyThickness(8.0, 0.0, 8.0, 8.0),
			TitlePadding = 0.0
		};
		Y_Axis_bar = new LinearAxis
		{
			Title = "H₂O %",
			Position = AxisPosition.Left,
			Minimum = 0.0,
			Maximum = 100.0,
			MajorGridlineThickness = 1.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "Y_Axis_bar",
			IsZoomEnabled = false,
			IsPanEnabled = false
		};
		X_Axis_bar = new CategoryAxis
		{
			Position = AxisPosition.Bottom,
			Minimum = -0.5,
			Maximum = 39.5,
			GapWidth = 0.0,
			Key = "X_Axis_bar",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			Selectable = false,
			IsAxisVisible = false
		};
		X_Axis_bar_Display = new LinearAxis
		{
			Position = AxisPosition.Bottom,
			Minimum = 0.0,
			Maximum = 20.0,
			MajorGridlineThickness = 1.0,
			MajorGridlineStyle = LineStyle.DashDashDot,
			Key = "X_Axis_bar_Display",
			IsZoomEnabled = false,
			IsPanEnabled = false,
			Selectable = false,
			FontSize = 10.0
		};
		BarChart.Axes.Add(X_Axis_bar);
		BarChart.Axes.Add(Y_Axis_bar);
		BarChart.Axes.Add(X_Axis_bar_Display);
		ColumnDatas = new List<BarItem>();
		BarSeries = new BarSeries
		{
			ItemsSource = ColumnDatas,
			SelectionMode = OxyPlot.SelectionMode.Single,
			IsStacked = true,
			FillColor = OxyColors.DarkGreen,
			Selectable = false,
			StrokeThickness = 1.0,
			YAxisKey = "X_Axis_bar",
			XAxisKey = "Y_Axis_bar"
		};
		for (int i = 0; i < 100; i++)
		{
			ColumnDatas.Add(new BarItem(-50.0));
		}
		BarChart.Series.Add(BarSeries);
	}

	public void UpdateBarSeriesParam(ParamSettingConfig p)
	{
		BarSeries.LabelAngle = p.BarChartLabelAngle;
		BarSeries.LabelFormatString = $"{{0:F{p.BarChartLabelDecimal}}}";
		BarSeries.FontSize = p.BarChartFontSize;
	}

	public void SetTitle(string title)
	{
		WaveChart.Title = title;
		WaveChart.InvalidatePlot(updateData: true);
	}

	public void ClearAll()
	{
		ColumnDatas.Clear();
		WaveData.Clear();
		WaveChart.InvalidatePlot(updateData: true);
		BarChart.InvalidatePlot(updateData: true);
		colorindex = 0;
		UpdateRange(ConfigModel.Instance.ParamSettingConfig.MaxChargeTime_s);
	}

	public void DropPointAndReDraw(int dropCount)
	{
		int dropWave = dropCount;
		if (WaveData.Count < dropWave)
		{
			dropWave = WaveData.Count;
		}
		WaveData.RemoveRange(WaveData.Count - dropWave, dropWave);
		int dropBar = (int)Math.Floor((double)dropCount / (double)ConfigModel.Instance.ParamSettingConfig.OneBarWidthPoint);
		if (dropBar > ColumnDatas.Count)
		{
			dropBar = ColumnDatas.Count;
		}
		ColumnDatas.RemoveRange(ColumnDatas.Count - dropBar, dropBar);
		AdjustRange();
	}

	public void AddWavePoint(double h20Current)
	{
		double MultiParam = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? 1.0 : ConfigModel.Instance.ParamSettingConfig.Velosity);
		WaveData.Add(new DataPoint((double)((float)WaveData.Count * ((float)ConfigModel.Instance.PollQueryInterval_ms / 1000f)) * MultiParam, h20Current));
		WaveChart.InvalidatePlot(updateData: true);
	}

	public void AddBarPoint(double H20Avg1Sencond)
	{
		if (H20Avg1Sencond == 0.0)
		{
			H20Avg1Sencond = -50.0;
		}
		ColumnDatas.Add(new BarItem(H20Avg1Sencond)
		{
			Color = BarColors[colorindex++ % 2]
		});
		BarChart.InvalidatePlot(updateData: true);
	}

	public int GetCurrentBarCount()
	{
		return ColumnDatas.Count;
	}

	public void SetAxisType(string type)
	{
		lb_AxisType.Dispatcher.Invoke(() => lb_AxisType.Text = ((type == "time") ? GCProject.Main.LangResource.Lang.MainChart_Time_s : GCProject.Main.LangResource.Lang.MainChart_Distance_m));
	}

	public void UpdateRange(double MaxTime)
	{
		double MultiParam = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? 1.0 : ConfigModel.Instance.ParamSettingConfig.Velosity);
		double calMax = MaxTime * MultiParam;
		X_Axis_wave.Maximum = calMax;
		X_Axis_wave.MajorStep = calMax / 10.0;
		X_Axis_wave.MinorStep = 1.0 * MultiParam;
		WaveData.Clear();
		WaveChart.InvalidatePlot(updateData: true);
		X_Axis_bar_Display.Maximum = calMax;
		X_Axis_bar_Display.MajorStep = calMax / 10.0;
		X_Axis_bar_Display.MinorStep = 1.0 * MultiParam;
		colorindex = 0;
		float a = (float)ConfigModel.Instance.PollQueryInterval_ms / 1000f * (float)ConfigModel.Instance.ParamSettingConfig.OneBarWidthPoint;
		X_Axis_bar.Maximum = MaxTime / (double)a - 0.5;
		BarSeries.FontSize = ConfigModel.Instance.ParamSettingConfig.BarChartFontSize;
		ColumnDatas.Clear();
		BarChart.InvalidatePlot(updateData: true);
	}

	private void AdjustRange()
	{
		double MaxTime = (float)WaveData.Count * ((float)ConfigModel.Instance.PollQueryInterval_ms / 1000f);
		if (!(MaxTime < 1.0))
		{
			MaxTime = Math.Ceiling(MaxTime);
			double MultiParam = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? 1.0 : ConfigModel.Instance.ParamSettingConfig.Velosity);
			double calMax = MaxTime * MultiParam;
			X_Axis_wave.Maximum = calMax;
			X_Axis_wave.MajorStep = calMax / 10.0;
			X_Axis_wave.MinorStep = 1.0 * MultiParam;
			WaveChart.InvalidatePlot(updateData: true);
			X_Axis_bar_Display.Maximum = calMax;
			X_Axis_bar_Display.MajorStep = calMax / 10.0;
			X_Axis_bar_Display.MinorStep = 1.0 * MultiParam;
			colorindex = 0;
			float a = (float)ConfigModel.Instance.PollQueryInterval_ms / 1000f * (float)ConfigModel.Instance.ParamSettingConfig.OneBarWidthPoint;
			X_Axis_bar.Maximum = MaxTime / (double)a - 0.5;
			BarSeries.FontSize = ConfigModel.Instance.ParamSettingConfig.BarChartFontSize;
			BarChart.InvalidatePlot(updateData: true);
		}
	}

	public void UpdateYAxisRange(ParamSettingConfig config)
	{
		Y_Axis_Wave.Minimum = config.H20DisplayRangeMin;
		Y_Axis_bar.Minimum = config.H20DisplayRangeMin;
		Y_Axis_Wave.Maximum = config.H20DisplayRangeMax;
		Y_Axis_bar.Maximum = config.H20DisplayRangeMax;
		Y_Axis_Wave.MajorStep = config.H20DisplayRangeMax / 5.0;
		Y_Axis_bar.MajorStep = config.H20DisplayRangeMax / 5.0;
		if (config.H20AlarmRangeMax_868M > 0.0 && config.H20AlarmRangeMax_868M < 100.0)
		{
			HighAlramLine.Y = config.H20AlarmRangeMax_868M;
		}
		if (config.H20AlarmRangeMin_868M > 0.0 && config.H20AlarmRangeMin_868M < 100.0)
		{
			LowAlramLine.Y = config.H20AlarmRangeMin_868M;
		}
		UpdateBarSeriesParam(config);
		WaveChart.InvalidatePlot(updateData: true);
		BarChart.InvalidatePlot(updateData: true);
	}
}
