using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public CryptoInt selectedKnife;

    public CryptoInt selectedPistol;

    public CryptoInt selectedRifle;

    public static WeaponType DefaultWeaponType = WeaponType.Rifle;

    public static CryptoBool MaxDamage = false;

    private PlayerWeapons CachedPlayerWeapons;

    private static WeaponManager instance;

    public static bool isStarted;

    private void Awake()
	{
		bool flag = WeaponManager.instance == null;
		if (flag)
		{
			WeaponManager.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public static void Init()
	{
		if (AccountManager.isConnect && !WeaponManager.isStarted)
		{
			Application.Quit();
			WeaponManager.isStarted = false;
		}
		else
		{
			WeaponManager.isStarted = true;
		}
		if (WeaponManager.instance == null)
		{
			new GameObject("WeaponManager").AddComponent<WeaponManager>();
		}
		WeaponManager.UpdateData();
	}

	public static void UpdateData()
	{
		WeaponManager.MaxDamage = false;
		WeaponManager.instance.selectedKnife = AccountManager.GetWeaponSelected(WeaponType.Knife);
		WeaponManager.instance.selectedPistol = AccountManager.GetWeaponSelected(WeaponType.Pistol);
		WeaponManager.instance.selectedRifle = AccountManager.GetWeaponSelected(WeaponType.Rifle);
	}

	public static int GetSelectWeapon(WeaponType type)
	{
		int result;
		switch (type)
		{
		case WeaponType.Knife:
			result = WeaponManager.instance.selectedKnife;
			break;
		case WeaponType.Pistol:
			result = WeaponManager.instance.selectedPistol;
			break;
		case WeaponType.Rifle:
			result = WeaponManager.instance.selectedRifle;
			break;
		default:
			result = 0;
			break;
		}
		return result;
	}

	public static void SetSelectWeapon(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = GameSettings.instance.Weapons[i].ID == weaponID;
			if (flag)
			{
				WeaponManager.SetSelectWeapon(GameSettings.instance.Weapons[i].Type, weaponID);
				break;
			}
		}
	}

	public static void SetSelectWeapon(WeaponType weapon, int weaponID)
	{
		switch (weapon)
		{
		case WeaponType.Knife:
		{
			bool flag = (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0) && WeaponManager.GetWeaponData(weaponID).Secret;
			if (!flag)
			{
				WeaponManager.instance.selectedKnife = weaponID;
			}
			break;
		}
		case WeaponType.Pistol:
			WeaponManager.instance.selectedPistol = weaponID;
			break;
		case WeaponType.Rifle:
			WeaponManager.instance.selectedRifle = weaponID;
			break;
		}
	}

	public static bool HasSelectWeapon(WeaponType type)
	{
		bool result;
		switch (type)
		{
		case WeaponType.Knife:
			result = (WeaponManager.instance.selectedKnife != 0);
			break;
		case WeaponType.Pistol:
			result = (WeaponManager.instance.selectedPistol != 0);
			break;
		case WeaponType.Rifle:
			result = (WeaponManager.instance.selectedRifle != 0);
			break;
		default:
			result = false;
			break;
		}
		return result;
	}

	public static WeaponData GetSelectWeaponData(WeaponType type)
	{
		return GameSettings.instance.Weapons[WeaponManager.GetSelectWeapon(type) - 1];
	}

	public static int GetMemberDamage(PlayerSkinMember member, int weaponID)
	{
		bool flag = WeaponManager.instance.CachedPlayerWeapons == null;
		if (flag)
		{
			WeaponManager.instance.CachedPlayerWeapons = GameManager.player.PlayerWeapon;
		}
		PlayerWeapons.PlayerWeaponData weaponData = WeaponManager.instance.CachedPlayerWeapons.GetWeaponData(weaponID);
		bool flag2 = weaponData == null;
		int memberDamage;
		if (flag2)
		{
			memberDamage = WeaponManager.GetMemberDamage(member, WeaponManager.GetWeaponData(weaponID));
		}
		else
		{
			memberDamage = WeaponManager.GetMemberDamage(member, weaponData.FaceDamage, weaponData.BodyDamage, weaponData.HandDamage, weaponData.LegDamage);
		}
		return memberDamage;
	}

	public static int GetMemberDamage(PlayerSkinMember member, WeaponData weaponData)
	{
		return WeaponManager.GetMemberDamage(member, weaponData.FaceDamage, weaponData.BodyDamage, weaponData.HandDamage, weaponData.LegDamage);
	}

	public static int GetMemberDamage(PlayerSkinMember member, int faceDamage, int bodyDamage, int handDamage, int legDamage)
	{
		bool flag = WeaponManager.MaxDamage;
		int result;
		if (flag)
		{
			result = 100;
		}
		else
		{
			switch (member)
			{
			case PlayerSkinMember.Face:
			{
				bool flag2 = faceDamage == 100 || faceDamage == 0;
				if (flag2)
				{
					result = faceDamage;
				}
				else
				{
					result = faceDamage + UnityEngine.Random.Range(-5, 5);
				}
				break;
			}
			case PlayerSkinMember.Body:
			{
				bool flag3 = bodyDamage == 100 || bodyDamage == 0;
				if (flag3)
				{
					result = bodyDamage;
				}
				else
				{
					result = bodyDamage + UnityEngine.Random.Range(-4, 4);
				}
				break;
			}
			case PlayerSkinMember.Hands:
			{
				bool flag4 = handDamage == 100 || handDamage == 0;
				if (flag4)
				{
					result = handDamage;
				}
				else
				{
					result = handDamage + UnityEngine.Random.Range(-3, 3);
				}
				break;
			}
			case PlayerSkinMember.Legs:
			{
				bool flag5 = legDamage == 100 || legDamage == 0;
				if (flag5)
				{
					result = legDamage;
				}
				else
				{
					result = legDamage + UnityEngine.Random.Range(-2, 2);
				}
				break;
			}
			default:
				result = bodyDamage;
				break;
			}
		}
		return result;
	}

	public static int GetRandomWeaponID()
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = !GameSettings.instance.Weapons[i].Lock;
			if (flag)
			{
				bool flag2 = GameSettings.instance.Weapons[i].Secret;
				if (flag2)
				{
					bool weapon = AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID);
					if (weapon)
					{
						list.Add(GameSettings.instance.Weapons[i]);
					}
				}
				else
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)].ID;
	}

	public static int GetRandomWeaponID(WeaponType type)
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = GameSettings.instance.Weapons[i].Type == type && !GameSettings.instance.Weapons[i].Lock;
			if (flag)
			{
				bool flag2 = GameSettings.instance.Weapons[i].Secret;
				if (flag2)
				{
					bool weapon = AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID);
					if (weapon)
					{
						list.Add(GameSettings.instance.Weapons[i]);
					}
				}
				else
				{
					list.Add(GameSettings.instance.Weapons[i]);
				}
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)].ID;
	}

	public static int GetRandomWeaponID(bool rifle, bool pistol, bool knife, bool secret)
	{
		List<WeaponData> list = new List<WeaponData>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = !GameSettings.instance.Weapons[i].Lock;
			if (flag)
			{
				bool flag2 = GameSettings.instance.Weapons[i].Secret;
				if (flag2)
				{
					bool flag3 = secret && AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID);
					if (flag3)
					{
						list.Add(GameSettings.instance.Weapons[i]);
					}
				}
				else
				{
					switch (GameSettings.instance.Weapons[i].Type)
					{
					case WeaponType.Knife:
						if (knife)
						{
							list.Add(GameSettings.instance.Weapons[i]);
						}
						break;
					case WeaponType.Pistol:
						if (pistol)
						{
							list.Add(GameSettings.instance.Weapons[i]);
						}
						break;
					case WeaponType.Rifle:
						if (rifle)
						{
							list.Add(GameSettings.instance.Weapons[i]);
						}
						break;
					}
				}
			}
		}
		return list[UnityEngine.Random.Range(0, list.Count)].ID;
	}

	public static string GetWeaponName(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = GameSettings.instance.Weapons[i].ID == weaponID;
			if (flag)
			{
				return GameSettings.instance.Weapons[i].Name;
			}
		}
		return string.Empty;
	}

	public static int GetWeaponID(string weaponName)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = GameSettings.instance.Weapons[i].Name == weaponName;
			if (flag)
			{
				return GameSettings.instance.Weapons[i].ID;
			}
		}
		return -1;
	}

	public static WeaponData GetWeaponData(int weaponID)
	{
		bool flag = weaponID <= 0;
		if (flag)
		{
			weaponID = 3;
		}
		return GameSettings.instance.Weapons[weaponID - 1];
	}

	public static WeaponData GetWeaponData(string weaponName)
	{
		return WeaponManager.GetWeaponData(WeaponManager.GetWeaponID(weaponName));
	}

	public static WeaponStoreData GetWeaponStoreData(int weaponID)
	{
		bool flag = weaponID <= 0;
		if (flag)
		{
			weaponID = 3;
		}
		return GameSettings.instance.WeaponsStore[weaponID - 1];
	}

	public static WeaponSkinData GetWeaponSkin(int weaponID, int skinID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			bool flag = GameSettings.instance.Weapons[i].ID == weaponID;
			if (flag)
			{
				for (int j = 0; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
				{
					bool flag2 = GameSettings.instance.WeaponsStore[i].Skins[j].ID == skinID;
					if (flag2)
					{
						return GameSettings.instance.WeaponsStore[i].Skins[j];
					}
				}
			}
		}
		return null;
	}

	public static bool HasWeaponLock(int weaponID)
	{
		return WeaponManager.GetWeaponData(weaponID).Lock;
	}

	public static WeaponSkinData GetRandomWeaponSkin(int weaponID)
	{
		return GameSettings.instance.WeaponsStore[weaponID - 1].Skins[UnityEngine.Random.Range(0, GameSettings.instance.WeaponsStore[weaponID - 1].Skins.Count)];
	}

	public static StickerData GetStickerData(int id)
	{
		for (int i = 0; i < GameSettings.instance.Stickers.Count; i++)
		{
			bool flag = GameSettings.instance.Stickers[i].ID == id;
			if (flag)
			{
				return GameSettings.instance.Stickers[i];
			}
		}
		return null;
	}

	public static string GetStickerName(int id)
	{
		return WeaponManager.GetStickerData(id).Name;
	}
}
