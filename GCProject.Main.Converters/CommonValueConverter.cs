using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class CommonValueConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return System.Convert.ChangeType(value, targetType);
		}
		catch (Exception)
		{
			throw new FormatException("dsda");
		}
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			return System.Convert.ChangeType(value, targetType);
		}
		catch (Exception)
		{
			throw new FormatException("dsda");
		}
	}
}
