using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000433 RID: 1075
public class UIEventClick : MonoBehaviour
{
	// Token: 0x060025B6 RID: 9654 RVA: 0x0001A6C3 File Offset: 0x000188C3
	private void Start()
	{
		this.mGameObject = base.gameObject;
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x0001A6D1 File Offset: 0x000188D1
	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x0001A6F3 File Offset: 0x000188F3
	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	// Token: 0x060025B9 RID: 9657 RVA: 0x0001A715 File Offset: 0x00018915
	private void OnClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (this.onClick != null)
		{
			this.onClick.Invoke();
		}
	}

	// Token: 0x040016B9 RID: 5817
	public UnityEvent onClick;

	// Token: 0x040016BA RID: 5818
	private GameObject mGameObject;
}
