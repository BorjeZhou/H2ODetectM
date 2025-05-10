using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GCProject.Main.Lib;

public class DataErrorInfoBase : IDataErrorInfo
{
	protected string error;

	public string Error => error;

	public string this[string propertyName]
	{
		get
		{
			if (propertyName == "Item")
			{
				return null;
			}
			PropertyInfo property = GetType().GetProperty(propertyName);
			if (property != null)
			{
				object value = property.GetValue(this, null);
				object[] customAttributes = property.GetCustomAttributes(inherit: false);
				for (int i = 0; i < customAttributes.Length; i++)
				{
					if (!(customAttributes[i] is ValidationAttribute validater) || validater.IsValid(value))
					{
						continue;
					}
					if (validater is RangeAttribute rg)
					{
						rg.ErrorMessage = $"this field must between {rg.Minimum} and {rg.Maximum}";
					}
					if (validater is BiggerThanAttrbute bt)
					{
						if (!double.TryParse(GetPropertyValue(bt.TargetPropertyName).ToString(), out var taget))
						{
							validater.ErrorMessage = null;
						}
						else if (bt.IsBigger)
						{
							if ((double)value < taget)
							{
								validater.ErrorMessage = bt.CustomError ?? "this field must greater than or equal to Min value";
							}
						}
						else if ((double)value > taget)
						{
							validater.ErrorMessage = bt.CustomError ?? "this field must less than or equal to Max value";
						}
					}
					error = validater.ErrorMessage;
					return validater.ErrorMessage;
				}
			}
			return null;
		}
	}

	public string ManulValidate(string name)
	{
		return this[name];
	}

	protected object GetPropertyValue(string Name)
	{
		PropertyInfo property = GetType().GetProperty(Name);
		if (property != null)
		{
			return property.GetValue(this);
		}
		return null;
	}

	public virtual bool IsValid()
	{
		PropertyInfo[] properties = GetType().GetProperties();
		foreach (PropertyInfo prop in properties)
		{
			if (!string.IsNullOrEmpty(ManulValidate(prop.Name)))
			{
				return false;
			}
		}
		error = null;
		return true;
	}
}
