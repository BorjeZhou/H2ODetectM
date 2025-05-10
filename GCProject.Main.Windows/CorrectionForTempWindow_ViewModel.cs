using System.Collections.Generic;
using System.Collections.ObjectModel;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.Lib;
using OxyPlot;

namespace GCProject.Main.Windows;

public class CorrectionForTempWindow_ViewModel : NotifyBase
{
	private double _weightAvgR;

	private bool _useCustomTempCorrection;

	private bool _useStatic = true;

	private ObservableCollection<RegressionForTempConfig> _staticConfigs;

	private ObservableCollection<RegressionForTempConfig> _regressionConfig;

	private ObservableCollection<SpliteTemp> _regressionSplitTempList;

	public GCRecipe SelectedRecipe { get; set; }

	public double WeightAvgR
	{
		get
		{
			return _weightAvgR;
		}
		set
		{
			if (_weightAvgR != value)
			{
				_weightAvgR = value;
				OnPropertyChanged("WeightAvgR");
			}
		}
	}

	public bool UseCustomTempCorrection
	{
		get
		{
			return _useCustomTempCorrection;
		}
		set
		{
			if (_useCustomTempCorrection != value)
			{
				_useCustomTempCorrection = value;
				OnPropertyChanged("UseCustomTempCorrection");
			}
		}
	}

	public bool UseStatic
	{
		get
		{
			return _useStatic;
		}
		set
		{
			if (_useStatic != value)
			{
				_useStatic = value;
				OnPropertyChanged("UseStatic");
			}
		}
	}

	public ObservableCollection<RegressionForTempConfig> StaticConfigs
	{
		get
		{
			return _staticConfigs;
		}
		set
		{
			if (_staticConfigs != value)
			{
				_staticConfigs = value;
				OnPropertyChanged("StaticConfigs");
			}
		}
	}

	public ObservableCollection<RegressionForTempConfig> RegressionConfig
	{
		get
		{
			return _regressionConfig;
		}
		set
		{
			if (_regressionConfig != value)
			{
				_regressionConfig = value;
				OnPropertyChanged("RegressionConfig");
			}
		}
	}

	public ObservableCollection<SpliteTemp> RegressionSplitTempList
	{
		get
		{
			return _regressionSplitTempList;
		}
		set
		{
			if (_regressionSplitTempList != value)
			{
				_regressionSplitTempList = value;
				OnPropertyChanged("RegressionSplitTempList");
			}
		}
	}

	public List<DataPoint> CorrectionPoints { get; set; } = new List<DataPoint>();


	public CorrectionForTempWindow_ViewModel()
	{
		StaticConfigs = new ObservableCollection<RegressionForTempConfig>();
		RegressionConfig = new ObservableCollection<RegressionForTempConfig>();
		RegressionSplitTempList = new ObservableCollection<SpliteTemp>();
	}
}
