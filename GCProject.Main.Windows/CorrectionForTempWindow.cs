using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using GCProject.Main.Log;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;
using OxyPlot;

namespace GCProject.Main.Windows;

public partial class CorrectionForTempWindow : System.Windows.Window, IWindow, IComponentConnector
{
	private CorrectionForTempWindow_ViewModel ViewModel { get; set; } = new CorrectionForTempWindow_ViewModel();


	public CorrectionForTempWindow()
	{
		InitializeComponent();
		Init(null);
	}

	public void Init(object args)
	{
		try
		{
			GCRecipe recipe = args as GCRecipe;
			if (recipe != null)
			{
				ViewModel.SelectedRecipe = recipe;
				ViewModel.UseCustomTempCorrection = recipe.UseCustomTempCorrection;
				ViewModel.UseStatic = recipe.TempCorrectionSetting.UseStatic;
				ViewModel.WeightAvgR = recipe.TempCorrectionSetting.WeightAvgR;
				ViewModel.StaticConfigs.Clear();
				recipe.TempCorrectionSetting.StaticConfigs.ForEach(delegate(RegressionForTempConfig s)
				{
					ViewModel.StaticConfigs.Add(s);
				});
				ViewModel.RegressionConfig.Clear();
				recipe.TempCorrectionSetting.RegressionConfig = recipe.TempCorrectionSetting.RegressionConfig.OrderBy((RegressionForTempConfig r) => r.StartTemp).ToList();
				recipe.TempCorrectionSetting.RegressionConfig.ForEach(delegate(RegressionForTempConfig s)
				{
					ViewModel.RegressionConfig.Add(s);
				});
				recipe.TempCorrectionSetting.RegressionSplitTempList = recipe.TempCorrectionSetting.RegressionSplitTempList.OrderBy((SpliteTemp c) => c).ToList();
				ViewModel.RegressionSplitTempList.Clear();
				recipe.TempCorrectionSetting.RegressionSplitTempList.ForEach(delegate(SpliteTemp c)
				{
					ViewModel.RegressionSplitTempList.Add(c);
				});
				base.DataContext = null;
				base.DataContext = ViewModel;
				ViewModel.CorrectionPoints.Clear();
				(from g in new GCContext().GCSummaryEntities.AsNoTracking()
					where g.GCRecipeID == recipe.GCRecipeID && g.IsTempSelected
					select g).ToList().ForEach(delegate(GCSummaryEntity c)
				{
					ViewModel.CorrectionPoints.Add(new DataPoint(c.Temperature, c.Diff_H20_OnlyCPS));
				});
				RefreshClick(null, null);
			}
		}
		catch (Exception e)
		{
			GCLogger.log.Error(e.Message);
			GCLogger.log.Error(e.StackTrace);
		}
	}

	private void AddStaticConfigClick(object sender, RoutedEventArgs e)
	{
		ViewModel.StaticConfigs.Add(new RegressionForTempConfig
		{
			StaticCorrection = 0.0,
			Order = ViewModel.StaticConfigs.Count
		});
	}

	private void RemoveStaticConfigClick(object sender, RoutedEventArgs e)
	{
		if (!(dg_static.SelectedItem is RegressionForTempConfig rf))
		{
			return;
		}
		ViewModel.StaticConfigs.Remove(rf);
		int i = 0;
		foreach (RegressionForTempConfig staticConfig in ViewModel.StaticConfigs)
		{
			staticConfig.Order = i++;
		}
	}

