using System;
using System.Collections.Generic;

[Serializable]
public class WeaponStoreData
{
	public GameCurrency Currency;

	public CryptoInt Price;

	public List<WeaponUpgradeData> Upgrades = new List<WeaponUpgradeData>();

	public List<WeaponSkinData> Skins = new List<WeaponSkinData>();
}
