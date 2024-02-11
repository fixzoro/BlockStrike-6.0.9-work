using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class UIShowControlScheme : MonoBehaviour
{
	// Token: 0x06000398 RID: 920 RVA: 0x00007683 File Offset: 0x00005883
	private void OnEnable()
	{
		UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Combine(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
		this.OnScheme();
	}

	// Token: 0x06000399 RID: 921 RVA: 0x000076AB File Offset: 0x000058AB
	private void OnDisable()
	{
		UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Remove(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0002F0F0 File Offset: 0x0002D2F0
	private void OnScheme()
	{
		if (this.target != null)
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			if (currentScheme == UICamera.ControlScheme.Mouse)
			{
				this.target.SetActive(this.mouse);
			}
			else if (currentScheme == UICamera.ControlScheme.Touch)
			{
				this.target.SetActive(this.touch);
			}
			else if (currentScheme == UICamera.ControlScheme.Controller)
			{
				this.target.SetActive(this.controller);
			}
		}
	}

	// Token: 0x040002B2 RID: 690
	public GameObject target;

	// Token: 0x040002B3 RID: 691
	public bool mouse;

	// Token: 0x040002B4 RID: 692
	public bool touch;

	// Token: 0x040002B5 RID: 693
	public bool controller = true;
}
