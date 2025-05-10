using System;
using System.Collections.Generic;

namespace GCProject.Main.Protocol;

public class GCProtocolBoardSend : IProtocol
{
	public override byte FunctionCode => 148;

	public ushort A1Value { get; set; }

	public ushort A2Value { get; set; }

	public ushort A3Value { get; set; }

	public ushort A4Value { get; set; }

	public byte Output4 { get; set; }

	public byte Input4 { get; set; }

	public void PareData()
	{
		throw new NotImplementedException();
	}

	protected override byte[] BuildBodyData()
	{
		List<byte> list = new List<byte>();
		list.AddRange(BitConverter.GetBytes(A1Value));
		list.AddRange(BitConverter.GetBytes(A2Value));
		list.AddRange(BitConverter.GetBytes(A3Value));
		list.AddRange(BitConverter.GetBytes(A4Value));
		list.Add(Output4);
		list.Add(Input4);
		return list.ToArray();
	}
}
