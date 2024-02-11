using System;
using Photon;
using UnityEngine;

public class Deathmatch : Photon.MonoBehaviour
{
    public CryptoInt MaxScore = 50;

    private CryptoInt BlueScore = 0;

    private CryptoInt RedScore = 0;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Deathmatch)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonOnScore", new PhotonView.MessageDelegate(this.PhotonOnScore));
		base.photonView.AddMessage("OnKilledPlayer", new PhotonView.MessageDelegate(this.OnKilledPlayer));
		base.photonView.AddMessage("PhotonNextLevel", new PhotonView.MessageDelegate(this.PhotonNextLevel));
		GameManager.roundState = RoundState.PlayRound;
		GameManager.startDamageTime = (float)nValue.int1;
		GameManager.friendDamage = true;
		UIScore.SetActiveScore(true, this.MaxScore);
		GameManager.maxScore = this.MaxScore;
		UIPanelManager.ShowPanel("Display");
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			this.OnRevivalPlayer();
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	private void OnRevivalPlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		SpawnPoint randomSpawn = SpawnManager.GetRandomSpawn();
		GameManager.controller.ActivePlayer(randomSpawn.spawnPosition, randomSpawn.spawnRotation);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		PlayerRoundManager.SetDeaths1();
		this.RedScore = ++this.RedScore;
		UIScore.UpdateScore(this.MaxScore, this.BlueScore, this.RedScore);
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
		TimerManager.In((float)nValue.int3, delegate()
		{
			this.OnRevivalPlayer();
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
		this.BlueScore = ++this.BlueScore;
		UIScore.UpdateScore(this.MaxScore, this.BlueScore, this.RedScore);
		if (this.BlueScore >= this.MaxScore)
		{
			this.OnScore(PhotonNetwork.player);
		}
		if (e.headshot)
		{
			PlayerRoundManager.SetXP(nValue.int10);
			PlayerRoundManager.SetMoney(nValue.int6);
			PlayerRoundManager.SetHeadshot1();
		}
		else
		{
			PlayerRoundManager.SetXP(nValue.int5);
			PlayerRoundManager.SetMoney(nValue.int3);
		}
	}

	public void OnScore(PhotonPlayer player)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(player);
		base.photonView.RPC("PhotonOnScore", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void PhotonOnScore(PhotonMessage message)
	{
		if (GameManager.roundState != RoundState.PlayRound)
		{
			return;
		}
		PhotonPlayer photonPlayer = PhotonPlayer.Find(message.ReadInt());
		GameManager.roundState = RoundState.EndRound;
		UIMainStatus.Add(photonPlayer.UserId + " [@]", false, (float)nValue.int5, "Win");
		base.photonView.RPC("PhotonNextLevel", PhotonTargets.All);
	}

	[PunRPC]
	private void PhotonNextLevel(PhotonMessage message)
	{
		GameManager.LoadNextLevel(GameMode.Deathmatch);
	}
}
