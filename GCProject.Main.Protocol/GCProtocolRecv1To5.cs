using System;
using System.Collections.Generic;
using GCProject.Main.Log;

namespace GCProject.Main.Protocol;

public class GCProtocolRecv1To5 : IProtocol
{
	public override byte FunctionCode => 16;

	public int ProtocolNum { get; set; }

	public double MircroValue { get; set; }

	public double TempValue { get; set; }

	public double DistanceValue { get; set; }

	public GCProtocolRecv1To5()
	{
	}

	public GCProtocolRecv1To5(byte[] DataBody)
	{
		queue = new ProtocolDataQueue(DataBody);
		PareData();
	}

	public void PareData()
	{
		try
		{
			string ProNum = queue.TakeByte().ToString("X");
			if (ProNum.Length > 1)
			{
				ProNum = ProNum.Substring(1, 1);
			}
			ProtocolNum = int.Parse(ProNum);
			if (ProtocolNum > 6)
			{
				GCLogger.log.Error("GCProtocolRecv1To5 ProtocolNum error > 6");
				base.IsValid = false;
			}
			MircroValue = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
			TempValue = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
			DistanceValue = (int)BitConverter.ToUInt16(queue.TakeBytes(2));
		}
		catch (Exception ex)
		{
			GCLogger.log.Error("GCProtocolRecv1To5 PareData error");
			GCLogger.log.Error(ex.Message);
			GCLogger.log.Error(ex.StackTrace);
			base.IsValid = false;
		}
	}

	protected override byte[] BuildBodyData()
	{
		byte pronum = 1;
		switch (ProtocolNum)
		{
		case 1:
			pronum = 1;
			break;
		case 2:
			pronum = 18;
			break;
		case 3:
			pronum = 3;
			break;
		case 4:
			pronum = 20;
			break;
		case 5:
			pronum = 5;
			break;
		}
		List<byte> list = new List<byte>();
		list.Add(pronum);
		list.AddRange(BitConverter.GetBytes((short)MircroValue));
		list.AddRange(BitConverter.GetBytes((short)TempValue));
		list.AddRange(BitConverter.GetBytes((short)DistanceValue));
		list.Add(pronum);
		return list.ToArray();
	}
}
