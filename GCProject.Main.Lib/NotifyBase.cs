using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GCProject.Main.Lib;

public class NotifyBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
	}
}
