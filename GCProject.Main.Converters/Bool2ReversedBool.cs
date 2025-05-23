using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class Bool2ReversedBool : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool b)
		{
			return !b;
		}
		return false;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
