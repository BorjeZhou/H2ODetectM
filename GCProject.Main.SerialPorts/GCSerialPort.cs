using System;
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text;
using System.Threading;
using GCProject.Main.Configs;
using GCProject.Main.Log;
using GCProject.Main.Protocol;

namespace GCProject.Main.SerialPorts;

public class GCSerialPort
{
	public delegate void DataReceivedEventHandler(byte[] RecvByte);

	private SerialPort SerialPort;

	public ConcurrentQueue<string> TempLogDatas = new ConcurrentQueue<string>();

	private object serialLock = new object();

	public SerialPortDataQueue DataQueue { get; } = new SerialPortDataQueue();


	public bool IsOpen => SerialPort.IsOpen;

	public GCSerialPort(string ComName, int baudRate, Parity parity, int dataBit, StopBits stopBits)
	{
		SerialPort = new SerialPort();
		if (!string.IsNullOrEmpty(ComName))
		{
			SerialPort.PortName = ComName;
		}
		SerialPort.BaudRate = baudRate;
		SerialPort.BaudRate = baudRate;
		SerialPort.Parity = parity;
		SerialPort.DataBits = dataBit;
		SerialPort.StopBits = stopBits;
		SerialPort.ReceivedBytesThreshold = 1;
		SerialPort.DataReceived += SerialPort_DataReceived;
	}

	public GCSerialPort(GCSerialPortConfig config)
		: this(config.ComName, config.baudRate, config.parity, config.dateBit, config.stopBits)
	{
		DataQueue.RebootSerialRequest += DataQueue_RebootSerialRequest;
	}

	private void DataQueue_RebootSerialRequest()
	{
		ClearInOutDatas();
	}

	public void SetComName(string ComName)
	{
		if (SerialPort.IsOpen)
		{
			Close();
		}
		SerialPort.PortName = ComName;
	}

	public bool Open(out string error)
	{
		try
		{
			if (string.IsNullOrEmpty(SerialPort.PortName))
			{
				error = "PortName is Null";
				return false;
			}
			DataQueue.Clear();
			if (SerialPort.IsOpen)
			{
				error = string.Empty;
				return true;
			}
			SerialPort.Open();
			error = string.Empty;
			return true;
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return false;
		}
	}

	public void ClearInOutDatas()
	{
		lock (serialLock)
		{
			SerialPort.DiscardInBuffer();
			SerialPort.DiscardOutBuffer();
			DataQueue.Clear();
		}
	}

	public void Close()
	{
		try
		{
			if (SerialPort.IsOpen)
			{
				SerialPort.Close();
			}
		}
		catch (Exception)
		{
		}
	}

	private void AddLog(string Title, byte[] datas, int len)
	{
		ThreadPool.QueueUserWorkItem(delegate
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Title);
			for (int i = 0; i < len; i++)
			{
				stringBuilder.Append($"{datas[i]:X2} ");
			}
			TempLogDatas.Enqueue(stringBuilder.ToString());
		});
	}

	private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		try
		{
			byte[] recvBuffer = new byte[SerialPort.BytesToRead];
			int recvLen = SerialPort.Read(recvBuffer, 0, recvBuffer.Length);
			DataQueue.EnqueueRange(recvBuffer, recvLen);
		}
		catch (Exception ex)
		{
			GCLogger.log.Error("SerialPort_DataReceived Function Error");
			GCLogger.log.Error(ex.Message);
			GCLogger.log.Error(ex.StackTrace);
		}
	}

	public void SendByte(byte[] data)
	{
		try
		{
			SerialPort.DiscardOutBuffer();
			SerialPort.Write(data, 0, data.Length);
		}
		catch (Exception ex)
		{
			GCLogger.log.Error("SendByte Function Error");
			GCLogger.log.Error(ex.Message);
			GCLogger.log.Error(ex.StackTrace);
		}
	}
}
