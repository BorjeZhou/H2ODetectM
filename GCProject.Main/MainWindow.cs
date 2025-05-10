using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using GCProject.Main.Aggregate;
using GCProject.Main.Chart;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.EventArgs;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using GCProject.Main.Log;
using GCProject.Main.Protocol;
using GCProject.Main.SerialPorts;
using GCProject.Main.Windows;

namespace GCProject.Main;

public partial class MainWindow : Window, IComponentConnector
{
	private bool ComfirmClose;

	private GCMainChart MainChart;

	public GCRecipe SelectedRecipe;

	private Timer ResizeTimer;

	private const int defaultfontSize = 15;

	private Stopwatch sw = new Stopwatch();

	private int lastRequestSortNum;

	private bool init;

	public static BusDataProxy BusDataProxy { get; set; }

	public DrawChartDriver Drawer { get; set; }

	public MainWindow()
	{
		InitializeComponent();
		Library.HoldSystem();
		ThreadPool.SetMinThreads(15, 15);
		MainInit();
	}

	private void MainInit()
	{
		UpdateHelper.CheckConfigModelUpdate();
		SetMRDisplay();
		GCContext.Init(ConfigModel.Instance.DataBaseFile);
		tb_DataSource.Text = ((ConfigModel.Instance.ParamSettingConfig.RunMode == RunModeEnum.Mode_868M) ? "Micro wave 868MHz" : "2.4GHz");
		BusDataProxy = new BusDataProxy();
		base.DataContext = BusDataProxy;
		List<GCProject.Main.Configs.Lang> langs = GCProject.Main.Configs.Lang.SupportLangs;
		cmbx_lang.ItemsSource = langs;
		cmbx_lang.SelectedItem = langs.FirstOrDefault((GCProject.Main.Configs.Lang l) => l.Code == GCProject.Main.Configs.Lang.GetCurrentLangCode());
		InitChart();
		LoadRecipe();
		GCSerialPortManager.Init();
		GCSerialPortManager.DataRecved += GCSerialPortManager_DataRecved;
		this.RegisterHandler<ReloadRecipeEvent>(delegate
		{
			LoadRecipe();
		});
		this.RegisterHandler<ReloadChartRangeEvent>(delegate
		{
			loadChartRange();
		});
		this.RegisterRequest<RequestChangeRecipeEvent>(RequestChangeRecipe);
		WindowsFactory.PreBuildWindows();
		base.Closing += MainWindow_Closing;
		base.Loaded += MainWindow_Loaded;
		base.SizeChanged += MainWindow_SizeChanged;
		ResizeTimer = new Timer(ResizeMainChart, null, -1, -1);
		base.WindowState = WindowState.Normal;
		base.WindowStyle = WindowStyle.ThreeDBorderWindow;
	}

	private void InitChart()
	{
		Size ControlSize = new Size(100.0, 100.0);
		MainChartHost.Child = null;
		MainChartHost.Child = WorkThreadUIHelper.StartWorkThreadAndCreateElement(typeof(GCMainChart), ControlSize);
		while (MainChartHost.Child == null)
		{
			Thread.Sleep(50);
		}
		MainChart = (GCMainChart)((HostVisualEx)MainChartHost.Child).Context;
		loadChartRange();
		MainChart?.SetTitle((lbx_Recipe.SelectedItem as GCRecipe)?.DisplayName);
		BusDataProxy.MainChart = MainChart;
		Drawer = new DrawChartDriver(BusDataProxy);
	}

