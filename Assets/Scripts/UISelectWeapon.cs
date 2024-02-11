using System;
using System.Collections.Generic;
using UnityEngine;

public class UISelectWeapon : MonoBehaviour
{
    public WeaponType Weapon;

    public UILabel WeaponNameLabel;

    public UISprite WeaponSprite;

    public float Size = 1f;

    public UILabel ChangeButtonLabel;

    private bool SkinMode;

    private List<string> WeaponList = new List<string>();

    private List<int> SkinList = new List<int>();

    private int SelectWeapon;

    private int SelectSkin;

    private static bool isChange = false;

    public static CryptoBool AllWeapons = false;

    public static CryptoBool SelectedUpdateWeaponManager = false;

    private void Start()
	{
		this.UpdateWeaponsName();
		this.GetSelectWeapon();
		this.UpdateSelectedWeapon();
		if (this.ChangeButtonLabel != null)
		{
			this.ChangeButtonLabel.text = Localization.Get("Weapons", true);
		}
	}

	public void Close()
	{
		if (UISelectWeapon.SelectedUpdateWeaponManager && UISelectWeapon.isChange)
		{
			PlayerInput.instance.PlayerWeapon.UpdateWeaponAll();
			UISelectWeapon.isChange = false;
		}
	}

	public void Left()
	{
		if (this.SkinMode)
		{
			this.SelectSkin--;
			if (this.SelectSkin < 0)
			{
				this.SelectSkin = this.SkinList.Count - 1;
			}
			this.UpdateSelectedSkin();
		}
		else
		{
			this.SelectWeapon--;
			if (this.SelectWeapon < 0)
			{
				this.SelectWeapon = this.WeaponList.Count - 1;
			}
			if (!UISelectWeapon.AllWeapons)
			{
				AccountManager.SetWeaponSelected(this.Weapon, WeaponManager.GetWeaponID(this.WeaponList[this.SelectWeapon]));
                AccountManager.SetFirebaseWeaponsSelected(null, null);
            }
			this.UpdateSelectedWeapon();
		}
	}

	public void Right()
	{
		if (this.SkinMode)
		{
			this.SelectSkin++;
			if (this.SelectSkin > this.SkinList.Count - 1)
			{
				this.SelectSkin = 0;
			}
			this.UpdateSelectedSkin();
		}
		else
		{
			this.SelectWeapon++;
			if (this.SelectWeapon > this.WeaponList.Count - 1)
			{
				this.SelectWeapon = 0;
			}
			if (!UISelectWeapon.AllWeapons)
			{
				AccountManager.SetWeaponSelected(this.Weapon, WeaponManager.GetWeaponID(this.WeaponList[this.SelectWeapon]));
                AccountManager.SetFirebaseWeaponsSelected(null, null);
            }
			this.UpdateSelectedWeapon();
		}
	}

	private void GetSelectWeapon()
	{
		int weaponSelected = AccountManager.GetWeaponSelected(this.Weapon);
		string weaponName = WeaponManager.GetWeaponName(weaponSelected);
		for (int i = 0; i < this.WeaponList.Count; i++)
		{
			if (this.WeaponList[i] == weaponName)
			{
				this.SelectWeapon = i;
				break;
			}
		}
	}

	private void UpdateWeaponsName()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Type == this.Weapon)
			{
				int num = GameSettings.instance.Weapons[i].ID;
				string item = GameSettings.instance.Weapons[i].Name;
				if (AccountManager.GetWeapon(num) || UISelectWeapon.AllWeapons)
				{
					if (num == 4 || num == 3 || num == 12)
					{
						this.WeaponList.Insert(0, item);
					}
					else if (!GameSettings.instance.Weapons[i].Lock)
					{
						if (GameSettings.instance.Weapons[i].Secret)
						{
							if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID) && AccountManager.GetMoney() >= 0 && AccountManager.GetGold() >= 0)
							{
								this.WeaponList.Add(item);
							}
						}
						else
						{
							this.WeaponList.Add(item);
						}
					}
				}
			}
		}
	}

	private void UpdateSelectedWeapon()
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(this.WeaponList[this.SelectWeapon]);
		WeaponManager.SetSelectWeapon(this.Weapon, weaponData.ID);
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(weaponData.ID, AccountManager.GetWeaponSkinSelected(weaponData.ID));
		this.WeaponNameLabel.text = weaponData.Name + "  |  " + this.GetWeaponSkinRarityColor(weaponSkin);
		this.WeaponSprite.spriteName = weaponData.ID + "-" + weaponSkin.ID;
		this.WeaponSprite.width = (int)(GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].x * this.Size);
		this.WeaponSprite.height = (int)(GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].y * this.Size);
		if (UISelectWeapon.SelectedUpdateWeaponManager)
		{
			UISelectWeapon.isChange = true;
		}
	}

	private string GetWeaponSkinRarityColor(WeaponSkinData skin)
	{
		switch (skin.Quality)
		{
		case WeaponSkinQuality.Default:
		case WeaponSkinQuality.Normal:
			return Utils.ColorToHex(new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), skin.Name);
		case WeaponSkinQuality.Basic:
			return Utils.ColorToHex(new Color32(54, 189, byte.MaxValue, byte.MaxValue), skin.Name);
		case WeaponSkinQuality.Professional:
			return Utils.ColorToHex(new Color32(byte.MaxValue, 0, 0, byte.MaxValue), skin.Name);
		case WeaponSkinQuality.Legendary:
			return Utils.ColorToHex(new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue), skin.Name);
		default:
			return skin.Name;
		}
	}

	public void ChangeMode()
	{
		this.SkinMode = !this.SkinMode;
		if (this.SkinMode)
		{
			this.GetWeaponSkins();
			if (this.ChangeButtonLabel != null)
			{
				this.ChangeButtonLabel.text = Localization.Get("Skins", true);
			}
		}
		else if (this.ChangeButtonLabel != null)
		{
			this.ChangeButtonLabel.text = Localization.Get("Weapons", true);
		}
	}

	private void GetWeaponSkins()
	{
		this.SkinList.Clear();
		int weaponID = WeaponManager.GetWeaponID(this.WeaponList[this.SelectWeapon]);
		int weaponSkinSelected = AccountManager.GetWeaponSkinSelected(weaponID);
		WeaponStoreData weaponStoreData = WeaponManager.GetWeaponStoreData(weaponID);
		for (int i = 0; i < weaponStoreData.Skins.Count; i++)
		{
			if (AccountManager.GetWeaponSkin(weaponID, weaponStoreData.Skins[i].ID))
			{
				this.SkinList.Add(weaponStoreData.Skins[i].ID);
				if (weaponSkinSelected == weaponStoreData.Skins[i].ID)
				{
					this.SelectSkin = i;
				}
			}
		}
	}

	private void UpdateSelectedSkin()
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(this.WeaponList[this.SelectWeapon]);
		WeaponManager.SetSelectWeapon(this.Weapon, weaponData.ID);
		AccountManager.SetWeaponSkinSelected(weaponData.ID, this.SkinList[this.SelectSkin]);
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(weaponData.ID, AccountManager.GetWeaponSkinSelected(weaponData.ID));
		this.WeaponNameLabel.text = weaponData.Name + "  |  " + this.GetWeaponSkinRarityColor(weaponSkin);
		this.WeaponSprite.spriteName = weaponData.ID + "-" + weaponSkin.ID;
		if (UISelectWeapon.SelectedUpdateWeaponManager)
		{
			UISelectWeapon.isChange = true;
		}
	}
}
