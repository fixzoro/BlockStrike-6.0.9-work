using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
[AddComponentMenu("NGUI/Tween/Spring Position")]
public class SpringPosition : MonoBehaviour
{
	// Token: 0x060006A8 RID: 1704 RVA: 0x0000958F File Offset: 0x0000778F
	private void Start()
	{
		this.mTrans = base.transform;
		if (this.updateScrollView)
		{
			this.mSv = NGUITools.FindInParents<UIScrollView>(base.gameObject);
		}
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x00046564 File Offset: 0x00044764
	private void Update()
	{
		float deltaTime = (!this.ignoreTimeScale) ? Time.deltaTime : RealTime.deltaTime;
		if (this.worldSpace)
		{
			if (this.mThreshold == 0f)
			{
				this.mThreshold = (this.target - this.mTrans.position).sqrMagnitude * 0.001f;
			}
			this.mTrans.position = NGUIMath.SpringLerp(this.mTrans.position, this.target, this.strength, deltaTime);
			if (this.mThreshold >= (this.target - this.mTrans.position).sqrMagnitude)
			{
				this.mTrans.position = this.target;
				this.NotifyListeners();
				base.enabled = false;
			}
		}
		else
		{
			if (this.mThreshold == 0f)
			{
				this.mThreshold = (this.target - this.mTrans.localPosition).sqrMagnitude * 1E-05f;
			}
			this.mTrans.localPosition = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
			if (this.mThreshold >= (this.target - this.mTrans.localPosition).sqrMagnitude)
			{
				this.mTrans.localPosition = this.target;
				this.NotifyListeners();
				base.enabled = false;
			}
		}
		if (this.mSv != null)
		{
			this.mSv.UpdateScrollbars(true);
		}
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0004670C File Offset: 0x0004490C
	private void NotifyListeners()
	{
		SpringPosition.current = this;
		if (this.onFinished != null)
		{
			this.onFinished();
		}
		if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
		{
			this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
		}
		SpringPosition.current = null;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00046770 File Offset: 0x00044970
	public static SpringPosition Begin(GameObject go, Vector3 pos, float strength)
	{
		SpringPosition springPosition = go.GetComponent<SpringPosition>();
		if (springPosition == null)
		{
			springPosition = go.AddComponent<SpringPosition>();
		}
		springPosition.target = pos;
		springPosition.strength = strength;
		springPosition.onFinished = null;
		if (!springPosition.enabled)
		{
			springPosition.enabled = true;
		}
		return springPosition;
	}

	// Token: 0x0400049E RID: 1182
	public static SpringPosition current;

	// Token: 0x0400049F RID: 1183
	public Vector3 target = Vector3.zero;

	// Token: 0x040004A0 RID: 1184
	public float strength = 10f;

	// Token: 0x040004A1 RID: 1185
	public bool worldSpace;

	// Token: 0x040004A2 RID: 1186
	public bool ignoreTimeScale;

	// Token: 0x040004A3 RID: 1187
	public bool updateScrollView;

	// Token: 0x040004A4 RID: 1188
	public SpringPosition.OnFinished onFinished;

	// Token: 0x040004A5 RID: 1189
	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	// Token: 0x040004A6 RID: 1190
	[HideInInspector]
	[SerializeField]
	public string callWhenFinished;

	// Token: 0x040004A7 RID: 1191
	private Transform mTrans;

	// Token: 0x040004A8 RID: 1192
	private float mThreshold;

	// Token: 0x040004A9 RID: 1193
	private UIScrollView mSv;

	// Token: 0x020000CE RID: 206
	// (Invoke) Token: 0x060006AD RID: 1709
	public delegate void OnFinished();
}
