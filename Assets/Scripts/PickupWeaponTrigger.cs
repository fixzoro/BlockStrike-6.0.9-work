using System;
using UnityEngine;

public class PickupWeaponTrigger : MonoBehaviour
{
    [SelectedWeapon(WeaponType.Rifle)]
    public int Weapon;

    private BoxCollider boxCollider;

    private void Start()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput player = other.GetComponent<PlayerInput>();
		if (player == null)
		{
			return;
		}
		if (this.boxCollider == null)
		{
			return;
		}
		if (!this.boxCollider.bounds.Intersects(player.mCharacterController.bounds))
		{
			return;
		}
		if (player.PlayerWeapon.GetWeaponData(WeaponType.Rifle).Enabled)
		{
			return;
		}
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, this.Weapon);
		player.PlayerWeapon.UpdateWeapon(WeaponType.Rifle, true);
		TimerManager.In(0.05f, delegate()
		{
			player.PlayerWeapon.SetWeapon(WeaponType.Rifle, false);
		});
	}
}
