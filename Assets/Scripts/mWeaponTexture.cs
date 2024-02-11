using UnityEngine;

public class mWeaponTexture : MonoBehaviour
{
    [SelectedWeapon]
    public int Weapon;

    public float Size = 1f;

    public bool OnlyStart;

    public UILabel SkinNameLabel;

    private UISprite WeaponIcon;

    private void Start()
	{
		this.SetTexture();
	}

	private void OnEnable()
	{
		if (this.WeaponIcon == null)
		{
			return;
		}
		if (!this.OnlyStart)
		{
			this.SetTexture();
		}
	}

	[ContextMenu("Set Texture")]
	private void SetTexture()
	{
		if (this.WeaponIcon == null)
		{
			this.WeaponIcon = base.GetComponent<UISprite>();
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].ID == this.Weapon)
			{
				WeaponSkinData randomWeaponSkin = WeaponManager.GetRandomWeaponSkin(GameSettings.instance.Weapons[i].ID);
				this.WeaponIcon.spriteName = this.Weapon + "-" + randomWeaponSkin.ID;
				this.WeaponIcon.width = (int)(GameSettings.instance.WeaponsCaseSize[i].x * this.Size);
				this.WeaponIcon.height = (int)(GameSettings.instance.WeaponsCaseSize[i].y * this.Size);
				if (this.SkinNameLabel != null)
				{
					this.SetSkinName(randomWeaponSkin);
				}
				break;
			}
		}
	}

	private void SetSkinName(WeaponSkinData skinData)
	{
		switch (skinData.Quality)
		{
		case WeaponSkinQuality.Normal:
			this.SkinNameLabel.text = skinData.Name;
			break;
		case WeaponSkinQuality.Basic:
			this.SkinNameLabel.text = "[00aff0]" + skinData.Name;
			break;
		case WeaponSkinQuality.Professional:
			this.SkinNameLabel.text = "[ff0000]" + skinData.Name;
			break;
		case WeaponSkinQuality.Legendary:
			this.SkinNameLabel.text = "[E00061]" + skinData.Name;
			break;
		}
	}
}
