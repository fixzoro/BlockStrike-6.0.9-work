using System;
using System.Collections.Generic;
using DG.Tweening;
using Photon;
using UnityEngine;

public class BombManager : Photon.MonoBehaviour
{
    public GameObject Bomb;

    public Transform ZoneA;

    public Transform ZoneB;

    private int PlayerBomb = -1;

    private int PlayerDeactiveBomb = -1;

    private int Zone = -1;

    public static bool BombPlaced;

    public static bool BuyTime;

    private bool BombPlacing;

    public BombAudio BombAudio;

    public ParticleSystem Effect;

    public ControllerManager BombController;

    public Transform BombPlayerModel;

    public GameObject BombPlayerModel2;

    public GameObject LineBomb;

    private Vector3 BombPosition;

    private bool BuyZone;

    private UISprite DefuseBombIcon;

    private float IgnoreDropBombTime = -1f;

    private float DropBombTime = -1f;

    private static BombManager instance;

    public UISprite defuseBombIcon
	{
		get
		{
			if (this.DefuseBombIcon == null)
			{
				this.DefuseBombIcon = UIElements.Get<UISprite>("BombIcon");
			}
			return this.DefuseBombIcon;
		}
	}

	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb && PhotonNetwork.room.GetGameMode() != GameMode.Bomb2)
		{
			Resources.UnloadAsset(this.BombAudio.BombAudioClip);
			Resources.UnloadAsset(this.Effect.GetComponent<AudioSource>().clip);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			BombManager.instance = this;
			this.ZoneA.gameObject.SetActive(true);
			this.ZoneB.gameObject.SetActive(true);
			GameManager.defaultScore = false;
		}
	}

	private void Start()
	{
		PhotonRPC.AddMessage("SetStartRoundBomb", new PhotonRPC.MessageDelegate(this.SetStartRoundBomb));
		PhotonRPC.AddMessage("PhotonMasterPickupBomb", new PhotonRPC.MessageDelegate(this.PhotonMasterPickupBomb));
		PhotonRPC.AddMessage("PhotonPickupBomb", new PhotonRPC.MessageDelegate(this.PhotonPickupBomb));
		PhotonRPC.AddMessage("PhotonSetBomb", new PhotonRPC.MessageDelegate(this.PhotonSetBomb));
		PhotonRPC.AddMessage("PhotonDeactiveBomb", new PhotonRPC.MessageDelegate(this.PhotonDeactiveBomb));
		PhotonRPC.AddMessage("PhotonDeactiveBoom", new PhotonRPC.MessageDelegate(this.PhotonDeactiveBoom));
		PhotonRPC.AddMessage("PhotonDeactiveBombExit", new PhotonRPC.MessageDelegate(this.PhotonDeactiveBombExit));
		PhotonRPC.AddMessage("PhotonSetPosition", new PhotonRPC.MessageDelegate(this.PhotonSetPosition));
		PhotonRPC.AddMessage("PhotonOnPhotonPlayerConnected", new PhotonRPC.MessageDelegate(this.PhotonOnPhotonPlayerConnected));
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
		PhotonNetwork.onPhotonPlayerDisconnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerDisconnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerDisconnected));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Store" && this.BuyZone)
		{
			UIControllerList.BuyWeapons.cachedGameObject.SetActive(true);
		}
		if (name == "Bomb")
		{
			if (PhotonNetwork.player.GetTeam() == Team.Red && this.Zone != -nValue.int1 && this.PlayerBomb == PhotonNetwork.player.ID && !BombManager.BombPlaced)
			{
				if (PlayerInput.instance.PlayerWeapon.isFire)
				{
					return;
				}
				UIDuration.StartDuration((float)nValue.int5, true, new TweenCallback(this.SetBomb));
				PlayerInput.instance.SetMove(false);
				PlayerInput.instance.SetLook(false);
				this.BombPlacing = true;
			}
			else if (PhotonNetwork.player.GetTeam() == Team.Blue && this.Zone != -nValue.int1 && this.PlayerDeactiveBomb == PhotonNetwork.player.ID && BombManager.BombPlaced)
			{
				if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
				{
					UIDuration.StartDuration((float)nValue.int5, true, new TweenCallback(this.DeactiveBoom));
				}
				else
				{
					UIDuration.StartDuration((float)((!UIDefuseKit.defuseKit) ? nValue.int8 : nValue.int4), true, new TweenCallback(this.DeactiveBoom));
				}
				PlayerInput.instance.SetMove(false);
				PlayerInput.instance.SetLook(false);
				this.BombPlacing = true;
			}
		}
		else if (this.BombPlacing)
		{
			UIDuration.StopDuration();
			if (PlayerInput.instance != null)
			{
				PlayerInput.instance.SetMove(true);
				PlayerInput.instance.SetLook(true);
			}
			this.BombPlacing = false;
		}
		if (name == "Reload")
		{
			this.DropBombTime = 0f;
		}
	}

	private void GetButtonUp(string name)
	{
		if (name == "Bomb")
		{
			UIDuration.StopDuration();
			PlayerInput.instance.SetMove(true);
			PlayerInput.instance.SetLook(true);
			this.BombPlacing = false;
		}
		if (name == "Reload")
		{
			this.DropBombTime = -1f;
		}
	}

	private void Update()
	{
		if (this.DropBombTime >= 0f)
		{
			if (this.DropBombTime >= 0.5f)
			{
				BombManager.DropBomb();
				this.DropBombTime = -1f;
			}
			this.DropBombTime += Time.deltaTime;
		}
	}

	private void StartRound()
	{
		BombManager.BombPlaced = false;
		this.BombPlacing = false;
		this.Bomb.SetActive(false);
		this.PlayerBomb = -nValue.int1;
		this.PlayerDeactiveBomb = -nValue.int1;
		this.Zone = -nValue.int1;
		this.BombPosition = Vector3.zero;
		this.BombAudio.Stop();
		this.Effect.Stop();
		this.Effect.GetComponent<AudioSource>().Stop();
		if (GameManager.roundState != RoundState.WaitPlayer)
		{
			TimerManager.In((float)nValue.int1, delegate()
			{
				if (GameManager.roundState != RoundState.WaitPlayer && PhotonNetwork.isMasterClient)
				{
					List<PhotonPlayer> list = new List<PhotonPlayer>();
					for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
					{
						if (PhotonNetwork.playerList[i].GetTeam() == Team.Red)
						{
							list.Add(PhotonNetwork.playerList[i]);
						}
					}
					if (list.Count > nValue.int0)
					{
						PhotonPlayer photonPlayer;
						do
						{
							photonPlayer = list[UnityEngine.Random.Range(nValue.int0, list.Count)];
						}
						while (photonPlayer.GetDead());
						PhotonDataWrite data = PhotonRPC.GetData();
						data.Write(photonPlayer.ID);
						PhotonRPC.RPC("SetStartRoundBomb", PhotonTargets.All, data);
					}
				}
			});
		}
	}

	[PunRPC]
	private void SetStartRoundBomb(PhotonMessage message)
	{
		this.PlayerBomb = message.ReadInt();
		if (this.PlayerBomb == PhotonNetwork.player.ID)
		{
			UIWarningToast.Show(Localization.Get("You have a bomb", true));
			this.defuseBombIcon.cachedGameObject.SetActive(true);
			this.defuseBombIcon.spriteName = "BombIcon";
			if (this.BombPlayerModel == null)
			{
				this.BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(this.BombPlayerModel2)).transform;
			}
			this.BombPlayerModel.parent = null;
			this.BombPlayerModel.position = Vector3.up * 1000f;
		}
		else
		{
			if (PhotonNetwork.player.GetTeam() == Team.Blue && UIDefuseKit.defuseKit)
			{
				this.defuseBombIcon.cachedGameObject.SetActive(true);
				this.defuseBombIcon.spriteName = "Pilers";
			}
			else
			{
				this.defuseBombIcon.cachedGameObject.SetActive(false);
			}
			if (this.BombPlayerModel == null)
			{
				this.BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(this.BombPlayerModel2)).transform;
			}
			this.BombPlayerModel.parent = null;
			this.BombPlayerModel.position = Vector3.up * 1000f;
			if (PhotonNetwork.player.GetTeam() == Team.Red)
			{
				UIWarningToast.Show(PhotonPlayer.Find(this.PlayerBomb).UserId + " " + Localization.Get("picked up a bomb", true));
			}
			this.BombController = ControllerManager.FindController(this.PlayerBomb);
			if (this.BombController != null)
			{
				this.BombPlayerModel.SetParent(this.BombController.playerSkin.PlayerWeaponContainers[2]);
				this.BombPlayerModel.localPosition = new Vector3(0.1f, 0f, 0.1f);
				this.BombPlayerModel.localEulerAngles = new Vector3(-90f, 0f, 0f);
			}
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(nValue.float05, delegate()
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(BombManager.BombPlaced);
				if (BombManager.BombPlaced)
				{
					float value = UIScore2.timeData.endTime - Time.time;
					data.Write(this.BombPosition);
					data.Write(value);
					PhotonRPC.RPC("PhotonOnPhotonPlayerConnected", playerConnect, data);
				}
				else
				{
					PhotonRPC.RPC("PhotonOnPhotonPlayerConnected", playerConnect, data);
				}
			});
		}
	}

	[PunRPC]
	private void PhotonOnPhotonPlayerConnected(PhotonMessage message)
	{
		BombManager.BombPlaced = message.ReadBool();
		if (BombManager.BombPlaced)
		{
			this.BombPosition = message.ReadVector3();
			float time = message.ReadFloat() - (float)(PhotonNetwork.time - message.timestamp);
			UIScore2.StartTime(time, new Action(this.Boom));
			this.BombAudio.Play(time);
			BombManager.SetPosition(this.BombPosition);
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient && playerDisconnect.ID == this.PlayerBomb)
		{
			this.SetRandomPlayer();
		}
	}

	private void SetRandomPlayer()
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() == Team.Red && !PhotonNetwork.playerList[i].GetDead())
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		if (list.Count > nValue.int0)
		{
			PhotonPlayer photonPlayer = list[UnityEngine.Random.Range(nValue.int0, list.Count)];
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(photonPlayer.ID);
			PhotonRPC.RPC("SetStartRoundBomb", PhotonTargets.All, data);
		}
	}

	public static void DeadPlayer()
	{
		if (PhotonNetwork.player.ID == BombManager.instance.PlayerBomb)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out raycastHit, (float)nValue.int50))
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(raycastHit.point);
				data.Write(raycastHit.normal);
				data.Write(true);
				PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
			}
			else
			{
				BombManager.instance.SetRandomPlayer();
			}
		}
		BombManager.instance.defuseBombIcon.cachedGameObject.SetActive(false);
		if (PhotonNetwork.player.ID == BombManager.instance.PlayerDeactiveBomb)
		{
			PhotonRPC.RPC("PhotonDeactiveBombExit", PhotonTargets.All);
		}
		BombManager.instance.BombPlacing = false;
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIControllerList.Store.cachedGameObject.SetActive(false);
		UIControllerList.BuyWeapons.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
	}

	public static void DropBomb()
	{
		if (GameManager.roundState != RoundState.PlayRound)
		{
			return;
		}
		RaycastHit raycastHit;
		if (PhotonNetwork.player.ID == BombManager.instance.PlayerBomb && Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out raycastHit, (float)nValue.int50))
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(raycastHit.point);
			data.Write(raycastHit.normal);
			data.Write(false);
			PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
			BombManager.instance.IgnoreDropBombTime = Time.time + 2f;
			BombManager.instance.defuseBombIcon.cachedGameObject.SetActive(false);
		}
	}

	[PunRPC]
	private void PhotonSetPosition(PhotonMessage message)
	{
		Vector3 pos = message.ReadVector3();
		Vector3 normal = message.ReadVector3();
		bool flag = message.ReadBool();
		if (flag && this.PlayerBomb != -nValue.int1 && PhotonNetwork.player.GetTeam() == Team.Red)
		{
			UIWarningToast.Show(PhotonPlayer.Find(this.PlayerBomb).UserId + " " + Localization.Get("lost the bomb", true));
		}
		this.PlayerBomb = -nValue.int1;
		BombManager.SetPosition(pos, normal);
		if (this.BombPlayerModel == null)
		{
			this.BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(this.BombPlayerModel2)).transform;
		}
		this.BombPlayerModel.parent = null;
		this.BombPlayerModel.position = Vector3.up * 1000f;
	}

	public static void SetPosition(Vector3 pos, Vector3 normal)
	{
		BombManager.instance.Bomb.transform.position = pos + normal * nValue.float001;
		BombManager.instance.Bomb.transform.rotation = Quaternion.LookRotation(normal);
		BombManager.instance.Bomb.SetActive(true);
	}

	public static void SetPosition(Vector3 pos)
	{
		BombManager.instance.Bomb.transform.position = pos;
		BombManager.instance.Bomb.transform.rotation = Quaternion.Euler((float)(-(float)nValue.int90), (float)UnityEngine.Random.Range(nValue.int0, nValue.int360), (float)nValue.int0);
		BombManager.instance.Bomb.SetActive(true);
	}

	public void OnTriggerEnterBomb()
	{
		if (GameManager.roundState == RoundState.PlayRound)
		{
			if (this.IgnoreDropBombTime > Time.time)
			{
				return;
			}
			PhotonRPC.RPC("PhotonMasterPickupBomb", PhotonTargets.MasterClient);
		}
	}

	public void OnTriggerExitBomb()
	{
		if (BombManager.BombPlaced)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(false);
			if (PhotonNetwork.player.ID == this.PlayerDeactiveBomb)
			{
				PhotonRPC.RPC("PhotonDeactiveBombExit", PhotonTargets.All);
			}
		}
	}

	[PunRPC]
	private void PhotonMasterPickupBomb(PhotonMessage message)
	{
		PhotonPlayer sender = message.sender;
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(message.sender.ID);
		if (BombManager.BombPlaced)
		{
			if (sender.GetTeam() == Team.Blue && this.PlayerDeactiveBomb == -nValue.int1)
			{
				PhotonRPC.RPC("PhotonDeactiveBomb", PhotonTargets.All, data);
			}
		}
		else if (sender.GetTeam() == Team.Red && this.PlayerBomb == -nValue.int1)
		{
			PhotonRPC.RPC("PhotonPickupBomb", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonPickupBomb(PhotonMessage message)
	{
		this.Bomb.SetActive(false);
		this.PlayerBomb = message.ReadInt();
		if (this.PlayerBomb == PhotonNetwork.player.ID)
		{
			UIWarningToast.Show(Localization.Get("You have a bomb", true));
			this.defuseBombIcon.cachedGameObject.SetActive(true);
			this.defuseBombIcon.spriteName = "BombIcon";
		}
		else
		{
			if (this.BombPlayerModel == null)
			{
				this.BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(this.BombPlayerModel2)).transform;
			}
			this.BombPlayerModel.parent = null;
			this.BombPlayerModel.position = Vector3.up * 1000f;
			if (PhotonNetwork.player.GetTeam() == Team.Red)
			{
				UIWarningToast.Show(PhotonPlayer.Find(this.PlayerBomb).UserId + " " + Localization.Get("picked up a bomb", true));
			}
			this.BombController = ControllerManager.FindController(this.PlayerBomb);
			if (this.BombController != null)
			{
				this.BombPlayerModel.SetParent(this.BombController.playerSkin.PlayerWeaponContainers[2]);
				this.BombPlayerModel.localPosition = new Vector3(0.1f, 0f, 0.1f);
				this.BombPlayerModel.localEulerAngles = new Vector3(-90f, 0f, 0f);
			}
		}
	}

	[PunRPC]
	private void PhotonDeactiveBomb(PhotonMessage message)
	{
		this.PlayerDeactiveBomb = message.ReadInt();
		if (this.PlayerDeactiveBomb == PhotonNetwork.player.ID)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(true);
		}
	}

	[PunRPC]
	private void PhotonDeactiveBombExit(PhotonMessage message)
	{
		this.PlayerDeactiveBomb = -nValue.int1;
	}

	public void OnTriggerEnterZone(int zone)
	{
		this.Zone = zone;
		if (PhotonNetwork.player.GetTeam() == Team.Red && this.PlayerBomb == PhotonNetwork.player.ID && GameManager.roundState == RoundState.PlayRound)
		{
			UIControllerList.Bomb.cachedGameObject.SetActive(true);
		}
	}

	public void OnTriggerExitZone()
	{
		this.Zone = -nValue.int1;
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		this.BombPlacing = false;
	}

	private void SetBomb()
	{
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		if (GameManager.roundState == RoundState.PlayRound)
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(PlayerInput.instance.PlayerTransform.position - new Vector3((float)nValue.int0, nValue.float008, (float)nValue.int0));
			PhotonRPC.RPC("PhotonSetBomb", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonSetBomb(PhotonMessage message)
	{
		if (GameManager.roundState != RoundState.PlayRound)
		{
			return;
		}
		UIWarningToast.Show(Localization.Get("The bomb has been planted", true));
		this.BombPlacing = false;
		BombManager.BombPlaced = true;
		if (this.BombPlayerModel == null)
		{
			this.BombPlayerModel = ((GameObject)UnityEngine.Object.Instantiate(this.BombPlayerModel2)).transform;
		}
		this.BombPlayerModel.parent = null;
		this.BombPlayerModel.position = Vector3.up * 1000f;
		this.BombController = null;
		this.PlayerBomb = -nValue.int1;
		float time = 35f - (float)(PhotonNetwork.time - message.timestamp);
		UIScore2.StartTime(time, new Action(this.Boom));
		BombManager.SetPosition(message.ReadVector3());
		this.BombAudio.Play(time);
		this.defuseBombIcon.cachedGameObject.SetActive(false);
	}

	private void Boom()
	{
		this.BombAudio.Boom();
		this.Effect.Play();
		this.Effect.GetComponent<AudioSource>().Play();
		this.Effect.transform.position = this.Bomb.transform.position;
		this.BombAudio.Stop();
		this.Bomb.SetActive(false);
		this.BombPlacing = false;
		BombManager.BombPlaced = false;
		this.PlayerBomb = -nValue.int1;
		this.PlayerDeactiveBomb = -nValue.int1;
		UIScore2.timeData.active = false;
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
		{
			BombMode.instance.Boom();
		}
		else
		{
			BombMode2.instance.Boom();
		}
		UIDuration.StopDuration();
		if (GameManager.player != null && !GameManager.player.Dead)
		{
			GameManager.player.SetMove(true);
			int num = (nValue.int50 - (int)Vector3.Distance(PlayerInput.instance.PlayerTransform.position, this.Bomb.transform.position)) * nValue.int5;
			num = Mathf.Clamp(num, nValue.int0, nValue.int100);
			if (num > nValue.int0)
			{
				DamageInfo damageInfo = DamageInfo.Get(num, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
				PlayerInput.instance.Damage(damageInfo);
			}
			PlayerInput.instance.FPCamera.AddRollForce(UnityEngine.Random.Range((float)(-(float)nValue.int3), (float)nValue.int3));
		}
	}

	private void DeactiveBoom()
	{
		UIControllerList.Bomb.cachedGameObject.SetActive(false);
		UIDuration.StopDuration();
		PlayerInput.instance.SetMove(true);
		PlayerInput.instance.SetLook(true);
		PhotonRPC.RPC("PhotonDeactiveBoom", PhotonTargets.All);
	}

	[PunRPC]
	private void PhotonDeactiveBoom(PhotonMessage message)
	{
		this.BombAudio.Stop();
		this.Bomb.SetActive(false);
		this.Bomb.SetActive(false);
		this.BombPlacing = false;
		BombManager.BombPlaced = false;
		this.PlayerBomb = -nValue.int1;
		this.PlayerDeactiveBomb = -nValue.int1;
		UIScore2.timeData.active = false;
		if (PhotonNetwork.room.GetGameMode() == GameMode.Bomb)
		{
			BombMode.instance.DeactiveBoom();
		}
		else
		{
			BombMode2.instance.DeactiveBoom();
		}
	}

	public void OnTriggerEnterZoneBuy(int team)
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb2)
		{
			return;
		}
		if (PhotonNetwork.player.GetTeam() == (Team)team && GameManager.roundState == RoundState.PlayRound && (BombManager.BuyTime || UIScore2.timeData.endTime - Time.time > 90f))
		{
			UIControllerList.Store.cachedGameObject.SetActive(true);
			this.BuyZone = true;
		}
	}

	public void OnTriggerExitZoneBuy()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.Bomb2)
		{
			return;
		}
		UIControllerList.Store.cachedGameObject.SetActive(false);
		UIControllerList.BuyWeapons.cachedGameObject.SetActive(false);
		this.BuyZone = false;
	}

	public static int GetPlayerBombID()
	{
		return BombManager.instance.PlayerBomb;
	}
}
