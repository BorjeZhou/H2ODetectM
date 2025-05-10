using GCProject.Main.Lib;

namespace GCProject.Main.Windows;

public class SplitCPSViewModel : NotifyBase
{
	private double _Split2_CPS;

	private double _Split3_CPS;

	private double _Split2_H20;

	private double _Split3_H20;

	private bool _EnabledStage2;

	private bool _EnabledStage3;

	public double Split2_CPS
	{
		get
		{
			return _Split2_CPS;
		}
		set
		{
			if (_Split2_CPS != value)
			{
				_Split2_CPS = value;
				OnPropertyChanged("Split2_CPS");
			}
		}
	}

	public double Split3_CPS
	{
		get
		{
			return _Split3_CPS;
		}
		set
		{
			if (_Split3_CPS != value)
			{
				_Split3_CPS = value;
				OnPropertyChanged("Split3_CPS");
			}
		}
	}

	public double Split2_H20
	{
		get
		{
			return _Split2_H20;
		}
		set
		{
			if (_Split2_H20 != value)
			{
				_Split2_H20 = value;
				OnPropertyChanged("Split2_H20");
			}
		}
	}

	public double Split3_H20
	{
		get
		{
			return _Split3_H20;
		}
		set
		{
			if (_Split3_H20 != value)
			{
				_Split3_H20 = value;
				OnPropertyChanged("Split3_H20");
			}
		}
	}

	public bool EnabledStage2
	{
		get
		{
			return _EnabledStage2;
		}
		set
		{
			if (_EnabledStage2 != value)
			{
				_EnabledStage2 = value;
				OnPropertyChanged("EnabledStage2");
			}
		}
	}

	public bool EnabledStage3
	{
		get
		{
			return _EnabledStage3;
		}
		set
		{
			if (_EnabledStage3 != value)
			{
				_EnabledStage3 = value;
				OnPropertyChanged("EnabledStage3");
			}
		}
	}

	public bool UscCPSSplitType { get; set; } = true;

}
