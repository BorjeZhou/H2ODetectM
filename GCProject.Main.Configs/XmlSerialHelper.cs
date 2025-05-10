using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using GCProject.Main.Lib;

namespace GCProject.Main.Configs;

public class XmlSerialHelper
{
	public static bool XmlStore<ConfigEntity>(object obj, string file = "Config.xml") where ConfigEntity : class, new()
	{
		try
		{
			file = Path.Combine(Library.GetCurrentPath(), file);
			using (StreamWriter sw = new StreamWriter(file, append: false, Encoding.UTF8))
			{
				XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
				namespaces.Add(string.Empty, string.Empty);
				new XmlSerializer(typeof(ConfigEntity)).Serialize(sw, obj, namespaces);
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static ConfigEntity XmlRestore<ConfigEntity>(string fileName = "Config.xml") where ConfigEntity : class, new()
	{
		try
		{
			fileName = Path.Combine(Library.GetCurrentPath(), fileName);
			if (!File.Exists(fileName))
			{
				return null;
			}
			using StreamReader SR = new StreamReader(fileName);
			return new XmlSerializer(typeof(ConfigEntity)).Deserialize(SR) as ConfigEntity;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
