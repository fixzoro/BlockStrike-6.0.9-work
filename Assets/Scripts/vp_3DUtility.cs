using System;
using UnityEngine;

public static class vp_3DUtility
{
	public static Vector3 HorizontalVector(Vector3 value)
	{
		value.y = 0f;
		return value;
	}

	public static bool OnScreen(Camera camera, Renderer renderer, Vector3 worldPosition, out Vector3 screenPosition)
	{
		screenPosition = Vector2.zero;
		if (camera == null || renderer == null || !renderer.isVisible)
		{
			return false;
		}
		screenPosition = camera.WorldToScreenPoint(worldPosition);
		return screenPosition.z >= 0f;
	}

	public static bool InLineOfSight(Vector3 from, Transform target, Vector3 targetOffset, int layerMask)
	{
		RaycastHit raycastHit;
		Physics.Linecast(from, target.position + targetOffset, out raycastHit, layerMask);
		return raycastHit.collider == null || raycastHit.collider.transform.root == target;
	}

	public static bool WithinRange(Vector3 from, Vector3 to, float range, out float distance)
	{
		distance = Vector3.Distance(from, to);
		return distance <= range;
	}

	public static float DistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	public static float LookAtAngle(Vector3 sourcePosition, Vector3 sourceDirection, Vector3 targetPosition)
	{
		return Mathf.Acos(Vector3.Dot((sourcePosition - targetPosition).normalized, -sourceDirection)) * 57.29578f;
	}
}
