using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class EscortMode : Photon.MonoBehaviour
{
    public static bool canEscort;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Escort)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("UpdateTimer", new PhotonView.MessageDelegate(this.UpdateTimer));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("PhotonNextLevel", new PhotonView.MessageDelegate(this.PhotonNextLevel));
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, 0);
		CameraManager.SetType(CameraType.Static, new object[0]);
		UISelectTeam.OnSpectator();
		UISelectTeam.OnStart(new Action<Team>(this.OnSelectTeam));
		GameManager.StartAutoBalance();
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
		EventManager.AddListener<Team>("AutoBalance", new EventManager.Callback<Team>(this.OnAutoBalance));
		Tram.finishCallback = new Action(this.Finish);
		if (PhotonNetwork.isMasterClient)
		{
			this.StartTimer();
		}
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		if (team == Team.None)
		{
			CameraManager.SetType(CameraType.Spectate, new object[0]);
			CameraManager.ChangeType = true;
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
			UISpectator.SetActive(true);
		}
		else
		{
			this.OnRevivalPlayer();
		}
	}

	private void OnAutoBalance(Team team)
	{
		GameManager.team = team;
		this.OnRevivalPlayer();
	}

	private void OnWaitPlayer()
	{
		if (this.GetPlayers().Length <= nValue.int1)
		{
			UIStatus.Add(Localization.Get("Waiting for other players", true), true);
			EscortMode.canEscort = false;
		}
		else
		{
			EscortMode.canEscort = true;
		}
		TimerManager.In((float)nValue.int4, new TimerManager.Callback(this.OnWaitPlayer));
	}

	private void OnRevivalPlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
		Team team = GameManager.team;
		if (team != Team.Blue)
		{
			if (team == Team.Red)
			{
				UIFollowTarget.SetTarget(Tram.GetModel(), player.PlayerCamera, Localization.Get("Attack", true), new Color(0.827f, 0.184f, 0.184f, 0.8f));
			}
		}
		else
		{
			UIFollowTarget.SetTarget(Tram.GetModel(), player.PlayerCamera, Localization.Get("Protect", true), new Color(0f, 0.471f, 0.843f, 0.8f));
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		UIDeathScreen.Show(damageInfo);
		UIStatus.Add(damageInfo);
		UIFollowTarget.Deactive();
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
			GameManager.redScore = ++GameManager.redScore;
			UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		}
		TimerManager.In((float)nValue.int3, delegate()
		{
			this.OnRevivalPlayer();
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient && UIScore.timeData.active)
		{
			TimerManager.In(nValue.float05, delegate()
			{
				PhotonDataWrite data = this.photonView.GetData();
				data.Write(UIScore.timeData.endTime - Time.time);
				this.photonView.RPC("UpdateTimer", playerConnect, data);
			});
		}
	}

	private void StartTimer()
	{
		UIScore.StartTime(900f, new Action(this.StopTimer));
		TimerManager.In(1f, delegate()
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(900f);
			base.photonView.RPC("UpdateTimer", PhotonTargets.Others, data);
		});
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.EndRound;
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Blue Win");
			base.photonView.RPC("PhotonNextLevel", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float time = message.ReadFloat();
		double timestamp = message.timestamp;
		TimerManager.In(nValue.float15, delegate()
		{
			time -= (float)(PhotonNetwork.time - timestamp);
			UIScore.StartTime(time, new Action(this.StopTimer));
		});
	}

	public void Finish()
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound)
		{
			GameManager.roundState = RoundState.EndRound;
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Red Win");
			base.photonView.RPC("PhotonNextLevel", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnKilledPlayer(PhotonMessage message)
	{
		DamageInfo e = DamageInfo.Serialize(message.ReadBytes());
		GameManager.blueScore = ++GameManager.blueScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		EventManager.Dispatch<DamageInfo>("KillPlayer", e);
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetKills1();
		UIDeathScreen.AddKill(message.sender.ID);
		if (e.headshot)
		{
			PlayerRoundManager.SetXP(nValue.int10);
			PlayerRoundManager.SetMoney(nValue.int8);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int4);
		}
	}

	[PunRPC]
	private void PhotonNextLevel(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.Escort);
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
