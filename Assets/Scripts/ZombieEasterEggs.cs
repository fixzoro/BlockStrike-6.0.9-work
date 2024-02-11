using System;
using UnityEngine;

public class ZombieEasterEggs : MonoBehaviour
{
    [Disabled]
    public bool Complete;

    public Transform TeleportPosition;

    public ZombieEasterEggs[] OtherEggs;

    private bool isTrigger;

    private void Start()
	{
		if (this.TeleportPosition != null)
		{
			EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		}
	}

	private void StartRound()
	{
		this.Complete = false;
		for (int i = 0; i < this.OtherEggs.Length; i++)
		{
			this.OtherEggs[i].Complete = false;
		}
	}

	private void GetButtonDown(string name)
	{
		if (GameManager.player.Dead)
		{
			return;
		}
		if (name == "Fire" && this.isTrigger)
		{
			this.Complete = true;
			if (this.OtherEggs != null)
			{
				for (int i = 0; i < this.OtherEggs.Length; i++)
				{
					if (!this.OtherEggs[i].Complete)
					{
						return;
					}
				}
			}
			if (this.TeleportPosition != null)
			{
				PlayerInput.instance.Controller.SetPosition(this.TeleportPosition.position);
				PlayerInput.instance.FPCamera.SetRotation(this.TeleportPosition.eulerAngles, true, true);
				this.StartRound();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		this.isTrigger = true;
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component != null)
		{
			this.isTrigger = false;
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		}
	}
}
