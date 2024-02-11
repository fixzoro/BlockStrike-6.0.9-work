using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public List<EventManager.EventClass> Events = new List<EventManager.EventClass>();

    public Dictionary<string, Delegate> Values = new Dictionary<string, Delegate>();

    public Dictionary<string, List<Delegate>> ValuesList = new Dictionary<string, List<Delegate>>();

    private static EventManager instance;

    private int nextID;

    public struct EventClass
    {

        public string key;

        public int id;

        public Delegate mDelegate;

        public bool dontDestroy;
    }

    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, T2>(T arg1, T2 arg2);

    public delegate void Callback<T, T2, T3>(T arg1, T2 arg2, T3 arg3);

    public delegate T Getter<T>();

    private static void Init()
	{
		if (EventManager.instance == null)
		{
			GameObject gameObject = new GameObject("EventManager");
			EventManager.instance = gameObject.AddComponent<EventManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		EventManager.ClearEventsAll();
		EventManager.ClearValuesAll();
	}

	public static int AddListener(string key, EventManager.Callback e)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener(string key, EventManager.Callback e, bool dontDestroy)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T>(string key, EventManager.Callback<T> e)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T>(string key, EventManager.Callback<T> e, bool dontDestroy)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T, T2>(string key, EventManager.Callback<T, T2> e)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T, T2>(string key, EventManager.Callback<T, T2> e, bool dontDestroy)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, dontDestroy);
	}

	public static int AddListener<T, T2, T3>(string key, EventManager.Callback<T, T2, T3> e)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, false);
	}

	public static int AddListener<T, T2, T3>(string key, EventManager.Callback<T, T2, T3> e, bool dontDestroy)
	{
		EventManager.Init();
		return EventManager.instance.OnListenerAdding(key, e, dontDestroy);
	}

	private int OnListenerAdding(string key, Delegate del, bool dontDestroy)
	{
		EventManager.EventClass item = default(EventManager.EventClass);
		item.key = key;
		item.id = this.GetNextID();
		item.mDelegate = del;
		item.dontDestroy = dontDestroy;
		this.Events.Add(item);
		return item.id;
	}

	public static void Dispatch(string key)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].key == key)
			{
				EventManager.Callback callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback;
				if (callback != null)
				{
					callback();
				}
			}
		}
	}

	public static void Dispatch(int id)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].id == id)
			{
				EventManager.Callback callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback;
				if (callback != null)
				{
					callback();
				}
				break;
			}
		}
	}

	public static void Dispatch<T>(string key, T e)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].key == key)
			{
				EventManager.Callback<T> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T>;
				if (callback != null)
				{
					callback(e);
				}
			}
		}
	}

	public static void Dispatch<T>(int id, T e)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].id == id)
			{
				EventManager.Callback<T> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T>;
				if (callback != null)
				{
					callback(e);
				}
				break;
			}
		}
	}

	public static void Dispatch<T, T2>(string key, T e, T2 e2)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].key == key)
			{
				EventManager.Callback<T, T2> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T, T2>;
				if (callback != null)
				{
					callback(e, e2);
				}
			}
		}
	}

	public static void Dispatch<T, T2>(int id, T e, T2 e2)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].id == id)
			{
				EventManager.Callback<T, T2> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T, T2>;
				if (callback != null)
				{
					callback(e, e2);
				}
				break;
			}
		}
	}

	public static void Dispatch<T, T2, T3>(string key, T e, T2 e2, T3 e3)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].key == key)
			{
				EventManager.Callback<T, T2, T3> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T, T2, T3>;
				if (callback != null)
				{
					callback(e, e2, e3);
				}
			}
		}
	}

	public static void Dispatch<T, T2, T3>(int id, T e, T2 e2, T3 e3)
	{
		EventManager.Init();
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].id == id)
			{
				EventManager.Callback<T, T2, T3> callback = EventManager.instance.Events[i].mDelegate as EventManager.Callback<T, T2, T3>;
				if (callback != null)
				{
					callback(e, e2, e3);
				}
				break;
			}
		}
	}

	public static void ClearEvent(string key)
	{
		if (EventManager.instance == null)
		{
			return;
		}
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].key == key)
			{
				EventManager.instance.Events.RemoveAt(i);
			}
		}
	}

	public static void ClearEvent(int id)
	{
		if (EventManager.instance == null)
		{
			return;
		}
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (EventManager.instance.Events[i].id == id)
			{
				EventManager.instance.Events.RemoveAt(i);
				break;
			}
		}
	}

	public static void ClearEventsAll()
	{
		if (EventManager.instance == null)
		{
			return;
		}
		for (int i = 0; i < EventManager.instance.Events.Count; i++)
		{
			if (!EventManager.instance.Events[i].dontDestroy)
			{
				EventManager.instance.Events.RemoveAt(i);
				i--;
			}
		}
	}

	private int GetNextID()
	{
		this.nextID++;
		return this.nextID;
	}

	public static void SetValue<T>(string key, EventManager.Getter<T> e)
	{
		EventManager.Init();
		EventManager.instance.Values.Add(key, e);
	}

	public static T GetValue<T>(string key)
	{
		EventManager.Init();
		if (EventManager.instance.Values.ContainsKey(key))
		{
			EventManager.Getter<T> getter = EventManager.instance.Values[key] as EventManager.Getter<T>;
			if (getter != null)
			{
				return getter();
			}
		}
		return default(T);
	}

	public static bool HasValue(string key)
	{
		EventManager.Init();
		return EventManager.instance.Values.ContainsKey(key);
	}

	public static void ClearValue(string key)
	{
		EventManager.Init();
		EventManager.instance.Values.Remove(key);
	}

	public static void ClearValuesAll()
	{
		EventManager.Init();
		EventManager.instance.Values.Clear();
	}

	public static void SetValueList<T>(string key, EventManager.Getter<T> e)
	{
		EventManager.Init();
		if (!EventManager.instance.ValuesList.ContainsKey(key))
		{
			EventManager.instance.ValuesList.Add(key, new List<Delegate>());
		}
		EventManager.instance.ValuesList[key].Add(e);
	}

	public static T GetValueList<T>(string key)
	{
		EventManager.Init();
		if (EventManager.instance.ValuesList.ContainsKey(key))
		{
			EventManager.Getter<T> getter = EventManager.instance.Values[key] as EventManager.Getter<T>;
			if (getter != null)
			{
				return getter();
			}
		}
		return default(T);
	}
}
