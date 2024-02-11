using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class BombMode : Photon.MonoBehaviour
{
    public static BombMode instance;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			BombMode.instance = this;
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonStartRound", new PhotonView.MessageDelegate(this.PhotonStartRound));
		base.photonView.AddMessage("UpdateTimer", new PhotonView.MessageDelegate(this.UpdateTimer));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
		UIScore2.SetActiveScore(true, nValue.int20);
		GameManager.startDamageTime = (float)nValue.int1;
		GameManager.globalChat = false;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static, new object[0]);
		CameraManager.Team = true;
		CameraManager.ChangeType = true;
		UIChangeTeam.SetChangeTeam(true, true);
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(nValue.float05, delegate()
			{
				this.ActivationWaitPlayer();
			});
		}
		else
		{
			UISelectTeam.OnSpectator();
			UISelectTeam.OnStart(new Action<Team>(this.OnSelectTeam));
		}
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

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		if (team == Team.None)
		{
			CameraManager.Team = false;
			CameraManager.SetType(CameraType.Spectate, new object[0]);
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
			UISpectator.SetActive(true);
		}
		else if (GameManager.player.Dead)
		{
			CameraManager.SetType(CameraType.Spectate, new object[0]);
			UISpectator.SetActive(true);
		}
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.roundState = RoundState.WaitPlayer;
		GameManager.team = Team.Red;
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
				if (this.GetPlayers().Length <= nValue.int1)
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
				if (UIScore2.timeData.active && !BombManager.BombPlaced)
				{
					TimerManager.In(nValue.float05, delegate()
					{
						PhotonDataWrite data = this.photonView.GetData();
						data.Write(UIScore2.timeData.endTime - Time.time);
						this.photonView.RPC("UpdateTimer", playerConnect, data);
					});
				}
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
		if (this.GetPlayers().Length <= nValue.int1)
		{
			this.ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			base.photonView.RPC("PhotonStartRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonStartRound(PhotonMessage message)
	{
		EventManager.Dispatch("StartRound");
		float num = (float)nValue.int120;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore2.StartTime(num, new Action(this.StopTimer));
		this.OnCreatePlayer();
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.blueScore = ++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	public void Boom()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			GameManager.redScore = ++GameManager.redScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	public void DeactiveBoom()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			GameManager.blueScore = ++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float time = message.ReadFloat();
		TimerManager.In(nValue.float15, delegate()
		{
			time -= (float)(PhotonNetwork.time - message.timestamp);
			UIScore2.StartTime(time, new Action(this.StopTimer));
		});
	}

	private void OnCreatePlayer()
	{
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			UISpectator.SetActive(false);
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None, new object[0]);
			GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		UIStatus.Add(damageInfo);
		UIDeathScreen.Show(damageInfo);
		PlayerInput player = GameManager.player;
		Vector3 ragdollForce = Utils.GetRagdollForce(player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, new object[]
		{
			GameManager.player.FPCamera.Transform.position,
			player.FPCamera.Transform.eulerAngles,
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
		GameManager.BalanceTeam(true);
		TimerManager.In((float)nValue.int3, delegate()
		{
			if (GameManager.player.Dead)
			{
				UISpectator.SetActive(true);
				CameraManager.SetType(CameraType.Spectate, new object[0]);
			}
		});
		BombManager.DeadPlayer();
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
		UIScore2.timeData.active = false;
        if (PhotonNetwork.room.playerCount == 1)
        {
            GameManager.roundState = RoundState.WaitPlayer;
        }
        else
        {
            GameManager.roundState = RoundState.EndRound;
        }
        GameManager.BalanceTeam(true);
		if (GameManager.blueScore + GameManager.redScore == GameManager.maxScore / nValue.int2)
		{
			if (PhotonNetwork.isMasterClient)
			{
				int value = GameManager.blueScore;
				GameManager.blueScore = GameManager.redScore;
				GameManager.redScore = value;
				GameManager.SetScore();
			}
			if (PhotonNetwork.player.GetTeam() == Team.Blue)
			{
				GameManager.team = Team.Red;
			}
			else if (PhotonNetwork.player.GetTeam() == Team.Red)
			{
				GameManager.team = Team.Blue;
			}
		}
		else
		{
			GameManager.BalanceTeam(true);
		}
		if (GameManager.checkScore)
		{
			GameManager.LoadNextLevel(GameMode.Bomb);
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
			bool flag2 = false;
			for (int i = nValue.int0; i < playerList.Length; i++)
			{
				if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
				{
					flag = true;
					break;
				}
			}
			if (!BombManager.BombPlaced)
			{
				for (int j = nValue.int0; j < playerList.Length; j++)
				{
					if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
					{
						flag2 = true;
						break;
					}
				}
			}
			else
			{
				flag2 = true;
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

	private PhotonPlayer[] GetPlayers()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() != Team.None)
			{
				list.Add(playerList[i]);
			}
		}
		return list.ToArray();
	}
}
