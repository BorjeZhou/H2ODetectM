using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using GCProject.Main.Converters;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;

namespace GCProject.Main.Windows;

public partial class SamplingWindow : Window, IWindow, IClose, INotifyPropertyChanged, IComponentConnector
{
	private GCContext db = new GCContext();

	private int _sampleNo;

	private bool canMasetemp;

	public int SampleNo
	{
		get
		{
			return _sampleNo;
		}
		set
		{
			_sampleNo = value;
			OnPropertyChanged("SampleNo");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public SamplingWindow()
	{
		InitializeComponent();
		Init(null);
	}

	public void Init(object args)
	{
		GCRecipe SelectedRecipe = MainWindow.BusDataProxy.SelectedRecipe;
		if (SelectedRecipe == null)
		{
			return;
		}
		tbx_RecipeName.Text = SelectedRecipe.DisplayName;
		using (GCContext db = new GCContext())
		{
			SampleNo = db.GCSummaryEntities.Count((GCSummaryEntity g) => g.GCRecipeID == SelectedRecipe.GCRecipeID && g.IsSampleData);
		}
		tb_sampleNo.DataContext = this;
		canMasetemp = MainWindow.BusDataProxy.CanMeas;
		MainWindow.BusDataProxy.CanMeas = true;
	}

	private void btn_startClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		Start();
	}

	private void btn_saveAndStopClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		Stop();
		ThreadPool.QueueUserWorkItem(delegate
		{
			MainWindow.BusDataProxy.PerformSaveData();
		}, null);
	}

	private void SetSampleNo()
	{
		List<int> sampleList = (from g in db.GCSummaryEntities
			where g.GCRecipeID == MainWindow.BusDataProxy.SelectedRecipe.GCRecipeID && g.IsSampleData
			select g into s
			select s.SampleNo).ToList();
		if (sampleList.Count > 0)
		{
			SampleNo = sampleList.Max() + 1;
		}
		else
		{
			SampleNo = 0;
		}
		tb_sampleNo.Text = SampleNo.ToString();
	}

	private void Start()
	{
		MainWindow.BusDataProxy.SaveDataFinished += BusDataProxy_SaveDataFinished;
		SetSampleNo();
		MainWindow.BusDataProxy.SampleNo = SampleNo;
		MainWindow.BusDataProxy.DropDataRequest = true;
		SetBindingManual();
		MainWindow.BusDataProxy.InSample = true;
		btn_start.IsEnabled = false;
		btn_drop.IsEnabled = true;
		btn_Stop.IsEnabled = true;
	}

	private void SetBindingManual()
	{
		tb_duration.SetBinding(TextBox.TextProperty, new Binding
		{
			Source = MainWindow.BusDataProxy,
			Path = new PropertyPath("DurationTime"),
			Mode = BindingMode.OneWay
		});
		tb_startTime.SetBinding(TextBox.TextProperty, new Binding
		{
			Source = MainWindow.BusDataProxy,
			Path = new PropertyPath("StartTime"),
			Mode = BindingMode.OneWay,
			Converter = new DateTime2StringConverter(),
			ConverterParameter = "yyyy-MM-dd HH:mm:ss"
		});
		tb_cps.SetBinding(TextBox.TextProperty, new Binding
		{
			Source = MainWindow.BusDataProxy,
			Path = new PropertyPath("CPS_Total"),
			Mode = BindingMode.OneWay
		});
		tb_h20.SetBinding(TextBox.TextProperty, new Binding
		{
			Source = MainWindow.BusDataProxy,
			Path = new PropertyPath("H20_Total"),
			Mode = BindingMode.OneWay
		});
		btn_run.SetBinding(Control.BackgroundProperty, new Binding
		{
			Source = MainWindow.BusDataProxy,
			Path = new PropertyPath("InRun"),
			Mode = BindingMode.OneWay,
			Converter = new Bool2BgColorConverter()
		});
	}

	private void ClearBindingManual()
	{
		BindingOperations.ClearBinding(tb_duration, TextBox.TextProperty);
		BindingOperations.ClearBinding(tb_startTime, TextBox.TextProperty);
		BindingOperations.ClearBinding(tb_cps, TextBox.TextProperty);
		BindingOperations.ClearBinding(tb_h20, TextBox.TextProperty);
		BindingOperations.ClearBinding(btn_run, Control.BackgroundProperty);
	}

	private void BusDataProxy_SaveDataFinished()
	{
		MainWindow.BusDataProxy.SaveDataFinished -= BusDataProxy_SaveDataFinished;
		base.Dispatcher.Invoke(delegate
		{
			Stop();
			btn_start.IsEnabled = true;
		});
	}

	private void Stop()
	{
		if (btn_drop.IsEnabled)
		{
			tb_cps.Text = MainWindow.BusDataProxy.CPS_Total.ToString();
			tb_duration.Text = MainWindow.BusDataProxy.DurationTime.ToString();
			tb_startTime.Text = MainWindow.BusDataProxy.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
			btn_run.Background = Brushes.Transparent;
			MainWindow.BusDataProxy.InMeastimeout = true;
			btn_drop.IsEnabled = false;
			btn_Stop.IsEnabled = false;
		}
	}

	private void btn_cancelClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		Stop();
		btn_start.IsEnabled = true;
	}

	public void SelfClose()
	{
		if (!btn_start.IsEnabled)
		{
			MessageBox.Show(Lang.SamplingWindow_InvalidClose, Lang.Information, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			return;
		}
		ClearBindingManual();
		MainWindow.BusDataProxy.CanMeas = canMasetemp;
		MainWindow.BusDataProxy.InSample = false;
		Hide();
	}
}
