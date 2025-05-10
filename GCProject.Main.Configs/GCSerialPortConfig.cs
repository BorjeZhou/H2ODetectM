using System.IO.Ports;
using System.Xml.Serialization;

namespace GCProject.Main.Configs;

public class GCSerialPortConfig
{
	[XmlAttribute]
	public int ID { get; set; }

	[XmlAttribute]
	public string ComName { get; set; }

	[XmlAttribute]
	public int baudRate { get; set; }

	[XmlAttribute]
	public Parity parity { get; set; }

	[XmlAttribute]
	public int dateBit { get; set; }

	[XmlAttribute]
	public StopBits stopBits { get; set; }
}
