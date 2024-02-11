using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000106 RID: 262
[RequireComponent(typeof(UITexture))]
public class UIColorPicker : MonoBehaviour
{
	// Token: 0x060008A6 RID: 2214 RVA: 0x0004F1B4 File Offset: 0x0004D3B4
	private void Start()
	{
		this.mTrans = base.transform;
		this.mUITex = base.GetComponent<UITexture>();
		this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		this.mWidth = this.mUITex.width;
		this.mHeight = this.mUITex.height;
		Color[] array = new Color[this.mWidth * this.mHeight];
		for (int i = 0; i < this.mHeight; i++)
		{
			float y = ((float)i - 1f) / (float)this.mHeight;
			for (int j = 0; j < this.mWidth; j++)
			{
				float x = ((float)j - 1f) / (float)this.mWidth;
				int num = j + i * this.mWidth;
				array[num] = UIColorPicker.Sample(x, y);
			}
		}
		this.mTex = new Texture2D(this.mWidth, this.mHeight, TextureFormat.RGB24, false);
		this.mTex.SetPixels(array);
		this.mTex.filterMode = FilterMode.Trilinear;
		this.mTex.wrapMode = TextureWrapMode.Clamp;
		this.mTex.Apply();
		this.mUITex.mainTexture = this.mTex;
		this.Select(this.value);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0004F2FC File Offset: 0x0004D4FC
	private void OnEnable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onPan = (UICamera.VectorDelegate)Delegate.Combine(UICamera.onPan, new UICamera.VectorDelegate(this.OnPan));
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0004F36C File Offset: 0x0004D56C
	private void OnDisable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		UICamera.onDrag = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onDrag, new UICamera.VectorDelegate(this.OnDrag));
		UICamera.onPan = (UICamera.VectorDelegate)Delegate.Remove(UICamera.onPan, new UICamera.VectorDelegate(this.OnPan));
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0000A695 File Offset: 0x00008895
	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.mTex);
		this.mTex = null;
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0000A6A9 File Offset: 0x000088A9
	private void OnPress(GameObject go, bool pressed)
	{
		if (this.mUITex.cachedGameObject == go && base.enabled && pressed && UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			this.Sample();
		}
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0000A6E3 File Offset: 0x000088E3
	private void OnDrag(GameObject go, Vector2 delta)
	{
		if (this.mUITex.cachedGameObject == go && base.enabled)
		{
			this.Sample();
		}
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0004F3DC File Offset: 0x0004D5DC
	private void OnPan(GameObject go, Vector2 delta)
	{
		if (this.mUITex.cachedGameObject == go && base.enabled)
		{
			this.mPos.x = Mathf.Clamp01(this.mPos.x + delta.x);
			this.mPos.y = Mathf.Clamp01(this.mPos.y + delta.y);
			this.Select(this.mPos);
		}
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0004F45C File Offset: 0x0004D65C
	private void Sample()
	{
		Vector3 vector = this.mTrans.InverseTransformPoint(UICamera.lastWorldPosition);
		Vector3[] localCorners = this.mUITex.localCorners;
		this.mPos.x = Mathf.Clamp01((vector.x - localCorners[0].x) / (localCorners[2].x - localCorners[0].x));
		this.mPos.y = Mathf.Clamp01((vector.y - localCorners[0].y) / (localCorners[2].y - localCorners[0].y));
		if (this.selectionWidget != null)
		{
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			vector = this.mTrans.TransformPoint(vector);
			this.selectionWidget.transform.OverlayPosition(vector, this.mCam.cachedCamera);
		}
		this.value = UIColorPicker.Sample(this.mPos.x, this.mPos.y);
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = null;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0004F5D4 File Offset: 0x0004D7D4
	public void Select(Vector2 v)
	{
		v.x = Mathf.Clamp01(v.x);
		v.y = Mathf.Clamp01(v.y);
		this.mPos = v;
		if (this.selectionWidget != null)
		{
			Vector3[] localCorners = this.mUITex.localCorners;
			v.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			v.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			v = this.mTrans.TransformPoint(v);
			this.selectionWidget.transform.OverlayPosition(v, this.mCam.cachedCamera);
		}
		this.value = UIColorPicker.Sample(this.mPos.x, this.mPos.y);
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = null;
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0004F6FC File Offset: 0x0004D8FC
	public Vector2 Select(Color c)
	{
		if (this.mUITex == null)
		{
			this.value = c;
			return this.mPos;
		}
		float num = float.MaxValue;
		for (int i = 0; i < this.mHeight; i++)
		{
			float y = ((float)i - 1f) / (float)this.mHeight;
			for (int j = 0; j < this.mWidth; j++)
			{
				float x = ((float)j - 1f) / (float)this.mWidth;
				Color color = UIColorPicker.Sample(x, y);
				Color color2 = color;
				color2.r -= c.r;
				color2.g -= c.g;
				color2.b -= c.b;
				float num2 = color2.r * color2.r + color2.g * color2.g + color2.b * color2.b;
				if (num2 < num)
				{
					num = num2;
					this.mPos.x = x;
					this.mPos.y = y;
				}
			}
		}
		if (this.selectionWidget != null)
		{
			Vector3[] localCorners = this.mUITex.localCorners;
			Vector3 vector;
			vector.x = Mathf.Lerp(localCorners[0].x, localCorners[2].x, this.mPos.x);
			vector.y = Mathf.Lerp(localCorners[0].y, localCorners[2].y, this.mPos.y);
			vector.z = 0f;
			vector = this.mTrans.TransformPoint(vector);
			this.selectionWidget.transform.OverlayPosition(vector, this.mCam.cachedCamera);
		}
		this.value = c;
		UIColorPicker.current = this;
		EventDelegate.Execute(this.onChange);
		UIColorPicker.current = null;
		return this.mPos;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0004F900 File Offset: 0x0004DB00
	public static Color Sample(float x, float y)
	{
		if (UIColorPicker.mRed == null)
		{
			UIColorPicker.mRed = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 1f),
				new Keyframe(0.142857149f, 1f),
				new Keyframe(0.2857143f, 0f),
				new Keyframe(0.428571433f, 0f),
				new Keyframe(0.5714286f, 0f),
				new Keyframe(0.714285731f, 1f),
				new Keyframe(0.857142866f, 1f),
				new Keyframe(1f, 0.5f)
			});
			UIColorPicker.mGreen = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.142857149f, 1f),
				new Keyframe(0.2857143f, 1f),
				new Keyframe(0.428571433f, 1f),
				new Keyframe(0.5714286f, 0f),
				new Keyframe(0.714285731f, 0f),
				new Keyframe(0.857142866f, 0f),
				new Keyframe(1f, 0.5f)
			});
			UIColorPicker.mBlue = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.142857149f, 0f),
				new Keyframe(0.2857143f, 0f),
				new Keyframe(0.428571433f, 1f),
				new Keyframe(0.5714286f, 1f),
				new Keyframe(0.714285731f, 1f),
				new Keyframe(0.857142866f, 0f),
				new Keyframe(1f, 0.5f)
			});
		}
		Vector3 from = new Vector3(UIColorPicker.mRed.Evaluate(x), UIColorPicker.mGreen.Evaluate(x), UIColorPicker.mBlue.Evaluate(x));
		if (y < 0.5f)
		{
			y *= 2f;
			from.x *= y;
			from.y *= y;
			from.z *= y;
		}
		else
		{
			from = Vector3.Lerp(from, Vector3.one, y * 2f - 1f);
		}
		return new Color(from.x, from.y, from.z, 1f);
	}

	// Token: 0x04000603 RID: 1539
	public static UIColorPicker current;

	// Token: 0x04000604 RID: 1540
	public Color value = Color.white;

	// Token: 0x04000605 RID: 1541
	public UIWidget selectionWidget;

	// Token: 0x04000606 RID: 1542
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000607 RID: 1543
	[NonSerialized]
	private Transform mTrans;

	// Token: 0x04000608 RID: 1544
	[NonSerialized]
	private UITexture mUITex;

	// Token: 0x04000609 RID: 1545
	[NonSerialized]
	private Texture2D mTex;

	// Token: 0x0400060A RID: 1546
	[NonSerialized]
	private UICamera mCam;

	// Token: 0x0400060B RID: 1547
	[NonSerialized]
	private Vector2 mPos;

	// Token: 0x0400060C RID: 1548
	[NonSerialized]
	private int mWidth;

	// Token: 0x0400060D RID: 1549
	[NonSerialized]
	private int mHeight;

	// Token: 0x0400060E RID: 1550
	private static AnimationCurve mRed;

	// Token: 0x0400060F RID: 1551
	private static AnimationCurve mGreen;

	// Token: 0x04000610 RID: 1552
	private static AnimationCurve mBlue;
}
