using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class UIGeometry
{
	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000605 RID: 1541 RVA: 0x00008E73 File Offset: 0x00007073
	public bool hasVertices
	{
		get
		{
			return this.verts.Count > 0;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000606 RID: 1542 RVA: 0x00008E83 File Offset: 0x00007083
	public bool hasTransformed
	{
		get
		{
			return this.mRtpVerts != null && this.mRtpVerts.Count > 0 && this.mRtpVerts.Count == this.verts.Count;
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00008EBC File Offset: 0x000070BC
	public void Clear()
	{
		this.verts.Clear();
		this.uvs.Clear();
		this.cols.Clear();
		this.mRtpVerts.Clear();
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000432C4 File Offset: 0x000414C4
	public void ApplyTransform(Matrix4x4 widgetToPanel, bool generateNormals = true)
	{
		if (this.verts.Count > 0)
		{
			this.mRtpVerts.Clear();
			int i = 0;
			int count = this.verts.Count;
			while (i < count)
			{
				this.mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(this.verts[i]));
				i++;
			}
			if (generateNormals)
			{
				this.mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
				Vector3 normalized = widgetToPanel.MultiplyVector(Vector3.right).normalized;
				this.mRtpTan = new Vector4(normalized.x, normalized.y, normalized.z, -1f);
			}
		}
		else
		{
			this.mRtpVerts.Clear();
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00043394 File Offset: 0x00041594
	public void WriteToBuffers(List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2)
	{
		nProfiler.BeginSample("UIGeometry.WriteToBuffers");
		if (this.mRtpVerts != null && this.mRtpVerts.Count > 0)
		{
			if (n == null)
			{
				nProfiler.BeginSample("1");
				int i = 0;
				int count = this.mRtpVerts.Count;
				while (i < count)
				{
					v.Add(this.mRtpVerts[i]);
					u.Add(this.uvs[i]);
					c.Add(this.cols[i]);
					i++;
				}
				nProfiler.EndSample();
			}
			else
			{
				nProfiler.BeginSample("2");
				int j = 0;
				int count2 = this.mRtpVerts.Count;
				while (j < count2)
				{
					v.Add(this.mRtpVerts[j]);
					u.Add(this.uvs[j]);
					c.Add(this.cols[j]);
					n.Add(this.mRtpNormal);
					t.Add(this.mRtpTan);
					j++;
				}
				nProfiler.EndSample();
			}
			if (u2 != null)
			{
				nProfiler.BeginSample("3");
				Vector4 zero = Vector4.zero;
				int k = 0;
				int count3 = this.verts.Count;
				while (k < count3)
				{
					zero.x = this.verts[k].x;
					zero.y = this.verts[k].y;
					u2.Add(zero);
					k++;
				}
				nProfiler.EndSample();
			}
			nProfiler.BeginSample("4");
			if (this.onCustomWrite != null)
			{
				this.onCustomWrite(v, u, c, n, t, u2);
			}
			nProfiler.EndSample();
		}
		nProfiler.EndSample();
	}

	// Token: 0x0400043B RID: 1083
	public List<Vector3> verts = new List<Vector3>();

	// Token: 0x0400043C RID: 1084
	public List<Vector2> uvs = new List<Vector2>();

	// Token: 0x0400043D RID: 1085
	public List<Color> cols = new List<Color>();

	// Token: 0x0400043E RID: 1086
	public UIGeometry.OnCustomWrite onCustomWrite;

	// Token: 0x0400043F RID: 1087
	private List<Vector3> mRtpVerts = new List<Vector3>();

	// Token: 0x04000440 RID: 1088
	private Vector3 mRtpNormal;

	// Token: 0x04000441 RID: 1089
	private Vector4 mRtpTan;

	// Token: 0x020000BF RID: 191
	// (Invoke) Token: 0x0600060B RID: 1547
	public delegate void OnCustomWrite(List<Vector3> v, List<Vector2> u, List<Color> c, List<Vector3> n, List<Vector4> t, List<Vector4> u2);
}
