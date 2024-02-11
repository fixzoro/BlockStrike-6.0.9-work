using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, PoolManager.PoolObject> list = new Dictionary<string, PoolManager.PoolObject>();

    private Transform mTransform;

    private static PoolManager instance;

    private Transform cachedTransform
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

	public static void ClearAll()
	{
		if (PoolManager.instance == null)
		{
			return;
		}
		foreach (KeyValuePair<string, PoolManager.PoolObject> keyValuePair in PoolManager.instance.list)
		{
			for (int i = 0; i < keyValuePair.Value.cache.Count; i++)
			{
				UnityEngine.Object.Destroy(keyValuePair.Value.cache[i]);
			}
		}
		PoolManager.instance.list.Clear();
	}

	private static void Init()
	{
		if (PoolManager.instance == null)
		{
			GameObject gameObject = new GameObject("PoolManager");
			PoolManager.instance = gameObject.AddComponent<PoolManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static bool isContains(string key)
	{
		return PoolManager.instance.list.ContainsKey(key);
	}

	public static GameObject Spawn(string key, GameObject prefab)
	{
		return PoolManager.Spawn(key, prefab, Vector3.zero, Vector3.zero, null);
	}

	public static GameObject Spawn(string key, GameObject prefab, Vector3 position, Vector3 rotation)
	{
		return PoolManager.Spawn(key, prefab, position, rotation, null);
	}

	public static GameObject Spawn(string key, GameObject prefab, Vector3 position, Vector3 rotation, Transform parent)
	{
		PoolManager.Init();
		if (prefab == null)
		{
			Debug.LogError("Prefab null");
			return null;
		}
		if (!PoolManager.instance.list.ContainsKey(key))
		{
			PoolManager.PoolObject poolObject = new PoolManager.PoolObject();
			poolObject.prefab = prefab;
			PoolManager.instance.list.Add(key, poolObject);
		}
		PoolManager.PoolObject poolObject2 = PoolManager.instance.list[key];
		GameObject gameObject;
		if (poolObject2.cache.Count > 0)
		{
			gameObject = poolObject2.cache[0];
			poolObject2.cache.RemoveAt(0);
			Transform transform = gameObject.transform;
			transform.SetParent(parent, false);
			transform.localPosition = position;
			transform.localEulerAngles = rotation;
			gameObject.SetActive(true);
		}
		else
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab, position, Quaternion.Euler(rotation));
			gameObject.name = prefab.name + " [Pool]";
			gameObject.transform.SetParent(parent, false);
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public static void Despawn(string key, GameObject prefab)
	{
		PoolManager.Init();
		if (prefab == null)
		{
			Debug.LogError("Prefab null");
			return;
		}
		if (!PoolManager.instance.list.ContainsKey(key))
		{
			PoolManager.PoolObject poolObject = new PoolManager.PoolObject();
			poolObject.prefab = prefab;
			PoolManager.instance.list.Add(key, poolObject);
		}
		PoolManager.instance.list[key].cache.Add(prefab);
		prefab.SetActive(false);
		prefab.transform.SetParent(PoolManager.instance.cachedTransform);
	}

	public static int CacheCount(string key)
	{
		PoolManager.Init();
		return PoolManager.instance.list[key].cache.Count;
	}

	public class PoolObject
	{
		public GameObject prefab;

		public List<GameObject> cache = new List<GameObject>();
	}
}
