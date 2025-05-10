using System.Xml.Serialization;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public class SpliteTemp : NotifyBase
{
	private int _Order;

	private double _temp;

	[XmlAttribute]
	public int Order
	{
		get
		{
			return _Order;
		}
		set
		{
			if (_Order != value)
			{
				_Order = value;
				OnPropertyChanged("Order");
			}
		}
	}

	[XmlAttribute]
	public double Temp
	{
		get
		{
			return _temp;
		}
		set
		{
			if (_temp != value)
			{
				_temp = value;
				OnPropertyChanged("Temp");
			}
		}
	}
}
