using System;
using UnityEngine;
using UnityEngine.Events;

public class EventListener2 : MonoBehaviour
{
    public UnityEvent onEnable;

    public UnityEvent onDisable;

    public UnityEvent onBecameVisible;

    public UnityEvent onBecameInvisible;

    private void OnEnable()
	{
		if (this.onEnable != null)
		{
			this.onEnable.Invoke();
		}
	}

	private void OnDisable()
	{
		if (this.onDisable != null)
		{
			this.onDisable.Invoke();
		}
	}

	private void OnBecameVisible()
	{
		if (this.onBecameVisible != null)
		{
			this.onBecameVisible.Invoke();
		}
	}

	private void OnBecameInvisible()
	{
		if (this.onBecameInvisible != null)
		{
			this.onBecameInvisible.Invoke();
		}
	}
}
