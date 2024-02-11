using System;
using System.Collections.Generic;
using System.Linq;
using Photon;
using UnityEngine;

public class DeathRun : Photon.MonoBehaviour
{
    private static DeathRun instance;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.DeathRun)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		DeathRun.instance = this;
		base.photonView.AddMessage("OnSendKillerInfo", new PhotonView.MessageDelegate(this.OnSendKillerInfo));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
		base.photonView.AddMessage("PhotonClickButton", new PhotonView.MessageDelegate(this.PhotonClickButton));
		UIScore.SetActiveScore(true, GameManager.maxScore);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.maxScore = nValue.int20;
		GameManager.changeWeapons = false;
		GameManager.globalChat = false;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float05, delegate()
		{
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
		GameManager.team = Team.Blue;
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
			List<PhotonPlayer> list = PhotonNetwork.playerList.ToList<PhotonPlayer>();
			int num = this.OnSelectMaxDeaths(list.Count);
			string text = string.Empty;
			for (int i = 0; i < num; i++)
			{
				int index = UnityEngine.Random.Range(nValue.int0, list.Count);
				text = text + list[index].ID + "#";
				list.RemoveAt(index);
			}
			GameManager.roundState = RoundState.PlayRound;
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(text);
			base.photonView.RPC("OnSendKillerInfo", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void OnSendKillerInfo(PhotonMessage message)
	{
		string text = message.ReadString();
		string[] array = text.Split(new char[]
		{
			"#"[nValue.int0]
		});
		bool flag = false;
		for (int i = nValue.int0; i < array.Length - nValue.int1; i++)
		{
			if (PhotonNetwork.player.ID == int.Parse(array[i]))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameManager.team = Team.Red;
		}
		else
		{
			GameManager.team = Team.Blue;
		}
		EventManager.Dispatch("StartRound");
		this.OnCreatePlayer();
		TimerManager.In((float)nValue.int3, delegate()
		{
			if (PhotonNetwork.isMasterClient)
			{
				this.CheckPlayers();
			}
		});
	}

	private void OnCreatePlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		if (PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			player.UpdatePlayerSpeed(nValue.float02);
		}
		else
		{
			player.UpdatePlayerSpeed(nValue.float04);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		EventManager.Dispatch<DamageInfo>("KillPlayer", damageInfo);
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonNetwork.player.SetDeaths1();
			PlayerRoundManager.SetDeaths1();
		}
		if (GameManager.roundState == RoundState.PlayRound)
		{
			UIStatus.Add(damageInfo);
			UIDeathScreen.Show(damageInfo);
		}
		if (GameManager.roundState == RoundState.PlayRound)
		{
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
			CameraManager.SetType(CameraType.Dead, new object[]
			{
				GameManager.player.FPCamera.Transform.position,
				GameManager.player.FPCamera.Transform.eulerAngles,
				ragdollForce * (float)nValue.int100
			});
			GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		}
		else
		{
			this.OnCreatePlayer();
		}
		if (damageInfo.otherPlayer)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		if (GameManager.roundState == RoundState.PlayRound)
		{
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
			TimerManager.In((float)nValue.int3, delegate()
			{
				if (GameManager.player.Dead)
				{
					CameraManager.SetType(CameraType.Spectate, new object[0]);
				}
			});
		}
	}

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo damageInfo = DamageInfo.Serialize(message.ReadBytes());
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		if (damageInfo.headshot)
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
			GameManager.LoadNextLevel(GameMode.DeathRun);
		}
		else
		{
			float num = (float)nValue.int8 - (float)(PhotonNetwork.time - message.timestamp);
			if (num <= (float)nValue.int0)
			{
				num = nValue.float01;
			}
			TimerManager.In(num, delegate()
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
			bool flag2 = false;
			for (int i = nValue.int0; i < playerList.Length; i++)
			{
				if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
				{
					flag = true;
					break;
				}
			}
			for (int j = nValue.int0; j < playerList.Length; j++)
			{
				if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
				{
					flag2 = true;
					break;
				}
			}
			if (!flag)
			{
				GameManager.redScore = ++GameManager.redScore;
				GameManager.SetScore();
				UIMainStatus.Add("[@]", false, (float)nValue.int5, "Red Win");
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			}
			else if (!flag2)
			{
				GameManager.blueScore = ++GameManager.blueScore;
				GameManager.SetScore();
				UIMainStatus.Add("[@]", false, (float)nValue.int5, "Blue Win");
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			}
		}
	}

	public static void ClickButton(byte button)
	{
		PhotonDataWrite data = DeathRun.instance.photonView.GetData();
		data.Write(button);
		DeathRun.instance.photonView.RPC("PhotonClickButton", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonClickButton(PhotonMessage message)
	{
		EventManager.Dispatch<byte>("DeathRunClickButton", message.ReadByte());
	}

	private int OnSelectMaxDeaths(int maxPlayers)
	{
		if (maxPlayers > nValue.int6)
		{
			return nValue.int2;
		}
		return nValue.int1;
	}
}
