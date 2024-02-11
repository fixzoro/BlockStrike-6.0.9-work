using System;
using UnityEngine;

public class UINameManager : MonoBehaviour
{
    private static bool showPlayerName = true;

    public Camera m_Camera;

    private UILabel label;

    private string hitName;

    private Ray ray;

    private Vector3 cameraPoint = new Vector3(0.5f, 0.5f, 0f);

    private static Color32 blueColor = new Color32(0, 181, byte.MaxValue, byte.MaxValue);

    private static Color32 redColor = new Color32(byte.MaxValue, 0, 80, byte.MaxValue);

    private void Start()
	{
		this.label = UIElements.Get<UILabel>("NameLabel");
		UINameManager.showPlayerName = GameConsole.Load<bool>("show_player_name", true);
        label.gameObject.SetActive(true);
	}

	private void OnEnable()
	{
		TimerManager.In("NameTimer", 0.2f, -1, 0.2f, new TimerManager.Callback(this.UpdateName));
	}

	private void OnDisable()
	{
		TimerManager.Cancel("NameTimer");
		if (this.label != null)
		{
			this.label.text = string.Empty;
		}
	}

	private void UpdateName()
	{
		if (!UINameManager.showPlayerName)
		{
			return;
		}
		this.ray = this.m_Camera.ViewportPointToRay(this.cameraPoint);
		if (nRaycast.RaycastName(this.ray, 100f))
		{
			this.hitName = nRaycast.container.playerSkin.Controller.photonView.owner.UserId;
			if (nRaycast.container.playerSkin.PlayerTeam == Team.Blue)
			{
				this.label.color = UINameManager.blueColor;
			}
			else
			{
				this.label.color = UINameManager.redColor;
			}
			this.label.text = this.hitName;
		}
		else
		{
			this.label.text = string.Empty;
		}
	}
}
