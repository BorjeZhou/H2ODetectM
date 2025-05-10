using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GCProject.Main.Configs;
using GCProject.Main.Log;

namespace GCProject.Main.Lib;

public class UpdateHelper
{
	public static void CheckConfigModelUpdate()
	{
		string UpdateFile = Path.Combine(Library.GetCurrentPath(), "ConfigUpdate.txt");
		if (!File.Exists(UpdateFile))
		{
			return;
		}
		try
		{
			List<string> configs = new List<string>();
			using (StreamReader sr = new StreamReader(UpdateFile, Encoding.UTF8))
			{
				string oneConfig = null;
				while (!string.IsNullOrEmpty(oneConfig = sr.ReadLine()))
				{
					if (!oneConfig.StartsWith("//"))
					{
						configs.Add(oneConfig);
					}
				}
			}
			if (configs.Count == 0)
			{
				return;
			}
			ConfigModel config = ConfigModel.Instance;
			foreach (string item in configs)
			{
				string[] res = item.Split('=');
				if (res.Length != 2)
				{
					continue;
				}
				string key = res[0].Trim();
				string value = res[1].Trim();
				string[] keys = key.Split('.');
				if (keys.Length > 1)
				{
					keys = keys.Select((string k) => k.Trim()).ToArray();
					RecursivelySetValue(config, keys, value);
				}
				else
				{
					SetPropertyValue(config, key, value);
				}
				GCLogger.log.Info("Update ConfigModel " + item);
			}
			File.Delete(UpdateFile);
			ConfigModel.Instance.Save();
			GCLogger.log.Info("Update ConfigModel Success");
		}
		catch (Exception ex)
		{
			GCLogger.log.Error("Try Update ConfigModel Error");
			GCLogger.log.Error(ex.Message);
			GCLogger.log.Error(ex.StackTrace);
		}
	}

	private static void RecursivelySetValue(object Target, string[] ProCliams, string value)
	{
		for (int i = 0; i < ProCliams.Length - 1; i++)
		{
			Target = GetPropertyValue(Target, ProCliams[i]);
		}
		SetPropertyValue(Target, ProCliams.Last(), value);
	}

	private static object GetPropertyValue(object Target, string ProName)
	{
		object Res = null;
		if (string.IsNullOrEmpty(ProName) || Target == null)
		{
			return Res;
		}
		PropertyInfo pro = Target.GetType().GetProperty(ProName);
		if (pro != null)
		{
			Res = pro.GetValue(Target);
		}
		return Res;
	}

	private static void SetPropertyValue(object Target, string ProName, string value)
	{
		if (string.IsNullOrEmpty(ProName) || Target == null)
		{
			return;
		}
		PropertyInfo tarPro = Target.GetType().GetProperty(ProName);
		if (!(tarPro != null))
		{
			return;
		}
		try
		{
			object targetVal = null;
			targetVal = ((!tarPro.PropertyType.IsSubclassOf(typeof(Enum))) ? Convert.ChangeType(value, tarPro.PropertyType) : Enum.Parse(tarPro.PropertyType, value));
			tarPro.SetValue(Target, targetVal);
		}
		catch (Exception)
		{
		}
	}
}
