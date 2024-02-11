using System;
using UnityEngine;

public class ZombieEscapeManager : MonoBehaviour
{
    public ZombieEscapeManager.DataClass[] doors;

    public ZombieEscapeManager.SpawnClass[] spawn;

    public ZombieEscapeManager.SpawnClass defaultSpawn;

    public Color gizmosColor;

    public Vector3 gizmosSize;

    private Transform zombieSpawn;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		this.zombieSpawn = SpawnManager.GetTeamSpawn(Team.Red).cachedTransform;
		base.InvokeRepeating("UpdateData", nValue.float01, (float)nValue.int1);
	}

	private void UpdateData()
	{
		if (UIScore.timeData.active)
		{
			for (int i = nValue.int0; i < this.doors.Length; i++)
			{
				if (!this.doors[i].active && this.doors[i].time >= UIScore.timeData.endTime - Time.time)
				{
					this.doors[i].target.gameObject.SetActive(false);
					this.doors[i].active = true;
				}
			}
			for (int j = nValue.int0; j < this.spawn.Length; j++)
			{
				if (!this.spawn[j].active && this.spawn[j].time >= UIScore.timeData.endTime - Time.time)
				{
					this.spawn[j].active = true;
					this.zombieSpawn.localPosition = this.spawn[j].pos;
					this.zombieSpawn.localEulerAngles = this.spawn[j].rot;
				}
			}
		}
	}

	private void StartRound()
	{
		for (int i = nValue.int0; i < this.doors.Length; i++)
		{
			this.doors[i].target.gameObject.SetActive(true);
			this.doors[i].active = false;
		}
		for (int j = nValue.int0; j < this.spawn.Length; j++)
		{
			this.spawn[j].active = false;
		}
		this.zombieSpawn.localPosition = this.defaultSpawn.pos;
		this.zombieSpawn.localEulerAngles = this.defaultSpawn.rot;
	}

	[Serializable]
	public class DataClass
	{
		public Transform target;

		public float time;

		public bool active;
	}

	[Serializable]
	public class SpawnClass
	{
		public Vector3 pos;

		public Vector3 rot;

		public float time;

		public bool active;
	}
}
