using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
[RequireComponent(typeof(UIWidget))]
public class SetColorPickerColor : MonoBehaviour
{
	// Token: 0x060001B2 RID: 434 RVA: 0x00023F34 File Offset: 0x00022134
	public void SetToCurrent()
	{
		if (this.mWidget == null)
		{
			this.mWidget = base.GetComponent<UIWidget>();
		}
		if (UIColorPicker.current != null)
		{
			this.mWidget.color = UIColorPicker.current.value;
		}
	}

	// Token: 0x040000C1 RID: 193
	[NonSerialized]
	private UIWidget mWidget;
}
