using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using GCProject.Main.Chart;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Log;
using GCProject.Main.Protocol;
using GCProject.Main.SerialPorts;

namespace GCProject.Main.Lib;

public class BusDataProxy : INotifyPropertyChanged
{
	public bool Is868MMode = true;

	private Timer HolePackTimer;

	private Timer MeasTimeoutTimer;

	public double _sensor1Raw_CPS;

	public double _sensor1Raw_Temp;

	public double _sensor1Raw_Dist;

	public double _sensor2Raw_CPS;

	public double _sensor2Raw_Temp;

	public double _sensor2Raw_Dist;

	public double _sensor3Raw_CPS;

	public double _sensor3Raw_Temp;

	public double _sensor3Raw_Dist;

	public double _sensor4Raw_CPS;

	public double _sensor4Raw_Temp;

	public double _sensor4Raw_Dist;

	public double _sensor5Raw_CPS;

	public double _sensor5Raw_Temp;

	public double _sensor5Raw_Dist;

	private double _a1Value;

	private double _a2Value;

	private double _a3Value;

	private double _a4Value;

	public double LastH20Value;

	private string _SlopOffset_Debug;

	private string _ZeroRate_Debug;

	private string _TempCorrection_Debug;

	private List<double> WithTrigger = new List<double>();

	private object WithTriggerLock = new object();

	private bool widthChangedFlag;

	private double _MR_Total;

	private double H20_Total__;

	public double _dynamicZeroRate;

	private bool _EnableDynamicZeroRate;

	public double? _autoZeroRate;

	private bool _showTempData = true;

	private DateTime? _endTime;

	private DateTime? _startTime;

	private bool _inRun;

	private bool _inMeastimeout;

	private bool _canMeas = true;

	private bool _inSample;

	private double _packageWeight;

	private string _packageNumber;

	private bool _clearNumberWhenFinished;

	private int _sampleNo = -1;

	private volatile bool inSaving;

	private PropertyIntervalCalculater ProCalculator { get; }

	public GCMainChart MainChart { get; set; }

	public GCRecipe SelectedRecipe { get; private set; }

	private ParamSettingConfig Setting => ConfigModel.Instance.ParamSettingConfig;

