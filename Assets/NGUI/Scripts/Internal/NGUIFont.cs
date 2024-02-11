using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E6 RID: 230
[ExecuteInEditMode]
public class NGUIFont : ScriptableObject, INGUIFont
{
	// Token: 0x170000FD RID: 253
	// (get) Token: 0x060007A8 RID: 1960 RVA: 0x00048C14 File Offset: 0x00046E14
	// (set) Token: 0x060007A9 RID: 1961 RVA: 0x00048C40 File Offset: 0x00046E40
	public BMFont bmFont
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? this.mFont : replacement.bmFont;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.bmFont = value;
			}
			else
			{
				this.mFont = value;
			}
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x060007AA RID: 1962 RVA: 0x00048C70 File Offset: 0x00046E70
	// (set) Token: 0x060007AB RID: 1963 RVA: 0x00048CB4 File Offset: 0x00046EB4
	public int texWidth
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? ((this.mFont == null) ? 1 : this.mFont.texWidth) : replacement.texWidth;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.texWidth = value;
			}
			else if (this.mFont != null)
			{
				this.mFont.texWidth = value;
			}
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060007AC RID: 1964 RVA: 0x00048CF4 File Offset: 0x00046EF4
	// (set) Token: 0x060007AD RID: 1965 RVA: 0x00048D38 File Offset: 0x00046F38
	public int texHeight
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? ((this.mFont == null) ? 1 : this.mFont.texHeight) : replacement.texHeight;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.texHeight = value;
			}
			else if (this.mFont != null)
			{
				this.mFont.texHeight = value;
			}
		}
	}

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x060007AE RID: 1966 RVA: 0x00048D78 File Offset: 0x00046F78
	public bool hasSymbols
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? (this.mSymbols != null && this.mSymbols.Count != 0) : replacement.hasSymbols;
		}
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x060007AF RID: 1967 RVA: 0x00048DBC File Offset: 0x00046FBC
	// (set) Token: 0x060007B0 RID: 1968 RVA: 0x00048DE8 File Offset: 0x00046FE8
	public List<BMSymbol> symbols
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? this.mSymbols : replacement.symbols;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.symbols = value;
			}
			else
			{
				this.mSymbols = value;
			}
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060007B1 RID: 1969 RVA: 0x00048E18 File Offset: 0x00047018
	// (set) Token: 0x060007B2 RID: 1970 RVA: 0x00048E44 File Offset: 0x00047044
	public INGUIAtlas atlas
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.atlas;
			}
			return this.mAtlas as INGUIAtlas;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.atlas = value;
			}
			else if (this.mAtlas as INGUIAtlas != value)
			{
				this.mPMA = -1;
				this.mAtlas = (value as UnityEngine.Object);
				if (value != null)
				{
					this.mMat = value.spriteMaterial;
					if (this.sprite != null)
					{
						this.mUVRect = this.uvRect;
					}
				}
				else
				{
					this.mAtlas = null;
					this.mMat = null;
				}
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00048ED0 File Offset: 0x000470D0
	public UISpriteData GetSprite(string spriteName)
	{
		INGUIAtlas atlas = this.atlas;
		if (atlas != null)
		{
			return atlas.GetSprite(spriteName);
		}
		return null;
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x060007B4 RID: 1972 RVA: 0x00048EF4 File Offset: 0x000470F4
	// (set) Token: 0x060007B5 RID: 1973 RVA: 0x00048FAC File Offset: 0x000471AC
	public Material material
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.material;
			}
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				return inguiatlas.spriteMaterial;
			}
			if (this.mMat != null)
			{
				if (this.mDynamicFont != null && this.mMat != this.mDynamicFont.material)
				{
					this.mMat.mainTexture = this.mDynamicFont.material.mainTexture;
				}
				return this.mMat;
			}
			if (this.mDynamicFont != null)
			{
				return this.mDynamicFont.material;
			}
			return null;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.material = value;
			}
			else if (this.mMat != value)
			{
				this.mPMA = -1;
				this.mMat = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060007B6 RID: 1974 RVA: 0x00009FF8 File Offset: 0x000081F8
	[Obsolete("Use premultipliedAlphaShader instead")]
	public bool premultipliedAlpha
	{
		get
		{
			return this.premultipliedAlphaShader;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00048FF8 File Offset: 0x000471F8
	public bool premultipliedAlphaShader
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.premultipliedAlphaShader;
			}
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (inguiatlas != null)
			{
				return inguiatlas.premultipliedAlpha;
			}
			if (this.mPMA == -1)
			{
				Material material = this.material;
				this.mPMA = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x060007B8 RID: 1976 RVA: 0x00049094 File Offset: 0x00047294
	public bool packedFontShader
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.packedFontShader;
			}
			if (this.mAtlas != null)
			{
				return false;
			}
			if (this.mPacked == -1)
			{
				Material material = this.material;
				this.mPacked = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Packed")) ? 0 : 1);
			}
			return this.mPacked == 1;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x060007B9 RID: 1977 RVA: 0x00049128 File Offset: 0x00047328
	public Texture2D texture
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.texture;
			}
			Material material = this.material;
			return (!(material != null)) ? null : (material.mainTexture as Texture2D);
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x060007BA RID: 1978 RVA: 0x00049170 File Offset: 0x00047370
	// (set) Token: 0x060007BB RID: 1979 RVA: 0x000491D4 File Offset: 0x000473D4
	public Rect uvRect
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.uvRect;
			}
			return (!(this.mAtlas != null) || this.sprite == null) ? new Rect(0f, 0f, 1f, 1f) : this.mUVRect;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.uvRect = value;
			}
			else if (this.sprite == null && this.mUVRect != value)
			{
				this.mUVRect = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x060007BC RID: 1980 RVA: 0x00049224 File Offset: 0x00047424
	// (set) Token: 0x060007BD RID: 1981 RVA: 0x00049254 File Offset: 0x00047454
	public string spriteName
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? this.mFont.spriteName : replacement.spriteName;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.spriteName = value;
			}
			else if (this.mFont.spriteName != value)
			{
				this.mFont.spriteName = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x060007BE RID: 1982 RVA: 0x0000A000 File Offset: 0x00008200
	public bool isValid
	{
		get
		{
			return this.mDynamicFont != null || this.mFont.isValid;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060007BF RID: 1983 RVA: 0x0000A021 File Offset: 0x00008221
	// (set) Token: 0x060007C0 RID: 1984 RVA: 0x0000A029 File Offset: 0x00008229
	[Obsolete("Use defaultSize instead")]
	public int size
	{
		get
		{
			return this.defaultSize;
		}
		set
		{
			this.defaultSize = value;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x060007C1 RID: 1985 RVA: 0x000492A4 File Offset: 0x000474A4
	// (set) Token: 0x060007C2 RID: 1986 RVA: 0x000492F0 File Offset: 0x000474F0
	public int defaultSize
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.defaultSize;
			}
			if (this.isDynamic || this.mFont == null)
			{
				return this.mDynamicFontSize;
			}
			return this.mFont.charSize;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.defaultSize = value;
			}
			else
			{
				this.mDynamicFontSize = value;
			}
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x060007C3 RID: 1987 RVA: 0x00049320 File Offset: 0x00047520
	public UISpriteData sprite
	{
		get
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.sprite;
			}
			INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
			if (this.mSprite == null && inguiatlas != null && this.mFont != null && !string.IsNullOrEmpty(this.mFont.spriteName))
			{
				this.mSprite = inguiatlas.GetSprite(this.mFont.spriteName);
				if (this.mSprite == null)
				{
					this.mSprite = inguiatlas.GetSprite(base.name);
				}
				if (this.mSprite == null)
				{
					this.mFont.spriteName = null;
				}
				else
				{
					this.UpdateUVRect();
				}
				int i = 0;
				int count = this.mSymbols.Count;
				while (i < count)
				{
					this.symbols[i].MarkAsChanged();
					i++;
				}
			}
			return this.mSprite;
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0000A032 File Offset: 0x00008232
	// (set) Token: 0x060007C5 RID: 1989 RVA: 0x0004940C File Offset: 0x0004760C
	public INGUIFont replacement
	{
		get
		{
			if (this.mReplacement == null)
			{
				return null;
			}
			return this.mReplacement as INGUIFont;
		}
		set
		{
			INGUIFont inguifont = value;
			if (inguifont == this)
			{
				inguifont = null;
			}
			if (this.mReplacement as INGUIFont != inguifont)
			{
				if (inguifont != null && inguifont.replacement == this)
				{
					inguifont.replacement = null;
				}
				if (this.mReplacement != null)
				{
					this.MarkAsChanged();
				}
				this.mReplacement = (inguifont as UnityEngine.Object);
				if (inguifont != null)
				{
					this.mPMA = -1;
					this.mMat = null;
					this.mFont = null;
					this.mDynamicFont = null;
				}
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0004949C File Offset: 0x0004769C
	public bool isDynamic
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? (this.mDynamicFont != null) : replacement.isDynamic;
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x060007C7 RID: 1991 RVA: 0x000494D0 File Offset: 0x000476D0
	// (set) Token: 0x060007C8 RID: 1992 RVA: 0x000494FC File Offset: 0x000476FC
	public Font dynamicFont
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? this.mDynamicFont : replacement.dynamicFont;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.dynamicFont = value;
			}
			else if (this.mDynamicFont != value)
			{
				if (this.mDynamicFont != null)
				{
					this.material = null;
				}
				this.mDynamicFont = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x060007C9 RID: 1993 RVA: 0x00049558 File Offset: 0x00047758
	// (set) Token: 0x060007CA RID: 1994 RVA: 0x00049584 File Offset: 0x00047784
	public FontStyle dynamicFontStyle
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? this.mDynamicFontStyle : replacement.dynamicFontStyle;
		}
		set
		{
			INGUIFont replacement = this.replacement;
			if (replacement != null)
			{
				replacement.dynamicFontStyle = value;
			}
			else if (this.mDynamicFontStyle != value)
			{
				this.mDynamicFontStyle = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x000495C4 File Offset: 0x000477C4
	private void Trim()
	{
		Texture x = null;
		INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
		if (inguiatlas != null)
		{
			x = inguiatlas.texture;
		}
		if (x != null && this.mSprite != null)
		{
			Rect rect = NGUIMath.ConvertToPixels(this.mUVRect, this.texture.width, this.texture.height, true);
			Rect rect2 = new Rect((float)this.mSprite.x, (float)this.mSprite.y, (float)this.mSprite.width, (float)this.mSprite.height);
			int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
			int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
			int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
			int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
			this.mFont.Trim(xMin, yMin, xMax, yMax);
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x000496C8 File Offset: 0x000478C8
	public bool References(INGUIFont font)
	{
		if (font == null)
		{
			return false;
		}
		if (font == this)
		{
			return true;
		}
		INGUIFont replacement = this.replacement;
		return replacement != null && replacement.References(font);
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00049700 File Offset: 0x00047900
	public void MarkAsChanged()
	{
		INGUIFont replacement = this.replacement;
		if (replacement != null)
		{
			replacement.MarkAsChanged();
		}
		this.mSprite = null;
		UILabel[] array = NGUITools.FindActive<UILabel>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			UILabel uilabel = array[i];
			if (uilabel.enabled && NGUITools.GetActive(uilabel.gameObject) && NGUITools.CheckIfRelated(this, uilabel.bitmapFont))
			{
				INGUIFont bitmapFont = uilabel.bitmapFont;
				uilabel.bitmapFont = null;
				uilabel.bitmapFont = bitmapFont;
			}
			i++;
		}
		int j = 0;
		int count = this.symbols.Count;
		while (j < count)
		{
			this.symbols[j].MarkAsChanged();
			j++;
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x000497C8 File Offset: 0x000479C8
	public void UpdateUVRect()
	{
		if (this.mAtlas == null)
		{
			return;
		}
		Texture texture = null;
		INGUIAtlas inguiatlas = this.mAtlas as INGUIAtlas;
		if (inguiatlas != null)
		{
			texture = inguiatlas.texture;
		}
		if (texture != null)
		{
			this.mUVRect = new Rect((float)(this.mSprite.x - this.mSprite.paddingLeft), (float)(this.mSprite.y - this.mSprite.paddingTop), (float)(this.mSprite.width + this.mSprite.paddingLeft + this.mSprite.paddingRight), (float)(this.mSprite.height + this.mSprite.paddingTop + this.mSprite.paddingBottom));
			this.mUVRect = NGUIMath.ConvertToTexCoords(this.mUVRect, texture.width, texture.height);
			if (this.mSprite.hasPadding)
			{
				this.Trim();
			}
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x000498C4 File Offset: 0x00047AC4
	private BMSymbol GetSymbol(string sequence, bool createIfMissing)
	{
		int i = 0;
		int count = this.mSymbols.Count;
		while (i < count)
		{
			BMSymbol bmsymbol = this.mSymbols[i];
			if (bmsymbol.sequence == sequence)
			{
				return bmsymbol;
			}
			i++;
		}
		if (createIfMissing)
		{
			BMSymbol bmsymbol2 = new BMSymbol();
			bmsymbol2.sequence = sequence;
			this.mSymbols.Add(bmsymbol2);
			return bmsymbol2;
		}
		return null;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x00049934 File Offset: 0x00047B34
	public BMSymbol MatchSymbol(string text, int offset, int textLength)
	{
		int count = this.mSymbols.Count;
		if (count == 0)
		{
			return null;
		}
		textLength -= offset;
		for (int i = 0; i < count; i++)
		{
			BMSymbol bmsymbol = this.mSymbols[i];
			int length = bmsymbol.length;
			if (length != 0 && textLength >= length)
			{
				bool flag = true;
				for (int j = 0; j < length; j++)
				{
					if (text[offset + j] != bmsymbol.sequence[j])
					{
						flag = false;
						break;
					}
				}
				if (flag && bmsymbol.Validate(this.atlas))
				{
					return bmsymbol;
				}
			}
		}
		return null;
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x000499EC File Offset: 0x00047BEC
	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = this.GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		this.MarkAsChanged();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x00049A10 File Offset: 0x00047C10
	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = this.GetSymbol(sequence, false);
		if (symbol != null)
		{
			this.symbols.Remove(symbol);
		}
		this.MarkAsChanged();
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00049A40 File Offset: 0x00047C40
	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = this.GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		this.MarkAsChanged();
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00049A6C File Offset: 0x00047C6C
	public bool UsesSprite(string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			if (s.Equals(this.spriteName))
			{
				return true;
			}
			int i = 0;
			int count = this.symbols.Count;
			while (i < count)
			{
				BMSymbol bmsymbol = this.symbols[i];
				if (s.Equals(bmsymbol.spriteName))
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	// Token: 0x04000525 RID: 1317
	[HideInInspector]
	[SerializeField]
	private Material mMat;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	[HideInInspector]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	[HideInInspector]
	private BMFont mFont = new BMFont();

	// Token: 0x04000528 RID: 1320
	[HideInInspector]
	[SerializeField]
	private UnityEngine.Object mAtlas;

	// Token: 0x04000529 RID: 1321
	[SerializeField]
	[HideInInspector]
	private UnityEngine.Object mReplacement;

	// Token: 0x0400052A RID: 1322
	[HideInInspector]
	[SerializeField]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	// Token: 0x0400052B RID: 1323
	[SerializeField]
	[HideInInspector]
	private Font mDynamicFont;

	// Token: 0x0400052C RID: 1324
	[HideInInspector]
	[SerializeField]
	private int mDynamicFontSize = 16;

	// Token: 0x0400052D RID: 1325
	[SerializeField]
	[HideInInspector]
	private FontStyle mDynamicFontStyle;

	// Token: 0x0400052E RID: 1326
	[NonSerialized]
	private UISpriteData mSprite;

	// Token: 0x0400052F RID: 1327
	[NonSerialized]
	private int mPMA = -1;

	// Token: 0x04000530 RID: 1328
	[NonSerialized]
	private int mPacked = -1;
}
