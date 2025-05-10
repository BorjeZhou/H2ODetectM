using System;
using System.Reflection;
using System.Text;

namespace GCProject.Main.Protocol;

public class GCProtocolRecv1To5Package : IProtocol
{
	public double Protocol_1_Value1 => Protocol1.MircroValue;

	public double Protocol_1_Value2 => Protocol1.TempValue;

	public double Protocol_1_Value3 => Protocol1.DistanceValue;

	public bool Protocol_1_Recved => Protocol1 != null;

	public GCProtocolRecv1To5 Protocol1 { get; set; }

	public double Rx_868M_MicroValue => Protocol2.MircroValue;

	public double Rx_868M_TempValue => Protocol2.TempValue;

	public double Rx_868M_DistanceValue => Protocol2.DistanceValue;

	public bool Rx_868M_Recved => Protocol2 != null;

	public GCProtocolRecv1To5 Protocol2 { get; set; }

	public double Tx_868M_MicroValue => Protocol3.MircroValue;

	public double Tx_868M_TempValue => Protocol3.TempValue;

	public double Tx_868M_DistanceValue => Protocol3.DistanceValue;

	public bool Tx_868M_Recved => Protocol3 != null;

	public GCProtocolRecv1To5 Protocol3 { get; set; }

	public double Rx_24G_MicroValue => Protocol4.MircroValue;

	public double Rx_24G_TempValue => Protocol4.TempValue;

	public double Rx_24G_DistanceValue => Protocol4.DistanceValue;

	public bool Rx_24G_Recved => Protocol4 != null;

	public GCProtocolRecv1To5 Protocol4 { get; set; }

	public double Tx_24G_MicroValue => Protocol5.MircroValue;

	public double Tx_24G_TempValue => Protocol5.TempValue;

	public double Tx_24G_DistanceValue => Protocol5.DistanceValue;

	public bool Tx_24G_Recved => Protocol5 != null;

	public GCProtocolRecv1To5 Protocol5 { get; set; }

	public void SetGCPRotocolRecv1To5(GCProtocolRecv1To5 protocol)
	{
		switch (protocol.ProtocolNum)
		{
		case 1:
			Protocol1 = protocol;
			break;
		case 2:
			Protocol2 = protocol;
			break;
		case 3:
			Protocol3 = protocol;
			break;
		case 4:
			Protocol4 = protocol;
			break;
		case 5:
			Protocol5 = protocol;
			break;
		}
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		PropertyInfo[] properties = GetType().GetProperties();
		foreach (PropertyInfo prop in properties)
		{
			sb.AppendLine($"{prop.Name}:{prop.GetValue(this)}");
		}
		return sb.ToString();
	}

	protected override byte[] BuildBodyData()
	{
		throw new NotImplementedException();
	}
}
