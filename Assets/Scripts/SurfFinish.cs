using System;
using UnityEngine;

public class SurfFinish : MonoBehaviour
{
    public CryptoInt XP;

    public CryptoInt Money;

    private BoxCollider boxCollider;

    private Bounds bounds;

    private void Start()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
		this.bounds = this.boxCollider.bounds;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && this.bounds.Intersects(component.mCharacterController.bounds))
			{
				SurfMode.FinishMap(this.XP, this.Money);
			}
		}
	}
}
