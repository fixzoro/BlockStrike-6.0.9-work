using System;
using DG.Tweening;
using UnityEngine;

public class TrapRotation : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key = 1;

    public Transform Target;

    public bool setStartPosition;

    public Vector3 startPosition;

    public int repeatTimes;

    private int repeat;

    public Vector3 RotationIn;

    public Vector3 RotationOut;

    public RotateMode Mode;

    public float Duration;

    public float DelayIn;

    public float DelayOut = 3f;

    private Tweener Tween;

    public bool Activated;

    private void Start()
	{
		if (this.Target == null)
		{
			this.Target = base.transform;
		}
		if (this.setStartPosition)
		{
			base.transform.position = this.startPosition;
		}
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("Button" + this.Key, new EventManager.Callback(this.ActiveTrap));
	}

	[ContextMenu("Get Object")]
	private void GetObject()
	{
		this.Target = base.transform;
	}

	[ContextMenu("Get Start Position")]
	private void GetStart()
	{
		this.startPosition = base.transform.position;
	}

	[ContextMenu("Get Rotation")]
	private void GetValue()
	{
		this.RotationIn = this.Target.localEulerAngles;
		this.RotationOut = this.Target.localEulerAngles;
	}

	private void ActiveTrap()
	{
		if (!this.Activated)
		{
			this.Tween = this.Target.DORotate(this.RotationOut, this.Duration, this.Mode).OnComplete(new TweenCallback(this.ResetTrap)).SetDelay(this.DelayIn);
			this.Activated = true;
		}
	}

	private void ResetTrap()
	{
		if (this.Activated && this.repeat == 0)
		{
			this.Tween = this.Target.DORotate(this.RotationIn, this.Duration, this.Mode).SetDelay(this.DelayOut);
		}
		else if (this.repeat > 0)
		{
			this.Tween = this.Target.DORotate(this.RotationIn, this.Duration, this.Mode).OnComplete(new TweenCallback(this.ActiveTrap)).SetDelay(this.DelayOut);
			this.repeat--;
			this.Activated = false;
		}
	}

	private void StartRound()
	{
		if (this.Tween != null)
		{
			this.Tween.Kill(false);
		}
		this.Target.localEulerAngles = this.RotationIn;
		this.repeat = this.repeatTimes;
		this.Activated = false;
	}
}
