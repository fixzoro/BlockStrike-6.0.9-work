using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class UIAtlas : MonoBehaviour, INGUIAtlas
{
	// Token: 0x1700011D RID: 285
	// (get) Token: 0x060007F8 RID: 2040 RVA: 0x0004AD2C File Offset: 0x00048F2C
	// (set) Token: 0x060007F9 RID: 2041 RVA: 0x0004AD58 File Offset: 0x00048F58
	public Material spriteMaterial
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			return (replacement == null) ? this.material : replacement.spriteMaterial;
		}
		set
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				replacement.spriteMaterial = value;
			}
			else if (this.material == null)
			{
				this.mPMA = 0;
				this.material = value;
			}
			else
			{
				this.MarkAsChanged();
				this.mPMA = -1;
				this.material = value;
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x060007FA RID: 2042 RVA: 0x0004ADBC File Offset: 0x00048FBC
	public bool premultipliedAlpha
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.premultipliedAlpha;
			}
			if (this.mPMA == -1)
			{
				Material spriteMaterial = this.spriteMaterial;
				this.mPMA = ((!(spriteMaterial != null) || !(spriteMaterial.shader != null) || !spriteMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
			}
			return this.mPMA == 1;
		}
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x060007FB RID: 2043 RVA: 0x0004AE40 File Offset: 0x00049040
	// (set) Token: 0x060007FC RID: 2044 RVA: 0x0004AE80 File Offset: 0x00049080
	public List<UISpriteData> spriteList
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.spriteList;
			}
			if (this.mSprites.Count == 0)
			{
				this.Upgrade();
			}
			return this.mSprites;
		}
		set
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				replacement.spriteList = value;
			}
			else
			{
				this.mSprites = value;
			}
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x060007FD RID: 2045 RVA: 0x0004AEB0 File Offset: 0x000490B0
	public Texture texture
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			return (replacement == null) ? ((!(this.material != null)) ? null : this.material.mainTexture) : replacement.texture;
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x060007FE RID: 2046 RVA: 0x0004AEF8 File Offset: 0x000490F8
	// (set) Token: 0x060007FF RID: 2047 RVA: 0x0004AF24 File Offset: 0x00049124
	public float pixelSize
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			return (replacement == null) ? this.mPixelSize : replacement.pixelSize;
		}
		set
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				replacement.pixelSize = value;
			}
			else
			{
				float num = Mathf.Clamp(value, 0.25f, 4f);
				if (this.mPixelSize != num)
				{
					this.mPixelSize = num;
					this.MarkAsChanged();
				}
			}
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000800 RID: 2048 RVA: 0x0000A2E6 File Offset: 0x000084E6
	// (set) Token: 0x06000801 RID: 2049 RVA: 0x0004AF74 File Offset: 0x00049174
	public INGUIAtlas replacement
	{
		get
		{
			return this.mReplacement as INGUIAtlas;
		}
		set
		{
			INGUIAtlas inguiatlas = value;
			if (inguiatlas == this)
			{
				inguiatlas = null;
			}
			if (this.mReplacement as INGUIAtlas != inguiatlas)
			{
				if (inguiatlas != null && inguiatlas.replacement == this)
				{
					inguiatlas.replacement = null;
				}
				if (this.mReplacement != null)
				{
					this.MarkAsChanged();
				}
				this.mReplacement = (inguiatlas as UnityEngine.Object);
				if (inguiatlas != null)
				{
					this.material = null;
				}
				this.MarkAsChanged();
			}
		}
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0004AFEC File Offset: 0x000491EC
	public UISpriteData GetSprite(string name)
	{
		INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			return replacement.GetSprite(name);
		}
		if (!string.IsNullOrEmpty(name))
		{
			if (this.mSprites.Count == 0)
			{
				this.Upgrade();
			}
			if (this.mSprites.Count == 0)
			{
				return null;
			}
			if (this.mSpriteIndices.Count != this.mSprites.Count)
			{
				this.MarkSpriteListAsChanged();
			}
			int num;
			if (this.mSpriteIndices.TryGetValue(name, out num))
			{
				if (num > -1 && num < this.mSprites.Count)
				{
					return this.mSprites[num];
				}
				this.MarkSpriteListAsChanged();
				return (!this.mSpriteIndices.TryGetValue(name, out num)) ? null : this.mSprites[num];
			}
			else
			{
				int i = 0;
				int count = this.mSprites.Count;
				while (i < count)
				{
					UISpriteData uispriteData = this.mSprites[i];
					if (!string.IsNullOrEmpty(uispriteData.name) && name == uispriteData.name)
					{
#if UNITY_EDITOR
                        if (!Application.isPlaying) return uispriteData;
#endif
                        this.MarkSpriteListAsChanged();
						return uispriteData;
					}
					i++;
				}
			}
		}
		return null;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0004B120 File Offset: 0x00049320
	public void MarkSpriteListAsChanged()
	{
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            this.mSpriteIndices.Clear();
		int i = 0;
		int count = this.mSprites.Count;
		while (i < count)
		{
			this.mSpriteIndices[this.mSprites[i].name] = i;
			i++;
		}
        }
    }

	// Token: 0x06000804 RID: 2052 RVA: 0x0000A2F3 File Offset: 0x000084F3
	public void SortAlphabetically()
	{
		this.mSprites.Sort((UISpriteData s1, UISpriteData s2) => s1.name.CompareTo(s2.name));
#if UNITY_EDITOR
        NGUITools.SetDirty(this);
#endif
    }

    // Token: 0x06000805 RID: 2053 RVA: 0x0004B174 File Offset: 0x00049374
    public BetterList<string> GetListOfSprites()
	{
		INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			return replacement.GetListOfSprites();
		}
		if (this.mSprites.Count == 0)
		{
			this.Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		int count = this.mSprites.Count;
		while (i < count)
		{
			UISpriteData uispriteData = this.mSprites[i];
			if (uispriteData != null && !string.IsNullOrEmpty(uispriteData.name))
			{
				betterList.Add(uispriteData.name);
			}
			i++;
		}
		return betterList;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004B204 File Offset: 0x00049404
	public BetterList<string> GetListOfSprites(string match)
	{
		INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			return replacement.GetListOfSprites(match);
		}
		if (string.IsNullOrEmpty(match))
		{
			return this.GetListOfSprites();
		}
		if (this.mSprites.Count == 0)
		{
			this.Upgrade();
		}
		BetterList<string> betterList = new BetterList<string>();
		int i = 0;
		int count = this.mSprites.Count;
		while (i < count)
		{
			UISpriteData uispriteData = this.mSprites[i];
			if (uispriteData != null && !string.IsNullOrEmpty(uispriteData.name) && string.Equals(match, uispriteData.name, StringComparison.OrdinalIgnoreCase))
			{
				betterList.Add(uispriteData.name);
				return betterList;
			}
			i++;
		}
		string[] array = match.Split(new char[]
		{
			' '
		}, StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = array[j].ToLower();
		}
		int k = 0;
		int count2 = this.mSprites.Count;
		while (k < count2)
		{
			UISpriteData uispriteData2 = this.mSprites[k];
			if (uispriteData2 != null && !string.IsNullOrEmpty(uispriteData2.name))
			{
				string text = uispriteData2.name.ToLower();
				int num = 0;
				for (int l = 0; l < array.Length; l++)
				{
					if (text.Contains(array[l]))
					{
						num++;
					}
				}
				if (num == array.Length)
				{
					betterList.Add(uispriteData2.name);
				}
			}
			k++;
		}
		return betterList;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0004B39C File Offset: 0x0004959C
	public bool References(INGUIAtlas atlas)
	{
		if (atlas == null)
		{
			return false;
		}
		if (atlas == this)
		{
			return true;
		}
		INGUIAtlas replacement = this.replacement;
		return replacement != null && replacement.References(atlas);
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0004B3D4 File Offset: 0x000495D4
	public void MarkAsChanged()
	{
#if UNITY_EDITOR
        NGUITools.SetDirty(gameObject);
#endif
        INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			replacement.MarkAsChanged();
		}
		UISprite[] array = NGUITools.FindActive<UISprite>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			UISprite uisprite = array[i];
			if (NGUITools.CheckIfRelated(this, uisprite.atlas))
			{
				INGUIAtlas atlas = uisprite.atlas;
				uisprite.atlas = null;
				uisprite.atlas = atlas;
			}
			i++;
		}
		NGUIFont[] array2 = Resources.FindObjectsOfTypeAll<NGUIFont>();
		int j = 0;
		int num2 = array2.Length;
		while (j < num2)
		{
			NGUIFont nguifont = array2[j];
			if (nguifont.atlas != null)
			{
				if (NGUITools.CheckIfRelated(this, nguifont.atlas))
				{
					INGUIAtlas atlas2 = nguifont.atlas;
					nguifont.atlas = null;
					nguifont.atlas = atlas2;
				}
			}
			j++;
		}
		UIFont[] array3 = Resources.FindObjectsOfTypeAll<UIFont>();
		int k = 0;
		int num3 = array3.Length;
		while (k < num3)
		{
			UIFont uifont = array3[k];
			if (NGUITools.CheckIfRelated(this, uifont.atlas))
			{
				INGUIAtlas atlas3 = uifont.atlas;
				uifont.atlas = null;
				uifont.atlas = atlas3;
			}
			k++;
		}
		UILabel[] array4 = NGUITools.FindActive<UILabel>();
		int l = 0;
		int num4 = array4.Length;
		while (l < num4)
		{
			UILabel uilabel = array4[l];
			if (uilabel.atlas != null)
			{
				if (NGUITools.CheckIfRelated(this, uilabel.atlas))
				{
					INGUIFont bitmapFont = uilabel.bitmapFont;
					uilabel.bitmapFont = null;
					uilabel.bitmapFont = bitmapFont;
                }
            }
			l++;
		}
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0004B568 File Offset: 0x00049768
	private bool Upgrade()
	{
		INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			UIAtlas uiatlas = replacement as UIAtlas;
			if (uiatlas != null)
			{
				return uiatlas.Upgrade();
			}
		}
		if (this.mSprites.Count == 0 && this.sprites.Count > 0 && this.material)
		{
			Texture mainTexture = this.material.mainTexture;
			int width = (!(mainTexture != null)) ? 512 : mainTexture.width;
			int height = (!(mainTexture != null)) ? 512 : mainTexture.height;
			for (int i = 0; i < this.sprites.Count; i++)
			{
				UIAtlas.Sprite sprite = this.sprites[i];
				Rect outer = sprite.outer;
				Rect inner = sprite.inner;
				if (this.mCoordinates == UIAtlas.Coordinates.TexCoords)
				{
					NGUIMath.ConvertToPixels(outer, width, height, true);
					NGUIMath.ConvertToPixels(inner, width, height, true);
				}
				UISpriteData uispriteData = new UISpriteData();
				uispriteData.name = sprite.name;
				uispriteData.x = Mathf.RoundToInt(outer.xMin);
				uispriteData.y = Mathf.RoundToInt(outer.yMin);
				uispriteData.width = Mathf.RoundToInt(outer.width);
				uispriteData.height = Mathf.RoundToInt(outer.height);
				uispriteData.paddingLeft = Mathf.RoundToInt(sprite.paddingLeft * outer.width);
				uispriteData.paddingRight = Mathf.RoundToInt(sprite.paddingRight * outer.width);
				uispriteData.paddingBottom = Mathf.RoundToInt(sprite.paddingBottom * outer.height);
				uispriteData.paddingTop = Mathf.RoundToInt(sprite.paddingTop * outer.height);
				uispriteData.borderLeft = Mathf.RoundToInt(inner.xMin - outer.xMin);
				uispriteData.borderRight = Mathf.RoundToInt(outer.xMax - inner.xMax);
				uispriteData.borderBottom = Mathf.RoundToInt(outer.yMax - inner.yMax);
				uispriteData.borderTop = Mathf.RoundToInt(inner.yMin - outer.yMin);
				this.mSprites.Add(uispriteData);
			}
			this.sprites.Clear();
#if UNITY_EDITOR
            NGUITools.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            return true;
		}
		return false;
	}

	// Token: 0x04000556 RID: 1366
	[SerializeField]
	[HideInInspector]
	private Material material;

	// Token: 0x04000557 RID: 1367
	[SerializeField]
	[HideInInspector]
	private List<UISpriteData> mSprites = new List<UISpriteData>();

	// Token: 0x04000558 RID: 1368
	[HideInInspector]
	[SerializeField]
	private float mPixelSize = 1f;

	// Token: 0x04000559 RID: 1369
	[HideInInspector]
	[SerializeField]
	private UnityEngine.Object mReplacement;

	// Token: 0x0400055A RID: 1370
	[SerializeField]
	[HideInInspector]
	private UIAtlas.Coordinates mCoordinates;

	// Token: 0x0400055B RID: 1371
	[HideInInspector]
	[SerializeField]
	private List<UIAtlas.Sprite> sprites = new List<UIAtlas.Sprite>();

	// Token: 0x0400055C RID: 1372
	[NonSerialized]
	private int mPMA = -1;

	// Token: 0x0400055D RID: 1373
	[NonSerialized]
	private Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();

	// Token: 0x020000EC RID: 236
	[Serializable]
	private class Sprite
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600080C RID: 2060 RVA: 0x0004B82C File Offset: 0x00049A2C
		public bool hasPadding
		{
			get
			{
				return this.paddingLeft != 0f || this.paddingRight != 0f || this.paddingTop != 0f || this.paddingBottom != 0f;
			}
		}

		// Token: 0x0400055F RID: 1375
		public string name = "Unity Bug";

		// Token: 0x04000560 RID: 1376
		public Rect outer = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04000561 RID: 1377
		public Rect inner = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04000562 RID: 1378
		public bool rotated;

		// Token: 0x04000563 RID: 1379
		public float paddingLeft;

		// Token: 0x04000564 RID: 1380
		public float paddingRight;

		// Token: 0x04000565 RID: 1381
		public float paddingTop;

		// Token: 0x04000566 RID: 1382
		public float paddingBottom;
	}

	// Token: 0x020000ED RID: 237
	private enum Coordinates
	{
		// Token: 0x04000568 RID: 1384
		Pixels,
		// Token: 0x04000569 RID: 1385
		TexCoords
	}
}
