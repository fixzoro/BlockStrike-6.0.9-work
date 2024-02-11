using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200011D RID: 285
[AddComponentMenu("NGUI/UI/Root")]
[RequireComponent(typeof(UIUpdateManager))]
[ExecuteInEditMode]
public class UIRoot : MonoBehaviour
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static UIRoot uiroot;
#endif

    public static UIRoot instance;

    // Token: 0x170001AF RID: 431
    // (get) Token: 0x06000A03 RID: 2563 RVA: 0x0000B552 File Offset: 0x00009752
    public UIRoot.Constraint constraint
	{
		get
		{
			if (this.fitWidth)
			{
				if (this.fitHeight)
				{
					return UIRoot.Constraint.Fit;
				}
				return UIRoot.Constraint.FitWidth;
			}
			else
			{
				if (this.fitHeight)
				{
					return UIRoot.Constraint.FitHeight;
				}
				return UIRoot.Constraint.Fill;
			}
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000A04 RID: 2564 RVA: 0x0005773C File Offset: 0x0005593C
	public UIRoot.Scaling activeScaling
	{
		get
		{
			UIRoot.Scaling scaling = this.scalingStyle;
			if (scaling == UIRoot.Scaling.ConstrainedOnMobiles)
			{
				return UIRoot.Scaling.Constrained;
			}
			return scaling;
		}
	}

    // Token: 0x170001B1 RID: 433
    // (get) Token: 0x06000A05 RID: 2565 RVA: 0x0005775C File Offset: 0x0005595C
    public int activeHeight
	{
		get
		{
			if (this.activeScaling == UIRoot.Scaling.Flexible)
			{
				Vector2 screenSize = NGUITools.screenSize;
				float num = screenSize.x / screenSize.y;
				if (screenSize.y < (float)this.minimumHeight)
				{
					screenSize.y = (float)this.minimumHeight;
					screenSize.x = screenSize.y * num;
				}
				else if (screenSize.y > (float)this.maximumHeight)
				{
					screenSize.y = (float)this.maximumHeight;
					screenSize.x = screenSize.y * num;
				}
				int num2 = Mathf.RoundToInt((!this.shrinkPortraitUI || screenSize.y <= screenSize.x) ? screenSize.y : (screenSize.y / num));
				return (!this.adjustByDPI) ? num2 : NGUIMath.AdjustByDPI((float)num2);
			}
			UIRoot.Constraint constraint = this.constraint;
			if (constraint == UIRoot.Constraint.FitHeight)
			{
				return this.manualHeight;
			}
			Vector2 screenSize2 = NGUITools.screenSize;
			float num3 = screenSize2.x / screenSize2.y;
			float num4 = (float)this.manualWidth / (float)this.manualHeight;
			switch (constraint)
			{
			case UIRoot.Constraint.Fit:
				return (num4 <= num3) ? this.manualHeight : Mathf.RoundToInt((float)this.manualWidth / num3);
			case UIRoot.Constraint.Fill:
				return (num4 >= num3) ? this.manualHeight : Mathf.RoundToInt((float)this.manualWidth / num3);
			case UIRoot.Constraint.FitWidth:
				return Mathf.RoundToInt((float)this.manualWidth / num3);
			default:
				return this.manualHeight;
			}
		}
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000A06 RID: 2566 RVA: 0x00057900 File Offset: 0x00055B00
	public float pixelSizeAdjustment
	{
		get
		{
			int num = Mathf.RoundToInt(NGUITools.screenSize.y);
			return (num != -1) ? this.GetPixelSizeAdjustment(num) : 1f;
		}
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00057938 File Offset: 0x00055B38
	public static float GetPixelSizeAdjustment(GameObject go)
	{
		UIRoot uiroot = NGUITools.FindInParents<UIRoot>(go);
		return (!(uiroot != null)) ? 1f : uiroot.pixelSizeAdjustment;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00057968 File Offset: 0x00055B68
	public float GetPixelSizeAdjustment(int height)
	{
		height = Mathf.Max(2, height);
		if (this.activeScaling == UIRoot.Scaling.Constrained)
		{
			return (float)this.activeHeight / (float)height;
		}
		if (height < this.minimumHeight)
		{
			return (float)this.minimumHeight / (float)height;
		}
		if (height > this.maximumHeight)
		{
			return (float)this.maximumHeight / (float)height;
		}
		return 1f;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0000B57C File Offset: 0x0000977C
	protected virtual void Awake()
	{
		this.mTrans = base.transform;
        instance = this;
#if UNITY_EDITOR
        uiroot = this;
#endif
}

// Token: 0x06000A0A RID: 2570 RVA: 0x0000B58A File Offset: 0x0000978A
protected virtual void OnEnable()
	{
		UIRoot.list.Add(this);
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0000B597 File Offset: 0x00009797
	protected virtual void OnDisable()
	{
		UIRoot.list.Remove(this);
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x000579C8 File Offset: 0x00055BC8
	protected virtual void Start()
	{
		UIOrthoCamera componentInChildren = base.GetComponentInChildren<UIOrthoCamera>();
		if (componentInChildren != null)
		{
			Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", componentInChildren);
			Camera component = componentInChildren.gameObject.GetComponent<Camera>();
			componentInChildren.enabled = false;
			if (component != null)
			{
				component.orthographicSize = 1f;
			}
		}
		else
		{
			this.UpdateScale(false);
			base.Invoke("UpdateScale", 0.5f);
		}
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0000B5A5 File Offset: 0x000097A5
	public void UpdateScale()
	{
		this.UpdateScale(true);
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00057A3C File Offset: 0x00055C3C
	public void UpdateScale(bool updateAnchors)
	{
		if (this.mTrans != null)
		{
			float num = (float)this.activeHeight;
			if (num > 0f)
			{
				float num2 = 2f / num;
				Vector3 localScale = this.mTrans.localScale;
				if (Mathf.Abs(localScale.x - num2) > 1.401298E-45f || Mathf.Abs(localScale.y - num2) > 1.401298E-45f || Mathf.Abs(localScale.z - num2) > 1.401298E-45f)
				{
					this.mTrans.localScale = new Vector3(num2, num2, num2);
					if (updateAnchors)
					{
						base.BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00057AF0 File Offset: 0x00055CF0
	public static void Broadcast(string funcName)
	{
		int i = 0;
		int count = UIRoot.list.Count;
		while (i < count)
		{
			UIRoot uiroot = UIRoot.list[i];
			if (uiroot != null)
			{
				uiroot.BroadcastMessage(funcName, SendMessageOptions.DontRequireReceiver);
			}
			i++;
		}
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00057B3C File Offset: 0x00055D3C
	public static void Broadcast(string funcName, object param)
	{
		if (param == null)
		{
			Debug.LogError("SendMessage is bugged when you try to pass 'null' in the parameter field. It behaves as if no parameter was specified.");
		}
		else
		{
			int i = 0;
			int count = UIRoot.list.Count;
			while (i < count)
			{
				UIRoot uiroot = UIRoot.list[i];
				if (uiroot != null)
				{
					uiroot.BroadcastMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
				}
				i++;
			}
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00057B9C File Offset: 0x00055D9C
	[ContextMenu("Fix Anchor")]
	private void FixAnchor()
	{
		UIAnchor[] componentsInChildren = base.GetComponentsInChildren<UIAnchor>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].uiCamera == null)
			{
				componentsInChildren[i].uiCamera = componentsInChildren[i].GetComponentInParent<Camera>();
				Debug.Log(componentsInChildren[i].name, componentsInChildren[i].gameObject);
			}
		}
	}

	// Token: 0x040006E2 RID: 1762
	public static List<UIRoot> list = new List<UIRoot>();

	// Token: 0x040006E3 RID: 1763
	public UIRoot.Scaling scalingStyle;

	// Token: 0x040006E4 RID: 1764
	public int manualWidth = 1280;

	// Token: 0x040006E5 RID: 1765
	public int manualHeight = 720;

	// Token: 0x040006E6 RID: 1766
	public int minimumHeight = 320;

	// Token: 0x040006E7 RID: 1767
	public int maximumHeight = 1536;

	// Token: 0x040006E8 RID: 1768
	public bool fitWidth;

	// Token: 0x040006E9 RID: 1769
	public bool fitHeight = true;

	// Token: 0x040006EA RID: 1770
	public bool adjustByDPI;

	// Token: 0x040006EB RID: 1771
	public bool shrinkPortraitUI;

	// Token: 0x040006EC RID: 1772
	private Transform mTrans;

	// Token: 0x0200011E RID: 286
	[DoNotObfuscateNGUI]
	public enum Scaling
	{
		// Token: 0x040006EE RID: 1774
		Flexible,
		// Token: 0x040006EF RID: 1775
		Constrained,
		// Token: 0x040006F0 RID: 1776
		ConstrainedOnMobiles
	}

	// Token: 0x0200011F RID: 287
	[DoNotObfuscateNGUI]
	public enum Constraint
	{
		// Token: 0x040006F2 RID: 1778
		Fit,
		// Token: 0x040006F3 RID: 1779
		Fill,
		// Token: 0x040006F4 RID: 1780
		FitWidth,
		// Token: 0x040006F5 RID: 1781
		FitHeight
	}
}
