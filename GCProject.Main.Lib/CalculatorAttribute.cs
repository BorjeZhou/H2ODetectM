using System;
using System.Collections.Generic;
using System.Linq;
using GCProject.Main.Configs;

namespace GCProject.Main.Lib;

public class CalculatorAttribute : CalculatorBaseAttribute
{
	private object RawDatalocker = new object();

	private double _result;

	private object ResultLocker = new object();

	public bool InRecursive { get; set; } = true;


	private List<double> RawDetailDatas { get; set; } = new List<double>();


	public int DataCount => RawDetailDatas.Count;

	private double Result
	{
		get
		{
			double res = 0.0;
			lock (ResultLocker)
			{
				return Math.Round(_result, base.Decimals);
			}
		}
		set
		{
			lock (ResultLocker)
			{
				_result = value;
			}
		}
	}

	public void CalRes()
	{
		lock (RawDatalocker)
		{
			calRes();
		}
	}

	private void calRes()
	{
		if (!RawDetailDatas.Any())
		{
			if (CalType == CalTypeEnum.Sum_1s)
			{
				Result = 0.0;
			}
			return;
		}
		if (CalType == CalTypeEnum.Average_Total)
		{
			Result = RawDetailDatas.Average();
			return;
		}
		if (CalType == CalTypeEnum.Average_1s)
		{
			Result = RawDetailDatas.Average();
		}
		else if (CalType == CalTypeEnum.Sum_1s)
		{
			Result = RawDetailDatas.Sum();
		}
		else if (CalType == CalTypeEnum.Recursive)
		{
			int countPoint = ConfigModel.Instance.ParamSettingConfig.RecursiveFilterDuration * 8;
			if (base.Name == "DynamicZeroRate")
			{
				countPoint = ConfigModel.Instance.ParamSettingConfig.ZeroRateRecursiveDuration * 8;
			}
			if (countPoint < 8)
			{
				countPoint = 4800;
			}
			int removePoint = RawDetailDatas.Count - countPoint;
			if (removePoint > 0)
			{
				RawDetailDatas.RemoveRange(0, removePoint);
			}
			if (InRecursive)
			{
				if (RawDetailDatas.Count > 0)
				{
					Result = RawDetailDatas.Average();
				}
				return;
			}
			int startIndex = RawDetailDatas.Count - 8;
			if (startIndex < 0)
			{
				startIndex = 0;
			}
			List<double> res = new List<double>();
			for (int i = startIndex; i < RawDetailDatas.Count - 1; i++)
			{
				res.Add(RawDetailDatas[i]);
			}
			if (res.Count > 0)
			{
				Result = res.Average();
			}
			return;
		}
		RawDetailDatas.Clear();
	}

	public void DropPoint(int drop)
	{
		lock (RawDatalocker)
		{
			if (drop > RawDetailDatas.Count)
			{
				drop = RawDetailDatas.Count;
			}
			RawDetailDatas.RemoveRange(RawDetailDatas.Count - drop, drop);
			calRes();
		}
	}

	public void Update(double newValue)
	{
		if (double.IsNaN(newValue) || double.IsInfinity(newValue) || CalType == CalTypeEnum.None)
		{
			return;
		}
		lock (RawDatalocker)
		{
			RawDetailDatas.Add(newValue);
			if (base.UpdateImmediately)
			{
				calRes();
			}
		}
	}

	public double GetResult(bool ClearDatas = false)
	{
		if (CalType == CalTypeEnum.None)
		{
			throw new Exception("Current Property do not have Calculate Type");
		}
		return Result;
	}

	public void Clear()
	{
		lock (RawDatalocker)
		{
			RawDetailDatas.Clear();
		}
	}
}
