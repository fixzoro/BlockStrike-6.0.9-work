using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000108 RID: 264
[ExecuteInEditMode]
public class UIFont : MonoBehaviour, INGUIFont
{
	// Token: 0x17000140 RID: 320
	// (get) Token: 0x060008B5 RID: 2229 RVA: 0x0004FCD8 File Offset: 0x0004DED8
	// (set) Token: 0x060008B6 RID: 2230 RVA: 0x0004FD04 File Offset: 0x0004DF04
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

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x060008B7 RID: 2231 RVA: 0x0004FD34 File Offset: 0x0004DF34
	// (set) Token: 0x060008B8 RID: 2232 RVA: 0x0004FD78 File Offset: 0x0004DF78
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

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x060008B9 RID: 2233 RVA: 0x0004FDB8 File Offset: 0x0004DFB8
	// (set) Token: 0x060008BA RID: 2234 RVA: 0x0004FDFC File Offset: 0x0004DFFC
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

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x060008BB RID: 2235 RVA: 0x0004FE3C File Offset: 0x0004E03C
	public bool hasSymbols
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? (this.mSymbols != null && this.mSymbols.Count != 0) : replacement.hasSymbols;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x060008BC RID: 2236 RVA: 0x0004FE80 File Offset: 0x0004E080
	// (set) Token: 0x060008BD RID: 2237 RVA: 0x0004FEAC File Offset: 0x0004E0AC
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

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x060008BE RID: 2238 RVA: 0x0004FEDC File Offset: 0x0004E0DC
	// (set) Token: 0x060008BF RID: 2239 RVA: 0x0004FF08 File Offset: 0x0004E108
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

