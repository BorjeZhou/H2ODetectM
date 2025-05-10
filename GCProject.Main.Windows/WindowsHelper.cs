using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GCProject.Main.Windows;

public class WindowsHelper
{
	public static void GetTextBoxsError(DependencyObject obj)
	{
		foreach (object? child in LogicalTreeHelper.GetChildren(obj))
		{
			if (child is TextBox tb && Validation.GetHasError(tb) && Validation.GetErrors(tb).FirstOrDefault((ValidationError e) => e.Exception != null && e.Exception.GetType() == typeof(FormatException)) != null)
			{
				throw new Exception();
			}
			if (child is DependencyObject do1)
			{
				GetTextBoxsError(do1);
			}
		}
	}
}
