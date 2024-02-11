using System;
using Photon;
using UnityEngine;

public class BunnyHop : Photon.MonoBehaviour
{
    private Vector3 StartSpawnPosition;

    private Quaternion StartSpawnRotation;

    private static BunnyHop instance;

    private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.BunnyHop)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		BunnyHop.instance = this;
		base.photonView.AddMessage("StartTimer", new PhotonView.MessageDelegate(this.StartTimer));
		base.photonView.AddMessage("UpdateTimer", new PhotonView.MessageDelegate(this.UpdateTimer));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("PhotonSetTopList", new PhotonView.MessageDelegate(this.PhotonSetTopList));
		base.photonView.AddMessage("PhotonSetDataTopList", new PhotonView.MessageDelegate(this.PhotonSetDataTopList));
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			this.StartSpawnPosition = SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position;
			this.StartSpawnRotation = SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.rotation;
			PlayerInput player = GameManager.player;
			player.BunnyHopEnabled = true;
			player.FPController.MotorJumpForce = nValue.float02;
			player.FPController.MotorAirSpeed = (float)nValue.int1;
			this.OnRevivalPlayer();
			BunnyHopTop.StartTimer();
			TimerManager.In(nValue.float15, delegate()
			{
				if (PhotonNetwork.isMasterClient && !LevelManager.customScene)
				{
					base.photonView.RPC("StartTimer", PhotonTargets.All);
				}
			});
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		GameManager.redScore = ++GameManager.redScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		this.OnSpawnPlayer();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (LevelManager.customScene)
		{
			return;
		}
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(1f, delegate()
			{
				if (UIScore.timeData.active)
				{
					PhotonDataWrite data = this.photonView.GetData();
					data.Write(UIScore.timeData.endTime - Time.time);
					this.photonView.RPC("UpdateTimer", playerConnect, data);
				}
				string topList = BunnyHopTop.GetTopList();
				if (!string.IsNullOrEmpty(topList))
				{
					PhotonDataWrite data2 = this.photonView.GetData();
					data2.Write(topList);
					this.photonView.RPC("PhotonSetTopList", playerConnect, data2);
				}
			});
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessage message)
	{
		float num = (float)nValue.int900;
		num -= (float)(PhotonNetwork.time - message.timestamp);
		UIScore.StartTime(num, new Action(this.StopTimer));
	}

	[PunRPC]
	private void UpdateTimer(PhotonMessage message)
	{
		float num = message.ReadFloat();
		double timestamp = message.timestamp;
		num -= (float)(PhotonNetwork.time - timestamp);
		UIScore.StartTime(num, new Action(this.StopTimer));
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.EndRound;
			UIMainStatus.Add("[@]", false, (float)nValue.int5, "Next Map");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.BunnyHop);
	}

	public static void FinishMap(int xp, int money)
	{
		Transform cachedTransform = SpawnManager.GetTeamSpawn().cachedTransform;
		cachedTransform.position = BunnyHop.instance.StartSpawnPosition;
		cachedTransform.rotation = BunnyHop.instance.StartSpawnRotation;
		UIMainStatus.Add(PhotonNetwork.player.UserId + " [@]", false, (float)nValue.int5, "Finished map");
		PlayerRoundManager.SetXP(xp, true);
		PlayerRoundManager.SetMoney(money, true);
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn().spawnPosition, Vector3.up * (float)UnityEngine.Random.Range(nValue.int0, nValue.int360));
		PhotonNetwork.player.SetKills1();
		GameManager.blueScore = ++GameManager.blueScore;
		UIScore.UpdateScore(nValue.int0, GameManager.blueScore, GameManager.redScore);
		BunnyHopTop.RestartTimer();
	}

	public static void SpawnDead()
	{
		BunnyHop.instance.OnSpawnPlayer();
	}

	[PunRPC]
	private void PhotonSetTopList(PhotonMessage message)
	{
		BunnyHopTop.SetTopList(message.ReadString());
	}

	public static void SetDataTopList(float time, int deaths)
	{
		PhotonDataWrite data = BunnyHop.instance.photonView.GetData();
		data.Write(time);
		data.Write(deaths);
		BunnyHop.instance.photonView.RPC("PhotonSetDataTopList", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonSetDataTopList(PhotonMessage message)
	{
		float time = message.ReadFloat();
		int deaths = message.ReadInt();
		BunnyHopTop.UpdateData(message.sender.UserId, time, deaths);
	}
}
