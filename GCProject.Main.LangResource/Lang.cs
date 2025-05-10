using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GCProject.Main.LangResource;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class Lang
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Lang", typeof(Lang).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string Accept => ResourceManager.GetString("Accept", resourceCulture);

	public static string Add => ResourceManager.GetString("Add", resourceCulture);

	public static string ALangDisplay => ResourceManager.GetString("ALangDisplay", resourceCulture);

	public static string BusWindow_Current => ResourceManager.GetString("BusWindow_Current", resourceCulture);

	public static string BusWindow_Input => ResourceManager.GetString("BusWindow_Input", resourceCulture);

	public static string BusWindow_Output => ResourceManager.GetString("BusWindow_Output", resourceCulture);

	public static string BusWindow_Title => ResourceManager.GetString("BusWindow_Title", resourceCulture);

	public static string BusWindow_Total => ResourceManager.GetString("BusWindow_Total", resourceCulture);

	public static string CANCEL => ResourceManager.GetString("CANCEL", resourceCulture);

	public static string ClearWhenFinished => ResourceManager.GetString("ClearWhenFinished", resourceCulture);

	public static string Comfirm => ResourceManager.GetString("Comfirm", resourceCulture);

	public static string CPS_ZeroRate => ResourceManager.GetString("CPS_ZeroRate", resourceCulture);

	public static string CTW_AddSplitTemp => ResourceManager.GetString("CTW_AddSplitTemp", resourceCulture);

	public static string CTW_AvgWeightRange => ResourceManager.GetString("CTW_AvgWeightRange", resourceCulture);

	public static string CTW_CorrectionValue_d => ResourceManager.GetString("CTW_CorrectionValue_d", resourceCulture);

	public static string CTW_EnableTempCorr => ResourceManager.GetString("CTW_EnableTempCorr", resourceCulture);

	public static string CTW_EndTemp_d => ResourceManager.GetString("CTW_EndTemp_d", resourceCulture);

	public static string CTW_Msg1 => ResourceManager.GetString("CTW_Msg1", resourceCulture);

	public static string CTW_Msg2 => ResourceManager.GetString("CTW_Msg2", resourceCulture);

	public static string CTW_Msg3 => ResourceManager.GetString("CTW_Msg3", resourceCulture);

	public static string CTW_RegressionCorrectionParam => ResourceManager.GetString("CTW_RegressionCorrectionParam", resourceCulture);

	public static string CTW_RemoveSplitTemp => ResourceManager.GetString("CTW_RemoveSplitTemp", resourceCulture);

	public static string CTW_SplitTemp => ResourceManager.GetString("CTW_SplitTemp", resourceCulture);

	public static string CTW_StartTemp_d => ResourceManager.GetString("CTW_StartTemp_d", resourceCulture);

	public static string CTW_StaticCorrectionParam => ResourceManager.GetString("CTW_StaticCorrectionParam", resourceCulture);

	public static string CTW_TempCorrType => ResourceManager.GetString("CTW_TempCorrType", resourceCulture);

	public static string DELETE => ResourceManager.GetString("DELETE", resourceCulture);

	public static string Diff_TempCorr => ResourceManager.GetString("Diff_TempCorr", resourceCulture);

	public static string Distance_m => ResourceManager.GetString("Distance_m", resourceCulture);

	public static string Drop => ResourceManager.GetString("Drop", resourceCulture);

	public static string Edit => ResourceManager.GetString("Edit", resourceCulture);

	public static string Error => ResourceManager.GetString("Error", resourceCulture);

	public static string EXIT => ResourceManager.GetString("EXIT", resourceCulture);

	public static string H20_TempCorr => ResourceManager.GetString("H20_TempCorr", resourceCulture);

	public static string H2O => ResourceManager.GetString("H2O", resourceCulture);

	public static string Index => ResourceManager.GetString("Index", resourceCulture);

	public static string Information => ResourceManager.GetString("Information", resourceCulture);

	public static string InputError => ResourceManager.GetString("InputError", resourceCulture);

	public static string Language => ResourceManager.GetString("Language", resourceCulture);

	public static string MainChart_Distance_m => ResourceManager.GetString("MainChart_Distance_m", resourceCulture);

	public static string MainChart_Time_s => ResourceManager.GetString("MainChart_Time_s", resourceCulture);

	public static string MainWindow_BACH_Data => ResourceManager.GetString("MainWindow_BACH_Data", resourceCulture);

	public static string MainWindow_BachBus => ResourceManager.GetString("MainWindow_BachBus", resourceCulture);

	public static string MainWindow_Bus => ResourceManager.GetString("MainWindow_Bus", resourceCulture);

	public static string MainWindow_CanRun => ResourceManager.GetString("MainWindow_CanRun", resourceCulture);

	public static string MainWindow_ChangeLanguageNotify => ResourceManager.GetString("MainWindow_ChangeLanguageNotify", resourceCulture);

	public static string MainWindow_Corr => ResourceManager.GetString("MainWindow_Corr", resourceCulture);

	public static string MainWindow_Current => ResourceManager.GetString("MainWindow_Current", resourceCulture);

	public static string MainWindow_Exit => ResourceManager.GetString("MainWindow_Exit", resourceCulture);

	public static string MainWindow_ExitInfo => ResourceManager.GetString("MainWindow_ExitInfo", resourceCulture);

	public static string MainWindow_H2O => ResourceManager.GetString("MainWindow_H2O", resourceCulture);

	public static string MainWindow_Height => ResourceManager.GetString("MainWindow_Height", resourceCulture);

	public static string MainWindow_Height_mA => ResourceManager.GetString("MainWindow_Height_mA", resourceCulture);

	public static string MainWindow_MR => ResourceManager.GetString("MainWindow_MR", resourceCulture);

	public static string MainWindow_No => ResourceManager.GetString("MainWindow_No", resourceCulture);

	public static string MainWindow_Parameter => ResourceManager.GetString("MainWindow_Parameter", resourceCulture);

	public static string MainWindow_Password => ResourceManager.GetString("MainWindow_Password", resourceCulture);

	public static string MainWindow_Recipe => ResourceManager.GetString("MainWindow_Recipe", resourceCulture);

	public static string MainWindow_RecordData => ResourceManager.GetString("MainWindow_RecordData", resourceCulture);

	public static string MainWindow_Regression => ResourceManager.GetString("MainWindow_Regression", resourceCulture);

	public static string MainWindow_Run_Waitting => ResourceManager.GetString("MainWindow_Run_Waitting", resourceCulture);

	public static string MainWindow_Sampling => ResourceManager.GetString("MainWindow_Sampling", resourceCulture);

	public static string MainWindow_System => ResourceManager.GetString("MainWindow_System", resourceCulture);

	public static string MainWindow_Temp_C => ResourceManager.GetString("MainWindow_Temp_C", resourceCulture);

	public static string MainWindow_Temp_mA => ResourceManager.GetString("MainWindow_Temp_mA", resourceCulture);

	public static string MainWindow_Title => ResourceManager.GetString("MainWindow_Title", resourceCulture);

	public static string MainWindow_Total => ResourceManager.GetString("MainWindow_Total", resourceCulture);

	public static string MainWindow_Width => ResourceManager.GetString("MainWindow_Width", resourceCulture);

	public static string MainWindow_Width_mA => ResourceManager.GetString("MainWindow_Width_mA", resourceCulture);

	public static string MainWindow_Yes => ResourceManager.GetString("MainWindow_Yes", resourceCulture);

	public static string Max => ResourceManager.GetString("Max", resourceCulture);

	public static string Message => ResourceManager.GetString("Message", resourceCulture);

	public static string Min => ResourceManager.GetString("Min", resourceCulture);

	public static string New => ResourceManager.GetString("New", resourceCulture);

	public static string OK => ResourceManager.GetString("OK", resourceCulture);

	public static string Old => ResourceManager.GetString("Old", resourceCulture);

	public static string PackageNum => ResourceManager.GetString("PackageNum", resourceCulture);

	public static string PackageWeight => ResourceManager.GetString("PackageWeight", resourceCulture);

	public static string ParamSetting => ResourceManager.GetString("ParamSetting", resourceCulture);

	public static string Password => ResourceManager.GetString("Password", resourceCulture);

	public static string PSW_24GAmendmentParam => ResourceManager.GetString("PSW_24GAmendmentParam", resourceCulture);

	public static string PSW_Alarm_High_Digital => ResourceManager.GetString("PSW_Alarm_High_Digital", resourceCulture);

	public static string PSW_Alarm_Low_Digital => ResourceManager.GetString("PSW_Alarm_Low_Digital", resourceCulture);

	public static string PSW_AlarmDigitalSetting => ResourceManager.GetString("PSW_AlarmDigitalSetting", resourceCulture);

	public static string PSW_AlarmH20Range => ResourceManager.GetString("PSW_AlarmH20Range", resourceCulture);

	public static string PSW_AlarmMRRange => ResourceManager.GetString("PSW_AlarmMRRange", resourceCulture);

	public static string PSW_AlarmPLCOutput => ResourceManager.GetString("PSW_AlarmPLCOutput", resourceCulture);

	public static string PSW_Behind => ResourceManager.GetString("PSW_Behind", resourceCulture);

	public static string PSW_BoardTempElecRange => ResourceManager.GetString("PSW_BoardTempElecRange", resourceCulture);

	public static string PSW_BoardTempRange => ResourceManager.GetString("PSW_BoardTempRange", resourceCulture);

	public static string PSW_ChangedAt => ResourceManager.GetString("PSW_ChangedAt", resourceCulture);

	public static string PSW_Chart_Distance => ResourceManager.GetString("PSW_Chart_Distance", resourceCulture);

	public static string PSW_Chart_DrawChartType => ResourceManager.GetString("PSW_Chart_DrawChartType", resourceCulture);

	public static string PSW_Chart_RealWave => ResourceManager.GetString("PSW_Chart_RealWave", resourceCulture);

	public static string PSW_Chart_Time => ResourceManager.GetString("PSW_Chart_Time", resourceCulture);

	public static string PSW_Chart_Trend => ResourceManager.GetString("PSW_Chart_Trend", resourceCulture);

	public static string PSW_Chart_X_AxisBar_Angle_Decimal => ResourceManager.GetString("PSW_Chart_X_AxisBar_Angle_Decimal", resourceCulture);

	public static string PSW_Chart_X_AxisBarWidth_point_FontSize => ResourceManager.GetString("PSW_Chart_X_AxisBarWidth_point_FontSize", resourceCulture);

	public static string PSW_Chart_X_AxisVelosity => ResourceManager.GetString("PSW_Chart_X_AxisVelosity", resourceCulture);

	public static string PSW_Chart_X_DisplayType => ResourceManager.GetString("PSW_Chart_X_DisplayType", resourceCulture);

	public static string PSW_ChartOption => ResourceManager.GetString("PSW_ChartOption", resourceCulture);

	public static string PSW_ConfirmAlarm => ResourceManager.GetString("PSW_ConfirmAlarm", resourceCulture);

	public static string PSW_CutPoints_Cycles => ResourceManager.GetString("PSW_CutPoints_Cycles", resourceCulture);

	public static string PSW_CutPoints_Cycles_Name => ResourceManager.GetString("PSW_CutPoints_Cycles_Name", resourceCulture);

	public static string PSW_DigitalInput1 => ResourceManager.GetString("PSW_DigitalInput1", resourceCulture);

	public static string PSW_DigitalInput2 => ResourceManager.GetString("PSW_DigitalInput2", resourceCulture);

	public static string PSW_DisplayH20Range => ResourceManager.GetString("PSW_DisplayH20Range", resourceCulture);

	public static string PSW_DisplayMR => ResourceManager.GetString("PSW_DisplayMR", resourceCulture);

	public static string PSW_DistanceBetweenWidthHeightSensors => ResourceManager.GetString("PSW_DistanceBetweenWidthHeightSensors", resourceCulture);

	public static string PSW_DurationCharge_Min_Max => ResourceManager.GetString("PSW_DurationCharge_Min_Max", resourceCulture);

	public static string PSW_DurationForCalibration => ResourceManager.GetString("PSW_DurationForCalibration", resourceCulture);

	public static string PSW_DurationTimes => ResourceManager.GetString("PSW_DurationTimes", resourceCulture);

	public static string PSW_DynamicZerorateCutPoint_Sec => ResourceManager.GetString("PSW_DynamicZerorateCutPoint_Sec", resourceCulture);

	public static string PSW_EnableStage_SpliteCPS => ResourceManager.GetString("PSW_EnableStage_SpliteCPS", resourceCulture);

	public static string PSW_Fix => ResourceManager.GetString("PSW_Fix", resourceCulture);

	public static string PSW_Front => ResourceManager.GetString("PSW_Front", resourceCulture);

	public static string PSW_H20ToElecRange => ResourceManager.GetString("PSW_H20ToElecRange", resourceCulture);

	public static string PSW_HeightSensorElec => ResourceManager.GetString("PSW_HeightSensorElec", resourceCulture);

	public static string PSW_HoleWidthMax => ResourceManager.GetString("PSW_HoleWidthMax", resourceCulture);

	public static string PSW_MeasFinished => ResourceManager.GetString("PSW_MeasFinished", resourceCulture);

	public static string PSW_MeasureCPSRange => ResourceManager.GetString("PSW_MeasureCPSRange", resourceCulture);

	public static string PSW_MeasureH20Range => ResourceManager.GetString("PSW_MeasureH20Range", resourceCulture);

	public static string PSW_MeasurementFinishedRelay => ResourceManager.GetString("PSW_MeasurementFinishedRelay", resourceCulture);

	public static string PSW_None => ResourceManager.GetString("PSW_None", resourceCulture);

	public static string PSW_Off => ResourceManager.GetString("PSW_Off", resourceCulture);

	public static string PSW_Off_NoConfirm => ResourceManager.GetString("PSW_Off_NoConfirm", resourceCulture);

	public static string PSW_Offset => ResourceManager.GetString("PSW_Offset", resourceCulture);

	public static string PSW_Only1 => ResourceManager.GetString("PSW_Only1", resourceCulture);

	public static string PSW_Only2 => ResourceManager.GetString("PSW_Only2", resourceCulture);

	public static string PSW_OutputAlarmInfoToPLCPort => ResourceManager.GetString("PSW_OutputAlarmInfoToPLCPort", resourceCulture);

	public static string PSW_ParamSetting => ResourceManager.GetString("PSW_ParamSetting", resourceCulture);

	public static string PSW_RecipeCoefficients => ResourceManager.GetString("PSW_RecipeCoefficients", resourceCulture);

	public static string PSW_RecipeParanWarning => ResourceManager.GetString("PSW_RecipeParanWarning", resourceCulture);

	public static string PSW_Relay2 => ResourceManager.GetString("PSW_Relay2", resourceCulture);

	public static string PSW_RunMode_DataSource => ResourceManager.GetString("PSW_RunMode_DataSource", resourceCulture);

	public static string PSW_SaveSuccess => ResourceManager.GetString("PSW_SaveSuccess", resourceCulture);

	public static string PSW_SelectRecipe => ResourceManager.GetString("PSW_SelectRecipe", resourceCulture);

	public static string PSW_Slope => ResourceManager.GetString("PSW_Slope", resourceCulture);

	public static string PSW_SomeFieldError => ResourceManager.GetString("PSW_SomeFieldError", resourceCulture);

	public static string PSW_SplitCPSRange => ResourceManager.GetString("PSW_SplitCPSRange", resourceCulture);

	public static string PSW_StaticRecursiveFilterDuration => ResourceManager.GetString("PSW_StaticRecursiveFilterDuration", resourceCulture);

	public static string PSW_TempCorrection_Custom => ResourceManager.GetString("PSW_TempCorrection_Custom", resourceCulture);

	public static string PSW_TempCorrection_Title => ResourceManager.GetString("PSW_TempCorrection_Title", resourceCulture);

	public static string PSW_TempCorrection_Value => ResourceManager.GetString("PSW_TempCorrection_Value", resourceCulture);

	public static string PSW_TemperatureCorrection => ResourceManager.GetString("PSW_TemperatureCorrection", resourceCulture);

	public static string PSW_TemperatureCorrectionParam => ResourceManager.GetString("PSW_TemperatureCorrectionParam", resourceCulture);

	public static string PSW_Title => ResourceManager.GetString("PSW_Title", resourceCulture);

	public static string PSW_TotalH2OCalculateType => ResourceManager.GetString("PSW_TotalH2OCalculateType", resourceCulture);

	public static string PSW_TotalH2OCalculateType_AvgCPS => ResourceManager.GetString("PSW_TotalH2OCalculateType_AvgCPS", resourceCulture);

	public static string PSW_TotalH2OCalculateType_AvgH2O => ResourceManager.GetString("PSW_TotalH2OCalculateType_AvgH2O", resourceCulture);

	public static string PSW_ValidWidthForTrigger => ResourceManager.GetString("PSW_ValidWidthForTrigger", resourceCulture);

	public static string PSW_ValueCaseIfDetectorError => ResourceManager.GetString("PSW_ValueCaseIfDetectorError", resourceCulture);

	public static string PSW_ValueCaseIfDetectorError_Fix => ResourceManager.GetString("PSW_ValueCaseIfDetectorError_Fix", resourceCulture);

	public static string PSW_ValueCaseIfDetectorError_Max => ResourceManager.GetString("PSW_ValueCaseIfDetectorError_Max", resourceCulture);

	public static string PSW_ValueCaseIfDetectorError_Min => ResourceManager.GetString("PSW_ValueCaseIfDetectorError_Min", resourceCulture);

	public static string PSW_WidthHeightCorrection => ResourceManager.GetString("PSW_WidthHeightCorrection", resourceCulture);

	public static string PSW_WidthOffset => ResourceManager.GetString("PSW_WidthOffset", resourceCulture);

	public static string PSW_WidthSensorElecRecv => ResourceManager.GetString("PSW_WidthSensorElecRecv", resourceCulture);

	public static string PSW_WidthSensorElecSend => ResourceManager.GetString("PSW_WidthSensorElecSend", resourceCulture);

	public static string PSW_WidthSensorMaxDistance => ResourceManager.GetString("PSW_WidthSensorMaxDistance", resourceCulture);

	public static string PSW_WidthSensorMinDistance => ResourceManager.GetString("PSW_WidthSensorMinDistance", resourceCulture);

	public static string PSW_WidthSensorSetZeroMaxElec => ResourceManager.GetString("PSW_WidthSensorSetZeroMaxElec", resourceCulture);

	public static string PSW_WidthSlope => ResourceManager.GetString("PSW_WidthSlope", resourceCulture);

	public static string PSW_ZeroRate => ResourceManager.GetString("PSW_ZeroRate", resourceCulture);

	public static string PSW_ZeroRate_EnableDynamic => ResourceManager.GetString("PSW_ZeroRate_EnableDynamic", resourceCulture);

	public static string PwdGuard_InputNewPassword => ResourceManager.GetString("PwdGuard_InputNewPassword", resourceCulture);

	public static string PwdGuard_InputPassword => ResourceManager.GetString("PwdGuard_InputPassword", resourceCulture);

	public static string PwdGuard_PasswordIncorrect => ResourceManager.GetString("PwdGuard_PasswordIncorrect", resourceCulture);

	public static string PwdWindow_Input => ResourceManager.GetString("PwdWindow_Input", resourceCulture);

	public static string RawDataChartWindow_Height => ResourceManager.GetString("RawDataChartWindow_Height", resourceCulture);

	public static string RawDataChartWindow_LeftAxis => ResourceManager.GetString("RawDataChartWindow_LeftAxis", resourceCulture);

	public static string RawDataChartWindow_RightAxis => ResourceManager.GetString("RawDataChartWindow_RightAxis", resourceCulture);

	public static string RawDataChartWindow_Title => ResourceManager.GetString("RawDataChartWindow_Title", resourceCulture);

	public static string RawDataChartWindow_Width => ResourceManager.GetString("RawDataChartWindow_Width", resourceCulture);

	public static string RecipeWindow_Display => ResourceManager.GetString("RecipeWindow_Display", resourceCulture);

	public static string RecipeWindow_Edit_Accept => ResourceManager.GetString("RecipeWindow_Edit_Accept", resourceCulture);

	public static string RecipeWindow_LastModifyDate => ResourceManager.GetString("RecipeWindow_LastModifyDate", resourceCulture);

	public static string RecipeWindow_Name => ResourceManager.GetString("RecipeWindow_Name", resourceCulture);

	public static string RecipeWindow_SortNum => ResourceManager.GetString("RecipeWindow_SortNum", resourceCulture);

	public static string RecipeWindow_Title => ResourceManager.GetString("RecipeWindow_Title", resourceCulture);

	public static string RecordWindow_DataView => ResourceManager.GetString("RecordWindow_DataView", resourceCulture);

	public static string RecordWindow_DeleteConfirm => ResourceManager.GetString("RecordWindow_DeleteConfirm", resourceCulture);

	public static string RecordWindow_Details => ResourceManager.GetString("RecordWindow_Details", resourceCulture);

	public static string RecordWindow_Duration => ResourceManager.GetString("RecordWindow_Duration", resourceCulture);

	public static string RecordWindow_End => ResourceManager.GetString("RecordWindow_End", resourceCulture);

	public static string RecordWindow_ExportSuccess => ResourceManager.GetString("RecordWindow_ExportSuccess", resourceCulture);

	public static string RecordWindow_Height => ResourceManager.GetString("RecordWindow_Height", resourceCulture);

	public static string RecordWindow_Latest => ResourceManager.GetString("RecordWindow_Latest", resourceCulture);

	public static string RecordWindow_No => ResourceManager.GetString("RecordWindow_No", resourceCulture);

	public static string RecordWindow_Recipe => ResourceManager.GetString("RecordWindow_Recipe", resourceCulture);

	public static string RecordWindow_Records => ResourceManager.GetString("RecordWindow_Records", resourceCulture);

	public static string RecordWindow_SelectDate => ResourceManager.GetString("RecordWindow_SelectDate", resourceCulture);

	public static string RecordWindow_Start => ResourceManager.GetString("RecordWindow_Start", resourceCulture);

	public static string RecordWindow_Temp => ResourceManager.GetString("RecordWindow_Temp", resourceCulture);

	public static string RecordWindow_Title => ResourceManager.GetString("RecordWindow_Title", resourceCulture);

	public static string RecordWindow_ToCSV => ResourceManager.GetString("RecordWindow_ToCSV", resourceCulture);

	public static string RecordWindow_Total => ResourceManager.GetString("RecordWindow_Total", resourceCulture);

	public static string RecordWindow_Width => ResourceManager.GetString("RecordWindow_Width", resourceCulture);

	public static string Refresh => ResourceManager.GetString("Refresh", resourceCulture);

	public static string RegressionCalWindow_Caculation => ResourceManager.GetString("RegressionCalWindow_Caculation", resourceCulture);

	public static string RegressionCalWindow_SaveError => ResourceManager.GetString("RegressionCalWindow_SaveError", resourceCulture);

	public static string RegressionCalWindow_Title => ResourceManager.GetString("RegressionCalWindow_Title", resourceCulture);

	public static string RegressionCorrection => ResourceManager.GetString("RegressionCorrection", resourceCulture);

	public static string RegressionWindow_Diff => ResourceManager.GetString("RegressionWindow_Diff", resourceCulture);

	public static string RegressionWindow_Labor => ResourceManager.GetString("RegressionWindow_Labor", resourceCulture);

	public static string RegressionWindow_Mark_CPS => ResourceManager.GetString("RegressionWindow_Mark_CPS", resourceCulture);

	public static string RegressionWindow_Mark_TEMP => ResourceManager.GetString("RegressionWindow_Mark_TEMP", resourceCulture);

	public static string RegressionWindow_NoDataSelected => ResourceManager.GetString("RegressionWindow_NoDataSelected", resourceCulture);

	public static string RegressionWindow_Regression => ResourceManager.GetString("RegressionWindow_Regression", resourceCulture);

	public static string RegressionWindow_SampleGraph => ResourceManager.GetString("RegressionWindow_SampleGraph", resourceCulture);

	public static string RegressionWindow_TempCorrH2O => ResourceManager.GetString("RegressionWindow_TempCorrH2O", resourceCulture);

	public static string RegressionWindow_Title => ResourceManager.GetString("RegressionWindow_Title", resourceCulture);

	public static string RSCW_RegressionSampleCompare => ResourceManager.GetString("RSCW_RegressionSampleCompare", resourceCulture);

	public static string S_Enabled_SpliteCPS => ResourceManager.GetString("S_Enabled_SpliteCPS", resourceCulture);

	public static string S_Split_data_type => ResourceManager.GetString("S_Split_data_type", resourceCulture);

	public static string SampleCompareChart_LaberValue => ResourceManager.GetString("SampleCompareChart_LaberValue", resourceCulture);

	public static string SampleCompareChart_MeasureValue => ResourceManager.GetString("SampleCompareChart_MeasureValue", resourceCulture);

	public static string SampleCompareChart_SampleIndex => ResourceManager.GetString("SampleCompareChart_SampleIndex", resourceCulture);

	public static string SamplingWindow_Duration_s => ResourceManager.GetString("SamplingWindow_Duration_s", resourceCulture);

	public static string SamplingWindow_InvalidClose => ResourceManager.GetString("SamplingWindow_InvalidClose", resourceCulture);

	public static string SamplingWindow_Running => ResourceManager.GetString("SamplingWindow_Running", resourceCulture);

	public static string SamplingWindow_SampleH2O => ResourceManager.GetString("SamplingWindow_SampleH2O", resourceCulture);

	public static string SamplingWindow_SampleNo => ResourceManager.GetString("SamplingWindow_SampleNo", resourceCulture);

	public static string SamplingWindow_StartTime => ResourceManager.GetString("SamplingWindow_StartTime", resourceCulture);

	public static string SamplingWindow_title => ResourceManager.GetString("SamplingWindow_title", resourceCulture);

	public static string SAVE => ResourceManager.GetString("SAVE", resourceCulture);

	public static string SettingWindow_BoardPort => ResourceManager.GetString("SettingWindow_BoardPort", resourceCulture);

	public static string SettingWindow_ChangePwd => ResourceManager.GetString("SettingWindow_ChangePwd", resourceCulture);

	public static string SettingWindow_DataLog => ResourceManager.GetString("SettingWindow_DataLog", resourceCulture);

	public static string SettingWindow_DataPort => ResourceManager.GetString("SettingWindow_DataPort", resourceCulture);

	public static string SettingWindow_DigIn => ResourceManager.GetString("SettingWindow_DigIn", resourceCulture);

	public static string SettingWindow_DigitalLogic => ResourceManager.GetString("SettingWindow_DigitalLogic", resourceCulture);

	public static string SettingWindow_DigOut => ResourceManager.GetString("SettingWindow_DigOut", resourceCulture);

	public static string SettingWindow_OtherSetting => ResourceManager.GetString("SettingWindow_OtherSetting", resourceCulture);

	public static string SettingWindow_PlcAddr => ResourceManager.GetString("SettingWindow_PlcAddr", resourceCulture);

	public static string SettingWindow_PlcPort => ResourceManager.GetString("SettingWindow_PlcPort", resourceCulture);

	public static string SettingWindow_RS232Comminication => ResourceManager.GetString("SettingWindow_RS232Comminication", resourceCulture);

	public static string SettingWindow_SamePortError => ResourceManager.GetString("SettingWindow_SamePortError", resourceCulture);

	public static string SettingWindow_Title => ResourceManager.GetString("SettingWindow_Title", resourceCulture);

	public static string Start => ResourceManager.GetString("Start", resourceCulture);

	public static string StaticCorrection => ResourceManager.GetString("StaticCorrection", resourceCulture);

	public static string Stop => ResourceManager.GetString("Stop", resourceCulture);

	public static string TC_Order => ResourceManager.GetString("TC_Order", resourceCulture);

	public static string TC_Order_Repeat => ResourceManager.GetString("TC_Order_Repeat", resourceCulture);

	public static string TC_Reorder => ResourceManager.GetString("TC_Reorder", resourceCulture);

	public static string TempCorrection => ResourceManager.GetString("TempCorrection", resourceCulture);

	public static string Time_s => ResourceManager.GetString("Time_s", resourceCulture);

	internal Lang()
	{
	}
}
