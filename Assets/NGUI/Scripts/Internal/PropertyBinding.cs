using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
[AddComponentMenu("NGUI/Internal/Property Binding")]
[ExecuteInEditMode]
public class PropertyBinding : MonoBehaviour
{
	// Token: 0x0600055C RID: 1372 RVA: 0x000085F0 File Offset: 0x000067F0
	private void Start()
	{
		this.UpdateTarget();
		if (this.update == PropertyBinding.UpdateCondition.OnStart)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0000860A File Offset: 0x0000680A
	private void Update()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnUpdate)
		{
			this.UpdateTarget();
		}
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0000861E File Offset: 0x0000681E
	private void LateUpdate()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnLateUpdate)
		{
			this.UpdateTarget();
		}
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00008632 File Offset: 0x00006832
	private void FixedUpdate()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnFixedUpdate)
		{
			this.UpdateTarget();
		}
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00008646 File Offset: 0x00006846
	private void OnValidate()
	{
		if (this.source != null)
		{
			this.source.Reset();
		}
		if (this.target != null)
		{
			this.target.Reset();
		}
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0003E4F0 File Offset: 0x0003C6F0
	[ContextMenu("Update Now")]
	public void UpdateTarget()
	{
		if (this.source != null && this.target != null && this.source.isValid && this.target.isValid)
		{
			if (this.direction == PropertyBinding.Direction.SourceUpdatesTarget)
			{
				this.target.Set(this.source.Get());
			}
			else if (this.direction == PropertyBinding.Direction.TargetUpdatesSource)
			{
				this.source.Set(this.target.Get());
			}
			else if (this.source.GetPropertyType() == this.target.GetPropertyType())
			{
				object obj = this.source.Get();
				if (this.mLastValue == null || !this.mLastValue.Equals(obj))
				{
					this.mLastValue = obj;
					this.target.Set(obj);
				}
				else
				{
					obj = this.target.Get();
					if (!this.mLastValue.Equals(obj))
					{
						this.mLastValue = obj;
						this.source.Set(obj);
					}
				}
			}
		}
	}

	// Token: 0x040003B1 RID: 945
	public PropertyReference source;

	// Token: 0x040003B2 RID: 946
	public PropertyReference target;

	// Token: 0x040003B3 RID: 947
	public PropertyBinding.Direction direction;

	// Token: 0x040003B4 RID: 948
	public PropertyBinding.UpdateCondition update = PropertyBinding.UpdateCondition.OnUpdate;

	// Token: 0x040003B5 RID: 949
	public bool editMode = true;

	// Token: 0x040003B6 RID: 950
	private object mLastValue;

	// Token: 0x020000A8 RID: 168
	[DoNotObfuscateNGUI]
	public enum UpdateCondition
	{
		// Token: 0x040003B8 RID: 952
		OnStart,
		// Token: 0x040003B9 RID: 953
		OnUpdate,
		// Token: 0x040003BA RID: 954
		OnLateUpdate,
		// Token: 0x040003BB RID: 955
		OnFixedUpdate
	}

	// Token: 0x020000A9 RID: 169
	[DoNotObfuscateNGUI]
	public enum Direction
	{
		// Token: 0x040003BD RID: 957
		SourceUpdatesTarget,
		// Token: 0x040003BE RID: 958
		TargetUpdatesSource,
		// Token: 0x040003BF RID: 959
		BiDirectional
	}
}
