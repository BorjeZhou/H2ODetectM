using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GCProject.Main.Configs;
using GCProject.Main.Data.Models;
using GCProject.Main.LangResource;

namespace GCProject.Main.Lib;

public class Library
{
	public const string ProgressName = "BACHIndustrieMesstechnologie";

	private const uint ES_SYSTEM_REQUIRED = 1u;

	private const uint ES_DISPLAY_REQUIRED = 2u;

	private const uint ES_CONTINUOUS = 2147483648u;

	public static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	[DllImport("kernel32.dll")]
	private static extern uint SetThreadExecutionState(uint esFlags);

	public static string GetCurrentPath()
	{
		return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
	}

	public static void HoldSystem()
	{
		SetThreadExecutionState(2147483651u);
	}

	public static void ReleaseSystem()
	{
		SetThreadExecutionState(2147483648u);
	}

	public static void GCSummeryToCsvFile(string file, List<GCSummaryEntity> Datas)
	{
		string[] Colnums = new string[11]
		{
			GCProject.Main.LangResource.Lang.RecordWindow_No,
			GCProject.Main.LangResource.Lang.RecordWindow_Start,
			GCProject.Main.LangResource.Lang.RecordWindow_End,
			GCProject.Main.LangResource.Lang.RecordWindow_Duration,
			GCProject.Main.LangResource.Lang.H2O,
			"CPS",
			GCProject.Main.LangResource.Lang.RecordWindow_Width,
			GCProject.Main.LangResource.Lang.RecordWindow_Height,
			GCProject.Main.LangResource.Lang.PackageNum,
			GCProject.Main.LangResource.Lang.PackageWeight,
			GCProject.Main.LangResource.Lang.RecordWindow_Temp
		};
		using StreamWriter sw = new StreamWriter(file, append: false, Encoding.UTF8);
		sw.WriteLine(string.Join(",", Colnums));
		int index = 1;
		foreach (GCSummaryEntity item in Datas)
		{
			sw.Write(index + ",");
			sw.Write($"{item.StartDT:yyyy-MM-dd HH:mm:ss},");
			sw.Write($"{item.EndDT:yyyy-MM-dd HH:mm:ss},");
			sw.Write($"{item.Duration},");
			sw.Write(item.H20.ToString(ConfigModel.DecimalStr) + ",");
			sw.Write($"{item.CPS},");
			sw.Write($"{item.Width},");
			sw.Write($"{item.Height},");
			sw.Write(item.PackageNo + ",");
			sw.Write($"{item.Weight},");
			sw.Write($"{item.Temperature}\r\n");
			sw.Flush();
			index++;
		}
		sw.Flush();
	}

	public static T MapObject<T>(T obj) where T : class
	{
		T res = Activator.CreateInstance(typeof(T)) as T;
		PropertyInfo[] properties = typeof(T).GetProperties();
		foreach (PropertyInfo pro in properties)
		{
			if (pro.CanWrite && pro.CanRead)
			{
				object value = pro.GetValue(obj);
				pro.SetValue(res, value);
			}
		}
		return res;
	}

	public static bool IsPropertychange<T>(T obj, T compareobj) where T : class
	{
		PropertyInfo[] properties = typeof(T).GetProperties();
		foreach (PropertyInfo pro in properties)
		{
			if (pro.CanWrite && pro.CanRead)
			{
				object? value = pro.GetValue(obj);
				object val2 = pro.GetValue(compareobj);
				if (!object.Equals(value, val2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void FindVisualElement<TElement>(Visual visual, Action<TElement> invoke, bool IncludeCollapsed = true) where TElement : Visual
	{
		if (visual == null || (visual is UIElement { Visibility: Visibility.Collapsed } && !IncludeCollapsed))
		{
			return;
		}
		if (visual is ContentControl cc)
		{
			FindVisualElement(cc.Content as Visual, invoke, IncludeCollapsed);
			return;
		}
		if (visual is Panel g)
		{
			{
				foreach (object? child in g.Children)
				{
					FindVisualElement(child as Visual, invoke, IncludeCollapsed);
				}
				return;
			}
		}
		if (visual is ItemsControl ic)
		{
			{
				foreach (object? item in (IEnumerable)ic.Items)
				{
					if (item is TabItem ti)
					{
						ti.IsSelected = true;
					}
					FindVisualElement(item as Visual, invoke, IncludeCollapsed);
				}
				return;
			}
		}
		if (visual is TElement find2)
		{
			invoke(find2);
			return;
		}
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
		{
			Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
			if (childVisual != null)
			{
				if (childVisual is TElement find)
				{
					invoke(find);
				}
				else
				{
					FindVisualElement(childVisual, invoke, IncludeCollapsed);
				}
			}
		}
	}

	public static void AddValidateRuleForAllTextBox(Visual visual, ValidationRule validationRule)
	{
		FindVisualElement(visual, delegate(TextBox tb)
		{
			tb.GetBindingExpression(TextBox.TextProperty)?.ParentBinding?.ValidationRules.Add(validationRule);
		});
	}

	public static bool CheckValidateRuleForAllTextBox(Visual visual, bool IncludeCollapsed = true)
	{
		try
		{
			FindVisualElement(visual, delegate(TextBox tb)
			{
				if (IncludeCollapsed || tb.Visibility != Visibility.Collapsed)
				{
					BindingExpression bindingExpression = tb.GetBindingExpression(TextBox.TextProperty);
					if (bindingExpression != null)
					{
						bindingExpression.UpdateSource();
						if (bindingExpression.HasError)
						{
							throw new Exception();
						}
					}
				}
			}, IncludeCollapsed);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static string CheckDataGridError(DataGrid dg)
	{
		for (int i = 0; i < dg.Items.Count; i++)
		{
			DependencyObject o = dg.ItemContainerGenerator.ContainerFromIndex(i);
			if (Validation.GetHasError(o))
			{
				return Validation.GetErrors(o)[0].ErrorContent.ToString();
			}
		}
		return null;
	}
}
