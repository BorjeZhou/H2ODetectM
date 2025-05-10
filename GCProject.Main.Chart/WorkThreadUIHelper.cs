using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace GCProject.Main.Chart;

public class WorkThreadUIHelper
{
	private static AutoResetEvent s_event = new AutoResetEvent(initialState: false);

	public static HostVisual StartWorkThreadAndCreateElement(Type ElememtType, Size ControlSize)
	{
		WorkThreadUIParam workThreadUIParam = new WorkThreadUIParam
		{
			hostVisual = new HostVisualEx(),
			ControlType = ElememtType,
			ControlSize = ControlSize
		};
		Thread thread = new Thread(CreateAndRun);
		thread.SetApartmentState(ApartmentState.STA);
		thread.IsBackground = true;
		thread.Start(workThreadUIParam);
		s_event.WaitOne();
		return workThreadUIParam.hostVisual;
	}

	private static void CreateAndRun(object param)
	{
		WorkThreadUIParam workThreadUIParam = (WorkThreadUIParam)param;
		HostVisualEx obj = (HostVisualEx)workThreadUIParam.hostVisual;
		VisualTargetPresentationSource visualTargetPresentationSource = new VisualTargetPresentationSource(obj);
		FrameworkElement Elememt = (FrameworkElement)workThreadUIParam.ControlType.GetConstructor(Type.EmptyTypes).Invoke(null);
		Elememt.Width = workThreadUIParam.ControlSize.Width;
		Elememt.Height = workThreadUIParam.ControlSize.Height;
		visualTargetPresentationSource.RootVisual = Elememt;
		obj.Context = Elememt;
		s_event.Set();
		Dispatcher.Run();
	}
}
