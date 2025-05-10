using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using GCProject.Main.Log;
using Microsoft.EntityFrameworkCore;
using OxyPlot;

namespace GCProject.Main.Windows;

public partial class RawDataChartWindow : Window, IWindow, INotifyPropertyChanged, IComponentConnector
{
	public const string Field_H20 = "H₂O(%)";

	public const string Field_CPS = "CPS";

	public const string Field_Width = "Width(m)";

	public const string Field_Height = "Height(m)";

	private string _leftAxisSelected = "H₂O(%)";

	private double _leftMin;

	private double _leftMax;

	private double _rightMin;

	private double _rightMax;

	private string _rightAxisSelected = "CPS";

	private List<DataPoint> H20Datas;

	private List<DataPoint> CPSDatas;

	private List<DataPoint> WidthDatas;

	private List<DataPoint> HeightDatas;

	private GCSummaryEntity entity;

	public string LeftAxisSelected
	{
		get
		{
			return _leftAxisSelected;
		}
		set
		{
			_leftAxisSelected = value;
			OnPropertyChanged("LeftAxisSelected");
		}
	}

	public double LeftMin
	{
		get
		{
			return _leftMin;
		}
		set
		{
			_leftMin = value;
			OnPropertyChanged("LeftMin");
		}
	}

	public double LeftMax
	{
		get
		{
			return _leftMax;
		}
		set
		{
			_leftMax = value;
			OnPropertyChanged("LeftMax");
		}
	}

	public double RightMin
	{
		get
		{
			return _rightMin;
		}
		set
		{
			_rightMin = value;
			OnPropertyChanged("RightMin");
		}
	}

	public double RightMax
	{
		get
		{
			return _rightMax;
		}
		set
		{
			_rightMax = value;
			OnPropertyChanged("RightMax");
		}
	}

	public string RightAxisSelected
	{
		get
		{
			return _rightAxisSelected;
		}
		set
		{
			_rightAxisSelected = value;
			OnPropertyChanged("RightAxisSelected");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public RawDataChartWindow()
	{
		InitializeComponent();
	}

	public void Init(object args)
	{
		base.DataContext = null;
		base.DataContext = this;
		GCSummaryEntity summaryEntity = args as GCSummaryEntity;
		if (args == null)
		{
			return;
		}
		using (GCContext db = new GCContext())
		{
			long id = summaryEntity.GCSummaryEntityID;
			entity = db.GCSummaryEntities.Include((GCSummaryEntity g) => g.RawDatas).FirstOrDefault((GCSummaryEntity g) => g.GCSummaryEntityID == id);
			double MultiParam = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? 1.0 : ConfigModel.Instance.ParamSettingConfig.Velosity);
			if (entity != null)
			{
				List<GCRawEntity> RawDatas = entity.RawDatas.OrderBy((GCRawEntity r) => r.Index).ToList();
				double step = (float)ConfigModel.Instance.PollQueryInterval_ms / 1000f;
				H20Datas = RawDatas.Select((GCRawEntity r) => new DataPoint((double)(int)r.Index * step * MultiParam, r.H20)).ToList();
				CPSDatas = RawDatas.Select((GCRawEntity r) => new DataPoint((double)(int)r.Index * step * MultiParam, r.CPS)).ToList();
				WidthDatas = RawDatas.Select((GCRawEntity r) => new DataPoint((double)(int)r.Index * step * MultiParam, r.Width)).ToList();
				try
				{
					HeightDatas = RawDatas.Select((GCRawEntity r) => new DataPoint((double)(int)r.Index * step * MultiParam, r.Height.Value)).ToList();
				}
				catch (Exception e)
				{
					HeightDatas = new List<DataPoint>();
					GCLogger.log.Error(e.Message);
					GCLogger.log.Error(e.StackTrace);
				}
			}
		}
		ReloadDatas();
	}

	private void ReloadDatas()
	{
		if (entity != null)
		{
			double MultiParam = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? 1.0 : ConfigModel.Instance.ParamSettingConfig.Velosity);
			MainChart.SetRange(1, 0.0, entity.Duration * MultiParam);
			MainChart.X_Axis.Title = ((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? GCProject.Main.LangResource.Lang.Time_s : GCProject.Main.LangResource.Lang.Distance_m);
			SetData(2, LeftMin, LeftMax, LeftAxisSelected);
			SetData(3, RightMin, RightMax, RightAxisSelected);
			if (LeftAxisSelected == "H₂O(%)")
			{
				MainChart.LeftYAxisDatas = H20Datas;
			}
			else if (LeftAxisSelected == "CPS")
			{
				MainChart.LeftYAxisDatas = CPSDatas;
			}
			else if (LeftAxisSelected == GCProject.Main.LangResource.Lang.RawDataChartWindow_Height)
			{
				MainChart.LeftYAxisDatas = HeightDatas;
			}
			else
			{
				MainChart.LeftYAxisDatas = WidthDatas;
			}
			if (RightAxisSelected == "H₂O(%)")
			{
				MainChart.RightYAxisDatas = H20Datas;
			}
			else if (RightAxisSelected == "CPS")
			{
				MainChart.RightYAxisDatas = CPSDatas;
			}
			else if (RightAxisSelected == GCProject.Main.LangResource.Lang.RawDataChartWindow_Height)
			{
				MainChart.RightYAxisDatas = HeightDatas;
			}
			else
			{
				MainChart.RightYAxisDatas = WidthDatas;
			}
			MainChart.UpdateChart();
		}
	}

	private void SetData(int AxisName, double min, double max, string title)
	{
		MainChart.SetRange(AxisName, min, max);
		if (AxisName == 2)
		{
			MainChart.Y_Axis_Left.Title = title;
			MainChart.LeftYAxisSeries.Title = title;
		}
		else
		{
			MainChart.Y_Axis_Right.Title = title;
			MainChart.RightYAxisSeries.Title = title;
		}
	}

	private void btn_RefreshClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		ReloadDatas();
	}

