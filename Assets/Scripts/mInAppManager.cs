using System;
using UnityEngine;

public class mInAppManager : MonoBehaviour
{
    private GameCurrency Currency;

    private bool isRewardedVideo;

    public void OnRewardedVideo(int currency)
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("By DRRIXS", true));
			return;
		}
		if (this.isRewardedVideo)
		{
			return;
		}
		this.Currency = (GameCurrency)currency;
		if (this.Currency == GameCurrency.Gold && AccountManager.GetGold() >= 8000)
		{
			return;
		}
		UIToast.Show(Localization.Get("Please wait", true) + "...");
		this.isRewardedVideo = true;
		TimerManager.In(0.5f, delegate()
		{
			AdsManager.ShowRewardedVideo(new Action<int, string>(this.RewardedVideoComplete), new Action(this.RewardedVideoFailed), new Action(this.RewardedVideoAborted), (this.Currency != GameCurrency.Gold) ? "RewardedMoney" : "RewardedGold");
		});
	}

	private void RewardedVideoComplete(int amount, string name)
	{
		TimerManager.In(0.3f, delegate()
		{
			AccountManager.Rewarded(this.Currency, delegate
			{
				if (this.Currency == GameCurrency.Money)
				{
					UIToast.Show("+50 BS Silver");
				}
				else
				{
					UIToast.Show("+1 BS Coins");
				}
				EventManager.Dispatch("AccountUpdate");
				this.isRewardedVideo = false;
			}, delegate(string e)
			{
				UIToast.Show(e);
				this.isRewardedVideo = false;
			});
		});
	}

	private void RewardedVideoAborted()
	{
		UIToast.Show(Localization.Get("Cancel", true));
		this.isRewardedVideo = false;
	}

	private void RewardedVideoFailed()
	{
		this.isRewardedVideo = false;
		UIToast.Show(Localization.Get("Video not available", true), 3f);
	}
}
