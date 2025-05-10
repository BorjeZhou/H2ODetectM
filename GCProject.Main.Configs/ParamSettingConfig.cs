using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using GCProject.Main.Data.Models;
using GCProject.Main.Lib;

namespace GCProject.Main.Configs;

public class ParamSettingConfig : DataErrorInfoBase
{
	public static List<TempCorrectionTypes> TempCorrectionTypesItemSource { get; } = new List<TempCorrectionTypes>
	{
		TempCorrectionTypes.None,
		TempCorrectionTypes.Only1,
		TempCorrectionTypes.Only2,
		TempCorrectionTypes.Average12,
		TempCorrectionTypes.Board
	};


	public static List<int> FourDigitalItemSource { get; } = new List<int> { 1, 2, 3, 4 };


	[XmlAttribute]
	public RunModeEnum RunMode { get; set; } = RunModeEnum.Mode_24G;


	[XmlAttribute]
	public ConfirmAlarmEnum ConfirmAlarm { get; set; } = ConfirmAlarmEnum.Digital_Input1;


	[XmlAttribute]
	public MeasFinishedRelayEnum MeasFinishedRelay { get; set; }

	[XmlAttribute]
	public XScaleDispalyEnum XScaleDispaly { get; set; }

	[XmlAttribute]
	public DrawChartTypeEnum DrawChartType { get; set; } = DrawChartTypeEnum.RealWave;


	public TotalH2OCalculateType TotalH2OType { get; set; }

	[XmlAttribute]
	[Range(1, 40)]
	public int OneBarWidthPoint { get; set; } = 16;


	[XmlAttribute]
	[Range(6, 30)]
	public int BarChartFontSize { get; set; } = 12;


	[XmlAttribute]
	[Range(0, 360)]
	public int BarChartLabelAngle { get; set; }

	[XmlAttribute]
	[Range(1, 3)]
	public int BarChartLabelDecimal { get; set; } = 2;


	[Range(0.01, 10.0)]
	[XmlAttribute]
	public double Velosity { get; set; } = 2.0;


	[XmlAttribute]
	public bool OutputAlarmInfoToPLCPort { get; set; }

	[XmlAttribute]
	public string OutputAlarmInfo { get; set; } = "is not ok";


	[Range(-50, 150)]
	[XmlAttribute]
	public double TempCorrectionValue { get; set; }

	[Range(-100, 100)]
	[XmlAttribute]
	public double TempCorrectionParam { get; set; }

	[XmlAttribute]
	public TempCorrectionTypes TempCorrectionType { get; set; } = TempCorrectionTypes.Board;


	[XmlAttribute]
	public double BoardTempMinElec { get; set; } = 4.0;


	[XmlAttribute]
	public double BoardTempMinTemp { get; set; } = -50.0;


	[XmlAttribute]
	[BiggerThanAttrbute("BoardTempMinElec", true, null, CustomError = "param error")]
	public double BoardTempMaxElec { get; set; } = 20.0;


	[XmlAttribute]
	[BiggerThanAttrbute("BoardTempMinTemp", true, null, CustomError = "param error")]
	public double BoardTempMaxTemp { get; set; } = 150.0;


	[Range(1, 100)]
	[BiggerThanAttrbute("MaxChargeTime_s", false, null, CustomError = "CalibrationTime must less than MaxChargeTime time")]
	[XmlAttribute]
	public double CalibrationTime_s { get; set; } = 30.0;


	[Range(1, 100)]
	[BiggerThanAttrbute("MaxChargeTime_s", false, null)]
	[XmlAttribute]
	public double MinChargeTime_s { get; set; } = 1.0;


	[Range(1, 100)]
	[BiggerThanAttrbute("MinChargeTime_s", true, null)]
	[XmlAttribute]
	public double MaxChargeTime_s { get; set; } = 30.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double MaxEmptyPackTime { get; set; }

	[Range(0, 5)]
	[XmlAttribute]
	public double WidthSensorsDistance { get; set; } = 3.2;


	[Range(0, 5)]
	[XmlAttribute]
	public double HeightSensorsDistance { get; set; } = 2.0;


	[Range(0, 100)]
	[XmlAttribute]
	public int DropFrontPoint_OrdinaryTruck { get; set; } = 8;


	[Range(0, 100)]
	[XmlAttribute]
	public int DropBehindPoint_OrdinaryTruck { get; set; } = 8;


	[Range(0, 100)]
	[XmlAttribute]
	public int DropFrontPoint_Semi_trailerTruck { get; set; } = 8;


	[Range(0, 100)]
	[XmlAttribute]
	public int DropBehindPoint_Semi_trailerTruck { get; set; } = 8;


	[Range(0, 100)]
	[BiggerThanAttrbute("H20MeasRangeMin_868M", true, null)]
	[XmlAttribute]
	public double H20MeasRangeMax_868M { get; set; } = 100.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double H20MeasRangeMin_868M { get; set; }

