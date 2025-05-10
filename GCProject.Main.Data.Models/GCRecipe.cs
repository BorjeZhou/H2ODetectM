using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using GCProject.Main.Configs;
using GCProject.Main.Lib;

namespace GCProject.Main.Data.Models;

public class GCRecipe : INotifyPropertyChanged
{
	public class KBStageInfo
	{
		public int Min { get; }

		public int Max { get; }

		public double Slope1 { get; }

		public double Offset1 { get; }

		public double? Slope2 { get; }

		public double? Offset2 { get; }

		public KBStageInfo(int min, int max, double slope1, double offset1, double? slope2 = null, double? offset2 = null)
		{
			Min = min;
			Max = max;
			Slope1 = slope1;
			Offset1 = offset1;
			Slope2 = slope2;
			Offset2 = offset2;
		}
	}

	private bool _display = true;

	private bool _EnableDynamicZeroRate;

	private double _slope = 1.0;

	private double _offSet;

	private double _zeroRate = 1000.0;

	private bool _enabledStage2;

	private int _split2 = 35000;

	private double _slope2 = 1.0;

	private double _offSet2;

	private bool _enabledStage3;

	private int _split3 = 40000;

	private double _slope3 = 1.0;

	private double _offSet3;

	private int _DropFrontPoint_Custom;

	private int _DropBehindPoint_Custom;

	private bool _UseCustomDropPoint;

	private bool _UseCustomTempCorrection;

	private DateTime _lastModifyDate = DateTime.Now;

	private List<KBStageInfo> _kBStageInfos;

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[XmlAttribute]
	public long GCRecipeID { get; set; }

	[XmlAttribute]
	public string DisplayName { get; set; }

	public string DisplayNameForMainWindow => $"{RecipeNum}-{DisplayName}";

	[XmlAttribute]
	public bool Display
	{
		get
		{
			return _display;
		}
		set
		{
			_display = value;
			OnPropertyChanged("Display");
		}
	}

	[XmlAttribute]
	public int RecipeNum { get; set; }

	[XmlAttribute]
	public bool EnableDynamicZeroRate
	{
		get
		{
			return _EnableDynamicZeroRate;
		}
		set
		{
			if (_EnableDynamicZeroRate != value)
			{
				_EnableDynamicZeroRate = value;
				OnPropertyChanged("EnableDynamicZeroRate");
			}
		}
	}

	[XmlAttribute]
	public int DynamicZeroRateCutSecond_Begin { get; set; } = 4;


	[XmlAttribute]
	public int DynamicZeroRateCutSecond_End { get; set; } = 4;


	[XmlAttribute]
	public int SplitCPSRange { get; set; } = 50;


	[XmlAttribute]
	public double Slope
	{
		get
		{
			return Math.Round(_slope, 6);
		}
		set
		{
			_slope = value;
			OnPropertyChanged("Slope");
		}
	}

	[XmlAttribute]
	public double Offset
	{
		get
		{
			return Math.Round(_offSet, 6);
		}
		set
		{
			_offSet = value;
			OnPropertyChanged("Offset");
		}
	}

	[XmlAttribute]
	public double ZeroRate
	{
		get
		{
			return Math.Round(_zeroRate, 6);
		}
		set
		{
			_zeroRate = value;
			OnPropertyChanged("ZeroRate");
		}
	}

	[XmlAttribute]
	public bool EnabledStage2
	{
		get
		{
			return _enabledStage2;
		}
		set
		{
			_enabledStage2 = value;
			OnPropertyChanged("EnabledStage2");
			if (!value)
			{
				EnabledStage3 = false;
			}
		}
	}

	[XmlAttribute]
	public int Split2
	{
		get
		{
			return _split2;
		}
		set
		{
			_split2 = value;
			OnPropertyChanged("Split2");
		}
	}

	[XmlAttribute]
	public double Slope2
	{
		get
		{
			return Math.Round(_slope2, 6);
		}
		set
		{
			_slope2 = value;
			OnPropertyChanged("Slope2");
		}
	}

	[XmlAttribute]
	public double Offset2
	{
		get
		{
			return Math.Round(_offSet2, 6);
		}
		set
		{
			_offSet2 = value;
			OnPropertyChanged("Offset2");
		}
	}

	[XmlAttribute]
	public bool EnabledStage3
	{
		get
		{
			return _enabledStage3;
		}
		set
		{
			_enabledStage3 = value;
			OnPropertyChanged("EnabledStage3");
		}
	}

	[XmlAttribute]
	public int Split3
	{
		get
		{
			return _split3;
		}
		set
		{
			_split3 = value;
			OnPropertyChanged("Split3");
		}
	}

	[XmlAttribute]
	public double Slope3
	{
		get
		{
			return Math.Round(_slope3, 6);
		}
		set
		{
			_slope3 = value;
			OnPropertyChanged("Slope3");
		}
	}

