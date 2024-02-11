using System;
using UnityEngine;

public class UIBuyWeapon : MonoBehaviour
{
    public UIBuyWeapon.Weapon[] Weapons;

    public UILabel WeaponNameLabel;

    public UISprite WeaponSprite;

    public int SelectWeapon;

    private static UILabel MoneyLabel;

    private static CryptoInt money = 800;

    public static int Money
	{
		get
		{
			return UIBuyWeapon.money;
		}
		set
		{
			UIBuyWeapon.money = value;
			UIBuyWeapon.money = Mathf.Clamp(UIBuyWeapon.money, nValue.int0, nValue.int15000);
			UIBuyWeapon.MoneyLabel.text = "$ " + UIBuyWeapon.money.ToString("n0");
		}
	}

	private void Start()
	{
		this.SelectWeapon = 0;
		this.UpdateSelectedWeapon();
	}

	public void Left()
	{
		this.SelectWeapon--;
		if (this.SelectWeapon < 0)
		{
			this.SelectWeapon = this.Weapons.Length - 1;
		}
		this.UpdateSelectedWeapon();
	}

	public void Right()
	{
		this.SelectWeapon++;
		if (this.SelectWeapon > this.Weapons.Length - 1)
		{
			this.SelectWeapon = 0;
		}
		this.UpdateSelectedWeapon();
	}

	public void Buy()
	{
		if (this.Weapons[this.SelectWeapon].money > UIBuyWeapon.Money)
		{
			UIToast.Show(Localization.Get("Not enough money", true));
			return;
		}
		if (this.Weapons[this.SelectWeapon].weapon == PlayerInput.instance.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(this.Weapons[this.SelectWeapon].weapon).Type).ID && PlayerInput.instance.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(this.Weapons[this.SelectWeapon].weapon).Type).Enabled)
		{
			UIToast.Show(Localization.Get("Already available", true));
			return;
		}
		UIBuyWeapon.Money -= this.Weapons[this.SelectWeapon].money;
		WeaponData weaponData = WeaponManager.GetWeaponData(this.Weapons[this.SelectWeapon].weapon);
		WeaponManager.SetSelectWeapon(this.Weapons[this.SelectWeapon].weapon);
		GameManager.player.PlayerWeapon.DropWeapon(weaponData.Type);
		PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(weaponData.Type);
		this.UpdateSelectedWeapon();
	}

	private void UpdateSelectedWeapon()
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(this.Weapons[this.SelectWeapon].weapon);
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(weaponData.ID, AccountManager.GetWeaponSkinSelected(weaponData.ID));
		this.WeaponNameLabel.text = weaponData.Name + "  |  $" + this.Weapons[this.SelectWeapon].money;
		this.WeaponSprite.spriteName = weaponData.ID + "-" + weaponSkin.ID;
		this.WeaponSprite.width = (int)GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].x;
		this.WeaponSprite.height = (int)GameSettings.instance.WeaponsCaseSize[weaponData.ID - 1].y;
	}

	public static void SetActive(bool active)
	{
		if (UIBuyWeapon.MoneyLabel == null)
		{
			UIBuyWeapon.MoneyLabel = UIElements.Get<UILabel>("MoneyLabel");
		}
		UIBuyWeapon.MoneyLabel.cachedGameObject.SetActive(active);
	}

	[Serializable]
	public class Weapon
	{
		[SelectedWeapon]
		public int weapon;

		public CryptoInt money;
	}
}
