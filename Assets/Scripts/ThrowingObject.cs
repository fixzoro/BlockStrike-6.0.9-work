using System;
using Photon;
using UnityEngine;

public class ThrowingObject : Photon.MonoBehaviour, IPunObservable
{
    public float time = 3f;

    public float speed = 5f;

    public GameObject cachedGameObject;

    public Transform cachedTransform;

    public Rigidbody cachedRigidbody;

    public GameObject cachedGameObjectModel;

    private Vector3 PhotonPosition = Vector3.zero;

    private Quaternion PhotonRotation = Quaternion.identity;

    private bool isMine;

    private bool isActive;

    private void Start()
	{
		this.isMine = base.photonView.isMine;
		this.cachedRigidbody.isKinematic = !base.photonView.isMine;
		this.cachedRigidbody.useGravity = base.photonView.isMine;
		if (this.isMine)
		{
			base.photonView.RPC("SetTime", PhotonTargets.All);
			this.PhotonPosition = this.cachedRigidbody.position;
			this.PhotonRotation = this.cachedRigidbody.rotation;
		}
	}

	private void OnEnable()
	{
		this.isActive = true;
	}

	private void OnDisable()
	{
		this.isActive = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.name, other.gameObject);
		if (this.isMine && !other.CompareTag("Player"))
		{
			this.cachedRigidbody.isKinematic = true;
			this.cachedRigidbody.useGravity = false;
		}
	}

	[PunRPC]
	private void SetTime(PhotonMessageInfo info)
	{
		if (this.isActive)
		{
			float num = this.time - (float)(PhotonNetwork.time - info.timestamp);
			if (num > 0f)
			{
				TimerManager.In(this.time - (float)(PhotonNetwork.time - info.timestamp), new TimerManager.Callback(this.Clear));
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