	[XmlAttribute]
	public double Offset3
	{
		get
		{
			return Math.Round(_offSet3, 6);
		}
		set
		{
			_offSet3 = value;
			OnPropertyChanged("Offset3");
		}
	}

	[XmlAttribute]
	public int DropFrontPoint_Custom
	{
		get
		{
			return _DropFrontPoint_Custom;
		}
		set
		{
			if (_DropFrontPoint_Custom != value && value >= 0 && value <= 100)
			{
				_DropFrontPoint_Custom = value;
				OnPropertyChanged("DropFrontPoint_Custom");
			}
		}
	}

	[XmlAttribute]
	public int DropBehindPoint_Custom
	{
		get
		{
			return _DropBehindPoint_Custom;
		}
		set
		{
			if (_DropBehindPoint_Custom != value && value >= 0 && value <= 100)
			{
				_DropBehindPoint_Custom = value;
				OnPropertyChanged("DropBehindPoint_Custom");
			}
		}
	}

	[XmlAttribute]
	public bool UseCustomDropPoint
	{
		get
		{
			return _UseCustomDropPoint;
		}
		set
		{
			_UseCustomDropPoint = value;
			OnPropertyChanged("UseCustomDropPoint");
		}
	}

	[XmlAttribute]
	public bool UseCustomTempCorrection
	{
		get
		{
			return _UseCustomTempCorrection;
		}
		set
		{
			if (_UseCustomTempCorrection != value)
			{
				_UseCustomTempCorrection = value;
				OnPropertyChanged("UseCustomTempCorrection");
			}
		}
	}

	public TempCorrectionSetting TempCorrectionSetting { get; set; } = new TempCorrectionSetting();


	[XmlAttribute]
	public DateTime LastModifyDate
	{
		get
		{
			return _lastModifyDate;
		}
		set
		{
			_lastModifyDate = value;
			OnPropertyChanged("LastModifyDate");
		}
	}

