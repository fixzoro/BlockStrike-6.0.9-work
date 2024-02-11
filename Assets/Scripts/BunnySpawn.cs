using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BunnySpawn : MonoBehaviour
{
    public bool FinishSpawn;

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
				if (this.FinishSpawn)
				{
					BunnyHop.FinishMap(this.XP, this.Money);
				}
				else
				{
					SpawnManager.GetTeamSpawn().cachedTransform.position = base.transform.position;
					SpawnManager.GetTeamSpawn().cachedTransform.rotation = base.transform.rotation;
				}
			}
		}
	}
}
