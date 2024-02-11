using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public List<GameObject> list = new List<GameObject>();

    private static UIPanelManager instance;

    private void Awake()
	{
		UIPanelManager.instance = this;
	}

	public static void ShowPanel(string panel)
	{
		for (int i = 0; i < UIPanelManager.instance.list.Count; i++)
		{
			if (UIPanelManager.instance.list[i].name == panel)
			{
				UIPanelManager.instance.list[i].SetActive(true);
			}
			else
			{
				UIPanelManager.instance.list[i].SetActive(false);
			}
		}
	}

	public void Show(string panel)
	{
		UIPanelManager.ShowPanel(panel);
	}

	public static GameObject Get(string panel)
	{
		for (int i = 0; i < UIPanelManager.instance.list.Count; i++)
		{
			if (UIPanelManager.instance.list[i].name == panel)
			{
				return UIPanelManager.instance.list[i];
			}
		}
		return null;
	}

	public static void HideAll()
	{
		for (int i = 0; i < UIPanelManager.instance.list.Count; i++)
		{
			UIPanelManager.instance.list[i].SetActive(false);
		}
	}
}
