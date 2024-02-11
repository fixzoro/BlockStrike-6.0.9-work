using System;
using UnityEngine;

public class UIOthers : MonoBehaviour
{
    public UICamera cam;

    public UIGrid buttonGrid;

    public GameObject weaponButton;

    public static event Action pauseEvent;

	private void Start()
	{
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
		UICamera.clickUIInput = false;
		this.cam.useMouse = false;
		this.cam.useKeyboard = false;
		this.cam.useController = false;
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		PhotonNetwork.onLeftRoom = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onLeftRoom, new PhotonNetwork.VoidDelegate(this.OnLeftRoom));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		PhotonNetwork.onLeftRoom = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onLeftRoom, new PhotonNetwork.VoidDelegate(this.OnLeftRoom));
	}

	private void GetButtonDown(string name)
	{
		if (name != "Pause")
		{
			return;
		}
		UIPanelManager.ShowPanel("Pause");
		if (UIOthers.pauseEvent != null)
		{
			UIOthers.pauseEvent();
		}
		if (!GameManager.changeWeapons)
		{
			this.weaponButton.SetActive(false);
		}
		this.buttonGrid.Reposition();
	}

	private void OnSettings()
	{
		UIElements.Get<UIPanel>("DisplayPanel").alpha = ((!Settings.HUD) ? 0f : 1f);
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom(true);
	}

	private void OnLeftRoom()
	{
		PlayerRoundManager.Show();
		LevelManager.customScene = false;
		PhotonNetwork.leavingRoom = false;
		LevelManager.LoadLevel("Menu");
	}
}
