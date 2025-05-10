using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using GCProject.Main.Configs;
using Microsoft.EntityFrameworkCore;

namespace GCProject.Main.Data.Models;

[Index(new string[] { "GCRecipeID" }, IsUnique = false)]
[Index(new string[] { "StartDT" }, IsUnique = true)]
public class GCSummaryEntity : INotifyPropertyChanged
{
	[NotMapped]
	private bool _isMultiSelected;

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long GCSummaryEntityID { get; set; }

	[NotMapped]
	public bool IsMultiSelected
	{
		get
		{
			return _isMultiSelected;
		}
		set
		{
			_isMultiSelected = value;
			OnPropertyChanged("IsMultiSelected");
		}
	}

	[NotMapped]
	public int Year => StartDT.Year;

	[NotMapped]
	public int Month => StartDT.Month;

	public DateTime StartDT { get; set; }

	public DateTime EndDT { get; set; }

	[NotMapped]
	public double Duration => Math.Round((EndDT - StartDT).TotalSeconds, 0);

	public double H20_OnlyCPS { get; set; }

	[NotMapped]
	public double Diff2 => H20_OnlyCPS - Labor;

	public double H20 { get; set; }

	public double CPS { get; set; }

	[NotMapped]
	public double CPS_ZeroRate { get; set; }

	public double Diff_H20_OnlyCPS { get; set; }

	[NotMapped]
	public double Diff => H20 - Labor;

	public double Width { get; set; }

	public double Labor { get; set; }

	public int SampleNo { get; set; }

	public double Temperature { get; set; }

	public double? RealZeroRate { get; set; }

	public bool IsSelected { get; set; }

	public bool IsTempSelected { get; set; }

	public bool IsSampleData { get; set; }

	public long GCRecipeID { get; set; }

	public string PackageNo { get; set; }

	public double? Weight { get; set; }

	public double? Height { get; set; }

	[NotMapped]
	public virtual GCRecipe GCRecipe { get; set; }

	public virtual ICollection<GCRawEntity> RawDatas { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public void CalculatorValueByRecipe(GCRecipe recipe, ParamSettingConfig set, bool CalTempCorretcion = true)
	{
		if (!set.WidthCorrectionEnabled)
		{
			Width = set.WidthSensorsDistance;
		}
		if (!set.HeightCorrectionEnabled)
		{
			Height = set.HeightSensorsDistance;
		}
		GetAmendCPS(recipe);
		double height = (Height.HasValue ? Height.Value : set.HeightSensorsDistance);
		(H20_OnlyCPS, H20) = recipe.CalH20(CPS, RealZeroRate, Width, set.WidthSensorsDistance, height, set.HeightSensorsDistance, showBusDataProxy: false, Temperature, CalTempCorretcion);
		if (H20 > set.H20MeasRangeMax_868M)
		{
			H20 = set.H20MeasRangeMax_868M;
		}
		else if (H20 < set.H20MeasRangeMin_868M)
		{
			H20 = set.H20MeasRangeMin_868M;
		}
	}

	public void GetAmendCPS(GCRecipe recipe)
	{
		try
		{
			CPS_ZeroRate = recipe.GetAmendCPS(CPS, RealZeroRate);
		}
		catch (Exception)
		{
		}
	}
}
