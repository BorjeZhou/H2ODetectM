using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GCProject.Main.Converters;

public class NullableBool2BgColorConverter : IValueConverter
{
	private Brush TrueBackground { get; set; } = Brushes.Blue;


	private Brush NormalBackground { get; set; } = Brushes.Green;


	private Brush FalseBackground { get; set; } = Brushes.Red;


	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return NormalBackground;
		}
		if ((bool)value)
		{
			return TrueBackground;
		}
		return FalseBackground;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