	private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		e.Handled = true;
		if (!(sender is ComboBox cmbx))
		{
			return;
		}
		if (cmbx.Tag as string == "left")
		{
			if (cmbx.SelectedValue as string == "H₂O(%)")
			{
				LeftMin = ConfigModel.Instance.ParamSettingConfig.H20MeasRangeMin_868M;
				LeftMax = ConfigModel.Instance.ParamSettingConfig.H20MeasRangeMax_868M;
			}
			else if (cmbx.SelectedValue as string == "CPS")
			{
				LeftMin = ConfigModel.Instance.ParamSettingConfig.CPSMeasRangeMin_868M;
				LeftMax = ConfigModel.Instance.ParamSettingConfig.CPSMeasRangeMax_868M;
			}
			else if (cmbx.SelectedValue as string == GCProject.Main.LangResource.Lang.RawDataChartWindow_Height)
			{
				LeftMin = 0.0;
				LeftMax = ConfigModel.Instance.ParamSettingConfig.HeightSensorsDistance;
			}
			else
			{
				LeftMin = 0.0;
				LeftMax = ConfigModel.Instance.ParamSettingConfig.WidthSensorsDistance;
			}
		}
		else if (cmbx.SelectedValue as string == "H₂O(%)")
		{
			RightMin = ConfigModel.Instance.ParamSettingConfig.H20MeasRangeMin_868M;
			RightMax = ConfigModel.Instance.ParamSettingConfig.H20MeasRangeMax_868M;
		}
		else if (cmbx.SelectedValue as string == "CPS")
		{
			RightMin = ConfigModel.Instance.ParamSettingConfig.CPSMeasRangeMin_868M;
			RightMax = ConfigModel.Instance.ParamSettingConfig.CPSMeasRangeMax_868M;
		}
		else if (cmbx.SelectedValue as string == GCProject.Main.LangResource.Lang.RawDataChartWindow_Height)
		{
			RightMin = 0.0;
			RightMin = ConfigModel.Instance.ParamSettingConfig.HeightSensorsDistance;
		}
		else
		{
			RightMin = 0.0;
			RightMax = ConfigModel.Instance.ParamSettingConfig.WidthSensorsDistance;
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "8.0.8.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
