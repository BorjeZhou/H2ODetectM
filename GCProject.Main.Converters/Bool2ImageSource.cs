using System;
using System.Globalization;
using System.Windows.Data;
using GCProject.Main.Resource;

namespace GCProject.Main.Converters;

public class Bool2ImageSource : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool b)
		{
			_ = Res.Car1;
			if (!b)
			{
				return Res.Car2;
			}
			return Res.Car1;
		}
		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
