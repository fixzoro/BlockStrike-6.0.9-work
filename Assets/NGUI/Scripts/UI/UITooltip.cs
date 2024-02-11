using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
[AddComponentMenu("NGUI/UI/Tooltip")]
public class UITooltip : MonoBehaviour
{
	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06000AAD RID: 2733 RVA: 0x0000BCB4 File Offset: 0x00009EB4
	public static bool isVisible
	{
		get
		{
			return UITooltip.mInstance != null && UITooltip.mInstance.mTarget == 1f;
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0000BCDA File Offset: 0x00009EDA
	private void Awake()
	{
		UITooltip.mInstance = this;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0000BCE2 File Offset: 0x00009EE2
	private void OnDestroy()
	{
		UITooltip.mInstance = null;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0005AF0C File Offset: 0x0005910C
	protected virtual void Start()
	{
		this.mTrans = base.transform;
		this.mWidgets = base.GetComponentsInChildren<UIWidget>();
		this.mPos = this.mTrans.localPosition;
		if (this.uiCamera == null)
		{
			this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		this.SetAlpha(0f);
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0005AF74 File Offset: 0x00059174
	protected virtual void Update()
	{
		if (this.mTooltip != UICamera.tooltipObject)
		{
			this.mTooltip = null;
			this.mTarget = 0f;
		}
		if (this.mCurrent != this.mTarget)
		{
			this.mCurrent = Mathf.Lerp(this.mCurrent, this.mTarget, RealTime.deltaTime * this.appearSpeed);
			if (Mathf.Abs(this.mCurrent - this.mTarget) < 0.001f)
			{
				this.mCurrent = this.mTarget;
			}
			this.SetAlpha(this.mCurrent * this.mCurrent);
			if (this.scalingTransitions)
			{
				Vector3 b = this.mSize * 0.25f;
				b.y = -b.y;
				Vector3 localScale = Vector3.one * (1.5f - this.mCurrent * 0.5f);
				Vector3 localPosition = Vector3.Lerp(this.mPos - b, this.mPos, this.mCurrent);
				this.mTrans.localPosition = localPosition;
				this.mTrans.localScale = localScale;
			}
		}
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0005B098 File Offset: 0x00059298
	protected virtual void SetAlpha(float val)
	{
		int i = 0;
		int num = this.mWidgets.Length;
		while (i < num)
		{
			UIWidget uiwidget = this.mWidgets[i];
			Color color = uiwidget.color;
			color.a = val;
			uiwidget.color = color;
			i++;
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0005B0E0 File Offset: 0x000592E0
	protected virtual void SetText(string tooltipText)
	{
		if (this.text != null && !string.IsNullOrEmpty(tooltipText))
		{
			this.mTarget = 1f;
			this.mTooltip = UICamera.tooltipObject;
			this.text.text = tooltipText;
			this.mPos = UICamera.lastEventPosition;
			Transform transform = this.text.transform;
			Vector3 localPosition = transform.localPosition;
			Vector3 localScale = transform.localScale;
			this.mSize = this.text.printedSize;
			this.mSize.x = this.mSize.x * localScale.x;
			this.mSize.y = this.mSize.y * localScale.y;
			if (this.background != null)
			{
				Vector4 border = this.background.border;
				this.mSize.x = this.mSize.x + (border.x + border.z + (localPosition.x - border.x) * 2f);
				this.mSize.y = this.mSize.y + (border.y + border.w + (-localPosition.y - border.y) * 2f);
				this.background.width = Mathf.RoundToInt(this.mSize.x);
				this.background.height = Mathf.RoundToInt(this.mSize.y);
			}
			if (this.uiCamera != null)
			{
				this.mPos.x = Mathf.Clamp01(this.mPos.x / (float)Screen.width);
				this.mPos.y = Mathf.Clamp01(this.mPos.y / (float)Screen.height);
				float num = this.uiCamera.orthographicSize / this.mTrans.parent.lossyScale.y;
				float num2 = (float)Screen.height * 0.5f / num;
				Vector2 vector = new Vector2(num2 * this.mSize.x / (float)Screen.width, num2 * this.mSize.y / (float)Screen.height);
				this.mPos.x = Mathf.Min(this.mPos.x, 1f - vector.x);
				this.mPos.y = Mathf.Max(this.mPos.y, vector.y);
				this.mTrans.position = this.uiCamera.ViewportToWorldPoint(this.mPos);
				this.mPos = this.mTrans.localPosition;
				this.mPos.x = Mathf.Round(this.mPos.x);
				this.mPos.y = Mathf.Round(this.mPos.y);
			}
			else
			{
				if (this.mPos.x + this.mSize.x > (float)Screen.width)
				{
					this.mPos.x = (float)Screen.width - this.mSize.x;
				}
				if (this.mPos.y - this.mSize.y < 0f)
				{
					this.mPos.y = this.mSize.y;
				}
				this.mPos.x = this.mPos.x - (float)Screen.width * 0.5f;
				this.mPos.y = this.mPos.y - (float)Screen.height * 0.5f;
			}
			this.mTrans.localPosition = this.mPos;
			if (this.tooltipRoot != null)
			{
				this.tooltipRoot.BroadcastMessage("UpdateAnchors");
			}
			else
			{
				this.text.BroadcastMessage("UpdateAnchors");
			}
		}
		else
		{
			this.mTooltip = null;
			this.mTarget = 0f;
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0000BCEA File Offset: 0x00009EEA
	[Obsolete("Use UITooltip.Show instead")]
	public static void ShowText(string text)
	{
		if (UITooltip.mInstance != null)
		{
			UITooltip.mInstance.SetText(text);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0000BCEA File Offset: 0x00009EEA
	public static void Show(string text)
	{
		if (UITooltip.mInstance != null)
		{
			UITooltip.mInstance.SetText(text);
		}
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0000BD07 File Offset: 0x00009F07
	public static void Hide()
	{
		if (UITooltip.mInstance != null)
		{
			UITooltip.mInstance.mTooltip = null;
			UITooltip.mInstance.mTarget = 0f;
		}
	}

	// Token: 0x04000756 RID: 1878
	protected static UITooltip mInstance;

	// Token: 0x04000757 RID: 1879
	public Camera uiCamera;

	// Token: 0x04000758 RID: 1880
	public UILabel text;

	// Token: 0x04000759 RID: 1881
	public GameObject tooltipRoot;

	// Token: 0x0400075A RID: 1882
	public UISprite background;

	// Token: 0x0400075B RID: 1883
	public float appearSpeed = 10f;

	// Token: 0x0400075C RID: 1884
	public bool scalingTransitions = true;

	// Token: 0x0400075D RID: 1885
	protected GameObject mTooltip;

	// Token: 0x0400075E RID: 1886
	protected Transform mTrans;

	// Token: 0x0400075F RID: 1887
	protected float mTarget;

	// Token: 0x04000760 RID: 1888
	protected float mCurrent;

	// Token: 0x04000761 RID: 1889
	protected Vector3 mPos;

	// Token: 0x04000762 RID: 1890
	protected Vector3 mSize = Vector3.zero;

	// Token: 0x04000763 RID: 1891
	protected UIWidget[] mWidgets;
}
