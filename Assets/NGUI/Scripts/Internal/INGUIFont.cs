using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public interface INGUIFont
{
	// Token: 0x170000EA RID: 234
	// (get) Token: 0x0600077F RID: 1919
	// (set) Token: 0x06000780 RID: 1920
	BMFont bmFont { get; set; }

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000781 RID: 1921
	// (set) Token: 0x06000782 RID: 1922
	int texWidth { get; set; }

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000783 RID: 1923
	// (set) Token: 0x06000784 RID: 1924
	int texHeight { get; set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000785 RID: 1925
	bool hasSymbols { get; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000786 RID: 1926
	// (set) Token: 0x06000787 RID: 1927
	List<BMSymbol> symbols { get; set; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000788 RID: 1928
	// (set) Token: 0x06000789 RID: 1929
	INGUIAtlas atlas { get; set; }

	// Token: 0x0600078A RID: 1930
	UISpriteData GetSprite(string spriteName);

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x0600078B RID: 1931
	// (set) Token: 0x0600078C RID: 1932
	Material material { get; set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x0600078D RID: 1933
	bool premultipliedAlphaShader { get; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x0600078E RID: 1934
	bool packedFontShader { get; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x0600078F RID: 1935
	Texture2D texture { get; }

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000790 RID: 1936
	// (set) Token: 0x06000791 RID: 1937
	Rect uvRect { get; set; }

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000792 RID: 1938
	// (set) Token: 0x06000793 RID: 1939
	string spriteName { get; set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000794 RID: 1940
	bool isValid { get; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000795 RID: 1941
	// (set) Token: 0x06000796 RID: 1942
	int defaultSize { get; set; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000797 RID: 1943
	UISpriteData sprite { get; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000798 RID: 1944
	// (set) Token: 0x06000799 RID: 1945
	INGUIFont replacement { get; set; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600079A RID: 1946
	bool isDynamic { get; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600079B RID: 1947
	// (set) Token: 0x0600079C RID: 1948
	Font dynamicFont { get; set; }

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x0600079D RID: 1949
	// (set) Token: 0x0600079E RID: 1950
	FontStyle dynamicFontStyle { get; set; }

	// Token: 0x0600079F RID: 1951
	bool References(INGUIFont font);

	// Token: 0x060007A0 RID: 1952
	void MarkAsChanged();

	// Token: 0x060007A1 RID: 1953
	void UpdateUVRect();

	// Token: 0x060007A2 RID: 1954
	BMSymbol MatchSymbol(string text, int offset, int textLength);

	// Token: 0x060007A3 RID: 1955
	void AddSymbol(string sequence, string spriteName);

	// Token: 0x060007A4 RID: 1956
	void RemoveSymbol(string sequence);

	// Token: 0x060007A5 RID: 1957
	void RenameSymbol(string before, string after);

	// Token: 0x060007A6 RID: 1958
	bool UsesSprite(string s);
}
