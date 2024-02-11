using System;
using UnityEngine;

public class UISpectatorElement : MonoBehaviour
{
    public UISprite LineSprite;

    public UILabel NameLabel;

    public UISprite WeaponSprite;

    public UISprite DeadSprite;

    public UILabel HealthLabel;

    public UITexture AvatarTexture;

    public PhotonPlayer Player;

    private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	public void UpdateWidget()
	{
		if (!this.LineSprite.cachedGameObject.activeSelf)
		{
			return;
		}
		this.LineSprite.UpdateWidget();
		this.NameLabel.UpdateWidget();
		this.WeaponSprite.UpdateWidget();
		this.DeadSprite.UpdateWidget();
		this.HealthLabel.UpdateWidget();
	}

	private void OnClick(GameObject go)
	{
		if (go != this.LineSprite.cachedGameObject)
		{
			return;
		}
		if (this.Player == null)
		{
			return;
		}
		if (this.Player.GetDead())
		{
			return;
		}
		CameraManager.selectPlayer = this.Player.ID;
	}

	public bool SetWeapon(int playerID, int weapon, int skin)
	{
		if (this.Player != null && this.Player.ID == playerID)
		{
			this.SetWeapon(weapon, skin);
			return true;
		}
		return false;
	}

	public void SetWeapon(int weapon, int skin)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].ID == weapon)
			{
				this.WeaponSprite.spriteName = weapon + "-" + skin;
				this.WeaponSprite.width = (int)(GameSettings.instance.WeaponsCaseSize[i].x * 0.45f);
				this.WeaponSprite.height = (int)(GameSettings.instance.WeaponsCaseSize[i].y * 0.45f);
				break;
			}
		}
	}

	public void SetData(PhotonPlayer player)
	{
		this.Player = player;
		this.NameLabel.text = player.UserId;
		this.AvatarTexture.mainTexture = AvatarManager.Get(player.GetAvatarUrl());
		this.SetDead(player.GetDead());
	}

	public bool SetDead(int playerID, bool dead)
	{
		if (this.Player != null && this.Player.ID == playerID)
		{
			this.SetDead(dead);
			return true;
		}
		return false;
	}

	public void SetDead(bool dead)
	{
		this.DeadSprite.cachedGameObject.SetActive(dead);
		this.HealthLabel.cachedGameObject.SetActive(!dead);
		this.WeaponSprite.cachedGameObject.SetActive(!dead);
	}

	public bool SetHealth(int playerID, byte health)
	{
		if (this.Player != null && this.Player.ID == playerID)
		{
			this.SetHealth(health);
			return true;
		}
		return false;
	}

	public void SetHealth(byte health)
	{
		if (health == 0)
		{
			this.HealthLabel.cachedGameObject.SetActive(false);
			return;
		}
		this.HealthLabel.text = "+" + health;
	}
}
