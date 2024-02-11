using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
[AddComponentMenu("NGUI/Internal/Spring Panel")]
[RequireComponent(typeof(UIPanel))]
public class SpringPanel : MonoBehaviour
{
	// Token: 0x0600057E RID: 1406 RVA: 0x00008796 File Offset: 0x00006996
	private void Start()
	{
		this.mPanel = base.GetComponent<UIPanel>();
		this.mDrag = base.GetComponent<UIScrollView>();
		this.mTrans = base.transform;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x000087BC File Offset: 0x000069BC
	private void Update()
	{
		this.AdvanceTowardsPosition();
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0003EC54 File Offset: 0x0003CE54
	protected virtual void AdvanceTowardsPosition()
	{
		this.mDelta += RealTime.deltaTime;
		bool flag = false;
		Vector3 localPosition = this.mTrans.localPosition;
		Vector3 vector = NGUIMath.SpringLerp(localPosition, this.target, this.strength, this.mDelta);
		if ((vector - this.target).sqrMagnitude < 0.01f)
		{
			vector = this.target;
			base.enabled = false;
			flag = true;
		}
		vector.x = Mathf.Round(vector.x);
		vector.y = Mathf.Round(vector.y);
		vector.z = Mathf.Round(vector.z);
		if ((vector - this.target).sqrMagnitude < 0.01f)
		{
			return;
		}
		this.mDelta = 0f;
		this.mTrans.localPosition = vector;
		Vector3 vector2 = vector - localPosition;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= vector2.x;
		clipOffset.y -= vector2.y;
		this.mPanel.clipOffset = clipOffset;
		if (this.mDrag != null)
		{
			this.mDrag.UpdateScrollbars(false);
		}
		if (flag && this.onFinished != null)
		{
			SpringPanel.current = this;
			this.onFinished();
			SpringPanel.current = null;
		}
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0003EDCC File Offset: 0x0003CFCC
	public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
	{
		SpringPanel springPanel = go.GetComponent<SpringPanel>();
		if (springPanel == null)
		{
			springPanel = go.AddComponent<SpringPanel>();
		}
		springPanel.target = pos;
		springPanel.strength = strength;
		springPanel.onFinished = null;
		springPanel.enabled = true;
		return springPanel;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0003EE10 File Offset: 0x0003D010
	public static SpringPanel Stop(GameObject go)
	{
		SpringPanel component = go.GetComponent<SpringPanel>();
		if (component != null && component.enabled)
		{
			if (component.onFinished != null)
			{
				component.onFinished();
			}
			component.enabled = false;
		}
		return component;
	}

	// Token: 0x040003C5 RID: 965
	public static SpringPanel current;

	// Token: 0x040003C6 RID: 966
	public Vector3 target = Vector3.zero;

	// Token: 0x040003C7 RID: 967
	public float strength = 10f;

	// Token: 0x040003C8 RID: 968
	public SpringPanel.OnFinished onFinished;

	// Token: 0x040003C9 RID: 969
	[NonSerialized]
	private UIPanel mPanel;

	// Token: 0x040003CA RID: 970
	[NonSerialized]
	private Transform mTrans;

	// Token: 0x040003CB RID: 971
	[NonSerialized]
	private UIScrollView mDrag;

	// Token: 0x040003CC RID: 972
	[NonSerialized]
	private float mDelta;

	// Token: 0x020000AD RID: 173
	// (Invoke) Token: 0x06000584 RID: 1412
	public delegate void OnFinished();
}
