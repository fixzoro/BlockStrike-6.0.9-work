using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UIToast : MonoBehaviour
{
    public UILabel label;

    public UISprite background;

    private List<string> textList = new List<string>();

    private List<float> timeList = new List<float>();

    private bool isShow;

    private Tweener tween;

    private static UIToast instance;

    private void Awake()
	{
		UIToast.instance = this;
	}

	public static void Show(string text)
	{
		UIToast.Show(text, 2f, false);
	}

	public static void Show(string text, float duration)
	{
		UIToast.Show(text, duration, false);
	}

	public static void Show(string text, float duration, bool queue)
	{
		if (UIToast.instance == null)
		{
			return;
		}
		if (queue && UIToast.instance.isShow)
		{
			UIToast.instance.textList.Add(text);
			UIToast.instance.timeList.Add(duration);
			return;
		}
		TimerManager.Cancel("Toast");
		UIToast.instance.label.alpha = 0f;
		if (UIToast.instance.tween == null)
		{
			UIToast.instance.tween = DOTween.To(() => UIToast.instance.label.alpha, delegate(float x)
			{
				UIToast.instance.label.alpha = x;
			}, 1f, 0.2f).SetAutoKill(false).OnUpdate(new TweenCallback(UIToast.instance.OnUpdate));
		}
		else
		{
			UIToast.instance.tween.ChangeStartValue(0f, -1f).ChangeEndValue(1f, -1f, false).Restart(true, -1f);
		}
		UIToast.instance.label.text = text;
		UIToast.instance.isShow = true;
		UIToast.instance.background.UpdateAnchors();
		TimerManager.In("Toast", duration, delegate()
		{
			if (UIToast.instance.textList.Count != 0)
			{
				UIToast.Show(UIToast.instance.textList[0], UIToast.instance.timeList[0]);
				UIToast.instance.textList.RemoveAt(0);
				UIToast.instance.timeList.RemoveAt(0);
			}
			else
			{
				UIToast.instance.isShow = false;
				UIToast.instance.tween.ChangeStartValue(1f, -1f).ChangeEndValue(0f, -1f, false).Restart(true, -1f);
			}
		});
	}

	private void OnUpdate()
	{
		this.label.UpdateWidget();
	}
}
