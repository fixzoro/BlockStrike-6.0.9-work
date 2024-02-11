using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPushToTalk : MonoBehaviour
{
    public UILabel Element;

    public UIGrid Grid;

    private List<UILabel> ElementPool = new List<UILabel>();

    private static UIPushToTalk instance;

    private void Start()
	{
		UIPushToTalk.instance = this;
	}

	public static void Add(string playerName, float duration)
	{
		UILabel label;
		if (UIPushToTalk.instance.ElementPool.Count == 0)
		{
			label = UIPushToTalk.instance.Grid.gameObject.AddChild(UIPushToTalk.instance.Element.gameObject).GetComponent<UILabel>();
		}
		else
		{
			label = UIPushToTalk.instance.ElementPool[0];
			UIPushToTalk.instance.ElementPool.RemoveAt(0);
		}
		label.cachedGameObject.SetActive(true);
		label.text = playerName;
		UIPushToTalk.instance.Grid.repositionNow = true;
		TimerManager.In(duration, delegate()
		{
			label.cachedGameObject.SetActive(false);
			UIPushToTalk.instance.Grid.repositionNow = true;
			UIPushToTalk.instance.ElementPool.Add(label);
		});
	}
}
