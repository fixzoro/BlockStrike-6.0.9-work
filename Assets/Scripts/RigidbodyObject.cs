using System;
using Photon;
using UnityEngine;

public class RigidbodyObject : Photon.MonoBehaviour, IPunObservable
{
    public Rigidbody mRigidbody;

    private Transform mTransform;

    public SyncBuffer syncBuffer;

    private float timeSinceLastSync;

    private Vector3 lastSentVelocity;

    private Vector3 lastSentPosition;

    private bool forceSync;

    private PhotonPlayer LastContactPlayer;

    private void Awake()
	{
		base.photonView.AddPunObservable(this);
		this.mTransform = base.transform;
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonForce", new PhotonView.MessageDelegate(this.PhotonForce));
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	public void Force(Vector3 force)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(force);
		data.Write(PhotonNetwork.player);
		base.photonView.RPC("PhotonForce", PhotonTargets.MasterClient, data);
	}

	public void Force(Vector3 force, PhotonPlayer player)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(force);
		data.Write(player);
		base.photonView.RPC("PhotonForce", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void PhotonForce(PhotonMessage message)
	{
		Vector3 force = message.ReadVector3();
		PhotonPlayer lastContactPlayer = PhotonPlayer.Find(message.ReadInt());
		if (message.timestamp + 0.5 > PhotonNetwork.time)
		{
			this.LastContactPlayer = lastContactPlayer;
			this.mRigidbody.AddForce(force);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		this.forceSync = true;
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
        if (stream.isWriting)
        {
            if (!this.forceSync && this.mRigidbody.velocity == this.lastSentVelocity && this.mRigidbody.position == this.lastSentPosition)
            {
                return;
            }
            this.forceSync = false;
            stream.Write(this.timeSinceLastSync);
            stream.Write(this.mRigidbody.position);
            stream.Write(this.mRigidbody.rotation);
            stream.Write(this.mRigidbody.velocity);
            this.lastSentVelocity = this.mRigidbody.velocity;
            this.lastSentPosition = this.mRigidbody.position;
            this.timeSinceLastSync = 0f;
        }
        else
        {
            float interpolationTime = Mathf.Max(stream.ReadFloat(), 0.001f);
            Vector3 position = stream.ReadVector3();
            Quaternion rotation = stream.ReadQuaternion();
            Vector3 value = stream.ReadVector3();
            this.syncBuffer.AddKeyframe(interpolationTime, position, value, new Vector3(), rotation, default(Vector3), default(Vector3));
        }
    }

	private void FixedUpdate()
	{
		if (base.photonView.isMine)
		{
			this.timeSinceLastSync += Time.deltaTime;
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine && this.syncBuffer.HasKeyframes)
		{
			this.syncBuffer.UpdatePlayback(Time.deltaTime);
			this.mTransform.position = this.syncBuffer.Position;
			this.mTransform.rotation = this.syncBuffer.Rotation;
		}
	}

	public PhotonPlayer GetLastContactPlayer()
	{
		return this.LastContactPlayer;
	}
}
