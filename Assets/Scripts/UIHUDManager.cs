using System;
using UnityEngine;

public class UIHUDManager : MonoBehaviour
{
    public UIWidget[] list;

    private void Start()
	{
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
	}

	private void OnSettings()
	{
		TimerManager.In(0.1f, delegate()
		{
			for (int i = 0; i < this.list.Length; i++)
			{
				this.list[i].isCalculateFinalAlpha = !Settings.HUD;
				this.list[i].UpdateWidget();
			}
		});
	}
}
