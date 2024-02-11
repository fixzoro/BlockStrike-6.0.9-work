using System;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    public GameObject score;

    public UILabel maxScoreLabel;

    public UILabel blueScoreLabel;

    public UILabel redScoreLabel;

    public static UIScore.TimeData timeData = new UIScore.TimeData();

    private static UIScore instance;

    private void Awake()
	{
		UIScore.instance = this;
	}

	private void OnDisable()
	{
		UIScore.timeData = new UIScore.TimeData();
	}

	public static void SetActiveScore(bool active)
	{
		UIScore.instance.score.SetActive(active);
	}

	public static void SetActiveScore(bool active, int maxScore)
	{
		UIScore.instance.score.SetActive(active);
		if (maxScore <= nValue.int0)
		{
			UIScore.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore.instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
	}

	public static void UpdateScore()
	{
		if (GameManager.maxScore <= nValue.int0)
		{
			UIScore.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore.instance.maxScoreLabel.text = StringCache.Get(GameManager.maxScore);
		}
		UIScore.instance.blueScoreLabel.text = StringCache.Get(GameManager.blueScore);
		UIScore.instance.redScoreLabel.text = StringCache.Get(GameManager.redScore);
	}

	public static void UpdateScore(int maxScore, int blueScore, int redScore)
	{
		if (maxScore <= nValue.int0)
		{
			UIScore.instance.maxScoreLabel.text = "-";
		}
		else
		{
			UIScore.instance.maxScoreLabel.text = StringCache.Get(maxScore);
		}
		UIScore.instance.blueScoreLabel.text = StringCache.Get(blueScore);
		UIScore.instance.redScoreLabel.text = StringCache.Get(redScore);
	}

	public static void StartTime(float time, Action callback)
	{
		UIScore.StartTime(time, true, callback);
	}

	public static void StartTime(float time, bool show, Action callback)
	{
		UIScore.timeData.active = true;
		TimerManager.In("UIScoreTimer", 0.25f, -1, 0.25f, new TimerManager.Callback(UIScore.instance.UpdateTimer));
		UIScore.timeData.show = show;
		UIScore.timeData.endTime = time + Time.time;
		UIScore.timeData.callback = callback;
	}

	public static void StopTime(bool callback)
	{
		TimerManager.Cancel("UIScoreTimer");
		UIScore.timeData.active = false;
		if (callback && UIScore.timeData.callback != null)
		{
			UIScore.timeData.callback();
		}
	}

	private void UpdateTimer()
	{
		if (UIScore.timeData.active)
		{
			if (UIScore.timeData.show)
			{
				this.maxScoreLabel.text = StringCache.GetTime(UIScore.timeData.endTime - Time.time);
			}
			if (UIScore.timeData.endTime <= Time.time)
			{
				TimerManager.Cancel("UIScoreTimer");
				UIScore.timeData.active = false;
				if (UIScore.timeData.callback != null)
				{
					UIScore.timeData.callback();
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
