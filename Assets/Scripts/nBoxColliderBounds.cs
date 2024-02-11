using System;
using UnityEngine;

public class nBoxColliderBounds : MonoBehaviour
{
    public Vector3 center = Vector3.zero;

    public Vector3 size = Vector3.one;

    public Renderer meshRenderer;

    [HideInInspector]
    public float distance = 1000f;

    public bool gizmo = true;

    private Bounds bounds = default(Bounds);

    private Transform mTransform;

    public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}

	private void Start()
	{
		this.bounds.size = this.size;
	}

	public bool Raycast(Ray ray, float maxDistance)
	{
		if (!this.meshRenderer.isVisible)
		{
			this.distance = 1000f;
			return false;
		}
		this.bounds.center = this.center + this.cachedTransform.position;
		if (this.bounds.IntersectRay(ray, out this.distance) && this.distance <= maxDistance)
		{
			return true;
		}
		this.distance = 1000f;
		return false;
	}
}
