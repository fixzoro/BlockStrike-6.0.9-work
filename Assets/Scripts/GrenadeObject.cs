using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class GrenadeObject : Photon.MonoBehaviour, IPunObservable
{
    public static List<GrenadeObject> list = new List<GrenadeObject>();

    public float time = 3f;

    public float speed = 5f;

    public ParticleSystem effect;

    public Transform effectTransform;

    public GameObject cachedGameObject;

    public Transform cachedTransform;

    public Rigidbody cachedRigidbody;

    public GameObject cachedGameObjectModel;

    private Vector3 PhotonPosition = Vector3.zero;

    private Quaternion PhotonRotation = Quaternion.identity;

    private bool isMine;

    private bool isActive;

    private void Awake()
	{
		base.photonView.AddPunObservable(this);
		base.photonView.AddMessage("SetTime", new PhotonView.MessageDelegate(this.SetTime));
	}

	private void OnEnable()
	{
		GrenadeObject.list.Add(this);
		this.isActive = true;
		this.isMine = base.photonView.isMine;
		this.PhotonRotation = Quaternion.identity;
		this.PhotonPosition = Vector3.zero;
		this.cachedGameObjectModel.SetActive(true);
		this.cachedRigidbody.isKinematic = !base.photonView.isMine;
		if (!this.cachedRigidbody.isKinematic)
		{
			this.cachedRigidbody.velocity = Vector3.zero;
		}
		this.cachedRigidbody.useGravity = base.photonView.isMine;
		if (this.isMine)
		{
			base.photonView.RPC("SetTime", PhotonTargets.All);
			this.PhotonPosition = this.cachedRigidbody.position;
			this.PhotonRotation = this.cachedRigidbody.rotation;
		}
	}

	private void OnDisable()
	{
		GrenadeObject.list.Remove(this);
		this.isActive = false;
	}

	[PunRPC]
	private void SetTime(PhotonMessage message)
	{
		if (this.isActive)
		{
			float num = this.time - (float)(PhotonNetwork.time - message.timestamp);
			if (num > 0f)
			{
				TimerManager.In(this.time - (float)(PhotonNetwork.time - message.timestamp), new TimerManager.Callback(this.Bomb));
			}
			else
			{
				this.Clear();
			}
		}
		else
		{
			this.Clear();
		}
	}

	private void Bomb()
	{
		int num = (int)Vector3.Distance(PlayerInput.instance.PlayerTransform.position, this.cachedTransform.position);
		int num2 = (nValue.int12 - num) * nValue.int6;
		num2 = Mathf.Clamp(num2, nValue.int0, nValue.int80);
		if (num2 > nValue.int0 && PhotonNetwork.player.GetTeam() != base.photonView.owner.GetTeam())
		{
			DamageInfo damageInfo = DamageInfo.Get(num2, Vector3.zero, base.photonView.owner.GetTeam(), 46, nValue.int0, base.photonView.owner.ID, false);
			PlayerInput.instance.Damage(damageInfo);
			PlayerInput.instance.FPCamera.AddRollForce(UnityEngine.Random.Range((float)(-(float)num2) * 0.03f, (float)num2 * 0.03f));
		}
		for (int i = 0; i < GrenadeObject.list.Count; i++)
		{
			if (GrenadeObject.list[i] != this && GrenadeObject.list[i].photonView.isMine)
			{
				GrenadeObject.list[i].cachedRigidbody.AddExplosionForce(150f, this.cachedTransform.position, 15f);
			}
		}
		this.cachedGameObjectModel.SetActive(false);
		this.effectTransform.eulerAngles = new Vector3(-90f, 0f, 0f);
		this.effect.Play(true);
		SoundClip soundClip = SoundManager.Play3D("Explosion", this.cachedTransform.position);
		if (soundClip != null)
		{
			soundClip.GetSource().rolloffMode = AudioRolloffMode.Linear;
		}
		TimerManager.In(0.2f, new TimerManager.Callback(this.Clear));
	}

	private void Clear()
	{
		if (this.isActive && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(this.cachedGameObject);
		}
	}

	private void Update()
	{
		if (!this.isMine)
		{
			this.cachedRigidbody.MovePosition(Vector3.Lerp(this.cachedRigidbody.position, this.PhotonPosition, Time.deltaTime * this.speed));
			this.cachedRigidbody.MoveRotation(Quaternion.Lerp(this.cachedRigidbody.rotation, this.PhotonRotation, Time.deltaTime * this.speed));
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
        if (stream.isWriting)
        {
            stream.Write(this.cachedRigidbody.position);
            stream.Write(this.cachedRigidbody.rotation);
        }
        else
        {
            this.PhotonPosition = stream.ReadVector3();
            this.PhotonRotation = stream.ReadQuaternion();
        }
    }
}
