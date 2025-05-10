using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using GCProject.Main.Aggregate;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.EventArgs;
using GCProject.Main.Log;
using GCProject.Main.Protocol;
using Microsoft.EntityFrameworkCore;

namespace GCProject.Main.SerialPorts;

public class GCSerialPortManager
{
	private static int PollQueryInterval_ms;

	private static Thread BusSerialPortRecvThread;

	private static Thread BusSerialPortSendThread;

	private static Thread BoardSerialPortSendThread;

	private static Thread BoardSerialPortRecvThread;

	private static Thread PlcSerialPortRecvThread;

	private static byte[] RequireProtocol1To5Bytes = new GCProtocolSend1To5().ToBytes();

	private static Timer SendH20AlarmInfoToPLCTimer = new Timer(SendH20Alarm);

	private static bool InSendPLCData;

	private static GCSerialPort BusSerialPort { get; set; }

	private static GCSerialPort BoardSerialPort { get; set; }

	private static GCSerialPort PlcSerialPort { get; set; }

	private static bool SerialPortTerminal { get; set; } = false;


	public static bool IsBusUseWithBoardSerialPort { get; private set; }

	public static bool BusSerialPortConn { get; private set; } = true;


	public static bool BoardSerialPortConn { get; private set; } = true;


	public static event Action<IProtocol> DataRecved;

	public static string[] GetCOMList()
	{
		return SerialPort.GetPortNames();
	}

	public static void Init()
	{
		PollQueryInterval_ms = ConfigModel.Instance.PollQueryInterval_ms;
		BusSerialPort = new GCSerialPort(ConfigModel.Instance.GCBusSerialPort);
		PlcSerialPort = new GCSerialPort(ConfigModel.Instance.GCPlcSerialPort);
		OpenPort();
	}

	public static void OpenPort()
	{
		CloseAll();
		IsBusUseWithBoardSerialPort = ConfigModel.Instance.GCBusSerialPort.ComName == ConfigModel.Instance.GCBoardSerialPort.ComName || string.IsNullOrEmpty(ConfigModel.Instance.GCBoardSerialPort.ComName);
		GCLogger.log.Info($"Board Com Reuse Bus COM:{IsBusUseWithBoardSerialPort}");
		string PortName = ConfigModel.Instance.GCBusSerialPort.ComName;
		SerialPortTerminal = false;
		if (!string.IsNullOrEmpty(PortName))
		{
			BusSerialPort.SetComName(PortName);
			if (BusSerialPort.Open(out var error3))
			{
				BusSerialPortRecvThread = new Thread(BusSerialHandData);
				BusSerialPortRecvThread.IsBackground = true;
				BusSerialPortRecvThread.Start();
				BusSerialPortSendThread = new Thread(BusSerialPollQuery);
				BusSerialPortSendThread.IsBackground = true;
				BusSerialPortSendThread.Start();
				GCLogger.log.Info("BUS port " + PortName + " open success");
			}
			else
			{
				GCLogger.log.Info("BUS port " + PortName + " open Failed :" + error3);
			}
		}
		PortName = ConfigModel.Instance.GCBoardSerialPort.ComName;
		if (!string.IsNullOrEmpty(PortName) && !IsBusUseWithBoardSerialPort)
		{
			if (BoardSerialPort == null)
			{
				BoardSerialPort = new GCSerialPort(ConfigModel.Instance.GCBoardSerialPort);
			}
			BoardSerialPort.SetComName(PortName);
			if (BoardSerialPort.Open(out var error2))
			{
				BoardSerialPortRecvThread = new Thread(BoardSerialHandData);
				BoardSerialPortRecvThread.IsBackground = true;
				BoardSerialPortRecvThread.Start();
				BoardSerialPortSendThread = new Thread(BoardSerialPollQuery);
				BoardSerialPortSendThread.IsBackground = true;
				BoardSerialPortSendThread.Start();
				GCLogger.log.Info("Board port " + PortName + " open success");
			}
			else
			{
				GCLogger.log.Info("Board port " + PortName + " open Failed :" + error2);
			}
		}
		PortName = ConfigModel.Instance.GCPlcSerialPort.ComName;
		if (!string.IsNullOrEmpty(PortName))
		{
			PlcSerialPort.SetComName(PortName);
			if (PlcSerialPort.Open(out var error))
			{
				PlcSerialPortRecvThread = new Thread(PlcSerialHandData);
				PlcSerialPortRecvThread.IsBackground = true;
				PlcSerialPortRecvThread.Start();
				GCLogger.log.Info("PLC port " + PortName + " open success");
			}
			else
			{
				GCLogger.log.Info("PLC port " + PortName + " open Failed :" + error);
			}
		}
		MainWindow.BusDataProxy.InRun = false;
	}

