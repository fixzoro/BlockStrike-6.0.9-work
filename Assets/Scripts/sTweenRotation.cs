using System;
using UnityEngine;

public class sTweenRotation : MonoBehaviour
{
    public Vector3 speed = Vector3.forward;

    private Transform mTransform;

    private Vector3 startEulerAngles;

    public bool changePosition;

    public Vector3 position;

    private void Start()
	{
		if (this.changePosition)
		{
			base.transform.localPosition = this.position;
		}
		this.mTransform = base.transform;
		this.startEulerAngles = base.transform.localEulerAngles;
	}

	private void FixedUpdate()
	{
		this.mTransform.localEulerAngles = this.startEulerAngles + this.speed * Time.time * 10f;
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		this.position = base.transform.localPosition;
	}
}
