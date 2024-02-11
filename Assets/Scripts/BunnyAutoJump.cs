using System;
using UnityEngine;

public class BunnyAutoJump : MonoBehaviour
{
    private PlayerInput player;

    public CryptoInt jumpTime = 2;

    private int TimerID;

    private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			this.player = other.GetComponent<PlayerInput>();
			if (this.player != null)
			{
				this.player.SetBunnyHopAutoJump(true);
				this.TimerID = TimerManager.In((float)this.jumpTime, delegate()
				{
					BunnyHop.SpawnDead();
				});
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && this.player != null)
		{
			this.player.SetBunnyHopAutoJump(false);
			this.player = null;
			TimerManager.Cancel(this.TimerID);
		}
	}
}
