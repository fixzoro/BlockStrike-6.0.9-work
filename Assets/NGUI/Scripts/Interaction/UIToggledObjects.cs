using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000087 RID: 135
[AddComponentMenu("NGUI/Interaction/Toggled Objects")]
public class UIToggledObjects : MonoBehaviour
{
	// Token: 0x060003CE RID: 974 RVA: 0x00030328 File Offset: 0x0002E528
	private void Awake()
	{
		if (this.target != null)
		{
			if (this.activate.Count == 0 && this.deactivate.Count == 0)
			{
				if (this.inverse)
				{
					this.deactivate.Add(this.target);
				}
				else
				{
					this.activate.Add(this.target);
				}
			}
			else
			{
				this.target = null;
			}
		}
		UIToggle component = base.GetComponent<UIToggle>();
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.Toggle));
	}

	// Token: 0x060003CF RID: 975 RVA: 0x000303C4 File Offset: 0x0002E5C4
	public void Toggle()
	{
		bool value = UIToggle.current.value;
		if (base.enabled)
		{
			for (int i = 0; i < this.activate.Count; i++)
			{
				this.Set(this.activate[i], value);
			}
			for (int j = 0; j < this.deactivate.Count; j++)
			{
				this.Set(this.deactivate[j], !value);
			}
		}
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000078DF File Offset: 0x00005ADF
	private void Set(GameObject go, bool state)
	{
		if (go != null)
		{
			NGUITools.SetActive(go, state);
		}
	}

	// Token: 0x040002ED RID: 749
	public List<GameObject> activate;

	// Token: 0x040002EE RID: 750
	public List<GameObject> deactivate;

	// Token: 0x040002EF RID: 751
	[HideInInspector]
	[SerializeField]
	private GameObject target;

	// Token: 0x040002F0 RID: 752
	[SerializeField]
	[HideInInspector]
	private bool inverse;
}
