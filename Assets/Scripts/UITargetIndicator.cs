using System;
using UnityEngine;

public class UITargetIndicator : MonoBehaviour
{
    public Transform Target;

    public UISprite Indicator;

    public Camera MainCamera;

    public float Limit = 0.9f;

    public float MaxDistance = 5f;

    public float MinDistance = 2f;

    private Vector2 center;

    private Vector2 LimitRect;

    private Vector2 LimitRect2;

    private Transform MainCameraTransform;

    private static UITargetIndicator instance;

    private void Start()
	{
		UITargetIndicator.instance = this;
		if (this.MainCamera != null)
		{
			this.MainCameraTransform = this.MainCamera.transform;
		}
		this.center = new Vector2(400f, 240f);
		this.LimitRect = new Vector2((1f - this.Limit) * 400f, (1f - this.Limit) * 240f);
		this.LimitRect2 = new Vector2(800f - this.LimitRect.x, 480f - this.LimitRect.y);
	}

	public static void SetTarget(Transform target)
	{
		UITargetIndicator.instance.Target = target;
	}

	public static void SetCamera(Camera cam)
	{
		UITargetIndicator.instance.MainCamera = cam;
		UITargetIndicator.instance.MainCameraTransform = cam.transform;
	}

	private void LateUpdate()
	{
		float num = Vector3.Distance(this.MainCameraTransform.position, this.Target.position);
		if (this.MaxDistance + this.MinDistance > num)
		{
			this.Indicator.alpha = (num - this.MinDistance) / this.MaxDistance;
			if (this.Indicator.alpha == 0f)
			{
				return;
			}
		}
		Vector3 localPosition = this.MainCamera.WorldToScreenPoint(this.Target.position);
		if (localPosition.z > 0f && localPosition.x > this.LimitRect.x && localPosition.x < this.LimitRect2.x && localPosition.y > this.LimitRect.y && localPosition.y < this.LimitRect2.y)
		{
			this.Indicator.cachedTransform.localPosition = new Vector3(localPosition.x - this.center.x, localPosition.y - this.center.y, 0f);
		}
		else
		{
			if (localPosition.z < 0f)
			{
				localPosition.z *= -1f;
			}
			localPosition.x -= this.center.x;
			localPosition.y -= this.center.y;
			float num2 = Mathf.Atan2(localPosition.y, localPosition.x);
			num2 -= 1.57079637f;
			float num3 = Mathf.Cos(num2);
			float num4 = -Mathf.Sin(num2);
			localPosition = this.center + new Vector2(num4 * 150f, num3 * 150f);
			float num5 = num3 / num4;
			Vector3 vector = this.center * this.Limit;
			if (num3 > 0f)
			{
				localPosition = new Vector3(vector.y / num5, vector.y, 0f);
			}
			else
			{
				localPosition = new Vector3(-vector.y / num5, -vector.y, 0f);
			}
			if (localPosition.x > vector.x)
			{
				localPosition = new Vector3(vector.x, vector.x * num5, 0f);
			}
			else if (localPosition.x < -vector.x)
			{
				localPosition = new Vector3(-vector.x, -vector.x * num5, 0f);
			}
			this.Indicator.cachedTransform.localPosition = localPosition;
		}
	}
}
