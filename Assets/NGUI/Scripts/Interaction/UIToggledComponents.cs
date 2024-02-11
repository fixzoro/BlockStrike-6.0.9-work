using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000086 RID: 134
[AddComponentMenu("NGUI/Interaction/Toggled Components")]
[RequireComponent(typeof(UIToggle))]
[ExecuteInEditMode]
public class UIToggledComponents : MonoBehaviour
{
	// Token: 0x060003CB RID: 971 RVA: 0x000301FC File Offset: 0x0002E3FC
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

	// Token: 0x060003CC RID: 972 RVA: 0x00030298 File Offset: 0x0002E498
	public void Toggle()
	{
		if (base.enabled)
		{
			for (int i = 0; i < this.activate.Count; i++)
			{
				MonoBehaviour monoBehaviour = this.activate[i];
				monoBehaviour.enabled = UIToggle.current.value;
			}
			for (int j = 0; j < this.deactivate.Count; j++)
			{
				MonoBehaviour monoBehaviour2 = this.deactivate[j];
				monoBehaviour2.enabled = !UIToggle.current.value;
			}
		}
	}

	// Token: 0x040002E9 RID: 745
	public List<MonoBehaviour> activate;

	// Token: 0x040002EA RID: 746
	public List<MonoBehaviour> deactivate;

	// Token: 0x040002EB RID: 747
	[HideInInspector]
	[SerializeField]
	private MonoBehaviour target;

	// Token: 0x040002EC RID: 748
	[HideInInspector]
	[SerializeField]
	private bool inverse;
}
