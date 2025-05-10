using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GCProject.Main.Converters;

public class Alarm2BgColorConverter : IValueConverter
{
	private Brush AlarmColor => Brushes.PaleVioletRed;

	private Brush GreenColor => Brushes.Yellow;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return GreenColor;
			}
			return AlarmColor;
		}
		return Brushes.AntiqueWhite;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
