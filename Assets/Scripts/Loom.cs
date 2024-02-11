using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Loom : MonoBehaviour
{
    public static int maxThreads = 8;

    private static int numThreads;

    private static Loom _current;

    private int _count;

    private static bool initialized;

    private List<Action> _actions = new List<Action>();

    private List<Loom.DelayedQueueItem> _delayed = new List<Loom.DelayedQueueItem>();

    private List<Loom.DelayedQueueItem> _currentDelayed = new List<Loom.DelayedQueueItem>();

    private List<Action> _currentActions = new List<Action>();

    public struct DelayedQueueItem
    {
        public float time;

        public Action action;
    }

    public static Loom Current
	{
		get
		{
			Loom.Initialize();
			return Loom._current;
		}
	}

	private void Awake()
	{
		Loom._current = this;
		Loom.initialized = true;
	}

	private static void Initialize()
	{
		if (!Loom.initialized)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Loom.initialized = true;
			GameObject gameObject = new GameObject("Loom");
            #if UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            #endif
            Loom._current = gameObject.AddComponent<Loom>();
		}
	}

	public static void QueueOnMainThread(Action action)
	{
		Loom.QueueOnMainThread(action, 0f);
	}

	public static void QueueOnMainThread(Action action, float time)
	{
		if (time != 0f)
		{
			List<Loom.DelayedQueueItem> delayed = Loom.Current._delayed;
			lock (delayed)
			{
				Loom.Current._delayed.Add(new Loom.DelayedQueueItem
				{
					time = Time.time + time,
					action = action
				});
			}
		}
		else
		{
			List<Action> actions = Loom.Current._actions;
			lock (actions)
			{
				Loom.Current._actions.Add(action);
			}
		}
	}

	public static Thread RunAsync(Action a)
	{
		Loom.Initialize();
		while (Loom.numThreads >= Loom.maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref Loom.numThreads);
		ThreadPool.QueueUserWorkItem(new WaitCallback(Loom.RunAction), a);
		return null;
	}

	private static void RunAction(object action)
	{
		try
		{
			((Action)action)();
		}
		catch
		{
		}
		finally
		{
			Interlocked.Decrement(ref Loom.numThreads);
		}
	}

	private void OnDisable()
	{
		if (Loom._current == this)
		{
			Loom._current = null;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		List<Action> actions = this._actions;
		lock (actions)
		{
			this._currentActions.Clear();
			this._currentActions.AddRange(this._actions);
			this._actions.Clear();
		}
		foreach (Action action in this._currentActions)
		{
			action();
		}
		List<Loom.DelayedQueueItem> delayed = this._delayed;
		lock (delayed)
		{
			this._currentDelayed.Clear();
			this._currentDelayed.AddRange(from d in this._delayed
			where d.time <= Time.time
			select d);
			foreach (Loom.DelayedQueueItem item in this._currentDelayed)
			{
				this._delayed.Remove(item);
			}
		}
		foreach (Loom.DelayedQueueItem delayedQueueItem in this._currentDelayed)
		{
			delayedQueueItem.action();
		}
	}
}
