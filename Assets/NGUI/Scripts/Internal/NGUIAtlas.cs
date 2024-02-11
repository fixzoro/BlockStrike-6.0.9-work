using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class NGUIAtlas : ScriptableObject, INGUIAtlas
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x0600076D RID: 1901 RVA: 0x000483CC File Offset: 0x000465CC
	// (set) Token: 0x0600076E RID: 1902 RVA: 0x000483F8 File Offset: 0x000465F8
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

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x0004845C File Offset: 0x0004665C
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

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000770 RID: 1904 RVA: 0x000484E0 File Offset: 0x000466E0
	// (set) Token: 0x06000771 RID: 1905 RVA: 0x00048508 File Offset: 0x00046708
	public List<UISpriteData> spriteList
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			if (replacement != null)
			{
				return replacement.spriteList;
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

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000772 RID: 1906 RVA: 0x00048538 File Offset: 0x00046738
	public Texture texture
	{
		get
		{
			INGUIAtlas replacement = this.replacement;
			return (replacement == null) ? ((!(this.material != null)) ? null : this.material.mainTexture) : replacement.texture;
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000773 RID: 1907 RVA: 0x00048580 File Offset: 0x00046780
	// (set) Token: 0x06000774 RID: 1908 RVA: 0x000485AC File Offset: 0x000467AC
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

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000775 RID: 1909 RVA: 0x00009F9B File Offset: 0x0000819B
	// (set) Token: 0x06000776 RID: 1910 RVA: 0x000485FC File Offset: 0x000467FC
	public INGUIAtlas replacement
	{
		get
		{
			if (this.mReplacement == null)
			{
				return null;
			}
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

	// Token: 0x06000777 RID: 1911 RVA: 0x00048674 File Offset: 0x00046874
	public UISpriteData GetSprite(string name)
	{
        //INGUIAtlas replacement = this.replacement;
        //if (replacement != null)
        //{
        //	return replacement.GetSprite(name);
        //}
        //if (!string.IsNullOrEmpty(name))
        //{
        //	if (this.mSprites.Count == 0)
        //	{
        //		return null;
        //	}
        //	if (this.mSpriteIndices.Count != this.mSprites.Count)
        //	{
        //		this.MarkSpriteListAsChanged();
        //	}
        //	int num;
        //	if (this.mSpriteIndices.TryGetValue(name, out num))
        //	{
        //		if (num > -1 && num < this.mSprites.Count)
        //		{
        //			return this.mSprites[num];
        //		}
        //		this.MarkSpriteListAsChanged();
        //		return (!this.mSpriteIndices.TryGetValue(name, out num)) ? null : this.mSprites[num];
        //	}
        //	else
        //	{
        //		int i = 0;
        //		int count = this.mSprites.Count;
        //		while (i < count)
        //		{
        //			UISpriteData uispriteData = this.mSprites[i];
        //			if (!string.IsNullOrEmpty(uispriteData.name) && name == uispriteData.name)
        //			{
        //				this.MarkSpriteListAsChanged();
        //				return uispriteData;
        //			}
        //			i++;
        //		}
        //	}
        //}
        //return null;
        var rep = replacement;
        if (rep != null) return rep.GetSprite(name);

        if (!string.IsNullOrEmpty(name))
        {
            if (mSprites.Count == 0) return null;

            // O(1) lookup via a dictionary
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                // The number of indices differs from the sprite list? Rebuild the indices.
                if (mSpriteIndices.Count != mSprites.Count)
                    MarkSpriteListAsChanged();

                int index;
                if (mSpriteIndices.TryGetValue(name, out index))
                {
                    // If the sprite is present, return it as-is
                    if (index > -1 && index < mSprites.Count) return mSprites[index];

                    // The sprite index was out of range -- perhaps the sprite was removed? Rebuild the indices.
                    MarkSpriteListAsChanged();

                    // Try to look up the index again
                    return mSpriteIndices.TryGetValue(name, out index) ? mSprites[index] : null;
                }
            }

            // Sequential O(N) lookup.
            for (int i = 0, imax = mSprites.Count; i < imax; ++i)
            {
                UISpriteData s = mSprites[i];

                // string.Equals doesn't seem to work with Flash export
                if (!string.IsNullOrEmpty(s.name) && name == s.name)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying) return s;
#endif
                    // If this point was reached then the sprite is present in the non-indexed list,
                    // so the sprite indices should be updated.
                    MarkSpriteListAsChanged();
                    return s;
                }
            }
        }
        return null;
    }

	// Token: 0x06000778 RID: 1912 RVA: 0x00048794 File Offset: 0x00046994
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

	// Token: 0x06000779 RID: 1913 RVA: 0x00009FBB File Offset: 0x000081BB
	public void SortAlphabetically()
	{
		this.mSprites.Sort((UISpriteData s1, UISpriteData s2) => s1.name.CompareTo(s2.name));
#if UNITY_EDITOR
        NGUITools.SetDirty(this);
#endif
    }

    // Token: 0x0600077A RID: 1914 RVA: 0x000487E8 File Offset: 0x000469E8
    public BetterList<string> GetListOfSprites()
	{
		INGUIAtlas replacement = this.replacement;
		if (replacement != null)
		{
			return replacement.GetListOfSprites();
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

	// Token: 0x0600077B RID: 1915 RVA: 0x00048864 File Offset: 0x00046A64
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

	// Token: 0x0600077C RID: 1916 RVA: 0x000489E8 File Offset: 0x00046BE8
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

	// Token: 0x0600077D RID: 1917 RVA: 0x00048A20 File Offset: 0x00046C20
	public void MarkAsChanged()
	{
#if UNITY_EDITOR
        NGUITools.SetDirty(this);
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
#if UNITY_EDITOR
                NGUITools.SetDirty(uisprite);
#endif
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
#if UNITY_EDITOR
                NGUITools.SetDirty(nguifont);
#endif
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
#if UNITY_EDITOR
            NGUITools.SetDirty(uifont);
#endif
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
#if UNITY_EDITOR
                NGUITools.SetDirty(uilabel);
#endif
            }
            l++;
		}
	}

	// Token: 0x0400051B RID: 1307
	[SerializeField]
	[HideInInspector]
	private Material material;

	// Token: 0x0400051C RID: 1308
	[SerializeField]
	[HideInInspector]
	private List<UISpriteData> mSprites = new List<UISpriteData>();

	// Token: 0x0400051D RID: 1309
	[SerializeField]
	[HideInInspector]
	private float mPixelSize = 1f;

	// Token: 0x0400051E RID: 1310
	[SerializeField]
	[HideInInspector]
	private UnityEngine.Object mReplacement;

	// Token: 0x0400051F RID: 1311
	[NonSerialized]
	private int mPMA = -1;

	// Token: 0x04000520 RID: 1312
	[NonSerialized]
	private Dictionary<string, int> mSpriteIndices = new Dictionary<string, int>();

	// Token: 0x020000E4 RID: 228
	private enum Coordinates
	{
		// Token: 0x04000523 RID: 1315
		Pixels,
		// Token: 0x04000524 RID: 1316
		TexCoords
	}
}
