using System;
using Photon;
using UnityEngine;

public class GDPR : Photon.MonoBehaviour
{
    public GameObject panel;

    public GameObject panel2;

    private void Start()
	{
		if (PlayerPrefs.GetInt("GDPR", 0) == 0)
		{
			this.ShowGDPR();
		}
		else
		{
			LevelManager.LoadLevel("Menu");
		}
	}

	private void OnEnable()
	{
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(this.OnJoinedRoom));
	}

	private void OnDisable()
	{
		PhotonNetwork.onJoinedRoom = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onJoinedRoom, new PhotonNetwork.VoidDelegate(this.OnJoinedRoom));
	}

	private void ShowGDPR()
	{
		this.panel.SetActive(true);
		UISettings.UpdateLanguage();
	}

	public void OnAccept()
	{
		this.panel.SetActive(false);
		this.panel2.SetActive(true);
		PlayerPrefs.SetInt("GDPR", 1);
	}

	public void OnPrivacyPolicy()
	{
		Application.OpenURL("http://rexetstudio.com/en/privacypolicy");
	}

	public void OnClickTraining(bool isTraining)
	{
		this.panel2.SetActive(false);
		if (isTraining)
		{
			this.LoadTutorial();
		}
		else
		{
			LevelManager.LoadLevel("Menu");
		}
	}

	private void LoadTutorial()
	{
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.CreateRoom("tutorial");
	}

	private void OnJoinedRoom()
	{
		LevelManager.LoadLevel("MainTutorial");
	}
}
