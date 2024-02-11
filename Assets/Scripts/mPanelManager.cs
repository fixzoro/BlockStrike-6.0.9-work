using System.Collections.Generic;
using UnityEngine;

public class mPanelManager : MonoBehaviour
{
    public List<UIPanel> panels = new List<UIPanel>();

    public UIPanel playerData;

    public UIPanel ignoreClick;

    public bool tween;

    public float duration = 0.2f;

    public static float tweenDuration = 0.2f;

    private UIPanel selectPanel;

    private UIPanel lastPanel;

    private static mPanelManager instance;

    public static UIPanel last
	{
		get
		{
			return mPanelManager.instance.lastPanel;
		}
	}

	public static UIPanel select
	{
		get
		{
			return mPanelManager.instance.selectPanel;
		}
	}

	public static bool isPlayerData
	{
		get
		{
			if (mPanelManager.instance.tween)
			{
				return mPanelManager.instance.playerData.alpha == 1f;
			}
			return mPanelManager.instance.playerData.cachedGameObject.activeSelf;
		}
		set
		{
			if (mPanelManager.instance.tween)
			{
				TweenAlpha.Begin(mPanelManager.instance.playerData.cachedGameObject, mPanelManager.tweenDuration, (float)((!value) ? 0 : 1), 0f);
			}
			else
			{
				mPanelManager.instance.playerData.cachedGameObject.SetActive(value);
			}
		}
	}

	private void Awake()
	{
		mPanelManager.instance = this;
		this.selectPanel = this.panels[0];
		mPanelManager.tweenDuration = this.duration;
	}

	public static void Show(string name, bool playerData)
	{
		if (mPanelManager.select != null && mPanelManager.select.name == name)
		{
			return;
		}
		mPanelManager.ShowIgnoreClick(mPanelManager.tweenDuration);
		mPanelManager.isPlayerData = playerData;
		if (mPanelManager.select != null)
		{
			for (int i = 0; i < mPanelManager.instance.panels.Count; i++)
			{
				if (mPanelManager.instance.tween && mPanelManager.select == mPanelManager.instance.panels[i])
				{
					UIPanel uipanel = mPanelManager.instance.panels[i];
					TweenAlpha.Begin(uipanel.cachedGameObject, mPanelManager.tweenDuration, 0f, 0f);
				}
				else
				{
					if (mPanelManager.instance.tween)
					{
						mPanelManager.instance.panels[i].alpha = 0f;
					}
					mPanelManager.instance.panels[i].cachedGameObject.SetActive(false);
				}
			}
		}
		for (int j = 0; j < mPanelManager.instance.panels.Count; j++)
		{
			if (mPanelManager.instance.panels[j].name == name)
			{
				if (mPanelManager.instance.selectPanel != null)
				{
					mPanelManager.instance.lastPanel = mPanelManager.instance.selectPanel;
				}
				mPanelManager.instance.selectPanel = mPanelManager.instance.panels[j];
				if (mPanelManager.instance.tween)
				{
					mPanelManager.instance.panels[j].alpha = 0f;
					TweenAlpha.Begin(mPanelManager.instance.panels[j].cachedGameObject, mPanelManager.tweenDuration, 1f, 0f);
				}
				else
				{
					mPanelManager.instance.panels[j].alpha = 1f;
				}
				mPanelManager.instance.panels[j].cachedGameObject.SetActive(true);
			}
		}
	}

	public void Show(string panel)
	{
		mPanelManager.Show(panel, true);
	}

	public void Show(GameObject panel)
	{
		mPanelManager.Show(panel.name, true);
	}

	public static void ShowIgnoreClick(float duration)
	{
		if (!mPanelManager.instance.tween)
		{
			return;
		}
		mPanelManager.instance.ignoreClick.cachedGameObject.SetActive(true);
		TimerManager.In(duration, delegate()
		{
			mPanelManager.instance.ignoreClick.cachedGameObject.SetActive(false);
		});
	}

	public static void ShowTween(GameObject go)
	{
		if (mPanelManager.instance.tween)
		{
			go.GetComponent<UIPanel>().alpha = 0f;
			TweenAlpha.Begin(go, mPanelManager.tweenDuration, 1f, 0f);
		}
		else
		{
			go.GetComponent<UIPanel>().alpha = 1f;
		}
		go.SetActive(true);
	}

	public static void HideTween(GameObject go)
	{
		if (mPanelManager.instance.tween)
		{
			go.GetComponent<UIPanel>().alpha = 1f;
			TweenAlpha.Begin(go, mPanelManager.tweenDuration, 0f, 0f);
		}
		else
		{
			go.SetActive(false);
		}
	}

	public void ShowAnim(GameObject go)
	{
		mPanelManager.ShowTween(go);
	}

	public void HideAnim(GameObject go)
	{
		mPanelManager.HideTween(go);
	}

	public static void Hide()
	{
		for (int i = 0; i < mPanelManager.instance.panels.Count; i++)
		{
			if (mPanelManager.instance.selectPanel != null && mPanelManager.instance.selectPanel == mPanelManager.instance.panels[i])
			{
				UIPanel panel = mPanelManager.instance.panels[i];
				if (mPanelManager.instance.tween)
				{
					mPanelManager.HideTween(panel.cachedGameObject);
					TimerManager.In(mPanelManager.tweenDuration, delegate()
					{
						panel.alpha = 0f;
						panel.cachedGameObject.SetActive(false);
					});
				}
				else
				{
					panel.cachedGameObject.SetActive(false);
				}
			}
			else
			{
				if (mPanelManager.instance.tween)
				{
					mPanelManager.instance.panels[i].alpha = 0f;
				}
				mPanelManager.instance.panels[i].cachedGameObject.SetActive(false);
			}
		}
		mPanelManager.instance.selectPanel = null;
		mPanelManager.isPlayerData = false;
	}

	public void SetPlayerData(bool active)
	{
		mPanelManager.ShowIgnoreClick(mPanelManager.tweenDuration);
		if (this.tween)
		{
			TweenAlpha.Begin(this.playerData.cachedGameObject, mPanelManager.tweenDuration, (float)((!active) ? 0 : 1), 0f);
		}
		else
		{
			this.playerData.cachedGameObject.SetActive(active);
		}
	}
}
