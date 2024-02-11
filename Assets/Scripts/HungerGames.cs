using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class HungerGames : Photon.MonoBehaviour
{
    private List<byte> UsedBox = new List<byte>();

    private static HungerGames instance;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.HungerGames)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", new PhotonView.MessageDelegate(this.OnCreatePlayer));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
		base.photonView.AddMessage("MasterPickupBox", new PhotonView.MessageDelegate(this.MasterPickupBox));
		base.photonView.AddMessage("EventPickupBox", new PhotonView.MessageDelegate(this.EventPickupBox));
		base.photonView.AddMessage("PhotonHideBoxes", new PhotonView.MessageDelegate(this.PhotonHideBoxes));
		HungerGames.instance = this;
		UIScore.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = (float)nValue.int3;
		GameManager.friendDamage = true;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		GameManager.globalChat = false;
		CameraManager.SetType(CameraType.Static, new object[0]);
		UIPlayerStatistics.isOnlyBluePanel = true;
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			if (PhotonNetwork.isMasterClient)
			{
				this.ActivationWaitPlayer();
			}
			else if (GameManager.player.Dead)
			{
				CameraManager.SetType(CameraType.Spectate, new object[0]);
			}
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.roundState = RoundState.WaitPlayer;
		this.OnWaitPlayer();
		this.OnCreatePlayer();
	}

	private void OnWaitPlayer()
	{
		UIStatus.Add(Localization.Get("Waiting for other players", true), true);
		TimerManager.In((float)nValue.int4, delegate()
		{
			if (GameManager.roundState == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= nValue.int1)
				{
					this.OnWaitPlayer();
				}
				else
				{
					TimerManager.In((float)nValue.int4, delegate()
					{
						this.OnStartRound();
					});
				}
			}
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.SetScore(playerConnect);
			if (GameManager.roundState != RoundState.WaitPlayer)
			{
				this.CheckPlayers();
			}
			TimerManager.In(nValue.float15, delegate()
			{
				if (this.UsedBox.Count != 0)
				{
					PhotonDataWrite data = this.photonView.GetData();
					data.Write(this.UsedBox.ToArray());
					this.photonView.RPC("PhotonHideBoxes", playerConnect, data);
				}
			});
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.CheckPlayers();
		}
	}

	private void OnStartRound()
	{
		UIDeathScreen.ClearAll();
		DecalsManager.ClearBulletHoles();
		if (PhotonNetwork.playerList.Length <= nValue.int1)
		{
			this.ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			base.photonView.RPC("OnCreatePlayer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnCreatePlayer(PhotonMessage message)
	{
		this.OnCreatePlayer();
	}

	private void OnCreatePlayer()
	{
		EventManager.Dispatch("StartRound");
		this.UsedBox.Clear();
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None, new object[0]);
			player.FPCamera.GetComponent<Camera>().farClipPlane = (float)nValue.int300;
			SpawnPoint playerIDSpawn = SpawnManager.GetPlayerIDSpawn();
			GameManager.controller.ActivePlayer(playerIDSpawn.spawnPosition, playerIDSpawn.spawnRotation);
			WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int4);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		UIStatus.Add(damageInfo);
		UIDeathScreen.Show(damageInfo);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, new object[]
		{
			GameManager.player.FPCamera.Transform.position,
			GameManager.player.FPCamera.Transform.eulerAngles,
			ragdollForce * (float)nValue.int100
		});
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		if (damageInfo.otherPlayer)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		TimerManager.In((float)nValue.int3, delegate()
		{
			if (GameManager.player.Dead)
			{
				CameraManager.SetType(CameraType.Spectate, new object[0]);
			}
		});
	}

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo e = DamageInfo.Serialize(message.ReadBytes());
		EventManager.Dispatch<DamageInfo>("KillPlayer", e);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		if (e.headshot)
		{
			PlayerRoundManager.SetXP(nValue.int12);
			PlayerRoundManager.SetMoney(nValue.int10);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int6);
			PlayerRoundManager.SetMoney(nValue.int5);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
        if (PhotonNetwork.room.playerCount == 1)
        {
            GameManager.roundState = RoundState.WaitPlayer;
        }
        else
        {
            GameManager.roundState = RoundState.EndRound;
        }
        if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.HungerGames);
		}
		else
		{
			float delay = (float)nValue.int8 - (float)(PhotonNetwork.time - message.timestamp);
			TimerManager.In(delay, delegate()
			{
				this.OnStartRound();
			});
		}
	}

	[PunRPC]
	private void CheckPlayers(PhotonMessage message)
	{
		this.CheckPlayers();
	}

	private void CheckPlayers()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState != RoundState.EndRound)
		{
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			bool flag = false;
			int num = -nValue.int1;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (!playerList[i].GetDead())
				{
					if (num != -nValue.int1)
					{
						flag = false;
						break;
					}
					num = playerList[i].ID;
					flag = true;
				}
			}
			if (flag)
			{
				GameManager.blueScore = ++GameManager.blueScore;
				GameManager.SetScore();
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
				UIMainStatus.Add(PhotonPlayer.Find(num).UserId + " [@]", false, (float)nValue.int5, "Win");
			}
		}
	}

	public static void PickupBox(int id)
	{
		PhotonDataWrite data = HungerGames.instance.photonView.GetData();
		data.Write((byte)id);
		HungerGames.instance.photonView.RPC("MasterPickupBox", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void MasterPickupBox(PhotonMessage message)
	{
		byte b = message.ReadByte();
		if (!this.UsedBox.Contains(b))
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(b);
			data.Write(message.sender.ID);
			base.photonView.RPC("EventPickupBox", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void EventPickupBox(PhotonMessage message)
	{
		byte b = message.ReadByte();
		int e = message.ReadInt();
		this.UsedBox.Add(b);
		EventManager.Dispatch<int, int>("EventPickupBox", (int)b, e);
	}

	public static void HideBoxes(byte[] idBoxes)
	{
		PhotonDataWrite data = HungerGames.instance.photonView.GetData();
		data.Write(idBoxes);
		HungerGames.instance.photonView.RPC("PhotonHideBoxes", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonHideBoxes(PhotonMessage message)
	{
		byte[] ids = message.ReadBytes();
		TimerManager.In(nValue.float01, delegate()
		{
			for (int i = nValue.int0; i < ids.Length; i++)
			{
				this.UsedBox.Add(ids[i]);
				EventManager.Dispatch<int, int>("EventPickupBox", (int)ids[i], -nValue.int1);
			}
		});
	}

	public static void SetWeapon(int weaponID)
	{
		WeaponType type = WeaponManager.GetWeaponData(weaponID).Type;
		PlayerInput player = GameManager.player;
		WeaponManager.SetSelectWeapon(weaponID);
		WeaponCustomData weaponCustomData = new WeaponCustomData();
		weaponCustomData.AmmoMax = nValue.int0;
		player.PlayerWeapon.UpdateWeapon(type, true, weaponCustomData);
	}
}
