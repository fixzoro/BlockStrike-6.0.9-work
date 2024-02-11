using System;
using BestHTTP;
using BestHTTP.ServerSentEvents;
using FreeJSON;
using UnityEngine;

[Serializable]
public class mClanChat
{
    public GameObject panel;

    public UILabel nameLabel;

    public UIInput input;

    public UITextList textList;

    private float time;

    private float maxTime;

    private EventSource sse;

    private JsonObject json = new JsonObject();

    private bool isOpen;

    public void Open()
	{
		this.textList.Clear();
		if (this.sse == null)
		{
			new AccountData();
			this.sse = new EventSource(new Uri("https://db-default-rtdb.firebaseio.com/Clans/" + AccountManager.GetClan() + "/c.json"));
			this.sse.Open();
			this.sse.OnOpen += delegate(EventSource eventSource)
			{
				this.isOpen = true;
			};
			this.sse.OnError += delegate(EventSource eventSource, string error)
			{
				UIToast.Show("Global Chat Error: " + error);
			};
			this.sse.OnClosed += delegate(EventSource eventSource)
			{
				this.isOpen = false;
			};
			this.sse.OnMessage += this.OnMessage;
			this.sse.OnRetry += this.OnRetry;
			return;
		}
		this.sse.Open();
	}

	private void OnMessage(EventSource source, Message message)
	{
		if (string.IsNullOrEmpty(message.Data))
		{
			return;
		}
		if (message.Data == "null")
		{
			return;
		}
		if (!JsonObject.isJson(message.Data))
		{
			return;
		}
		this.json = JsonObject.Parse(message.Data);
		JsonObject jsonObject = this.json.Get<JsonObject>("data");
		if (jsonObject.Length == 0)
		{
			return;
		}
		if (jsonObject.ContainsKey("m") && jsonObject.ContainsKey("n") && jsonObject.ContainsKey("t"))
		{
			string text = string.Concat(new string[]
			{
				jsonObject.Get<string>("n"),
				" (",
				this.ConvertTime(jsonObject.Get<long>("t")),
				"): ",
				jsonObject.Get<string>("m")
			});
			this.textList.Add(text);
		}
		else
		{
			for (int i = 0; i < jsonObject.Length; i++)
			{
				JsonObject jsonObject2 = jsonObject.Get<JsonObject>(jsonObject.GetKey(i));
				string text2 = string.Concat(new string[]
				{
					jsonObject2.Get<string>("n"),
					" (",
					this.ConvertTime(jsonObject2.Get<long>("t")),
					"): ",
					jsonObject2.Get<string>("m")
				});
				this.textList.Add(text2);
			}
		}
	}

	public void Close()
	{
		if (this.sse != null)
		{
			this.sse.Close();
			HTTPManager.AbortAll();
            sse = null;
        }
	}

	private bool OnRetry(EventSource source)
	{
		return true;
	}

	public void OnSubmit()
	{
		if (!this.isOpen)
		{
			return;
		}
		if (UIInput.current != this.input)
		{
			return;
		}
		if (this.time + this.maxTime > Time.time)
		{
			this.textList.Add("Message sending limit " + StringCache.Get(Mathf.CeilToInt(this.time + this.maxTime - Time.time)) + " sec");
			return;
		}
		this.Add(this.input.value);
	}

	private void Add(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (this.isOnlySpace(text))
		{
			return;
		}
		if (text.Contains("\n"))
		{
			text = text.Replace("\n", string.Empty);
		}
		text = BadWordsManager.Check(text);
		if (this.time + this.maxTime + 2f > Time.time)
		{
			this.maxTime += 1f;
		}
		else
		{
			this.maxTime = 0f;
		}
		this.time = Time.time;
		text = NGUIText.StripSymbols(text);
		AccountManager.Clan.SendMessage(text, true);
		this.input.value = string.Empty;
	}

	private bool isOnlySpace(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] != ' ')
			{
				return false;
			}
		}
		return true;
	}

	private string ConvertTime(long time)
	{
		time += NTPManager.GetMilliSeconds(DateTime.Now) - NTPManager.GetMilliSeconds(DateTime.UtcNow);
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddMilliseconds((double)time);
		dateTime = dateTime.ToLocalTime();
		DateTime now = DateTime.Now;
		if (dateTime.Year != now.Year)
		{
			return string.Concat(new object[]
			{
				dateTime.Day.ToString("D2"),
				"/",
				dateTime.Month.ToString("D2"),
				"/",
				dateTime.Year,
				" ",
				dateTime.Hour.ToString("D2"),
				":",
				dateTime.Minute.ToString("D2"),
				":",
				dateTime.Second.ToString("D2")
			});
		}
		if (dateTime.Month != now.Month || dateTime.Day != now.Day)
		{
			return string.Concat(new string[]
			{
				dateTime.Day.ToString("D2"),
				"/",
				dateTime.Month.ToString("D2"),
				" ",
				dateTime.Hour.ToString("D2"),
				":",
				dateTime.Minute.ToString("D2"),
				":",
				dateTime.Second.ToString("D2")
			});
		}
		return string.Concat(new string[]
		{
			dateTime.Hour.ToString("D2"),
			":",
			dateTime.Minute.ToString("D2"),
			":",
			dateTime.Second.ToString("D2")
		});
	}
}
