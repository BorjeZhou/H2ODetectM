using System;
using GCProject.Main.Configs;

namespace GCProject.Main.Lib;

public class BaseConveter
{
	private const double MaxElec_mA = 20.0;

	private const double MinElec_mA = 4.0;

	private const double MaxRaw = 1023.0;

	private const double MinRaw = 204.0;

	private const double RawRange = 819.0;

	private const double MaxTemp = 80.0;

	private const double MinTemp = -40.0;

	private const double TempRange = 120.0;

	public static ParamSettingConfig Setting => ConfigModel.Instance.ParamSettingConfig;

	public static double ConvertToTemp(double rawData)
	{
		if (rawData == 0.0)
		{
			return 0.0;
		}
		double incRate = (rawData - 204.0) / 819.0;
		return Math.Round(-40.0 + incRate * 120.0, ConfigModel.Instance.Decimals);
	}

	public static double ConvertToHeightElec(double rawData)
	{
		double height_elec = rawData / 100.0;
		double minElec = Setting.HeightSensorMinElec;
		double maxElec = Setting.HeightSensorMaxElec;
		if (height_elec < minElec)
		{
			height_elec = minElec;
		}
		else if (height_elec > maxElec)
		{
			height_elec = maxElec;
		}
		return height_elec;
	}

	public static double ConvertToHeight(double rawData)
	{
		double num = ConvertToHeightElec(rawData);
		double minElec = Setting.HeightSensorMinElec;
		double maxElec = Setting.HeightSensorMaxElec;
		return Math.Round((num - minElec) / (maxElec - minElec) * (Setting.HeightSensorMaxDist - 0.0), ConfigModel.Instance.Decimals);
	}

	public static double ConvertToWidth(double rawData, int num, out bool SetZeroWith)
	{
		double width_elec = ConvertToElec(rawData);
		double minElec = ((num == 1) ? Setting.WidthSensorMinElec_1 : Setting.WidthSensorMinElec_2);
		double maxElec = ((num == 1) ? Setting.WidthSensorMaxElec_1 : Setting.WidthSensorMaxElec_2);
		double setZeroMaxElec = ((num == 1) ? Setting.WidthSensorSetZeroWidthMaxElec_1 : Setting.WidthSensorSetZeroWidthMaxElec_2);
		SetZeroWith = width_elec >= setZeroMaxElec;
		if (width_elec < minElec)
		{
			width_elec = minElec;
		}
		else if (width_elec > maxElec)
		{
			width_elec = maxElec;
		}
		return Math.Round((width_elec - minElec) / (maxElec - minElec) * (Setting.WidthSensorMaxDist - 0.0), ConfigModel.Instance.Decimals);
	}

	public static double ConvertToElec(double rawData)
	{
		double incRate = (rawData - 204.0) / 819.0;
		return Math.Round(4.0 + incRate * 16.0, ConfigModel.Instance.Decimals);
	}

	public static int ConvertH20ToElec(double h20, double minH20, double maxH20)
	{
		double incH20Rate = (h20 - minH20) / (maxH20 - minH20);
		double res = 4.0 + incH20Rate * 16.0;
		if (res > 20.0)
		{
			res = 20.0;
		}
		if (res < 4.0)
		{
			res = 4.0;
		}
		res *= 100.0;
		return (int)Math.Round(res, 0);
	}
}
