using System;
using Photon;
using UnityEngine;

public class sTweenTime : Photon.MonoBehaviour
{
    private static float delay;

    public static float time
	{
		get
		{
			return sTweenTime.delay + Time.timeSinceLevelLoad;
		}
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonSendTime", new PhotonView.MessageDelegate(this.PhotonSendTime));
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(this.OnPhotonPlayerConnected));
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(1.5f, delegate()
			{
				PhotonDataWrite data = this.photonView.GetData();
				data.Write(sTweenTime.time);
				this.photonView.RPC("PhotonSendTime", playerConnect, data);
			});
		}
	}

	[PunRPC]
	private void PhotonSendTime(PhotonMessage message)
	{
		sTweenTime.delay = message.ReadFloat();
		sTweenTime.delay += (float)(PhotonNetwork.time - message.timestamp);
	}
}
