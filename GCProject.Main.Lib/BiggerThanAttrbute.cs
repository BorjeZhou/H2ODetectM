using System;
using System.ComponentModel.DataAnnotations;

namespace GCProject.Main.Lib;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class BiggerThanAttrbute : ValidationAttribute
{
	public string TargetPropertyName { get; private set; }

	public bool IsBigger { get; private set; }

	public string CustomError { get; set; }

	public BiggerThanAttrbute(string targetPropertyName, bool isBigger = true, string CustomError = null)
	{
		TargetPropertyName = targetPropertyName;
		this.CustomError = CustomError;
		IsBigger = isBigger;
	}

	public override bool IsValid(object value)
	{
		return false;
	}
}
