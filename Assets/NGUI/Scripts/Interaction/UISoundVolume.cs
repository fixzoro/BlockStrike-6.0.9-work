using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
[AddComponentMenu("NGUI/Interaction/Sound Volume")]
[RequireComponent(typeof(UISlider))]
public class UISoundVolume : MonoBehaviour
{
	// Token: 0x060003A9 RID: 937 RVA: 0x0002F430 File Offset: 0x0002D630
	private void Awake()
	{
		UISlider component = base.GetComponent<UISlider>();
		component.value = NGUITools.soundVolume;
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnChange));
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00007769 File Offset: 0x00005969
	private void OnChange()
	{
		NGUITools.soundVolume = UIProgressBar.current.value;
	}
}
