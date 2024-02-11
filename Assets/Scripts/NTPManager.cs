using System;
using System.Collections;
using UnityEngine;

public class NTPManager : MonoBehaviour
{
    private string Url = "http://chronic.herokuapp.com/utc/now";

    private Action<DateTime> Callback;

    public static int LastGetSeconds;

    private static NTPManager instance;

    private void Awake()
	{
		NTPManager.instance = this;
	}

	public static void GetTime(Action<DateTime> callback)
	{
		NTPManager.instance.Callback = callback;
		NTPManager.instance.StartCoroutine(NTPManager.instance.GetNTPTime());
	}

	public static int GetSeconds(DateTime time)
	{
		DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (int)(time.ToUniversalTime() - d).TotalSeconds;
	}

	public static long GetMilliSeconds(DateTime time)
	{
		DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (long)(time.ToUniversalTime() - d).TotalMilliseconds;
	}

	public static int GetHours(DateTime time)
	{
		DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return (int)(time.ToUniversalTime() - d).TotalHours;
	}

	private IEnumerator GetNTPTime()
	{
		WWW www = new WWW(this.Url);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			string[] data = www.text.Split(new char[]
			{
				" "[0]
			});
			string[] date = data[0].Split(new char[]
			{
				"-"[0]
			});
			string[] time = data[1].Split(new char[]
			{
				":"[0]
			});
			int year = int.Parse(date[0]);
			int month = int.Parse(date[1]);
			int day = int.Parse(date[2]);
			int hour = int.Parse(time[0]);
			int minute = int.Parse(time[1]);
			int second = int.Parse(time[2]);
			DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
			NTPManager.LastGetSeconds = NTPManager.GetSeconds(dateTime);
			if (this.Callback != null)
			{
				this.Callback(dateTime);
			}
		}
		else
		{
			this.Callback(new DateTime(1970L));
		}
		yield break;
	}
}
