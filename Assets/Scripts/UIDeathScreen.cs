using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDeathScreen : MonoBehaviour
{
    public UIPanel Panel;

    public UILabel PlayerLabel;

    public UILabel WeaponLabel;

    public UILabel DamageLabel;

    public UITexture HeadshotTexture;

    public UISprite WeaponSprite;

    public UITexture AvatarTexture;

    public float sizeWeapon = 1f;

    private Dictionary<int, int> kills = new Dictionary<int, int>();

    private Dictionary<int, int> deaths = new Dictionary<int, int>();

    private Dictionary<int, int> takenDamages = new Dictionary<int, int>();

    private Dictionary<int, int> takenHits = new Dictionary<int, int>();

    private Dictionary<int, int> givenDamages = new Dictionary<int, int>();

    private Dictionary<int, int> givenHits = new Dictionary<int, int>();

    private int Timer;

    private static UIDeathScreen instance;

    private void Start()
	{
		UIDeathScreen.instance = this;
	}

	public static void Show(DamageInfo damageInfo)
	{
		if (!damageInfo.otherPlayer)
		{
			return;
		}
		if (damageInfo.weapon == 46)
		{
			return;
		}
		UIDeathScreen.AddDeath(damageInfo.player);
		TimerManager.Cancel(UIDeathScreen.instance.Timer);
		UIDeathScreen.instance.Panel.cachedGameObject.SetActive(true);
		UIDeathScreen.instance.Panel.alpha = 0f;
		TweenAlpha.Begin(UIDeathScreen.instance.Panel.cachedGameObject, 0.2f, 1f, 0f);
		PhotonPlayer photonPlayer = PhotonPlayer.Find(damageInfo.player);
		UIDeathScreen.instance.PlayerLabel.text = photonPlayer.UserId;
		UIDeathScreen.instance.HeadshotTexture.cachedGameObject.SetActive(damageInfo.headshot);
		UIDeathScreen.instance.SetWeaponData(damageInfo);
		int num = 0;
		int number = 0;
		int num2 = 0;
		int number2 = 0;
		if (UIDeathScreen.instance.takenDamages.ContainsKey(damageInfo.player))
		{
			num = UIDeathScreen.instance.takenDamages[damageInfo.player];
			number = UIDeathScreen.instance.takenHits[damageInfo.player];
		}
		if (UIDeathScreen.instance.givenDamages.ContainsKey(damageInfo.player))
		{
			num2 = UIDeathScreen.instance.givenDamages[damageInfo.player];
			number2 = UIDeathScreen.instance.givenHits[damageInfo.player];
		}
		UIDeathScreen.instance.DamageLabel.text = string.Concat(new object[]
		{
			num,
			" ",
			Localization.Get("Damage taken", true).ToLower(),
			" | ",
			StringCache.Get(number),
			" ",
			Localization.Get("Hits", true).ToLower(),
			"\n",
			num2,
			" ",
			Localization.Get("Damage given", true).ToLower(),
			" | ",
			StringCache.Get(number2),
			" ",
			Localization.Get("Hits", true).ToLower()
		});
		if (Settings.ShowAvatars)
		{
			UIDeathScreen.instance.AvatarTexture.mainTexture = AvatarManager.Get(photonPlayer.GetAvatarUrl());
		}
		else
		{
			UIDeathScreen.instance.AvatarTexture.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
		UIDeathScreen.instance.Timer = TimerManager.In(3f, delegate()
		{
			TweenAlpha.Begin(UIDeathScreen.instance.Panel.cachedGameObject, 0.2f, 0f, 0f);
			TimerManager.In(0.2f, delegate()
			{
				UIDeathScreen.instance.Panel.cachedGameObject.SetActive(false);
			});
		});
	}

	public static void AddKill(int playerID)
	{
		if (UIDeathScreen.instance.kills.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary2;
			Dictionary<int, int> dictionary = dictionary2 = UIDeathScreen.instance.kills;
			int num = dictionary2[playerID];
			dictionary[playerID] = num + 1;
		}
		else
		{
			UIDeathScreen.instance.kills.Add(playerID, 1);
		}
	}

	public static void AddTakenDamage(int playerID, int damage)
	{
		if (UIDeathScreen.instance.takenDamages.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary2;
			Dictionary<int, int> dictionary = dictionary2 = UIDeathScreen.instance.takenDamages;
			int num = dictionary2[playerID];
			dictionary[playerID] = num + damage;
			Dictionary<int, int> dictionary4;
			Dictionary<int, int> dictionary3 = dictionary4 = UIDeathScreen.instance.takenHits;
			num = dictionary4[playerID];
			dictionary3[playerID] = num + 1;
		}
		else
		{
			UIDeathScreen.instance.takenDamages.Add(playerID, damage);
			UIDeathScreen.instance.takenHits.Add(playerID, 1);
		}
	}

	public static void AddGivenDamage(int playerID, int damage)
	{
		if (UIDeathScreen.instance.givenDamages.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary2;
			Dictionary<int, int> dictionary = dictionary2 = UIDeathScreen.instance.givenDamages;
			int num = dictionary2[playerID];
			dictionary[playerID] = num + damage;
			Dictionary<int, int> dictionary4;
			Dictionary<int, int> dictionary3 = dictionary4 = UIDeathScreen.instance.givenHits;
			num = dictionary4[playerID];
			dictionary3[playerID] = num + 1;
		}
		else
		{
			UIDeathScreen.instance.givenDamages.Add(playerID, damage);
			UIDeathScreen.instance.givenHits.Add(playerID, 1);
		}
	}

	public static void ClearTakenDamage()
	{
		UIDeathScreen.instance.takenDamages.Clear();
		UIDeathScreen.instance.takenHits.Clear();
	}

	public static void ClearGivenDamage()
	{
		UIDeathScreen.instance.givenDamages.Clear();
		UIDeathScreen.instance.givenHits.Clear();
	}

	public static void ClearGivenDamage(int playerID)
	{
		if (UIDeathScreen.instance.givenDamages.ContainsKey(playerID))
		{
			UIDeathScreen.instance.givenDamages[playerID] = 0;
			UIDeathScreen.instance.givenHits[playerID] = 0;
		}
	}

	public static void ClearAll()
	{
		UIDeathScreen.ClearTakenDamage();
		UIDeathScreen.ClearGivenDamage();
	}

	private static void AddDeath(int playerID)
	{
		if (UIDeathScreen.instance.deaths.ContainsKey(playerID))
		{
			Dictionary<int, int> dictionary2;
			Dictionary<int, int> dictionary = dictionary2 = UIDeathScreen.instance.deaths;
			int num = dictionary2[playerID];
			dictionary[playerID] = num + 1;
		}
		else
		{
			UIDeathScreen.instance.deaths.Add(playerID, 1);
		}
	}

	private void SetWeaponData(DamageInfo damageInfo)
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(damageInfo.weapon);
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(damageInfo.weapon, damageInfo.weaponSkin);
		this.WeaponLabel.text = weaponData.Name + " | " + weaponSkin.Name;
		this.WeaponLabel.color = this.GetWeaponSkinQualityColor(weaponSkin.Quality);
		this.WeaponSprite.spriteName = damageInfo.weapon + "-" + damageInfo.weaponSkin;
		this.WeaponSprite.width = (int)(GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].x * this.sizeWeapon);
		this.WeaponSprite.height = (int)(GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].y * this.sizeWeapon);
	}

	private Color GetWeaponSkinQualityColor(WeaponSkinQuality quality)
	{
		switch (quality)
		{
		case WeaponSkinQuality.Default:
		case WeaponSkinQuality.Normal:
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		case WeaponSkinQuality.Basic:
			return new Color32(54, 189, byte.MaxValue, byte.MaxValue);
		case WeaponSkinQuality.Professional:
			return new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
		case WeaponSkinQuality.Legendary:
			return new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
		default:
			return Color.white;
		}
	}
}
