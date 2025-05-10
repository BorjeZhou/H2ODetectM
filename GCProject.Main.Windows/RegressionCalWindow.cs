using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Aggregate;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.EventArgs;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using Microsoft.EntityFrameworkCore;
using OxyPlot;

namespace GCProject.Main.Windows;

public partial class RegressionCalWindow : Window, IWindow, IComponentConnector
{
	private GCRecipe SelectedRecipe;

	private SplitCPSViewModel splitCPSModelView;

	public RegressionCalWindow()
	{
		InitializeComponent();
		Init(null);
	}

	public void Init(object args)
	{
		if (args is GCRecipe cRecipe)
		{
			MainChart.Clear();
			SelectedRecipe = Library.MapObject(cRecipe);
			base.DataContext = SelectedRecipe;
			splitCPSModelView = new SplitCPSViewModel
			{
				EnabledStage2 = SelectedRecipe.EnabledStage2,
				EnabledStage3 = SelectedRecipe.EnabledStage3,
				Split2_CPS = SelectedRecipe.Split2,
				Split3_CPS = SelectedRecipe.Split3,
				Split2_H20 = 20.0,
				Split3_H20 = 10.0,
				UscCPSSplitType = true
			};
			tb_newSlope.Text = string.Empty;
			tb_newOffset.Text = string.Empty;
			tb_newSlope2.Text = null;
			tb_newOffset2.Text = null;
			tb_newSplit2.Text = null;
			tb_newSlope3.Text = null;
			tb_newOffset3.Text = null;
			tb_newSplit3.Text = null;
			AddDrawPoint(null);
		}
	}

	private void ReadSplitCPSViewModelData()
	{
		splitCPSModelView.EnabledStage2 = SelectedRecipe.EnabledStage2;
		splitCPSModelView.EnabledStage3 = SelectedRecipe.EnabledStage3;
	}

	private void AddDrawPoint(List<GCSummaryEntity> data, bool IsOld = true)
	{
		List<GCSummaryEntity> datas = null;
		if (data == null)
		{
			long RecipeID = SelectedRecipe.GCRecipeID;
			using GCContext db = new GCContext();
			datas = (from g in db.GCSummaryEntities.AsNoTracking()
				where g.GCRecipeID == RecipeID && g.IsSelected
				select g).ToList();
		}
		else
		{
			datas = data;
		}
		datas.ForEach(delegate(GCSummaryEntity d)
		{
			if (d.H20_OnlyCPS < 0.0)
			{
				d.H20_OnlyCPS = 0.0;
			}
			else if (d.H20_OnlyCPS > 100.0)
			{
				d.H20_OnlyCPS = 100.0;
			}
		});
		List<DataPoint> dataPoints = datas.Select((GCSummaryEntity d) => new DataPoint(d.Labor, d.H20_OnlyCPS)).ToList();
		if (IsOld)
		{
			MainChart.LoadOldData(dataPoints);
		}
		else
		{
			MainChart.LoadNewData(dataPoints);
		}
	}

	private void CalRegression(double[] Xs, double[] Ys, out double Slope, out double Offset)
	{
		Slope = 0.0;
		Offset = 0.0;
		double len = Xs.Length;
		double x = 0.0;
		double y = 0.0;
		double xx = 0.0;
		double xy = 0.0;
		for (int i = 0; (double)i < len; i++)
		{
			x += Xs[i];
			y += Ys[i];
			xx += Xs[i] * Xs[i];
			xy += Xs[i] * Ys[i];
		}
		if (x * x != xx * len)
		{
			Slope = (y * x - xy * len) / (x * x - xx * len);
			Offset = (y * xx - xy * x) / (len * xx - x * x);
		}
		Slope = MathF.Round((float)Slope, 5);
		Offset = MathF.Round((float)Offset, 5);
	}

