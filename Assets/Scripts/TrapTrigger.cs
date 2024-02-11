using System;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.layer = 2;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			DamageInfo damageInfo = DamageInfo.Get(nValue.int1000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
			PlayerInput.instance.Damage(damageInfo);
		}
	}
}
