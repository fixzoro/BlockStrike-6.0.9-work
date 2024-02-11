using System;
using DG.Tweening;
using UnityEngine;

public class TrapPosition : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key = 1;

    public Transform Target;

    public Vector3 PositionIn;

    public Vector3 PositionOut;

    public float Duration;

    public float DelayIn;

    public float DelayOut = 3f;

    public int Loop;

    private Tweener Tween;

    private bool Activated;

    private void Start()
	{
		if (this.Target == null)
		{
			this.Target = base.transform;
		}
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("Button" + this.Key, new EventManager.Callback(this.ActiveTrap));
		this.StartRound();
	}

	[ContextMenu("Get Transform")]
	private void GetTarget()
	{
		this.Target = base.transform;
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		this.PositionIn = this.Target.localPosition;
		this.PositionOut = this.Target.localPosition;
	}

	private void ActiveTrap()
	{
		if (!this.Activated)
		{
			this.Tween = this.Target.DOLocalMove(this.PositionOut, this.Duration, false).OnComplete(new TweenCallback(this.ResetTrap)).SetDelay(this.DelayIn).SetLoops(this.Loop, LoopType.Yoyo);
			this.Activated = true;
		}
	}

	private void ResetTrap()
	{
		if (this.Activated && this.DelayOut != 0f)
		{
			this.Tween = this.Target.DOLocalMove(this.PositionIn, this.Duration, false).SetDelay(this.DelayOut);
		}
	}

	private void StartRound()
	{
		if (this.Tween != null)
		{
			this.Tween.Kill(false);
		}
		this.Target.localPosition = this.PositionIn;
		this.Activated = false;
	}
}
