using System;

namespace GCProject.Main.Lib;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class CalculatorBaseAttribute : Attribute
{
	public string Name { get; set; }

	public string NotifyProName { get; set; }

	public int PriorityLevel { get; set; }

	public bool KeepNotify { get; set; } = true;


	public int NotifyIntervalMS { get; set; } = 1000;


	public bool UpdateImmediately { get; set; }

	public int Decimals { get; set; } = 2;


	public virtual CalTypeEnum CalType { get; set; } = CalTypeEnum.None;

}
