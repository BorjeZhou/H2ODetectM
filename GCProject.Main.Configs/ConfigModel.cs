using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Xml.Serialization;
using GCProject.Main.Data.Models;
using GCProject.Main.Lib;
using GCProject.Main.Log;

namespace GCProject.Main.Configs;

[XmlRoot(ElementName = "Config")]
public class ConfigModel
{
	public const string RunModel_24G = "2.4G";

	public const string RunModel_868M = "868M";

	public const string DefaultAdminPwd = "";

	public const string ConfigFileName = "Config.xml";

	public const string DataBaseFileName = "GCData.db";

	public const string LogDirectoryName = "Log";

	public const string DefaultInstType = "Bach-PMT9200/PMT9000";

	public const int DefaultPollQueryInterval_ms = 125;

	public const int DefaultCalAvgInterval_ms = 1000;

	public const string ConfigUpdateFile = "ConfigUpdate.txt";

	public const string Car1Type = "Car1Type";

	public const string Car2Type = "Car2Type";

	private static ConfigModel _instance;

	public static ConfigModel Instance
	{
		get
		{
			if (_instance == null)
			{
				InitConfigs();
			}
			return _instance;
		}
	}

	[XmlAttribute]
	public string ModBusDateFormat { get; set; } = "MM-dd-yyyyHH:mm:ss";


	[XmlAttribute]
	public string Language { get; set; } = "en-US";


	[XmlAttribute]
	public string EncyptPwd { get; set; } = PwdGuard.MD5Encrypt16("");


	[XmlAttribute]
	public string InstType { get; set; } = "Bach-PMT9200/PMT9000";


	[XmlAttribute]
	public int DataRetentionYear { get; set; } = 10;


	[XmlAttribute]
	public string DataBaseFileSave { get; set; } = Path.Combine("Data", "GCData.db");


	public string DataBaseFile => Path.Combine(Library.GetCurrentPath(), DataBaseFileSave);

	public string TempConfigFile => "tempConfig.xml";

	[XmlAttribute]
	public int PollQueryInterval_ms { get; set; } = 123;


	[XmlAttribute]
	public int CalAvgInterval_ms { get; set; } = 1000;


	public static List<GCRecipe> GCRecipes => Instance.ParamSettingConfig.Recipes;

	[XmlAttribute]
	public int Decimals { get; set; } = 2;


	public static string DecimalStr => $"F{Instance.Decimals}";

	[XmlElement]
	public GCSerialPortConfig GCBusSerialPort { get; set; } = new GCSerialPortConfig
	{
		ComName = "",
		baudRate = 38400,
		parity = Parity.None,
		dateBit = 8,
		stopBits = StopBits.One
	};


	[XmlElement]
	public GCSerialPortConfig GCBoardSerialPort { get; set; } = new GCSerialPortConfig
	{
		ComName = "",
		baudRate = 38400,
		parity = Parity.None,
		dateBit = 8,
		stopBits = StopBits.One
	};


	[XmlElement]
	public GCSerialPortConfig GCPlcSerialPort { get; set; } = new GCSerialPortConfig
	{
		ComName = "",
		baudRate = 9600,
		parity = Parity.None,
		dateBit = 8,
		stopBits = StopBits.One
	};


	[XmlAttribute]
	public int PLCSlaveAddr { get; set; } = 1;


	[XmlAttribute]
	public string SelectedRecipe { get; set; }

	[XmlAttribute]
	public string SelectedCar { get; set; } = "Car1Type";


	[XmlElement]
	public ParamSettingConfig ParamSettingConfig { get; set; } = new ParamSettingConfig();


	[XmlAttribute]
	public int MaxDispalyRecipeCount { get; set; } = 5;


	public void Save()
	{
		XmlSerialHelper.XmlStore<ConfigModel>(this);
	}

	public void Refresh()
	{
		InitConfigs();
	}

	public static void InitConfigs(bool NewCreate = false)
	{
		try
		{
			_instance = XmlSerialHelper.XmlRestore<ConfigModel>();
		}
		catch
		{
		}
		if (NewCreate || _instance == null)
		{
			_instance = new ConfigModel();
			CheckRecipe();
			XmlSerialHelper.XmlStore<ConfigModel>(_instance);
		}
		Lang.SetLangCode(_instance.Language);
		CheckRecipe();
	}

	private static void CheckRecipe()
	{
		if (_instance != null && !_instance.ParamSettingConfig.Recipes.Any())
		{
			_instance.ParamSettingConfig.Recipes.Add(new GCRecipe
			{
				DisplayName = "DefaultRecipe",
				LastModifyDate = DateTime.Now
			});
			GCLogger.log.Info("add default Recipe");
		}
		string dateFormat = "MM-dd-yyyyHH:mm:ss";
		if (string.IsNullOrEmpty(Instance.ModBusDateFormat))
		{
			Instance.ModBusDateFormat = dateFormat;
		}
	}

	public static void CheckOrCreateConfig()
	{
		_instance = XmlSerialHelper.XmlRestore<ConfigModel>();
		if (_instance == null)
		{
			InitConfigs(NewCreate: true);
			Instance.Save();
		}
	}

	public static ConfigModel GetTempConfigModel()
	{
		return XmlSerialHelper.XmlRestore<ConfigModel>();
	}

	public void CheckRecipeIfUpdate()
	{
		foreach (GCRecipe old in GetTempConfigModel().ParamSettingConfig.Recipes)
		{
			GCRecipes.FirstOrDefault((GCRecipe re) => re.GCRecipeID == old.GCRecipeID)?.CheckUpdate(old);
		}
	}
}
