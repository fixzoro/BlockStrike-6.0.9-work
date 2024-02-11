using System;
using UnityEngine;

public class UIScore2 : MonoBehaviour
{
    public GameObject score;

    public UILabel maxScoreLabel;

    public UILabel blueScoreLabel;

    public UILabel redScoreLabel;

    public UILabel bluePlayersLabel;

    public UILabel redPlayersLabel;

    public static UIScore2.TimeData timeData = new UIScore2.TimeData();

    private static UIScore2 instance;

    private void Awake()
	{
		UIScore2.instance = this;
	}

	private void OnDisable()
	{
		ControllerManager.SetDeadEvent -= this.OnUpdatePlayers;
		UIScore2.timeData = new UIScore2.TimeData();
	}

	private void OnUpdatePlayers()
	{
		this.OnUpdatePlayers(0, false);
	}

	private void OnUpdatePlayers(DamageInfo damageInfo)
	{
		this.OnUpdatePlayers(0, false);
	}

	private void OnUpdatePlayers(int player, bool dead)
	{
		int num = 0;
		int num2 = 0;
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				num2++;
			}
			else if (playerList[i].GetTeam() == Team.Red && !playerList[i].GetDead())
			{
				num++;
			}
		}
		this.bluePlayersLabel.text = StringCache.Get(num2);
		this.redPlayersLabel.text = StringCache.Get(num);
	}

	public static void SetActiveScore(bool active)
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(UIScore2.instance.OnUpdatePlayers));
		ControllerManager.SetDeadEvent += UIScore2.instance.OnUpdatePlayers;
		UIScore2.instance.score.SetActive(active);
		UIScore2.instance.OnUpdatePlayers();
	}

	public static void SetActiveScore(bool active, int maxScore)
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(UIScore2.instance.OnUpdatePlayers));
		ControllerManager.SetDeadEvent += UIScore2.instance.OnUpdatePlayers;
		UIScore2.instance.score.SetActive(active);
		if (maxScore <= nValue.int0)
		{
			UIScore2.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore2.instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
		UIScore2.instance.OnUpdatePlayers();
	}

	public static void UpdateScore()
	{
		if (GameManager.maxScore <= nValue.int0)
		{
			UIScore2.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore2.instance.maxScoreLabel.text = StringCache.Get(GameManager.maxScore);
		}
		UIScore2.instance.blueScoreLabel.text = StringCache.Get(GameManager.blueScore);
		UIScore2.instance.redScoreLabel.text = StringCache.Get(GameManager.redScore);
		UIScore2.instance.OnUpdatePlayers();
	}

	public static void UpdateScore(int maxScore, int blueScore, int redScore)
	{
		if (maxScore <= nValue.int0)
		{
			UIScore2.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore2.instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
		UIScore2.instance.blueScoreLabel.text = StringCache.Get(blueScore);
		UIScore2.instance.redScoreLabel.text = StringCache.Get(redScore);
		UIScore2.instance.OnUpdatePlayers();
	}

	public static void StartTime(float time, Action callback)
	{
		UIScore2.StartTime(time, true, callback);
	}

	public static void StartTime(float time, bool show, Action callback)
	{
		UIScore2.timeData.active = true;
		TimerManager.In("UIScoreTimer", 0.25f, -1, 0.25f, new TimerManager.Callback(UIScore2.instance.UpdateTimer));
		UIScore2.timeData.show = show;
		UIScore2.timeData.endTime = time + Time.time;
		UIScore2.timeData.callback = callback;
	}

	public static void StopTime(bool callback)
	{
		TimerManager.Cancel("UIScoreTimer");
		UIScore2.timeData.active = false;
		if (callback && UIScore2.timeData.callback != null)
		{
			UIScore2.timeData.callback();
		}
	}

	private void UpdateTimer()
	{
		if (UIScore2.timeData.active)
		{
			if (UIScore2.timeData.show)
			{
				this.maxScoreLabel.text = StringCache.GetTime(UIScore2.timeData.endTime - Time.time);
			}
			if (UIScore2.timeData.endTime <= Time.time)
			{
				TimerManager.Cancel("UIScoreTimer");
				UIScore2.timeData.active = false;
				if (UIScore2.timeData.callback != null)
				{
					UIScore2.timeData.callback();
				}
			}
		}
		else
		{
			TimerManager.Cancel("UIScoreTimer");
		}
	}

	public class TimeData
	{
		public bool active;

		public bool show;

		public float endTime;

		public Action callback;
	}
}
