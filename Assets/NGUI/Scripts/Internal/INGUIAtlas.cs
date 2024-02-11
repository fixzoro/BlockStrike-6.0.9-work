using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public interface INGUIAtlas
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600075C RID: 1884
	// (set) Token: 0x0600075D RID: 1885
	Material spriteMaterial { get; set; }

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x0600075E RID: 1886
	// (set) Token: 0x0600075F RID: 1887
	List<UISpriteData> spriteList { get; set; }

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000760 RID: 1888
	Texture texture { get; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x06000761 RID: 1889
	// (set) Token: 0x06000762 RID: 1890
	float pixelSize { get; set; }

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000763 RID: 1891
	bool premultipliedAlpha { get; }

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x06000764 RID: 1892
	// (set) Token: 0x06000765 RID: 1893
	INGUIAtlas replacement { get; set; }

	// Token: 0x06000766 RID: 1894
	UISpriteData GetSprite(string name);

	// Token: 0x06000767 RID: 1895
	BetterList<string> GetListOfSprites();

	// Token: 0x06000768 RID: 1896
	BetterList<string> GetListOfSprites(string match);

	// Token: 0x06000769 RID: 1897
	bool References(INGUIAtlas atlas);

	// Token: 0x0600076A RID: 1898
	void MarkAsChanged();

	// Token: 0x0600076B RID: 1899
	void SortAlphabetically();
}
