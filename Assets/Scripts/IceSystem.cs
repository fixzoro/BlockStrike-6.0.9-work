using System;
using UnityEngine;

public class IceSystem : MonoBehaviour
{
    private BoxCollider boxCollider;

    private Bounds bounds;

    private bool secure;

    private void Start()
	{
		base.gameObject.layer = 2;
		this.boxCollider = base.GetComponent<BoxCollider>();
		if (this.boxCollider == null)
		{
			return;
		}
		this.bounds = this.boxCollider.bounds;
		this.secure = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (this.secure)
		{
			if (this.bounds.Intersects(component.mCharacterController.bounds))
			{
				component.SetMoveIce(true);
			}
		}
		else
		{
			component.SetMoveIce(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		component.SetMoveIce(false);
	}
}
