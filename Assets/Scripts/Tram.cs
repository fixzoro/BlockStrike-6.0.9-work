using System;
using DG.Tweening;
using UnityEngine;

public class Tram : MonoBehaviour, IPunObservable
{
    public GameObject model;

    public GameObject path;

    public Vector3[] pos;

    public byte[] controlIndex;

    public byte index;

    private byte lastIndex;

    public float speed;

    public float stepSpeed;

    public int players = 1;

    public bool back;

    public float distance;

    public float lerpSpeed = 10f;

    private Transform mTransform;

    private ControllerManager player;

    private Vector3 photonPosition;

    private Quaternion photonRotation;

    public static Action finishCallback;

    private float noPlayersTime;

    private static Tram instance;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode || PhotonNetwork.room.GetGameMode() != GameMode.Escort)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Tram.instance = this;
		GameManager.main.photonView.AddPunObservable(this);
		GameManager.main.photonView.synchronization = ViewSynchronization.Unreliable;
		this.mTransform = this.model.transform;
		this.photonPosition = this.mTransform.position;
		this.photonRotation = this.mTransform.rotation;
		this.model.SetActive(true);
		this.path.SetActive(true);
	}

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == GameMode.Escort)
		{
			TimerManager.In(0.5f, -1, 0.1f, new TimerManager.Callback(this.CheckDistance));
		}
	}

	private void Update()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (this.back)
			{
				if (this.mTransform.position == this.pos[(int)(this.index - 1)])
				{
					if (this.lastIndex != this.index - 1)
					{
						this.index -= 1;
						this.mTransform.position = Vector3.MoveTowards(this.mTransform.position, this.pos[(int)(this.index - 1)], Time.deltaTime * (this.speed * (float)this.players));
						this.mTransform.DOLookAt(this.pos[(int)this.index], 0.5f, AxisConstraint.None, null);
					}
				}
				else
				{
					this.mTransform.position = Vector3.MoveTowards(this.mTransform.position, this.pos[(int)(this.index - 1)], Time.deltaTime * (this.speed * (float)this.players));
				}
			}
			else if (this.mTransform.position == this.pos[(int)this.index])
			{
				if (this.pos.Length - 1 <= (int)this.index)
				{
					if (Tram.finishCallback != null)
					{
						Tram.finishCallback();
						Tram.finishCallback = null;
					}
				}
				else
				{
					this.index += 1;
					for (int i = 0; i < this.controlIndex.Length; i++)
					{
						if (this.controlIndex[i] == this.index)
						{
							this.lastIndex = this.index;
							break;
						}
					}
					this.mTransform.DOLookAt(this.pos[(int)this.index], 0.5f, AxisConstraint.None, null);
				}
			}
			else
			{
				this.mTransform.position = Vector3.MoveTowards(this.mTransform.position, this.pos[(int)this.index], Time.deltaTime * (this.speed * (float)this.players));
			}
		}
		else
		{
			this.mTransform.position = Vector3.Lerp(this.mTransform.position, this.photonPosition, Time.deltaTime * this.lerpSpeed);
			this.mTransform.rotation = Quaternion.Lerp(this.mTransform.rotation, this.photonRotation, Time.deltaTime * this.lerpSpeed);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
		if (stream.isWriting)
		{
			stream.Write(this.mTransform.position);
			stream.Write(this.mTransform.rotation);
			stream.Write(this.index);
			stream.Write(this.lastIndex);
		}
		else
		{
			this.photonPosition = stream.ReadVector3();
			this.photonRotation = stream.ReadQuaternion();
			this.index = stream.ReadByte();
			this.lastIndex = stream.ReadByte();
		}
	}

	private void CheckDistance()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (this.GetPlayers() && GameManager.roundState == RoundState.PlayRound)
			{
				byte b = 0;
				byte b2 = 0;
				if (!GameManager.player.Dead && this.isZone(GameManager.player.PlayerTransform))
				{
					if (GameManager.team == Team.Blue)
					{
						b2 += 1;
					}
					else if (GameManager.team == Team.Red)
					{
						b += 1;
					}
				}
				else
				{
					this.speed = 0f;
				}
				for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
				{
					this.player = ControllerManager.ControllerList[i];
					if (this.player != null && !this.player.playerSkin.Dead && this.isZone(this.player.playerSkin.PlayerTransform))
					{
						if (this.player.playerSkin.PlayerTeam == Team.Blue)
						{
							b2 += 1;
						}
						else if (this.player.playerSkin.PlayerTeam == Team.Red)
						{
							b += 1;
						}
					}
				}
				if (b != 0 && b2 != 0)
				{
					this.speed = 0f;
					this.noPlayersTime = 0f;
					return;
				}
				if (b == 0 && b2 == 0)
				{
					if (this.noPlayersTime == 0f)
					{
						this.noPlayersTime = Time.time;
					}
					else if (this.noPlayersTime + 10f < Time.time)
					{
						this.back = true;
						this.speed = 0.2f;
					}
					return;
				}
				if (b != 0)
				{
					this.speed = Mathf.Clamp((float)b * this.stepSpeed, 0f, 1.2f);
					this.back = false;
					this.noPlayersTime = 0f;
				}
				else if (b2 != 0)
				{
					this.speed = 0.2f;
					this.back = true;
					this.noPlayersTime = 0f;
				}
			}
			else
			{
				this.speed = 0f;
			}
		}
	}

	private bool isZone(Transform target)
	{
		return !(target == null) && Mathf.Abs(Vector3.Distance(this.mTransform.position, target.position)) <= this.distance;
	}

	public static Transform GetModel()
	{
		return Tram.instance.mTransform;
	}

	private bool GetPlayers()
	{
		byte b = 0;
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].GetTeam() != Team.None)
			{
				b += 1;
				if (b > 1)
				{
					return true;
				}
			}
		}
		return false;
	}
}
