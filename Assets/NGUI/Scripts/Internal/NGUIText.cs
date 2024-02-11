using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public static class NGUIText
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00008168 File Offset: 0x00006368
	public static bool isDynamic
	{
		get
		{
			return NGUIText.bitmapFont == null;
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00008172 File Offset: 0x00006372
	public static void Update()
	{
		NGUIText.Update(true);
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00036194 File Offset: 0x00034394
	public static void Update(bool request)
	{
		NGUIText.finalSize = Mathf.RoundToInt((float)NGUIText.fontSize / NGUIText.pixelDensity);
		NGUIText.finalSpacingX = NGUIText.spacingX * NGUIText.fontScale;
		NGUIText.finalLineHeight = ((float)NGUIText.fontSize + NGUIText.spacingY) * NGUIText.fontScale;
		NGUIText.useSymbols = ((NGUIText.dynamicFont != null || NGUIText.bitmapFont != null) && NGUIText.encoding && NGUIText.symbolStyle != NGUIText.SymbolStyle.None);
		Font font = NGUIText.dynamicFont;
		if (font != null && request)
		{
			font.RequestCharactersInTexture(")_-", NGUIText.finalSize, NGUIText.fontStyle);
			if (!font.GetCharacterInfo(')', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle) || NGUIText.mTempChar.vert.height == 0f)
			{
				font.RequestCharactersInTexture("A", NGUIText.finalSize, NGUIText.fontStyle);
				if (!font.GetCharacterInfo('A', out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
				{
					NGUIText.baseline = 0f;
					return;
				}
			}
			float yMax = NGUIText.mTempChar.vert.yMax;
			float yMin = NGUIText.mTempChar.vert.yMin;
			NGUIText.baseline = Mathf.Round(yMax + ((float)NGUIText.finalSize - yMax + yMin) * 0.5f);
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0000817A File Offset: 0x0000637A
	public static void Prepare(string text)
	{
		NGUIText.mColors.Clear();
		if (NGUIText.dynamicFont != null)
		{
			NGUIText.dynamicFont.RequestCharactersInTexture(text, NGUIText.finalSize, NGUIText.fontStyle);
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x000081AB File Offset: 0x000063AB
	public static BMSymbol GetSymbol(string text, int index, int textLength)
	{
		if (NGUIText.bitmapFont != null)
		{
			return NGUIText.bitmapFont.MatchSymbol(text, index, textLength);
		}
		return null;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x000362F0 File Offset: 0x000344F0
	public static float GetGlyphWidth(int ch, int prev, float fontScale)
	{
		if (NGUIText.bitmapFont != null)
		{
			bool flag = false;
			if (ch == 8201)
			{
				flag = true;
				ch = 32;
			}
			BMGlyph bmglyph = null;
			if (NGUIText.bitmapFont != null)
			{
				bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
			}
			if (bmglyph != null)
			{
				int num = bmglyph.advance;
				if (flag)
				{
					num >>= 1;
				}
				return fontScale * (float)((prev == 0) ? bmglyph.advance : (num + bmglyph.GetKerning(prev)));
			}
		}
		else if (NGUIText.dynamicFont != null && NGUIText.dynamicFont.GetCharacterInfo((char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
		{
			return NGUIText.mTempChar.width * fontScale * NGUIText.pixelDensity;
		}
		return 0f;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x000363BC File Offset: 0x000345BC
	public static NGUIText.GlyphInfo GetGlyph(int ch, int prev, float fontScale = 1f)
	{
		if (NGUIText.bitmapFont != null)
		{
			bool flag = false;
			if (ch == 8201)
			{
				flag = true;
				ch = 32;
			}
			BMGlyph bmglyph = null;
			if (NGUIText.bitmapFont != null)
			{
				bmglyph = NGUIText.bitmapFont.bmFont.GetGlyph(ch);
			}
			if (bmglyph != null)
			{
				int num = (prev == 0) ? 0 : bmglyph.GetKerning(prev);
				NGUIText.glyph.v0.x = (float)((prev == 0) ? bmglyph.offsetX : (bmglyph.offsetX + num));
				NGUIText.glyph.v1.y = (float)(-(float)bmglyph.offsetY);
				NGUIText.glyph.v1.x = NGUIText.glyph.v0.x + (float)bmglyph.width;
				NGUIText.glyph.v0.y = NGUIText.glyph.v1.y - (float)bmglyph.height;
				NGUIText.glyph.u0.x = (float)bmglyph.x;
				NGUIText.glyph.u0.y = (float)(bmglyph.y + bmglyph.height);
				NGUIText.glyph.u2.x = (float)(bmglyph.x + bmglyph.width);
				NGUIText.glyph.u2.y = (float)bmglyph.y;
				NGUIText.glyph.u1.x = NGUIText.glyph.u0.x;
				NGUIText.glyph.u1.y = NGUIText.glyph.u2.y;
				NGUIText.glyph.u3.x = NGUIText.glyph.u2.x;
				NGUIText.glyph.u3.y = NGUIText.glyph.u0.y;
				int num2 = bmglyph.advance;
				if (flag)
				{
					num2 >>= 1;
				}
				NGUIText.glyph.advance = (float)(num2 + num);
				NGUIText.glyph.channel = bmglyph.channel;
				if (fontScale != 1f)
				{
					NGUIText.glyph.v0 *= fontScale;
					NGUIText.glyph.v1 *= fontScale;
					NGUIText.glyph.advance *= fontScale;
				}
				return NGUIText.glyph;
			}
		}
		else if (NGUIText.dynamicFont != null && NGUIText.dynamicFont.GetCharacterInfo((char)ch, out NGUIText.mTempChar, NGUIText.finalSize, NGUIText.fontStyle))
		{
			NGUIText.glyph.v0.x = NGUIText.mTempChar.vert.xMin;
			NGUIText.glyph.v1.x = NGUIText.glyph.v0.x + NGUIText.mTempChar.vert.width;
			NGUIText.glyph.v0.y = NGUIText.mTempChar.vert.yMax - NGUIText.baseline;
			NGUIText.glyph.v1.y = NGUIText.glyph.v0.y - NGUIText.mTempChar.vert.height;
			NGUIText.glyph.u0.x = NGUIText.mTempChar.uv.xMin;
			NGUIText.glyph.u0.y = NGUIText.mTempChar.uv.yMin;
			NGUIText.glyph.u2.x = NGUIText.mTempChar.uv.xMax;
			NGUIText.glyph.u2.y = NGUIText.mTempChar.uv.yMax;
			if (NGUIText.mTempChar.flipped)
			{
				NGUIText.glyph.u1 = new Vector2(NGUIText.glyph.u2.x, NGUIText.glyph.u0.y);
				NGUIText.glyph.u3 = new Vector2(NGUIText.glyph.u0.x, NGUIText.glyph.u2.y);
			}
			else
			{
				NGUIText.glyph.u1 = new Vector2(NGUIText.glyph.u0.x, NGUIText.glyph.u2.y);
				NGUIText.glyph.u3 = new Vector2(NGUIText.glyph.u2.x, NGUIText.glyph.u0.y);
			}
			NGUIText.glyph.advance = NGUIText.mTempChar.width;
			NGUIText.glyph.channel = 0;
			NGUIText.glyph.v0.x = Mathf.Round(NGUIText.glyph.v0.x);
			NGUIText.glyph.v0.y = Mathf.Round(NGUIText.glyph.v0.y);
			NGUIText.glyph.v1.x = Mathf.Round(NGUIText.glyph.v1.x);
			NGUIText.glyph.v1.y = Mathf.Round(NGUIText.glyph.v1.y);
			float num3 = fontScale * NGUIText.pixelDensity;
			if (num3 != 1f)
			{
				NGUIText.glyph.v0 *= num3;
				NGUIText.glyph.v1 *= num3;
				NGUIText.glyph.advance *= num3;
			}
			return NGUIText.glyph;
		}
		return null;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x00036920 File Offset: 0x00034B20
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static float ParseAlpha(string text, int index)
	{
		int num = NGUIMath.HexToDecimal(text[index + 1]) << 4 | NGUIMath.HexToDecimal(text[index + 2]);
		return Mathf.Clamp01((float)num / 255f);
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x000081C6 File Offset: 0x000063C6
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static Color ParseColor(string text, int offset = 0)
	{
		return NGUIText.ParseColor24(text, offset);
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0003695C File Offset: 0x00034B5C
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static Color ParseColor24(string text, int offset = 0)
	{
		int num = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
		float num4 = 0.003921569f;
		return new Color(num4 * (float)num, num4 * (float)num2, num4 * (float)num3);
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x000369E0 File Offset: 0x00034BE0
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static Color ParseColor32(string text, int offset)
	{
		int num = NGUIMath.HexToDecimal(text[offset]) << 4 | NGUIMath.HexToDecimal(text[offset + 1]);
		int num2 = NGUIMath.HexToDecimal(text[offset + 2]) << 4 | NGUIMath.HexToDecimal(text[offset + 3]);
		int num3 = NGUIMath.HexToDecimal(text[offset + 4]) << 4 | NGUIMath.HexToDecimal(text[offset + 5]);
		int num4 = NGUIMath.HexToDecimal(text[offset + 6]) << 4 | NGUIMath.HexToDecimal(text[offset + 7]);
		float num5 = 0.003921569f;
		return new Color(num5 * (float)num, num5 * (float)num2, num5 * (float)num3, num5 * (float)num4);
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x000081CF File Offset: 0x000063CF
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor(Color c)
	{
		return NGUIText.EncodeColor24(c);
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x000081D7 File Offset: 0x000063D7
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static string EncodeColor(string text, Color c)
	{
		return string.Concat(new string[]
		{
			"[c][",
			NGUIText.EncodeColor24(c),
			"]",
			text,
			"[-][/c]"
		});
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00036A8C File Offset: 0x00034C8C
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeAlpha(float a)
	{
		int num = Mathf.Clamp(Mathf.RoundToInt(a * 255f), 0, 255);
		return NGUIMath.DecimalToHex8(num);
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00036AB8 File Offset: 0x00034CB8
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor24(Color c)
	{
		int num = 16777215 & NGUIMath.ColorToInt(c) >> 8;
		return NGUIMath.DecimalToHex24(num);
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00036ADC File Offset: 0x00034CDC
	[DebuggerHidden]
	[DebuggerStepThrough]
	public static string EncodeColor32(Color c)
	{
		int num = NGUIMath.ColorToInt(c);
		return NGUIMath.DecimalToHex32(num);
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00036AF8 File Offset: 0x00034CF8
	public static bool ParseSymbol(string text, ref int index)
	{
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		return NGUIText.ParseSymbol(text, ref index, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5);
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00008209 File Offset: 0x00006409
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static bool IsHex(char ch)
	{
		return (ch >= '0' && ch <= '9') || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00036B28 File Offset: 0x00034D28
	public static bool ParseSymbol(string text, ref int index, BetterList<Color> colors, bool premultiply, ref int sub, ref bool bold, ref bool italic, ref bool underline, ref bool strike, ref bool ignoreColor)
	{
        int length = text.Length;
        if (index + 3 > length || text[index] != '[')
        {
            return false;
        }
        if (text[index + 2] == ']')
        {
            if (text[index + 1] == '-')
            {
                if (colors != null && colors.size > 1)
                {
                    colors.RemoveAt(colors.size - 1);
                }
                index += 3;
                return true;
            }
            switch (text.Substring(index, 3))
            {
                case "[b]":
                case "[B]":
                    bold = true;
                    index += 3;
                    return true;
                case "[i]":
                case "[I]":
                    italic = true;
                    index += 3;
                    return true;
                case "[u]":
                case "[U]":
                    underline = true;
                    index += 3;
                    return true;
                case "[s]":
                case "[S]":
                    strike = true;
                    index += 3;
                    return true;
                case "[c]":
                case "[C]":
                    ignoreColor = true;
                    index += 3;
                    return true;
            }
        }
        if (index + 4 > length)
        {
            return false;
        }
        if (text[index + 3] == ']')
        {
            switch (text.Substring(index, 4))
            {
                case "[/b]":
                case "[/B]":
                    bold = false;
                    index += 4;
                    return true;
                case "[/i]":
                case "[/I]":
                    italic = false;
                    index += 4;
                    return true;
                case "[/u]":
                case "[/U]":
                    underline = false;
                    index += 4;
                    return true;
                case "[/s]":
                case "[/S]":
                    strike = false;
                    index += 4;
                    return true;
                case "[/c]":
                case "[/C]":
                    ignoreColor = false;
                    index += 4;
                    return true;
            }
            char ch = text[index + 1];
            char ch2 = text[index + 2];
            if (IsHex(ch) && IsHex(ch2))
            {
                int num = (NGUIMath.HexToDecimal(ch) << 4) | NGUIMath.HexToDecimal(ch2);
                mAlpha = (float)num / 255f;
                index += 4;
                return true;
            }
        }
        if (index + 5 > length)
        {
            return false;
        }
        if (text[index + 4] == ']')
        {
            switch (text.Substring(index, 5))
            {
                case "[sub]":
                case "[SUB]":
                    sub = 1;
                    index += 5;
                    return true;
                case "[sup]":
                case "[SUP]":
                    sub = 2;
                    index += 5;
                    return true;
            }
        }
        if (index + 6 > length)
        {
            return false;
        }
        if (text[index + 5] == ']')
        {
            switch (text.Substring(index, 6))
            {
                case "[/sub]":
                case "[/SUB]":
                    sub = 0;
                    index += 6;
                    return true;
                case "[/sup]":
                case "[/SUP]":
                    sub = 0;
                    index += 6;
                    return true;
                case "[/url]":
                case "[/URL]":
                    index += 6;
                    return true;
            }
        }
        if (text[index + 1] == 'u' && text[index + 2] == 'r' && text[index + 3] == 'l' && text[index + 4] == '=')
        {
            int num2 = text.IndexOf(']', index + 4);
            if (num2 != -1)
            {
                index = num2 + 1;
                return true;
            }
            index = text.Length;
            return true;
        }
        if (index + 8 > length)
        {
            return false;
        }
        if (text[index + 7] == ']')
        {
            Color val = ParseColor24(text, index + 1);
            if (EncodeColor24(val) != text.Substring(index + 1, 6).ToUpper())
            {
                return false;
            }
            if (colors != null && colors.size > 0)
            {
                val.a = colors[colors.size - 1].a;
                if (premultiply && val.a != 1f)
                {
                    val = Color.Lerp(mInvisible, val, val.a);
                }
                colors.Add(val);
            }
            index += 8;
            return true;
        }
        if (index + 10 > length)
        {
            return false;
        }
        if (text[index + 9] == ']')
        {
            Color val2 = ParseColor32(text, index + 1);
            if (EncodeColor32(val2) != text.Substring(index + 1, 8).ToUpper())
            {
                return false;
            }
            if (colors != null)
            {
                if (premultiply && val2.a != 1f)
                {
                    val2 = Color.Lerp(mInvisible, val2, val2.a);
                }
                colors.Add(val2);
            }
            index += 10;
            return true;
        }
        return false;
    }

	// Token: 0x060004D6 RID: 1238 RVA: 0x000371B4 File Offset: 0x000353B4
	public static string StripSymbols(string text)
	{
		if (text != null)
		{
			int i = 0;
			int length = text.Length;
			while (i < length)
			{
				char c = text[i];
				if (c == '[')
				{
					int num = 0;
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					bool flag5 = false;
					int num2 = i;
					if (NGUIText.ParseSymbol(text, ref num2, null, false, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
					{
						text = text.Remove(i, num2 - i);
						length = text.Length;
						continue;
					}
				}
				i++;
			}
		}
		return text;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x0003723C File Offset: 0x0003543C
	public static void Align(List<Vector3> verts, int indexOffset, float printedWidth, int elements = 4)
	{
		switch (NGUIText.alignment)
		{
		case NGUIText.Alignment.Center:
		{
			float num = ((float)NGUIText.rectWidth - printedWidth) * 0.5f;
			if (num < 0f)
			{
				return;
			}
			int num2 = Mathf.RoundToInt((float)NGUIText.rectWidth - printedWidth);
			int num3 = Mathf.RoundToInt((float)NGUIText.rectWidth);
			bool flag = (num2 & 1) == 1;
			bool flag2 = (num3 & 1) == 1;
			if ((flag && !flag2) || (!flag && flag2))
			{
				num += 0.5f * NGUIText.fontScale;
			}
			int i = indexOffset;
			int count = verts.Count;
			while (i < count)
			{
				Vector3 value = verts[i];
				value.x += num;
				verts[i] = value;
				i++;
			}
			break;
		}
		case NGUIText.Alignment.Right:
		{
			float num4 = (float)NGUIText.rectWidth - printedWidth;
			if (num4 < 0f)
			{
				return;
			}
			int j = indexOffset;
			int count2 = verts.Count;
			while (j < count2)
			{
				Vector3 value2 = verts[j];
				value2.x += num4;
				verts[j] = value2;
				j++;
			}
			break;
		}
		case NGUIText.Alignment.Justified:
		{
			if (printedWidth < (float)NGUIText.rectWidth * 0.65f)
			{
				return;
			}
			float num5 = ((float)NGUIText.rectWidth - printedWidth) * 0.5f;
			if (num5 < 1f)
			{
				return;
			}
			int num6 = (verts.Count - indexOffset) / elements;
			if (num6 < 1)
			{
				return;
			}
			float num7 = 1f / (float)(num6 - 1);
			float num8 = (float)NGUIText.rectWidth / printedWidth;
			int k = indexOffset + elements;
			int num9 = 1;
			int count3 = verts.Count;
			while (k < count3)
			{
				float num10 = verts[k].x;
				float num11 = verts[k + elements / 2].x;
				float num12 = num11 - num10;
				float num13 = num10 * num8;
				float from = num13 + num12;
				float num14 = num11 * num8;
				float to = num14 - num12;
				float t = (float)num9 * num7;
				num11 = Mathf.Lerp(from, num14, t);
				num10 = Mathf.Lerp(num13, to, t);
				num10 = Mathf.Round(num10);
				num11 = Mathf.Round(num11);
				if (elements == 4)
				{
					Vector3 value3 = verts[k];
					value3.x = num10;
					verts[k++] = value3;
					value3 = verts[k];
					value3.x = num10;
					verts[k++] = value3;
					value3 = verts[k];
					value3.x = num11;
					verts[k++] = value3;
					value3 = verts[k];
					value3.x = num11;
					verts[k++] = value3;
				}
				else if (elements == 2)
				{
					Vector3 value3 = verts[k];
					value3.x = num10;
					verts[k++] = value3;
					value3 = verts[k];
					value3.x = num11;
					verts[k++] = value3;
				}
				else if (elements == 1)
				{
					Vector3 value3 = verts[k];
					value3.x = num10;
					verts[k++] = value3;
				}
				num9++;
			}
			break;
		}
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00037590 File Offset: 0x00035790
	public static int GetExactCharacterIndex(List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		int i = 0;
		int count = indices.Count;
		while (i < count)
		{
			int num = i << 1;
			int index = num + 1;
			float x = verts[num].x;
			if (pos.x >= x)
			{
				float x2 = verts[index].x;
				if (pos.x <= x2)
				{
					float y = verts[num].y;
					if (pos.y >= y)
					{
						float y2 = verts[index].y;
						if (pos.y <= y2)
						{
							return indices[i];
						}
					}
				}
			}
			i++;
		}
		return 0;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0003765C File Offset: 0x0003585C
	public static int GetApproximateCharacterIndex(List<Vector3> verts, List<int> indices, Vector2 pos)
	{
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		int index = 0;
		int i = 0;
		int count = verts.Count;
		while (i < count)
		{
			float num3 = Mathf.Abs(pos.y - verts[i].y);
			if (num3 <= num2)
			{
				float num4 = Mathf.Abs(pos.x - verts[i].x);
				if (num3 < num2)
				{
					num2 = num3;
					num = num4;
					index = i;
				}
				else if (num4 < num)
				{
					num = num4;
					index = i;
				}
			}
			i++;
		}
		return indices[index];
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00008241 File Offset: 0x00006441
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static bool IsSpace(int ch)
	{
		return ch == 32 || ch == 8202 || ch == 8203 || ch == 8201;
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00037708 File Offset: 0x00035908
	[DebuggerStepThrough]
	[DebuggerHidden]
	public static void EndLine(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && NGUIText.IsSpace((int)s[num]))
		{
			s[num] = '\n';
		}
		else
		{
			s.Append('\n');
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00037754 File Offset: 0x00035954
	[DebuggerStepThrough]
	[DebuggerHidden]
	private static void ReplaceSpaceWithNewline(ref StringBuilder s)
	{
		int num = s.Length - 1;
		if (num > 0 && NGUIText.IsSpace((int)s[num]))
		{
			s[num] = '\n';
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00037790 File Offset: 0x00035990
	public static Vector2 CalculatePrintedSize(string text)
	{
		Vector2 zero = Vector2.zero;
		if (!string.IsNullOrEmpty(text))
		{
			NGUIText.Prepare(text);
			int prev = 0;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = (float)NGUIText.regionWidth + 0.01f;
			int length = text.Length;
			int num5 = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			for (int i = 0; i < length; i++)
			{
				int num6 = (int)text[i];
				if (num6 == 10)
				{
					if (num > num3)
					{
						num3 = num;
					}
					num = 0f;
					num2 += NGUIText.finalLineHeight;
					prev = 0;
				}
				else if (num6 < 32)
				{
					prev = num6;
				}
				else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num5, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
				{
					i--;
				}
				else
				{
					BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
					float num7 = (num5 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
					if (bmsymbol != null)
					{
						float num8 = (float)bmsymbol.advance * num7;
						float num9 = num + num8;
						if (num9 > num4)
						{
							if (num == 0f)
							{
								break;
							}
							if (num > num3)
							{
								num3 = num;
							}
							num = 0f;
							num2 += NGUIText.finalLineHeight;
						}
						else if (num9 > num3)
						{
							num3 = num9;
						}
						num += num8 + NGUIText.finalSpacingX;
						i += bmsymbol.length - 1;
						prev = 0;
					}
					else
					{
						NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(num6, prev, num7);
						if (glyphInfo != null)
						{
							prev = num6;
							float num10 = glyphInfo.advance;
							if (num5 != 0)
							{
								if (num5 == 1)
								{
									float num11 = NGUIText.fontScale * (float)NGUIText.fontSize * 0.4f;
									NGUIText.GlyphInfo glyphInfo2 = glyphInfo;
									glyphInfo2.v0.y = glyphInfo2.v0.y - num11;
									NGUIText.GlyphInfo glyphInfo3 = glyphInfo;
									glyphInfo3.v1.y = glyphInfo3.v1.y - num11;
								}
								else
								{
									float num12 = NGUIText.fontScale * (float)NGUIText.fontSize * 0.05f;
									NGUIText.GlyphInfo glyphInfo4 = glyphInfo;
									glyphInfo4.v0.y = glyphInfo4.v0.y + num12;
									NGUIText.GlyphInfo glyphInfo5 = glyphInfo;
									glyphInfo5.v1.y = glyphInfo5.v1.y + num12;
								}
							}
							num10 += NGUIText.finalSpacingX;
							float num13 = num + num10;
							if (num13 > num4)
							{
								if (num == 0f)
								{
									goto IL_2D5;
								}
								num2 += NGUIText.finalLineHeight;
							}
							else if (num13 > num3)
							{
								num3 = num13;
							}
							if (NGUIText.IsSpace(num6))
							{
								if (flag3)
								{
									num6 = 95;
								}
								else if (flag4)
								{
									num6 = 45;
								}
							}
							num = num13;
							if (num5 != 0)
							{
								num = Mathf.Round(num);
							}
							if (NGUIText.IsSpace(num6))
							{
							}
						}
					}
				}
				IL_2D5:;
			}
			zero.x = Mathf.Ceil((num <= num3) ? num3 : (num - NGUIText.finalSpacingX));
			zero.y = Mathf.Ceil(num2 + NGUIText.finalLineHeight);
		}
		return zero;
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00037AB8 File Offset: 0x00035CB8
	public static int CalculateOffsetToFit(string text)
	{
		if (string.IsNullOrEmpty(text) || NGUIText.regionWidth < 1)
		{
			return 0;
		}
		NGUIText.Prepare(text);
		int length = text.Length;
		int prev = 0;
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		int i = 0;
		int length2 = text.Length;
		while (i < length2)
		{
			float num2 = (num != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
			BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
			if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
			{
				i--;
			}
			else if (bmsymbol == null)
			{
				int num3 = (int)text[i];
				float glyphWidth = NGUIText.GetGlyphWidth(num3, prev, num2);
				if (glyphWidth != 0f)
				{
					NGUIText.mSizes.Add(NGUIText.finalSpacingX + glyphWidth);
				}
				prev = num3;
			}
			else
			{
				NGUIText.mSizes.Add(NGUIText.finalSpacingX + (float)bmsymbol.advance * num2);
				int j = 0;
				int num4 = bmsymbol.sequence.Length - 1;
				while (j < num4)
				{
					NGUIText.mSizes.Add(0f);
					j++;
				}
				i += bmsymbol.sequence.Length - 1;
				prev = 0;
			}
			i++;
		}
		float num5 = (float)NGUIText.regionWidth;
		int num6 = NGUIText.mSizes.size;
		while (num6 > 0 && num5 > 0f)
		{
			num5 -= NGUIText.mSizes[--num6];
		}
		NGUIText.mSizes.Clear();
		if (num5 < 0f)
		{
			num6++;
		}
		return num6;
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00037C98 File Offset: 0x00035E98
	public static string GetEndOfLineThatFits(string text)
	{
		int length = text.Length;
		int num = NGUIText.CalculateOffsetToFit(text);
		return text.Substring(num, length - num);
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0000826C File Offset: 0x0000646C
	public static bool WrapText(string text, out string finalText, bool wrapLineColors = false)
	{
		return NGUIText.WrapText(text, out finalText, false, wrapLineColors, false);
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x00037CC0 File Offset: 0x00035EC0
	public static bool WrapText(string text, out string finalText, bool keepCharCount, bool wrapLineColors, bool useEllipsis = false)
	{
		if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 1 || NGUIText.finalLineHeight < 1f)
		{
			finalText = string.Empty;
			return false;
		}
		float num = (NGUIText.maxLines <= 0) ? ((float)NGUIText.regionHeight) : Mathf.Min((float)NGUIText.regionHeight, NGUIText.finalLineHeight * (float)NGUIText.maxLines);
		int num2 = (NGUIText.maxLines <= 0) ? 1000000 : NGUIText.maxLines;
		num2 = Mathf.FloorToInt(Mathf.Min((float)num2, num / NGUIText.finalLineHeight) + 0.01f);
		if (num2 == 0)
		{
			finalText = string.Empty;
			return false;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		int length = text.Length;
		NGUIText.Prepare(text);
		StringBuilder stringBuilder = new StringBuilder();
		float num3 = (float)NGUIText.regionWidth;
		float num4 = 0f;
		int num5 = 0;
		int i = 0;
		int num6 = 1;
		int prev = 0;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		Color color = NGUIText.tint;
		int num7 = 0;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		float num8 = (!useEllipsis) ? NGUIText.finalSpacingX : ((NGUIText.finalSpacingX + NGUIText.GetGlyphWidth(46, 46, NGUIText.fontScale)) * 3f);
		int num9 = 0;
		NGUIText.mColors.Add(color);
		if (!NGUIText.useSymbols)
		{
			wrapLineColors = false;
		}
		if (wrapLineColors)
		{
			stringBuilder.Append("[");
			stringBuilder.Append(NGUIText.EncodeColor(color));
			stringBuilder.Append("]");
		}
		while (i < length)
		{
			char c = text[i];
			bool flag9 = NGUIText.IsSpace((int)c);
			if (c > '⿿')
			{
				flag3 = true;
			}
			if (c == '\n')
			{
				if (num6 == num2)
				{
					break;
				}
				num4 = 0f;
				if (num5 < i)
				{
					stringBuilder.Append(text.Substring(num5, i - num5 + 1));
				}
				else
				{
					stringBuilder.Append(c);
				}
				if (wrapLineColors)
				{
					for (int j = 0; j < NGUIText.mColors.size; j++)
					{
						stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
					}
					for (int k = 0; k < NGUIText.mColors.size; k++)
					{
						stringBuilder.Append("[");
						stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[k]));
						stringBuilder.Append("]");
					}
				}
				flag = true;
				num6++;
				num5 = i + 1;
				prev = 0;
			}
			else
			{
				bool flag10 = flag || num6 == num2;
				int num10 = num7;
				if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num7, ref flag4, ref flag5, ref flag6, ref flag7, ref flag8))
				{
					if (num6 == num2 && useEllipsis && num5 < num9)
					{
						if (num9 > num5)
						{
							stringBuilder.Append(text.Substring(num5, num9 - num5 + 1));
						}
						if (num10 != 0)
						{
							stringBuilder.Append("[/sub]");
						}
						stringBuilder.Append("...");
						num5 = i;
						break;
					}
					if (num9 + 1 > i)
					{
						stringBuilder.Append(text.Substring(num5, i - num5));
						num5 = i;
					}
					if (wrapLineColors)
					{
						if (flag8)
						{
							color = NGUIText.mColors[NGUIText.mColors.size - 1];
							color.a *= NGUIText.mAlpha * NGUIText.tint.a;
						}
						else
						{
							color = NGUIText.tint * NGUIText.mColors[NGUIText.mColors.size - 1];
							color.a *= NGUIText.mAlpha;
						}
						int l = 0;
						int num11 = NGUIText.mColors.size - 2;
						while (l < num11)
						{
							color.a *= NGUIText.mColors[l].a;
							l++;
						}
					}
					if (num5 < i)
					{
						stringBuilder.Append(text.Substring(num5, i - num5));
					}
					else
					{
						stringBuilder.Append(c);
					}
					num5 = i--;
					num9 = num5;
				}
				else
				{
					BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
					float num12 = (num7 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
					float num13;
					if (bmsymbol == null)
					{
						float glyphWidth = NGUIText.GetGlyphWidth((int)c, prev, num12);
						if (glyphWidth == 0f && !flag9)
						{
							goto IL_8B7;
						}
						num13 = NGUIText.finalSpacingX + glyphWidth;
					}
					else
					{
						num13 = NGUIText.finalSpacingX + (float)bmsymbol.advance * num12;
					}
					if (num7 != 0)
					{
						num13 = Mathf.Round(num13);
					}
					num4 += num13;
					prev = (int)c;
					float num14 = (!useEllipsis || !flag10) ? num3 : (num3 - num8);
					if (flag9 && !flag3 && num5 < i)
					{
						int num15 = i - num5;
						if (num6 == num2 && num4 >= num14 && i < length)
						{
							char c2 = text[i];
							if (c2 < ' ' || NGUIText.IsSpace((int)c2))
							{
								num15--;
							}
						}
						if (flag10 && useEllipsis && num5 < num9 && num4 < num3 && num4 > num14)
						{
							if (num9 > num5)
							{
								stringBuilder.Append(text.Substring(num5, num9 - num5 + 1));
							}
							if (num7 != 0)
							{
								stringBuilder.Append("[/sub]");
							}
							stringBuilder.Append("...");
							num5 = i;
							break;
						}
						stringBuilder.Append(text.Substring(num5, num15 + 1));
						flag = false;
						num5 = i + 1;
					}
					if (useEllipsis && !flag9 && num4 < num14)
					{
						num9 = i;
					}
					if (num4 > num14)
					{
						if (flag10)
						{
							if (useEllipsis && i > 0)
							{
								if (num9 > num5)
								{
									stringBuilder.Append(text.Substring(num5, num9 - num5 + 1));
								}
								if (num7 != 0)
								{
									stringBuilder.Append("[/sub]");
								}
								stringBuilder.Append("...");
								num5 = i;
								break;
							}
							stringBuilder.Append(text.Substring(num5, Mathf.Max(0, i - num5)));
							if (!flag9 && !flag3)
							{
								flag2 = false;
							}
							if (wrapLineColors && NGUIText.mColors.size > 0)
							{
								stringBuilder.Append("[-]");
							}
							if (num6++ == num2)
							{
								num5 = i;
								break;
							}
							if (keepCharCount)
							{
								NGUIText.ReplaceSpaceWithNewline(ref stringBuilder);
							}
							else
							{
								NGUIText.EndLine(ref stringBuilder);
							}
							if (wrapLineColors)
							{
								for (int m = 0; m < NGUIText.mColors.size; m++)
								{
									stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
								}
								for (int n = 0; n < NGUIText.mColors.size; n++)
								{
									stringBuilder.Append("[");
									stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[n]));
									stringBuilder.Append("]");
								}
							}
							flag = true;
							if (flag9)
							{
								num5 = i + 1;
								num4 = 0f;
							}
							else
							{
								num5 = i;
								num4 = num13;
							}
							num9 = i;
							prev = 0;
						}
						else
						{
							while (num5 < length && NGUIText.IsSpace((int)text[num5]))
							{
								num5++;
							}
							flag = true;
							num4 = 0f;
							i = num5 - 1;
							prev = 0;
							if (num6++ == num2)
							{
								break;
							}
							if (keepCharCount)
							{
								NGUIText.ReplaceSpaceWithNewline(ref stringBuilder);
							}
							else
							{
								NGUIText.EndLine(ref stringBuilder);
							}
							if (wrapLineColors)
							{
								for (int num16 = 0; num16 < NGUIText.mColors.size; num16++)
								{
									stringBuilder.Insert(stringBuilder.Length - 1, "[-]");
								}
								for (int num17 = 0; num17 < NGUIText.mColors.size; num17++)
								{
									stringBuilder.Append("[");
									stringBuilder.Append(NGUIText.EncodeColor(NGUIText.mColors[num17]));
									stringBuilder.Append("]");
								}
							}
							goto IL_8B7;
						}
					}
					if (bmsymbol != null)
					{
						i += bmsymbol.length - 1;
						prev = 0;
					}
				}
			}
			IL_8B7:
			i++;
		}
		if (num5 < i)
		{
			stringBuilder.Append(text.Substring(num5, i - num5));
		}
		if (wrapLineColors && NGUIText.mColors.size > 0)
		{
			stringBuilder.Append("[-]");
		}
		finalText = stringBuilder.ToString();
		NGUIText.mColors.Clear();
		return flag2 && (i == length || ((NGUIText.maxLines == 0) ? (num6 == 0) : (num6 == NGUIText.maxLines)));
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00038618 File Offset: 0x00036818
	public static void Print(string text, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int count = verts.Count;
		NGUIText.Prepare(text);
		NGUIText.mColors.Add(Color.white);
		NGUIText.mAlpha = 1f;
		int prev = 0;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = (float)NGUIText.finalSize;
		Color a = NGUIText.tint * NGUIText.gradientBottom;
		Color b = NGUIText.tint * NGUIText.gradientTop;
		Color color = NGUIText.tint;
		int length = text.Length;
		Rect rect = default(Rect);
		float num5 = 0f;
		float num6 = 0f;
		float num7 = num4 * NGUIText.pixelDensity;
		float num8 = (float)NGUIText.regionWidth + 0.01f;
		int num9 = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (NGUIText.bitmapFont != null)
		{
			rect = NGUIText.bitmapFont.uvRect;
			num5 = rect.width / (float)NGUIText.bitmapFont.texWidth;
			num6 = rect.height / (float)NGUIText.bitmapFont.texHeight;
		}
		for (int i = 0; i < length; i++)
		{
			int num10 = (int)text[i];
			float num11 = num;
			if (num10 == 10)
			{
				if (num > num3)
				{
					num3 = num;
				}
				if (NGUIText.alignment != NGUIText.Alignment.Left)
				{
					NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 4);
					count = verts.Count;
				}
				num = 0f;
				num2 += NGUIText.finalLineHeight;
				prev = 0;
			}
			else if (num10 < 32)
			{
				prev = num10;
			}
			else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num9, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
			{
				if (flag5)
				{
					color = NGUIText.mColors[NGUIText.mColors.size - 1];
					color.a *= NGUIText.mAlpha * NGUIText.tint.a;
				}
				else
				{
					color = NGUIText.tint * NGUIText.mColors[NGUIText.mColors.size - 1];
					color.a *= NGUIText.mAlpha;
				}
				int j = 0;
				int num12 = NGUIText.mColors.size - 2;
				while (j < num12)
				{
					color.a *= NGUIText.mColors[j].a;
					j++;
				}
				if (NGUIText.gradient)
				{
					a = NGUIText.gradientBottom * color;
					b = NGUIText.gradientTop * color;
				}
				i--;
			}
			else
			{
				BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
				float num13 = (num9 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
				if (bmsymbol != null)
				{
					float num14 = num + (float)bmsymbol.offsetX * NGUIText.fontScale;
					float num15 = num14 + (float)bmsymbol.width * NGUIText.fontScale;
					float num16 = -(num2 + (float)bmsymbol.offsetY * NGUIText.fontScale);
					float num17 = num16 - (float)bmsymbol.height * NGUIText.fontScale;
					float num18 = (float)bmsymbol.advance * num13;
					if (num + num18 > num8)
					{
						if (num == 0f)
						{
							return;
						}
						if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
						{
							NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 4);
							count = verts.Count;
						}
						num14 -= num;
						num15 -= num;
						num17 -= NGUIText.finalLineHeight;
						num16 -= NGUIText.finalLineHeight;
						num = 0f;
						num2 += NGUIText.finalLineHeight;
					}
					verts.Add(new Vector3(num14, num17));
					verts.Add(new Vector3(num14, num16));
					verts.Add(new Vector3(num15, num16));
					verts.Add(new Vector3(num15, num17));
					num += num18 + NGUIText.finalSpacingX;
					i += bmsymbol.length - 1;
					prev = 0;
					if (uvs != null)
					{
						Rect uvRect = bmsymbol.uvRect;
						float xMin = uvRect.xMin;
						float yMin = uvRect.yMin;
						float xMax = uvRect.xMax;
						float yMax = uvRect.yMax;
						uvs.Add(new Vector2(xMin, yMin));
						uvs.Add(new Vector2(xMin, yMax));
						uvs.Add(new Vector2(xMax, yMax));
						uvs.Add(new Vector2(xMax, yMin));
					}
					if (cols != null)
					{
						if (NGUIText.symbolStyle == NGUIText.SymbolStyle.Colored)
						{
							for (int k = 0; k < 4; k++)
							{
								cols.Add(color);
							}
						}
						else
						{
							Color white = Color.white;
							if (NGUIText.symbolStyle == NGUIText.SymbolStyle.NoOutline)
							{
								white.r = -1f;
								white.a = 0f;
							}
							else
							{
								white.a = color.a;
							}
							for (int l = 0; l < 4; l++)
							{
								cols.Add(white);
							}
						}
					}
				}
				else
				{
					NGUIText.GlyphInfo glyphInfo = NGUIText.GetGlyph(num10, prev, num13);
					if (glyphInfo != null)
					{
						prev = num10;
						float num19 = glyphInfo.advance;
						if (num9 != 0)
						{
							if (num9 == 1)
							{
								float num20 = NGUIText.fontScale * (float)NGUIText.fontSize * 0.4f;
								NGUIText.GlyphInfo glyphInfo2 = glyphInfo;
								glyphInfo2.v0.y = glyphInfo2.v0.y - num20;
								NGUIText.GlyphInfo glyphInfo3 = glyphInfo;
								glyphInfo3.v1.y = glyphInfo3.v1.y - num20;
							}
							else
							{
								float num21 = NGUIText.fontScale * (float)NGUIText.fontSize * 0.05f;
								NGUIText.GlyphInfo glyphInfo4 = glyphInfo;
								glyphInfo4.v0.y = glyphInfo4.v0.y + num21;
								NGUIText.GlyphInfo glyphInfo5 = glyphInfo;
								glyphInfo5.v1.y = glyphInfo5.v1.y + num21;
							}
						}
						num19 += NGUIText.finalSpacingX;
						float num14 = glyphInfo.v0.x + num;
						float num17 = glyphInfo.v0.y - num2;
						float num15 = glyphInfo.v1.x + num;
						float num16 = glyphInfo.v1.y - num2;
						if (num + num19 > num8)
						{
							if (num == 0f)
							{
								return;
							}
							if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
							{
								NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 4);
								count = verts.Count;
							}
							num14 -= num;
							num15 -= num;
							num17 -= NGUIText.finalLineHeight;
							num16 -= NGUIText.finalLineHeight;
							num = 0f;
							num2 += NGUIText.finalLineHeight;
							num11 = 0f;
						}
						if (NGUIText.IsSpace(num10))
						{
							if (flag3)
							{
								num10 = 95;
							}
							else if (flag4)
							{
								num10 = 45;
							}
						}
						num += num19;
						if (num9 != 0)
						{
							num = Mathf.Round(num);
						}
						if (!NGUIText.IsSpace(num10))
						{
							if (uvs != null)
							{
								if (NGUIText.bitmapFont != null)
								{
									glyphInfo.u0.x = rect.xMin + num5 * glyphInfo.u0.x;
									glyphInfo.u2.x = rect.xMin + num5 * glyphInfo.u2.x;
									glyphInfo.u0.y = rect.yMax - num6 * glyphInfo.u0.y;
									glyphInfo.u2.y = rect.yMax - num6 * glyphInfo.u2.y;
									glyphInfo.u1.x = glyphInfo.u0.x;
									glyphInfo.u1.y = glyphInfo.u2.y;
									glyphInfo.u3.x = glyphInfo.u2.x;
									glyphInfo.u3.y = glyphInfo.u0.y;
								}
								int m = 0;
								int num22 = (!flag) ? 1 : 4;
								while (m < num22)
								{
									uvs.Add(glyphInfo.u0);
									uvs.Add(glyphInfo.u1);
									uvs.Add(glyphInfo.u2);
									uvs.Add(glyphInfo.u3);
									m++;
								}
							}
							if (cols != null)
							{
								if (glyphInfo.channel == 0 || glyphInfo.channel == 15)
								{
									if (NGUIText.gradient)
									{
										float num23 = num7 + glyphInfo.v0.y / NGUIText.fontScale;
										float num24 = num7 + glyphInfo.v1.y / NGUIText.fontScale;
										num23 /= num7;
										num24 /= num7;
										NGUIText.s_c0 = Color.Lerp(a, b, num23);
										NGUIText.s_c1 = Color.Lerp(a, b, num24);
										int n = 0;
										int num25 = (!flag) ? 1 : 4;
										while (n < num25)
										{
											cols.Add(NGUIText.s_c0);
											cols.Add(NGUIText.s_c1);
											cols.Add(NGUIText.s_c1);
											cols.Add(NGUIText.s_c0);
											n++;
										}
									}
									else
									{
										int num26 = 0;
										int num27 = (!flag) ? 4 : 16;
										while (num26 < num27)
										{
											cols.Add(color);
											num26++;
										}
									}
								}
								else
								{
									Color color2 = color;
									color2 *= 0.49f;
									switch (glyphInfo.channel)
									{
									case 1:
										color2.b += 0.51f;
										break;
									case 2:
										color2.g += 0.51f;
										break;
									case 4:
										color2.r += 0.51f;
										break;
									case 8:
										color2.a += 0.51f;
										break;
									}
									int num28 = 0;
									int num29 = (!flag) ? 4 : 16;
									while (num28 < num29)
									{
										cols.Add(color2);
										num28++;
									}
								}
							}
							if (!flag)
							{
								if (!flag2)
								{
									verts.Add(new Vector3(num14, num17));
									verts.Add(new Vector3(num14, num16));
									verts.Add(new Vector3(num15, num16));
									verts.Add(new Vector3(num15, num17));
								}
								else
								{
									float num30 = (float)NGUIText.fontSize * 0.1f * ((num16 - num17) / (float)NGUIText.fontSize);
									verts.Add(new Vector3(num14 - num30, num17));
									verts.Add(new Vector3(num14 + num30, num16));
									verts.Add(new Vector3(num15 + num30, num16));
									verts.Add(new Vector3(num15 - num30, num17));
								}
							}
							else
							{
								for (int num31 = 0; num31 < 4; num31++)
								{
									float num32 = NGUIText.mBoldOffset[num31 * 2];
									float num33 = NGUIText.mBoldOffset[num31 * 2 + 1];
									float num34 = (!flag2) ? 0f : ((float)NGUIText.fontSize * 0.1f * ((num16 - num17) / (float)NGUIText.fontSize));
									verts.Add(new Vector3(num14 + num32 - num34, num17 + num33));
									verts.Add(new Vector3(num14 + num32 + num34, num16 + num33));
									verts.Add(new Vector3(num15 + num32 + num34, num16 + num33));
									verts.Add(new Vector3(num15 + num32 - num34, num17 + num33));
								}
							}
							if (flag3 || flag4)
							{
								NGUIText.GlyphInfo glyphInfo6 = NGUIText.GetGlyph((!flag4) ? 95 : 45, prev, num13);
								if (glyphInfo6 != null)
								{
									if (uvs != null)
									{
										if (NGUIText.bitmapFont != null)
										{
											glyphInfo6.u0.x = rect.xMin + num5 * glyphInfo6.u0.x;
											glyphInfo6.u2.x = rect.xMin + num5 * glyphInfo6.u2.x;
											glyphInfo6.u0.y = rect.yMax - num6 * glyphInfo6.u0.y;
											glyphInfo6.u2.y = rect.yMax - num6 * glyphInfo6.u2.y;
										}
										float x = (glyphInfo6.u0.x + glyphInfo6.u2.x) * 0.5f;
										int num35 = 0;
										int num36 = (!flag) ? 1 : 4;
										while (num35 < num36)
										{
											uvs.Add(new Vector2(x, glyphInfo6.u0.y));
											uvs.Add(new Vector2(x, glyphInfo6.u2.y));
											uvs.Add(new Vector2(x, glyphInfo6.u2.y));
											uvs.Add(new Vector2(x, glyphInfo6.u0.y));
											num35++;
										}
									}
									num17 = -num2 + glyphInfo6.v0.y;
									num16 = -num2 + glyphInfo6.v1.y;
									if (flag)
									{
										for (int num37 = 0; num37 < 4; num37++)
										{
											float num38 = NGUIText.mBoldOffset[num37 * 2];
											float num39 = NGUIText.mBoldOffset[num37 * 2 + 1];
											verts.Add(new Vector3(num11 + num38, num17 + num39));
											verts.Add(new Vector3(num11 + num38, num16 + num39));
											verts.Add(new Vector3(num + num38, num16 + num39));
											verts.Add(new Vector3(num + num38, num17 + num39));
										}
									}
									else
									{
										verts.Add(new Vector3(num11, num17));
										verts.Add(new Vector3(num11, num16));
										verts.Add(new Vector3(num, num16));
										verts.Add(new Vector3(num, num17));
									}
									if (NGUIText.gradient)
									{
										float num40 = num7 + glyphInfo6.v0.y / num13;
										float num41 = num7 + glyphInfo6.v1.y / num13;
										num40 /= num7;
										num41 /= num7;
										NGUIText.s_c0 = Color.Lerp(a, b, num40);
										NGUIText.s_c1 = Color.Lerp(a, b, num41);
										int num42 = 0;
										int num43 = (!flag) ? 1 : 4;
										while (num42 < num43)
										{
											cols.Add(NGUIText.s_c0);
											cols.Add(NGUIText.s_c1);
											cols.Add(NGUIText.s_c1);
											cols.Add(NGUIText.s_c0);
											num42++;
										}
									}
									else
									{
										int num44 = 0;
										int num45 = (!flag) ? 4 : 16;
										while (num44 < num45)
										{
											cols.Add(color);
											num44++;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
		{
			NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 4);
			count = verts.Count;
		}
		NGUIText.mColors.Clear();
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x00039538 File Offset: 0x00037738
	public static void PrintApproximateCharacterPositions(string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		NGUIText.Prepare(text);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)NGUIText.regionWidth + 0.01f;
		int length = text.Length;
		int count = verts.Count;
		int prev = 0;
		int num4 = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		for (int i = 0; i < length; i++)
		{
			int num5 = (int)text[i];
			float num6 = (num4 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
			float num7 = num6 * 0.5f;
			verts.Add(new Vector3(num, -num2 - num7));
			indices.Add(i);
			if (num5 == 10)
			{
				if (NGUIText.alignment != NGUIText.Alignment.Left)
				{
					NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 1);
					count = verts.Count;
				}
				num = 0f;
				num2 += NGUIText.finalLineHeight;
				prev = 0;
			}
			else if (num5 < 32)
			{
				prev = 0;
			}
			else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num4, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
			{
				i--;
			}
			else
			{
				BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
				if (bmsymbol == null)
				{
					float num8 = NGUIText.GetGlyphWidth(num5, prev, num6);
					if (num8 != 0f)
					{
						num8 += NGUIText.finalSpacingX;
						if (num + num8 > num3)
						{
							if (num == 0f)
							{
								return;
							}
							if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
							{
								NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 1);
								count = verts.Count;
							}
							num = num8;
							num2 += NGUIText.finalLineHeight;
						}
						else
						{
							num += num8;
						}
						verts.Add(new Vector3(num, -num2 - num7));
						indices.Add(i + 1);
						prev = num5;
					}
				}
				else
				{
					float num9 = (float)bmsymbol.advance * num6 + NGUIText.finalSpacingX;
					if (num + num9 > num3)
					{
						if (num == 0f)
						{
							return;
						}
						if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
						{
							NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 1);
							count = verts.Count;
						}
						num = num9;
						num2 += NGUIText.finalLineHeight;
					}
					else
					{
						num += num9;
					}
					verts.Add(new Vector3(num, -num2 - num7));
					indices.Add(i + 1);
					i += bmsymbol.sequence.Length - 1;
					prev = 0;
				}
			}
		}
		if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
		{
			NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 1);
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00039814 File Offset: 0x00037A14
	public static void PrintExactCharacterPositions(string text, List<Vector3> verts, List<int> indices)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		NGUIText.Prepare(text);
		float num = 0f;
		float num2 = 0f;
		float num3 = (float)NGUIText.regionWidth + 0.01f;
		float num4 = (float)NGUIText.fontSize * NGUIText.fontScale;
		int length = text.Length;
		int count = verts.Count;
		int prev = 0;
		int num5 = 0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		for (int i = 0; i < length; i++)
		{
			int num6 = (int)text[i];
			float num7 = (num5 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
			if (num6 == 10)
			{
				if (NGUIText.alignment != NGUIText.Alignment.Left)
				{
					NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 2);
					count = verts.Count;
				}
				num = 0f;
				num2 += NGUIText.finalLineHeight;
				prev = 0;
			}
			else if (num6 < 32)
			{
				prev = 0;
			}
			else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num5, ref flag, ref flag2, ref flag3, ref flag4, ref flag5))
			{
				i--;
			}
			else
			{
				BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
				if (bmsymbol == null)
				{
					float glyphWidth = NGUIText.GetGlyphWidth(num6, prev, num7);
					if (glyphWidth != 0f)
					{
						float num8 = glyphWidth + NGUIText.finalSpacingX;
						if (num + num8 > num3)
						{
							if (num == 0f)
							{
								return;
							}
							if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
							{
								NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 2);
								count = verts.Count;
							}
							num = 0f;
							num2 += NGUIText.finalLineHeight;
							prev = 0;
							i--;
						}
						else
						{
							indices.Add(i);
							verts.Add(new Vector3(num, -num2 - num4));
							verts.Add(new Vector3(num + num8, -num2));
							prev = num6;
							num += num8;
						}
					}
				}
				else
				{
					float num9 = (float)bmsymbol.advance * num7 + NGUIText.finalSpacingX;
					if (num + num9 > num3)
					{
						if (num == 0f)
						{
							return;
						}
						if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
						{
							NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 2);
							count = verts.Count;
						}
						num = 0f;
						num2 += NGUIText.finalLineHeight;
						prev = 0;
						i--;
					}
					else
					{
						indices.Add(i);
						verts.Add(new Vector3(num, -num2 - num4));
						verts.Add(new Vector3(num + num9, -num2));
						i += bmsymbol.sequence.Length - 1;
						num += num9;
						prev = 0;
					}
				}
			}
		}
		if (NGUIText.alignment != NGUIText.Alignment.Left && count < verts.Count)
		{
			NGUIText.Align(verts, count, num - NGUIText.finalSpacingX, 2);
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00039B14 File Offset: 0x00037D14
	public static void PrintCaretAndSelection(string text, int start, int end, List<Vector3> caret, List<Vector3> highlight)
	{
		if (string.IsNullOrEmpty(text))
		{
			text = " ";
		}
		NGUIText.Prepare(text);
		int num = end;
		if (start > end)
		{
			end = start;
			start = num;
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = (float)NGUIText.fontSize * NGUIText.fontScale;
		int indexOffset = (caret == null) ? 0 : caret.Count;
		int num5 = (highlight == null) ? 0 : highlight.Count;
		int length = text.Length;
		int i = 0;
		int prev = 0;
		bool flag = false;
		bool flag2 = false;
		int num6 = 0;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		while (i < length)
		{
			float num7 = (num6 != 0) ? (NGUIText.fontScale * 0.75f) : NGUIText.fontScale;
			if (caret != null && !flag2 && num <= i)
			{
				flag2 = true;
				caret.Add(new Vector3(num2 - 1f, -num3 - num4));
				caret.Add(new Vector3(num2 - 1f, -num3));
				caret.Add(new Vector3(num2 + 1f, -num3));
				caret.Add(new Vector3(num2 + 1f, -num3 - num4));
			}
			int num8 = (int)text[i];
			if (num8 == 10)
			{
				if (caret != null && flag2)
				{
					if (NGUIText.alignment != NGUIText.Alignment.Left)
					{
						NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
					}
					caret = null;
				}
				if (highlight != null)
				{
					if (flag)
					{
						flag = false;
						highlight.Add(zero2);
						highlight.Add(zero);
					}
					else if (start <= i && end > i)
					{
						highlight.Add(new Vector3(num2, -num3 - num4));
						highlight.Add(new Vector3(num2, -num3));
						highlight.Add(new Vector3(num2 + 2f, -num3));
						highlight.Add(new Vector3(num2 + 2f, -num3 - num4));
					}
					if (NGUIText.alignment != NGUIText.Alignment.Left && num5 < highlight.Count)
					{
						NGUIText.Align(highlight, num5, num2 - NGUIText.finalSpacingX, 4);
						num5 = highlight.Count;
					}
				}
				num2 = 0f;
				num3 += NGUIText.finalLineHeight;
				prev = 0;
			}
			else if (num8 < 32)
			{
				prev = 0;
			}
			else if (NGUIText.encoding && NGUIText.ParseSymbol(text, ref i, NGUIText.mColors, NGUIText.premultiply, ref num6, ref flag3, ref flag4, ref flag5, ref flag6, ref flag7))
			{
				i--;
			}
			else
			{
				BMSymbol bmsymbol = (!NGUIText.useSymbols) ? null : NGUIText.GetSymbol(text, i, length);
				float num9 = (bmsymbol == null) ? NGUIText.GetGlyphWidth(num8, prev, num7) : ((float)bmsymbol.advance * num7);
				if (num9 != 0f)
				{
					float num10 = num2;
					float num11 = num2 + num9;
					float num12 = -num3 - num4;
					float num13 = -num3;
					if (num11 + NGUIText.finalSpacingX > (float)NGUIText.regionWidth)
					{
						if (num2 == 0f)
						{
							return;
						}
						if (caret != null && flag2)
						{
							if (NGUIText.alignment != NGUIText.Alignment.Left)
							{
								NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
							}
							caret = null;
						}
						if (highlight != null)
						{
							if (flag)
							{
								flag = false;
								highlight.Add(zero2);
								highlight.Add(zero);
							}
							else if (start <= i && end > i)
							{
								highlight.Add(new Vector3(num2, -num3 - num4));
								highlight.Add(new Vector3(num2, -num3));
								highlight.Add(new Vector3(num2 + 2f, -num3));
								highlight.Add(new Vector3(num2 + 2f, -num3 - num4));
							}
							if (NGUIText.alignment != NGUIText.Alignment.Left && num5 < highlight.Count)
							{
								NGUIText.Align(highlight, num5, num2 - NGUIText.finalSpacingX, 4);
								num5 = highlight.Count;
							}
						}
						num10 -= num2;
						num11 -= num2;
						num12 -= NGUIText.finalLineHeight;
						num13 -= NGUIText.finalLineHeight;
						num2 = 0f;
						num3 += NGUIText.finalLineHeight;
					}
					num2 += num9 + NGUIText.finalSpacingX;
					if (highlight != null)
					{
						if (start > i || end <= i)
						{
							if (flag)
							{
								flag = false;
								highlight.Add(zero2);
								highlight.Add(zero);
							}
						}
						else if (!flag)
						{
							flag = true;
							highlight.Add(new Vector3(num10, num12));
							highlight.Add(new Vector3(num10, num13));
						}
					}
					zero = new Vector2(num11, num12);
					zero2 = new Vector2(num11, num13);
					prev = num8;
				}
			}
			i++;
		}
		if (caret != null)
		{
			if (!flag2)
			{
				caret.Add(new Vector3(num2 - 1f, -num3 - num4));
				caret.Add(new Vector3(num2 - 1f, -num3));
				caret.Add(new Vector3(num2 + 1f, -num3));
				caret.Add(new Vector3(num2 + 1f, -num3 - num4));
			}
			if (NGUIText.alignment != NGUIText.Alignment.Left)
			{
				NGUIText.Align(caret, indexOffset, num2 - NGUIText.finalSpacingX, 4);
			}
		}
		if (highlight != null)
		{
			if (flag)
			{
				highlight.Add(zero2);
				highlight.Add(zero);
			}
			else if (start < i && end == i)
			{
				highlight.Add(new Vector3(num2, -num3 - num4));
				highlight.Add(new Vector3(num2, -num3));
				highlight.Add(new Vector3(num2 + 2f, -num3));
				highlight.Add(new Vector3(num2 + 2f, -num3 - num4));
			}
			if (NGUIText.alignment != NGUIText.Alignment.Left && num5 < highlight.Count)
			{
				NGUIText.Align(highlight, num5, num2 - NGUIText.finalSpacingX, 4);
			}
		}
		NGUIText.mColors.Clear();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0003A124 File Offset: 0x00038324
	public static bool ReplaceLink(ref string text, ref int index, string type, string prefix = null, string suffix = null)
	{
		if (index == -1)
		{
			return false;
		}
		index = text.IndexOf(type, index);
		if (index == -1)
		{
			return false;
		}
		if (index > 5)
		{
			for (int i = index - 5; i >= 0; i--)
			{
				if (text[i] == '[')
				{
					if (text[i + 1] == 'u' && text[i + 2] == 'r' && text[i + 3] == 'l' && text[i + 4] == '=')
					{
						index += type.Length;
						return NGUIText.ReplaceLink(ref text, ref index, type, prefix, suffix);
					}
					if (text[i + 1] == '/' && text[i + 2] == 'u' && text[i + 3] == 'r' && text[i + 4] == 'l')
					{
						break;
					}
				}
			}
		}
		int num = index + type.Length;
		int num2 = text.IndexOfAny(new char[]
		{
			' ',
			'\n',
			'\u200a',
			'​',
			'\u2009'
		}, num);
		if (num2 == -1)
		{
			num2 = text.Length;
		}
		int num3 = text.IndexOfAny(new char[]
		{
			'/',
			' '
		}, num);
		if (num3 == -1 || num3 == num)
		{
			index += type.Length;
			return true;
		}
		string text2 = text.Substring(0, index);
		string text3 = text.Substring(index, num2 - index);
		string text4 = text.Substring(num2);
		string text5 = text.Substring(num, num3 - num);
		if (!string.IsNullOrEmpty(prefix))
		{
			text2 += prefix;
		}
		text = string.Concat(new string[]
		{
			text2,
			"[url=",
			text3,
			"][u]",
			text5,
			"[/u][/url]"
		});
		index = text.Length;
		if (string.IsNullOrEmpty(suffix))
		{
			text += text4;
		}
		else
		{
			text = text + suffix + text4;
		}
		return true;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0003A338 File Offset: 0x00038538
	public static bool InsertHyperlink(ref string text, ref int index, string keyword, string link, string prefix = null, string suffix = null)
	{
		int num = text.IndexOf(keyword, index, StringComparison.CurrentCultureIgnoreCase);
		if (num == -1)
		{
			return false;
		}
		if (num > 5)
		{
			for (int i = num - 5; i >= 0; i--)
			{
				if (text[i] == '[')
				{
					if (text[i + 1] == 'u' && text[i + 2] == 'r' && text[i + 3] == 'l' && text[i + 4] == '=')
					{
						index = num + keyword.Length;
						return NGUIText.InsertHyperlink(ref text, ref index, keyword, link, prefix, suffix);
					}
					if (text[i + 1] == '/' && text[i + 2] == 'u' && text[i + 3] == 'r' && text[i + 4] == 'l')
					{
						break;
					}
				}
			}
		}
		string str = text.Substring(0, num);
		string str2 = "[url=" + link + "][u]";
		string text2 = text.Substring(num, keyword.Length);
		if (!string.IsNullOrEmpty(prefix))
		{
			text2 = prefix + text2;
		}
		if (!string.IsNullOrEmpty(suffix))
		{
			text2 += suffix;
		}
		string str3 = text.Substring(num + keyword.Length);
		text = str + str2 + text2 + "[/u][/url]";
		index = text.Length;
		text += str3;
		return true;
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0003A4B8 File Offset: 0x000386B8
	public static void ReplaceLinks(ref string text, string prefix = null, string suffix = null)
	{
		int i = 0;
		while (i < text.Length)
		{
			if (!NGUIText.ReplaceLink(ref text, ref i, "http://", prefix, suffix))
			{
				break;
			}
		}
		int j = 0;
		while (j < text.Length)
		{
			if (!NGUIText.ReplaceLink(ref text, ref j, "https://", prefix, suffix))
			{
				break;
			}
		}
	}

	// Token: 0x04000369 RID: 873
	private const float sizeShrinkage = 0.75f;

	// Token: 0x0400036A RID: 874
	public static INGUIFont bitmapFont;

	// Token: 0x0400036B RID: 875
	public static Font dynamicFont;

	// Token: 0x0400036C RID: 876
	public static NGUIText.GlyphInfo glyph = new NGUIText.GlyphInfo();

	// Token: 0x0400036D RID: 877
	public static int fontSize = 16;

	// Token: 0x0400036E RID: 878
	public static float fontScale = 1f;

	// Token: 0x0400036F RID: 879
	public static float pixelDensity = 1f;

	// Token: 0x04000370 RID: 880
	public static FontStyle fontStyle = FontStyle.Normal;

	// Token: 0x04000371 RID: 881
	public static NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	// Token: 0x04000372 RID: 882
	public static Color tint = Color.white;

	// Token: 0x04000373 RID: 883
	public static int rectWidth = 1000000;

	// Token: 0x04000374 RID: 884
	public static int rectHeight = 1000000;

	// Token: 0x04000375 RID: 885
	public static int regionWidth = 1000000;

	// Token: 0x04000376 RID: 886
	public static int regionHeight = 1000000;

	// Token: 0x04000377 RID: 887
	public static int maxLines = 0;

	// Token: 0x04000378 RID: 888
	public static bool gradient = false;

	// Token: 0x04000379 RID: 889
	public static Color gradientBottom = Color.white;

	// Token: 0x0400037A RID: 890
	public static Color gradientTop = Color.white;

	// Token: 0x0400037B RID: 891
	public static bool encoding = false;

	// Token: 0x0400037C RID: 892
	public static float spacingX = 0f;

	// Token: 0x0400037D RID: 893
	public static float spacingY = 0f;

	// Token: 0x0400037E RID: 894
	public static bool premultiply = false;

	// Token: 0x0400037F RID: 895
	public static NGUIText.SymbolStyle symbolStyle;

	// Token: 0x04000380 RID: 896
	public static int finalSize = 0;

	// Token: 0x04000381 RID: 897
	public static float finalSpacingX = 0f;

	// Token: 0x04000382 RID: 898
	public static float finalLineHeight = 0f;

	// Token: 0x04000383 RID: 899
	public static float baseline = 0f;

	// Token: 0x04000384 RID: 900
	public static bool useSymbols = false;

	// Token: 0x04000385 RID: 901
	private static Color mInvisible = new Color(0f, 0f, 0f, 0f);

	// Token: 0x04000386 RID: 902
	private static BetterList<Color> mColors = new BetterList<Color>();

	// Token: 0x04000387 RID: 903
	private static float mAlpha = 1f;

	// Token: 0x04000388 RID: 904
	private static CharacterInfo mTempChar;

	// Token: 0x04000389 RID: 905
	private static BetterList<float> mSizes = new BetterList<float>();

	// Token: 0x0400038A RID: 906
	private static Color s_c0;

	// Token: 0x0400038B RID: 907
	private static Color s_c1;

	// Token: 0x0400038C RID: 908
	private static float[] mBoldOffset = new float[]
	{
		-0.25f,
		0f,
		0.25f,
		0f,
		0f,
		-0.25f,
		0f,
		0.25f
	};

	// Token: 0x020000A1 RID: 161
	[DoNotObfuscateNGUI]
	public enum Alignment
	{
		// Token: 0x04000392 RID: 914
		Automatic,
		// Token: 0x04000393 RID: 915
		Left,
		// Token: 0x04000394 RID: 916
		Center,
		// Token: 0x04000395 RID: 917
		Right,
		// Token: 0x04000396 RID: 918
		Justified
	}

	// Token: 0x020000A2 RID: 162
	[DoNotObfuscateNGUI]
	public enum SymbolStyle
	{
		// Token: 0x04000398 RID: 920
		None,
		// Token: 0x04000399 RID: 921
		Normal,
		// Token: 0x0400039A RID: 922
		Colored,
		// Token: 0x0400039B RID: 923
		NoOutline
	}

	// Token: 0x020000A3 RID: 163
	public class GlyphInfo
	{
		// Token: 0x0400039C RID: 924
		public Vector2 v0;

		// Token: 0x0400039D RID: 925
		public Vector2 v1;

		// Token: 0x0400039E RID: 926
		public Vector2 u0;

		// Token: 0x0400039F RID: 927
		public Vector2 u1;

		// Token: 0x040003A0 RID: 928
		public Vector2 u2;

		// Token: 0x040003A1 RID: 929
		public Vector2 u3;

		// Token: 0x040003A2 RID: 930
		public float advance;

		// Token: 0x040003A3 RID: 931
		public int channel;
	}
}