	private static void TerminalThread(Thread t)
	{
		if (t != null && t.IsAlive)
		{
			try
			{
				t.Abort();
			}
			catch
			{
			}
		}
	}

	public static void CloseAll()
	{
		SerialPortTerminal = true;
		Thread.Sleep(1100);
		TerminalThread(BusSerialPortSendThread);
		TerminalThread(BusSerialPortRecvThread);
		if (BusSerialPort != null && BusSerialPort.IsOpen)
		{
			BusSerialPort.Close();
		}
		TerminalThread(BoardSerialPortSendThread);
		TerminalThread(BoardSerialPortRecvThread);
		if (BoardSerialPort != null && BoardSerialPort.IsOpen)
		{
			BoardSerialPort.Close();
			BoardSerialPort = null;
		}
		TerminalThread(PlcSerialPortRecvThread);
		if (PlcSerialPort != null && PlcSerialPort.IsOpen)
		{
			PlcSerialPort.Close();
		}
	}

	private static void BusSerialPollQuery()
	{
		while (!SerialPortTerminal)
		{
			try
			{
				if (BusSerialPort != null && BusSerialPort.IsOpen)
				{
					BusSerialPort.SendByte(RequireProtocol1To5Bytes);
					if (IsBusUseWithBoardSerialPort)
					{
						Thread.Sleep(PollQueryInterval_ms / 2);
						BusSerialPort.SendByte(MainWindow.BusDataProxy.GetCurrentBoradSendProtocol());
						Thread.Sleep(PollQueryInterval_ms / 2);
					}
					else
					{
						Thread.Sleep(PollQueryInterval_ms);
					}
				}
				else
				{
					GCLogger.log.Info("SerialPort Closed ! Cannot SendCmd");
					Thread.Sleep(PollQueryInterval_ms);
				}
			}
			catch (Exception ex)
			{
				GCLogger.log.Error("BusSerialPollQuery function error");
				GCLogger.log.Error(ex.Message);
				GCLogger.log.Error(ex.StackTrace);
			}
		}
	}

	public static void SendH20Alarm(object _)
	{
		if (PlcSerialPort != null && PlcSerialPort.IsOpen)
		{
			try
			{
				PlcSerialPort.SendByte(Encoding.ASCII.GetBytes(ConfigModel.Instance.ParamSettingConfig.OutputAlarmInfo));
			}
			catch (Exception)
			{
			}
		}
	}

	public static void SendH20ValueToPLC(int index, double value)
	{
		if (PlcSerialPort == null || !PlcSerialPort.IsOpen || InSendPLCData)
		{
			return;
		}
		try
		{
			InSendPLCData = true;
			PlcSerialPort.SendByte(Encoding.ASCII.GetBytes($"{index},{value.ToString(ConfigModel.DecimalStr)}"));
		}
		catch (Exception)
		{
		}
		finally
		{
			InSendPLCData = false;
		}
	}

	public static void StartSendH20Alarm(bool Start)
	{
		if (Start)
		{
			SendH20AlarmInfoToPLCTimer.Change(0, 2000);
		}
		else
		{
			SendH20AlarmInfoToPLCTimer.Change(-1, -1);
		}
	}

