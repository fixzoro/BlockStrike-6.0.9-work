using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000554 RID: 1364
public class PhotonPoolManager : MonoBehaviour, IPunPrefabPool
{
	// Token: 0x06002E22 RID: 11810 RVA: 0x00020162 File Offset: 0x0001E362
	private void Start()
	{
		if (PhotonPoolManager.instance != null)
		{
			this.Destroy(base.gameObject);
			return;
		}
		PhotonPoolManager.instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x0010BA9C File Offset: 0x00109C9C
	public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject;
		Transform transform;
		for (int i = 0; i < this.pool.Count; i++)
		{
			if (this.poolID[i] == prefabId)
			{
				gameObject = this.pool[i];
				this.pool.RemoveAt(i);
				this.poolID.RemoveAt(i);
				this.listID.Add(prefabId);
				this.list.Add(gameObject);
				transform = gameObject.transform;
				transform.position = position;
				transform.rotation = rotation;
				return gameObject;
			}
		}
		for (int j = 0; j < this.prefabs.Length; j++)
		{
			if (this.prefabID[j] == prefabId)
			{
				gameObject = (GameObject)UnityEngine.Object.Instantiate(this.prefabs[j]);
				gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
				this.listID.Add(prefabId);
				this.list.Add(gameObject);
				transform = gameObject.transform;
				transform.position = position;
				transform.rotation = rotation;
				return gameObject;
			}
		}
		Debug.LogWarning("No Photon Pool: " + prefabId);
		gameObject = (GameObject)Resources.Load(prefabId, typeof(GameObject));
		this.listID.Add(prefabId);
		this.list.Add(gameObject);
		transform = gameObject.transform;
		transform.position = position;
		transform.rotation = rotation;
		return gameObject;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x0010BC10 File Offset: 0x00109E10
	public void Destroy(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		go.SetActive(false);
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i] == go)
			{
				this.list.RemoveAt(i);
				string item = this.listID[i];
				this.listID.RemoveAt(i);
				this.pool.Add(go);
				this.poolID.Add(item);
				return;
			}
		}
		this.Destroy(go);
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x00020192 File Offset: 0x0001E392
	public static void ClearAll()
	{
		PhotonPoolManager.instance.listID.Clear();
		PhotonPoolManager.instance.list.Clear();
		PhotonPoolManager.instance.poolID.Clear();
		PhotonPoolManager.instance.pool.Clear();
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000201D0 File Offset: 0x0001E3D0
	private void OnLevelWasLoaded(int level)
	{
		PhotonPoolManager.ClearAll();
	}

	// Token: 0x04001D9C RID: 7580
	public string[] prefabID;

	// Token: 0x04001D9D RID: 7581
	public GameObject[] prefabs;

	// Token: 0x04001D9E RID: 7582
	private List<string> listID = new List<string>();

	// Token: 0x04001D9F RID: 7583
	private List<GameObject> list = new List<GameObject>();

	// Token: 0x04001DA0 RID: 7584
	private List<string> poolID = new List<string>();

	// Token: 0x04001DA1 RID: 7585
	public List<GameObject> pool = new List<GameObject>();

	// Token: 0x04001DA2 RID: 7586
	private static PhotonPoolManager instance;
}
