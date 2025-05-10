using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GCProject.Main.UserControls;

public partial class BusUnitControl : UserControl, INotifyPropertyChanged, IComponentConnector
{
	public bool IsRxUnit { get; set; } = true;


	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}

	public BusUnitControl()
	{
		InitializeComponent();
		base.Loaded += BusUnitControl_Loaded;
	}

	private void BusUnitControl_Loaded(object sender, RoutedEventArgs e)
	{
		if (IsRxUnit)
		{
			gd_tx.Visibility = Visibility.Collapsed;
		}
		else
		{
			gd_rx.Visibility = Visibility.Collapsed;
		}
	}
}
