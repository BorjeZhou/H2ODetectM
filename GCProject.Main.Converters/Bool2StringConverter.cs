using System;
using System.Globalization;
using System.Windows.Data;

namespace GCProject.Main.Converters;

public class Bool2StringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		string r = "error param";
		if (parameter is string s && value is bool b)
		{
			string[] res = s.Split("|");
			if (res.Length != 2)
			{
				return r;
			}
			if (!b)
			{
				return res[1];
			}
			return res[0];
		}
		return r;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