	private void AddRegressionTempSplitClick(object sender, RoutedEventArgs e)
	{
		int max = 3;
		if (ViewModel.RegressionSplitTempList.Count >= max)
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.CTW_Msg1, GCProject.Main.LangResource.Lang.Error);
			return;
		}
		ViewModel.RegressionSplitTempList.Add(new SpliteTemp
		{
			Temp = 50.0,
			Order = ViewModel.RegressionSplitTempList.Count
		});
	}

	private void RemoveRegressionTempSplitClick(object sender, RoutedEventArgs e)
	{
		if (!(dg_tempSplit.SelectedItem is SpliteTemp rf))
		{
			return;
		}
		ViewModel.RegressionSplitTempList.Remove(rf);
		int i = 0;
		foreach (SpliteTemp regressionSplitTemp in ViewModel.RegressionSplitTempList)
		{
			regressionSplitTemp.Order = i++;
		}
	}

	private void SaveClick(object sender, RoutedEventArgs e)
	{
		string err = null;
		if (ViewModel.UseCustomTempCorrection && ViewModel.UseStatic)
		{
			err = Library.CheckDataGridError(dg_static);
			foreach (RegressionForTempConfig staticConfig in ViewModel.StaticConfigs)
			{
				if (!staticConfig.StaticCorrection.HasValue)
				{
					err = GCProject.Main.LangResource.Lang.CTW_Msg2;
					break;
				}
			}
		}
		else if (ViewModel.UseCustomTempCorrection && !ViewModel.UseStatic)
		{
			err = Library.CheckDataGridError(dg_regression);
			if (string.IsNullOrEmpty(err) && ViewModel.RegressionConfig.Count == 0)
			{
				err = "No regression data";
			}
		}
		if (!string.IsNullOrEmpty(err))
		{
			MessageBox.Show(err, GCProject.Main.LangResource.Lang.Error);
			return;
		}
		_ = ViewModel;
		GCRecipe configRecipe = ConfigModel.Instance.ParamSettingConfig.Recipes.FirstOrDefault((GCRecipe rc) => rc.DisplayName == ViewModel.SelectedRecipe.DisplayName);
		if (configRecipe == null)
		{
			return;
		}
		configRecipe.UseCustomTempCorrection = ViewModel.UseCustomTempCorrection;
		configRecipe.TempCorrectionSetting.WeightAvgR = ViewModel.WeightAvgR;
		configRecipe.TempCorrectionSetting.UseStatic = ViewModel.UseStatic;
		configRecipe.TempCorrectionSetting.StaticConfigs = ViewModel.StaticConfigs.ToList();
		configRecipe.TempCorrectionSetting.RegressionConfig = ViewModel.RegressionConfig.OrderBy((RegressionForTempConfig c) => c.StartTemp).ToList();
		configRecipe.TempCorrectionSetting.RegressionSplitTempList = ViewModel.RegressionSplitTempList.OrderBy((SpliteTemp c) => c).ToList();
		ConfigModel.Instance.Save();
		GCContext db = new GCContext();
		foreach (GCSummaryEntity item in db.GCSummaryEntities.Where((GCSummaryEntity g) => g.GCRecipeID == ViewModel.SelectedRecipe.GCRecipeID && g.IsSampleData).ToList())
		{
			item.CalculatorValueByRecipe(configRecipe, ConfigModel.Instance.ParamSettingConfig);
		}
		db.SaveChanges();
		MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SaveSuccess, GCProject.Main.LangResource.Lang.SAVE);
	}

	private List<RegressionForTempConfig> GetAndRefreshTempCorretionConfigs()
	{
		List<RegressionForTempConfig> result = new List<RegressionForTempConfig>();
		List<List<DataPoint>> loadDatas = new List<List<DataPoint>>();
		List<double> splitTemps = null;
		if (ViewModel.UseCustomTempCorrection && ViewModel.UseStatic)
		{
			foreach (RegressionForTempConfig config in ViewModel.StaticConfigs)
			{
				List<DataPoint> line = new List<DataPoint>();
				line.Add(new DataPoint(config.StartTemp, config.StaticCorrection.Value));
				line.Add(new DataPoint(config.EndTemp, config.StaticCorrection.Value));
				loadDatas.Add(line);
				result.Add(new RegressionForTempConfig
				{
					StartTemp = config.StartTemp,
					EndTemp = config.EndTemp,
					StaticCorrection = config.StaticCorrection.Value
				});
			}
		}
		else if (ViewModel.UseCustomTempCorrection && !ViewModel.UseStatic)
		{
			List<LineInfo> lines = new List<LineInfo>();
			splitTemps = new List<double>();
			double step = 0.1;
			if (ViewModel.WeightAvgR < 0.0)
			{
				throw new Exception("invalid param");
			}
			double avgStepRange = ViewModel.WeightAvgR;
			double minTemp = ViewModel.CorrectionPoints.Select((DataPoint d) => d.X).Min() - 5.0;
			double maxTemp = ViewModel.CorrectionPoints.Select((DataPoint d) => d.X).Max() + 5.0;
			double min = minTemp;
			double max = minTemp;
			foreach (SpliteTemp item in ViewModel.RegressionSplitTempList.OrderBy((SpliteTemp d) => d.Temp))
			{
				double temp = item.Temp;
				splitTemps.Add(temp);
				max = Math.Max(max, temp);
				var (offset, slope) = GetSlopeOffset(ViewModel.CorrectionPoints.Where((DataPoint p) => p.X > min && p.X <= max).ToList());
				lines.Add(new LineInfo
				{
					StartTemp = min,
					EndTemp = max,
					Slope = slope,
					Offset = offset
				});
				min = max;
			}
			max = maxTemp;
			var (offset2, slope2) = GetSlopeOffset(ViewModel.CorrectionPoints.Where((DataPoint p) => p.X > min && p.X <= max).ToList());
			lines.Add(new LineInfo
			{
				StartTemp = min,
				EndTemp = max,
				Slope = slope2,
				Offset = offset2
			});
			if (lines.Count > 1 && ViewModel.WeightAvgR > 0.0)
			{
				for (int i = 0; i < lines.Count; i++)
				{
					LineInfo line3 = lines[i];
					line3.EndTemp -= avgStepRange;
					AddLine(line3);
					RegressionForTempConfig config2 = new RegressionForTempConfig
					{
						StartTemp = line3.StartTemp,
						EndTemp = line3.EndTemp,
						Offset = line3.Offset,
						Slope = line3.Slope
					};
					result.Add(config2);
					if (i + 1 < lines.Count)
					{
						LineInfo line4 = lines[i + 1];
						line4.StartTemp += avgStepRange;
						config2 = new RegressionForTempConfig
						{
							StartTemp = line3.EndTemp,
							EndTemp = line4.StartTemp,
							Offset = line3.Offset,
							Slope = line3.Slope,
							Offset2 = line4.Offset,
							Slope2 = line4.Slope
						};
						result.Add(config2);
						double r_1 = line3.EndTemp;
						double r_2 = line4.StartTemp;
						List<DataPoint> linePoints = new List<DataPoint>();
						for (double j = r_1; j <= r_2; j += step)
						{
							double rate2 = (j - r_1) / (avgStepRange * 2.0);
							double rate1 = 1.0 - rate2;
							double num = line3.callValue(j);
							double v2 = line4.callValue(j);
							double res = num * rate1 + v2 * rate2;
							linePoints.Add(new DataPoint(j, res));
						}
						linePoints.Add(new DataPoint(r_2, line4.callValue(r_2)));
						loadDatas.Add(linePoints);
					}
				}
			}
			else
			{
				foreach (LineInfo line2 in lines)
				{
					AddLine(line2);
					RegressionForTempConfig config3 = new RegressionForTempConfig
					{
						StartTemp = line2.StartTemp,
						EndTemp = line2.EndTemp,
						Offset = line2.Offset,
						Slope = line2.Slope
					};
					result.Add(config3);
				}
			}
		}
		if (result.Any((RegressionForTempConfig r) => r.StartTemp > r.EndTemp))
		{
			throw new Exception("invalid param");
		}
		MainChart.LoadCorrectionData(ViewModel.CorrectionPoints, loadDatas, splitTemps);
		MainChart.UpdateAll();
		return result;
		void AddLine(LineInfo lineInfo)
		{
			List<DataPoint> line5 = new List<DataPoint>
			{
				new DataPoint(lineInfo.StartTemp, lineInfo.StartTemp * lineInfo.Slope + lineInfo.Offset),
				new DataPoint(lineInfo.EndTemp, lineInfo.EndTemp * lineInfo.Slope + lineInfo.Offset)
			};
			loadDatas.Add(line5);
		}
		static (double, double) GetSlopeOffset(List<DataPoint> ps)
		{
			return Fit.Line(ps.Select((DataPoint c) => c.X).ToArray(), ps.Select((DataPoint c) => c.Y).ToArray());
		}
	}

	private void RefreshClick(object sender, RoutedEventArgs e)
	{
		try
		{
			List<RegressionForTempConfig> config = GetAndRefreshTempCorretionConfigs();
			config = config.OrderBy((RegressionForTempConfig c) => c.StartTemp).ToList();
			if (!ViewModel.UseStatic)
			{
				ViewModel.RegressionConfig.Clear();
				int index = 0;
				config.ForEach(delegate(RegressionForTempConfig c)
				{
					c.Order = index++;
					ViewModel.RegressionConfig.Add(c);
				});
			}
		}
		catch (Exception)
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.CTW_Msg3, GCProject.Main.LangResource.Lang.Error);
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}
}
