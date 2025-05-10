using System.Threading;
using GCProject.Main.Chart;
using GCProject.Main.Configs;
using GCProject.Main.SerialPorts;

namespace GCProject.Main.Lib;

public class DrawChartDriver
{
	private Thread RunThread;

	public BusDataProxy DataProxy { get; private set; }

	public GCMainChart MainChart { get; set; }

	public DrawChartDriver(BusDataProxy bus)
	{
		RunThread = new Thread(Dowork);
		RunThread.IsBackground = true;
		DataProxy = bus;
		MainChart = bus.MainChart;
		RunThread.Start();
	}

	public void Dowork()
	{
		int count = 0;
		while (true)
		{
			Thread.Sleep(ConfigModel.Instance.PollQueryInterval_ms);
			if (!DataProxy.InRun || DataProxy.InSkipPoint || DataProxy.InMeastimeout || !GCSerialPortManager.BusSerialPortConn)
			{
				count = 0;
				continue;
			}
			double h20 = DataProxy.H20_Real__;
			MainChart.AddWavePoint(h20);
			if (++count != ConfigModel.Instance.ParamSettingConfig.OneBarWidthPoint)
			{
				continue;
			}
			if (ConfigModel.Instance.ParamSettingConfig.DrawChartType == DrawChartTypeEnum.RealWave)
			{
				double h20Bar = DataProxy.GetCurrentH20();
				MainChart.AddBarPoint(h20Bar);
				int index2 = MainChart.GetCurrentBarCount();
				ThreadPool.QueueUserWorkItem(delegate
				{
					GCSerialPortManager.SendH20ValueToPLC(index2, h20Bar);
				});
			}
			else
			{
				MainChart.AddBarPoint(h20);
				int index = MainChart.GetCurrentBarCount();
				ThreadPool.QueueUserWorkItem(delegate
				{
					GCSerialPortManager.SendH20ValueToPLC(index, h20);
				});
			}
			count = 0;
		}
	}
}
