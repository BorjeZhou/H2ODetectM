using System;
using System.Collections.Generic;

namespace GCProject.Main.Protocol;

public abstract class IProtocol
{
	public static readonly byte[] Header = new byte[2] { 109, 115 };

	protected ProtocolDataQueue queue;

	public virtual byte FunctionCode { get; protected set; }

	public virtual string ProtocolName { get; protected set; }

	protected virtual bool EnableRecv => true;

	protected virtual bool EnableSend => true;

	public byte[] RecvBodyData { get; protected set; }

	public byte[] SendData { get; protected set; }

	public byte[] SendBodyData { get; protected set; }

	public bool IsValid { get; protected set; } = true;


	public static string ExtractData(byte[] recvData, out byte[] BodyData, out byte FunctionCode)
	{
		FunctionCode = 0;
		BodyData = null;
		ProtocolDataQueue DataQueue = new ProtocolDataQueue(recvData);
		if (!DataQueue.EqualBytes(Header))
		{
			return "错误的数据头";
		}
		FunctionCode = DataQueue.TakeByte();
		int ProtocolLen = DataQueue.TakeByte();
		BodyData = DataQueue.TakeBytes(ProtocolLen - 2);
		int crc = BitConverter.ToInt16(DataQueue.TakeBytes(2));
		int crcAdd = FunctionCode + ProtocolLen;
		byte[] array = BodyData;
		foreach (byte b in array)
		{
			crcAdd += b;
		}
		if (crc != crcAdd)
		{
			BodyData = null;
			FunctionCode = 0;
			return "校验错误";
		}
		return string.Empty;
	}

	protected abstract byte[] BuildBodyData();

	public virtual byte[] ToBytes()
	{
		byte[] BodyData = BuildBodyData();
		int ProLen = 2 + BodyData.Length;
		int crc = FunctionCode + ProLen;
		byte[] array = BodyData;
		foreach (byte b in array)
		{
			crc += b;
		}
		byte[] calCRC = BitConverter.GetBytes((short)crc);
		List<byte> list = new List<byte>();
		list.AddRange(Header);
		list.Add(FunctionCode);
		list.Add((byte)ProLen);
		list.AddRange(BodyData);
		list.AddRange(calCRC);
		return list.ToArray();
	}
}
