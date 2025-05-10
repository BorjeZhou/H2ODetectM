using System;

namespace GCProject.Main.Protocol;

public class GCProtocolRecv6 : IProtocol
{
	public override byte FunctionCode => 20;

	public double A1Value { get; set; }

	public double A2Value { get; set; }

	public GCProtocolRecv6(byte[] DataBody)
	{
		queue = new ProtocolDataQueue(DataBody);
		PareData();
	}

	public void PareData()
	{
		queue.SkipBytes();
		A1Value = BitConverter.ToInt16(queue.TakeBytes(2));
		A2Value = BitConverter.ToInt16(queue.TakeBytes(2));
	}

	protected override byte[] BuildBodyData()
	{
		throw new NotImplementedException();
	}
}
