using System;
using UnityEngine;

public class vp_Spin : MonoBehaviour
{
    public Vector3 RotationSpeed = new Vector3(0f, 90f, 0f);

    private Transform m_Transform;

    protected virtual void Start()
	{
		this.m_Transform = base.transform;
	}

	protected virtual void Update()
	{
		this.m_Transform.Rotate(this.RotationSpeed * Time.deltaTime);
	}
}