	public static BusDataProxy Instance { get; private set; }

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor1Raw_CPS
	{
		get
		{
			return _sensor1Raw_CPS;
		}
		set
		{
			if (_sensor1Raw_CPS != value)
			{
				_sensor1Raw_CPS = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor1Raw_CPS");
				}
			}
			ProCalculator.SetValue(value, "Sensor1Raw_CPS");
		}
	}

	[Calculator]
	public double Sensor1Avg_CPS => ProCalculator["Sensor1Raw_CPS"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor1Raw_Temp
	{
		get
		{
			return _sensor1Raw_Temp;
		}
		set
		{
			if (_sensor1Raw_Temp != value)
			{
				_sensor1Raw_Temp = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor1Raw_Temp");
				}
			}
			ProCalculator.SetValue(value, "Sensor1Raw_Temp");
		}
	}

	[Calculator]
	public double Sensor1Avg_Temp => ProCalculator["Sensor1Raw_Temp"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor1Raw_Distance
	{
		get
		{
			return _sensor1Raw_Dist;
		}
		set
		{
			if (_sensor1Raw_Dist != value)
			{
				_sensor1Raw_Dist = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor1Raw_Distance");
				}
			}
			ProCalculator.SetValue(value, "Sensor1Raw_Distance");
		}
	}

	[Calculator]
	public double Sensor1Avg_Distance => ProCalculator["Sensor1Raw_Distance"];

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double Sensor1PackSum
	{
		get
		{
			return ProCalculator.GetValue("Sensor1PackSum");
		}
		set
		{
			ProCalculator.SetValue(value, "Sensor1PackSum");
		}
	}

	[Calculator]
	public bool Sensor1Conn => Sensor1PackSum > 0.0;

	[Calculator(CalType = CalTypeEnum.Recursive, KeepNotify = false)]
	public double Sensor2Raw_CPS
	{
		get
		{
			return _sensor2Raw_CPS;
		}
		set
		{
			if (_sensor2Raw_CPS != value)
			{
				_sensor2Raw_CPS = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor2Raw_CPS");
				}
			}
			ProCalculator.SetValue(value, "Sensor2Raw_CPS");
		}
	}

	[Calculator]
	public double Sensor2Avg_CPS => ProCalculator["Sensor2Raw_CPS"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor2Raw_Temp
	{
		get
		{
			return _sensor2Raw_Temp;
		}
		set
		{
			if (_sensor2Raw_Temp != value)
			{
				_sensor2Raw_Temp = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor2Raw_Temp");
				}
			}
			ProCalculator.SetValue(value, "Sensor2Raw_Temp");
		}
	}

	[Calculator]
	public double Sensor2Avg_Temp => ProCalculator["Sensor2Raw_Temp"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor2Raw_Distance
	{
		get
		{
			return _sensor2Raw_Dist;
		}
		set
		{
			if (_sensor2Raw_Dist != value)
			{
				_sensor2Raw_Dist = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor2Raw_Distance");
				}
			}
			ProCalculator.SetValue(value, "Sensor2Raw_Distance");
		}
	}

	[Calculator]
	public double Sensor2Avg_Distance => ProCalculator["Sensor2Raw_Distance"];

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double Sensor2PackSum
	{
		get
		{
			double value = ProCalculator.GetValue("Sensor2PackSum");
			_ = 6.0;
			return value;
		}
		set
		{
			ProCalculator.SetValue(value, "Sensor2PackSum");
		}
	}

	[Calculator]
	public bool Sensor2Conn => Sensor2PackSum > 0.0;

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor3Raw_CPS
	{
		get
		{
			return _sensor3Raw_CPS;
		}
		set
		{
			if (_sensor3Raw_CPS != value)
			{
				_sensor3Raw_CPS = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor3Raw_CPS");
				}
			}
			ProCalculator.SetValue(value, "Sensor3Raw_CPS");
		}
	}

	[Calculator]
	public double Sensor3Avg_CPS => ProCalculator["Sensor3Raw_CPS"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor3Raw_Temp
	{
		get
		{
			return _sensor3Raw_Temp;
		}
		set
		{
			if (_sensor3Raw_Temp != value)
			{
				_sensor3Raw_Temp = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor3Raw_Temp");
				}
			}
			ProCalculator.SetValue(value, "Sensor3Raw_Temp");
		}
	}

	[Calculator]
	public double Sensor3Avg_Temp => ProCalculator["Sensor3Raw_Temp"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor3Raw_Distance
	{
		get
		{
			return _sensor3Raw_Dist;
		}
		set
		{
			if (_sensor3Raw_Dist != value)
			{
				_sensor3Raw_Dist = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor3Raw_Distance");
				}
			}
			ProCalculator.SetValue(value, "Sensor3Raw_Distance");
		}
	}

	[Calculator]
	public double Sensor3Avg_Distance => ProCalculator["Sensor3Raw_Distance"];

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double Sensor3PackSum
	{
		get
		{
			double value = ProCalculator.GetValue("Sensor3PackSum");
			_ = 6.0;
			return value;
		}
		set
		{
			ProCalculator.SetValue(value, "Sensor3PackSum");
		}
	}

	[Calculator]
	public bool Sensor3Conn => Sensor3PackSum > 0.0;

	[Calculator(CalType = CalTypeEnum.Recursive, KeepNotify = false)]
	public double Sensor4Raw_CPS
	{
		get
		{
			return _sensor4Raw_CPS;
		}
		set
		{
			if (_sensor4Raw_CPS != value)
			{
				_sensor4Raw_CPS = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor4Raw_CPS");
				}
			}
			ProCalculator.SetValue(value, "Sensor4Raw_CPS");
		}
	}

	[Calculator]
	public double Sensor4Avg_CPS => ProCalculator["Sensor4Raw_CPS"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor4Raw_Temp
	{
		get
		{
			return _sensor4Raw_Temp;
		}
		set
		{
			if (_sensor4Raw_Temp != value)
			{
				_sensor4Raw_Temp = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor4Raw_Temp");
				}
			}
			ProCalculator.SetValue(value, "Sensor4Raw_Temp");
		}
	}

	[Calculator]
	public double Sensor4Avg_Temp => ProCalculator["Sensor4Raw_Temp"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor4Raw_Distance
	{
		get
		{
			return _sensor4Raw_Dist;
		}
		set
		{
			if (_sensor4Raw_Dist != value)
			{
				_sensor4Raw_Dist = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor4Raw_Distance");
				}
			}
			ProCalculator.SetValue(value, "Sensor4Raw_Distance");
		}
	}

	[Calculator]
	public double Sensor4Avg_Distance => ProCalculator["Sensor4Raw_Distance"];

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double Sensor4PackSum
	{
		get
		{
			double value = ProCalculator.GetValue("Sensor4PackSum");
			_ = 6.0;
			return value;
		}
		set
		{
			ProCalculator.SetValue(value, "Sensor4PackSum");
		}
	}

	[Calculator]
	public bool Sensor4Conn => Sensor4PackSum > 0.0;

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor5Raw_CPS
	{
		get
		{
			return _sensor5Raw_CPS;
		}
		set
		{
			if (_sensor5Raw_CPS != value)
			{
				_sensor5Raw_CPS = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor5Raw_CPS");
				}
			}
			ProCalculator.SetValue(value, "Sensor5Raw_CPS");
		}
	}

	[Calculator]
	public double Sensor5Avg_CPS => ProCalculator["Sensor5Raw_CPS"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor5Raw_Temp
	{
		get
		{
			return _sensor5Raw_Temp;
		}
		set
		{
			if (_sensor5Raw_Temp != value)
			{
				_sensor5Raw_Temp = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor5Raw_Temp");
				}
			}
			ProCalculator.SetValue(value, "Sensor5Raw_Temp");
		}
	}

	[Calculator]
	public double Sensor5Avg_Temp => ProCalculator["Sensor5Raw_Temp"];

	[Calculator(CalType = CalTypeEnum.Average_1s, KeepNotify = false)]
	public double Sensor5Raw_Distance
	{
		get
		{
			return _sensor5Raw_Dist;
		}
		set
		{
			if (_sensor5Raw_Dist != value)
			{
				_sensor5Raw_Dist = value;
				if (ShowFashRawData)
				{
					OnPropertyChanged("Sensor5Raw_Distance");
				}
			}
			ProCalculator.SetValue(value, "Sensor5Raw_Distance");
		}
	}

	[Calculator]
	public double Sensor5Avg_Distance => ProCalculator["Sensor5Raw_Distance"];

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double Sensor5PackSum
	{
		get
		{
			double value = ProCalculator.GetValue("Sensor5PackSum");
			_ = 6.0;
			return value;
		}
		set
		{
			ProCalculator.SetValue(value, "Sensor5PackSum");
		}
	}

	[Calculator]
	public bool Sensor5Conn => Sensor5PackSum > 0.0;

	public double A1Value
	{
		get
		{
			return _a1Value;
		}
		set
		{
			if (_a1Value != value)
			{
				_a1Value = value;
				OnPropertyChanged("A1Value");
			}
			ProCalculator["A1Value_Avg"] = value;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_1s)]
	public double A1Value_Avg => ProCalculator.GetValue("A1Value_Avg");

	public double A2Value
	{
		get
		{
			return _a2Value;
		}
		set
		{
			if (_a2Value != value)
			{
				_a2Value = value;
				OnPropertyChanged("A2Value");
			}
			ProCalculator["A2Value_Avg"] = value;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_1s)]
	public double A2Value_Avg => ProCalculator.GetValue("A2Value_Avg");

	public double A3Value
	{
		get
		{
			return _a3Value;
		}
		set
		{
			if (_a3Value != value)
			{
				_a3Value = value;
				OnPropertyChanged("A3Value");
			}
			ProCalculator["A3Value_Avg"] = value;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_1s)]
	public double A3Value_Avg => ProCalculator.GetValue("A3Value_Avg");

	public double A4Value
	{
		get
		{
			return _a4Value;
		}
		set
		{
			if (_a4Value != value)
			{
				_a4Value = value;
				OnPropertyChanged("A4Value");
			}
			ProCalculator["A4Value_Avg"] = value;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_1s)]
	public double A4Value_Avg => ProCalculator.GetValue("A4Value_Avg");

	[Calculator(CalType = CalTypeEnum.Sum_1s)]
	public double BoradPackSum
	{
		get
		{
			return ProCalculator.GetValue("BoradPackSum");
		}
		set
		{
			ProCalculator["BoradPackSum"] = value;
		}
	}

	[Calculator]
	public bool BoardConn => BoradPackSum > 0.0;

	public int H20ToElec_Real => BaseConveter.ConvertH20ToElec(H20_Real__, Setting.H20ToElecRangeMin_868M, Setting.H20ToElecRangeMax_868M);

	public int H20ToElec_Total => BaseConveter.ConvertH20ToElec(LastH20Value, Setting.H20ToElecRangeMin_868M, Setting.H20ToElecRangeMax_868M);

	public DisplayByte2bit Board_Input { get; private set; } = new DisplayByte2bit();


	public DisplayByte2bit Board_Output { get; private set; } = new DisplayByte2bit();


	public string SlopOffset_Debug
	{
		get
		{
			return _SlopOffset_Debug;
		}
		set
		{
			if (_SlopOffset_Debug != value)
			{
				_SlopOffset_Debug = value;
				OnPropertyChanged("SlopOffset_Debug");
			}
		}
	}

	public string ZeroRate_Debug
	{
		get
		{
			return _ZeroRate_Debug;
		}
		set
		{
			if (_ZeroRate_Debug != value)
			{
				_ZeroRate_Debug = value;
				OnPropertyChanged("ZeroRate_Debug");
			}
		}
	}

	public string TempCorrection_Debug
	{
		get
		{
			return _TempCorrection_Debug;
		}
		set
		{
			if (_TempCorrection_Debug != value)
			{
				_TempCorrection_Debug = value;
				OnPropertyChanged("TempCorrection_Debug");
			}
		}
	}

	public double CPS_Real
	{
		get
		{
			double CPS_real = (Is868MMode ? Sensor2Raw_CPS : Sensor4Raw_CPS);
			return GetValidCPS(CPS_real);
		}
	}

	public double CPS_1s__ { get; private set; }

	[Calculator]
	public double CPS_1s
	{
		get
		{
			double res = (Is868MMode ? Sensor2Avg_CPS : Sensor4Avg_CPS);
			CPS_1s__ = GetValidCPS(res);
			return CPS_1s__;
		}
	}

	public double CPS_WithTemp_1s__ { get; private set; }

	[Calculator]
	public double CPS_WithTemp_1s
	{
		get
		{
			CPS_WithTemp_1s__ = RoundValue(CPS_1s__ * (Setting.TempCorrectionParam * (Temp__ - Setting.TempCorrectionValue) + 1.0));
			return CPS_WithTemp_1s__;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_Total, UpdateImmediately = true)]
	public double CPS_Total => ProCalculator.GetValue("CPS_Total");

	public double Height_Real => RoundValue(GetValidHeight(A4Value));

	public double Height_1s__ { get; private set; }

	[Calculator]
	public double Height_1s
	{
		get
		{
			if (!GCSerialPortManager.BoardSerialPortConn)
			{
				return Height_1s__;
			}
			Height_1s__ = RoundValue(GetValidHeight(A4Value_Avg));
			return Height_1s__;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_Total, KeepNotify = false, UpdateImmediately = true)]
	public double Height_Total => ProCalculator.GetValue("Height_Total");

	public double Width_Real
	{
		get
		{
			double ResultWidth = (Is868MMode ? GetValidWidth(Sensor2Raw_Distance, Sensor3Raw_Distance) : GetValidWidth(Sensor4Raw_Distance, Sensor5Raw_Distance));
			return RoundValue(ResultWidth);
		}
	}

	public double Width_1s__ { get; private set; }

	[Calculator]
	public double Width_1s
	{
		get
		{
			if (!GCSerialPortManager.BusSerialPortConn)
			{
				return Width_1s__;
			}
			double ResultWidth = (Is868MMode ? GetValidWidth(Sensor2Avg_Distance, Sensor3Avg_Distance) : GetValidWidth(Sensor4Avg_Distance, Sensor5Avg_Distance));
			Width_1s__ = RoundValue(ResultWidth);
			return Width_1s__;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_Total, KeepNotify = false, UpdateImmediately = true)]
	public double Width_Total => ProCalculator.GetValue("Width_Total");

	public double H20_Real__ { get; private set; }

	public double H20_Real_Draw
	{
		get
		{
			if (InSkipPoint || InHolepack)
			{
				return H20_Real__;
			}
			double useCPS = ((Setting.DrawChartType == DrawChartTypeEnum.RealWave) ? GetRealCPSWithtemp() : CPS_Total);
			H20_Real__ = GetValidH20(SelectedRecipe.CalH20(useCPS, AutoZeroRate, Width_Total, Setting.WidthSensorsDistance, Height_Total, Setting.HeightSensorsDistance, showBusDataProxy: true, Temp__, calTempCorretcion: true).Item2);
			return H20_Real__;
		}
	}

	[Calculator]
	public double H20_Total
	{
		get
		{
			double res = GetTotalH20();
			if (!double.IsNaN(res))
			{
				H20_Total__ = res;
			}
			MR_Total = CalMRValue(H20_Total__);
			return H20_Total__;
		}
	}

	public double MR_Total
	{
		get
		{
			return _MR_Total;
		}
		set
		{
			if (_MR_Total != value)
			{
				_MR_Total = value;
				OnPropertyChanged("MR_Total");
			}
		}
	}

	[Calculator]
	public bool? H20_Alarm => GetAlarmStatus(H20_Total__, MR_Total);

	[Calculator(CalType = CalTypeEnum.Recursive, KeepNotify = false, UpdateImmediately = true)]
	public double DynamicZeroRate
	{
		get
		{
			return _dynamicZeroRate;
		}
		set
		{
			if (_dynamicZeroRate != value)
			{
				_dynamicZeroRate = value;
				OnPropertyChanged("DynamicZeroRate");
				DynamicZeroRateDisplay = value;
				CalculatorAttribute caller = ProCalculator.GetCalculatorItem("DynamicZeroRate");
				DynamicZeroRateCount = caller.DataCount;
			}
		}
	}

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

	[Calculator(CalType = CalTypeEnum.None)]
	public double DynamicZeroRateDisplay { get; set; }

	[Calculator(CalType = CalTypeEnum.None)]
	public int DynamicZeroRateCount { get; set; }

	private double DynamicZeroRateSkip_End { get; set; }

	public double AutoZeroRate
	{
		get
		{
			if (!_autoZeroRate.HasValue)
			{
				_autoZeroRate = (SelectedRecipe.EnableDynamicZeroRate ? DynamicZeroRate : SelectedRecipe.ZeroRate);
			}
			ZeroRate_Debug = $"z:[{_autoZeroRate:F2}]";
			return _autoZeroRate.Value;
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_Total, KeepNotify = false, UpdateImmediately = true)]
	public double CPS_Current
	{
		get
		{
			double value = ProCalculator.GetValue("CPS_Current");
			ProCalculator.ClearValue("CPS_Current");
			return value;
		}
	}

	[Calculator]
	public double Temp1
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToTemp(Sensor4Avg_Temp);
			}
			return BaseConveter.ConvertToTemp(Sensor2Avg_Temp);
		}
	}

	[Calculator]
	public double Temp2
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToTemp(Sensor5Avg_Temp);
			}
			return BaseConveter.ConvertToTemp(Sensor3Avg_Temp);
		}
	}

	public double Temp__ { get; private set; }

	[Calculator]
	public double Temp
	{
		get
		{
			double res = 1.0;
			ShowTempData = true;
			switch (Setting.TempCorrectionType)
			{
			case TempCorrectionTypes.Only1:
				res = Temp1;
				break;
			case TempCorrectionTypes.Only2:
				res = Temp2;
				break;
			case TempCorrectionTypes.Average12:
				res = Math.Round((Temp1 + Temp2) / 2.0, 2);
				break;
			case TempCorrectionTypes.Board:
				res = CalBoardTemp(A3Value_Avg);
				break;
			default:
				ShowTempData = false;
				break;
			}
			Temp__ = res;
			ProCalculator["TempAvg"] = Temp__;
			return Temp__;
		}
	}

	public bool ShowTempData
	{
		get
		{
			return _showTempData;
		}
		set
		{
			if (_showTempData != value)
			{
				_showTempData = value;
				OnPropertyChanged("ShowTempData");
			}
		}
	}

	[Calculator(CalType = CalTypeEnum.Average_Total, UpdateImmediately = true, PriorityLevel = 1)]
	public double TempAvg => ProCalculator.GetValue("TempAvg");

	[Calculator]
	public double HeightElec => BaseConveter.ConvertToHeightElec(A4Value_Avg);

	[Calculator]
	public double Width1Elec
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToElec(Sensor4Avg_Distance);
			}
			return BaseConveter.ConvertToElec(Sensor2Avg_Distance);
		}
	}

	[Calculator]
	public double Width2Elec
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToElec(Sensor5Avg_Distance);
			}
			return BaseConveter.ConvertToElec(Sensor3Avg_Distance);
		}
	}

	[Calculator]
	public double Temp1Elec
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToElec(Sensor4Avg_Temp);
			}
			return BaseConveter.ConvertToElec(Sensor2Avg_Temp);
		}
	}

	[Calculator]
	public double Temp2Elec
	{
		get
		{
			if (!Is868MMode)
			{
				return BaseConveter.ConvertToElec(Sensor5Avg_Temp);
			}
			return BaseConveter.ConvertToElec(Sensor3Avg_Temp);
		}
	}

	[Calculator]
	public bool BusSuccess
	{
		get
		{
			if (!Sensor1Conn && !Sensor2Conn && !Sensor3Conn && !Sensor4Conn && !Sensor5Conn)
			{
				return BoardConn;
			}
			return true;
		}
	}

	private DateTime CurrentTime__ { get; set; }

	[Calculator(PriorityLevel = 1, UpdateImmediately = true)]
	public DateTime CurrentTime
	{
		get
		{
			CurrentTime__ = DateTime.Now;
			return CurrentTime__;
		}
	}

	[Calculator(PriorityLevel = 1, KeepNotify = true)]
	public int DurationTime
	{
		get
		{
			if (!StartTime.HasValue || !StartTime.HasValue || !EndTime.HasValue || !EndTime.HasValue)
			{
				return 0;
			}
			int res = (int)Math.Round((EndTime - StartTime).Value.TotalSeconds);
			if (res <= 0)
			{
				return 0;
			}
			return res;
		}
	}

	[Calculator(PriorityLevel = 1, UpdateImmediately = true)]
	public DateTime? EndTime
	{
		get
		{
			if (InMeastimeout)
			{
				return _endTime;
			}
			if (InRun && !InSkipPoint)
			{
				return CurrentTime__;
			}
			return null;
		}
		set
		{
			_endTime = value;
		}
	}

	public DateTime? StartTime
	{
		get
		{
			if (!InRun || InSkipPoint)
			{
				return null;
			}
			return _startTime;
		}
		set
		{
			if (_startTime != value)
			{
				_startTime = value;
				OnPropertyChanged("StartTime");
			}
		}
	}

	public bool InRun
	{
		get
		{
			return _inRun;
		}
		set
		{
			if (_inRun == value)
			{
				return;
			}
			if (!CanMeas)
			{
				_inRun = false;
				OnPropertyChanged("InRun");
				return;
			}
			_inRun = value;
			OnPropertyChanged("InRun");
			if (value)
			{
				CutDynamicZeroRat();
				GCSerialPortManager.StartSendH20Alarm(Start: false);
				ClearOutputDelay();
				InSkipPoint = true;
				InMeastimeout = false;
				InHolepack = false;
				SkipCountDown = GetSkipPoint();
				_autoZeroRate = null;
				GCLogger.log.Info($"Start Skip {SkipCountDown} Point");
				ClearCurrentTotal();
				MainChart?.ClearAll();
			}
			else
			{
				SetDynamicZeroRateSkipEnd();
				MeasTimeoutTimer.Change(-1, -1);
				MainChart.DropPointAndReDraw(GetDropPoint());
			}
			SetCPSToRecursiveModel(!value);
		}
	}

	public int SkipCountDown { get; private set; } = 100;


	public bool InSkipPoint { get; set; } = true;


	public bool InMeastimeout
	{
		get
		{
			return _inMeastimeout;
		}
		set
		{
			_inMeastimeout = value;
			if (_inMeastimeout)
			{
				MeasTimeoutTimer.Change(-1, -1);
				EndTime = DateTime.Now;
			}
		}
	}

	public bool InHolepack { get; set; }

	public bool CanMeas
	{
		get
		{
			return _canMeas;
		}
		set
		{
			if (_canMeas != value)
			{
				_canMeas = value;
				InRun = false;
				if (!_canMeas)
				{
					ClearCurrentTotal();
					MainChart?.ClearAll();
				}
				OnPropertyChanged("CanMeas");
			}
		}
	}

	public bool InSample
	{
		get
		{
			return _inSample;
		}
		set
		{
			_inSample = value;
			InRun = false;
		}
	}

	public bool ShowFashRawData { get; set; }

	public bool DropDataRequest
	{
		set
		{
			if (value)
			{
				InSample = false;
			}
		}
	}

	public double PackageWeight
	{
		get
		{
			return _packageWeight;
		}
		set
		{
			if (_packageWeight != value)
			{
				_packageWeight = value;
				OnPropertyChanged("PackageWeight");
			}
		}
	}

	public string PackageNumber
	{
		get
		{
			return _packageNumber;
		}
		set
		{
			if (_packageNumber != value)
			{
				_packageNumber = value;
				OnPropertyChanged("PackageNumber");
			}
		}
	}

	public bool ClearNumberWhenFinished
	{
		get
		{
			return _clearNumberWhenFinished;
		}
		set
		{
			if (_clearNumberWhenFinished != value)
			{
				_clearNumberWhenFinished = value;
				OnPropertyChanged("ClearNumberWhenFinished");
			}
		}
	}

	public int SampleNo
	{
		get
		{
			return _sampleNo;
		}
		set
		{
			if (_sampleNo != value)
			{
				_sampleNo = value;
				OnPropertyChanged("SampleNo");
			}
		}
	}

	public RawDataCollection RawDatas { get; private set; } = new RawDataCollection();


	public event PropertyChangedEventHandler PropertyChanged;

	public event Action SaveDataFinished;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public BusDataProxy()
	{
		Is868MMode = Setting.RunMode == RunModeEnum.Mode_868M;
		ProCalculator = new PropertyIntervalCalculater(GetType(), OnPropertyChanged);
		MeasTimeoutTimer = new Timer(delegate
		{
			InMeastimeout = true;
			if (InSample)
			{
				PerformSaveData();
			}
			else
			{
				GCLogger.log.Info("drop data for meas timeout and not in sample");
			}
			InRun = false;
		}, null, -1, -1);
		HolePackTimer = new Timer(delegate
		{
			InMeastimeout = true;
			InHolepack = false;
			PerformSaveData();
			InRun = false;
		}, null, -1, -1);
		SetCPSToRecursiveModel();
		LastH20Value = GetLastH20Value();
		Instance = this;
	}

	private double GetLastH20Value()
	{
		using GCContext db = new GCContext();
		try
		{
			if (!db.GCSummaryEntities.Any((GCSummaryEntity g) => !g.IsSampleData))
			{
				return 0.0;
			}
			long id = db.GCSummaryEntities.Where((GCSummaryEntity g) => !g.IsSampleData).Max((GCSummaryEntity g) => g.GCSummaryEntityID);
			return db.GCSummaryEntities.Find(id).H20;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	public void ReadData(IProtocol Protocol)
	{
		if (Protocol is GCProtocolRecv1To5 pack)
		{
			if (pack.ProtocolNum == 1)
			{
				ReadPro1(pack);
			}
			else if (pack.ProtocolNum == 2)
			{
				ReadPro2(pack);
			}
			else if (pack.ProtocolNum == 3)
			{
				ReadPro3(pack);
			}
			else if (pack.ProtocolNum == 4)
			{
				ReadPro4(pack);
			}
			else if (pack.ProtocolNum == 5)
			{
				ReadPro5(pack);
			}
		}
		else if (Protocol is GCProtocolBoardRecv boardRecv)
		{
			ReadBoardData(boardRecv);
		}
	}

	private void ReadPro1(object prol)
	{
		GCProtocolRecv1To5 Protocol = prol as GCProtocolRecv1To5;
		Sensor1Raw_CPS = Protocol.MircroValue;
		Sensor1Raw_Temp = Protocol.TempValue;
		Sensor1Raw_Distance = Protocol.DistanceValue;
		Sensor1PackSum = 1.0;
	}

	private void ReadPro2(object prol)
	{
		GCProtocolRecv1To5 Protocol = prol as GCProtocolRecv1To5;
		Sensor2Raw_CPS = Protocol.MircroValue;
		Sensor2Raw_Temp = Protocol.TempValue;
		Sensor2Raw_Distance = Protocol.DistanceValue;
		Sensor2PackSum = 1.0;
		if (Is868MMode)
		{
			SaveOneRawData();
		}
	}

	private void ReadPro3(object prol)
	{
		GCProtocolRecv1To5 Protocol = prol as GCProtocolRecv1To5;
		Sensor3Raw_CPS = Protocol.MircroValue;
		Sensor3Raw_Temp = Protocol.TempValue;
		Sensor3Raw_Distance = Protocol.DistanceValue;
		Sensor3PackSum = 1.0;
	}

	private void ReadPro4(object prol)
	{
		GCProtocolRecv1To5 Protocol = prol as GCProtocolRecv1To5;
		Sensor4Raw_CPS = Math.Round(Protocol.MircroValue * Setting.CPS24GAmendmentParam, 0);
		Sensor4Raw_Temp = Protocol.TempValue;
		Sensor4Raw_Distance = Protocol.DistanceValue;
		Sensor4PackSum = 1.0;
		if (!Is868MMode)
		{
			SaveOneRawData();
		}
	}

	private void ReadPro5(object prol)
	{
		GCProtocolRecv1To5 Protocol = prol as GCProtocolRecv1To5;
		Sensor5Raw_CPS = Math.Round(Protocol.MircroValue * Setting.CPS24GAmendmentParam, 0);
		Sensor5Raw_Temp = Protocol.TempValue;
		Sensor5Raw_Distance = Protocol.DistanceValue;
		Sensor5PackSum = 1.0;
	}

	public byte[] GetCurrentBoradSendProtocol()
	{
		ushort Current = (ushort)H20ToElec_Real;
		ushort Total = (ushort)H20ToElec_Total;
		return new GCProtocolBoardSend
		{
			A1Value = Total,
			A2Value = Current,
			Output4 = Board_Output.Value
		}.ToBytes();
	}

	public void ReadBoardData(GCProtocolBoardRecv pro)
	{
		A1Value = pro.A1Value;
		A2Value = pro.A2Value;
		A3Value = pro.A3Value;
		A4Value = pro.A4Value;
		Board_Input.SetBits(pro.Input);
		ResponseDelayData();
		BoradPackSum = 1.0;
	}

	public void ClearOutputDelay()
	{
		GCLogger.log.Info("Empty Output relay state");
		Board_Output.Clear();
	}

	private void ResponseDelayData()
	{
		bool clearDelayFlag = false;
		if (Board_Input.Bit7 == 0)
		{
			Board_Output.Bit7 = 0;
			if (!clearDelayFlag && IsAlarmDelay(7))
			{
				clearDelayFlag = true;
			}
		}
		if (Board_Input.Bit6 == 0)
		{
			Board_Output.Bit6 = 0;
			if (!clearDelayFlag && IsAlarmDelay(6))
			{
				clearDelayFlag = true;
			}
		}
		if (Board_Input.Bit5 == 0)
		{
			Board_Output.Bit5 = 0;
			if (!clearDelayFlag && IsAlarmDelay(5))
			{
				clearDelayFlag = true;
			}
		}
		if (Board_Input.Bit4 == 0)
		{
			Board_Output.Bit4 = 0;
			if (!clearDelayFlag && IsAlarmDelay(4))
			{
				clearDelayFlag = true;
			}
		}
		Board_Output.UpdateValue();
		if (clearDelayFlag)
		{
			GCSerialPortManager.StartSendH20Alarm(Start: false);
		}
	}

	public void CheckH20ResultForDelayAlarm(bool sendToPCL, bool? alarmStatus)
	{
		if (sendToPCL)
		{
			GCSerialPortManager.StartSendH20Alarm(alarmStatus.HasValue);
		}
		else
		{
			GCSerialPortManager.StartSendH20Alarm(Start: false);
		}
		if (alarmStatus.HasValue)
		{
			SetAllDelay(alarmStatus.Value, !alarmStatus.Value, finished: true);
		}
		else
		{
			SetAllDelay(highAlarm: false, lowAlarm: false, finished: true);
		}
	}

	private void SetBoard_OutputDelayBit(int index0_3, bool status)
	{
		switch (index0_3)
		{
		case 1:
			Board_Output.Bit7 = (status ? 1 : 0);
			break;
		case 2:
			Board_Output.Bit6 = (status ? 1 : 0);
			break;
		case 3:
			Board_Output.Bit5 = (status ? 1 : 0);
			break;
		case 4:
			Board_Output.Bit4 = (status ? 1 : 0);
			break;
		}
		Board_Output.UpdateValue();
	}

	public void SetAllDelay(bool highAlarm, bool lowAlarm, bool finished)
	{
		int highBit = ConfigModel.Instance.ParamSettingConfig.HighH20AlarmDigital;
		int lowBit = ConfigModel.Instance.ParamSettingConfig.LowH20AlarmDigital;
		int finishedBit = ConfigModel.Instance.ParamSettingConfig.MeasFinishedDigital;
		SetBoard_OutputDelayBit(highBit, highAlarm);
		SetBoard_OutputDelayBit(lowBit, lowAlarm);
		SetBoard_OutputDelayBit(finishedBit, finished);
		GCLogger.log.Info($"Set relay [HighAlarm(bit{highBit}):{(highAlarm ? 1 : 0)}],[LowAlarm(bit{lowBit}):{(lowAlarm ? 1 : 0)}],[Finish(bit{finishedBit}):{(finished ? 1 : 0)}]");
	}

	public bool IsAlarmDelay(int bit)
	{
		int num = 0;
		switch (bit)
		{
		case 7:
			num = 1;
			break;
		case 6:
			num = 2;
			break;
		case 5:
			num = 3;
			break;
		case 4:
			num = 4;
			break;
		}
		if (num != ConfigModel.Instance.ParamSettingConfig.HighH20AlarmDigital)
		{
			return num == ConfigModel.Instance.ParamSettingConfig.LowH20AlarmDigital;
		}
		return true;
	}

	private double GetValidCPS(double value)
	{
		if (value < Setting.CPSMeasRangeMin_868M)
		{
			value = Setting.CPSMeasRangeMin_868M;
		}
		if (value > Setting.CPSMeasRangeMax_868M)
		{
			value = Setting.CPSMeasRangeMax_868M;
		}
		return value;
	}

	private void AddRealCPS(double value)
	{
		double cps = value * (Setting.TempCorrectionParam * (Temp__ - Setting.TempCorrectionValue) + 1.0);
		ProCalculator["CPS_Total"] = cps;
		ProCalculator["CPS_Current"] = cps;
	}

	private double GetRealCPSWithtemp()
	{
		return CPS_Real * (Setting.TempCorrectionParam * (Temp__ - Setting.TempCorrectionValue) + 1.0);
	}

	public void SetCPSToRecursiveModel(bool SetRecursive = true)
	{
		ProCalculator.GetCalculatorItem("Sensor2Raw_CPS").InRecursive = SetRecursive;
		ProCalculator.GetCalculatorItem("Sensor4Raw_CPS").InRecursive = SetRecursive;
	}

	private double GetValidHeight(double rawHeight)
	{
		double Height1 = BaseConveter.ConvertToHeight(rawHeight);
		if (Height1 < 0.0)
		{
			Height1 = 0.0;
		}
		double Result = Setting.HeightSensorsDistance - Height1;
		if (Result < 0.0)
		{
			Result = 0.0;
		}
		return Result;
	}

	private void AddRealHeight(double value)
	{
		if (Setting.HeightCorrectionEnabled)
		{
			ProCalculator["Height_Total"] = value;
		}
		else
		{
			ProCalculator["Height_Total"] = Setting.HeightSensorsDistance;
		}
	}

	private double GetValidWidth(double rawWidth1, double rawWidth2)
	{
		bool setZeroWidth1;
		double width1 = BaseConveter.ConvertToWidth(rawWidth1, 1, out setZeroWidth1);
		bool setZeroWidth2;
		double width2 = BaseConveter.ConvertToWidth(rawWidth2, 2, out setZeroWidth2);
		if (setZeroWidth1 || setZeroWidth2)
		{
			return 0.0;
		}
		if (width1 < 0.0)
		{
			width1 = 0.0;
		}
		if (width2 < 0.0)
		{
			width2 = 0.0;
		}
		double ResultWidth = Setting.WidthSensorsDistance - width1 - width2;
		if (ResultWidth < 0.0)
		{
			ResultWidth = 0.0;
		}
		return ResultWidth;
	}

	private void AddRealWidth(double value)
	{
		if (Setting.WidthCorrectionEnabled)
		{
			ProCalculator["Width_Total"] = value;
		}
		else
		{
			ProCalculator["Width_Total"] = Setting.WidthSensorsDistance;
		}
	}

	private void UpdateTriggerWidth(double realWith)
	{
		lock (WithTriggerLock)
		{
			WithTrigger.Add(realWith);
			if (WithTrigger.Count > 8)
			{
				WithTrigger.RemoveAt(0);
			}
			double ResultWidth = WithTrigger.Average();
			if (_inRun)
			{
				if (ResultWidth < Setting.ValidMeasWidthMin || ResultWidth > Setting.ValidMeasWidthMax)
				{
					StartHoleCountDown();
				}
				else
				{
					CloseHoleCountDown();
				}
			}
			else if (ResultWidth > Setting.ValidMeasWidthMin && ResultWidth < Setting.ValidMeasWidthMax)
			{
				if (!InMeastimeout)
				{
					CloseHoleCountDown();
				}
				else if (InMeastimeout && widthChangedFlag)
				{
					widthChangedFlag = false;
					CloseHoleCountDown();
				}
			}
			else if (InMeastimeout)
			{
				widthChangedFlag = true;
			}
		}
	}

	private double GetValidH20(double value)
	{
		if (double.IsPositiveInfinity(value))
		{
			value = Setting.H20MeasRangeMax_868M;
		}
		else if (double.IsNegativeInfinity(value))
		{
			value = Setting.H20MeasRangeMin_868M;
		}
		if (value < Setting.H20MeasRangeMin_868M)
		{
			value = Setting.H20MeasRangeMin_868M;
		}
		if (value > Setting.H20MeasRangeMax_868M)
		{
			value = Setting.H20MeasRangeMax_868M;
		}
		return RoundValue(value);
	}

	private double CalMRValue(double h2o)
	{
		if (h2o >= 100.0)
		{
			return 100.0;
		}
		return h2o / (100.0 - h2o) * 100.0;
	}

	private bool? GetAlarmStatus(double h2o, double? mr = null)
	{
		double valueOrDefault = mr.GetValueOrDefault();
		if (!mr.HasValue)
		{
			valueOrDefault = CalMRValue(h2o);
			mr = valueOrDefault;
		}
		if (Setting.DisplayMR)
		{
			if (mr > Setting.MRAlarmRangeMax)
			{
				return true;
			}
			if (mr < Setting.MRAlarmRangeMin)
			{
				return false;
			}
			return null;
		}
		if (h2o > Setting.H20AlarmRangeMax_868M)
		{
			return true;
		}
		if (h2o < Setting.H20AlarmRangeMin_868M)
		{
			return false;
		}
		return null;
	}

	private void SetDynamicZeroRateSkipEnd()
	{
		if (SelectedRecipe.EnableDynamicZeroRate)
		{
			int drop = SelectedRecipe.DynamicZeroRateCutSecond_End * 8;
			if (drop < 0)
			{
				drop = 0;
			}
			DynamicZeroRateSkip_End = drop;
		}
	}

	private void AddDynamicZeroRate(double value)
	{
		if (DynamicZeroRateSkip_End > 0.0)
		{
			DynamicZeroRateSkip_End--;
		}
		else
		{
			DynamicZeroRate = ProCalculator.SetValueWithReturn(value, "DynamicZeroRate");
		}
	}

	private void CutDynamicZeroRat()
	{
		if (SelectedRecipe.EnableDynamicZeroRate)
		{
			int drop_s = SelectedRecipe.DynamicZeroRateCutSecond_Begin;
			if (drop_s < 0)
			{
				drop_s = 4;
			}
			CalculatorAttribute caller = ProCalculator.GetCalculatorItem("DynamicZeroRate");
			caller.DropPoint(drop_s * 8);
			DynamicZeroRate = caller.GetResult();
		}
	}

	private double GetTotalH20()
	{
		double res = ((Setting.TotalH2OType == TotalH2OCalculateType.TotalCPS) ? SelectedRecipe.CalH20(CPS_Total, AutoZeroRate, Width_Total, Setting.WidthSensorsDistance, Height_Total, Setting.HeightSensorsDistance, showBusDataProxy: false, TempAvg, calTempCorretcion: true).Item2 : ((RawDatas.Count <= 0) ? LastH20Value : RawDatas.GetAvgH2OValue()));
		return GetValidH20(res);
	}

	public double GetCurrentH20()
	{
		return GetValidH20(SelectedRecipe.CalH20(CPS_Current, AutoZeroRate, Width_Total, Setting.WidthSensorsDistance, Height_Total, Setting.HeightSensorsDistance, showBusDataProxy: false, TempAvg, calTempCorretcion: true).Item2);
	}

	private double CalBoardTemp(double A3Value)
	{
		try
		{
			double rate = (A3Value / 100.0 - Setting.BoardTempMinElec) / (Setting.BoardTempMaxElec - Setting.BoardTempMinElec);
			return (Setting.BoardTempMaxTemp - Setting.BoardTempMinTemp) * rate + Setting.BoardTempMinTemp;
		}
		catch (Exception ex)
		{
			GCLogger.log.Error(ex);
			return 0.0;
		}
	}

	public int GetSkipPoint()
	{
		if (SelectedRecipe.UseCustomDropPoint)
		{
			return SelectedRecipe.DropFrontPoint_Custom;
		}
		if (ConfigModel.Instance.SelectedCar == "Car1Type")
		{
			return Setting.DropFrontPoint_OrdinaryTruck;
		}
		return Setting.DropFrontPoint_Semi_trailerTruck;
	}

	public int GetDropPoint()
	{
		if (SelectedRecipe.UseCustomDropPoint)
		{
			return SelectedRecipe.DropBehindPoint_Custom;
		}
		if (ConfigModel.Instance.SelectedCar == "Car1Type")
		{
			return Setting.DropBehindPoint_OrdinaryTruck;
		}
		return Setting.DropBehindPoint_Semi_trailerTruck;
	}

	private void ClearCurrentTotal()
	{
		ProCalculator.ClearValuesAuto();
	}

	public void CountSkip()
	{
		if (SkipCountDown > 0)
		{
			SkipCountDown--;
		}
		if (SkipCountDown <= 0 && InSkipPoint)
		{
			if (InSample)
			{
				MeasTimeoutTimer.Change((int)(Setting.CalibrationTime_s * 1000.0), -1);
			}
			else
			{
				MeasTimeoutTimer.Change((int)(Setting.MaxChargeTime_s * 1000.0), -1);
			}
			InSkipPoint = false;
			GCLogger.log.Info("Start Reading and clear data");
			RawDatas.Clear();
			StartTime = DateTime.Now;
		}
	}

	private void StartHoleCountDown()
	{
		if (ConfigModel.Instance.ParamSettingConfig.MaxEmptyPackTime == 0.0)
		{
			HolePackTimer.Change(0, -1);
		}
		else if (!InHolepack)
		{
			HolePackTimer.Change((int)(ConfigModel.Instance.ParamSettingConfig.MaxEmptyPackTime * 1000.0), -1);
			InHolepack = true;
		}
	}

	private void CloseHoleCountDown()
	{
		_ = InHolepack;
		HolePackTimer.Change(-1, -1);
		InHolepack = false;
		InRun = true;
	}

	public void SetRecipe(GCRecipe recipe)
	{
		if (recipe == null)
		{
			throw new NullReferenceException();
		}
		SelectedRecipe = recipe;
		EnableDynamicZeroRate = SelectedRecipe.EnableDynamicZeroRate;
		DropDataRequest = true;
	}

	public void SaveOneRawData()
	{
		double widthreal = Width_Real;
		UpdateTriggerWidth(widthreal);
		if (inSaving || InHolepack || InMeastimeout || !CanMeas || !GCSerialPortManager.BusSerialPortConn || !InRun)
		{
			if (!InRun)
			{
				AddDynamicZeroRate(CPS_Real);
			}
			return;
		}
		CountSkip();
		if (!InSkipPoint)
		{
			if (!double.IsNaN(widthreal))
			{
				AddRealWidth(widthreal);
			}
			double heightreal = Height_Real;
			if (!double.IsNaN(heightreal))
			{
				AddRealHeight(heightreal);
			}
			_ = H20_Real_Draw;
			double cpsreal = CPS_Real;
			double h20real = GetValidH20(SelectedRecipe.CalH20(GetRealCPSWithtemp(), AutoZeroRate, Width_Total, Setting.WidthSensorsDistance, Height_Total, Setting.HeightSensorsDistance, showBusDataProxy: false, Temp__, calTempCorretcion: true).Item2);
			if (!double.IsNaN(h20real) && !double.IsNaN(cpsreal))
			{
				AddRealCPS(cpsreal);
				GCRawEntity entity = new GCRawEntity
				{
					CPS = (float)cpsreal,
					H20 = (float)h20real,
					Width = (float)widthreal,
					Height = (float)heightreal
				};
				RawDatas.Add(entity);
			}
		}
	}

	public void PerformSaveData()
	{
		try
		{
			if (inSaving)
			{
				return;
			}
			inSaving = true;
			InMeastimeout = true;
			if ((double)DurationTime <= ConfigModel.Instance.ParamSettingConfig.MinChargeTime_s)
			{
				RawDatas.Clear();
				inSaving = false;
				GCLogger.log.Info("Data is empty or Duration is too short, drop data");
				this.SaveDataFinished?.Invoke();
				return;
			}
			int drop = GetDropPoint();
			if (drop >= RawDatas.Count)
			{
				RawDatas.Clear();
				inSaving = false;
				GCLogger.log.Info("Raw Data is less then drop data count, drop data");
				this.SaveDataFinished?.Invoke();
				return;
			}
			ProCalculator.GetCalculatorItem("CPS_Total").DropPoint(drop);
			ProCalculator.GetCalculatorItem("Width_Total").DropPoint(drop);
			ProCalculator.GetCalculatorItem("Height_Total").DropPoint(drop);
			RawDatas.DropSome(drop);
			GCLogger.log.Info($"drop {drop} point for save");
			double totalH2O = GetTotalH20();
			GCSummaryEntity CurrentData = new GCSummaryEntity
			{
				Temperature = TempAvg,
				RealZeroRate = AutoZeroRate,
				CPS = CPS_Total,
				Width = Width_Total,
				Height = Height_Total,
				H20 = totalH2O,
				StartDT = StartTime.Value,
				EndDT = DateTime.Now,
				GCRecipeID = SelectedRecipe.GCRecipeID,
				IsSampleData = InSample,
				PackageNo = PackageNumber,
				Weight = PackageWeight,
				RawDatas = RawDatas.ToListAndClear()
			};
			if (InSample)
			{
				CurrentData.SampleNo = SampleNo;
			}
			if (!InSample)
			{
				bool? alarmStatus = GetAlarmStatus(CurrentData.H20);
				CheckH20ResultForDelayAlarm(Setting.OutputAlarmInfoToPLCPort, alarmStatus);
			}
			using (GCContext db = new GCContext())
			{
				db.GCSummaryEntities.Add(CurrentData);
				db.SaveChanges();
			}
			if (!InSample)
			{
				LastH20Value = CurrentData.H20;
			}
			if (ClearNumberWhenFinished)
			{
				PackageNumber = string.Empty;
				PackageWeight = 0.0;
			}
			InRun = false;
			GCLogger.log.Info($"Data Save Finished points:{CurrentData.RawDatas.Count},StartTime:{CurrentData.StartDT.ToLongTimeString()}.EndTime:{CurrentData.EndDT.ToLongTimeString()}");
			this.SaveDataFinished?.Invoke();
			inSaving = false;
		}
		catch (Exception e)
		{
			GCLogger.log.Error(e);
			throw;
		}
	}

	private double RoundValue(double value)
	{
		return Math.Round(value, ConfigModel.Instance.Decimals);
	}
}
