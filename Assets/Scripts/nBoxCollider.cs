using System;
using UnityEngine;

[ExecuteInEditMode]
public class nBoxCollider : MonoBehaviour
{
    public Vector3 center = Vector3.zero;

    public Vector3 size = Vector3.one;

    public PlayerSkinDamage playerDamage;

    [HideInInspector]
    public float distance = 1000f;

    private Transform mTransform;

    private Vector3[] coordinates = new Vector3[]
    {
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f)
    };

    private Vector3[] box = new Vector3[8];

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

	public bool Raycast(Ray ray, float maxDistance)
	{
		this.UpdateCoordinates();
		this.distance = nRaycast.Intersect(this.box[0], this.box[3], this.box[1], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[0], this.box[2], this.box[3], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[2], this.box[5], this.box[3], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[2], this.box[4], this.box[5], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[4], this.box[7], this.box[5], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[4], this.box[6], this.box[7], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[6], this.box[1], this.box[7], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[6], this.box[0], this.box[1], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[1], this.box[5], this.box[7], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[1], this.box[3], this.box[5], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[6], this.box[2], this.box[0], ray);
		if (this.distance < maxDistance)
		{
			return true;
		}
		this.distance = nRaycast.Intersect(this.box[6], this.box[4], this.box[2], ray);
		return this.distance < maxDistance;
	}

	public void UpdateCoordinates()
	{
		for (int i = 0; i < 8; i++)
		{
			this.box[i].x = (this.coordinates[i].x + this.center.x) * this.size.x;
			this.box[i].y = (this.coordinates[i].y + this.center.y) * this.size.y;
			this.box[i].z = (this.coordinates[i].z + this.center.z) * this.size.z;
			this.box[i] = this.cachedTransform.TransformPoint(this.box[i]);
		}
	}
}
