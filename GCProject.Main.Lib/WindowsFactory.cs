using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using GCProject.Main.Interface;

namespace GCProject.Main.Lib;

public class WindowsFactory
{
	private static Dictionary<Type, IWindow> WindowsCaches = new Dictionary<Type, IWindow>();

	public static void PreBuildWindows()
	{
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		foreach (Type t in types)
		{
			if (t.GetInterface("IWindow") != null)
			{
				Window window = Activator.CreateInstance(t) as Window;
				window.Closing += Window_Closing;
				WindowsCaches.Add(t, window as IWindow);
			}
		}
	}

	private static void Window_Closing(object sender, CancelEventArgs e)
	{
		if (sender is IClose c)
		{
			c.SelfClose();
		}
		else if (sender is Window w)
		{
			w.Hide();
		}
		e.Cancel = true;
	}

	public static T GetWindow<T>(Window Owner = null) where T : Window, IWindow
	{
		if (WindowsCaches.ContainsKey(typeof(T)))
		{
			T window = WindowsCaches[typeof(T)] as T;
			if (Owner != null)
			{
				window.Owner = Owner;
			}
			window.Init(null);
			return window;
		}
		return null;
	}

	public static Window GetWindow(Type windowType, Window Owner = null)
	{
		if (WindowsCaches.ContainsKey(windowType))
		{
			Window window = WindowsCaches[windowType] as Window;
			if (Owner != null)
			{
				window.Owner = Owner;
			}
			(window as IWindow).Init(null);
			return window;
		}
		return null;
	}
}
