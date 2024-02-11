using System;
using UnityEngine;

public class ShootingRangeAmmo : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(PlayerInput.instance.PlayerWeapon.SelectedWeapon);
		}
	}
}
