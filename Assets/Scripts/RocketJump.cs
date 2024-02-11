using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class RocketJump : Photon.MonoBehaviour
{
    public CryptoInt SelectLevel = 1;

    public List<RocketJump.LevelData> Levels;

    private float LastTime;

    private byte pIndex;

    private PhotonView pView;

    private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float005, delegate()
		{
			byte[] array = new byte[nValue.int100];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(i + 1);
			}
			PhotonNetwork.SetSendingEnabled(null, array);
			this.pView = GameManager.controller.photonView;
			this.UpdateGroup();
			base.InvokeRepeating("UpdateActiveGroup", (float)nValue.int0, nValue.float1 / (float)PhotonNetwork.sendRateOnSerialize);
		});
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			GameManager.player.FPController.MotorDoubleJump = true;
			this.OnRevivalPlayer();
			UIScore.UpdateScore(this.Levels.Count, GameManager.blueScore, GameManager.redScore);
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(this.Levels[this.SelectLevel - nValue.int1].Spawn.position, this.Levels[this.SelectLevel - nValue.int1].Spawn.eulerAngles);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int2);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(this.Levels[this.SelectLevel - nValue.int1].Spawn.position, this.Levels[this.SelectLevel - nValue.int1].Spawn.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
		if (player.PlayerTeam != Team.Blue)
		{
			GameManager.team = Team.Blue;
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		GameManager.redScore = ++GameManager.redScore;
		UIScore.UpdateScore(this.Levels.Count, GameManager.blueScore, GameManager.redScore);
		this.OnSpawnPlayer();
	}

	public void NextLevel()
	{
		if (this.LastTime + 2f > Time.time)
		{
			PhotonNetwork.LeaveRoom(true);
		}
		this.LastTime = Time.time;
		this.SelectLevel = ++this.SelectLevel;
		if (this.Levels.Count + nValue.int1 <= this.SelectLevel)
		{
			this.SelectLevel = nValue.int1;
			UIMainStatus.Add(PhotonNetwork.player.UserId + " [@]", false, (float)nValue.int5, "Finished map");
		}
		this.UpdateGroup();
		this.Levels[this.SelectLevel - nValue.int1].Map.SetActive(true);
		this.OnSpawnPlayer();
		PhotonNetwork.player.SetKills(this.SelectLevel);
		GameManager.blueScore = this.SelectLevel;
		PlayerRoundManager.SetXP(nValue.int3);
		PlayerRoundManager.SetMoney(nValue.int5);
		UIScore.UpdateScore(this.Levels.Count, GameManager.blueScore, GameManager.redScore);
		if (this.SelectLevel == nValue.int1)
		{
			this.Levels[this.Levels.Count - nValue.int1].Map.SetActive(false);
		}
		else
		{
			this.Levels[this.SelectLevel - nValue.int2].Map.SetActive(false);
		}
	}

	private void UpdateGroup()
	{
		if (this.pView != null)
		{
			this.pView.group = (byte)this.SelectLevel;
		}
		if (this.SelectLevel == nValue.int1)
		{
			PhotonNetwork.SetInterestGroups(new byte[]
			{
				(byte)this.Levels.Count
			}, new byte[]
			{
				(byte)this.SelectLevel
			});
		}
		else
		{
			PhotonNetwork.SetInterestGroups(new byte[]
			{
				(byte)(this.SelectLevel - nValue.int1)
			}, new byte[]
			{
				(byte)this.SelectLevel
			});
		}
	}

	private void UpdateActiveGroup()
	{
		this.pIndex += 1;
		if (this.pIndex >= 5)
		{
			this.pIndex = 0;
		}
		byte b = this.pIndex;
		if (b != 0)
		{
			if (b == 1)
			{
				this.pView.group = (byte)this.SelectLevel;
			}
		}
		else
		{
			this.pView.group = 0;
		}
	}

	[Serializable]
	public struct LevelData
	{
		public GameObject Map;

		public Transform Spawn;
	}
}