	[Range(0, 100)]
	[BiggerThanAttrbute("H20DisplayRangeMin", true, null)]
	[XmlAttribute]
	public double H20DisplayRangeMax { get; set; } = 100.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double H20DisplayRangeMin { get; set; }

	[Range(0, 100)]
	[BiggerThanAttrbute("H20AlarmRangeMin_868M", true, null)]
	[XmlAttribute]
	public double H20AlarmRangeMax_868M { get; set; } = 90.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double H20AlarmRangeMin_868M { get; set; } = 10.0;


	[Range(0, 100)]
	[BiggerThanAttrbute("MRAlarmRangeMin", true, null)]
	[XmlAttribute]
	public double MRAlarmRangeMax { get; set; } = 90.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double MRAlarmRangeMin { get; set; } = 10.0;


	[XmlAttribute]
	public bool DisplayMR { get; set; }

	[XmlAttribute]
	public bool DropPointWhenOutOfMeasRange { get; set; }

	[Range(0, 100000)]
	[BiggerThanAttrbute("CPSMeasRangeMin_868M", true, null)]
	[XmlAttribute]
	public double CPSMeasRangeMax_868M { get; set; } = 65535.0;


	[Range(0, 100000)]
	[XmlAttribute]
	public double CPSMeasRangeMin_868M { get; set; } = 5.0;


	[Range(0, 100)]
	[BiggerThanAttrbute("H20ToElecRangeMin_868M", true, null)]
	[XmlAttribute]
	public double H20ToElecRangeMax_868M { get; set; } = 100.0;


	[Range(0, 100)]
	[XmlAttribute]
	public double H20ToElecRangeMin_868M { get; set; }

	[Range(0, 24)]
	[BiggerThanAttrbute("WidthSensorMaxElec_1", false, null)]
	[XmlAttribute]
	public double WidthSensorMinElec_1 { get; set; } = 4.0;


	[Range(0, 24)]
	[BiggerThanAttrbute("WidthSensorMaxElec_2", false, null)]
	[XmlAttribute]
	public double WidthSensorMinElec_2 { get; set; } = 4.0;


	[Range(0, 24)]
	[XmlAttribute]
	public double WidthSensorSetZeroWidthMaxElec_1 { get; set; } = 22.0;


	[Range(0, 24)]
	[XmlAttribute]
	public double WidthSensorSetZeroWidthMaxElec_2 { get; set; } = 22.0;


	[Range(0, 24)]
	[BiggerThanAttrbute("WidthSensorMinElec_1", true, null)]
	[XmlAttribute]
	public double WidthSensorMaxElec_1 { get; set; } = 20.0;


	[Range(0, 24)]
	[BiggerThanAttrbute("WidthSensorMinElec_2", true, null)]
	[XmlAttribute]
	public double WidthSensorMaxElec_2 { get; set; } = 20.0;


	[Range(0, 3)]
	[BiggerThanAttrbute("DistanceSensorMinDist", true, null)]
	[XmlAttribute]
	public double WidthSensorMaxDist { get; set; } = 1.0;


	[Range(0, 3)]
	[XmlAttribute]
	public double DistanceSensorMinDist { get; set; }

	[Range(0, 3)]
	[BiggerThanAttrbute("DistanceSensorMinDist", true, null)]
	[XmlAttribute]
	public double HeightSensorMaxDist { get; set; } = 1.0;


	[Range(0, 24)]
	[XmlAttribute]
	public double HeightSensorMinElec { get; set; } = 4.0;


	[BiggerThanAttrbute("HeightSensorMinElec", true, null)]
	[Range(0, 24)]
	[XmlAttribute]
	public double HeightSensorMaxElec { get; set; } = 20.0;


	[XmlAttribute]
	public bool WidthCorrectionEnabled { get; set; } = true;


	[XmlAttribute]
	public bool HeightCorrectionEnabled { get; set; } = true;


	[Range(0, 5)]
	[BiggerThanAttrbute("ValidMeasWidthMax", false, null)]
	[XmlAttribute]
	public double ValidMeasWidthMin { get; set; } = 0.3;


	[Range(0, 5)]
	[BiggerThanAttrbute("ValidMeasWidthMin", true, null)]
	[XmlAttribute]
	public double ValidMeasWidthMax { get; set; } = 2.5;


	[XmlArray]
	public List<GCRecipe> Recipes { get; set; } = new List<GCRecipe>();


	[Range(5, 600)]
	[XmlAttribute]
	public int RecursiveFilterDuration { get; set; } = 10;


	[Range(5, 600)]
	[XmlAttribute]
	public int ZeroRateRecursiveDuration { get; set; } = 60;


	[XmlAttribute]
	public int HighH20AlarmDigital { get; set; } = 1;


	[XmlAttribute]
	public int LowH20AlarmDigital { get; set; } = 2;


	[XmlAttribute]
	public int MeasFinishedDigital { get; set; } = 3;


	[XmlAttribute]
	public double CPS24GAmendmentParam { get; set; } = 1.0;

}