	// Token: 0x060008C0 RID: 2240 RVA: 0x0004FF94 File Offset: 0x0004E194
	public UISpriteData GetSprite(string spriteName)
	{
		INGUIAtlas atlas = this.atlas;
		if (atlas != null)
		{
			return atlas.GetSprite(spriteName);
		}
		return null;
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x060008C1 RID: 2241 RVA: 0x0004FFB8 File Offset: 0x0004E1B8
	// (set) Token: 0x060008C2 RID: 2242 RVA: 0x00050070 File Offset: 0x0004E270
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

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x060008C3 RID: 2243 RVA: 0x0000A70C File Offset: 0x0000890C
	[Obsolete("Use premultipliedAlphaShader instead")]
	public bool premultipliedAlpha
	{
		get
		{
			return this.premultipliedAlphaShader;
		}
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060008C4 RID: 2244 RVA: 0x000500BC File Offset: 0x0004E2BC
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

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x060008C5 RID: 2245 RVA: 0x00050158 File Offset: 0x0004E358
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

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x000501EC File Offset: 0x0004E3EC
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

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x060008C7 RID: 2247 RVA: 0x00050234 File Offset: 0x0004E434
	// (set) Token: 0x060008C8 RID: 2248 RVA: 0x00050298 File Offset: 0x0004E498
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

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x060008C9 RID: 2249 RVA: 0x000502E8 File Offset: 0x0004E4E8
	// (set) Token: 0x060008CA RID: 2250 RVA: 0x00050318 File Offset: 0x0004E518
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

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x060008CB RID: 2251 RVA: 0x0000A714 File Offset: 0x00008914
	public bool isValid
	{
		get
		{
			return this.mDynamicFont != null || this.mFont.isValid;
		}
	}

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x060008CC RID: 2252 RVA: 0x0000A735 File Offset: 0x00008935
	// (set) Token: 0x060008CD RID: 2253 RVA: 0x0000A73D File Offset: 0x0000893D
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

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x060008CE RID: 2254 RVA: 0x00050368 File Offset: 0x0004E568
	// (set) Token: 0x060008CF RID: 2255 RVA: 0x000503B4 File Offset: 0x0004E5B4
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

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x060008D0 RID: 2256 RVA: 0x000503E4 File Offset: 0x0004E5E4
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

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0000A746 File Offset: 0x00008946
	// (set) Token: 0x060008D2 RID: 2258 RVA: 0x000504D0 File Offset: 0x0004E6D0
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

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x060008D3 RID: 2259 RVA: 0x00050560 File Offset: 0x0004E760
	public bool isDynamic
	{
		get
		{
			INGUIFont replacement = this.replacement;
			return (replacement == null) ? (this.mDynamicFont != null) : replacement.isDynamic;
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x060008D4 RID: 2260 RVA: 0x00050594 File Offset: 0x0004E794
	// (set) Token: 0x060008D5 RID: 2261 RVA: 0x000505C0 File Offset: 0x0004E7C0
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

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x060008D6 RID: 2262 RVA: 0x0005061C File Offset: 0x0004E81C
	// (set) Token: 0x060008D7 RID: 2263 RVA: 0x00050648 File Offset: 0x0004E848
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

	// Token: 0x060008D8 RID: 2264 RVA: 0x00050688 File Offset: 0x0004E888
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

	// Token: 0x060008D9 RID: 2265 RVA: 0x0005078C File Offset: 0x0004E98C
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

	// Token: 0x060008DA RID: 2266 RVA: 0x000507C4 File Offset: 0x0004E9C4
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

	// Token: 0x060008DB RID: 2267 RVA: 0x0005088C File Offset: 0x0004EA8C
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

	// Token: 0x060008DC RID: 2268 RVA: 0x00050988 File Offset: 0x0004EB88
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

	// Token: 0x060008DD RID: 2269 RVA: 0x000509F8 File Offset: 0x0004EBF8
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

	// Token: 0x060008DE RID: 2270 RVA: 0x00050AB0 File Offset: 0x0004ECB0
	public void AddSymbol(string sequence, string spriteName)
	{
		BMSymbol symbol = this.GetSymbol(sequence, true);
		symbol.spriteName = spriteName;
		this.MarkAsChanged();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00050AD4 File Offset: 0x0004ECD4
	public void RemoveSymbol(string sequence)
	{
		BMSymbol symbol = this.GetSymbol(sequence, false);
		if (symbol != null)
		{
			this.symbols.Remove(symbol);
		}
		this.MarkAsChanged();
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00050B04 File Offset: 0x0004ED04
	public void RenameSymbol(string before, string after)
	{
		BMSymbol symbol = this.GetSymbol(before, false);
		if (symbol != null)
		{
			symbol.sequence = after;
		}
		this.MarkAsChanged();
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00050B30 File Offset: 0x0004ED30
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

	// Token: 0x04000611 RID: 1553
	[SerializeField]
	[HideInInspector]
	private Material mMat;

	// Token: 0x04000612 RID: 1554
	[HideInInspector]
	[SerializeField]
	private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04000613 RID: 1555
	[SerializeField]
	[HideInInspector]
	private BMFont mFont = new BMFont();

	// Token: 0x04000614 RID: 1556
	[SerializeField]
	[HideInInspector]
	private UnityEngine.Object mAtlas;

	// Token: 0x04000615 RID: 1557
	[HideInInspector]
	[SerializeField]
	private UnityEngine.Object mReplacement;

	// Token: 0x04000616 RID: 1558
	[SerializeField]
	[HideInInspector]
	private List<BMSymbol> mSymbols = new List<BMSymbol>();

	// Token: 0x04000617 RID: 1559
	[HideInInspector]
	[SerializeField]
	private Font mDynamicFont;

	// Token: 0x04000618 RID: 1560
	[SerializeField]
	[HideInInspector]
	private int mDynamicFontSize = 16;

	// Token: 0x04000619 RID: 1561
	[SerializeField]
	[HideInInspector]
	private FontStyle mDynamicFontStyle;

	// Token: 0x0400061A RID: 1562
	[NonSerialized]
	private UISpriteData mSprite;

	// Token: 0x0400061B RID: 1563
	[NonSerialized]
	private int mPMA = -1;

	// Token: 0x0400061C RID: 1564
	[NonSerialized]
	private int mPacked = -1;
}
