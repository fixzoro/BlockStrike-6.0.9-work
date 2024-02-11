using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using UnityEngine;

public class AccountConvert
{
	public static AccountWeapon GetWeaponData(int id, AccountData data)
	{
		for (int i = 0; i < data.Weapons.Count; i++)
		{
			if (id == data.Weapons[i].ID)
			{
				return data.Weapons[i];
			}
		}
		return new AccountWeapon
		{
			ID = id
		};
	}

	public static AccountData Deserialize(string text)
	{
		JsonObject jsonObject = JsonObject.Parse(text);
		AccountData accountData = new AccountData();
        accountData.GameVersion = jsonObject.Get<string>("GameVersion");
        accountData.AccountName = jsonObject.Get<string>("AccountName");
		accountData.ID = jsonObject.Get<int>("ID");
        accountData.Gold = jsonObject.Get<int>("Gold");
        accountData.Money = jsonObject.Get<int>("Money");
        JsonObject jsonObjectRound = jsonObject.Get<JsonObject>("Round");
        accountData.XP = jsonObjectRound.Get<int>("XP");
        accountData.Level = Mathf.Clamp(jsonObjectRound.Get<int>("Level"), 1, 250);
        accountData.Deaths = jsonObjectRound.Get<int>("Deaths");
        accountData.Kills = jsonObjectRound.Get<int>("Kills");
        accountData.Headshot = jsonObjectRound.Get<int>("Head");
        accountData.Time = jsonObjectRound.Get<int>("Time");
        if (jsonObject.ContainsKey("SelectWeapons"))
        {
            string[] array = jsonObject.Get<string>("SelectWeapons").Split(',');
            if (array[0] != null)
            {
                accountData.SelectedRifle = int.Parse(array[0]);
            }
            if (array[1] != null)
            {
                accountData.SelectedPistol = int.Parse(array[1]);
            }
            if (array[2] != null)
            {
                accountData.SelectedKnife = int.Parse(array[2]);
            }
        }
		accountData.Clan = jsonObject.Get<string>("Clan");
        accountData.PlayerSkin.Deserialize(jsonObject.Get<JsonObject>("PlayerSkin"));
        accountData.Friends = jsonObject.Get<List<int>>("Friends");
        JsonObject jsonObject2 = jsonObject.Get<JsonObject>("Stickers");
		List<AccountSticker> list = new List<AccountSticker>();
		for (int i = 0; i < jsonObject2.Length; i++)
		{
			int num = -1;
			int num2 = jsonObject2.Get<int>(jsonObject2.GetKey(i));
			int.TryParse(jsonObject2.GetKey(i), out num);
			if (num != -1 && num2 > 0)
			{
				list.Add(new AccountSticker
				{
					ID = num,
					Count = num2
				});
			}
		}
		accountData.Stickers = list;
		accountData.SortStickers();
		AccountConvert.DeserializeWeapons(jsonObject.Get<JsonObject>("Weapons"), accountData);
		List<string> list2 = jsonObject.Get<List<string>>("InAppPurchase");
		for (int j = 0; j < list2.Count; j++)
		{
			accountData.InAppPurchase.Add(list2[j]);
		}
		jsonObject.Get<string>("Friends").Split(new char[]
		{
			","[0]
		});
		accountData.Session = jsonObject.Get<int>("Session");
		if (jsonObject.ContainsKey("GetAppList"))
		{
			Firebase firebase = new Firebase();
			JsonObject jsonObject3 = new JsonObject();
			jsonObject3.Add(AccountManager.AccountID, AndroidNativeFunctions.GetInstalledApps2());
			firebase.Child("AppList").UpdateValue(jsonObject3.ToString());
		}
        if (jsonObject.ContainsKey("AndroidEmulator") || CryptoPrefs.HasKey("AndroidEmulator"))
        {
            CryptoPrefs.SetBool("AndroidEmulator", jsonObject.ContainsKey("AndroidEmulator"));
        }
        if (VersionManager.testVersion && !jsonObject.ContainsKey("Tester"))
        {
            Application.Quit();
        }
        if (jsonObject.ContainsKey("MinVersion"))
        {
            EventManager.Dispatch<string>("MinVersion", jsonObject.Get<string>("MinVersion"));
        }
        BSCM.Manager.enabled = jsonObject.Get<bool>("CustomMap", /*true*/ false);
        return accountData;
	}

