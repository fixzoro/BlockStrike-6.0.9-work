using System;
using UnityEngine;

public class UIAmmo : MonoBehaviour
{
    public UILabel label;

    public UISprite sprite;

    private Color normalColor = new Color(0f, 0f, 0f, 0.705f);

    private Color criticalColor = new Color(1f, 0f, 0f, 0.705f);

    private static UIAmmo instance;

    private void Start()
	{
		UIAmmo.instance = this;
	}

	public static void SetAmmo(int ammo, int ammoMax)
	{
		UIAmmo.SetAmmo(ammo, ammoMax, false, -1);
	}

	public static void SetAmmo(int ammo, int ammoMax, bool infinity, int warning)
	{
		nProfiler.BeginSample("SetAmmo");
		if (ammoMax == -1)
		{
			UIAmmo.instance.label.text = " ";
			UIAmmo.instance.sprite.cachedGameObject.SetActive(false);
		}
		else
		{
			if (!UIAmmo.instance.sprite.cachedGameObject.activeSelf)
			{
				UIAmmo.instance.sprite.cachedGameObject.SetActive(true);
			}
			UIAmmo.instance.sprite.color = (((ammo > warning || ammoMax == 0) && ammoMax != 0) ? UIAmmo.instance.normalColor : UIAmmo.instance.criticalColor);
			if (infinity)
			{
				UIAmmo.instance.label.text = StringCache.Get(ammo) + "/∞";
			}
			else
			{
				UIAmmo.instance.label.text = StringCache.Get(ammo) + "/" + StringCache.Get(ammoMax);
			}
		}
		nProfiler.EndSample();
	}
}
