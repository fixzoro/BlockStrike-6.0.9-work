using System;
using UnityEngine;

public class mServerInfo : MonoBehaviour
{
    public UILabel ServerNameLabel;

    public UILabel ModeLabel;

    public UILabel PlayersLabel;

    public GameObject Password;

    public UISprite mapSprite;

    private RoomInfo room;

    private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	public void SetData(RoomInfo info)
	{
		this.room = info;
		this.ServerNameLabel.text = info.Name;
		if (info.GetGameMode() == GameMode.Only)
		{
			this.ModeLabel.text = Localization.Get(info.GetGameMode().ToString(), true) + " (" + WeaponManager.GetWeaponName(info.GetOnlyWeapon()) + ")";
		}
		else if (info.GetCustomMapHash() != 0)
		{
			this.ModeLabel.text = Localization.Get(info.GetGameMode().ToString(), true) + " | Custom Map | " + info.GetSceneName();
		}
		else
		{
			this.ModeLabel.text = Localization.Get(info.GetGameMode().ToString(), true) + " | " + info.GetSceneName();
		}
		UISpriteData sprite = this.mapSprite.GetSprite(info.GetSceneName());
		if (sprite == null)
		{
			this.mapSprite.spriteName = "CustomMap";
		}
		else
		{
			this.mapSprite.spriteName = sprite.name;
		}
		this.PlayersLabel.text = info.PlayerCount + "/" + info.MaxPlayers;
		if (string.IsNullOrEmpty(info.GetPassword()))
		{
			this.Password.SetActive(false);
		}
		else
		{
			this.Password.SetActive(true);
		}
	}

	private void OnClick(GameObject go)
	{
		if (go != base.gameObject)
		{
			return;
		}
		mJoinServer.room = this.room;
		mJoinServer.onBack = new Action(this.OnBack);
		mJoinServer.Join();
	}

	private void OnBack()
	{
		mPopUp.HideAll("ServerList", false);
	}
}