	private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (MainChart != null)
		{
			ResizeTimer.Change(200, -1);
		}
	}

	private void ResizeMainChart(object _)
	{
		if (MainChart != null)
		{
			MainChart.Dispatcher.Invoke(delegate
			{
				MainChart.Height = MainChartHost.ActualHeight;
				MainChart.Width = MainChartHost.ActualWidth;
			});
		}
		base.Dispatcher.Invoke(delegate
		{
			double num = 0.9;
			setTextBoxFontSize(tb1, num);
			setTextBoxFontSize(tb2, num);
			setTextBoxFontSize(tb3, num);
			setTextBoxFontSize(tb4, num);
			setTextBoxFontSize(tb5, num);
			setTextBoxFontSize(tb6, num);
			setTextBoxFontSize(tb7, num);
			setTextBoxFontSize(tb77, num);
			setTextBoxFontSize(tb8, num);
			setTextBoxFontSize(tb9, num);
			setTextBoxFontSize(tb10, num);
			setTextBoxFontSize(tb101, num);
			setTextBoxFontSize(tb11, num);
			setTextBoxFontSize(tb12, num);
			setTextBoxFontSize(tb13, num);
			setTextBoxFontSize(tb14, num);
			setTextBoxFontSize(tb15, num);
			setTextBoxFontSize(tb16, num);
			setTextBoxFontSize(tb17, num);
			try
			{
				lb0.FontSize = tb10.ActualHeight * num * 0.9 / 2.0;
				lb1.FontSize = lb0.FontSize;
				lb2.FontSize = lb0.FontSize;
				lb3.FontSize = lb0.FontSize;
			}
			catch (Exception)
			{
				lb0.FontSize = 15.0;
				lb1.FontSize = 15.0;
				lb2.FontSize = 15.0;
				lb3.FontSize = 15.0;
			}
		});
	}

	private void setTextBoxFontSize(TextBox tb, double param)
	{
		try
		{
			tb.FontSize = tb.ActualHeight * param / 2.0;
		}
		catch (Exception)
		{
			tb.FontSize = 15.0;
		}
	}

	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		ResizeMainChart(null);
	}

	private void LoadRecipe(string selected = null)
	{
		lbx_Recipe.ItemsSource = null;
		List<GCRecipe> source = (from g in ConfigModel.GCRecipes
			where g.Display
			select g into r
			orderby r.RecipeNum
			select r).ToList();
		GCLogger.log.Info($"Load {source.Count} Recipe");
		if (selected == null)
		{
			selected = ConfigModel.Instance.SelectedRecipe;
		}
		SelectedRecipe = source.FirstOrDefault((GCRecipe r) => r.DisplayName == selected);
		if (SelectedRecipe == null)
		{
			SelectedRecipe = source.FirstOrDefault();
		}
		lbx_Recipe.ItemsSource = source;
		lbx_Recipe.SelectedItem = SelectedRecipe;
	}

	private void loadChartRange()
	{
		double max = ConfigModel.Instance.ParamSettingConfig.MaxChargeTime_s;
		MainChart.SetAxisType((ConfigModel.Instance.ParamSettingConfig.XScaleDispaly == XScaleDispalyEnum.Time) ? "time" : "dis");
		MainChart.UpdateRange(max);
		MainChart.UpdateYAxisRange(ConfigModel.Instance.ParamSettingConfig);
	}

	private void GCSerialPortManager_DataRecved(IProtocol obj)
	{
		BusDataProxy?.ReadData(obj);
	}

	private void MainWindow_Closing(object sender, CancelEventArgs e)
	{
		ConfigModel.Instance.Save();
		if (!ComfirmClose)
		{
			e.Cancel = !PwdGuard.PerformInputPwd(this);
			if (!e.Cancel)
			{
				DisposeAll();
			}
		}
	}

	private void ExitClick(object sender, RoutedEventArgs e)
	{
		if (PwdGuard.PerformInputPwd(this))
		{
			switch (MessageBox.Show(GCProject.Main.LangResource.Lang.MainWindow_ExitInfo, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk))
			{
			case MessageBoxResult.Yes:
				ComfirmClose = true;
				DisposeAll();
				break;
			case MessageBoxResult.No:
				base.WindowState = WindowState.Minimized;
				break;
			}
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void PwdClick(object sender, RoutedEventArgs e)
	{
		PwdGuard.PerformInputPwd(this);
	}

	private void Showwindow(object sender, RoutedEventArgs e)
	{
		if (sender is Button btn)
		{
			Window window = WindowsFactory.GetWindow(btn.Tag as Type, this);
			window.ShowDialog();
			if (window is RegressionWindow)
			{
				LoadRecipe((lbx_Recipe.SelectedItem as GCRecipe)?.DisplayName);
			}
			if (window is ParamSettingWindow)
			{
				SetMRDisplay();
			}
		}
	}

	private void SetMRDisplay()
	{
		if (ConfigModel.Instance.ParamSettingConfig.DisplayMR)
		{
			mrPanel.Visibility = Visibility.Visible;
			h2oPanel.Visibility = Visibility.Hidden;
		}
		else
		{
			h2oPanel.Visibility = Visibility.Visible;
			mrPanel.Visibility = Visibility.Hidden;
		}
	}

	private void CanMeasClick(object sender, RoutedEventArgs e)
	{
		GCLogger.log.Info($"Switch AutoSave to {btn_canMeas.IsChecked}");
	}

	private void lbx_Recipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e != null)
		{
			e.Handled = true;
		}
		if (lbx_Recipe.SelectedItem == null)
		{
			LoadRecipe();
			return;
		}
		GCRecipe recipe = lbx_Recipe.SelectedItem as GCRecipe;
		ConfigModel.Instance.SelectedRecipe = recipe.DisplayName;
		BusDataProxy.SetRecipe(recipe);
		MainChart?.SetTitle(recipe.DisplayName);
		GCLogger.log.Info("Switch Recipe to " + recipe.DisplayNameForMainWindow);
		recipe.InitKBStageInfo();
	}

	private object RequestChangeRecipe(object Request)
	{
		RequestChangeRecipeEvent req = Request as RequestChangeRecipeEvent;
		if (req == null)
		{
			return null;
		}
		return base.Dispatcher.Invoke(delegate
		{
			GCRecipe gCRecipe = (lbx_Recipe.ItemsSource as List<GCRecipe>).FirstOrDefault((GCRecipe g) => g.RecipeNum == req.SortNum);
			if (gCRecipe == null)
			{
				if (lastRequestSortNum != req.SortNum)
				{
					GCLogger.log.Info($"Request Change Recipe[{req.SortNum}],Fail,Not Exsit Or Not Display On List");
				}
				lastRequestSortNum = req.SortNum;
				return (object)null;
			}
			if (lastRequestSortNum != req.SortNum)
			{
				GCLogger.log.Info($"Request Change Recipe[{req.SortNum}],Success");
			}
			lastRequestSortNum = req.SortNum;
			lbx_Recipe.SelectedItem = gCRecipe;
			req.Success = true;
			return req;
		});
	}

	private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		AboutWindow aboutWindow = new AboutWindow();
		aboutWindow.Owner = this;
		aboutWindow.ShowDialog();
	}

	private void DisposeAll()
	{
		GCSerialPortManager.CloseAll();
		Library.ReleaseSystem();
		Application.Current.Shutdown();
	}

	private void langSelectedChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!init || ConfigModel.Instance.Language == cmbx_lang.SelectedValue.ToString())
		{
			init = true;
		}
		else if (MessageBox.Show(GCProject.Main.LangResource.Lang.MainWindow_ChangeLanguageNotify, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.Cancel)
		{
			cmbx_lang.SelectedValue = ConfigModel.Instance.Language;
			e.Handled = true;
		}
		else
		{
			ConfigModel.Instance.Language = cmbx_lang.SelectedValue.ToString();
			ConfigModel.Instance.Save();
			DisposeAll();
		}
	}

	private void lbx_Recipe_MouseEnter(object sender, MouseEventArgs e)
	{
		lbx_Recipe.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
	}

	private void lbx_Recipe_MouseLeave(object sender, MouseEventArgs e)
	{
		lbx_Recipe.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);
	}

	private void DetelePackageNumClick(object sender, RoutedEventArgs e)
	{
		BusDataProxy.PackageNumber = string.Empty;
	}

	private void DetelePackageWeightClick(object sender, RoutedEventArgs e)
	{
		BusDataProxy.PackageWeight = 0.0;
	}
}
