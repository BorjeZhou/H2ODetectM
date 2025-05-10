using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Aggregate;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.EventArgs;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class ParamSettingWindow : Window, IWindow, IClose, IComponentConnector
{
	private bool canMasetemp;

	private ParamSettingWindow_ViewModel ViewModel { get; set; } = new ParamSettingWindow_ViewModel();


	public ParamSettingWindow()
	{
		InitializeComponent();
		Init(new object());
	}

	public void Init(object args)
	{
		if (args == null)
		{
			ReloadRecipe();
		}
		ViewModel.ParamSetting = ConfigModel.Instance.ParamSettingConfig;
		base.DataContext = null;
		base.DataContext = ViewModel;
		if (args == null)
		{
			canMasetemp = MainWindow.BusDataProxy.CanMeas;
			MainWindow.BusDataProxy.CanMeas = false;
		}
	}

	private void ReloadRecipe()
	{
		ViewModel.RecipeList = ConfigModel.GCRecipes.Where((GCRecipe g) => g.Display).ToList();
		ViewModel.SelectedRecipe = ViewModel.RecipeList.FirstOrDefault((GCRecipe r) => r.GCRecipeID == BusDataProxy.Instance.SelectedRecipe.GCRecipeID);
		if (ViewModel.SelectedRecipe == null)
		{
			ViewModel.SelectedRecipe = ViewModel.RecipeList.FirstOrDefault();
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void CancelClick(object sender, RoutedEventArgs e)
	{
		SelfClose();
		e.Handled = true;
	}

	public void SelfClose()
	{
		Hide();
		ConfigModel.Instance.Refresh();
		MainWindow.BusDataProxy.CanMeas = canMasetemp;
	}

	private void btnSaveClick(object sender, RoutedEventArgs e)
	{
		if (!ConfigModel.Instance.ParamSettingConfig.IsValid() || !CheckError())
		{
			MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SomeFieldError, GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			Init(new object());
		}
		else if (!IsRecipeParamValid())
		{
			Init(new object());
		}
		else if (PwdGuard.PerformInputPwd(this))
		{
			ConfigModel.Instance.CheckRecipeIfUpdate();
			ConfigModel.Instance.Save();
			this.PublishEvent(new ReloadRecipeEvent());
			this.PublishEvent(new ReloadChartRangeEvent());
			MessageBox.Show(GCProject.Main.LangResource.Lang.PSW_SaveSuccess, GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			Init(new object());
		}
	}

	private bool IsRecipeParamValid()
	{
		foreach (GCRecipe recipe in ViewModel.RecipeList)
		{
			if (!recipe.IsParamValid())
			{
				MessageBox.Show(string.Format(GCProject.Main.LangResource.Lang.PSW_RecipeParanWarning, recipe.DisplayName), GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return false;
			}
			if (recipe.DynamicZeroRateCutSecond_Begin > ConfigModel.Instance.ParamSettingConfig.ZeroRateRecursiveDuration || recipe.DynamicZeroRateCutSecond_End > ConfigModel.Instance.ParamSettingConfig.ZeroRateRecursiveDuration)
			{
				MessageBox.Show(string.Format(GCProject.Main.LangResource.Lang.PSW_RecipeParanWarning, recipe.DisplayName), GCProject.Main.LangResource.Lang.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return false;
			}
		}
		return true;
	}

	public bool CheckError()
	{
		try
		{
			ParamSettingConfig GS = ConfigModel.Instance.ParamSettingConfig;
			if (GS.HighH20AlarmDigital == GS.LowH20AlarmDigital || GS.HighH20AlarmDigital == GS.MeasFinishedDigital || GS.LowH20AlarmDigital == GS.MeasFinishedDigital)
			{
				return false;
			}
			WindowsHelper.GetTextBoxsError(MainGrid);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private void ShowTempCorrectionWindowClick(object sender, RoutedEventArgs e)
	{
	}
}
