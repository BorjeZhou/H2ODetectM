using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GCProject.Main.Configs;
using GCProject.Main.Data.Context;
using GCProject.Main.Data.Models;
using GCProject.Main.Interface;
using GCProject.Main.LangResource;
using GCProject.Main.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace GCProject.Main.Windows;

public partial class RecordWindow : Window, IWindow, IComponentConnector
{
	public class RecordItem
	{
		public string Date { get; set; }

		public List<GCSummaryEntity> SummaryEntities { get; set; } = new List<GCSummaryEntity>();


		public RecordItem(DateTime date)
		{
			Date = date.ToString("yyyy-MM-dd");
		}

		public void AttachRecipe(List<GCRecipe> recipes)
		{
			int index = 1;
			foreach (GCSummaryEntity item in SummaryEntities)
			{
				item.GCRecipe = recipes.FirstOrDefault((GCRecipe r) => r.GCRecipeID == item.GCRecipeID);
				if (item.GCRecipe == null)
				{
					item.GCRecipe = new GCRecipe
					{
						DisplayName = ""
					};
				}
				item.SampleNo = index++;
				item.GetAmendCPS(item.GCRecipe);
			}
		}
	}

	public List<int> Years { get; set; } = new List<int>();


	public List<int> Months { get; set; } = new List<int>();


	public RecordWindow()
	{
		InitializeComponent();
		LoadYearMonthDatas();
		base.StateChanged += RecordWindow_StateChanged;
	}

	private void RecordWindow_StateChanged(object sender, System.EventArgs e)
	{
		if (base.WindowState == WindowState.Minimized)
		{
			base.WindowState = WindowState.Normal;
		}
	}

	private void LoadYearMonthDatas()
	{
		DateTime now = DateTime.Now;
		Years.Add(now.Year);
		Years.Add(now.Year - 1);
		cmbx_year.ItemsSource = Years;
		cmbx_year.SelectedItem = now.Year;
		for (int i = 0; i < 12; i++)
		{
			Months.Add(i + 1);
		}
		cmbx_month.ItemsSource = Months;
		cmbx_month.SelectedItem = now.Month;
	}

	private string GetDateKey(DateTime date)
	{
		return date.ToString("yyyy-MM-dd");
	}

	private RecordItem LoadDate(DateTime date)
	{
		GetDateKey(date);
		_ = DateTime.Now;
		RecordItem Result = new RecordItem(date);
		DateTime DayFirst = new DateTime(date.Year, date.Month, date.Day);
		DateTime DayLast = DayFirst.AddDays(1.0).AddSeconds(-1.0);
		using GCContext db = new GCContext();
		Result.SummaryEntities = (from e in db.GCSummaryEntities.AsNoTracking()
			where !e.IsSampleData && e.StartDT > DayFirst && e.StartDT < DayLast
			select e).ToList();
		Result.AttachRecipe(ConfigModel.GCRecipes);
		return Result;
	}

	private List<string> _loadDateList(int year, int month)
	{
		DateTime First = new DateTime(year, month, 1);
		List<string> Result = new List<string>();
		using GCContext db = new GCContext();
		do
		{
			DateTime End = First.AddDays(1.0).AddSeconds(-1.0);
			if (db.GCSummaryEntities.AsNoTracking().Any((GCSummaryEntity e) => !e.IsSampleData && e.StartDT > First && e.StartDT < End))
			{
				Result.Add(First.ToString("yyyy-MM-dd"));
			}
			First = First.AddDays(1.0);
		}
		while (First.Month == month);
		return Result;
	}

	public void Init(object args)
	{
		ToLatestClick(null, null);
	}