	public static void BusSerialHandData()
	{
		IProtocol ResProtocol = null;
		while (!SerialPortTerminal)
		{
			try
			{
				byte cmd;
				byte[] bodyData = BusSerialPort.DataQueue.GetBusProtocolData(109, 115, ProtocolDefinde.GCProtocolRecvCMDs, out cmd);
				if (bodyData == null || bodyData.Length == 0)
				{
					BusSerialPortConn = false;
					continue;
				}
				BusSerialPortConn = true;
				switch (cmd)
				{
				case 16:
					ResProtocol = new GCProtocolRecv1To5(bodyData);
					break;
				case 20:
					ResProtocol = new GCProtocolBoardRecv(bodyData);
					break;
				}
				if (ResProtocol != null)
				{
					if (!ResProtocol.IsValid)
					{
						GCLogger.log.Error("Protocol parse error");
						OutPutRawData(bodyData);
					}
					GCSerialPortManager.DataRecved?.Invoke(ResProtocol);
				}
			}
			catch (Exception ex)
			{
				GCLogger.log.Error("BusSerialHandData function error");
				GCLogger.log.Error(ex.Message);
				GCLogger.log.Error(ex.StackTrace);
			}
		}
	}

	private static void BoardSerialPollQuery()
	{
		while (!SerialPortTerminal)
		{
			Thread.Sleep(PollQueryInterval_ms);
			try
			{
				if (BoardSerialPort != null && BoardSerialPort.IsOpen)
				{
					BoardSerialPort.SendByte(MainWindow.BusDataProxy.GetCurrentBoradSendProtocol());
				}
			}
			catch (Exception ex)
			{
				GCLogger.log.Error("BoardSerialPollQuery function error");
				GCLogger.log.Error(ex.Message);
				GCLogger.log.Error(ex.StackTrace);
			}
		}
	}

	public static void BoardSerialHandData()
	{
		IProtocol ResProtocol = null;
		while (!SerialPortTerminal)
		{
			try
			{
				byte cmd;
				byte[] bodyData = BoardSerialPort.DataQueue.GetBusProtocolData(109, 115, ProtocolDefinde.GCProtocolRecvCMDs, out cmd);
				if (bodyData == null || bodyData.Length == 0)
				{
					BoardSerialPortConn = false;
					continue;
				}
				BoardSerialPortConn = true;
				if (cmd == 20)
				{
					ResProtocol = new GCProtocolBoardRecv(bodyData);
				}
				if (ResProtocol != null)
				{
					if (!ResProtocol.IsValid)
					{
						GCLogger.log.Error("Protocol parse error");
						OutPutRawData(bodyData);
					}
					GCSerialPortManager.DataRecved?.Invoke(ResProtocol);
				}
			}
			catch (Exception ex)
			{
				GCLogger.log.Error("BoardSerialHandData function error");
				GCLogger.log.Error(ex.Message);
				GCLogger.log.Error(ex.StackTrace);
			}
		}
	}

	public static void OutPutRawData(byte[] rawdata)
	{
		StringBuilder sb = new StringBuilder();
		foreach (byte b in rawdata)
		{
			sb.Append($"{b:X2} ");
		}
		GCLogger.log.Debug("Raw Data :" + sb.ToString());
	}

	public static void PlcSerialHandData()
	{
		while (!SerialPortTerminal)
		{
			try
			{
				byte cmd;
				byte[] datas = PlcSerialPort.DataQueue.GetPlcProtocolData(ConfigModel.Instance.PLCSlaveAddr, out cmd);
				if (datas == null)
				{
					continue;
				}
				switch (cmd)
				{
				case 3:
				{
					int recordIndex = BitConverter.ToUInt16(new byte[2]
					{
						datas[3],
						datas[2]
					}, 0);
					GCSummaryEntity gcdatas = GetH20Data(recordIndex);
					if (gcdatas != null)
					{
						byte[] response = ResponseGetH20Protocol(ConfigModel.Instance.PLCSlaveAddr, gcdatas, recordIndex);
						PlcSerialPort.SendByte(response);
					}
					break;
				}
				case 6:
				case 16:
				{
					int sortNum = datas[datas.Length - 1 - 2];
					if (MainWindow.BusDataProxy.SelectedRecipe != null && (MainWindow.BusDataProxy.SelectedRecipe.RecipeNum == sortNum || PlcSerialPort.RequestEvent(new RequestChangeRecipeEvent(sortNum)) is RequestChangeRecipeEvent { Success: not false }))
					{
						byte[] response2 = null;
						response2 = ((cmd != 16) ? datas : ResponseSortMulti(ConfigModel.Instance.PLCSlaveAddr));
						PlcSerialPort.SendByte(response2);
					}
					break;
				}
				}
			}
			catch (Exception ex)
			{
				GCLogger.log.Error("PlcSerialHandData function error");
				GCLogger.log.Error(ex.Message);
				GCLogger.log.Error(ex.StackTrace);
			}
		}
	}

