using System;
using UnityEngine;

public class FootballOut : MonoBehaviour
{
    public FootballMode Target;

    private void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.isMasterClient && other.CompareTag("RigidbodyObject"))
		{
			FootballManager.StartRound();
		}
		if (other.CompareTag("Player"))
		{
			if (GameManager.roundState == RoundState.PlayRound)
			{
				PlayerInput.instance.SetMove(false, (float)nValue.int5);
			}
			this.Target.OnCreatePlayer();
		}
	}
}