	[XmlIgnore]
	public List<KBStageInfo> kBStageInfos
	{
		get
		{
			if (_kBStageInfos == null)
			{
				InitKBStageInfo();
			}
			return _kBStageInfos;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	private double GetSlopeOffsetFromCPS(double cps, double? zerorate, out double slope, out double offset, out int stage)
	{
		slope = Slope;
		offset = Offset;
		stage = 1;
		double resCps = cps;
		if (EnableDynamicZeroRate && zerorate.HasValue)
		{
			resCps = cps - (zerorate.Value - ZeroRate);
		}
		if (EnabledStage3 || EnabledStage2)
		{
			if (!EnabledStage3 && EnabledStage2)
			{
				if (resCps < (double)Split2)
				{
					slope = Slope;
					offset = Offset;
				}
				else
				{
					slope = Slope2;
					offset = Offset2;
					stage = 2;
				}
			}
			else if (EnabledStage3 && EnabledStage2)
			{
				if (resCps < (double)Split2)
				{
					slope = Slope;
					offset = Offset;
				}
				else if (resCps < (double)Split3)
				{
					slope = Slope2;
					offset = Offset2;
					stage = 2;
				}
				else
				{
					slope = Slope3;
					offset = Offset3;
					stage = 3;
				}
			}
		}
		return resCps;
	}

	private double GetKBStageInfobyCPS(double cps, double? zerorate, out KBStageInfo kbinfoRes)
	{
		kbinfoRes = kBStageInfos[kBStageInfos.Count - 1];
		double amdCPS = GetAmendCPS(cps, zerorate);
		for (int i = kBStageInfos.Count - 1; i >= 0; i--)
		{
			KBStageInfo kbinfo = kBStageInfos[i];
			if (amdCPS > (double)kbinfo.Min && amdCPS <= (double)kbinfo.Max)
			{
				kbinfoRes = kbinfo;
				break;
			}
		}
		return amdCPS;
	}

	public double GetAmendCPS(double cps, double? zerorate)
	{
		double resCps = cps;
		if (EnableDynamicZeroRate && zerorate.HasValue)
		{
			resCps = cps - (zerorate.Value - ZeroRate);
		}
		return resCps;
	}

	public (double, double) CalH20(double cps, double? zeroRate, double width, double widthSensorsDistance, double height, double heightSensorsDistance, bool showBusDataProxy, double temp, bool calTempCorretcion)
	{
		(double, double) H2OResult = default((double, double));
		KBStageInfo kninfo;
		double useCPS = GetKBStageInfobyCPS(cps, zeroRate, out kninfo);
		double h20 = (ZeroRate - useCPS) / (width / widthSensorsDistance) / (height / heightSensorsDistance) * kninfo.Slope1 + kninfo.Offset1;
		if (kninfo.Slope2.HasValue && kninfo.Offset2.HasValue)
		{
			double weightRight = (useCPS - (double)kninfo.Min) / (double)(kninfo.Max - kninfo.Min);
			double weightLeft = 1.0 - weightRight;
			double h21 = (ZeroRate - useCPS) / (width / widthSensorsDistance) / (height / heightSensorsDistance) * kninfo.Slope2.Value + kninfo.Offset2.Value;
			h20 = h20 * weightLeft + h21 * weightRight;
			if (showBusDataProxy)
			{
				BusDataProxy.Instance.SlopOffset_Debug = $"s:[{weightLeft:F2}*{kninfo.Slope1:F4}+{weightRight:F2}*{kninfo.Slope2:F4}]";
			}
		}
		else if (showBusDataProxy)
		{
			BusDataProxy.Instance.SlopOffset_Debug = $"s:[{kninfo.Slope1:F4}]";
		}
		double cv = 0.0;
		H2OResult.Item1 = h20;
		if (UseCustomTempCorrection && calTempCorretcion)
		{
			if (TempCorrectionSetting.UseStatic)
			{
				foreach (RegressionForTempConfig tc in TempCorrectionSetting.StaticConfigs)
				{
					if (temp > tc.StartTemp && temp <= tc.EndTemp && tc.StaticCorrection.HasValue)
					{
						cv = tc.StaticCorrection.Value;
						break;
					}
				}
			}
			else
			{
				int firstIndex = 0;
				int LastIndex = TempCorrectionSetting.RegressionConfig.Count - 1;
				int index = 0;
				foreach (RegressionForTempConfig tc2 in TempCorrectionSetting.RegressionConfig)
				{
					double starttemp = tc2.StartTemp;
					double endtemp = tc2.EndTemp;
					if (index == firstIndex)
					{
						starttemp = -1000.0;
					}
					if (index == LastIndex)
					{
						endtemp = 1000.0;
					}
					index++;
					if (temp > starttemp && temp <= endtemp)
					{
						double cvv = tc2.Slope * temp + tc2.Offset;
						cv = cvv;
						if (tc2.Offset2.HasValue)
						{
							double? obj = tc2.Slope2 * temp + tc2.Offset2;
							double rate2 = (temp - tc2.StartTemp) / (tc2.EndTemp - tc2.StartTemp);
							double rate1 = 1.0 - rate2;
							cv = (cvv * rate1 + obj * rate2).Value;
						}
						break;
					}
				}
			}
		}
		H2OResult.Item2 = h20 + cv;
		if (showBusDataProxy)
		{
			BusDataProxy.Instance.TempCorrection_Debug = $"{cv:F2}";
		}
		return H2OResult;
	}

	public int GetRecordStageNum(GCSummaryEntity record)
	{
		GetSlopeOffsetFromCPS(record.CPS, record.RealZeroRate, out var _, out var _, out var stage);
		return stage;
	}

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public void CheckUpdate(GCRecipe recipe)
	{
		if (Library.IsPropertychange(this, recipe))
		{
			LastModifyDate = DateTime.Now;
		}
	}

	public void InitKBStageInfo()
	{
		List<KBStageInfo> res = new List<KBStageInfo>();
		if (EnabledStage3)
		{
			res.Add(new KBStageInfo(0, Split2 - SplitCPSRange, Slope, Offset));
			res.Add(new KBStageInfo(Split2 - SplitCPSRange, Split2 + SplitCPSRange, Slope, Offset, Slope2, Offset2));
			res.Add(new KBStageInfo(Split2 + SplitCPSRange, Split3 - SplitCPSRange, Slope2, Offset2));
			res.Add(new KBStageInfo(Split3 - SplitCPSRange, Split3 + SplitCPSRange, Slope2, Offset2, Slope3, Offset3));
			res.Add(new KBStageInfo(Split3 + SplitCPSRange, int.MaxValue, Slope3, Offset3));
		}
		else if (EnabledStage2)
		{
			res.Add(new KBStageInfo(0, Split2 - SplitCPSRange, Slope, Offset));
			res.Add(new KBStageInfo(Split2 - SplitCPSRange, Split2 + SplitCPSRange, Slope, Offset, Slope2, Offset2));
			res.Add(new KBStageInfo(Split2 + SplitCPSRange, int.MaxValue, Slope2, Offset2));
		}
		else
		{
			res.Add(new KBStageInfo(0, int.MaxValue, Slope, Offset));
		}
		_kBStageInfos = res;
	}

	public bool IsParamValid()
	{
		if (DynamicZeroRateCutSecond_Begin < 0 || DynamicZeroRateCutSecond_End < 0 || SplitCPSRange <= 0 || Split2 <= 0 || Split3 <= 0)
		{
			return false;
		}
		InitKBStageInfo();
		int max0 = 0;
		foreach (KBStageInfo kbinfo in kBStageInfos)
		{
			if (kbinfo.Min >= kbinfo.Max)
			{
				return false;
			}
			if (kbinfo.Min < max0)
			{
				return false;
			}
			max0 = kbinfo.Max;
		}
		new List<int>();
		return true;
	}
}
