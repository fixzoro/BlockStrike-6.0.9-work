using System;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 Rotate;

    private Transform Target;

    private void Start()
	{
		this.Target = base.transform;
	}

	private void Update()
	{
		this.Target.Rotate(this.Rotate);
	}
}
