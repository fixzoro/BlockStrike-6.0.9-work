using System;
using UnityEngine;

public class SparkEffectManager : MonoBehaviour
{
    public ParticleSystem Spark;

    private Transform mTransform;

    private bool Active;

    private static SparkEffectManager instance;

    private Transform cacheTransform
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
		SparkEffectManager.instance = this;
	}

	public static void ClearParent()
	{
		SparkEffectManager.instance.cacheTransform.SetParent(null);
		SparkEffectManager.instance.Active = false;
	}

	public static void SetParent(Transform parent, Vector3 pos)
	{
		if (parent == null)
		{
			SparkEffectManager.instance.cacheTransform.SetParent(null);
			SparkEffectManager.instance.Active = false;
		}
		else
		{
			SparkEffectManager.instance.cacheTransform.SetParent(parent);
			SparkEffectManager.instance.cacheTransform.localPosition = pos;
			SparkEffectManager.instance.cacheTransform.localEulerAngles = new Vector3(0f, -4f, 0f);
			SparkEffectManager.instance.Active = true;
		}
	}

	public static void Fire(Vector3 point, float distance)
	{
		if (Settings.ProjectileEffect && distance > 2.5f && SparkEffectManager.instance.Active && UnityEngine.Random.value > 0.2f)
		{
			SparkEffectManager.instance.cacheTransform.LookAt(point);
			SparkEffectManager.instance.Spark.Emit(1);
		}
	}
}
