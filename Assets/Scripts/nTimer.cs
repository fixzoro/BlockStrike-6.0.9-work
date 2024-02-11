using System;
using UnityEngine;

public class nTimer : MonoBehaviour
{
    private BetterList<TimerData> timers = new BetterList<TimerData>();

    private BetterList<TimerData> pool = new BetterList<TimerData>();

    private static int maxInvokeInUpdate = 5;

    private void OnEnable()
	{
		TimerManager.OnUpdate = (TimerDelegate)Delegate.Combine(TimerManager.OnUpdate, new TimerDelegate(this.OnUpdate));
	}

	private void OnDisable()
	{
		TimerManager.OnUpdate = (TimerDelegate)Delegate.Remove(TimerManager.OnUpdate, new TimerDelegate(this.OnUpdate));
	}

	public TimerData Create(string tag, float delay, TimerDelegate callback)
	{
		return this.Create(tag, delay, false, callback);
	}

	public TimerData Create(string tag, float delay, bool loop, TimerDelegate callback)
	{
		TimerData timerData = new TimerData();
		timerData.tag = tag;
		timerData.delay = delay;
		timerData.endTime = TimerManager.time + timerData.delay;
		timerData.loop = loop;
		timerData.callback = callback;
		timerData.stop = false;
		this.pool.Add(timerData);
		return timerData;
	}

	public TimerData In(string tag)
	{
		for (int i = 0; i < this.pool.size; i++)
		{
			if (this.pool[i].tag == tag)
			{
				this.pool[i].endTime = TimerManager.time + this.pool[i].delay;
				this.pool[i].stop = false;
				this.timers.Add(this.pool[i]);
				return this.pool[i];
			}
		}
		Debug.LogError("No Find Timer: " + tag);
		return null;
	}

	public TimerData In(string tag, float delay)
	{
		for (int i = 0; i < this.pool.size; i++)
		{
			if (this.pool[i].tag == tag)
			{
				this.pool[i].delay = delay;
				this.pool[i].endTime = TimerManager.time + delay;
				this.pool[i].stop = false;
				this.timers.Add(this.pool[i]);
				return this.pool[i];
			}
		}
		Debug.LogError("No Find Timer: " + tag);
		return null;
	}

	public TimerData In(float delay, TimerDelegate callback)
	{
		return this.In(string.Empty, delay, false, callback);
	}

	public TimerData In(float delay, bool loop, TimerDelegate callback)
	{
		return this.In(string.Empty, delay, loop, callback);
	}

	public TimerData In(string tag, float delay, bool loop, TimerDelegate callback)
	{
		TimerData timerData = new TimerData();
		timerData.tag = tag;
		timerData.delay = delay;
		timerData.endTime = TimerManager.time + timerData.delay;
		timerData.loop = loop;
		timerData.callback = callback;
		timerData.stop = false;
		this.timers.Add(timerData);
		return timerData;
	}

	public bool Contains(string tag)
	{
		for (int i = 0; i < this.pool.size; i++)
		{
			if (this.pool[i].tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public bool isActive(string tag)
	{
		for (int i = 0; i < this.timers.size; i++)
		{
			if (this.timers[i].tag == tag)
			{
				return true;
			}
		}
		return false;
	}

	public void Cancel(string tag)
	{
		for (int i = 0; i < this.timers.size; i++)
		{
			if (this.timers[i].tag == tag)
			{
				this.timers[i].stop = true;
			}
		}
	}

	private void OnUpdate()
	{
		nProfiler.BeginSample("OnUpdate");
		if (TimerManager.stopOnUpdate)
		{
			return;
		}
		for (int i = 0; i < this.timers.size; i++)
		{
			if (this.timers[i].Update(TimerManager.time))
			{
				this.timers.RemoveAt(i);
				i--;
				if (nTimer.maxInvokeInUpdate < 0)
				{
					nTimer.maxInvokeInUpdate = 5;
					TimerManager.stopOnUpdate = true;
					break;
				}
				nTimer.maxInvokeInUpdate--;
			}
		}
		nProfiler.EndSample();
	}
}
