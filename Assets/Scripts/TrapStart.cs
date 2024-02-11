using System;
using UnityEngine;

public class TrapStart : MonoBehaviour
{
    public GameObject Target;

    private int Timer;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.WaitPlayer));
	}

	private void StartRound()
	{
		this.Target.SetActive(true);
		TimerManager.Cancel(this.Timer);
		this.Timer = TimerManager.In(5f, delegate()
		{
			this.Target.SetActive(false);
		});
	}

	private void WaitPlayer()
	{
		TimerManager.Cancel(this.Timer);
		this.Target.SetActive(false);
	}
}
