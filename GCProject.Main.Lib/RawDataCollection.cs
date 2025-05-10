using System.Collections.Generic;
using System.Linq;
using GCProject.Main.Data.Models;

namespace GCProject.Main.Lib;

public class RawDataCollection
{
	public List<GCRawEntity> RawDatas = new List<GCRawEntity>();

	private object locker = new object();

	private bool InLock;

	public int Count
	{
		get
		{
			lock (locker)
			{
				return RawDatas.Count;
			}
		}
	}

	public void Add(GCRawEntity raw)
	{
		if (InLock)
		{
			return;
		}
		lock (locker)
		{
			InLock = true;
			raw.Index = (ushort)RawDatas.Count;
			RawDatas.Add(raw);
			InLock = false;
		}
	}

	public void Clear()
	{
		lock (locker)
		{
			RawDatas.Clear();
		}
	}

	public List<GCRawEntity> ToListAndClear()
	{
		List<GCRawEntity> res = new List<GCRawEntity>();
		lock (locker)
		{
			InLock = true;
			foreach (GCRawEntity raw in RawDatas)
			{
				res.Add(new GCRawEntity
				{
					CPS = raw.CPS,
					H20 = raw.H20,
					Width = raw.Width,
					Height = raw.Height,
					Index = raw.Index
				});
			}
			RawDatas.Clear();
			InLock = false;
			return res;
		}
	}

	public double GetAvgH2OValue()
	{
		lock (locker)
		{
			if (RawDatas.Count == 0)
			{
				return 0.0;
			}
			return RawDatas.Average((GCRawEntity c) => c.H20);
		}
	}

	public void DropSome(int DropCount)
	{
		lock (locker)
		{
			RawDatas.RemoveRange(RawDatas.Count - DropCount, DropCount);
		}
	}
}
