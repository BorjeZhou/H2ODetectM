using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class DateTime2StringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is DateTime dt)
		{
			return dt.ToString(parameter.ToString());
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
