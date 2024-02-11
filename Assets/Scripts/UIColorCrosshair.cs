using System;
using UnityEngine;

public class UIColorCrosshair : MonoBehaviour
{
    private int SelectColor;

    private UISprite mSprite;

    private void Start()
	{
		this.mSprite = base.GetComponent<UISprite>();
		this.SelectColor = Settings.ColorCrosshair;
		Color color = Utils.GetColor(this.SelectColor);
		if (color == Color.clear)
		{
			color = new Color(1f, 1f, 1f, 0.5f);
		}
		this.mSprite.color = color;
	}

	private void OnClick()
	{
		this.SelectColor++;
		if (9 < this.SelectColor)
		{
			this.SelectColor = 0;
		}
		Color color = Utils.GetColor(this.SelectColor);
		if (color.a == 0f)
		{
			color = new Color(1f, 1f, 1f, 0.05f);
		}
		this.mSprite.color = color;
		Settings.ColorCrosshair = this.SelectColor;
	}
}
