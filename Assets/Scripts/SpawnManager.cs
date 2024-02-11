using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class SpawnManager : MonoBehaviour
{
    public SpawnPoint blue;

    public SpawnPoint red;

    public SpawnPoint[] random;

    public SpawnPoint cameraStatic;

    private static SpawnManager instance;

    private void Awake()
	{
		SpawnManager.instance = this;
	}

    #if UNITY_EDITOR
    private void Update()
    {
        red.team = Team.Red;
        blue.team = Team.Blue;
    }
    #endif

    public static SpawnPoint GetTeamSpawn()
	{
		return SpawnManager.GetTeamSpawn(GameManager.team);
	}

	public static SpawnPoint GetCameraStatic()
	{
		return SpawnManager.instance.cameraStatic;
	}

	public static SpawnPoint GetTeamSpawn(Team team)
	{
		if (team == Team.Blue)
		{
			return SpawnManager.instance.blue;
		}
		if (team == Team.Red)
		{
			return SpawnManager.instance.red;
		}
		return null;
	}

	public static SpawnPoint GetSpawn(int index)
	{
		return SpawnManager.instance.random[index];
	}

	public static SpawnPoint GetRandomSpawn()
	{
		return SpawnManager.instance.random[UnityEngine.Random.Range(0, SpawnManager.instance.random.Length)];
	}

	public static SpawnPoint GetPlayerIDSpawn()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			list.Add(playerList[i]);
		}
		list.Sort(new Comparison<PhotonPlayer>(SpawnManager.SortPlayerID));
		int id = PhotonNetwork.player.ID;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].ID == id)
			{
				return SpawnManager.instance.random[j];
			}
		}
		return SpawnManager.instance.random[0];
	}

	private static int SortPlayerID(PhotonPlayer a, PhotonPlayer b)
	{
		return a.ID.CompareTo(b.ID);
	}

	public static void SetCustomMapData(JsonObject json)
	{
		JsonObject jsonObject = json.Get<JsonObject>("BlueSpawn");
		SpawnManager.instance.blue.cachedTransform.position = jsonObject.Get<Vector3>("position");
		SpawnManager.instance.blue.cachedTransform.rotation = jsonObject.Get<Quaternion>("rotation");
		SpawnManager.instance.blue.cachedTransform.localScale = jsonObject.Get<Vector3>("localScale");
		jsonObject = json.Get<JsonObject>("RedSpawn");
		SpawnManager.instance.red.cachedTransform.position = jsonObject.Get<Vector3>("position");
		SpawnManager.instance.red.cachedTransform.rotation = jsonObject.Get<Quaternion>("rotation");
		SpawnManager.instance.red.cachedTransform.localScale = jsonObject.Get<Vector3>("localScale");
	}
}