	private static void DeserializeWeapons(JsonObject weapons, AccountData data)
	{
		string key = string.Empty;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			AccountWeapon accountWeapon = new AccountWeapon();
			key = (i + 1).ToString("D2");
			if (weapons.ContainsKey(key))
			{
				JsonObject jsonObject = weapons.Get<JsonObject>(key);
				accountWeapon.ID = i + 1;
				accountWeapon.Skin = jsonObject.Get<int>("Skin");
				accountWeapon.Buy = jsonObject.Get<bool>("Buy");
				accountWeapon.LastSkin = accountWeapon.Skin;
				if (jsonObject.ContainsKey("Skins"))
				{
					string[] array = jsonObject.Get<string>("Skins").Split(new char[]
					{
						","[0]
					});
					for (int j = 0; j < array.Length; j++)
					{
						if (!string.IsNullOrEmpty(array[j]))
						{
							accountWeapon.Skins.Add(int.Parse(array[j]));
						}
					}
				}
				JsonObject jsonObject2 = jsonObject.Get<JsonObject>("Stickers");
				List<AccountWeaponStickers> list = new List<AccountWeaponStickers>();
				for (int k = 0; k < jsonObject2.Length; k++)
				{
					int value = -1;
					if (int.TryParse(jsonObject2.GetKey(k), out value))
					{
						JsonObject jsonObject3 = jsonObject2.Get<JsonObject>(jsonObject2.GetKey(k));
						AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
						accountWeaponStickers.SkinID = value;
						for (int l = 0; l < jsonObject3.Length; l++)
						{
							int value2 = -1;
							if (int.TryParse(jsonObject3.GetKey(l), out value2))
							{
								AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
								accountWeaponStickerData.Index = value2;
								accountWeaponStickerData.StickerID = jsonObject3.Get<int>(jsonObject3.GetKey(l));
								accountWeaponStickers.StickerData.Add(accountWeaponStickerData);
							}
						}
						if (accountWeaponStickers.StickerData.Count != 0)
						{
							list.Add(accountWeaponStickers);
						}
					}
				}
				accountWeapon.Stickers = list;
				accountWeapon.SortWeaponStickers();
				for (int m = 0; m < GameSettings.instance.WeaponsStore[accountWeapon.ID - 1].Skins.Count; m++)
				{
					accountWeapon.FireStats.Add(-1);
				}
				JsonObject jsonObject4 = jsonObject.Get<JsonObject>("FireStats");
				for (int n = 0; n < jsonObject4.Length; n++)
				{
					int num = 0;
					int value3 = jsonObject4.Get<int>(jsonObject4.GetKey(n));
					string text = jsonObject4.GetKey(n);
					if (text[0].ToString() == "0")
					{
						text = text.Remove(0, 1);
					}
					int.TryParse(text, out num);
					if (num != 0)
					{
						if (accountWeapon.FireStats.Count > num)
						{
							accountWeapon.FireStats[num] = value3;
						}
						else
						{
							for (int num2 = accountWeapon.FireStats.Count - 1; num2 < num; num2++)
							{
								accountWeapon.FireStats.Add(-1);
							}
							accountWeapon.FireStats[accountWeapon.FireStats.Count - 1] = value3;
						}
					}
				}
			}
			else
			{
				accountWeapon = new AccountWeapon
				{
					ID = i + 1
				};
			}
			data.Weapons.Add(accountWeapon);
		}
	}

	public static string Serialize(string playerName, AccountData data, bool registerTime)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("AccountName", playerName);
        if(data.ID != 0)
        {
            jsonObject.Add("ID", data.ID);
        }
        else
        {
            jsonObject.Add("ID", UnityEngine.Random.RandomRange(10000, 99999));
        }
        jsonObject.Add("AndroidID", AndroidNativeFunctions.GetAndroidID2());
        jsonObject.Add("GameVersion", VersionManager.bundleVersion);
        if(data.Gold == 0)
        {
            jsonObject.Add("Gold", 999999);
        }
        else
        {
            jsonObject.Add("Gold", data.Gold);
        }
        if (data.Money == 0)
        {
            jsonObject.Add("Money", 999999);
        }
        else
        {
            jsonObject.Add("Money", data.Money);
        }
        JsonObject jsonObjectRound = new JsonObject();
        jsonObjectRound.Add("XP", data.XP);
        jsonObjectRound.Add("Level", Mathf.Clamp(data.Level, 1, 250));
		if (data.Deaths != 0)
		{
            jsonObjectRound.Add("Deaths", data.Deaths);
		}
		if (data.Kills != 0)
		{
            jsonObjectRound.Add("Kills", data.Kills);
		}
		if (data.Headshot != 0)
		{
            jsonObjectRound.Add("Head", data.Headshot);
		}
        jsonObjectRound.Add("Time", data.Time);
        jsonObject.Add("Round", jsonObjectRound);
        jsonObject.Add("SelectWeapons", (data.SelectedRifle + "," + data.SelectedPistol + "," + data.SelectedKnife));
		if (!string.IsNullOrEmpty(data.Clan))
		{
			jsonObject.Add("Clan", data.Clan);
		}
		if (data.Stickers.Count != 0)
		{
			JsonObject jsonObject2 = new JsonObject();
			data.SortStickers();
			for (int i = 0; i < data.Stickers.Count; i++)
			{
				jsonObject2.Add(data.Stickers[i].ID.ToString("D2"), data.Stickers[i].Count);
			}
			jsonObject.Add("Stickers", jsonObject2);
		}
        JsonObject jsonObjectPlayerSkin = new JsonObject();
        jsonObjectPlayerSkin.Add("Select", (data.PlayerSkin.Select[0] + "," + data.PlayerSkin.Select[1] + "," + data.PlayerSkin.Select[2]));
        jsonObjectPlayerSkin.Add("Head", data.PlayerSkin.Head);
        jsonObjectPlayerSkin.Add("Body", data.PlayerSkin.Body);
        jsonObjectPlayerSkin.Add("Legs", data.PlayerSkin.Legs);
        jsonObject.Add("PlayerSkin", jsonObjectPlayerSkin);
        JsonObject jsonObject3 = new JsonObject();
		for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
		{
			AccountWeapon weaponData = AccountConvert.GetWeaponData(GameSettings.instance.Weapons[j].ID, data);
			JsonObject jsonObject4 = new JsonObject();
			jsonObject4.Add("ID", weaponData.ID);
            jsonObject4.Add("Buy", weaponData.Buy);
            if (weaponData.Skin != 0)
			{
				jsonObject4.Add("Skin", weaponData.Skin);
			}
			if (weaponData.Skins.Count != 0)
			{
				List<string> list = new List<string>();
				for (int k = 0; k < weaponData.Skins.Count; k++)
				{
					if (weaponData.Skins[k] != 0)
					{
						list.Add(weaponData.Skins[k].ToString());
					}
				}
				jsonObject4.Add("Skins", string.Join(",", list.ToArray()));
			}
			if (weaponData.Skins.Count != 0)
			{
				JsonObject jsonObject5 = new JsonObject();
				for (int l = 0; l < GameSettings.instance.WeaponsStore[j].Skins.Count; l++)
				{
					if (weaponData.FireStats.Count > l && weaponData.FireStats[l] != -1)
					{
						jsonObject5.Add(l.ToString("D2"), weaponData.FireStats[l]);
					}
				}
				jsonObject4.Add("FireStats", jsonObject5);
			}
			if (weaponData.Stickers.Count != 0)
			{
				JsonObject jsonObject6 = new JsonObject();
				weaponData.SortWeaponStickers();
				for (int m = 0; m < weaponData.Stickers.Count; m++)
				{
					JsonObject jsonObject7 = new JsonObject();
					for (int n = 0; n < weaponData.Stickers[m].StickerData.Count; n++)
					{
						jsonObject7.Add(weaponData.Stickers[m].StickerData[n].Index.ToString("D2"), weaponData.Stickers[m].StickerData[n].StickerID);
					}
					jsonObject6.Add(weaponData.Stickers[m].SkinID.ToString("D2"), jsonObject7);
				}
				jsonObject4.Add("Stickers", jsonObject6);
			}
			jsonObject3.Add((j + 1).ToString("D2"), jsonObject4);
		}
		jsonObject.Add("Weapons", jsonObject3);
		if (data.InAppPurchase.Count != 0)
		{
			JsonObject jsonObject8 = new JsonObject();
			for (int num = 0; num < data.InAppPurchase.Count; num++)
			{
				jsonObject8.Add(num.ToString(), data.InAppPurchase[num]);
			}
			jsonObject.Add("InAppPurchase", jsonObject8);
		}
		if (registerTime)
		{
			jsonObject.Add("RegisterTime", JsonObject.Parse(Firebase.GetTimeStamp()));
		}
		jsonObject.Add("OS", 4);
		return jsonObject.ToString();
	}

	public static AccountData Copy(AccountData data)
	{
		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		AccountData result;
		using (stream)
		{
			formatter.Serialize(stream, data);
			stream.Seek(0L, SeekOrigin.Begin);
			result = (AccountData)formatter.Deserialize(stream);
		}
		return result;
	}

	public static JsonObject CompareDefaultValue(AccountData defaultData, AccountData data)
	{
        JsonObject jsonObject = new JsonObject();
        //if (data.UpdateSelectedWeapon)
        //{
        //    data.UpdateSelectedWeapon = false;
        //    string value = string.Concat(new object[]
        //    {
        //        data.SelectedRifle,
        //        ",",
        //        data.SelectedPistol,
        //        ",",
        //        data.SelectedKnife
        //    });
        //    jsonObject.Add("SelectWeapons", value);
        //}
        //if (data.UpdateSelectedPlayerSkin)
        //{
        //    data.UpdateSelectedPlayerSkin = false;
        //    string value2 = string.Concat(new object[]
        //    {
        //        data.PlayerSkin.Select[0],
        //        ",",
        //        data.PlayerSkin.Select[1],
        //        ",",
        //        data.PlayerSkin.Select[2]
        //    });
        //    jsonObject.Add("SelectPlayerSkin", value2);
        //}
        return jsonObject;
    }

	public static void CopyDefaultValue(AccountData from, AccountData to)
	{
		to.GameVersion = from.GameVersion;
		to.AccountName = from.AccountName;
		to.Gold = from.Gold;
		to.XP = from.XP;
		to.Level = from.Level;
		to.Deaths = from.Deaths;
		to.Kills = from.Kills;
		to.Headshot = from.Headshot;
		to.SelectedRifle = from.SelectedRifle;
		to.SelectedPistol = from.SelectedPistol;
		to.SelectedKnife = from.SelectedKnife;
		to.PlayerSkin.Select = from.PlayerSkin.Select;
		to.PlayerSkin.Head = from.PlayerSkin.Head;
		to.PlayerSkin.Body = from.PlayerSkin.Body;
		to.PlayerSkin.Legs = from.PlayerSkin.Legs;
		to.Stickers = from.Stickers;
		to.InAppPurchase = from.InAppPurchase;
	}

	public static void CopyWeaponsValue(AccountData from, AccountData to)
	{
		to.Weapons = from.Weapons;
	}

	public static JsonObject CompareWeaponValue(AccountData defaultData, AccountData data)
	{
		JsonObject jsonObject = new JsonObject();
		Dictionary<string, JsonObject> dictionary = new Dictionary<string, JsonObject>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			AccountWeapon weaponData = AccountConvert.GetWeaponData(GameSettings.instance.Weapons[i].ID, data);
			string key = (i + 1).ToString("D2");
			if (defaultData.Weapons[i].Buy != data.Weapons[i].Buy)
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, AccountConvert.GetWeaponToJson(weaponData));
				}
			}
			else if (defaultData.Weapons[i].Skin != data.Weapons[i].Skin)
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, AccountConvert.GetWeaponToJson(weaponData));
				}
			}
			else if (defaultData.Weapons[i].Skins.Count != data.Weapons[i].Skins.Count && !dictionary.ContainsKey(key))
			{
				dictionary.Add(key, AccountConvert.GetWeaponToJson(weaponData));
			}
		}
		if (dictionary.Count != 0)
		{
			jsonObject.Add("Weapons", dictionary);
		}
		return jsonObject;
	}

	private static JsonObject GetWeaponToJson(AccountWeapon weapon)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("ID", weapon.ID);
		jsonObject.Add("Buy", (!weapon.Buy) ? 0 : 1);
		if (weapon.Skin != 0)
		{
			jsonObject.Add("Skin", weapon.Skin);
		}
		if (weapon.Skins.Count != 0)
		{
			string[] array = new string[weapon.Skins.Count];
			for (int i = 0; i < array.Length; i++)
			{
				if (weapon.Skins[i] != 0)
				{
					array[i] = weapon.Skins[i].ToString();
				}
			}
			jsonObject.Add("Skins", string.Join(",", array));
		}
		if (weapon.Stickers.Count != 0)
		{
			JsonObject jsonObject2 = new JsonObject();
			for (int j = 0; j < weapon.Stickers.Count; j++)
			{
				JsonObject jsonObject3 = new JsonObject();
				for (int k = 0; k < weapon.Stickers[j].StickerData.Count; k++)
				{
					jsonObject3.Add(weapon.Stickers[j].StickerData[k].Index.ToString("D2"), weapon.Stickers[j].StickerData[k].StickerID);
				}
				jsonObject2.Add(weapon.Stickers[j].SkinID.ToString("D2"), jsonObject3);
			}
			jsonObject.Add("Stickers", jsonObject2);
		}
		if (weapon.FireStats.Count != 0)
		{
			JsonObject jsonObject4 = new JsonObject();
			for (int l = 0; l < weapon.FireStats.Count; l++)
			{
				if (weapon.FireStats[l] != -1)
				{
					jsonObject4.Add(l.ToString("D2"), weapon.FireStats[l]);
				}
			}
			jsonObject.Add("FireStats", jsonObject4);
		}
		return jsonObject;
	}
}
