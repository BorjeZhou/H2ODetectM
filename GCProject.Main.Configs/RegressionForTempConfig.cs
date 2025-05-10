using System.Xml.Serialization;
using GCProject.Main.Lib;

namespace GCProject.Main.Configs;

public class RegressionForTempConfig : NotifyBase
{
	private int _Order;

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
	public double StartTemp { get; set; }

	[XmlAttribute]
	public double EndTemp { get; set; }

	public double? StaticCorrection { get; set; }

	[XmlAttribute]
	public double Slope { get; set; }

	[XmlAttribute]
	public double Offset { get; set; }

	public double? Slope2 { get; set; }

	public double? Offset2 { get; set; }
}
