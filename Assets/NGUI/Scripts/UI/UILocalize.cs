using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
[AddComponentMenu("NGUI/UI/Localize")]
[RequireComponent(typeof(UIWidget))]
[ExecuteInEditMode]
public class UILocalize : MonoBehaviour
{
	// Token: 0x17000195 RID: 405
	// (set) Token: 0x0600099E RID: 2462 RVA: 0x0000B1BC File Offset: 0x000093BC
	public string value
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (this.lbl == null)
				{
					this.lbl = base.GetComponent<UILabel>();
				}
				this.lbl.text = value;
			}
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x0000B1F2 File Offset: 0x000093F2
	private void OnEnable()
	{
		Localization.onLocalize = (Localization.OnLocalizeNotification)Delegate.Combine(Localization.onLocalize, new Localization.OnLocalizeNotification(this.OnLocalize));
		if (this.mStarted)
		{
			this.OnLocalize();
		}
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x0000B225 File Offset: 0x00009425
	private void OnDisable()
	{
		Localization.onLocalize = (Localization.OnLocalizeNotification)Delegate.Remove(Localization.onLocalize, new Localization.OnLocalizeNotification(this.OnLocalize));
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0000B247 File Offset: 0x00009447
	private void Start()
	{
		this.mStarted = true;
		this.OnLocalize();
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0005484C File Offset: 0x00052A4C
	private void OnLocalize()
	{
		if (string.IsNullOrEmpty(this.key))
		{
			UILabel component = base.GetComponent<UILabel>();
			if (component != null)
			{
				this.key = component.text;
			}
		}
		if (!string.IsNullOrEmpty(this.key))
		{
			this.value = Localization.Get(this.key, true) + this.addon;
		}
	}

	// Token: 0x040006A9 RID: 1705
	public string key;

	// Token: 0x040006AA RID: 1706
	public string addon;

	// Token: 0x040006AB RID: 1707
	private UILabel lbl;

	// Token: 0x040006AC RID: 1708
	private bool mStarted;
}
