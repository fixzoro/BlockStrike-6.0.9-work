using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    public UISprite fadeSprite;

    public Tweener tween;

    private static UIFade instance;

    private void Start()
	{
		UIFade.instance = this;
	}

	public static void Fade(float from, float to, float duration)
	{
		UIFade.Fade(from, to, duration, null);
	}

	public static void Fade(float from, float to, float duration, TweenCallback finish)
	{
		if (UIFade.instance.tween == null)
		{
			UIFade.instance.tween = DOTween.To(() => UIFade.instance.fadeSprite.alpha, delegate(float x)
			{
				UIFade.instance.fadeSprite.alpha = x;
			}, to, duration).SetAutoKill(false).OnUpdate(new TweenCallback(UIFade.instance.OnUpdate));
			UIFade.instance.tween.onComplete = finish;
		}
		else
		{
			UIFade.instance.tween.ChangeValues(from, to, duration).Restart(true, -1f);
			UIFade.instance.tween.onComplete = finish;
		}
	}

	private void OnUpdate()
	{
		this.fadeSprite.UpdateWidget();
	}
}
