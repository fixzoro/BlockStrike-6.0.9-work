using System;
using UnityEngine;

public class PlayerAI2 : MonoBehaviour
{
    public Transform target;

    public float navigatorTime = 0.1f;

    public PlayerAI2.AttackData attack;

    public UnityEngine.AI.NavMeshAgent nav;

    private void Start()
	{
		TimerManager.In(this.navigatorTime, -1, this.navigatorTime, new TimerManager.Callback(this.UpdateNavigator));
		TimerManager.In(this.attack.speed, -1, this.attack.speed, new TimerManager.Callback(this.CheckAttackDistance));
	}

	private void UpdateNavigator()
	{
		if (this.target != null)
		{
			this.nav.SetDestination(this.target.position);
		}
		else if (this.nav.remainingDistance < 1f)
		{
			this.nav.SetDestination(base.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f)));
		}
	}

	private void Update()
	{
	}

	private void CheckAttackDistance()
	{
		if (this.target == null)
		{
			return;
		}
		if (Vector3.Distance(this.target.position, base.transform.position) <= this.attack.distance)
		{
			this.Attack();
		}
	}

	private void Attack()
	{
		MonoBehaviour.print("Attack");
	}

	public void SetTarget(Transform t)
	{
		this.target = t;
	}

	[Serializable]
	public class AttackData
	{
		public float damage = 20f;

		public float distance = 5f;

		public float speed = 0.5f;
	}
}
