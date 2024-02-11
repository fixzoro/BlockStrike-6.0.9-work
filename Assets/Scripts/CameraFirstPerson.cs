using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirstPerson : MonoBehaviour
{
    public CameraManager cameraManager;

    private Camera weaponCamera;

    public Vector3 headPos;

    private ControllerManager target;

    private int index;

    private Dictionary<int, FPWeaponShooter> weaponList = new Dictionary<int, FPWeaponShooter>();

    private FPWeaponShooter selectWeapon;

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
		if (this.weaponCamera == null)
		{
			this.CreateWeaponCamera();
		}
		else
		{
			this.weaponCamera.gameObject.SetActive(true);
		}
		this.UpdateSelectPlayer(playerID);
		LODObject.Target = this.cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		UICrosshair.SetActiveCrosshair(true);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		ControllerManager.SetWeaponEvent += this.SetWeaponEvent;
		ControllerManager.SetDeadEvent += this.SetDeadEvent;
		ControllerManager.SetFireEvent += this.SetFireEvent;
		ControllerManager.SetReloadEvent += this.SetReloadEvent;
	}

	public void Deactive()
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		this.cameraTransform.gameObject.SetActive(false);
		if (this.weaponCamera != null)
		{
			this.weaponCamera.gameObject.SetActive(false);
		}
		this.DeactiveWeapons();
		if (this.target != null)
		{
			this.target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
		}
		UICrosshair.SetActiveCrosshair(false);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		ControllerManager.SetWeaponEvent -= this.SetWeaponEvent;
		ControllerManager.SetDeadEvent -= this.SetDeadEvent;
		ControllerManager.SetFireEvent -= this.SetFireEvent;
		ControllerManager.SetReloadEvent -= this.SetReloadEvent;
	}

	private void GetButtonDown(string name)
	{
        if (CameraManager.type == CameraType.FirstPerson)
        {
            switch (name)
            {
                case "Fire":
                    UpdateSelectPlayer();
                    break;
            }
        }
    }

	public void OnUpdate()
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		if (this.target == null)
		{
			CameraManager.SetType(CameraType.Spectate, new object[0]);
		}
		this.cameraTransform.localPosition = Vector3.Lerp(this.cameraTransform.localPosition, this.target.playerSkin.PhotonPosition + this.headPos, Time.deltaTime * 10f);
		this.cameraTransform.localRotation = Quaternion.Lerp(this.cameraTransform.localRotation, this.target.playerSkin.PhotonRotation * Quaternion.Euler(this.target.playerSkin.Rotate * -60f, 0f, 0f), Time.deltaTime * 10f);
		if (this.selectWeapon != null)
		{
			this.selectWeapon.FPWeapon.SpectatorVelocity = this.target.playerSkin.GetMove() * 30f;
		}
		SkyboxManager.GetCamera().rotation = this.cameraTransform.rotation;
	}

	public void UpdateSelectPlayer()
	{
		this.UpdateSelectPlayer(-1);
	}

	public void UpdateSelectPlayer(int playerID)
	{
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
						if (this.target != null)
						{
							this.target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
						}
						this.target = controllerManager;
						this.target.playerSkin.PlayerAnimator.rootPos = Vector3.back * 2f;
						this.cameraTransform.localPosition = this.target.playerSkin.PhotonPosition + this.headPos;
						this.cameraTransform.localRotation = this.target.playerSkin.PhotonRotation * Quaternion.Euler(this.target.playerSkin.Rotate * -60f, 0f, 0f);
						this.UpdateWeapon();
						this.cameraManager.OnSelectPlayer(this.target.photonView.ownerId);
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
		List<ControllerManager> list = new List<ControllerManager>();
		for (int j = 0; j < ControllerManager.ControllerList.Count; j++)
		{
			ControllerManager controllerManager = ControllerManager.ControllerList[j];
			if (CameraManager.Team)
			{
				if (!controllerManager.photonView.owner.GetDead() && controllerManager.photonView.owner.GetTeam() == PhotonNetwork.player.GetTeam())
				{
					list.Add(ControllerManager.ControllerList[j]);
				}
			}
			else if (!controllerManager.photonView.owner.GetDead())
			{
				list.Add(ControllerManager.ControllerList[j]);
			}
		}
		this.index++;
		if (this.index > list.Count - 1)
		{
			this.index = 0;
		}
		if (list.Count != 0)
		{
			if (this.target != null)
			{
				this.target.playerSkin.PlayerAnimator.rootPos = Vector3.zero;
			}
			this.target = list[this.index];
			if (this.target == null)
			{
				this.UpdateSelectPlayer();
			}
			this.target.playerSkin.PlayerAnimator.rootPos = Vector3.back * 2f;
			this.cameraTransform.localPosition = this.target.playerSkin.PhotonPosition + this.headPos;
			this.cameraTransform.localRotation = this.target.playerSkin.PhotonRotation * Quaternion.Euler(this.target.playerSkin.Rotate * -60f, 0f, 0f);
			this.UpdateWeapon();
			this.cameraManager.OnSelectPlayer(this.target.photonView.ownerId);
		}
		else
		{
			CameraManager.SetType(CameraType.Static, new object[0]);
			this.cameraManager.OnSelectPlayer(-1);
		}
	}

	private void UpdateWeapon()
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		this.DeactiveWeapons();
		if (this.target.playerSkin.SelectWeapon != null)
		{
			TPWeaponShooter tpweaponShooter = this.target.playerSkin.SelectWeapon;
			if (!this.weaponList.ContainsKey(tpweaponShooter.Data.weapon))
			{
				WeaponData weaponData = WeaponManager.GetWeaponData(tpweaponShooter.Data.weapon);
				GameObject gameObject = weaponData.FpsPrefab;
				gameObject = Utils.AddChild(gameObject, this.cameraTransform, default(Vector3), default(Quaternion));
				this.selectWeapon = gameObject.GetComponent<FPWeaponShooter>();
				this.selectWeapon.FPWeapon.Spectator = true;
				this.weaponList.Add(weaponData.ID, this.selectWeapon);
				gameObject.SetActive(true);
			}
			else
			{
				this.selectWeapon = this.weaponList[tpweaponShooter.Data.weapon];
				this.selectWeapon.FPWeapon.Activate();
			}
			this.selectWeapon.UpdateWeaponData(tpweaponShooter.Data.weapon, tpweaponShooter.Data.skin, tpweaponShooter.GetStickers(), tpweaponShooter.FireStat.value);
			this.selectWeapon.UpdateHandAtlas(this.target.playerSkin.PlayerTeam, this.target.playerSkin.BodySkin);
			UICrosshair.SetAccuracy(WeaponManager.GetWeaponData(tpweaponShooter.Data.weapon).Accuracy);
		}
	}

	private void SetWeaponEvent(int playerID, int weapon, int skin)
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		if (this.target != null && CameraManager.selectPlayer == playerID)
		{
			this.UpdateWeapon();
		}
	}

	private void SetDeadEvent(int playerID, bool dead)
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		if (this.target != null && CameraManager.selectPlayer == playerID)
		{
			if (dead)
			{
				this.DeactiveWeapons();
			}
			else
			{
				this.UpdateWeapon();
			}
		}
	}

	private void SetFireEvent(int playerID)
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		if (this.target != null && CameraManager.selectPlayer == playerID && this.selectWeapon != null)
		{
			this.selectWeapon.Fire();
		}
	}

	private void SetReloadEvent(int playerID)
	{
		if (CameraManager.type != CameraType.FirstPerson)
		{
			return;
		}
		if (this.target != null && CameraManager.selectPlayer == playerID && this.selectWeapon != null)
		{
			this.selectWeapon.Reload();
		}
	}

	private void CreateWeaponCamera()
	{
		if (this.weaponCamera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("WeaponCamera");
		gameObject.transform.SetParent(this.cameraTransform);
		Camera camera = gameObject.AddComponent<Camera>();
		camera.transform.localPosition = Vector3.zero;
		camera.transform.localEulerAngles = Vector3.zero;
		camera.clearFlags = CameraClearFlags.Depth;
		camera.cullingMask = nValue.int1 << 31;
		camera.depth = (float)nValue.int1;
		camera.farClipPlane = (float)nValue.int100;
		camera.nearClipPlane = nValue.float001;
		camera.fieldOfView = (float)nValue.int60;
		this.weaponCamera = camera;
	}

	public void DeactiveWeapons()
	{
		foreach (KeyValuePair<int, FPWeaponShooter> keyValuePair in this.weaponList)
		{
			keyValuePair.Value.Deactive();
		}
	}
}