	public static GCSummaryEntity GetH20Data(int num)
	{
		using GCContext context = new GCContext();
		if (!(from s in context.GCSummaryEntities.AsNoTracking()
			where !s.IsSampleData
			select s).Any())
		{
			return null;
		}
		long lastDateID = (from s in context.GCSummaryEntities.AsNoTracking()
			where !s.IsSampleData
			select s).Max((GCSummaryEntity s) => s.GCSummaryEntityID);
		GCSummaryEntity lastData = context.GCSummaryEntities.AsNoTracking().FirstOrDefault((GCSummaryEntity s) => s.GCSummaryEntityID == lastDateID);
		if (lastData == null)
		{
			return null;
		}
		List<GCSummaryEntity> list = (from s in context.GCSummaryEntities.AsNoTracking()
			where !s.IsSampleData && s.StartDT >= lastData.StartDT.Date
			orderby s.GCSummaryEntityID
			select s).ToList();
		if (list.Count > 0)
		{
			if (num == 0)
			{
				num = list.Count;
			}
			if (num < 0 || num > list.Count)
			{
				return null;
			}
			num--;
			return list[num];
		}
		return null;
	}

	public static byte[] ResponseGetH20Protocol(int addr, GCSummaryEntity entity, int carnum)
	{
		List<byte> obj = new List<byte>
		{
			(byte)addr,
			3
		};
		List<byte> tempList = new List<byte>();
		if (carnum > 999)
		{
			carnum = 999;
		}
		string date = entity.EndDT.ToString(ConfigModel.Instance.ModBusDateFormat) + carnum.ToString().PadLeft(3, '0');
		tempList.AddRange(Encoding.ASCII.GetBytes(date));
		tempList.AddRange(Encoding.ASCII.GetBytes(FixH20String(entity.H20)));
		obj.Add((byte)tempList.Count);
		obj.AddRange(tempList);
		byte[] crcByte = BitConverter.GetBytes(SerialPortDataQueue.ModBusCRC16(obj.ToArray()));
		obj.AddRange(crcByte);
		return obj.ToArray();
	}

	public static string FixH20String(double H20)
	{
		if (H20 >= 100.0)
		{
			return "100.00";
		}
		if (H20 < 0.0)
		{
			H20 = 0.0;
		}
		string text = H20.ToString("F2");
		string h20Str_Left = text.Split('.')[0];
		h20Str_Left = h20Str_Left.PadLeft(2, '0');
		string h20Str_Right = text.Split('.')[1];
		h20Str_Right = h20Str_Right.PadRight(3, ' ');
		return h20Str_Left + "." + h20Str_Right;
	}

	public static string GetProtocolString(byte[] datas)
	{
		StringBuilder sb = new StringBuilder();
		byte[] protocolWithCRC = GetProtocolWithCRC(datas);
		foreach (byte b in protocolWithCRC)
		{
			sb.Append($"{b:X2} ");
		}
		return sb.ToString();
	}

	public static byte[] GetProtocolWithCRC(byte[] datas)
	{
		List<byte> list = new List<byte>(datas);
		byte[] crcByte = BitConverter.GetBytes(SerialPortDataQueue.ModBusCRC16(datas));
		list.AddRange(crcByte);
		return list.ToArray();
	}

	public static byte[] ResponseSortMulti(int addr)
	{
		List<byte> obj = new List<byte>
		{
			(byte)addr,
			16,
			2,
			0,
			0,
			1
		};
		byte[] crcByte = BitConverter.GetBytes(SerialPortDataQueue.ModBusCRC16(obj.ToArray()));
		obj.AddRange(crcByte);
		return obj.ToArray();
	}

	public static byte[] ResponseSortSingle(int addr)
	{
		List<byte> obj = new List<byte>
		{
			(byte)addr,
			6,
			2,
			0,
			0,
			1
		};
		byte[] crcByte = BitConverter.GetBytes(SerialPortDataQueue.ModBusCRC16(obj.ToArray()));
		obj.AddRange(crcByte);
		return obj.ToArray();
	}
}
