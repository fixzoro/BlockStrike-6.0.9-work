using System;
using DG.Tweening;
using UnityEngine;

public class UIDuration : MonoBehaviour
{
    public UISprite sprite;

    public UILabel label;

    private Tweener tween;

    private static UIDuration instance;

    public static UISprite duration
	{
		get
		{
			return UIDuration.instance.sprite;
		}
	}

	private void Start()
	{
		UIDuration.instance = this;
	}

	public static void StartDuration(float duration)
	{
		UIDuration.StartDuration(duration, false, null);
	}

	public static void StartDuration(float duration, bool time)
	{
		UIDuration.StartDuration(duration, time, null);
	}

	public static void StartDuration(float duration, bool time, TweenCallback callback)
	{
		UIDuration.StopDuration();
		UIDuration.instance.sprite.cachedGameObject.SetActive(true);
		if (callback != null)
		{
			UIDuration.instance.tween = DOTween.To(() => UIDuration.instance.sprite.width, delegate(int x)
			{
				UIDuration.instance.sprite.width = x;
			}, 155, duration).SetEase(Ease.Linear).OnComplete(callback);
			if (time)
			{
				UIDuration.instance.tween.OnUpdate(new TweenCallback(UIDuration.instance.UpdateDuration));
			}
		}
		else
		{
			UIDuration.instance.tween = DOTween.To(() => UIDuration.instance.sprite.width, delegate(int x)
			{
				UIDuration.instance.sprite.width = x;
			}, 155, duration).SetEase(Ease.Linear);
			if (time)
			{
				UIDuration.instance.tween.OnUpdate(new TweenCallback(UIDuration.instance.UpdateDuration));
			}
		}
	}

	private void UpdateDuration()
	{
		this.label.text = (this.tween.Duration(true) - this.tween.fullPosition).ToString("00:00");
	}

	public static void StopDuration()
	{
		if (UIDuration.instance.tween != null && UIDuration.instance.tween.IsActive())
		{
			UIDuration.instance.tween.Kill(false);
		}
		UIDuration.instance.sprite.cachedGameObject.SetActive(false);
		UIDuration.instance.sprite.width = 0;
		UIDuration.instance.label.text = string.Empty;
	}
}
