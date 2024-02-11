using System;
using UnityEngine;

// Token: 0x02000074 RID: 116
[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000360 RID: 864 RVA: 0x0000746B File Offset: 0x0000566B
	private string key
	{
		get
		{
			return (!string.IsNullOrEmpty(this.keyName)) ? this.keyName : ("NGUI State: " + base.name);
		}
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00007498 File Offset: 0x00005698
	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.mCheck = base.GetComponent<UIToggle>();
		this.mSlider = base.GetComponent<UIProgressBar>();
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0002CF2C File Offset: 0x0002B12C
	private void OnEnable()
	{
		if (this.mList != null)
		{
			EventDelegate.Add(this.mList.onChange, new EventDelegate.Callback(this.SaveSelection));
			string @string = PlayerPrefs.GetString(this.key);
			if (!string.IsNullOrEmpty(@string))
			{
				this.mList.value = @string;
			}
		}
		else if (this.mCheck != null)
		{
			EventDelegate.Add(this.mCheck.onChange, new EventDelegate.Callback(this.SaveState));
			this.mCheck.value = (PlayerPrefs.GetInt(this.key, (!this.mCheck.startsActive) ? 0 : 1) != 0);
		}
		else if (this.mSlider != null)
		{
			EventDelegate.Add(this.mSlider.onChange, new EventDelegate.Callback(this.SaveProgress));
			this.mSlider.value = PlayerPrefs.GetFloat(this.key, this.mSlider.value);
		}
		else
		{
			string string2 = PlayerPrefs.GetString(this.key);
			UIToggle[] componentsInChildren = base.GetComponentsInChildren<UIToggle>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				UIToggle uitoggle = componentsInChildren[i];
				uitoggle.value = (uitoggle.name == string2);
				i++;
			}
		}
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0002D090 File Offset: 0x0002B290
	private void OnDisable()
	{
		if (this.mCheck != null)
		{
			EventDelegate.Remove(this.mCheck.onChange, new EventDelegate.Callback(this.SaveState));
		}
		else if (this.mList != null)
		{
			EventDelegate.Remove(this.mList.onChange, new EventDelegate.Callback(this.SaveSelection));
		}
		else if (this.mSlider != null)
		{
			EventDelegate.Remove(this.mSlider.onChange, new EventDelegate.Callback(this.SaveProgress));
		}
		else
		{
			UIToggle[] componentsInChildren = base.GetComponentsInChildren<UIToggle>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				UIToggle uitoggle = componentsInChildren[i];
				if (uitoggle.value)
				{
					PlayerPrefs.SetString(this.key, uitoggle.name);
					break;
				}
				i++;
			}
		}
	}

	// Token: 0x06000364 RID: 868 RVA: 0x000074BE File Offset: 0x000056BE
	public void SaveSelection()
	{
		PlayerPrefs.SetString(this.key, UIPopupList.current.value);
	}

	// Token: 0x06000365 RID: 869 RVA: 0x000074D5 File Offset: 0x000056D5
	public void SaveState()
	{
		PlayerPrefs.SetInt(this.key, (!UIToggle.current.value) ? 0 : 1);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x000074F8 File Offset: 0x000056F8
	public void SaveProgress()
	{
		PlayerPrefs.SetFloat(this.key, UIProgressBar.current.value);
	}

	// Token: 0x04000274 RID: 628
	public string keyName;

	// Token: 0x04000275 RID: 629
	private UIPopupList mList;

	// Token: 0x04000276 RID: 630
	private UIToggle mCheck;

	// Token: 0x04000277 RID: 631
	private UIProgressBar mSlider;
}
