using System;
using System.Collections.Generic;
using System.Threading;

namespace GCProject.Main.Aggregate;

public static class EventAggregateMultiAction
{
	private static readonly Dictionary<Type, List<Action<object>>> RegisterHandlers = new Dictionary<Type, List<Action<object>>>();

	private static readonly Dictionary<Type, Func<object, object>> RegisterRequests = new Dictionary<Type, Func<object, object>>();

	public static void RegisterHandler(this object self, Type eventType, Action<object> eventHandleFun)
	{
		if (RegisterHandlers.ContainsKey(eventType))
		{
			if (!RegisterHandlers[eventType].Contains(eventHandleFun))
			{
				RegisterHandlers[eventType].Add(eventHandleFun);
			}
		}
		else
		{
			RegisterHandlers.Add(eventType, new List<Action<object>> { eventHandleFun });
		}
	}

	public static void RegisterRequest<TEventType>(this object self, Func<object, object> eventHandleFun)
	{
		RegisterRequests[typeof(TEventType)] = eventHandleFun;
	}

	public static void RegisterRequest(this object self, Type eventType, Func<object, object> eventHandleFun)
	{
		RegisterRequests[eventType] = eventHandleFun;
	}

	public static void RegisterHandler<TEventType>(this object self, Action<object> eventHandleFun)
	{
		self.RegisterHandler(typeof(TEventType), eventHandleFun);
	}

	public static void RegisterHandlerWithChilrens<TEventType>(this object self, Action<object> eventHandleFun)
	{
		self.RegisterHandler<TEventType>(eventHandleFun);
		Type[] types = typeof(TEventType).Assembly.GetTypes();
		foreach (Type childType in types)
		{
			if (childType.IsSubclassOf(typeof(TEventType)) && childType.IsPublic)
			{
				self.RegisterHandler(childType, eventHandleFun);
			}
		}
	}

	public static void UninstallHandler(this object self, Type unLoadEventType, Action<object> eventHandleFun = null)
	{
		if (!RegisterHandlers.ContainsKey(unLoadEventType))
		{
			return;
		}
		if (eventHandleFun == null)
		{
			RegisterHandlers.Remove(unLoadEventType);
		}
		else if (RegisterHandlers[unLoadEventType].Contains(eventHandleFun))
		{
			RegisterHandlers[unLoadEventType].Remove(eventHandleFun);
			if (RegisterHandlers[unLoadEventType].Count == 0)
			{
				RegisterHandlers.Remove(unLoadEventType);
			}
		}
	}

	public static void UninstallHandler<TUnLoadEventType>(this object self, Action<object> eventHandleFun)
	{
		self.UninstallHandler(typeof(TUnLoadEventType), eventHandleFun);
	}

	public static void UninstallHandlerWithChilrens<TUnLoadEventType>(this object self, Action<object> eventHandleFun)
	{
		self.UninstallHandler<TUnLoadEventType>(eventHandleFun);
		Type[] types = typeof(TUnLoadEventType).Assembly.GetTypes();
		foreach (Type childType in types)
		{
			if (childType.IsSubclassOf(typeof(TUnLoadEventType)) && childType.IsPublic)
			{
				self.UninstallHandler(childType, eventHandleFun);
			}
		}
	}

	public static object RequestEvent(this object self, object @event)
	{
		if (@event == null)
		{
			return null;
		}
		Type type = @event.GetType();
		if (RegisterRequests.ContainsKey(type) && RegisterRequests[type] != null)
		{
			return RegisterRequests[type](@event);
		}
		return null;
	}

	public static void PublishEvent(this object self, object @event, bool async = false)
	{
		if (@event == null)
		{
			return;
		}
		Type type = @event.GetType();
		if (!RegisterHandlers.ContainsKey(type) || RegisterHandlers[type] == null || RegisterHandlers[type].Count <= 0)
		{
			return;
		}
		foreach (Action<object> Handler in RegisterHandlers[type])
		{
			if (!async)
			{
				Handler(@event);
				continue;
			}
			ThreadPool.QueueUserWorkItem(delegate
			{
				Handler(@event);
			});
		}
	}
}
