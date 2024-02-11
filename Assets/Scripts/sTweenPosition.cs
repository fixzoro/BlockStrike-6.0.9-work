using System;
using UnityEngine;

public class sTweenPosition : MonoBehaviour
{
    public float speed = 1f;

    public Vector3 vector = Vector3.one;

    public float delay;

    public bool debug;

    private Transform mTransform;

    private Vector3 startPosition;

    public bool changePosition;

    public Vector3 position;

    private void Start()
	{
		if (this.changePosition)
		{
			base.transform.localPosition = this.position;
		}
		this.mTransform = base.transform;
		this.startPosition = base.transform.localPosition;
	}

	private void FixedUpdate()
	{
		this.mTransform.localPosition = this.startPosition + this.vector * Mathf.PingPong((sTweenTime.time + this.delay) * this.speed, 1f);
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		this.position = base.transform.localPosition;
	}
}
