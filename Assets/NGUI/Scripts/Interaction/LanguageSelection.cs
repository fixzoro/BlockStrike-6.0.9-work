using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
[RequireComponent(typeof(UIPopupList))]
[AddComponentMenu("NGUI/Interaction/Language Selection")]
public class LanguageSelection : MonoBehaviour
{
	// Token: 0x060001CC RID: 460 RVA: 0x00005E3E File Offset: 0x0000403E
	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00005E4C File Offset: 0x0000404C
	private void Start()
	{
		this.mStarted = true;
		this.Refresh();
		EventDelegate.Add(this.mList.onChange, delegate()
		{
			Localization.language = UIPopupList.current.value;
		});
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00005E89 File Offset: 0x00004089
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.Refresh();
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00024490 File Offset: 0x00022690
	public void Refresh()
	{
		if (this.mList != null && Localization.knownLanguages != null)
		{
			this.mList.Clear();
			int i = 0;
			int num = Localization.knownLanguages.Length;
			while (i < num)
			{
				this.mList.items.Add(Localization.knownLanguages[i]);
				i++;
			}
			this.mList.value = Localization.language;
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00005E9C File Offset: 0x0000409C
	private void OnLocalize()
	{
		this.Refresh();
	}

	// Token: 0x040000DD RID: 221
	private UIPopupList mList;

	// Token: 0x040000DE RID: 222
	private bool mStarted;
}
