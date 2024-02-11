using System;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    public UnityEvent Callback;

    private bool Activated;

    private void OnTriggerEnter(Collider other)
	{
		if (this.Activated)
		{
			return;
		}
		if (other.CompareTag("Player"))
		{
			this.Callback.Invoke();
			this.Activated = true;
		}
	}
}
