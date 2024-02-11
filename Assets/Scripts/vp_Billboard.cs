using System;
using UnityEngine;

public class vp_Billboard : MonoBehaviour
{
    public Transform m_CameraTransform;

    private Transform m_Transform;

    protected virtual void Start()
	{
		this.m_Transform = base.transform;
		if (this.m_CameraTransform == null)
		{
			this.m_CameraTransform = Camera.main.transform;
		}
	}

	protected virtual void Update()
	{
		if (this.m_CameraTransform != null)
		{
			this.m_Transform.localEulerAngles = this.m_CameraTransform.eulerAngles;
		}
		this.m_Transform.localEulerAngles = this.m_Transform.localEulerAngles;
	}
}
