using System;
using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{
    public Transform Target;

    public Vector3 Position;

    public SpawnPoint TargetElement;

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
		this.bounds = this.boxCollider.bounds;
		if (LevelManager.customScene)
		{
			this.Target = base.transform.Find("Finish");
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
			if (this.Position != Vector3.zero)
			{
				component.Controller.SetPosition(this.Position);
			}
			else if (this.Target != null)
			{
				component.Controller.SetPosition(this.Target.position);
				component.FPCamera.SetRotation(this.Target.eulerAngles, true, true);
			}
			else
			{
				component.Controller.SetPosition(this.TargetElement.spawnPosition);
			}
		}
	}
}
