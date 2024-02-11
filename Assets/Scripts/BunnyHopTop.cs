using System;
using System.Collections.Generic;
using System.Text;
using FreeJSON;
using UnityEngine;

public class BunnyHopTop : MonoBehaviour
{
    public TextMesh Text;

    private List<BunnyHopTop.PlayerData> list = new List<BunnyHopTop.PlayerData>();

    private int Deaths;

    private int Timer;

    private static BunnyHopTop instance;

    private void Start()
	{
		BunnyHopTop.instance = this;
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	public static void StartTimer()
	{
		if (BunnyHopTop.instance == null)
		{
			return;
		}
		TimerManager.Cancel(BunnyHopTop.instance.Timer);
		BunnyHopTop.instance.Timer = TimerManager.Start();
		BunnyHopTop.instance.Deaths = 0;
	}

	public static void RestartTimer()
	{
		if (BunnyHopTop.instance == null)
		{
			return;
		}
		BunnyHop.SetDataTopList(TimerManager.GetDuration(BunnyHopTop.instance.Timer), BunnyHopTop.instance.Deaths);
		BunnyHopTop.StartTimer();
	}

	public static void StopTimer()
	{
		if (BunnyHopTop.instance == null)
		{
			return;
		}
		TimerManager.Cancel(BunnyHopTop.instance.Timer);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		this.Deaths++;
	}

	public static void UpdateData(string playerName, float time, int deaths)
	{
		bool flag = false;
		for (int i = 0; i < BunnyHopTop.instance.list.Count; i++)
		{
			if (BunnyHopTop.instance.list[i].name == playerName)
			{
				if (BunnyHopTop.instance.list[i].time > time)
				{
					BunnyHopTop.instance.list[i].time = time;
					BunnyHopTop.instance.list[i].deaths = deaths;
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			BunnyHopTop.instance.list.Add(new BunnyHopTop.PlayerData(playerName, time, deaths));
		}
		BunnyHopTop.instance.list.Sort(new Comparison<BunnyHopTop.PlayerData>(BunnyHopTop.SortByTime));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Top List:");
		for (int j = 0; j < BunnyHopTop.instance.list.Count; j++)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				(j + 1).ToString(),
				". ",
				BunnyHopTop.instance.list[j].name,
				" ",
				BunnyHopTop.ConvertTime(BunnyHopTop.instance.list[j].time),
				" / ",
				BunnyHopTop.instance.list[j].deaths
			}));
			if (j == 4)
			{
				break;
			}
		}
		BunnyHopTop.instance.Text.text = stringBuilder.ToString();
	}

	public static int SortByTime(BunnyHopTop.PlayerData a, BunnyHopTop.PlayerData b)
	{
		if (a.time != b.time)
		{
			return a.time.CompareTo(b.time);
		}
		if (a.deaths == b.deaths)
		{
			return a.name.CompareTo(b.name);
		}
		return a.deaths.CompareTo(b.deaths);
	}

	private static string ConvertTime(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		return string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
	}

	public static string GetTopList()
	{
		if (BunnyHopTop.instance == null)
		{
			return string.Empty;
		}
		JsonArray jsonArray = new JsonArray();
		for (int i = 0; i < BunnyHopTop.instance.list.Count; i++)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.Add("n", BunnyHopTop.instance.list[i].name);
			jsonObject.Add("t", BunnyHopTop.instance.list[i].time);
			jsonObject.Add("d", BunnyHopTop.instance.list[i].deaths);
			jsonArray.Add(jsonObject);
			if (i == 4)
			{
				break;
			}
		}
		return jsonArray.ToString();
	}

	public static void SetTopList(string data)
	{
		if (BunnyHopTop.instance == null)
		{
			return;
		}
		JsonArray jsonArray = JsonArray.Parse(data);
		for (int i = 0; i < jsonArray.Length; i++)
		{
			JsonObject jsonObject = jsonArray.Get<JsonObject>(i);
			BunnyHopTop.PlayerData item = new BunnyHopTop.PlayerData(jsonObject.Get<string>("n"), jsonObject.Get<float>("t"), jsonObject.Get<int>("n"));
			BunnyHopTop.instance.list.Add(item);
		}
		BunnyHopTop.instance.list.Sort(new Comparison<BunnyHopTop.PlayerData>(BunnyHopTop.SortByTime));
		StringBuilder stringBuilder = new StringBuilder();
		if (BunnyHopTop.instance.list.Count == 0)
		{
			return;
		}
		stringBuilder.AppendLine("Top List:");
		for (int j = 0; j < BunnyHopTop.instance.list.Count; j++)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				(j + 1).ToString(),
				". ",
				BunnyHopTop.instance.list[j].name,
				" ",
				BunnyHopTop.ConvertTime(BunnyHopTop.instance.list[j].time),
				" / ",
				BunnyHopTop.instance.list[j].deaths
			}));
			if (j == 4)
			{
				break;
			}
		}
		BunnyHopTop.instance.Text.text = stringBuilder.ToString();
	}

	public class PlayerData
	{
		public PlayerData(string n, float t, int d)
		{
			this.name = n;
			this.time = t;
			this.deaths = d;
		}

		public string name;

		public float time;

		public int deaths;
	}
}
