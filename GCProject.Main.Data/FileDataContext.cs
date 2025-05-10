using System;
using System.Collections.Generic;
using System.IO;

namespace GCProject.Main.Data;

public class FileDataContext
{
	public const string DatetimeFormat = "yyyy_MM_dd";

	public static List<YearDataEntity> DataEntities;

	private static string DataRootPath => Path.Combine(Environment.CurrentDirectory, "Datas");

	public static void CheckOrCreatePath(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void Init()
	{
		GetAndCreateTodayDirectory();
		DataEntities = ScanDatas();
	}

	public static string GetAndCreateTodayDirectory()
	{
		DateTime now = DateTime.Now;
		string text = Path.Combine(DataRootPath, now.Year.ToString(), now.Month.ToString());
		CheckOrCreatePath(text);
		return text;
	}

	public static List<YearDataEntity> ScanDatas()
	{
		List<YearDataEntity> Result = new List<YearDataEntity>();
		string[] directories = Directory.GetDirectories(DataRootPath);
		foreach (string YearItem in directories)
		{
			YearDataEntity YearData = new YearDataEntity(Path.GetFileNameWithoutExtension(YearItem), YearItem);
			Result.Add(YearData);
			string[] directories2 = Directory.GetDirectories(YearData.Path);
			foreach (string MothItem in directories2)
			{
				MothDataEntity MothData = new MothDataEntity(Path.GetFileNameWithoutExtension(MothItem), MothItem);
				YearData.MothDatas.Add(MothData);
			}
		}
		return Result;
	}
}
