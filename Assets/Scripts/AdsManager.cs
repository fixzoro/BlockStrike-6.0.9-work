using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static bool isShow;

    private static Action<int, string> rewardedVideoComplete;

    private static Action rewardedVideoFailed;

    private static Action rewardedVideoAborted;

    private static string timeTag = "adsTag";

    private static int finishRewardedVideo = -1;

    private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Init()
	{
		
	}

	public static void ShowAny()
	{
		AdsManager.ShowAny("default");
	}

	public static void ShowAny(string placement)
	{
		
	}

	public static void ShowInterstitial()
	{
		AdsManager.ShowInterstitial("default");
	}

	public static void ShowInterstitial(string placement)
	{
		
	}

	public static void ShowRewardedVideo(Action<int, string> complete, Action failed, Action aborted)
	{
		AdsManager.ShowRewardedVideo(complete, failed, aborted, "default");
	}

	public static void ShowRewardedVideo(Action<int, string> complete, Action failed, Action aborted, string placement)
	{
		
	}

	private static void RewardedVideoFinished(int isComplete)
	{
		AdsManager.RewardedVideoFinished(isComplete, 0, string.Empty);
	}

	private static void RewardedVideoFinished(int isComplete, int amount, string name)
	{
		if (AdsManager.finishRewardedVideo != 0)
		{
			AdsManager.finishRewardedVideo = isComplete;
		}
		if (!TimerManager.IsActive(AdsManager.timeTag))
		{
			TimerManager.In(AdsManager.timeTag, 0.2f, delegate()
			{
				int isComplete2 = isComplete;
				if (isComplete2 != 0)
				{
					if (isComplete2 != 1)
					{
						if (AdsManager.rewardedVideoFailed != null)
						{
							AdsManager.rewardedVideoFailed();
						}
					}
					else if (AdsManager.rewardedVideoAborted != null)
					{
						AdsManager.rewardedVideoAborted();
					}
				}
				else if (AdsManager.rewardedVideoComplete != null)
				{
					AdsManager.rewardedVideoComplete(amount, name);
				}
				AdsManager.rewardedVideoComplete = null;
				AdsManager.rewardedVideoFailed = null;
				AdsManager.rewardedVideoAborted = null;
			});
		}
	}

	private void Print(string text)
	{
		if (Settings.Console)
		{
			TimerManager.In(0.1f, delegate()
			{
				MonoBehaviour.print(text);
			});
		}
	}

	public void onRewardedVideoLoaded(bool isPrecache)
	{
		this.Print("Rewarded Video loaded");
	}

	public void onRewardedVideoFailedToLoad()
	{
		AdsManager.RewardedVideoFinished(1);
		this.Print("Rewarded Video failed");
	}

	public void onRewardedVideoShown()
	{
		this.Print("Rewarded Video shown");
	}

	public void onRewardedVideoClosed(bool isFinished)
	{
		this.Print("Rewarded Video closed");
		AdsManager.isShow = false;
		AdsManager.RewardedVideoFinished((!isFinished) ? 1 : 0);
	}

	public void onRewardedVideoFinished(double amount, string name)
	{
		AdsManager.RewardedVideoFinished(0);
		this.Print("Rewarded Video finished");
		AdsManager.isShow = false;
	}

	public void onRewardedVideoExpired()
	{
	}

	public void onInterstitialLoaded(bool isPrecache)
	{
		this.Print("Interstitial loaded");
	}

	public void onInterstitialFailedToLoad()
	{
		this.Print("Interstitial failed");
	}

	public void onInterstitialShown()
	{
		this.Print("Interstitial opened");
	}

	public void onInterstitialClosed()
	{
		this.Print("Interstitial closed");
		AdsManager.isShow = false;
	}

	public void onInterstitialClicked()
	{
		this.Print("Interstitial clicked");
		AdsManager.isShow = false;
	}

	public void onInterstitialExpired()
	{

	}
}
