using System;
using UnityEngine;

// Token: 0x02000060 RID: 96
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060002A6 RID: 678 RVA: 0x00028A84 File Offset: 0x00026C84
	// (set) Token: 0x060002A7 RID: 679 RVA: 0x00028AAC File Offset: 0x00026CAC
	public bool isEnabled
	{
		get
		{
			Collider collider = base.GetComponent<Collider>();
			return collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if (!collider)
			{
				return;
			}
			if (collider.enabled != value)
			{
				collider.enabled = value;
				this.UpdateImage();
			}
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00006BE3 File Offset: 0x00004DE3
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<UISprite>();
		}
		this.UpdateImage();
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00028AE8 File Offset: 0x00026CE8
	private void OnValidate()
	{
		if (this.target != null)
		{
			if (string.IsNullOrEmpty(this.normalSprite))
			{
				this.normalSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.hoverSprite))
			{
				this.hoverSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.pressedSprite))
			{
				this.pressedSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.disabledSprite))
			{
				this.disabledSprite = this.target.spriteName;
			}
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00028B8C File Offset: 0x00026D8C
	private void UpdateImage()
	{
		if (this.target != null)
		{
			if (this.isEnabled)
			{
				this.SetSprite((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
			}
			else
			{
				this.SetSprite(this.disabledSprite);
			}
		}
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00006C08 File Offset: 0x00004E08
	private void OnHover(bool isOver)
	{
		if (this.isEnabled && this.target != null)
		{
			this.SetSprite((!isOver) ? this.normalSprite : this.hoverSprite);
		}
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00006C43 File Offset: 0x00004E43
	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			this.SetSprite(this.pressedSprite);
		}
		else
		{
			this.UpdateImage();
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00028BF0 File Offset: 0x00026DF0
	private void SetSprite(string sprite)
	{
		if (string.IsNullOrEmpty(sprite))
		{
			return;
		}
		INGUIAtlas atlas = this.target.atlas;
		if (atlas == null)
		{
			return;
		}
		INGUIAtlas inguiatlas = atlas;
		if (inguiatlas == null || inguiatlas.GetSprite(sprite) == null)
		{
			return;
		}
		this.target.spriteName = sprite;
		if (this.pixelSnap)
		{
			this.target.MakePixelPerfect();
		}
	}

	// Token: 0x040001C0 RID: 448
	public UISprite target;

	// Token: 0x040001C1 RID: 449
	public string normalSprite;

	// Token: 0x040001C2 RID: 450
	public string hoverSprite;

	// Token: 0x040001C3 RID: 451
	public string pressedSprite;

	// Token: 0x040001C4 RID: 452
	public string disabledSprite;

	// Token: 0x040001C5 RID: 453
	public bool pixelSnap = true;
}
