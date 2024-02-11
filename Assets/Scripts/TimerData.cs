using System;

public class TimerData
{
    public string tag;

    public TimerDelegate callback;

    public float endTime;

    public bool loop;

    public float delay;

    public bool cancelOnLoad;

    public bool stop;

    public bool Update(float time)
	{
		if (this.stop)
		{
			return true;
		}
		if (time < this.endTime)
		{
			return false;
		}
		this.callback();
		if (this.loop)
		{
			this.endTime = time + this.delay;
			return false;
		}
		this.stop = true;
		return true;
	}
}
