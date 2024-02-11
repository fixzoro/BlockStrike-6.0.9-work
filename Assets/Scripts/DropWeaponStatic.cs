using System;
using UnityEngine;

public class DropWeaponStatic : MonoBehaviour
{
    [SelectedWeapon]
    public int weaponID;

    public bool useCustomData;

    public bool updatePlayerData = true;

    public WeaponCustomData customData;

    public MeshAtlas[] weaponMeshes;

    private bool isActive;

    private BoxCollider boxCollider;

    public event Action onDropWeaponEvent;

	private void Start()
	{
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
		if (this.useCustomData && this.customData.Skin != -1)
		{
			for (int i = 0; i < this.weaponMeshes.Length; i++)
			{
				this.weaponMeshes[i].spriteName = this.weaponID + "-" + this.customData.Skin;
			}
		}
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	private void GetButtonDown(string name)
	{
		if (this.isActive && name == "Use")
		{
			if (PlayerInput.instance.Dead)
			{
				this.Deactive();
				return;
			}
			if (this.updatePlayerData)
			{
				WeaponType type = WeaponManager.GetWeaponData(this.weaponID).Type;
				WeaponManager.SetSelectWeapon(type, this.weaponID);
				PlayerInput.instance.PlayerWeapon.UpdateWeapon(type, true, (!this.useCustomData) ? null : this.customData);
			}
			this.Deactive();
			if (this.onDropWeaponEvent != null)
			{
				this.onDropWeaponEvent();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (this.boxCollider == null)
		{
			return;
		}
		if (!this.boxCollider.bounds.Intersects(component.mCharacterController.bounds))
		{
			return;
		}
		this.isActive = true;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		UIControllerList.Use.cachedGameObject.SetActive(true);
		UIControllerList.UseText.text = Localization.Get("Pick up", true) + " " + WeaponManager.GetWeaponName(this.weaponID);
	}

	private void OnTriggerExit(Collider other)
	{
		if (!this.isActive)
		{
			return;
		}
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		this.Deactive();
	}

	private void OnDeadPlayer(DamageInfo info)
	{
		this.Deactive();
	}

	private void Deactive()
	{
		this.isActive = false;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		UIControllerList.Use.cachedGameObject.SetActive(false);
		UIControllerList.UseText.text = string.Empty;
	}
}
