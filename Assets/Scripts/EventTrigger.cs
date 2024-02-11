using System;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;

    public UnityEvent TriggerExit;

    private BoxCollider boxCollider;

    private void Start()
	{
		this.boxCollider = base.GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (this.boxCollider == null)
		{
			return;
		}
		if (!this.boxCollider.bounds.Intersects(component.mCharacterController.bounds))
		{
			return;
		}
		this.TriggerEnter.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		this.TriggerExit.Invoke();
	}
}
