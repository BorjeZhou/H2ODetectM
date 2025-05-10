using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class Str2intConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (int.TryParse(value.ToString(), out var result))
		{
			return result;
		}
		return 0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
