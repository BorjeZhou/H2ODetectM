using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using GCProject.Main.Log;
using log4net;

namespace GCProject.Main.Protocol;

public class SerialPortDataQueue
{
	public const int Timeout = 1000;

	private object QueueLock = new object();

	private Stopwatch sw = new Stopwatch();

	private List<byte> PlcData = new List<byte>();

	private ILog Log => GCLogger.log;

	public ConcurrentQueue<byte> DataQueue { get; private set; } = new ConcurrentQueue<byte>();


	private bool inError { get; set; }

	public event Action RebootSerialRequest;

	public byte TryGet()
	{
		int timeOut = 1000;
		while (timeOut > 0)
		{
			if (DataQueue.TryDequeue(out var res))
			{
				return res;
			}
			timeOut -= 20;
			Thread.Sleep(20);
		}
		return 0;
	}

	public void Clear()
	{
		DataQueue.Clear();
	}

	public void EnqueueRange(byte[] data, int len)
	{
		for (int i = 0; i < len; i++)
		{
			DataQueue.Enqueue(data[i]);
		}
	}

	public byte[] GetBusProtocolData(byte header1, byte header2, byte[] CMDs, out byte cmd)
	{
		int protocolLevel = 0;
		byte[] DataBody = null;
		int DatabodyIndex = 0;
		int checkSum = 0;
		int len = 0;
		int sum = 0;
		cmd = 0;
		int timeout = 1000;
		while (timeout > 0)
		{
			if (!DataQueue.TryDequeue(out var b))
			{
				timeout -= 10;
				Thread.Sleep(10);
				continue;
			}
			if (protocolLevel == 0)
			{
				if (b == header1)
				{
					protocolLevel++;
					continue;
				}
				ClearAll();
				cmd = 0;
				continue;
			}
			if (protocolLevel == 1)
			{
				if (b == header2)
				{
					protocolLevel++;
					continue;
				}
				if (b != byte.MaxValue)
				{
					Log.Error($"Header2 error 0x{b:X2}");
				}
				ClearAll();
				cmd = 0;
				continue;
			}
			if (protocolLevel == 2)
			{
				if (CMDs.Contains(b))
				{
					protocolLevel++;
					cmd = b;
					sum = b;
				}
				else
				{
					Log.Error($"Cmd error 0x{b:X2}");
					ClearAll();
					cmd = 0;
				}
				continue;
			}
			if (protocolLevel == 3)
			{
				len = b;
				DataBody = new byte[len];
				sum += b;
				protocolLevel++;
				continue;
			}
			if (len > 2)
			{
				sum += b;
				DataBody[DatabodyIndex++] = b;
			}
			else if (len == 2)
			{
				checkSum = b;
			}
			else if (len == 1)
			{
				checkSum += b << 8;
				if (checkSum == sum)
				{
					byte[] Res = new byte[DataBody.Length];
					Array.Copy(DataBody, Res, DataBody.Length);
					ClearAll();
					return Res;
				}
				Log.Error($"CRC error sum = {sum:X2} checkCRC = {checkSum:X4}");
				ClearAll();
				cmd = 0;
			}
			len--;
		}
		return null;
		void ClearAll()
		{
			DataBody = null;
			DatabodyIndex = 0;
			protocolLevel = 0;
			sum = 0;
			len = 0;
		}
	}

	private void OutPutRawData(List<byte> rawdata)
	{
		StringBuilder sb = new StringBuilder();
		foreach (byte b in rawdata)
		{
			sb.Append($"{b:X2} ");
		}
		GCLogger.log.Debug("one pack Raw Data :" + sb.ToString());
	}

	public byte[] GetPlcProtocolData(int slaveAddr, out byte cmd)
	{
		int index = 0;
		int remainLen = 0;
		int proLen = 0;
		PlcData.Clear();
		cmd = 0;
		int timeout = 1000;
		while (timeout > 0)
		{
			if (!DataQueue.TryDequeue(out var b))
			{
				timeout -= 10;
				Thread.Sleep(10);
				continue;
			}
			PlcData.Add(b);
			switch (index)
			{
			case 0:
				if (b == slaveAddr)
				{
					index++;
					break;
				}
				Log.Debug("slave addr error " + b);
				PlcData.Clear();
				index = 0;
				break;
			case 1:
				switch (b)
				{
				case 3:
					cmd = b;
					proLen = 8;
					remainLen = proLen - 2;
					index++;
					break;
				case 16:
					cmd = b;
					proLen = 11;
					remainLen = proLen - 2;
					index++;
					break;
				case 6:
					cmd = b;
					proLen = 8;
					remainLen = proLen - 2;
					index++;
					break;
				default:
					Log.Debug("plc cmd error " + b);
					PlcData.Clear();
					index = 0;
					break;
				}
				break;
			case 2:
				if (--remainLen <= 0)
				{
					byte[] crc = new byte[2]
					{
						PlcData[proLen - 2],
						PlcData[proLen - 1]
					};
					byte[] CalCRC = new byte[proLen - 2];
					Array.Copy(PlcData.ToArray(), 0, CalCRC, 0, CalCRC.Length);
					if (ModBusCRC16(CalCRC) == BitConverter.ToUInt16(crc))
					{
						return PlcData.ToArray();
					}
					index = 0;
				}
				break;
			}
		}
		return null;
	}

	public static ushort ModBusCRC16(byte[] pDataBytes)
	{
		ushort crc = ushort.MaxValue;
		ushort polynom = 40961;
		for (int i = 0; i < pDataBytes.Length; i++)
		{
			crc ^= pDataBytes[i];
			for (int j = 0; j < 8; j++)
			{
				if ((crc & 1) == 1)
				{
					crc >>= 1;
					crc ^= polynom;
				}
				else
				{
					crc >>= 1;
				}
			}
		}
		return crc;
	}
}
