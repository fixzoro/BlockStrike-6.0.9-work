using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class SpleefMode : Photon.MonoBehaviour
{
    public SpleefBlock[] blocks;

    private List<short> activeBlocks = new List<short>();

    private List<short> deactiveBlocks = new List<short>();

    public static SpleefMode instance;

    private void Awake()
	{
		SpleefMode.instance = this;
	}

	private void Start()
	{
        base.photonView.AddMessage("OnCreatePlayer", new PhotonView.MessageDelegate(this.OnCreatePlayer));
        base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
        base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
        base.photonView.AddMessage("OnWinPlayer", new PhotonView.MessageDelegate(this.OnWinPlayer));
        base.photonView.AddMessage("PhotonHideBlock", new PhotonView.MessageDelegate(this.PhotonHideBlock));
        base.photonView.AddMessage("PhotonDamage", new PhotonView.MessageDelegate(this.PhotonDamage));
        UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static, new object[0]);
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Knife, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int3);
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

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(this.deactiveBlocks.ToArray());
			base.photonView.RPC("PhotonHideBlock", playerConnect, data);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.CheckPlayers();
		}
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
        UIStatus.Add(Localization.Get("Waiting for other players"), true);
        TimerManager.In((float)nValue.int4, delegate ()
        {
            if (GameManager.roundState == RoundState.WaitPlayer)
            {
                if (PhotonNetwork.playerList.Length <= nValue.int1)
                {
                    this.OnWaitPlayer();
                }
                else
                {
                    TimerManager.In((float)nValue.int4, delegate ()
                    {
                        this.OnStartRound();
                    });
                }
            }
        });
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
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Pistol);
		}
		this.activeBlocks.Clear();
		this.deactiveBlocks.Clear();
		short num = 0;
		while ((int)num < this.blocks.Length)
		{
			this.activeBlocks.Add(num);
			num += 1;
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
			base.photonView.RPC("OnCreatePlayer", PhotonTargets.All);
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.roundState == RoundState.PlayRound)
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
        float delay = (float)nValue.int8 - (float)(PhotonNetwork.time - message.timestamp);
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
        if (!PhotonNetwork.isMasterClient || GameManager.roundState == RoundState.EndRound)
        {
            return;
        }
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
            UIMainStatus.Add(photonPlayer.UserId + " [@]", false, nValue.int5, "Win");
            base.photonView.RPC("OnFinishRound", PhotonTargets.All);
            base.photonView.RPC("OnWinPlayer", photonPlayer);
        }
    }

	private void ResetBlocks()
	{
		for (int i = nValue.int0; i < this.blocks.Length; i++)
		{
			this.blocks[i].cachedGameObject.SetActive(true);
		}
	}

	public void Damage(int id)
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((short)id);
			base.photonView.RPC("PhotonDamage", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonDamage(PhotonMessage message)
	{
		short num = message.ReadShort();
		this.activeBlocks.Remove(num);
		this.deactiveBlocks.Add(num);
		SpleefBlock spleefBlock = this.blocks[(int)num];
		spleefBlock.cachedGameObject.SetActive(false);
	}

	[PunRPC]
	private void PhotonHideBlock(PhotonMessage message)
	{
		short[] array = message.ReadShorts();
		for (int i = nValue.int0; i < array.Length; i++)
		{
			this.activeBlocks.Remove(array[i]);
			this.deactiveBlocks.Add(array[i]);
			SpleefBlock spleefBlock = this.blocks[(int)array[i]];
			spleefBlock.cachedGameObject.SetActive(false);
		}
	}
}
