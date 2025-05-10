using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class RegressionWindow : Window, IWindow, IClose, INotifyPropertyChanged, IComponentConnector
{
	private GCContext db = new GCContext();

	private List<GCSummaryEntity> SelectedList = new List<GCSummaryEntity>();

	private bool _editMode;

	public List<GCRecipe> Recipes { get; set; }

	public bool IsMultiSelectMode { get; set; } = true;


	public bool EditMode
	{
		get
		{
			return _editMode;
		}
		set
		{
			if (_editMode != value)
			{
				_editMode = value;
				OnPropertyChanged("EditMode");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public RegressionWindow()
	{
		InitializeComponent();
		Init(null);
		base.DataContext = this;
		base.StateChanged += RegressionWindow_StateChanged;
	}

	private void RegressionWindow_StateChanged(object sender, System.EventArgs e)
	{
		if (base.WindowState == WindowState.Minimized)
		{
			base.WindowState = WindowState.Normal;
		}
	}

	public void Init(object args)
	{
		Recipes = ConfigModel.GCRecipes;
		cmbx_recipe.ItemsSource = null;
		cmbx_recipe.ItemsSource = Recipes;
		GCRecipe find = Recipes.FirstOrDefault((GCRecipe g) => g.GCRecipeID == MainWindow.BusDataProxy.SelectedRecipe.GCRecipeID);
		cmbx_recipe.SelectedItem = find;
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void ExitButtonClick(object sender, RoutedEventArgs e)
	{
		Hide();
		e.Handled = true;
	}

	private void cmbx_recipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		object selectedItem = cmbx_recipe.SelectedItem;
		GCRecipe re = selectedItem as GCRecipe;
		if (re == null)
		{
			return;
		}
		try
		{
			List<GCSummaryEntity> source = db.GCSummaryEntities.Where((GCSummaryEntity s) => s.GCRecipeID == re.GCRecipeID && s.IsSampleData).ToList();
			source.ForEach(delegate(GCSummaryEntity g)
			{
				g.IsMultiSelected = false;
				g.GetAmendCPS(re);
			});
			MainGrid.ItemsSource = source;
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void MainGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
	}

	private void RegressionWindowClick(object sender, RoutedEventArgs e)
	{
		if (sender is Button btn)
		{
			WindowsFactory.GetWindow(btn.Tag as Type, this).ShowDialog();
			db = new GCContext();
		}
	}

	private void ShowCalculateWindowClick(object sender, RoutedEventArgs e)
	{
		if (MainGrid.ItemsSource is List<GCSummaryEntity> list && list.Count((GCSummaryEntity g) => g.IsSelected) > 1)
		{
			RegressionCalWindow window = WindowsFactory.GetWindow<RegressionCalWindow>(this);
			window.Init(cmbx_recipe.SelectedItem);
			window.ShowDialog();
			ReloadData();
		}
		else
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.RegressionWindow_NoDataSelected, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	private void ReloadData()
	{
		db = new GCContext();
		long SelectedID = (cmbx_recipe.SelectedItem as GCRecipe).GCRecipeID;
		GCRecipe findRecipe = ConfigModel.Instance.ParamSettingConfig.Recipes.FirstOrDefault((GCRecipe r) => r.GCRecipeID == SelectedID);
		List<GCSummaryEntity> source = db.GCSummaryEntities.Where((GCSummaryEntity s) => s.GCRecipeID == SelectedID && s.IsSampleData).ToList();
		ParamSettingConfig set = ConfigModel.Instance.ParamSettingConfig;
		foreach (GCSummaryEntity item in source)
		{
			item.GetAmendCPS(findRecipe);
			item.CalculatorValueByRecipe(findRecipe, set, CalTempCorretcion: false);
		}
		MainGrid.ItemsSource = null;
		MainGrid.ItemsSource = source;
	}

	private void ShowTempCorrectionClick(object sender, RoutedEventArgs e)
	{
		CorrectionForTempWindow window = WindowsFactory.GetWindow<CorrectionForTempWindow>(this);
		window.Init(cmbx_recipe.SelectedItem);
		window.ShowDialog();
		ReloadData();
	}

	private void ShowSampleConpareWindowClick(object sender, RoutedEventArgs e)
	{
		RegressionSampleCompareWindow window = WindowsFactory.GetWindow<RegressionSampleCompareWindow>(this);
		window.Init(cmbx_recipe.SelectedItem);
		window.ShowDialog();
	}

	private void DeleteClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		if (MainGrid.SelectedItems.Count == 0 || !PwdGuard.PerformInputPwd(this) || MessageBox.Show(GCProject.Main.LangResource.Lang.RecordWindow_DeleteConfirm, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) != MessageBoxResult.OK)
		{
			return;
		}
		using (GCContext db = new GCContext())
		{
			foreach (GCSummaryEntity item in (MainGrid.ItemsSource as List<GCSummaryEntity>).Where((GCSummaryEntity g) => g.IsMultiSelected))
			{
				db.GCSummaryEntities.Remove(db.GCSummaryEntities.Find(item.GCSummaryEntityID));
			}
			db.SaveChanges();
			cmbx_recipe_SelectionChanged(null, null);
		}
		ApplyEditMode(mode: false);
	}

	private void ShowDetailWindowClick(object sender, RoutedEventArgs e)
	{
		if (MainGrid.SelectedItem != null)
		{
			RawDataChartWindow window = WindowsFactory.GetWindow<RawDataChartWindow>(this);
			window.Init(MainGrid.SelectedItem);
			window.ShowDialog();
		}
	}

	public void SelfClose()
	{
		ApplyEditMode(mode: false);
		Hide();
	}

	private void EdiClick(object sender, RoutedEventArgs e)
	{
		ApplyEditMode(mode: true);
	}

	private void ApplyEditMode(bool mode)
	{
		EditMode = mode;
		MainGrid.Columns[0].Visibility = ((!EditMode) ? Visibility.Hidden : Visibility.Visible);
	}

	private void SaveClick(object sender, RoutedEventArgs e)
	{
		List<GCSummaryEntity> obj = MainGrid.ItemsSource as List<GCSummaryEntity>;
		GCRecipe recipe = cmbx_recipe.SelectedItem as GCRecipe;
		ParamSettingConfig setting = ConfigModel.Instance.ParamSettingConfig;
		obj.ForEach(delegate(GCSummaryEntity s)
		{
			s.CalculatorValueByRecipe(recipe, setting);
		});
		db.SaveChanges();
		MainGrid.ItemsSource = db.GCSummaryEntities.Where((GCSummaryEntity s) => s.GCRecipeID == recipe.GCRecipeID && s.IsSampleData).ToList();
		ApplyEditMode(mode: false);
	}
}
