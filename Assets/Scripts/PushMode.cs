using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PushMode : Photon.MonoBehaviour
{
    public GameObject[] blocks;

    private List<byte> activeBlocks = new List<byte>();

    private List<byte> deactiveBlocks = new List<byte>();

    public Material defaultBlock;

    public Material damageBlock;

    private int mainTimer;

    private int damageTimer;

    private void Start()
	{
        base.photonView.AddMessage("OnCreatePlayer", new PhotonView.MessageDelegate(this.OnCreatePlayer));
        base.photonView.AddMessage("PhotonStartRound", new PhotonView.MessageDelegate(this.OnCreatePlayer));
        base.photonView.AddMessage("OnWinPlayer", new PhotonView.MessageDelegate(this.OnWinPlayer));
        base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
        base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
        base.photonView.AddMessage("PhotonHideBlock", new PhotonView.MessageDelegate(this.PhotonHideBlock));
        UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		GameManager.friendDamage = true;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static, new object[0]);
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int2);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
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
			base.photonView.RPC("PhotonHideBlock", playerConnect, this.deactiveBlocks.ToArray());
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.CheckPlayers();
		}
	}

    [PunRPC]
    private void OnCreatePlayer(PhotonMessage message)
    {
        this.OnCreatePlayer();
    }

	private void OnCreatePlayer()
	{
		if (PhotonNetwork.player.GetTeam() != Team.None)
		{
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None, new object[0]);
			SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Blue);
			GameManager.controller.ActivePlayer(teamSpawn.spawnPosition, new Vector3((float)nValue.int0, (float)UnityEngine.Random.Range(nValue.int0, nValue.int360), (float)nValue.int0));
			player.PlayerWeapon.InfiniteAmmo = true;
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.BodyDamage = nValue.int0;
			weaponCustomData.FaceDamage = nValue.int0;
			weaponCustomData.HandDamage = nValue.int0;
			weaponCustomData.LegDamage = nValue.int0;
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol, null, weaponCustomData, null);
			player.DamageForce = nValue.int40;
			player.FPController.MotorDoubleJump = true;
		}
	}

	private void OnStartRound()
	{
		this.ResetBlocks();
		if (PhotonNetwork.playerList.Length <= nValue.int1)
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
        PhotonStartRound();
    }

    private void PhotonStartRound()
	{
		this.StartTimer();
		this.OnCreatePlayer();
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			if (damageInfo.player == -nValue.int1)
			{
				PhotonNetwork.player.SetDeaths1();
				PlayerRoundManager.SetDeaths1();
				UIStatus.Add(damageInfo);
				Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
				CameraManager.SetType(CameraType.Dead, new object[]
				{
					GameManager.player.FPCamera.Transform.position,
					GameManager.player.FPCamera.Transform.eulerAngles,
					ragdollForce * (float)nValue.int100
				});
				GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
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
		else
		{
			this.OnCreatePlayer();
		}
	}

	[PunRPC]
	private void OnWinPlayer(PhotonMessage message)
	{
		PhotonNetwork.player.SetKills1();
		PlayerRoundManager.SetXP(nValue.int12);
		PlayerRoundManager.SetMoney(nValue.int10);
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessage info)
	{
		TimerManager.Cancel(this.mainTimer);
		TimerManager.Cancel(this.damageTimer);
        if (PhotonNetwork.room.playerCount == 1)
        {
            GameManager.roundState = RoundState.WaitPlayer;
        }
        else
        {
            GameManager.roundState = RoundState.EndRound;
        }
        float delay = (float)nValue.int8 - (float)(PhotonNetwork.time - info.timestamp);
		TimerManager.In(delay, delegate()
		{
			this.OnStartRound();
		});
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
			PhotonPlayer photonPlayer = null;
			for (int i = nValue.int0; i < playerList.Length; i++)
			{
				if (!playerList[i].GetDead())
				{
					if (photonPlayer != null)
					{
						return;
					}
					photonPlayer = playerList[i];
				}
			}
			if (photonPlayer != null)
			{
				UIMainStatus.Add(photonPlayer.UserId + " [@]", false, (float)nValue.int5, "Win");
				base.photonView.RPC("OnFinishRound", PhotonTargets.All);
				base.photonView.RPC("OnWinPlayer", photonPlayer);
			}
		}
	}

	private void ResetBlocks()
	{
		for (int i = nValue.int0; i < this.blocks.Length; i++)
		{
			this.blocks[i].SetActive(true);
			this.blocks[i].GetComponent<Renderer>().material = this.defaultBlock;
		}
	}

	public void StartTimer()
	{
		this.activeBlocks.Clear();
		this.deactiveBlocks.Clear();
		byte b = 0;
		while ((int)b < this.blocks.Length)
		{
			this.activeBlocks.Add(b);
			b += 1;
		}
		this.mainTimer = TimerManager.In((float)nValue.int3, -nValue.int1, (float)nValue.int3, delegate()
		{
			if (PhotonNetwork.isMasterClient && this.activeBlocks.Count != nValue.int1)
			{
				byte[] array;
				if (this.activeBlocks.Count > nValue.int200)
				{
					array = new byte[nValue.int5];
				}
				else if (this.activeBlocks.Count > nValue.int150)
				{
					array = new byte[nValue.int4];
				}
				else if (this.activeBlocks.Count > nValue.int100)
				{
					array = new byte[nValue.int3];
				}
				else
				{
					array = new byte[nValue.int2];
				}
				if (this.activeBlocks.Count <= nValue.int2)
				{
					array = new byte[nValue.int1];
				}
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.activeBlocks[UnityEngine.Random.Range(nValue.int0, this.activeBlocks.Count)];
					this.activeBlocks.Remove(array[i]);
				}
                PhotonDataWrite data = base.photonView.GetData();
                data.Write(array);
                base.photonView.RPC("PhotonHideBlock", PhotonTargets.All, array);
			}
		});
	}

	[PunRPC]
	private void PhotonHideBlock(PhotonMessage message)
	{
		if (message.ReadBytes().Length > nValue.int5)
		{
			if (GameManager.roundState == RoundState.PlayRound)
			{
				for (int i = nValue.int0; i < message.ReadBytes().Length; i++)
				{
					this.blocks[(int)message.ReadBytes()[i]].SetActive(false);
				}
			}
		}
		else
		{
			for (int j = nValue.int0; j < message.ReadBytes().Length; j++)
			{
				byte id = message.ReadBytes()[j];
				this.blocks[(int)id].GetComponent<Renderer>().material = this.damageBlock;
				this.activeBlocks.Remove(id);
				this.deactiveBlocks.Add(id);
				this.damageTimer = TimerManager.In(nValue.float15, delegate()
				{
					if (GameManager.roundState == RoundState.PlayRound)
					{
						this.blocks[(int)id].SetActive(false);
					}
				});
			}
		}
	}
}
