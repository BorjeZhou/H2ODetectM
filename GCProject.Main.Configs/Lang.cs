using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using GCProject.Main.LangResource;

namespace GCProject.Main.Configs;

public class Lang
{
	public const string Language_EN_Dispaly = "English";

	public const string Language_EN_Code = "en-US";

	public const string Language_ZH_Dispaly = "中文";

	public const string Language_ZH_Code = "zh-CN";

	public static List<Lang> SupportLangs => new List<Lang>
	{
		new Lang
		{
			Name = "English",
			Code = "en-US"
		},
		new Lang
		{
			Name = "中文",
			Code = "zh-CN"
		}
	};

	[XmlAttribute]
	public string Name { get; set; }

	[XmlAttribute]
	public string Code { get; set; }

	public static string GetCurrentLangCode()
	{
		return ConfigModel.Instance.Language;
	}

	public static void SetLangCode(string Code)
	{
		try
		{
			GCProject.Main.LangResource.Lang.Culture = (CultureInfo.CurrentUICulture = (CultureInfo.CurrentCulture = new CultureInfo(Code)));
		}
		catch (Exception)
		{
		}
	}
}
