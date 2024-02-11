using System;
using UnityEngine;

public class RadiationTrigger : MonoBehaviour
{
    public Team PlayerTeam;

    public int DamageMin;

    public int DamageMax;

    public float Period = 1.5f;

    private BoxCollider boxCollider;

    private Bounds bounds;

    private void Start()
	{
		base.gameObject.layer = 2;
		this.boxCollider = base.GetComponent<BoxCollider>();
		this.bounds = this.boxCollider.bounds;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == this.PlayerTeam && this.bounds.Intersects(component.mCharacterController.bounds))
			{
				base.InvokeRepeating("PlayerDamage", 0.2f, this.Period);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == this.PlayerTeam)
			{
				base.CancelInvoke("PlayerDamage");
			}
		}
	}

	private void PlayerDamage()
	{
		PlayerInput instance = PlayerInput.instance;
		if (instance.PlayerTeam == this.PlayerTeam)
		{
			DamageInfo damageInfo = DamageInfo.Get(UnityEngine.Random.Range(this.DamageMin, this.DamageMax), instance.PlayerTransform.position, Team.None, 0, 0, -1, false);
			instance.Damage(damageInfo);
		}
		else
		{
			base.CancelInvoke("PlayerDamage");
		}
	}
}
