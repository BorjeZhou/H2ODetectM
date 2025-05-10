using System.Windows;
using System.Windows.Media;

namespace GCProject.Main.Chart;

public class VisualTargetPresentationSource : PresentationSource
{
	private VisualTarget _visualTarget;

	public override Visual RootVisual
	{
		get
		{
			return _visualTarget.RootVisual;
		}
		set
		{
			Visual oldRoot = _visualTarget.RootVisual;
			_visualTarget.RootVisual = value;
			RootChanged(oldRoot, value);
			if (value is UIElement rootElement)
			{
				rootElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				rootElement.Arrange(new Rect(rootElement.DesiredSize));
			}
		}
	}

	public override bool IsDisposed => false;

	public VisualTargetPresentationSource(HostVisual hostVisual)
	{
		_visualTarget = new VisualTarget(hostVisual);
	}

	protected override CompositionTarget GetCompositionTargetCore()
	{
		return _visualTarget;
	}
}
