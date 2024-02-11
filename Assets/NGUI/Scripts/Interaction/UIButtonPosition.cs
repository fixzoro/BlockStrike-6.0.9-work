using System;
using UnityEngine;

// Token: 0x0200041D RID: 1053
public class UIButtonPosition : MonoBehaviour
{
	// Token: 0x0600250C RID: 9484 RVA: 0x000DDEEC File Offset: 0x000DC0EC
	private void Start()
	{
		this.sprite = base.GetComponent<UISprite>();
		this.defaultPosition = this.sprite.transform.localPosition;
		this.defaultSize = new Vector2((float)this.sprite.width, (float)this.sprite.height);
		EventManager.AddListener("OnDefaultButton", new EventManager.Callback(this.OnDefaultButton));
		EventManager.AddListener("OnSaveButton", new EventManager.Callback(this.OnSaveButton));
		this.OnPosition();
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x0001A087 File Offset: 0x00018287
	private void OnPosition()
	{
		TimerManager.In(0.1f, delegate()
		{
			if (nPlayerPrefs.HasKey("Button_Pos_" + this.button))
			{
				this.sprite.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Button_Pos_" + this.button);
				if (this.changeSize && nPlayerPrefs.HasKey("Button_Size_" + this.button))
				{
					Vector2 vector = nPlayerPrefs.GetVector2("Button_Size_" + this.button);
					if (vector != Vector2.zero)
					{
						this.sprite.width = (int)vector.x;
						this.sprite.height = (int)vector.y;
					}
				}
			}
		});
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000DDF74 File Offset: 0x000DC174
	private void OnEnable()
	{
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000DDFC4 File Offset: 0x000DC1C4
	private void OnDisable()
	{
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onDoubleClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onDoubleClick, new UICamera.VoidDelegate(this.OnDoubleClick));
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000DE014 File Offset: 0x000DC214
	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (this.sprite.cachedGameObject != go)
		{
			return;
		}
		this.sprite.cachedTransform.localPosition += new Vector3(delta.x, delta.y, 0f);
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000DE06C File Offset: 0x000DC26C
	private void OnDoubleClick(GameObject go)
	{
		if (this.sprite.cachedGameObject != go)
		{
			return;
		}
		if (!this.changeSize)
		{
			return;
		}
		this.sprite.width += 5;
		this.sprite.height += 5;
		if ((float)this.sprite.width > this.size.y)
		{
			this.sprite.width = (int)this.size.x;
			this.sprite.height = (int)this.size.x;
		}
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000DE10C File Offset: 0x000DC30C
	private void OnDefaultButton()
	{
		this.sprite.transform.localPosition = this.defaultPosition;
		if (!this.changeSize)
		{
			return;
		}
		this.sprite.width = (int)this.defaultSize.x;
		this.sprite.height = (int)this.defaultSize.y;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000DE16C File Offset: 0x000DC36C
	private void OnSaveButton()
	{
		nPlayerPrefs.SetVector3("Button_Pos_" + this.button, this.sprite.cachedTransform.localPosition);
		if (this.changeSize)
		{
			nPlayerPrefs.SetVector2("Button_Size_" + this.button, new Vector2((float)this.sprite.width, (float)this.sprite.height));
		}
	}

	// Token: 0x04001624 RID: 5668
	public string button;

	// Token: 0x04001625 RID: 5669
	public bool changeSize = true;

	// Token: 0x04001626 RID: 5670
	private Vector2 size = new Vector2(35f, 150f);

	// Token: 0x04001627 RID: 5671
	private Vector3 defaultPosition;

	// Token: 0x04001628 RID: 5672
	private Vector2 defaultSize;

	// Token: 0x04001629 RID: 5673
	private UISprite sprite;
}
