using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class Bool2SelectionModeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if ((bool)value)
		{
			return SelectionMode.Multiple;
		}
		return SelectionMode.Extended;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
