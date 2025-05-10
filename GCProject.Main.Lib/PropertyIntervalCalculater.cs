using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GCProject.Main.Lib;

public class PropertyIntervalCalculater
{
	private Dictionary<string, CalculatorAttribute> Items = new Dictionary<string, CalculatorAttribute>();

	private List<int> PriorityLevels = new List<int>();

	public Action<string> DelegateFunc;

	private Thread RunThread;

	public double this[string key]
	{
		get
		{
			return GetValue(key);
		}
		set
		{
			SetValue(value, key);
		}
	}

	public PropertyIntervalCalculater(Type type, Action<string> func)
	{
		DelegateFunc = func;
		Init(type);
		RunThread = new Thread(Run);
		RunThread.IsBackground = true;
		RunThread.Start();
	}

	private void Run()
	{
		if (DelegateFunc == null)
		{
			throw new Exception("No PropertyChangedNotify Delegate Exist for calculator");
		}
		Thread.Sleep(1000);
		while (true)
		{
			foreach (int level in PriorityLevels)
			{
				List<CalculatorAttribute> List = Items.Values.Where((CalculatorAttribute v) => v.PriorityLevel == level).ToList();
				ThreadPool.QueueUserWorkItem(CalAvgCallback, List);
			}
			Thread.Sleep(1000);
		}
	}

	private void CalAvgCallback(object p)
	{
		(p as List<CalculatorAttribute>).ForEach(delegate(CalculatorAttribute item)
		{
			item.CalRes();
			if (item.KeepNotify)
			{
				DelegateFunc(item.NotifyProName);
			}
		});
	}

	public CalculatorAttribute GetCalculatorItem(string Name)
	{
		if (Items.ContainsKey(Name))
		{
			return Items[Name];
		}
		return null;
	}

	public double GetValue([CallerMemberName] string Name = null)
	{
		if (Name == null)
		{
			return 0.0;
		}
		return Items[Name].GetResult();
	}

	public void SetValue(double value, [CallerMemberName] string Name = null)
	{
		if (Name != null)
		{
			Items[Name].Update(value);
		}
	}

	public double SetValueWithReturn(double value, [CallerMemberName] string Name = null)
	{
		if (Name == null)
		{
			return 0.0;
		}
		CalculatorAttribute calculatorAttribute = Items[Name];
		calculatorAttribute.Update(value);
		return calculatorAttribute.GetResult();
	}

	public void ClearValue([CallerMemberName] string Name = null)
	{
		if (Items.ContainsKey(Name))
		{
			Items[Name].Clear();
		}
	}

	public void ClearValuesAuto()
	{
		foreach (CalculatorAttribute item in Items.Values)
		{
			if (item.CalType == CalTypeEnum.Average_Total)
			{
				item.Clear();
			}
		}
	}

	private void Init(Type type)
	{
		Items.Clear();
		PropertyInfo[] properties = type.GetProperties();
		foreach (PropertyInfo Pro in properties)
		{
			object[] attrs = Pro.GetCustomAttributes(typeof(CalculatorAttribute), inherit: true);
			if (attrs.Length != 0)
			{
				CalculatorAttribute a = attrs[0] as CalculatorAttribute;
				if (string.IsNullOrEmpty(a.Name))
				{
					a.Name = Pro.Name;
				}
				if (string.IsNullOrEmpty(a.NotifyProName))
				{
					a.NotifyProName = Pro.Name;
				}
				if (!PriorityLevels.Contains(a.PriorityLevel))
				{
					PriorityLevels.Add(a.PriorityLevel);
				}
				Items.Add(a.Name, a);
			}
		}
		PriorityLevels = PriorityLevels.OrderBy((int r) => r).ToList();
	}
}
