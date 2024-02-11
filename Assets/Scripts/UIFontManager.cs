using System;
using UnityEngine;

public class UIFontManager : MonoBehaviour
{
    public UILabel[] labels;

    public UIPopupList[] popupLists;

    public Font[] fonts;

    private static UIFontManager instance;

    private void Start()
	{
		UIFontManager.instance = this;
		UIFontManager.SetFont(Settings.Font);
	}

	public static void SetFont(int index)
	{
		for (int i = 0; i < UIFontManager.instance.labels.Length; i++)
		{
			UIFontManager.instance.labels[i].trueTypeFont = UIFontManager.instance.fonts[index];
		}
		for (int j = 0; j < UIFontManager.instance.popupLists.Length; j++)
		{
			UIFontManager.instance.popupLists[j].trueTypeFont = UIFontManager.instance.fonts[index];
		}
	}

	public static Font[] GetFonts()
	{
		return UIFontManager.instance.fonts;
	}
}
