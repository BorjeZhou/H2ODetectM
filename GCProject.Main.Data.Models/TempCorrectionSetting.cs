using System.Collections.Generic;
using System.Xml.Serialization;
using GCProject.Main.Configs;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using GCProject.Main.Windows;

namespace GCProject.Main.Data.Models;

public class TempCorrectionSetting : NotifyBase
{
	private bool _useStatic = true;

	public static List<DisplayObject> CorrectionTypes { get; set; } = new List<DisplayObject>
	{
		new DisplayObject
		{
			Text = GCProject.Main.LangResource.Lang.StaticCorrection,
			Value = true
		},
		new DisplayObject
		{
			Text = GCProject.Main.LangResource.Lang.RegressionCorrection,
			Value = false
		}
	};


	[XmlAttribute]
	public bool UseStatic
	{
		get
		{
			return _useStatic;
		}
		set
		{
			if (_useStatic != value)
			{
				_useStatic = value;
				OnPropertyChanged("UseStatic");
			}
		}
	}

	[XmlAttribute]
	public double WeightAvgR { get; set; }

	[XmlArray]
	public List<RegressionForTempConfig> StaticConfigs { get; set; } = new List<RegressionForTempConfig>();


	[XmlArray]
	public List<RegressionForTempConfig> RegressionConfig { get; set; } = new List<RegressionForTempConfig>();


	[XmlArray]
	public List<SpliteTemp> RegressionSplitTempList { get; set; } = new List<SpliteTemp>();

}
