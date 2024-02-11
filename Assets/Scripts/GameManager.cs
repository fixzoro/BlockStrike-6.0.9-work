using System;
using System.Collections.Generic;
using BSCM.Game;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
    public RoundState state;

    public static bool changeWeapons = true;

    public static bool globalChat = true;

    public static CryptoInt maxScore = 20;

    public static CryptoInt blueScore = 0;

    public static CryptoInt redScore = 0;

    public static ControllerManager controller;

    public static bool friendDamage = false;

    public static CryptoBool startDamage = true;

    public static CryptoFloat startDamageTime = 4f;

    public static bool defaultScore = true;

    public static bool loadingLevel = false;

    public static string leaveRoomMessage;

    private static GameManager instance;

    public static PlayerInput player
	{
		get
		{
			return GameManager.controller.playerInput;
		}
	}

    #if UNITY_EDITOR
    public static void SetPassword(string text)
    {
        PhotonNetwork.room.SetPassword(text);
    }
    #endif

    public static Team team
	{
		get
		{
			return GameManager.player.PlayerTeam;
		}
		set
		{
			GameManager.controller.SetTeam(value);
		}
	}

	public static Team winTeam
	{
		get
		{
			if (GameManager.blueScore >= GameManager.maxScore)
			{
				return Team.Blue;
			}
			if (GameManager.redScore >= GameManager.maxScore)
			{
				return Team.Red;
			}
			return Team.None;
		}
	}

	public static bool checkScore
	{
		get
		{
			return !LevelManager.customScene && (GameManager.blueScore >= GameManager.maxScore || GameManager.redScore >= GameManager.maxScore);
		}
	}

	public static RoundState roundState
	{
		get
		{
			return GameManager.instance.state;
		}
		set
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.SetRoundState(value);
				GameManager.instance.state = value;
			}
		}
	}

	public static GameManager main
	{
		get
		{
			return GameManager.instance;
		}
	}

	private void Awake()
	{
		GameManager.instance = this;
		if (!PhotonNetwork.offlineMode && !PhotonNetwork.inRoom)
		{
			LevelManager.LoadLevel("Menu");
		}
        if(this.gameObject.GetComponent<PhotonView>() == null)
        {
            this.gameObject.AddComponent<PhotonView>();
            while (this.gameObject.GetComponent<PhotonView>() == null)
            {
                this.gameObject.GetComponent<PhotonView>().punObservables[0] = (IPunObservable)this;
                this.gameObject.GetComponent<PhotonView>().synchronization = ViewSynchronization.Unreliable;
            }
        }
    }

	private void Start()
	{
		GameManager.controller = PhotonNetwork.Instantiate("Player/ControllerManager", Vector3.zero, Quaternion.identity, 0).GetComponent<ControllerManager>();
		PlayerInput.instance = GameManager.controller.playerInput;
		PlayerRoundManager.SetMode(PhotonNetwork.room.GetGameMode());
		this.state = PhotonNetwork.room.GetRoundState();
		PhotonRPC.AddMessage("PhotonUpdateScore", new PhotonRPC.MessageDelegate(this.PhotonUpdateScore));
		PhotonRPC.AddMessage("PhotonLoadNextLevel", new PhotonRPC.MessageDelegate(this.PhotonLoadNextLevel));
		PhotonRPC.AddMessage("UpdateRoundInfo", new PhotonRPC.MessageDelegate(RoundPlayersData.UpdateRoundInfo));
		DropWeaponManager.ClearScene();
		if (PhotonNetwork.room.isCustomMap())
		{
			SceneSettings.instance.Create();
		}
        #if UNITY_EDITOR
        if (PlayerInput.instance != null)
        {
            PlayerInput.instance.cursor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        #endif
    }

    private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
		PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Combine(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(this.OnPhotonCustomRoomPropertiesChanged));
	}

	private void OnDisable()
	{
		GameManager.changeWeapons = true;
		GameManager.globalChat = true;
		GameManager.friendDamage = false;
		GameManager.startDamage = true;
		GameManager.startDamageTime = 4f;
		GameManager.blueScore = nValue.int0;
		GameManager.redScore = nValue.int0;
		GameManager.maxScore = nValue.int20;
		GameManager.loadingLevel = false;
		DOTween.Clear(false);
		PhotonRPC.Clear();
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
		PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Remove(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(this.OnPhotonCustomRoomPropertiesChanged));
		GameManager.defaultScore = true;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		UIStatus.ConnectPlayer(player);
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		UIStatus.DisconnectPlayer(player);
	}

	private void OnPhotonCustomRoomPropertiesChanged(Hashtable hash)
	{
		if (hash.ContainsKey(PhotonCustomValue.roundStateKey))
		{
			this.state = (RoundState)((byte)hash[PhotonCustomValue.roundStateKey]);
		}
	}

	public static void SetScore(PhotonPlayer player)
	{
		PhotonDataWrite data = GameManager.instance.photonView.GetData();
		data.Write((byte)GameManager.maxScore);
		data.Write((byte)GameManager.blueScore);
		data.Write((byte)GameManager.redScore);
		PhotonRPC.RPC("PhotonUpdateScore", player, data);
	}

	public static void SetScore()
	{
		PhotonDataWrite data = GameManager.instance.photonView.GetData();
		data.Write((byte)GameManager.maxScore);
		data.Write((byte)GameManager.blueScore);
		data.Write((byte)GameManager.redScore);
		PhotonRPC.RPC("PhotonUpdateScore", PhotonTargets.All, data);
	}

	[PunRPC]
	private void PhotonUpdateScore(PhotonMessage message)
	{
		byte value = message.ReadByte();
		byte value2 = message.ReadByte();
		byte value3 = message.ReadByte();
		GameManager.maxScore = (int)value;
		if (GameManager.maxScore > 0)
		{
			GameManager.blueScore = Mathf.Clamp((int)value2, nValue.int0, GameManager.maxScore);
			GameManager.redScore = Mathf.Clamp((int)value3, nValue.int0, GameManager.maxScore);
		}
		else
		{
			GameManager.blueScore = (int)value2;
			GameManager.redScore = (int)value3;
		}
		if (GameManager.defaultScore)
		{
			UIScore.UpdateScore(GameManager.maxScore, GameManager.blueScore, GameManager.redScore);
		}
		else
		{
			UIScore2.UpdateScore(GameManager.maxScore, GameManager.blueScore, GameManager.redScore);
		}
		EventManager.Dispatch("UpdateScore");
	}

	public static void LoadNextLevel()
	{
		GameManager.LoadNextLevel(PhotonNetwork.room.GetGameMode());
	}

	public static void LoadNextLevel(GameMode mode)
	{
		if (!GameManager.loadingLevel)
		{
			GameManager.loadingLevel = true;
			if (!LevelManager.customScene)
			{
				UIStatus.Add(Localization.Get("Next map", true) + ": " + LevelManager.GetNextScene(mode), true);
			}
			TimerManager.In((float)nValue.int4, delegate()
			{
				if (LevelManager.customScene)
				{
					PhotonNetwork.LeaveRoom(true);
				}
				else if (PhotonNetwork.isMasterClient)
				{
					PhotonDataWrite data = GameManager.instance.photonView.GetData();
					data.Write((byte)mode);
					PhotonRPC.RPC("PhotonLoadNextLevel", PhotonTargets.All, data);
				}
			});
			TimerManager.In(nValue.float15, delegate()
			{
				PhotonNetwork.player.ClearProperties();
			});
		}
	}

	[PunRPC]
	private void PhotonLoadNextLevel(PhotonMessage message)
	{
		byte mode = message.ReadByte();
		PhotonNetwork.RemoveRPCs(PhotonNetwork.player);
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
		PhotonNetwork.LoadLevel(LevelManager.GetNextScene((GameMode)mode));
	}

	public static void StartAutoBalance()
	{
		TimerManager.In((float)nValue.int30, -nValue.int1, (float)nValue.int30, new TimerManager.Callback(GameManager.BalanceTeam));
		UIChangeTeam.SetChangeTeam(true);
	}

	public static void BalanceTeam()
	{
		GameManager.BalanceTeam(false);
	}

	public static void BalanceTeam(bool updateTeam)
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red)
			{
				list2.Add(playerList[j]);
			}
		}
		if (list.Count > list2.Count + nValue.int1 && PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			list.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
			if (list[list.Count - nValue.int1].IsLocal)
			{
				if (updateTeam)
				{
					GameManager.team = Team.Red;
				}
				EventManager.Dispatch<Team>("AutoBalance", Team.Red);
				UIToast.Show(Localization.Get("Autobalance: You moved to another team", true));
			}
		}
		if (list2.Count > list.Count + nValue.int1 && PhotonNetwork.player.GetTeam() == Team.Red)
		{
			list2.Sort(new Comparison<PhotonPlayer>(UIPlayerStatistics.SortByKills));
			if (list2[list2.Count - nValue.int1].IsLocal)
			{
				if (updateTeam)
				{
					GameManager.team = Team.Blue;
				}
				EventManager.Dispatch<Team>("AutoBalance", Team.Blue);
				UIToast.Show(Localization.Get("Autobalance: You moved to another team", true));
			}
		}
	}
}
