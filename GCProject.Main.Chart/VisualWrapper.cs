using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace GCProject.Main.Chart;

[ContentProperty("Child")]
public class VisualWrapper : FrameworkElement
{
	private Visual _child;

	public Visual Child
	{
		get
		{
			return _child;
		}
		set
		{
			if (_child != null)
			{
				RemoveVisualChild(_child);
			}
			if (value != null)
			{
				_child = value;
				AddVisualChild(_child);
			}
		}
	}

	protected override int VisualChildrenCount => (_child != null) ? 1 : 0;

	protected override Visual GetVisualChild(int index)
	{
		if (_child != null && index == 0)
		{
			return _child;
		}
		return null;
	}
}
