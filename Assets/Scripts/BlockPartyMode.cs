using System;
using Photon;
using UnityEngine;

public class BlockPartyMode : Photon.MonoBehaviour
{
    public nMonoBehaviour[] blocks;

    public Color32[] colors;

    public int selectColor;

    public MeshFilter line;

    private System.Random rand = new System.Random(5);

    private int[] timers = new int[2];

    private float duration = 5f;

    private void Start()
	{
		base.photonView.AddMessage("OnCreatePlayer", new PhotonView.MessageDelegate(this.OnCreatePlayer));
		base.photonView.AddMessage("PhotonStartRound", new PhotonView.MessageDelegate(this.PhotonStartRound));
		base.photonView.AddMessage("OnFinishRound", new PhotonView.MessageDelegate(this.OnFinishRound));
		base.photonView.AddMessage("CheckPlayers", new PhotonView.MessageDelegate(this.CheckPlayers));
		base.photonView.AddMessage("OnWinPlayer", new PhotonView.MessageDelegate(this.OnWinPlayer));
		GameOthers.pauseInterval = 200;
		UIScore.SetActiveScore(false);
		GameManager.startDamageTime = nValue.float02;
		UIPanelManager.ShowPanel("Display");
		CameraManager.SetType(CameraType.Static, new object[0]);
		UIPlayerStatistics.isOnlyBluePanel = true;
		GameManager.changeWeapons = false;
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		CameraManager.main.cameraTransform.GetComponent<Camera>().renderingPath = RenderingPath.Forward;
		TimerManager.In(nValue.float05, delegate()
		{
			PlayerInput.instance.PlayerCamera.renderingPath = RenderingPath.Forward;
			PlayerInput.instance.AfkEnabled = true;
			PlayerInput.instance.AfkDuration = 15f;
			PlayerInput.instance.StartAFK();
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
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
			PlayerInput player = GameManager.player;
			player.SetHealth(nValue.int100);
			CameraManager.SetType(CameraType.None, new object[0]);
			SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Blue);
			GameManager.controller.ActivePlayer(teamSpawn.spawnPosition, new Vector3((float)nValue.int0, (float)UnityEngine.Random.Range(nValue.int0, nValue.int360), (float)nValue.int0));
			player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		}
	}

	private void OnStartRound()
	{
		if (PhotonNetwork.playerList.Length <= nValue.int1)
		{
			this.ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			GameManager.roundState = RoundState.PlayRound;
			PhotonDataWrite data = base.photonView.GetData();
			data.Write(UnityEngine.Random.Range(0, 100000));
			base.photonView.RPC("PhotonStartRound", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonStartRound(PhotonMessage message)
	{
		int seed = message.ReadInt();
		this.rand = new System.Random(seed);
		this.duration = (float)nValue.int5;
		this.OnCreatePlayer();
		this.SelectRandomColor();
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
		this.OnWinPlayer();
	}

	private void OnWinPlayer()
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
        UIDuration.StopDuration();
		TimerManager.Cancel(this.timers);
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
			this.blocks[i].cachedGameObject.SetActive(true);
		}
	}

	private void SetColor()
	{
		Color32[] array = new Color32[this.blocks[nValue.int0].cachedMeshFilter.mesh.vertices.Length];
		for (int i = nValue.int0; i < this.blocks.Length; i++)
		{
			int num = this.rand.Next(nValue.int0, this.colors.Length);
			for (int j = nValue.int0; j < array.Length; j++)
			{
				array[j] = this.colors[num];
			}
			this.blocks[i].cachedMeshFilter.mesh.colors32 = array;
			this.blocks[i].cachedGameObject.name = num.ToString();
		}
	}

	private void SetLineColor()
	{
		Color32[] array = new Color32[this.line.mesh.vertices.Length];
		for (int i = nValue.int0; i < array.Length; i++)
		{
			array[i] = this.colors[this.selectColor];
		}
		this.line.mesh.colors32 = array;
	}

	private void SelectRandomColor()
	{
		this.ResetBlocks();
		this.SetColor();
		this.selectColor = this.rand.Next(nValue.int0, this.colors.Length);
		this.SetLineColor();
		this.duration -= nValue.float01;
		this.duration = Mathf.Clamp(this.duration, 2f, 5f);
		this.timers[nValue.int0] = TimerManager.In(this.duration, new TimerManager.Callback(this.DeactiveBlocks));
		UIDuration.duration.color = new Color32((byte)(this.colors[this.selectColor].r - 40), (byte)(this.colors[this.selectColor].g - 40), (byte)(this.colors[this.selectColor].b - 40), byte.MaxValue);
		UIDuration.StartDuration(this.duration);
	}

	private void DeactiveBlocks()
	{
		byte b = 0;
		while ((int)b < this.blocks.Length)
		{
			if (this.blocks[(int)b].cachedGameObject.name != this.selectColor.ToString())
			{
				this.blocks[(int)b].cachedGameObject.SetActive(false);
			}
			b += 1;
		}
		this.timers[nValue.int1] = TimerManager.In((float)nValue.int2, new TimerManager.Callback(this.SelectRandomColor));
	}
}
