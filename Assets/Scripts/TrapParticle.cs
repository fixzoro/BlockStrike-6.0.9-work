using System;
using UnityEngine;

public class TrapParticle : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key = 1;

    public ParticleSystem Target;

    public float DelayIn;

    public float DelayOut = 3f;

    private bool isTrigger;

    private bool Active;

    private bool Activated;

    private int Timer;

    private void Start()
	{
		this.Target.Stop();
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("Button" + this.Key, new EventManager.Callback(this.ActiveTrap));
	}

	private void ActiveTrap()
	{
		if (!this.Activated)
		{
			this.Activated = true;
			this.Timer = TimerManager.In(this.DelayIn, delegate()
			{
				this.Active = true;
				this.Target.Play();
				if (this.isTrigger)
				{
					DamageInfo damageInfo = DamageInfo.Get(1000, Vector3.zero, Team.None, 0, 0, -1, false);
					PlayerInput.instance.Damage(damageInfo);
				}
				if (this.DelayOut != 0f)
				{
					TimerManager.In(this.DelayOut, delegate()
					{
						if (this.Activated)
						{
							this.Active = false;
							this.Target.Stop();
						}
					});
				}
			});
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.isTrigger = true;
			if (this.Active)
			{
				DamageInfo damageInfo = DamageInfo.Get(1000, Vector3.zero, Team.None, 0, 0, -1, false);
				PlayerInput.instance.Damage(damageInfo);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.isTrigger = false;
		}
	}

	private void StartRound()
	{
		this.Target.Stop();
		this.Activated = false;
		this.Active = false;
		TimerManager.Cancel(this.Timer);
	}
}
