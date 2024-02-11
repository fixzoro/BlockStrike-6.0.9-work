using System;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    public bool secure;

    private BoxCollider boxCollider;

    private Bounds bounds;

    private bool isTrigger;

    private void Start()
	{
		base.gameObject.layer = 2;
		if (this.secure)
		{
			this.boxCollider = base.GetComponent<BoxCollider>();
			this.bounds = this.boxCollider.bounds;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			DamageInfo damageInfo = DamageInfo.Get(nValue.int10000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
			PlayerInput.instance.Damage(damageInfo);
			if (this.secure)
			{
				TimerManager.In(0.1f, delegate()
				{
					if (!GameManager.player.Dead && this.bounds.Intersects(GameManager.player.mCharacterController.bounds))
					{
						PlayerInput.instance.Damage(damageInfo);
					}
				});
			}
		}
	}
}
