using UnityEngine;

public class mVersionManager : MonoBehaviour
{
    public UILabel VersionLabel;

    private static mVersionManager instance;

    private void Start()
	{
		mVersionManager.instance = this;
		this.VersionLabel.text = VersionManager.bundleVersion;
		mVersionManager.UpdateRegion();
	}

	private void OnLocalize()
	{
		mVersionManager.UpdateRegion();
	}

	public static void UpdateRegion()
	{
		string region = mPhotonSettings.region;
		string text = VersionManager.bundleVersion;
		string text2 = region;
		string text3;
		switch (text2)
		{
		case "ru":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Russia", true)
			});
			goto IL_38C;
		case "eu":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Europe", true)
			});
			goto IL_38C;
		case "us":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("USA", true)
			});
			goto IL_38C;
		case "kr":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("South Korea", true)
			});
			goto IL_38C;
		case "sa":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Brazil", true)
			});
			goto IL_38C;
		case "jp":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Japan", true)
			});
			goto IL_38C;
		case "in":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("India", true)
			});
			goto IL_38C;
		case "au":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Australia", true)
			});
			goto IL_38C;
		case "asia":
			text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				" | ",
				Localization.Get("Region", true),
				": ",
				Localization.Get("Asia", true)
			});
			goto IL_38C;
		}
		text3 = text;
		text = string.Concat(new string[]
		{
			text3,
			" | ",
			Localization.Get("Region", true),
			": ",
			Localization.Get("Optimal", true)
		});
		IL_38C:
		mVersionManager.instance.VersionLabel.text = text;
	}
}
