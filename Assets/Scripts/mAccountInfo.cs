using System;
using System.Text;
using UnityEngine;

public class mAccountInfo : MonoBehaviour
{
	public static void Convert(AccountData data)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Localization.Get("Name", true) + ": " + data.AccountName);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Money", true) + ": " + data.Money);
		stringBuilder.AppendLine(Localization.Get("Gold", true) + ": " + data.Gold);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("XP", true) + ": " + data.XP);
		stringBuilder.AppendLine(Localization.Get("Level", true) + ": " + data.Level);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Time in the game", true) + ": " + data.Time);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Deaths", true) + ": " + data.Deaths);
		stringBuilder.AppendLine(Localization.Get("Kills", true) + ": " + data.Kills);
		stringBuilder.AppendLine(Localization.Get("Headshot", true) + ": " + data.Headshot);
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Weapon", true));
		stringBuilder.AppendLine(Localization.Get("Main Weapon", true) + ": " + WeaponManager.GetWeaponName(data.SelectedRifle));
		stringBuilder.AppendLine(Localization.Get("Secondary weapon", true) + ": " + WeaponManager.GetWeaponName(data.SelectedPistol));
		stringBuilder.AppendLine(Localization.Get("Melee Weapon", true) + ": " + WeaponManager.GetWeaponName(data.SelectedKnife));
		stringBuilder.AppendLine("==========================================================");
		stringBuilder.AppendLine(Localization.Get("Player Skin", true));
		stringBuilder.Append(Localization.Get("Head", true) + ": " + (data.PlayerSkin.Select[0] + 1));
		if (data.PlayerSkin.Head.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int i = 0; i < data.PlayerSkin.Head.Count; i++)
			{
				stringBuilder.Append(data.PlayerSkin.Head[i] + 1 + ((i >= data.PlayerSkin.Head.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.Append(Localization.Get("Body", true) + ": " + (data.PlayerSkin.Select[1] + 1));
		if (data.PlayerSkin.Body.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int j = 0; j < data.PlayerSkin.Body.Count; j++)
			{
				stringBuilder.Append(data.PlayerSkin.Body[j] + 1 + ((j >= data.PlayerSkin.Body.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.Append(Localization.Get("Legs", true) + ": " + (data.PlayerSkin.Select[2] + 1));
		if (data.PlayerSkin.Legs.Count > 0)
		{
			stringBuilder.Append(" [");
			for (int k = 0; k < data.PlayerSkin.Legs.Count; k++)
			{
				stringBuilder.Append(data.PlayerSkin.Legs[k] + 1 + ((k >= data.PlayerSkin.Legs.Count - 1) ? string.Empty : ","));
			}
			stringBuilder.AppendLine("]");
		}
		stringBuilder.AppendLine("==========================================================");
		if (data.Stickers.Count > 0)
		{
			stringBuilder.AppendLine(Localization.Get("Stickers", true));
			for (int l = 0; l < data.Stickers.Count; l++)
			{
				stringBuilder.AppendLine(WeaponManager.GetStickerName(data.Stickers[l].ID) + ": " + data.Stickers[l].Count);
			}
			stringBuilder.AppendLine("==========================================================");
		}
		stringBuilder.AppendLine(Localization.Get("Weapons", true));
		for (int m = 0; m < data.Weapons.Count; m++)
		{
			stringBuilder.AppendLine("////////////////");
			stringBuilder.AppendLine(WeaponManager.GetWeaponName(data.Weapons[m].ID) + ((!data.Weapons[m].Buy) ? " -" : " +"));
			int n = 0;
			while (n < data.Weapons[m].Skins.Count)
			{
				WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(data.Weapons[m].ID, data.Weapons[m].Skins[n]);
				stringBuilder.AppendLine(weaponSkin.Name + ((data.Weapons[m].FireStats[n] == -1) ? string.Empty : (" [" + data.Weapons[m].FireStats[n] + "]")));
				m++;
			}
			stringBuilder.AppendLine("////////////////");
		}
		MonoBehaviour.print(stringBuilder.ToString());
	}
}