	private void lv_sumarylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (lv_sumarylist.SelectedItem == null)
		{
			e.Handled = true;
		}
		else
		{
			RefreshCurrentDayData();
		}
	}

	private void ExitButtonClick(object sender, RoutedEventArgs e)
	{
		Hide();
		e.Handled = true;
	}

	private void ToLatestClick(object sender, RoutedEventArgs e)
	{
		DateTime lastestDate = DateTime.Now;
		using (GCContext context = new GCContext())
		{
			if (!(from s in context.GCSummaryEntities.AsNoTracking()
				where !s.IsSampleData
				select s).Any())
			{
				return;
			}
			long lastDateID = (from s in context.GCSummaryEntities.AsNoTracking()
				where !s.IsSampleData
				select s).Max((GCSummaryEntity s) => s.GCSummaryEntityID);
			GCSummaryEntity gCSummaryEntity = context.GCSummaryEntities.AsNoTracking().FirstOrDefault((GCSummaryEntity s) => s.GCSummaryEntityID == lastDateID);
			if (gCSummaryEntity == null)
			{
				lastestDate = DateTime.Now;
			}
			lastestDate = gCSummaryEntity.EndDT;
		}
		cmbx_year.SelectedItem = lastestDate.Year;
		cmbx_month.SelectedItem = lastestDate.Month;
		lv_sumarylist.ItemsSource = null;
		List<string> DateList = _loadDateList(lastestDate.Year, lastestDate.Month);
		lv_sumarylist.ItemsSource = DateList;
		lv_sumarylist.SelectedItem = (DateList.Any() ? DateList.Last() : null);
	}

	private void cmbx_date_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		int year = DateTime.Now.Year;
		int month = DateTime.Now.Month;
		if (cmbx_year.SelectedItem != null)
		{
			year = (int)cmbx_year.SelectedItem;
		}
		if (cmbx_month.SelectedItem != null)
		{
			month = (int)cmbx_month.SelectedItem;
		}
		lv_sumarylist.ItemsSource = _loadDateList(year, month);
		lv_sumarylist.SelectedIndex = 0;
	}

	private void btn_deleteClick(object sender, RoutedEventArgs e)
	{
		e.Handled = true;
		if (MainGrid.SelectedItems.Count == 0 || !PwdGuard.PerformInputPwd(this) || MessageBox.Show(GCProject.Main.LangResource.Lang.RecordWindow_DeleteConfirm, GCProject.Main.LangResource.Lang.Comfirm, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) != MessageBoxResult.OK)
		{
			return;
		}
		DateTime DeleteItemDate = DateTime.Today;
		using (GCContext db = new GCContext())
		{
			foreach (object? item in MainGrid.SelectedItems)
			{
				GCSummaryEntity entity = db.GCSummaryEntities.Find(((GCSummaryEntity)item).GCSummaryEntityID);
				if (entity != null)
				{
					DeleteItemDate = entity.EndDT;
					db.GCSummaryEntities.Remove(entity);
				}
			}
			db.SaveChanges();
		}
		RefreshCurrentDayData(DeleteItemDate.Date);
	}

	private DateTime GetCurrentSelectedDate()
	{
		DateTime loadDate = DateTime.Now;
		if (lv_sumarylist.SelectedItem != null)
		{
			loadDate = DateTime.Parse(lv_sumarylist.SelectedItem as string);
		}
		return loadDate;
	}

	private void RefreshCurrentDayData(DateTime date = default(DateTime))
	{
		DateTime loadDate = DateTime.Today;
		if (date != default(DateTime))
		{
			loadDate = date;
		}
		else if (lv_sumarylist.SelectedItem != null)
		{
			loadDate = DateTime.Parse(lv_sumarylist.SelectedItem as string);
		}
		RecordItem record = LoadDate(loadDate);
		MainGrid.ItemsSource = null;
		MainGrid.ItemsSource = record.SummaryEntities;
		DvGroupBox.Header = $"{GCProject.Main.LangResource.Lang.RecordWindow_DataView}  {record.Date}  {GCProject.Main.LangResource.Lang.RecordWindow_Total} {record.SummaryEntities.Count} {GCProject.Main.LangResource.Lang.RecordWindow_Records}";
	}

	private void ExportCsvClick(object sender, RoutedEventArgs e)
	{
		SaveFileDialog sf = new SaveFileDialog();
		DateTime loadDate = GetCurrentSelectedDate();
		sf.FileName = $"SummaryDatasFor{loadDate:yyyy_MM_dd}.csv";
		if (sf.ShowDialog().Value)
		{
			RecordItem record = LoadDate(loadDate);
			Library.GCSummeryToCsvFile(sf.FileName, record.SummaryEntities);
			MessageBox.Show(GCProject.Main.LangResource.Lang.RecordWindow_ExportSuccess, GCProject.Main.LangResource.Lang.Information, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
	}

	private void ShowRawDataWindowClick(object sender, RoutedEventArgs e)
	{
		if (MainGrid.SelectedItem != null)
		{
			RawDataChartWindow window = WindowsFactory.GetWindow<RawDataChartWindow>(this);
			window.Init(MainGrid.SelectedItem);
			window.ShowDialog();
		}
	}
}
