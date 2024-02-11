using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000090 RID: 144
[Serializable]
public class BMFont
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x060003ED RID: 1005 RVA: 0x000079EE File Offset: 0x00005BEE
	public bool isValid
	{
		get
		{
			return this.mSaved.Count > 0;
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x000079FE File Offset: 0x00005BFE
	// (set) Token: 0x060003EF RID: 1007 RVA: 0x00007A06 File Offset: 0x00005C06
	public int charSize
	{
		get
		{
			return this.mSize;
		}
		set
		{
			this.mSize = value;
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00007A0F File Offset: 0x00005C0F
	// (set) Token: 0x060003F1 RID: 1009 RVA: 0x00007A17 File Offset: 0x00005C17
	public int baseOffset
	{
		get
		{
			return this.mBase;
		}
		set
		{
			this.mBase = value;
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x060003F2 RID: 1010 RVA: 0x00007A20 File Offset: 0x00005C20
	// (set) Token: 0x060003F3 RID: 1011 RVA: 0x00007A28 File Offset: 0x00005C28
	public int texWidth
	{
		get
		{
			return this.mWidth;
		}
		set
		{
			this.mWidth = value;
		}
	}

	// Token: 0x17000051 RID: 81
	// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00007A31 File Offset: 0x00005C31
	// (set) Token: 0x060003F5 RID: 1013 RVA: 0x00007A39 File Offset: 0x00005C39
	public int texHeight
	{
		get
		{
			return this.mHeight;
		}
		set
		{
			this.mHeight = value;
		}
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00007A42 File Offset: 0x00005C42
	public int glyphCount
	{
		get
		{
			return (!this.isValid) ? 0 : this.mSaved.Count;
		}
	}

	// Token: 0x17000053 RID: 83
	// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00007A60 File Offset: 0x00005C60
	// (set) Token: 0x060003F8 RID: 1016 RVA: 0x00007A68 File Offset: 0x00005C68
	public string spriteName
	{
		get
		{
			return this.mSpriteName;
		}
		set
		{
			this.mSpriteName = value;
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00007A71 File Offset: 0x00005C71
	public List<BMGlyph> glyphs
	{
		get
		{
			return this.mSaved;
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x000315A0 File Offset: 0x0002F7A0
	public BMGlyph GetGlyph(int index, bool createIfMissing)
	{
		BMGlyph bmglyph = null;
		if (this.mDict.Count == 0)
		{
			int i = 0;
			int count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph2 = this.mSaved[i];
				this.mDict.Add(bmglyph2.index, bmglyph2);
				i++;
			}
		}
		if (!this.mDict.TryGetValue(index, out bmglyph) && createIfMissing)
		{
			bmglyph = new BMGlyph();
			bmglyph.index = index;
			this.mSaved.Add(bmglyph);
			this.mDict.Add(index, bmglyph);
		}
		return bmglyph;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00007A79 File Offset: 0x00005C79
	public BMGlyph GetGlyph(int index)
	{
		return this.GetGlyph(index, false);
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00007A83 File Offset: 0x00005C83
	public void Clear()
	{
		this.mDict.Clear();
		this.mSaved.Clear();
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0003163C File Offset: 0x0002F83C
	public void Trim(int xMin, int yMin, int xMax, int yMax)
	{
		if (this.isValid)
		{
			int i = 0;
			int count = this.mSaved.Count;
			while (i < count)
			{
				BMGlyph bmglyph = this.mSaved[i];
				if (bmglyph != null)
				{
					bmglyph.Trim(xMin, yMin, xMax, yMax);
				}
				i++;
			}
		}
	}

	// Token: 0x04000323 RID: 803
	[HideInInspector]
	[SerializeField]
	private int mSize = 16;

	// Token: 0x04000324 RID: 804
	[HideInInspector]
	[SerializeField]
	private int mBase;

	// Token: 0x04000325 RID: 805
	[HideInInspector]
	[SerializeField]
	private int mWidth;

	// Token: 0x04000326 RID: 806
	[HideInInspector]
	[SerializeField]
	private int mHeight;

	// Token: 0x04000327 RID: 807
	[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	// Token: 0x04000328 RID: 808
	[SerializeField]
	[HideInInspector]
	private List<BMGlyph> mSaved = new List<BMGlyph>();

	// Token: 0x04000329 RID: 809
	private Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();
}
