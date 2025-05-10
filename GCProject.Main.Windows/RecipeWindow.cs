using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Aggregate;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.EventArgs;
using GCProject.Main.Interface;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public partial class RecipeWindow : Window, IWindow, IClose, IComponentConnector
{
	private List<GCRecipe> Recipes = new List<GCRecipe>();

	public RecipeWindow()
	{
		InitializeComponent();
	}

	private void LoadData()
	{
		Recipes = ConfigModel.GCRecipes;
		MainGrid.ItemsSource = Recipes;
	}

	public void Init(object args)
	{
		LoadData();
	}

	private void btn_Edit_Click(object sender, RoutedEventArgs e)
	{
		if (btn_Edit.IsChecked.Value)
		{
			bool success = true;
			string error = null;
			if (Recipes.Count((GCRecipe r) => r.Display) == 0)
			{
				success = false;
				error = "At least one recipe must be selected for display";
			}
			if ((from r in Recipes
				group r by r.RecipeNum).FirstOrDefault((IGrouping<int, GCRecipe> g) => g.Count() > 1) != null)
			{
				success = false;
				error = "The SortNum must be unique";
			}
			if (!success)
			{
				MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
				ConfigModel.InitConfigs();
				LoadData();
			}
			else
			{
				ConfigModel.Instance.CheckRecipeIfUpdate();
				ConfigModel.Instance.Save();
				this.PublishEvent(new ReloadRecipeEvent());
			}
		}
	}

	private void RequirKeyboardClick(object sender, RoutedEventArgs e)
	{
		KeyboardHelper.ShowKeyboard();
	}

	private void AddNewRecipeClick(object sender, RoutedEventArgs e)
	{
		Recipes.Add(new GCRecipe
		{
			DisplayName = $"Recipe{Recipes.Count + 1}",
			LastModifyDate = DateTime.Now,
			GCRecipeID = Recipes.Max((GCRecipe r) => r.GCRecipeID) + 1,
			RecipeNum = Recipes.Max((GCRecipe r) => r.RecipeNum) + 1
		});
		MainGrid.ItemsSource = null;
		MainGrid.ItemsSource = Recipes;
	}

	public void SelfClose()
	{
		btn_Edit.IsChecked = true;
		ConfigModel.Instance.Refresh();
		Hide();
	}
}
