using System;
using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    public bool freeGravity;

    private BoxCollider boxCollider;

    private void Start()
	{
		base.gameObject.layer = 2;
		this.boxCollider = base.GetComponent<BoxCollider>();
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
		if (this.boxCollider != null)
		{
			if (this.boxCollider.bounds.Intersects(component.mCharacterController.bounds))
			{
				component.SetWater(true, this.freeGravity);
			}
		}
		else
		{
			component.SetWater(true, this.freeGravity);
		}
	}

	private void OnTriggerExit(Collider other)
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
		component.SetWater(false);
	}
}
