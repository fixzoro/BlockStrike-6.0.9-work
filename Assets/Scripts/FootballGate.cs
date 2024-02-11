using System;
using UnityEngine;

public class FootballGate : MonoBehaviour
{
    public Team Team;

    public FootballMode Target;

    private void OnTriggerEnter(Collider other)
	{
		if (PhotonNetwork.isMasterClient && GameManager.roundState == RoundState.PlayRound && other.CompareTag("RigidbodyObject"))
		{
			RigidbodyObject component = other.GetComponent<RigidbodyObject>();
			if (component != null)
			{
				this.Target.Goal(component.GetLastContactPlayer(), this.Team);
			}
		}
	}
}
