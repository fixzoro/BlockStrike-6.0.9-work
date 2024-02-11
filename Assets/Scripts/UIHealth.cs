using System;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public UILabel label;

    public UISprite sprite;

    private Color normalColor = new Color(0f, 0f, 0f, 0.705f);

    private Color criticalColor = new Color(1f, 0f, 0f, 0.705f);

    private static UIHealth instance;

    private void Start()
	{
		UIHealth.instance = this;
	}

	public static void SetHealth(int health)
	{
		if (health == 0)
		{
			UIHealth.instance.label.text = " ";
			UIHealth.instance.sprite.cachedGameObject.SetActive(false);
			UIAmmo.SetAmmo(0, -1);
		}
		else
		{
			UIHealth.instance.sprite.cachedGameObject.SetActive(true);
			UIHealth.instance.label.text = "+" + StringCache.Get(health);
			if (health <= 25)
			{
				UIHealth.instance.sprite.color = UIHealth.instance.criticalColor;
			}
			else
			{
				UIHealth.instance.sprite.color = UIHealth.instance.normalColor;
			}
		}
	}
}
