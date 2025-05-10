using System.Collections.Generic;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public class ParamSettingWindow_ViewModel : NotifyBase
{
	private List<GCRecipe> _recipeList;

	private GCRecipe _selectedRecipe;

	private ParamSettingConfig _paramSetting;

	public List<GCRecipe> RecipeList
	{
		get
		{
			return _recipeList;
		}
		set
		{
			if (_recipeList != value)
			{
				_recipeList = value;
				OnPropertyChanged("RecipeList");
			}
		}
	}

	public GCRecipe SelectedRecipe
	{
		get
		{
			return _selectedRecipe;
		}
		set
		{
			if (_selectedRecipe != value)
			{
				_selectedRecipe = value;
				OnPropertyChanged("SelectedRecipe");
			}
		}
	}

	public ParamSettingConfig ParamSetting
	{
		get
		{
			return _paramSetting;
		}
		set
		{
			if (_paramSetting != value)
			{
				_paramSetting = value;
				OnPropertyChanged("ParamSetting");
			}
		}
	}
}
