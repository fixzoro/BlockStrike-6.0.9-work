using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class TrapPath : MonoBehaviour
{
    public Transform Target;

    public Transform Target2;

    public Transform[] Points;

    public float Duration = 60f;

    public float Delay = 20f;

    public float Delay2 = 25f;

    public Color GizmosColor = Color.white;

    private Tween tween;

    private Tween tween2;

    private Vector3 StartPosition;

    private Quaternion StartRotation;

    private void Start()
	{
		this.StartPosition = this.Target.position;
		this.StartRotation = this.Target.rotation;
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
	}

	private void OnWaypointChange(int waypointIndex)
	{
		if (this.Points.Length != waypointIndex)
		{
			this.Target.DOLookAt(this.Points[waypointIndex].position, nValue.float03, AxisConstraint.None, null);
		}
	}

	private void OnWaypointChange2(int waypointIndex)
	{
		if (this.Points.Length != waypointIndex)
		{
			this.Target2.DOLookAt(this.Points[waypointIndex].position, nValue.float03, AxisConstraint.None, null);
		}
	}

	private void StartRound()
	{
		if (this.tween != null)
		{
			this.tween.Kill(false);
			this.Target.position = this.StartPosition;
			this.Target.rotation = this.StartRotation;
		}
		if (this.tween2 != null && this.Target2 != null)
		{
			this.tween2.Kill(false);
			this.Target2.position = this.StartPosition;
			this.Target2.rotation = this.StartRotation;
		}
		Vector3[] array = new Vector3[this.Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.Points[i].position;
		}
		this.tween = this.Target.DOPath(array, this.Duration, PathType.Linear, PathMode.Full3D, 10, null).SetDelay(this.Delay).OnWaypointChange(new TweenCallback<int>(this.OnWaypointChange));
		if (this.Target2 != null)
		{
			this.tween2 = this.Target2.DOPath(array, this.Duration, PathType.Linear, PathMode.Full3D, 10, null).SetDelay(this.Delay2).OnWaypointChange(new TweenCallback<int>(this.OnWaypointChange2));
		}
	}
}
