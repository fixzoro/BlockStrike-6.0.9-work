using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class UISpectator : Photon.MonoBehaviour
{
    public GameObject Panel;

    public UIGrid BlueGrid;

    public UIGrid RedGrid;

    public UISpectatorElement[] BlueTeam;

    public UISpectatorElement[] RedTeam;

    public UISprite InfoSprite;

    public UISprite GradientSprite;

    public UILabel NameLabel;

    public UITexture AvatarTexture;

    public UILabel PlayerIdLabel;

    public UILabel ClanLabel;

    public UILabel KillsLabel;

    public UILabel DeathsLabel;

    public UILabel PingLabel;

    public UILabel HealthLabel;

    private PhotonPlayer selectPlayer;

    private bool isActive;

    private int InfoTimerId = -1;

    private static UISpectator instance;

    private void Start()
	{
		UISpectator.instance = this;
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		ControllerManager.SetWeaponEvent -= this.SetWeapon;
		ControllerManager.SetDeadEvent -= this.SetDead;
		ControllerManager.SetTeamEvent -= this.SetTeam;
		ControllerManager.SetHealthEvent -= this.SetHealth;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		CameraManager.selectPlayerEvent -= UISpectator.SetSelectPlayer;
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void GetButtonDown(string name)
	{
		if (this.isActive && name == "Reload")
		{
			this.BlueGrid.gameObject.SetActive(!this.BlueGrid.gameObject.activeSelf);
			this.RedGrid.gameObject.SetActive(!this.RedGrid.gameObject.activeSelf);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (!this.isActive)
		{
			return;
		}
		this.UpdateList();
	}

	public static void SetActive(bool active)
	{
		UISpectator.instance.Panel.SetActive(active);
		UISpectator.instance.isActive = active;
		if (active)
		{
			ControllerManager.SetWeaponEvent += UISpectator.instance.SetWeapon;
			ControllerManager.SetDeadEvent += UISpectator.instance.SetDead;
			ControllerManager.SetTeamEvent += UISpectator.instance.SetTeam;
			ControllerManager.SetHealthEvent += UISpectator.instance.SetHealth;
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(UISpectator.instance.GetButtonDown));
			CameraManager.selectPlayerEvent += UISpectator.SetSelectPlayer;
			UISpectator.instance.UpdateList();
		}
		else
		{
			UISpectator.instance.OnDisable();
		}
	}

	public static bool GetActive()
	{
		return UISpectator.instance.isActive;
	}

	public static void SetSelectPlayer(int playerID)
	{
		if (!UISpectator.instance.isActive)
		{
			return;
		}
		UISpectator.instance.selectPlayer = PhotonPlayer.Find(playerID);
		if (UISpectator.instance.selectPlayer != null && CameraManager.type == CameraType.FirstPerson)
		{
			UIToast.Show(UISpectator.instance.selectPlayer.UserId);
		}
	}

	private void UpdateDataSelectPlayer()
	{
		if (this.selectPlayer == null)
		{
			this.InfoSprite.cachedGameObject.SetActive(false);
			TimerManager.Cancel(this.InfoTimerId);
			return;
		}
		this.KillsLabel.text = StringCache.Get(this.selectPlayer.GetKills());
		this.DeathsLabel.text = StringCache.Get(this.selectPlayer.GetDeaths());
		this.PingLabel.text = StringCache.Get(this.selectPlayer.GetPing());
	}

	private void UpdateList()
	{
		for (int i = 0; i < this.BlueTeam.Length; i++)
		{
			this.BlueTeam[i].LineSprite.cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < this.RedTeam.Length; j++)
		{
			this.RedTeam[j].LineSprite.cachedGameObject.SetActive(false);
		}
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int k = 0; k < playerList.Length; k++)
		{
			if (playerList[k].GetTeam() == Team.Blue && !playerList[k].IsLocal)
			{
				list.Add(playerList[k]);
			}
			else if (playerList[k].GetTeam() == Team.Red && !playerList[k].IsLocal)
			{
				list2.Add(playerList[k]);
			}
		}
		list.Sort(new Comparison<PhotonPlayer>(UISpectator.SortByKills));
		list2.Sort(new Comparison<PhotonPlayer>(UISpectator.SortByKills));
		if (PhotonNetwork.player.GetTeam() != Team.Red)
		{
			for (int l = 0; l < list.Count; l++)
			{
				if (l > 5)
				{
					break;
				}
				this.BlueTeam[l].LineSprite.cachedGameObject.SetActive(true);
				this.BlueTeam[l].SetData(list[l]);
				ControllerManager controllerManager = ControllerManager.FindController(list[l].ID);
				if (controllerManager != null && controllerManager.playerSkin != null)
				{
					this.BlueTeam[l].SetHealth(controllerManager.playerSkin.Health);
					if (controllerManager.playerSkin.SelectWeapon != null)
					{
						this.BlueTeam[l].SetWeapon(controllerManager.playerSkin.SelectWeapon.Data.weapon, controllerManager.playerSkin.SelectWeapon.Data.skin);
					}
				}
			}
			this.BlueGrid.Reposition();
			for (int m = 0; m < this.BlueTeam.Length; m++)
			{
				this.BlueTeam[m].UpdateWidget();
			}
		}
		if (PhotonNetwork.player.GetTeam() != Team.Blue)
		{
			for (int n = 0; n < list2.Count; n++)
			{
				if (n > 5)
				{
					break;
				}
				this.RedTeam[n].LineSprite.cachedGameObject.SetActive(true);
				this.RedTeam[n].SetData(list2[n]);
				ControllerManager controllerManager = ControllerManager.FindController(list2[n].ID);
				if (controllerManager != null && controllerManager.playerSkin != null)
				{
					this.RedTeam[n].SetHealth(controllerManager.playerSkin.Health);
					if (controllerManager.playerSkin.SelectWeapon != null)
					{
						this.RedTeam[n].SetWeapon(controllerManager.playerSkin.SelectWeapon.Data.weapon, controllerManager.playerSkin.SelectWeapon.Data.skin);
					}
				}
			}
			this.RedGrid.Reposition();
			for (int num = 0; num < this.RedTeam.Length; num++)
			{
				this.RedTeam[num].UpdateWidget();
			}
		}
	}

	public static int SortByKills(PhotonPlayer a, PhotonPlayer b)
	{
		if (a.GetKills() != b.GetKills())
		{
			return b.GetKills().CompareTo(a.GetKills());
		}
		if (a.GetDeaths() != b.GetDeaths())
		{
			return a.GetDeaths().CompareTo(b.GetDeaths());
		}
		if (a.GetLevel() == b.GetLevel())
		{
			return b.UserId.CompareTo(a.UserId);
		}
		return b.GetLevel().CompareTo(a.GetLevel());
	}

	private void SetWeapon(int playerID, int weaponID, int skinID)
	{
		if (!this.isActive)
		{
			return;
		}
		for (int i = 0; i < this.BlueTeam.Length; i++)
		{
			if (this.BlueTeam[i].SetWeapon(playerID, weaponID, skinID))
			{
				return;
			}
		}
		for (int j = 0; j < this.RedTeam.Length; j++)
		{
			if (this.RedTeam[j].SetWeapon(playerID, weaponID, skinID))
			{
				return;
			}
		}
	}

	private void SetDead(int playerID, bool dead)
	{
		if (!this.isActive)
		{
			return;
		}
		for (int i = 0; i < this.BlueTeam.Length; i++)
		{
			if (this.BlueTeam[i].SetDead(playerID, dead))
			{
				return;
			}
		}
		for (int j = 0; j < this.RedTeam.Length; j++)
		{
			if (this.RedTeam[j].SetDead(playerID, dead))
			{
				return;
			}
		}
	}

	private void SetHealth(int playerID, byte health)
	{
		if (!this.isActive)
		{
			return;
		}
		if (this.selectPlayer != null && this.selectPlayer.ID == playerID)
		{
			this.HealthLabel.text = "+" + StringCache.Get((int)health);
		}
		for (int i = 0; i < this.BlueTeam.Length; i++)
		{
			if (this.BlueTeam[i].SetHealth(playerID, health))
			{
				return;
			}
		}
		for (int j = 0; j < this.RedTeam.Length; j++)
		{
			if (this.RedTeam[j].SetHealth(playerID, health))
			{
				return;
			}
		}
	}

	private void SetTeam(int playerID, Team team)
	{
		if (!this.isActive)
		{
			return;
		}
		this.UpdateList();
	}
}