	private void CalRegressionClick(object sender, RoutedEventArgs e)
	{
		ReadSplitCPSViewModelData();
		if (splitCPSModelView.EnabledStage2)
		{
			SplitCPSWindow splitCPSWindow = new SplitCPSWindow(splitCPSModelView);
			splitCPSWindow.Owner = this;
			splitCPSWindow.ShowDialog();
			if (!splitCPSWindow.Result.HasValue)
			{
				return;
			}
		}
		long RecipeID = SelectedRecipe.GCRecipeID;
		GCContext db = new GCContext();
		List<GCSummaryEntity> datas = (from g in db.GCSummaryEntities.AsNoTracking()
			where g.GCRecipeID == RecipeID && g.IsSelected
			select g).ToList();
		datas.ForEach(delegate(GCSummaryEntity g)
		{
			g.GetAmendCPS(SelectedRecipe);
		});
		if (!CheckStageDataValid(out var error2, out var splitDatas2))
		{
			MessageBox.Show(error2, "注意", MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		double SensorWidth = ConfigModel.Instance.ParamSettingConfig.WidthSensorsDistance;
		bool WidthCorrectionEnabled = ConfigModel.Instance.ParamSettingConfig.WidthCorrectionEnabled;
		double SensorHeight = ConfigModel.Instance.ParamSettingConfig.HeightSensorsDistance;
		bool HeightCorrectionEnabled = ConfigModel.Instance.ParamSettingConfig.HeightCorrectionEnabled;
		GCRecipe tempNew = new GCRecipe
		{
			ZeroRate = SelectedRecipe.ZeroRate,
			EnableDynamicZeroRate = SelectedRecipe.EnableDynamicZeroRate,
			SplitCPSRange = SelectedRecipe.SplitCPSRange,
			UseCustomTempCorrection = SelectedRecipe.UseCustomTempCorrection,
			TempCorrectionSetting = SelectedRecipe.TempCorrectionSetting
		};
		if (splitDatas2.ContainsKey(1))
		{
			List<GCSummaryEntity> datas_1 = splitDatas2[1];
			CalRegression(datas_1.Select((GCSummaryEntity c) => GetX(c)).ToArray(), datas_1.Select((GCSummaryEntity c) => c.Labor).ToArray(), out var newSlope, out var newOffset);
			tb_newSlope.Text = newSlope.ToString();
			tb_newOffset.Text = newOffset.ToString();
			tempNew.Slope = newSlope;
			tempNew.Offset = newOffset;
		}
		if (splitDatas2.ContainsKey(2))
		{
			List<GCSummaryEntity> datas_3 = splitDatas2[2];
			CalRegression(datas_3.Select((GCSummaryEntity c) => GetX(c)).ToArray(), datas_3.Select((GCSummaryEntity c) => c.Labor).ToArray(), out var newSlope2, out var newOffset2);
			tb_newSlope2.Text = newSlope2.ToString();
			tb_newOffset2.Text = newOffset2.ToString();
			tempNew.Slope2 = newSlope2;
			tempNew.Offset2 = newOffset2;
			tempNew.EnabledStage2 = true;
			tempNew.Split2 = (int)splitCPSModelView.Split2_CPS;
			tb_newSplit2.Text = tempNew.Split2.ToString();
		}
		else
		{
			tb_newSlope2.Text = null;
			tb_newOffset2.Text = null;
			tb_newSplit2.Text = null;
			tempNew.EnabledStage2 = false;
		}
		if (splitDatas2.ContainsKey(3))
		{
			List<GCSummaryEntity> datas_5 = splitDatas2[3];
			CalRegression(datas_5.Select((GCSummaryEntity c) => GetX(c)).ToArray(), datas_5.Select((GCSummaryEntity c) => c.Labor).ToArray(), out var newSlope3, out var newOffset3);
			tb_newSlope3.Text = newSlope3.ToString();
			tb_newOffset3.Text = newOffset3.ToString();
			tempNew.Slope3 = newSlope3;
			tempNew.Offset3 = newOffset3;
			tempNew.EnabledStage3 = true;
			tempNew.Split3 = (int)splitCPSModelView.Split3_CPS;
			tb_newSplit3.Text = tempNew.Split3.ToString();
		}
		else
		{
			tb_newSlope3.Text = null;
			tb_newOffset3.Text = null;
			tb_newSplit3.Text = null;
			tempNew.EnabledStage3 = false;
		}
		if (splitCPSModelView.UscCPSSplitType)
		{
			tempNew.Split3 = (int)splitCPSModelView.Split3_CPS;
			tempNew.Split2 = (int)splitCPSModelView.Split2_CPS;
		}
		else
		{
			if (tempNew.EnabledStage3)
			{
				List<GCSummaryEntity> source = splitDatas2[2];
				List<GCSummaryEntity> datas_4 = splitDatas2[3];
				double max2 = source.Max((GCSummaryEntity r) => r.CPS_ZeroRate);
				double min3 = datas_4.Min((GCSummaryEntity r) => r.CPS_ZeroRate);
				tempNew.Split3 = (int)(min3 + max2) / 2;
				if (tempNew.Split3 > 0 && tempNew.Split3 <= 65535)
				{
					tb_newSplit3.Text = tempNew.Split3.ToString();
				}
			}
			if (tempNew.EnabledStage2)
			{
				List<GCSummaryEntity> source2 = splitDatas2[1];
				List<GCSummaryEntity> datas_2 = splitDatas2[2];
				double max1 = source2.Max((GCSummaryEntity r) => r.CPS_ZeroRate);
				double min2 = datas_2.Min((GCSummaryEntity r) => r.CPS_ZeroRate);
				tempNew.Split2 = (int)(min2 + max1) / 2;
				if (tempNew.Split2 > 0 && tempNew.Split2 <= 65535)
				{
					tb_newSplit2.Text = tempNew.Split2.ToString();
				}
			}
		}
		tempNew.InitKBStageInfo();
		List<GCSummaryEntity> list = (from g in db.GCSummaryEntities.AsNoTracking()
			where g.GCRecipeID == SelectedRecipe.GCRecipeID && g.IsSelected
			select g).ToList();
		ParamSettingConfig set = ConfigModel.Instance.ParamSettingConfig;
		Dictionary<int, List<DataPoint>> addPoints = new Dictionary<int, List<DataPoint>>();
		foreach (GCSummaryEntity entity2 in list)
		{
			entity2.CalculatorValueByRecipe(tempNew, set, CalTempCorretcion: false);
			int stage = tempNew.GetRecordStageNum(entity2);
			if (!addPoints.ContainsKey(stage))
			{
				addPoints[stage] = new List<DataPoint>();
			}
			addPoints[stage].Add(new DataPoint(entity2.Labor, entity2.H20_OnlyCPS));
		}
		MainChart.LoadNewDataEx(addPoints);
		bool CheckStageDataValid(out string error, out Dictionary<int, List<GCSummaryEntity>> splitDatas)
		{
			error = null;
			splitDatas = new Dictionary<int, List<GCSummaryEntity>>();
			if (splitCPSModelView.EnabledStage3)
			{
				List<GCSummaryEntity> list6 = null;
				if (splitCPSModelView.UscCPSSplitType)
				{
					list6 = datas.Where((GCSummaryEntity p) => SelectedRecipe.GetAmendCPS(p.CPS, p.RealZeroRate) > splitCPSModelView.Split3_CPS).ToList();
					if (list6.Count <= 1)
					{
						error = "满足 Stage3 CPS的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				else
				{
					list6 = datas.Where((GCSummaryEntity p) => p.Labor < splitCPSModelView.Split3_H20).ToList();
					if (list6.Count <= 1)
					{
						error = "满足 Stage3 H2O的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				splitDatas[3] = list6;
				List<GCSummaryEntity> list5 = null;
				if (splitCPSModelView.UscCPSSplitType)
				{
					list5 = datas.Where(delegate(GCSummaryEntity p)
					{
						double amendCPS = SelectedRecipe.GetAmendCPS(p.CPS, p.RealZeroRate);
						return amendCPS > splitCPSModelView.Split2_CPS && amendCPS <= splitCPSModelView.Split3_CPS;
					}).ToList();
					if (list5.Count <= 1)
					{
						error = "满足 Stage2 CPS的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				else
				{
					list5 = datas.Where((GCSummaryEntity p) => p.Labor >= splitCPSModelView.Split3_H20 && p.Labor < splitCPSModelView.Split2_H20).ToList();
					if (list5.Count <= 1)
					{
						error = "满足 Stage2 H2O的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				splitDatas[2] = list5;
				List<GCSummaryEntity> list2 = null;
				if (splitCPSModelView.UscCPSSplitType)
				{
					list2 = datas.Where((GCSummaryEntity p) => SelectedRecipe.GetAmendCPS(p.CPS, p.RealZeroRate) < splitCPSModelView.Split2_CPS).ToList();
					if (list2.Count <= 1)
					{
						error = "满足 Stage1 CPS的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				else
				{
					list2 = datas.Where((GCSummaryEntity p) => p.Labor >= splitCPSModelView.Split2_H20).ToList();
					if (list2.Count <= 1)
					{
						error = "满足 Stage2 H2O的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				splitDatas[1] = list2;
			}
			else
			{
				if (!splitCPSModelView.EnabledStage2)
				{
					if (datas.Count <= 1)
					{
						error = "有效点的个数小于2个 无法计算";
						return false;
					}
					splitDatas[1] = datas;
					return true;
				}
				List<GCSummaryEntity> list4 = null;
				if (splitCPSModelView.UscCPSSplitType)
				{
					list4 = datas.Where((GCSummaryEntity p) => SelectedRecipe.GetAmendCPS(p.CPS, p.RealZeroRate) > splitCPSModelView.Split2_CPS).ToList();
					if (list4.Count <= 1)
					{
						error = "满足 Stage2 CPS的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				else
				{
					list4 = datas.Where((GCSummaryEntity p) => p.Labor < splitCPSModelView.Split2_H20).ToList();
					if (list4.Count <= 1)
					{
						error = "满足 Stage2 H2O的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				splitDatas[2] = list4;
				List<GCSummaryEntity> list3 = null;
				if (splitCPSModelView.UscCPSSplitType)
				{
					list3 = datas.Where((GCSummaryEntity p) => SelectedRecipe.GetAmendCPS(p.CPS, p.RealZeroRate) < splitCPSModelView.Split2_CPS).ToList();
					if (list3.Count <= 1)
					{
						error = "满足 Stage1 CPS的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				else
				{
					list3 = datas.Where((GCSummaryEntity p) => p.Labor >= splitCPSModelView.Split2_H20).ToList();
					if (list3.Count <= 1)
					{
						error = "满足 Stage2 H2O的点的个数小于2个 无法计算 或者取消该分段";
						return false;
					}
				}
				splitDatas[1] = list3;
			}
			return true;
		}
		double GetX(GCSummaryEntity entity)
		{
			if (!WidthCorrectionEnabled)
			{
				entity.Width = SensorWidth;
			}
			if (!HeightCorrectionEnabled)
			{
				entity.Height = SensorHeight;
			}
			return (SelectedRecipe.ZeroRate - SelectedRecipe.GetAmendCPS(entity.CPS, entity.RealZeroRate)) / (entity.Width / SensorWidth) / (entity.Height.Value / SensorHeight);
		}
	}

	public void btn_saveClick(object sender, RoutedEventArgs e)
	{
		try
		{
			using (GCContext db = new GCContext())
			{
				GCRecipe find = ConfigModel.GCRecipes.FirstOrDefault((GCRecipe g) => g.GCRecipeID == SelectedRecipe.GCRecipeID);
				double.TryParse(tb_newSlope.Text, out var newSlope);
				double.TryParse(tb_newOffset.Text, out var newOffset);
				find.Slope = newSlope;
				find.Offset = newOffset;
				SelectedRecipe.Slope = newSlope;
				SelectedRecipe.Offset = newOffset;
				if (SelectedRecipe.EnabledStage2)
				{
					double.TryParse(tb_newSlope2.Text, out var newSlope2);
					double.TryParse(tb_newOffset2.Text, out var newOffset2);
					double.TryParse(tb_newSplit2.Text, out var newSlip2);
					find.Slope2 = newSlope2;
					find.Offset2 = newOffset2;
					find.Split2 = (int)newSlip2;
					find.EnabledStage2 = true;
					SelectedRecipe.Slope2 = newSlope2;
					SelectedRecipe.Offset2 = newOffset2;
					SelectedRecipe.Split2 = find.Split2;
				}
				if (SelectedRecipe.EnabledStage3)
				{
					double.TryParse(tb_newSlope3.Text, out var newSlope3);
					double.TryParse(tb_newOffset3.Text, out var newOffset3);
					double.TryParse(tb_newSplit3.Text, out var newSlip3);
					find.Slope3 = newSlope3;
					find.Offset3 = newOffset3;
					find.Split3 = (int)newSlip3;
					find.EnabledStage3 = true;
					SelectedRecipe.Slope3 = newSlope3;
					SelectedRecipe.Offset3 = newOffset3;
					SelectedRecipe.Split3 = find.Split3;
				}
				base.DataContext = null;
				base.DataContext = SelectedRecipe;
				List<GCSummaryEntity> list = db.GCSummaryEntities.Where((GCSummaryEntity g) => g.GCRecipeID == SelectedRecipe.GCRecipeID && g.IsSampleData).ToList();
				ParamSettingConfig set = ConfigModel.Instance.ParamSettingConfig;
				foreach (GCSummaryEntity item in list)
				{
					item.CalculatorValueByRecipe(find, set, CalTempCorretcion: false);
				}
				db.SaveChanges();
			}
			ConfigModel.Instance.CheckRecipeIfUpdate();
			ConfigModel.Instance.Save();
			this.PublishEvent(new ReloadRecipeEvent());
			MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SaveSuccess, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception)
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.RegressionCalWindow_SaveError, GCProject.Main.LangResource.Lang.Error, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void btn_cancelclick(object sender, RoutedEventArgs e)
	{
		Hide();
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "8.0.8.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
