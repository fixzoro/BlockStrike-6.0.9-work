using System;
using UnityEngine;

public class nColliderContainer : MonoBehaviour
{
    public static BetterList<nColliderContainer> list = new BetterList<nColliderContainer>();

    public static nColliderContainer container;

    private static nColliderContainer container2;

    public static nBoxCollider playerBoxCollider;

    public nBoxColliderBounds playerMain;

    public nColliderContainer.Data[] playerColliders;

    public nBoxColliderBounds nameCollider;

    public PlayerSkin playerSkin;

    private bool actived;

    public static void RaycastFire(Ray ray, float maxDistance)
	{
		nColliderContainer.container = null;
		nColliderContainer.container2 = null;
		nColliderContainer.playerBoxCollider = null;
		float num = 1000f;
		for (int i = 0; i < nColliderContainer.list.size; i++)
		{
			if (nColliderContainer.list[i].actived && nColliderContainer.list[i].playerMain.Raycast(ray, maxDistance) && num > nColliderContainer.list[i].playerMain.distance)
			{
				if (nColliderContainer.container != null)
				{
					nColliderContainer.container2 = nColliderContainer.container;
				}
				num = nColliderContainer.list[i].playerMain.distance;
				nColliderContainer.container = nColliderContainer.list[i];
			}
		}
		if (nColliderContainer.container == null)
		{
			return;
		}
		if (!nColliderContainer.RaycastFire2(ray, maxDistance) && nColliderContainer.container2 != null)
		{
			nColliderContainer.container = nColliderContainer.container2;
			nColliderContainer.RaycastFire2(ray, maxDistance);
		}
	}

	private static bool RaycastFire2(Ray ray, float maxDistance)
	{
		float num = 1000f;
		nColliderContainer.Data data = null;
		nColliderContainer.Data data2 = null;
		for (int i = 0; i < nColliderContainer.container.playerColliders.Length; i++)
		{
			if (nColliderContainer.container.playerColliders[i].mainCollider.Raycast(ray, maxDistance) && num > nColliderContainer.container.playerColliders[i].mainCollider.distance)
			{
				num = nColliderContainer.container.playerColliders[i].mainCollider.distance;
				if (data != null)
				{
					data2 = data;
				}
				data = nColliderContainer.container.playerColliders[i];
			}
		}
		if (data == null)
		{
			return false;
		}
		for (int j = 0; j < data.otherColliders.Length; j++)
		{
			if (data.otherColliders[j].Raycast(ray, maxDistance))
			{
				nColliderContainer.playerBoxCollider = data.otherColliders[j];
				return true;
			}
		}
		if (data2 != null)
		{
			for (int k = 0; k < data2.otherColliders.Length; k++)
			{
				if (data2.otherColliders[k].Raycast(ray, maxDistance))
				{
					nColliderContainer.playerBoxCollider = data2.otherColliders[k];
					return true;
				}
			}
		}
		return false;
	}

	public static void RaycastName(Ray ray, float maxDistance)
	{
		nColliderContainer.container = null;
		float num = 1000f;
		for (int i = 0; i < nColliderContainer.list.size; i++)
		{
			if (nColliderContainer.list[i].actived && nColliderContainer.list[i].nameCollider.Raycast(ray, maxDistance) && num > nColliderContainer.list[i].nameCollider.distance)
			{
				num = nColliderContainer.list[i].nameCollider.distance;
				nColliderContainer.container = nColliderContainer.list[i];
			}
		}
	}

	private void Start()
	{
		nColliderContainer.list.Add(this);
	}

	private void OnEnable()
	{
		this.actived = true;
	}

	private void OnDisable()
	{
		this.actived = false;
	}

	private void OnDestroy()
	{
		nColliderContainer.list.Remove(this);
	}

	[Serializable]
	public class Data
	{
		public nBoxColliderBounds mainCollider;

		public nBoxCollider[] otherColliders;
	}
}
