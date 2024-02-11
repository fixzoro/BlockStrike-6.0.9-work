using System;
using UnityEngine;

public class TrapAddForce : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key = 1;

    public Vector3 Force;

    public float Delay;

    private bool Activated;

    private bool ActiveForce;

    private bool isTrigger;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("Button" + this.Key, new EventManager.Callback(this.ActiveTrap));
	}

	private void ActiveTrap()
	{
		if (!this.Activated)
		{
			this.Activated = true;
			this.ActiveForce = true;
			if (this.isTrigger)
			{
				PlayerInput.instance.FPController.AddForce(this.Force);
			}
			TimerManager.In(this.Delay, delegate()
			{
				this.ActiveForce = false;
			});
		}
	}

	private void StartRound()
	{
		this.Activated = false;
		this.isTrigger = false;
		this.ActiveForce = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.isTrigger = true;
			if (this.ActiveForce)
			{
				PlayerInput.instance.FPController.AddForce(this.Force);
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
}
