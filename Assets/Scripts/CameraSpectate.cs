using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpectate : MonoBehaviour
{
    public CameraManager cameraManager;

    public float distance;

    public float distanceMin = 0.5f;

    public float distanceMax = 3f;

    public float speedRotate = 2.5f;

    private int index;

    private Transform target;

    private Vector2 rotate;

    private Vector3 negDistance = Vector3.zero;

    private Ray ray = default(Ray);

    private RaycastHit raycastHit;

    private Transform cameraTransform;

    private void Start()
	{
		this.cameraTransform = this.cameraManager.cameraTransform;
	}

	public void Active()
	{
		this.Active(-1);
	}

	public void Active(object[] parameters)
	{
		if (parameters != null && parameters.Length > 0)
		{
			this.Active((int)parameters[0]);
		}
		else
		{
			this.Active(-1);
		}
	}

	public void Active(int playerID)
	{
		this.cameraTransform.gameObject.SetActive(true);
		this.UpdateSelectPlayer(playerID);
		LODObject.Target = this.cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
	}

	public void Deactive()
	{
		if (CameraManager.type != CameraType.Spectate)
		{
			return;
		}
		if (this.cameraTransform != null)
		{
			this.cameraTransform.gameObject.SetActive(false);
		}
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
	}

	private void GetButtonDown(string name)
	{
		if (CameraManager.type != CameraType.Spectate)
		{
			return;
		}
		if (name == "Fire")
		{
			this.UpdateSelectPlayer();
		}
	}

	private void GetAxis(string name, float value)
	{
        if (CameraManager.type == CameraType.Spectate)
        {
            switch (name)
            {
                case "Mouse X":
                    {
                        Vector2 reference2 = rotate;
                        reference2.x += value * speedRotate * distance;
                        rotate = reference2;
                        break;
                    }
                case "Mouse Y":
                    {
                        Vector2 reference = rotate;
                        reference.y -= value * speedRotate;
                        rotate = reference;
                        break;
                    }
            }
        }
    }

	public void OnUpdate()
	{
		if (CameraManager.type != CameraType.Spectate)
		{
			return;
		}
		if (this.target == null)
		{
			return;
		}
		this.distance = this.distanceMax;
		this.rotate.y = this.ClampAngle(this.rotate.y, -20f, 80f);
		Quaternion rotation = Quaternion.Euler(this.rotate.y, this.rotate.x, 0f);
		this.ray.origin = this.target.position;
		this.ray.direction = (this.cameraTransform.position - this.target.position).normalized;
		if (Physics.SphereCast(this.ray.origin, 0.25f, this.ray.direction, out this.raycastHit, this.distance))
		{
			this.distance = Mathf.Clamp(this.raycastHit.distance, this.distanceMin, this.distanceMax);
		}
		this.negDistance.z = -this.distance;
		Vector3 position = rotation * this.negDistance + this.target.position;
		this.cameraTransform.position = position;
		this.cameraTransform.rotation = rotation;
		SkyboxManager.GetCamera().rotation = rotation;
	}

	public void UpdateSelectPlayer()
	{
		this.UpdateSelectPlayer(-1);
	}

	public void UpdateSelectPlayer(int playerID)
	{
		if (CameraManager.type != CameraType.Spectate)
		{
			return;
		}
		if (playerID != -1)
		{
			int i = 0;
			while (i < ControllerManager.ControllerList.Count)
			{
				ControllerManager controllerManager = ControllerManager.ControllerList[i];
				if (controllerManager.photonView.ownerId == playerID)
				{
					if (controllerManager.playerSkin != null && controllerManager.playerSkin.isPlayerActive)
					{
						this.target = controllerManager.playerSkin.PlayerSpectatePoint;
						this.cameraManager.OnSelectPlayer(playerID);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		List<PlayerSkin> list = new List<PlayerSkin>();
		for (int j = 0; j < ControllerManager.ControllerList.Count; j++)
		{
			ControllerManager controllerManager = ControllerManager.ControllerList[j];
			if (controllerManager.playerSkin != null && controllerManager.playerSkin.isPlayerActive)
			{
				list.Add(controllerManager.playerSkin);
			}
		}
		if (CameraManager.Team)
		{
			Team team = PhotonNetwork.player.GetTeam();
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].PlayerTeam != team)
				{
					list.RemoveAt(k);
					k--;
				}
			}
			this.index++;
			if (this.index > list.Count - 1)
			{
				this.index = 0;
			}
			if (list.Count != 0)
			{
				this.target = list[this.index].PlayerSpectatePoint;
				this.cameraManager.OnSelectPlayer(list[this.index].Controller.photonView.ownerId);
			}
			else if (PhotonNetwork.player.GetTeam() != Team.None)
			{
				CameraManager.SetType(CameraType.Static, new object[0]);
				this.cameraManager.OnSelectPlayer(-1);
			}
		}
		else
		{
			this.index++;
			if (this.index > list.Count - 1)
			{
				this.index = 0;
			}
			if (list.Count != 0)
			{
				this.target = list[this.index].PlayerSpectatePoint;
				this.cameraManager.OnSelectPlayer(list[this.index].Controller.photonView.ownerId);
			}
			else if (PhotonNetwork.player.GetTeam() != Team.None)
			{
				CameraManager.SetType(CameraType.Static, new object[0]);
				this.cameraManager.OnSelectPlayer(-1);
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
