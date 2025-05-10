using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;

namespace GCProject.Main.Windows;

public partial class RegressionSampleCompareWindow : Window, IWindow, IComponentConnector
{
	public RegressionSampleCompareWindow()
	{
		InitializeComponent();
	}

	public void Init(object args)
	{
		GCRecipe cRecipe = args as GCRecipe;
		if (cRecipe == null)
		{
			return;
		}
		using GCContext db = new GCContext();
		List<GCSummaryEntity> Datas = db.GCSummaryEntities.Where((GCSummaryEntity g) => g.IsSelected && g.GCRecipeID == cRecipe.GCRecipeID).ToList();
		MainChart.SetDatas(Datas.Select((GCSummaryEntity g) => g.H20).ToList(), Datas.Select((GCSummaryEntity g) => g.Labor).ToList());
	}
}
