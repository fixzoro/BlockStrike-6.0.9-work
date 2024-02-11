using System;
using UnityEngine;

public class TrapEnable : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key = 1;

    public GameObject Target;

    public bool setStartPosition;

    public Vector3 startPosition;

    public bool Value;

    public float DelayIn;

    public float DelayOut = 3f;

    private bool Activated;

    private void Start()
	{
		if (this.Target == null)
		{
			this.Target = base.gameObject;
		}
		if (this.setStartPosition)
		{
			base.transform.position = this.startPosition;
		}
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("Button" + this.Key, new EventManager.Callback(this.ActiveTrap));
	}

	[ContextMenu("Get Start Position")]
	private void GetStart()
	{
		this.startPosition = base.transform.position;
	}

	[ContextMenu("Get Transform")]
	private void GetTransform()
	{
		this.Target = base.gameObject;
	}

	private void ActiveTrap()
	{
		if (!this.Activated)
		{
			this.Activated = true;
			TimerManager.In(this.DelayIn, delegate()
			{
				this.Target.SetActive(this.Value);
				if (this.DelayOut != 0f)
				{
					TimerManager.In(this.DelayOut, delegate()
					{
						if (this.Activated)
						{
							this.Target.SetActive(!this.Value);
						}
					});
				}
			});
		}
	}

	private void StartRound()
	{
		this.Target.SetActive(!this.Value);
		this.Activated = false;
	}
}
