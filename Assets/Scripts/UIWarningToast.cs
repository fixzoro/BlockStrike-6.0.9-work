using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UIWarningToast : MonoBehaviour
{
    public UIWidget widget;

    public UILabel label;

    public UISprite background;

    private List<string> textList = new List<string>();

    private List<float> timeList = new List<float>();

    private bool isShow;

    private Tweener tween;

    private static UIWarningToast instance;

    private void Awake()
	{
		UIWarningToast.instance = this;
	}

	public static void Show(string text)
	{
		UIWarningToast.Show(text, 2f, false);
	}

	public static void Show(string text, float duration)
	{
		UIWarningToast.Show(text, duration, false);
	}

	public static void Show(string text, float duration, bool queue)
	{
		if (UIWarningToast.instance == null)
		{
			return;
		}
		if (queue && UIWarningToast.instance.isShow)
		{
			UIWarningToast.instance.textList.Add(text);
			UIWarningToast.instance.timeList.Add(duration);
			return;
		}
		TimerManager.Cancel("WarningToast");
		UIWarningToast.instance.widget.alpha = 0f;
		if (UIWarningToast.instance.tween == null)
		{
			UIWarningToast.instance.tween = DOTween.To(() => UIWarningToast.instance.widget.alpha, delegate(float x)
			{
				UIWarningToast.instance.widget.alpha = x;
			}, 1f, 0.2f).SetAutoKill(false).OnUpdate(new TweenCallback(UIWarningToast.instance.OnUpdate));
		}
		else
		{
			UIWarningToast.instance.tween.ChangeStartValue(0f, -1f).ChangeEndValue(1f, -1f, false).Restart(true, -1f);
		}
		UIWarningToast.instance.label.text = text;
		UIWarningToast.instance.isShow = true;
		UIWarningToast.instance.background.UpdateAnchors();
		if (UIWarningToast.instance.background.width <= 120)
		{
			UIWarningToast.instance.background.width = 120;
		}
		TimerManager.In("WarningToast", duration, delegate()
		{
			if (UIWarningToast.instance.textList.Count != 0)
			{
				UIWarningToast.Show(UIWarningToast.instance.textList[0], UIWarningToast.instance.timeList[0]);
				UIWarningToast.instance.textList.RemoveAt(0);
				UIWarningToast.instance.timeList.RemoveAt(0);
			}
			else
			{
				UIWarningToast.instance.isShow = false;
				UIWarningToast.instance.tween.ChangeStartValue(1f, -1f).ChangeEndValue(0f, -1f, false).Restart(true, -1f);
			}
		});
	}

	private void OnUpdate()
	{
		this.widget.UpdateWidget();
	}
}
