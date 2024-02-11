using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x020000DF RID: 223
public abstract class UITweener : MonoBehaviour
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000740 RID: 1856 RVA: 0x00047C28 File Offset: 0x00045E28
	public float amountPerDelta
	{
		get
		{
			if (this.duration == 0f)
			{
				return 1000f;
			}
			if (this.mDuration != this.duration)
			{
				this.mDuration = this.duration;
				this.mAmountPerDelta = Mathf.Abs(1f / this.duration) * Mathf.Sign(this.mAmountPerDelta);
			}
			return this.mAmountPerDelta;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000741 RID: 1857 RVA: 0x00009DBD File Offset: 0x00007FBD
	// (set) Token: 0x06000742 RID: 1858 RVA: 0x00009DC5 File Offset: 0x00007FC5
	public float tweenFactor
	{
		get
		{
			return this.mFactor;
		}
		set
		{
			this.mFactor = Mathf.Clamp01(value);
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000743 RID: 1859 RVA: 0x00009DD3 File Offset: 0x00007FD3
	public Direction direction
	{
		get
		{
			return (this.amountPerDelta >= 0f) ? Direction.Forward : Direction.Reverse;
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00009DEC File Offset: 0x00007FEC
	private void Reset()
	{
		if (!this.mStarted)
		{
			this.SetStartToCurrentValue();
			this.SetEndToCurrentValue();
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x00009E05 File Offset: 0x00008005
	protected virtual void Start()
	{
		this.DoUpdate();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00009E0D File Offset: 0x0000800D
	protected void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.DoUpdate();
		}
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00009E20 File Offset: 0x00008020
	protected void FixedUpdate()
	{
		if (this.useFixedUpdate)
		{
			this.DoUpdate();
		}
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00047C94 File Offset: 0x00045E94
	protected void DoUpdate()
	{
		float num = (!this.ignoreTimeScale || this.useFixedUpdate) ? Time.deltaTime : Time.unscaledDeltaTime;
		float num2 = (!this.ignoreTimeScale || this.useFixedUpdate) ? Time.time : Time.unscaledTime;
		if (!this.mStarted)
		{
			num = 0f;
			this.mStarted = true;
			this.mStartTime = num2 + this.delay;
		}
		if (num2 < this.mStartTime)
		{
			return;
		}
		this.mFactor += ((this.duration != 0f) ? (this.amountPerDelta * num * this.timeScale) : 1f);
		if (this.style == UITweener.Style.Loop)
		{
			if (this.mFactor > 1f)
			{
				this.mFactor -= Mathf.Floor(this.mFactor);
			}
		}
		else if (this.style == UITweener.Style.PingPong)
		{
			if (this.mFactor > 1f)
			{
				this.mFactor = 1f - (this.mFactor - Mathf.Floor(this.mFactor));
				this.mAmountPerDelta = -this.mAmountPerDelta;
			}
			else if (this.mFactor < 0f)
			{
				this.mFactor = -this.mFactor;
				this.mFactor -= Mathf.Floor(this.mFactor);
				this.mAmountPerDelta = -this.mAmountPerDelta;
			}
		}
		if (this.style == UITweener.Style.Once && (this.duration == 0f || this.mFactor > 1f || this.mFactor < 0f))
		{
			this.mFactor = Mathf.Clamp01(this.mFactor);
			this.Sample(this.mFactor, true);
			base.enabled = false;
			if (UITweener.current != this)
			{
				UITweener uitweener = UITweener.current;
				UITweener.current = this;
				if (this.onFinished != null)
				{
					this.mTemp = this.onFinished;
					this.onFinished = new List<EventDelegate>();
					EventDelegate.Execute(this.mTemp);
					for (int i = 0; i < this.mTemp.Count; i++)
					{
						EventDelegate eventDelegate = this.mTemp[i];
						if (eventDelegate != null && !eventDelegate.oneShot)
						{
							EventDelegate.Add(this.onFinished, eventDelegate, eventDelegate.oneShot);
						}
					}
					this.mTemp = null;
				}
				if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
				{
					this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
				}
				UITweener.current = uitweener;
			}
		}
		else
		{
			this.Sample(this.mFactor, false);
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00009E33 File Offset: 0x00008033
	public void SetOnFinished(EventDelegate.Callback del)
	{
		EventDelegate.Set(this.onFinished, del);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00009E42 File Offset: 0x00008042
	public void SetOnFinished(EventDelegate del)
	{
		EventDelegate.Set(this.onFinished, del);
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00009E50 File Offset: 0x00008050
	public void AddOnFinished(EventDelegate.Callback del)
	{
		EventDelegate.Add(this.onFinished, del);
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00009E5F File Offset: 0x0000805F
	public void AddOnFinished(EventDelegate del)
	{
		EventDelegate.Add(this.onFinished, del);
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00009E6D File Offset: 0x0000806D
	public void RemoveOnFinished(EventDelegate del)
	{
		if (this.onFinished != null)
		{
			this.onFinished.Remove(del);
		}
		if (this.mTemp != null)
		{
			this.mTemp.Remove(del);
		}
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00009E9F File Offset: 0x0000809F
	private void OnDisable()
	{
		this.mStarted = false;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00009EA8 File Offset: 0x000080A8
	public void Finish()
	{
		if (base.enabled)
		{
			this.Sample((this.mAmountPerDelta <= 0f) ? 0f : 1f, true);
			base.enabled = false;
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00047F6C File Offset: 0x0004616C
	public void Sample(float factor, bool isFinished)
	{
		float num = Mathf.Clamp01(factor);
		if (this.method == UITweener.Method.EaseIn)
		{
			num = 1f - Mathf.Sin(1.57079637f * (1f - num));
			if (this.steeperCurves)
			{
				num *= num;
			}
		}
		else if (this.method == UITweener.Method.EaseOut)
		{
			num = Mathf.Sin(1.57079637f * num);
			if (this.steeperCurves)
			{
				num = 1f - num;
				num = 1f - num * num;
			}
		}
		else if (this.method == UITweener.Method.EaseInOut)
		{
			num -= Mathf.Sin(num * 6.28318548f) / 6.28318548f;
			if (this.steeperCurves)
			{
				num = num * 2f - 1f;
				float num2 = Mathf.Sign(num);
				num = 1f - Mathf.Abs(num);
				num = 1f - num * num;
				num = num2 * num * 0.5f + 0.5f;
			}
		}
		else if (this.method == UITweener.Method.BounceIn)
		{
			num = this.BounceLogic(num);
		}
		else if (this.method == UITweener.Method.BounceOut)
		{
			num = 1f - this.BounceLogic(1f - num);
		}
		this.OnUpdate((this.animationCurve == null) ? num : this.animationCurve.Evaluate(num), isFinished);
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x000480C0 File Offset: 0x000462C0
	private float BounceLogic(float val)
	{
		if (val < 0.363636f)
		{
			val = 7.5685f * val * val;
		}
		else if (val < 0.727272f)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f;
		}
		else if (val < 0.90909f)
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f;
		}
		else
		{
			val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f;
		}
		return val;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00009EE2 File Offset: 0x000080E2
	[Obsolete("Use PlayForward() instead")]
	public void Play()
	{
		this.Play(true);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00009EE2 File Offset: 0x000080E2
	public void PlayForward()
	{
		this.Play(true);
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00009EEB File Offset: 0x000080EB
	public void PlayReverse()
	{
		this.Play(false);
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00048158 File Offset: 0x00046358
	public virtual void Play(bool forward)
	{
		this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
		if (!forward)
		{
			this.mAmountPerDelta = -this.mAmountPerDelta;
		}
		if (!base.enabled)
		{
			base.enabled = true;
			this.mStarted = false;
		}
		this.DoUpdate();
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00009EF4 File Offset: 0x000080F4
	public void ResetToBeginning()
	{
		this.mStarted = false;
		this.mFactor = ((this.amountPerDelta >= 0f) ? 0f : 1f);
		this.Sample(this.mFactor, false);
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00009F2F File Offset: 0x0000812F
	public void Toggle()
	{
		if (this.mFactor > 0f)
		{
			this.mAmountPerDelta = -this.amountPerDelta;
		}
		else
		{
			this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
		}
		base.enabled = true;
	}

	// Token: 0x06000758 RID: 1880
	protected abstract void OnUpdate(float factor, bool isFinished);

	// Token: 0x06000759 RID: 1881 RVA: 0x000481A8 File Offset: 0x000463A8
	public static T Begin<T>(GameObject go, float duration, float delay = 0f) where T : UITweener
	{
		T t = go.GetComponent<T>();
		if (t != null && t.tweenGroup != 0)
		{
			t = (T)((object)null);
			T[] components = go.GetComponents<T>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
                t = components[i];
				if (t != null && t.tweenGroup == 0)
				{
					break;
				}
				t = (T)((object)null);
				i++;
			}
		}
		if (t == null)
		{
			t = go.AddComponent<T>();
			if (t == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Unable to add ",
					typeof(T),
					" to ",
					NGUITools.GetHierarchy(go)
				}), go);
				return (T)((object)null);
			}
        }
		t.mStarted = false;
		t.mFactor = 0f;
		t.duration = duration;
		t.mDuration = duration;
		t.delay = delay;
		t.mAmountPerDelta = ((duration <= 0f) ? 1000f : Mathf.Abs(1f / duration));
		t.style = UITweener.Style.Once;
		t.animationCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f, 0f, 1f),
			new Keyframe(1f, 1f, 1f, 0f)
		});
		t.eventReceiver = null;
		t.callWhenFinished = null;
		t.onFinished.Clear();
		if (t.mTemp != null)
		{
			t.mTemp.Clear();
		}
		t.enabled = true;
		return t;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x0000574F File Offset: 0x0000394F
	public virtual void SetStartToCurrentValue()
	{
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x0000574F File Offset: 0x0000394F
	public virtual void SetEndToCurrentValue()
	{
	}

	// Token: 0x040004FC RID: 1276
	public static UITweener current;

	// Token: 0x040004FD RID: 1277
	[HideInInspector]
	public UITweener.Method method;

	// Token: 0x040004FE RID: 1278
	[HideInInspector]
	public UITweener.Style style;

	// Token: 0x040004FF RID: 1279
	[HideInInspector]
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f, 0f, 1f),
		new Keyframe(1f, 1f, 1f, 0f)
	});

	// Token: 0x04000500 RID: 1280
	[HideInInspector]
	public bool ignoreTimeScale = true;

	// Token: 0x04000501 RID: 1281
	[HideInInspector]
	public float delay;

	// Token: 0x04000502 RID: 1282
	[HideInInspector]
	public float duration = 1f;

	// Token: 0x04000503 RID: 1283
	[HideInInspector]
	public bool steeperCurves;

	// Token: 0x04000504 RID: 1284
	[HideInInspector]
	public int tweenGroup;

	// Token: 0x04000505 RID: 1285
	[Tooltip("By default, Update() will be used for tweening. Setting this to 'true' will make the tween happen in FixedUpdate() insted.")]
	public bool useFixedUpdate;

	// Token: 0x04000506 RID: 1286
	[HideInInspector]
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x04000507 RID: 1287
	[HideInInspector]
	public GameObject eventReceiver;

	// Token: 0x04000508 RID: 1288
	[HideInInspector]
	public string callWhenFinished;

	// Token: 0x04000509 RID: 1289
	[NonSerialized]
	public float timeScale = 1f;

	// Token: 0x0400050A RID: 1290
	private bool mStarted;

	// Token: 0x0400050B RID: 1291
	private float mStartTime;

	// Token: 0x0400050C RID: 1292
	private float mDuration;

	// Token: 0x0400050D RID: 1293
	private float mAmountPerDelta = 1000f;

	// Token: 0x0400050E RID: 1294
	private float mFactor;

	// Token: 0x0400050F RID: 1295
	private List<EventDelegate> mTemp;

	// Token: 0x020000E0 RID: 224
	[DoNotObfuscateNGUI]
	public enum Method
	{
		// Token: 0x04000511 RID: 1297
		Linear,
		// Token: 0x04000512 RID: 1298
		EaseIn,
		// Token: 0x04000513 RID: 1299
		EaseOut,
		// Token: 0x04000514 RID: 1300
		EaseInOut,
		// Token: 0x04000515 RID: 1301
		BounceIn,
		// Token: 0x04000516 RID: 1302
		BounceOut
	}

	// Token: 0x020000E1 RID: 225
	[DoNotObfuscateNGUI]
	public enum Style
	{
		// Token: 0x04000518 RID: 1304
		Once,
		// Token: 0x04000519 RID: 1305
		Loop,
		// Token: 0x0400051A RID: 1306
		PingPong
	}
}
