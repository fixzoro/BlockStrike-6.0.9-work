using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

[Serializable]
public class AccountData
{
    public CryptoString GameVersion;

    public CryptoString AccountName;

    public CryptoInt2 ID;

    public Texture2D Avatar;

    public string AvatarUrl;

    public CryptoInt2 Money = 0;

    public CryptoInt2 Gold = 0;

    public CryptoInt2 XP = 0;

    public CryptoInt2 Level = 1;

    public long Time;

    public CryptoInt2 Deaths = 0;

    public CryptoInt2 Kills = 0;

    public CryptoInt2 Headshot = 0;

    public List<CryptoInt> SelectedWeapons = new List<CryptoInt>();

    public CryptoInt SelectedRifle = 12;

    public CryptoInt SelectedPistol = 3;

    public CryptoInt SelectedKnife = 4;

    public AccountPlayerSkinData PlayerSkin = new AccountPlayerSkinData();

    public CryptoString Clan;

    public CryptoInt Session;

    public List<AccountSticker> Stickers = new List<AccountSticker>();

    public List<AccountWeapon> Weapons = new List<AccountWeapon>();

    public List<int> Friends = new List<int>();

    public List<CryptoString> InAppPurchase = new List<CryptoString>();

    public bool UpdateSelectedWeapon;

    public bool UpdateSelectedPlayerSkin;

    public void SortStickers()
	{
		this.Stickers.Sort(new Comparison<AccountSticker>(this.SortStickersComparer));
	}

	private int SortStickersComparer(AccountSticker a, AccountSticker b)
	{
		return Convert.ToInt32(a.ID).CompareTo(b.ID);
	}

	public void UpdateData(string data)
	{
		if (JsonObject.isJson(data))
		{
			this.UpdateData(JsonObject.Parse(data));
		}
	}

	public void UpdateData(JsonObject json)
	{
		if (json.ContainsKey("Gold"))
		{
			this.Gold = json.Get<int>("Gold");
		}
		if (json.ContainsKey("Gold1"))
		{
			this.Gold += json.Get<int>("Gold1");
		}
		if (json.ContainsKey("Money"))
		{
			this.Money = json.Get<int>("Money");
		}
		if (json.ContainsKey("Money1"))
		{
			this.Money += json.Get<int>("Money1");
		}
		if (json.ContainsKey("XP"))
		{
			this.XP = json.Get<int>("XP");
		}
		if (json.ContainsKey("Level"))
		{
			this.Level = json.Get<int>("Level");
		}
		if (json.ContainsKey("Deaths"))
		{
			this.Deaths = json.Get<int>("Deaths");
		}
		if (json.ContainsKey("Kills"))
		{
			this.Kills = json.Get<int>("Kills");
		}
		if (json.ContainsKey("Head"))
		{
			this.Headshot = json.Get<int>("Head");
		}
		if (json.ContainsKey("Time"))
		{
			this.Time = json.Get<long>("Time");
		}
		if (json.ContainsKey("PlayerSkin"))
		{
			this.PlayerSkin.Deserialize(json.Get<JsonObject>("PlayerSkin"));
		}
		if (json.ContainsKey("Session"))
		{
			this.Session = json.Get<int>("Session");
		}
		if (json.ContainsKey("SetWeapon"))
		{
			int num = json.Get<int>("SetWeapon");
			for (int i = 0; i < this.Weapons.Count; i++)
			{
				if (this.Weapons[i].ID == num)
				{
					this.Weapons[i].Buy = true;
					break;
				}
			}
		}
		if (json.ContainsKey("Clan"))
		{
			this.Clan = json.Get<string>("Clan");
		}
		if (json.ContainsKey("Weapon") && json.ContainsKey("Skin"))
		{
			int num2 = json.Get<int>("Weapon");
			int value = json.Get<int>("Skin");
			for (int j = 0; j < this.Weapons.Count; j++)
			{
				if (this.Weapons[j].ID == num2)
				{
					if (!this.Weapons[j].Skins.Contains(value))
					{
						this.Weapons[j].Skins.Add(value);
					}
					break;
				}
			}
		}
		EventManager.Dispatch("AccountUpdate");
	}
}
