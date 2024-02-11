using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class TweenLetters : UITweener
{
	// Token: 0x060006E7 RID: 1767 RVA: 0x000098EE File Offset: 0x00007AEE
	private void OnEnable()
	{
		this.mVertexCount = -1;
		UILabel uilabel = this.mLabel;
		uilabel.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Combine(uilabel.onPostFill, new UIWidget.OnPostFillCallback(this.OnPostFill));
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x0000991E File Offset: 0x00007B1E
	private void OnDisable()
	{
		UILabel uilabel = this.mLabel;
		uilabel.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Remove(uilabel.onPostFill, new UIWidget.OnPostFillCallback(this.OnPostFill));
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00009947 File Offset: 0x00007B47
	private void Awake()
	{
		this.mLabel = base.GetComponent<UILabel>();
		this.mCurrent = this.hoverOver;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00009961 File Offset: 0x00007B61
	public override void Play(bool forward)
	{
		this.mCurrent = ((!forward) ? this.hoverOut : this.hoverOver);
		base.Play(forward);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00046DE4 File Offset: 0x00044FE4
	private void OnPostFill(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (verts == null)
		{
			return;
		}
		int count = verts.Count;
		if (verts == null || count == 0)
		{
			return;
		}
		if (this.mLabel == null)
		{
			return;
		}
		try
		{
			int quadsPerCharacter = this.mLabel.quadsPerCharacter;
			int num = count / quadsPerCharacter / 4;
			if (this.mVertexCount != count)
			{
				this.mVertexCount = count;
				this.SetLetterOrder(num);
				this.GetLetterDuration(num);
			}
			Matrix4x4 identity = Matrix4x4.identity;
			Vector3 pos = Vector3.zero;
			Quaternion q = Quaternion.identity;
			Vector3 s = Vector3.one;
			Vector3 b = Vector3.zero;
			Quaternion from = Quaternion.Euler(this.mCurrent.rot);
			Vector3 vector = Vector3.zero;
			Color value = Color.clear;
			float num2 = base.tweenFactor * this.duration;
			for (int i = 0; i < quadsPerCharacter; i++)
			{
				for (int j = 0; j < num; j++)
				{
					int num3 = this.mLetterOrder[j];
					int num4 = i * num * 4 + num3 * 4;
					if (num4 < count)
					{
						float start = this.mLetter[num3].start;
						float num5 = Mathf.Clamp(num2 - start, 0f, this.mLetter[num3].duration) / this.mLetter[num3].duration;
						num5 = this.animationCurve.Evaluate(num5);
						b = TweenLetters.GetCenter(verts, num4, 4);
						Vector2 offset = this.mLetter[num3].offset;
						pos = TweenLetters.LerpUnclamped(this.mCurrent.pos + new Vector3(offset.x, offset.y, 0f), Vector3.zero, num5);
						q = Quaternion.Slerp(from, Quaternion.identity, num5);
						s = TweenLetters.LerpUnclamped(this.mCurrent.scale, Vector3.one, num5);
						float num6 = TweenLetters.LerpUnclamped(this.mCurrent.alpha, 1f, num5);
						identity.SetTRS(pos, q, s);
						for (int k = num4; k < num4 + 4; k++)
						{
							vector = verts[k];
							vector -= b;
							vector = identity.MultiplyPoint3x4(vector);
							vector += b;
							verts[k] = vector;
							value = cols[k];
							value.a *= num6;
							cols[k] = value;
						}
					}
				}
			}
		}
		catch (Exception)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x0004708C File Offset: 0x0004528C
	private static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float f)
	{
		a.x += (b.x - a.x) * f;
		a.y += (b.y - a.y) * f;
		a.z += (b.z - a.z) * f;
		return a;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00009987 File Offset: 0x00007B87
	private static float LerpUnclamped(float a, float b, float f)
	{
		return a + (b - a) * f;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00009990 File Offset: 0x00007B90
	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.mLabel.MarkAsChanged();
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x000470FC File Offset: 0x000452FC
	private void SetLetterOrder(int letterCount)
	{
		if (letterCount == 0)
		{
			this.mLetter = null;
			this.mLetterOrder = null;
			return;
		}
		this.mLetterOrder = new int[letterCount];
		this.mLetter = new TweenLetters.LetterProperties[letterCount];
		for (int i = 0; i < letterCount; i++)
		{
			this.mLetterOrder[i] = ((this.mCurrent.animationOrder != TweenLetters.AnimationLetterOrder.Reverse) ? i : (letterCount - 1 - i));
			int num = this.mLetterOrder[i];
			this.mLetter[num] = new TweenLetters.LetterProperties();
			this.mLetter[num].offset = new Vector2(UnityEngine.Random.Range(-this.mCurrent.offsetRange.x, this.mCurrent.offsetRange.x), UnityEngine.Random.Range(-this.mCurrent.offsetRange.y, this.mCurrent.offsetRange.y));
		}
		if (this.mCurrent.animationOrder == TweenLetters.AnimationLetterOrder.Random)
		{
			System.Random random = new System.Random();
			int j = letterCount;
			while (j > 1)
			{
				int num2 = random.Next(--j + 1);
				int num3 = this.mLetterOrder[num2];
				this.mLetterOrder[num2] = this.mLetterOrder[j];
				this.mLetterOrder[j] = num3;
			}
		}
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0004723C File Offset: 0x0004543C
	private void GetLetterDuration(int letterCount)
	{
		if (this.mCurrent.randomDurations)
		{
			for (int i = 0; i < this.mLetter.Length; i++)
			{
				this.mLetter[i].start = UnityEngine.Random.Range(0f, this.mCurrent.randomness.x * this.duration);
				float num = UnityEngine.Random.Range(this.mCurrent.randomness.y * this.duration, this.duration);
				this.mLetter[i].duration = num - this.mLetter[i].start;
			}
		}
		else
		{
			float num2 = this.duration / (float)letterCount;
			float num3 = 1f - this.mCurrent.overlap;
			float num4 = num2 * (float)letterCount * num3;
			float duration = this.ScaleRange(num2, num4 + num2 * this.mCurrent.overlap, this.duration);
			float num5 = 0f;
			for (int j = 0; j < this.mLetter.Length; j++)
			{
				int num6 = this.mLetterOrder[j];
				this.mLetter[num6].start = num5;
				this.mLetter[num6].duration = duration;
				num5 += this.mLetter[num6].duration * num3;
			}
		}
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x0000999D File Offset: 0x00007B9D
	private float ScaleRange(float value, float baseMax, float limitMax)
	{
		return limitMax * value / baseMax;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0004738C File Offset: 0x0004558C
	private static Vector3 GetCenter(List<Vector3> verts, int firstVert, int length)
	{
		Vector3 a = verts[firstVert];
		for (int i = firstVert + 1; i < firstVert + length; i++)
		{
			a += verts[i];
		}
		return a / (float)length;
	}

	// Token: 0x040004C5 RID: 1221
	public TweenLetters.AnimationProperties hoverOver;

	// Token: 0x040004C6 RID: 1222
	public TweenLetters.AnimationProperties hoverOut;

	// Token: 0x040004C7 RID: 1223
	private UILabel mLabel;

	// Token: 0x040004C8 RID: 1224
	private int mVertexCount = -1;

	// Token: 0x040004C9 RID: 1225
	private int[] mLetterOrder;

	// Token: 0x040004CA RID: 1226
	private TweenLetters.LetterProperties[] mLetter;

	// Token: 0x040004CB RID: 1227
	private TweenLetters.AnimationProperties mCurrent;

	// Token: 0x020000D5 RID: 213
	[DoNotObfuscateNGUI]
	public enum AnimationLetterOrder
	{
		// Token: 0x040004CD RID: 1229
		Forward,
		// Token: 0x040004CE RID: 1230
		Reverse,
		// Token: 0x040004CF RID: 1231
		Random
	}

	// Token: 0x020000D6 RID: 214
	private class LetterProperties
	{
		// Token: 0x040004D0 RID: 1232
		public float start;

		// Token: 0x040004D1 RID: 1233
		public float duration;

		// Token: 0x040004D2 RID: 1234
		public Vector2 offset;
	}

	// Token: 0x020000D7 RID: 215
	[Serializable]
	public class AnimationProperties
	{
		// Token: 0x040004D3 RID: 1235
		public TweenLetters.AnimationLetterOrder animationOrder = TweenLetters.AnimationLetterOrder.Random;

		// Token: 0x040004D4 RID: 1236
		[Range(0f, 1f)]
		public float overlap = 0.5f;

		// Token: 0x040004D5 RID: 1237
		public bool randomDurations;

		// Token: 0x040004D6 RID: 1238
		[MinMaxRange(0f, 1f)]
		public Vector2 randomness = new Vector2(0.25f, 0.75f);

		// Token: 0x040004D7 RID: 1239
		public Vector2 offsetRange = Vector2.zero;

		// Token: 0x040004D8 RID: 1240
		public Vector3 pos = Vector3.zero;

		// Token: 0x040004D9 RID: 1241
		public Vector3 rot = Vector3.zero;

		// Token: 0x040004DA RID: 1242
		public Vector3 scale = Vector3.one;

		// Token: 0x040004DB RID: 1243
		public float alpha = 1f;
	}
}
