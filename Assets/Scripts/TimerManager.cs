using System;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
	private void Awake()
	{
        #if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }
        #endif
        if (TimerManager.instance == null)
		{
			TimerManager.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
	}

	private static void Init()
	{
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }
        #endif
        if (TimerManager.instance == null)
		{
			GameObject gameObject = new GameObject("TimerManager");
			gameObject.AddComponent<TimerManager>();
		}
	}

	private void Update()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }
        #endif
        TimerManager.stopOnUpdate = false;
		TimerManager.time = Time.time;
		for (int i = 0; i < this.list.size; i++)
		{
			if (this.list.size > i && this.list[i] != null && (TimerManager.time >= this.list[i].DueTime || this.list[i].ID == 0))
			{
				this.list[i].Execute();
			}
		}
		if (TimerManager.OnUpdate != null)
		{
			TimerManager.OnUpdate();
		}
	}

	public static int Start()
	{
		return TimerManager.Start(true);
	}

	public static int Start(bool cancelOnLoad)
	{
		return TimerManager.In(3.1536E+08f, cancelOnLoad, null);
	}

	public static float GetDuration(int id)
	{
		for (int i = 0; i < TimerManager.instance.list.size; i++)
		{
			if (TimerManager.instance.list[i].ID == id)
			{
				return Time.time - TimerManager.instance.list[i].StartTime;
			}
		}
		return 0f;
	}

	public static int In(float delay, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(string.Empty, delay, true, 1, 0f, callback);
	}

	public static int In(string tag, float delay, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(tag, delay, true, 1, 0f, callback);
	}

	public static int In(float delay, bool cancelOnLoad, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(string.Empty, delay, cancelOnLoad, 1, 0f, callback);
	}

	public static int In(string tag, float delay, bool cancelOnLoad, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(tag, delay, cancelOnLoad, 1, 0f, callback);
	}

	public static int In(float delay, int iterations, float interval, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
		return TimerManager.In(string.Empty, delay, true, iterations, interval, callback);
	}

	public static int In(string tag, float delay, int iterations, float interval, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(tag, delay, true, iterations, interval, callback);
	}

	public static int In(float delay, bool cancelOnLoad, int iterations, float interval, TimerManager.Callback callback)
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        return TimerManager.In(string.Empty, delay, true, iterations, interval, callback);
	}

	public static int In(string tag, float delay, bool cancelOnLoad, int iterations, float interval, TimerManager.Callback callback)
	{
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 0;
        }
        #endif
        TimerManager.Init();
		delay = Mathf.Max(0f, delay);
		iterations = Mathf.Max(0, iterations);
		interval = Mathf.Max(0f, interval);
		TimerManager.Event @event;
		if (TimerManager.instance.pool.size > 0)
		{
			@event = TimerManager.instance.pool[0];
			TimerManager.instance.pool.Remove(@event);
		}
		else
		{
			@event = new TimerManager.Event();
		}
		TimerManager.instance.eventCount++;
		@event.ID = TimerManager.instance.eventCount;
		@event.tag = tag;
		@event.Function = callback;
		@event.Iterations = iterations;
		@event.Interval = interval;
		@event.StartTime = Time.time;
		@event.DueTime = Time.time + delay;
		@event.CancelOnLoad = cancelOnLoad;
		TimerManager.instance.list.Add(@event);
		return @event.ID;
	}

	public static void Cancel(int id)
	{
		if (0 >= id)
		{
			return;
		}
		for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
		{
			if (TimerManager.instance.list[i].ID == id)
			{
				TimerManager.instance.list[i].ID = 0;
				break;
			}
		}
	}

	public static void Cancel(params int[] ids)
	{
		if (ids != null && ids.Length > 0)
		{
			for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
			{
				if (ids.Contains(TimerManager.instance.list[i].ID))
				{
					TimerManager.instance.list[i].ID = 0;
				}
			}
		}
	}

	public static void Cancel(string tag)
	{
		if (string.IsNullOrEmpty(tag))
		{
			return;
		}
		for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
		{
			if (TimerManager.instance.list[i].tag == tag)
			{
				TimerManager.instance.list[i].ID = 0;
			}
		}
	}

	public static void CancelAll()
	{
		for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
		{
			TimerManager.instance.list[i].ID = 0;
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		for (int i = this.list.size - 1; i > -1; i--)
		{
			if (this.list[i].CancelOnLoad)
			{
				this.list[i].ID = 0;
			}
		}
	}

	public static bool IsActive(int id)
	{
		for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
		{
			if (TimerManager.instance.list[i].ID == id)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsActive(string tag)
	{
		for (int i = TimerManager.instance.list.size - 1; i > -1; i--)
		{
			if (TimerManager.instance.list[i].tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public static float time;

	private BetterList<TimerManager.Event> list = new BetterList<TimerManager.Event>();

	private BetterList<TimerManager.Event> pool = new BetterList<TimerManager.Event>();

	public static TimerDelegate OnUpdate;

	public static bool stopOnUpdate;

	public int eventCount;

	private static TimerManager instance;

	private class Event
	{

		public void Execute()
		{
			if (this.ID == 0 || this.DueTime == 0f)
			{
				this.Recycle();
				return;
			}
			if (this.Function != null)
			{
				try
				{
					this.Function();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				if (this.Iterations > 0)
				{
					this.Iterations--;
					if (this.Iterations < 1)
					{
						this.Recycle();
						return;
					}
				}
				this.DueTime = Time.time + this.Interval;
				return;
			}
			this.Recycle();
		}

		public void Recycle()
		{
			this.ID = 0;
			this.DueTime = 0f;
			this.StartTime = 0f;
			this.CancelOnLoad = true;
			this.Function = null;
			if (TimerManager.instance.list.Remove(this))
			{
				TimerManager.instance.pool.Add(this);
			}
		}

		public int ID;

		public string tag;

		public TimerManager.Callback Function;

		public int Iterations = 1;

		public float Interval = -1f;

		public float StartTime;

		public float DueTime;

		public bool CancelOnLoad = true;
	}

	public delegate void Callback();
}
