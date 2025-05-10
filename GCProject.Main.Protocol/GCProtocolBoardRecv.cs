using System;
using System.Collections.Generic;

namespace GCProject.Main.Protocol;

public class GCProtocolBoardRecv : IProtocol
{
	public override byte FunctionCode => 20;

	public double A1Value { get; set; }

	public double A2Value { get; set; }

	public double A3Value { get; set; }

	public double A4Value { get; set; }

	public byte Output { get; set; }

	public byte Input { get; set; }

	public GCProtocolBoardRecv(byte[] DataBody)
	{
		queue = new ProtocolDataQueue(DataBody);
		if (DataBody != null)
		{
			PareData();
		}
	}

	public void PareData()
	{
		byte[] a = queue.TakeBytes(2);
		A1Value = (int)BitConverter.ToUInt16(a);
		A2Value = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
		A3Value = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
		A4Value = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
		Output = queue.TakeByte();
		Input = queue.TakeByte();
	}

	protected override byte[] BuildBodyData()
	{
		List<byte> list = new List<byte>();
		list.AddRange(BitConverter.GetBytes((ushort)A1Value));
		list.AddRange(BitConverter.GetBytes((ushort)A2Value));
		list.AddRange(BitConverter.GetBytes((ushort)A3Value));
		list.AddRange(BitConverter.GetBytes((ushort)A4Value));
		list.Add(Output);
		list.Add(Input);
		return list.ToArray();
	}
}
