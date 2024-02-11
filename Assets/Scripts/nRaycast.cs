using System;
using UnityEngine;

public static class nRaycast
{
    public static nBoxCollider hitBoxCollider;

    public static float hitDistance;

    public static Vector3 hitPoint;

    public static nColliderContainer container;

    private static RaycastHit hitInfo;

    private static Vector3 e1;

    private static Vector3 e2;

    private static Vector3 p;

    private static Vector3 q;

    private static Vector3 t;

    private static float det;

    private static float invDet;

    private static float u;

    private static float v;

    private static float result;

    public static bool RaycastName(Ray ray, float maxDistance)
	{
		nProfiler.BeginSample("nRaycast.RaycastName");
		nColliderContainer.RaycastName(ray, maxDistance);
		if (nColliderContainer.container == null)
		{
			nProfiler.EndSample();
			return false;
		}
		if (Physics.Raycast(ray, out nRaycast.hitInfo, maxDistance) && nRaycast.hitInfo.distance < nColliderContainer.container.nameCollider.distance)
		{
			nProfiler.EndSample();
			return false;
		}
		nRaycast.container = nColliderContainer.container;
		nProfiler.EndSample();
		return true;
	}

	public static bool RaycastFire(Ray ray, float maxDistance, int layerMask)
	{
		nProfiler.BeginSample("nRaycast.RaycastFire");
		nColliderContainer.RaycastFire(ray, maxDistance);
		if (nColliderContainer.container == null || nColliderContainer.playerBoxCollider == null)
		{
			nProfiler.EndSample();
			return false;
		}
		if (Physics.Raycast(ray, out nRaycast.hitInfo, maxDistance, layerMask) && nRaycast.hitInfo.distance < nColliderContainer.playerBoxCollider.distance)
		{
			nProfiler.EndSample();
			return false;
		}
		nRaycast.container = nColliderContainer.container;
		nRaycast.hitDistance = nColliderContainer.playerBoxCollider.distance;
		nRaycast.hitPoint = ray.GetPoint(nRaycast.hitDistance);
		nRaycast.hitBoxCollider = nColliderContainer.playerBoxCollider;
		nProfiler.EndSample();
		return true;
	}

	public static float Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
	{
		nRaycast.e1 = p2 - p1;
		nRaycast.e2 = p3 - p1;
		nRaycast.p = Vector3.Cross(ray.direction, nRaycast.e2);
		nRaycast.det = Vector3.Dot(nRaycast.e1, nRaycast.p);
		if (nRaycast.det > -1.401298E-45f && nRaycast.det < 1.401298E-45f)
		{
			return float.MaxValue;
		}
		nRaycast.invDet = 1f / nRaycast.det;
		nRaycast.t = ray.origin - p1;
		nRaycast.u = Vector3.Dot(nRaycast.t, nRaycast.p) * nRaycast.invDet;
		if (nRaycast.u < 0f || nRaycast.u > 1f)
		{
			return float.MaxValue;
		}
		nRaycast.q = Vector3.Cross(nRaycast.t, nRaycast.e1);
		nRaycast.v = Vector3.Dot(ray.direction, nRaycast.q) * nRaycast.invDet;
		if (nRaycast.v < 0f || nRaycast.u + nRaycast.v > 1f)
		{
			return float.MaxValue;
		}
		nRaycast.result = Vector3.Dot(nRaycast.e2, nRaycast.q) * nRaycast.invDet;
		if (nRaycast.result > 1.401298E-45f)
		{
			return nRaycast.result;
		}
		return float.MaxValue;
	}
}
