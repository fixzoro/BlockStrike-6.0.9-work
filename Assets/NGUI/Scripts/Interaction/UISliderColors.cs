using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
[AddComponentMenu("NGUI/Examples/Slider Colors")]
public class UISliderColors : MonoBehaviour
{
	// Token: 0x060001BE RID: 446 RVA: 0x00005D61 File Offset: 0x00003F61
	private void Start()
	{
		this.mBar = base.GetComponent<UIProgressBar>();
		this.mSprite = base.GetComponent<UIBasicSprite>();
		this.Update();
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0002414C File Offset: 0x0002234C
	private void Update()
	{
		if (this.sprite == null || this.colors.Length == 0)
		{
			return;
		}
		float num = (!(this.mBar != null)) ? this.mSprite.fillAmount : this.mBar.value;
		num *= (float)(this.colors.Length - 1);
		int num2 = Mathf.FloorToInt(num);
		Color color = this.colors[0];
		if (num2 >= 0)
		{
			if (num2 + 1 < this.colors.Length)
			{
				float t = num - (float)num2;
				color = Color.Lerp(this.colors[num2], this.colors[num2 + 1], t);
			}
			else if (num2 < this.colors.Length)
			{
				color = this.colors[num2];
			}
			else
			{
				color = this.colors[this.colors.Length - 1];
			}
		}
		color.a = this.sprite.color.a;
		this.sprite.color = color;
	}

	// Token: 0x040000C9 RID: 201
	public UISprite sprite;

	// Token: 0x040000CA RID: 202
	public Color[] colors = new Color[]
	{
		Color.red,
		Color.yellow,
		Color.green
	};

	// Token: 0x040000CB RID: 203
	private UIProgressBar mBar;

	// Token: 0x040000CC RID: 204
	private UIBasicSprite mSprite;
}
