using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class EnumToBoolConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value.ToString() == parameter.ToString();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool && (bool)value)
		{
			return System.Convert.ChangeType(parameter, targetType);
		}
		return null;
	}
}
