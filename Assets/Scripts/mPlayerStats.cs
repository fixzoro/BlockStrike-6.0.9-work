using System;
using UnityEngine;

public class mPlayerStats : MonoBehaviour
{
    public UITexture avatarTexture;

    public UILabel NameLabel;

    public UILabel IDLabel;

    public UILabel LevelLabel;

    public UILabel XPLabel;

    public UILabel DeathsLabel;

    public UILabel KillsLabel;

    public UILabel HeadshotKillsLabel;

    public UILabel TimeLabel;

    public UILabel TotalSkinLabel;

    public UILabel LegendarySkinLabel;

    public UILabel ProfessionalSkinLabel;

    public UILabel BasicSkinLabel;

    public UILabel NormalSkinLabel;

    public UILabel MoneyLabel;

    public UILabel GoldLabel;

    public GameObject Panel;

    public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		this.Panel.SetActive(true);
		this.avatarTexture.mainTexture = AccountManager.instance.Data.Avatar;
		this.NameLabel.text = AccountManager.instance.Data.AccountName;
		this.IDLabel.text = "ID: " + AccountManager.instance.Data.ID.ToString();
		this.LevelLabel.text = AccountManager.GetLevel().ToString();
		this.XPLabel.text = AccountManager.GetXP() + "/" + AccountManager.GetMaxXP();
		this.DeathsLabel.text = AccountManager.GetDeaths().ToString();
		this.KillsLabel.text = AccountManager.GetKills().ToString();
		this.HeadshotKillsLabel.text = AccountManager.GetHeadshot().ToString();
		this.TimeLabel.text = Localization.Get("Time in the game", true) + ": " + this.ConvertTime(AccountManager.instance.Data.Time);
		this.TotalSkinLabel.text = this.GetOpenSkins(WeaponSkinQuality.Default) + "/" + this.GetTotalSkins(WeaponSkinQuality.Default);
		this.MoneyLabel.text = AccountManager.GetMoney().ToString("n0");
		this.GoldLabel.text = AccountManager.GetGold().ToString("n0");
		this.LegendarySkinLabel.text = this.GetOpenSkins(WeaponSkinQuality.Legendary) + "/" + this.GetTotalSkins(WeaponSkinQuality.Legendary);
		this.ProfessionalSkinLabel.text = this.GetOpenSkins(WeaponSkinQuality.Professional) + "/" + this.GetTotalSkins(WeaponSkinQuality.Professional);
		this.BasicSkinLabel.text = this.GetOpenSkins(WeaponSkinQuality.Basic) + "/" + this.GetTotalSkins(WeaponSkinQuality.Basic);
		this.NormalSkinLabel.text = this.GetOpenSkins(WeaponSkinQuality.Normal) + "/" + this.GetTotalSkins(WeaponSkinQuality.Normal);
	}

	private int GetOpenSkins(WeaponSkinQuality quality)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
			{
				if ((GameSettings.instance.WeaponsStore[i].Skins[j].Quality == quality || quality == WeaponSkinQuality.Default) && AccountManager.GetWeaponSkin(i + 1, j))
				{
					num++;
				}
			}
		}
		return num;
	}

	private int GetTotalSkins(WeaponSkinQuality quality)
	{
		int num = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
			{
				if (GameSettings.instance.WeaponsStore[i].Skins[j].Quality == quality || quality == WeaponSkinQuality.Default)
				{
					num++;
				}
			}
		}
		return num;
	}

	private string ConvertTime(long time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		if (timeSpan.Days * 24 + timeSpan.Hours > 0)
		{
			return string.Concat(new object[]
			{
				timeSpan.Days * 24 + timeSpan.Hours,
				" ",
				Localization.Get("h", true),
				"."
			});
		}
		if (timeSpan.Minutes > 0)
		{
			return string.Concat(new object[]
			{
				timeSpan.Minutes,
				" ",
				Localization.Get("m", true),
				"."
			});
		}
		return string.Concat(new object[]
		{
			timeSpan.Seconds,
			" ",
			Localization.Get("s", true),
			"."
		});
	}
}
