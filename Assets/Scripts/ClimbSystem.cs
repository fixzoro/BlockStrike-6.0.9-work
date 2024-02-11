using System;
using UnityEngine;

public class ClimbSystem : MonoBehaviour
{
    private CryptoVector3 center;

    private CryptoVector3 size;

    private BoxCollider boxCollider;

    private Bounds bounds;

    private void Start()
	{
		base.gameObject.layer = 2;
		this.boxCollider = base.GetComponent<BoxCollider>();
		if (this.boxCollider == null)
		{
			return;
		}
		this.center = this.boxCollider.center;
		this.size = this.boxCollider.size;
		this.bounds = this.boxCollider.bounds;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && this.boxCollider != null)
		{
			this.boxCollider.center = this.center;
			this.boxCollider.size = this.size;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (this.bounds.Intersects(component.mCharacterController.bounds))
		{
			component.SetClimb(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		component.SetClimb(false);
	}
}
