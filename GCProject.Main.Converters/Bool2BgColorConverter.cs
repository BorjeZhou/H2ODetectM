using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GCProject.Main.Converters;

public class Bool2BgColorConverter : IValueConverter
{
	private Brush SuccessColor => Brushes.Green;

	private Brush ErrorColor => Brushes.Red;

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return ErrorColor;
			}
			return SuccessColor;
		}
		return ErrorColor;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
