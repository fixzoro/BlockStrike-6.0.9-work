using System;
using Photon;
using UnityEngine;

public class MurderMode : Photon.MonoBehaviour
{
    public static int Murder;

    public static int Detective;

    private int TimerID = -1;

    private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.Murder)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("StartTimer", new PhotonView.MessageDelegate(this.StartTimer));
		base.photonView.AddMessage("SetRoles", new PhotonView.MessageDelegate(this.SetRoles));
		base.photonView.AddMessage("UpdateTimer", new PhotonView.MessageDelegate(this.UpdateTimer));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
		UIPlayerStatistics.isOnlyBluePanel = true;
		UIScore.SetActiveScore(true, 20);
		GameManager.startDamageTime = 1f;
		GameManager.friendDamage = true;
		UIPanelManager.ShowPanel("Display");
		WeaponManager.MaxDamage = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		GameManager.changeWeapons = false;
		GameManager.globalChat = false;
		TimerManager.In(0.5f, delegate()
		{
			GameManager.team = Team.Blue;
			if (PhotonNetwork.isMasterClient)
			{
				this.ActivationWaitPlayer();
			}
			else
			{
				CameraManager.SetType(CameraType.Spectate, new object[0]);
			}
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.DeadPlayer));
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
		TimerManager.In(4f, delegate()
		{
			if (GameManager.roundState == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= 1)
				{
					this.OnWaitPlayer();
				}
				else
				{
					TimerManager.In(4f, delegate()
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
			if (UIScore.timeData.active)
			{
				TimerManager.In(1f, delegate()
				{
					PhotonDataWrite data = this.photonView.GetData();
					data.Write(UIScore.timeData.endTime - Time.time);
					data.Write(MurderMode.Murder);
					data.Write(MurderMode.Detective);
					this.photonView.RPC("UpdateTimer", playerConnect, data);
				});
			}
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (playerDisconnect.ID == MurderMode.Detective)
			{
				MurderModeManager.SetRandomPlayerPistol();
			}
			this.CheckPlayers();
		}
	}

	private void OnStartRound()
	{
		UIDeathScreen.ClearAll();
		DecalsManager.ClearBulletHoles();
		if (PhotonNetwork.playerList.Length <= 1)
		{
			this.ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.StartRound;
			base.photonView.RPC("StartTimer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		EventManager.Dispatch("StartRound");
		TimerManager.Cancel(this.TimerID);
		MurderMode.Murder = -1;
		MurderMode.Detective = -1;
		this.OnCreatePlayer();
		UIToast.Show(Localization.Get("Roles will be given in 15 seconds", true));
		float num = 15f;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		this.TimerID = TimerManager.In(num, delegate()
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonPlayer photonPlayer;
				do
				{
					photonPlayer = PhotonNetwork.playerList[UnityEngine.Random.Range(0, PhotonNetwork.playerList.Length)];
				}
				while (photonPlayer.GetDead());
				PhotonPlayer photonPlayer2;
				do
				{
					photonPlayer2 = PhotonNetwork.playerList[UnityEngine.Random.Range(0, PhotonNetwork.playerList.Length)];
					if (PhotonNetwork.playerList.Length <= 1)
					{
						break;
					}
				}
				while (photonPlayer.ID == photonPlayer2.ID || photonPlayer2.GetDead());
				PhotonDataWrite data = base.photonView.GetData();
				data.Write(photonPlayer.ID);
				data.Write(photonPlayer2.ID);
				base.photonView.RPC("SetRoles", PhotonTargets.All, data);
			}
			GameManager.roundState = RoundState.PlayRound;
		});
	}

	[PunRPC]
	private void SetRoles(PhotonMessage message)
	{
		int murder = message.ReadInt();
		int detective = message.ReadInt();
		float num = 240f;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, new Action(this.StopTimer));
		MurderMode.Murder = murder;
		MurderMode.Detective = detective;
		if (PhotonNetwork.player.GetTeam() != Team.None && !PhotonNetwork.player.GetDead())
		{
			PlayerInput player = GameManager.player;
			if (PhotonNetwork.player.ID == MurderMode.Murder)
			{
				UIToast.Show(Localization.Get("Murder", true));
				WeaponManager.SetSelectWeapon(WeaponType.Knife, 4);
				WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
				WeaponCustomData weaponCustomData = new WeaponCustomData();
				weaponCustomData.BodyDamage = 100;
				weaponCustomData.FaceDamage = 100;
				weaponCustomData.HandDamage = 100;
				weaponCustomData.LegDamage = 100;
				weaponCustomData.CustomData = false;
				WeaponCustomData weaponCustomData2 = new WeaponCustomData();
				weaponCustomData2.Mass = 0.02f;
				weaponCustomData.CustomData = false;
				player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, weaponCustomData, null, weaponCustomData2);
			}
			else if (PhotonNetwork.player.ID == MurderMode.Detective)
			{
				UIToast.Show(Localization.Get("Bystander", true) + " + Deagle");
				WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
				WeaponManager.SetSelectWeapon(WeaponType.Pistol, 2);
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
				WeaponCustomData weaponCustomData3 = new WeaponCustomData();
				weaponCustomData3.Ammo = 1;
				weaponCustomData3.AmmoTotal = 1;
				weaponCustomData3.AmmoMax = 99;
				weaponCustomData3.BodyDamage = 100;
				weaponCustomData3.FaceDamage = 100;
				weaponCustomData3.HandDamage = 100;
				weaponCustomData3.LegDamage = 100;
				weaponCustomData3.Skin = 0;
				weaponCustomData3.FireStatCounter = -1;
				weaponCustomData3.CustomData = false;
				WeaponCustomData weaponCustomData4 = new WeaponCustomData();
				weaponCustomData4.Mass = 0.02f;
				weaponCustomData4.CustomData = false;
				player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, null, weaponCustomData3, weaponCustomData4);
			}
			else
			{
				UIToast.Show(Localization.Get("Bystander", true));
			}
		}
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float time = message.ReadFloat();
		MurderMode.Murder = message.ReadInt();
		MurderMode.Detective = message.ReadInt();
		TimerManager.In(nValue.float15, delegate()
		{
			time -= (float)(PhotonNetwork.time - message.timestamp);
			UIScore.StartTime(time, new Action(this.StopTimer));
		});
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.blueScore = ++GameManager.blueScore;
			GameManager.SetScore();
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Bystander Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private void OnCreatePlayer()
	{
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput player = GameManager.player;
			player.SetHealth(100);
			CameraManager.SetType(CameraType.None, new object[0]);
			SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Red);
			GameManager.controller.ActivePlayer(teamSpawn.spawnPosition, teamSpawn.spawnRotation);
			WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.Ammo = 0;
			weaponCustomData.AmmoMax = 0;
			weaponCustomData.Mass = 0.02f;
			weaponCustomData.CustomData = false;
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, null, null, weaponCustomData);
		}
	}

	private void DeadPlayer(DamageInfo damageInfo)
	{
		PlayerRoundManager.SetDeaths1();
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, new object[]
		{
			GameManager.player.FPCamera.Transform.position,
			GameManager.player.FPCamera.Transform.eulerAngles,
			ragdollForce * 100f
		});
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		if (damageInfo.otherPlayer)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(damageInfo.Deserialize());
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.player), data);
		}
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		MurderModeManager.DeadPlayer();
		TimerManager.In(3f, delegate()
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
		DamageInfo damageInfo = DamageInfo.Serialize(message.ReadBytes());
		if (PhotonNetwork.player.ID == MurderMode.Detective && message.sender.ID != MurderMode.Murder)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
			PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, null, null, null);
			MurderModeManager.DeadBystander();
			return;
		}
		PlayerRoundManager.SetKills1();
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
		UIScore.timeData.active = false;
		TimerManager.Cancel(this.TimerID);
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
			GameManager.LoadNextLevel(GameMode.Murder);
		}
		else
		{
			float delay = 8f - (float)(PhotonNetwork.time - message.timestamp);
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
			byte b = 2;
			bool flag = false;
			for (int i = 0; i < playerList.Length; i++)
			{
				if (playerList[i].ID == MurderMode.Murder)
				{
					flag = true;
					if (playerList[i].GetDead())
					{
						b = 1;
						break;
					}
				}
			}
			if (GameManager.roundState != RoundState.StartRound && (!flag || b == 1))
			{
				GameManager.blueScore = ++GameManager.blueScore;
				GameManager.SetScore();
				UIMainStatus.Add("[@]", false, 5f, "Bystander Win");
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
				return;
			}
			for (int j = 0; j < playerList.Length; j++)
			{
				if (!playerList[j].GetDead() && playerList[j].ID != MurderMode.Murder)
				{
					b = 0;
					break;
				}
			}
			if (b == 2)
			{
				GameManager.redScore = ++GameManager.redScore;
				GameManager.SetScore();
				UIMainStatus.Add("[@]", false, 5f, "Murder Win");
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
			}
		}
	}
}
