using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class DropperMode : Photon.MonoBehaviour
{
    public GameObject[] Droppers;

    public GameObject Place;

    public int SelectLevel = -1;

    public int DestroyLevel = -3;

    public List<DropperMode.LevelData> Levels = new List<DropperMode.LevelData>();

    public int MaxLevel = 50;

    public int max;

    public int seed = 4545;

    private System.Random random;

    private Vector3 lastPos;

    private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		UIScore.SetActiveScore(true, nValue.int0);
		GameManager.startDamageTime = nValue.float01;
		UIPanelManager.ShowPanel("Display");
		GameManager.changeWeapons = false;
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			GameManager.player.FPController.MotorAirSpeed = 1f;
			GameManager.player.FPController.MotorBackwardsSpeed = 1f;
			this.OnRevivalPlayer();
			UIScore.UpdateScore(this.MaxLevel, GameManager.blueScore, GameManager.redScore);
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
		this.random = new System.Random(this.seed);
		for (int i = 0; i < this.max; i++)
		{
			this.GenerateLevel(i);
		}
	}

	private void OnSpawnPlayer()
	{
		GameManager.controller.SpawnPlayer(SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position, SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.eulerAngles);
	}

	private void OnRevivalPlayer()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position, SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		if (player.PlayerTeam != Team.Blue)
		{
			GameManager.team = Team.Blue;
		}
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		GameManager.redScore = ++GameManager.redScore;
		this.OnSpawnPlayer();
	}

	public void NextLevel(Transform spawn)
	{
		this.SelectLevel++;
		this.DestroyLevel++;
		this.DeactiveLevel();
		SpawnManager.GetTeamSpawn(Team.Blue).cachedTransform.position = spawn.position;
		spawn.gameObject.SetActive(false);
	}

	private void GenerateLevel(int level)
	{
		GameObject gameObject = PoolManager.Spawn("Place", this.Place, this.lastPos + new Vector3(0f, -10f, 0f), Vector3.zero);
		gameObject.name = "Place";
		this.lastPos = this.GetStartPosition() + gameObject.transform.localPosition;
		DropperMode.LevelData item = default(DropperMode.LevelData);
		item.level = level;
		item.Objects = new List<GameObject>();
		item.Objects.Add(gameObject);
		GameObject gameObject2 = PoolManager.Spawn("Dropper_2", this.Droppers[2], this.lastPos, Vector3.zero);
		gameObject2.name = "Dropper_2";
		item.Objects.Add(gameObject2);
		int num = this.random.Next(3, 5) + level;
		for (int i = 0; i < num; i++)
		{
			this.lastPos += new Vector3((float)this.random.Next(-3, 3), (float)this.random.Next(-40, -30), (float)this.random.Next(-3, 3));
			int num2 = this.random.Next(0, this.Droppers.Length - 1);
			gameObject2 = PoolManager.Spawn("Dropper_" + num2, this.Droppers[num2], this.lastPos, Vector3.zero);
			gameObject2.name = "Dropper_" + num2;
			item.Objects.Add(gameObject2);
		}
		this.Levels.Add(item);
	}

	private void DeactiveLevel()
	{
		if (this.DestroyLevel < 0)
		{
			return;
		}
		for (int i = 0; i < this.Levels.Count; i++)
		{
			if (this.Levels[i].level == this.DestroyLevel)
			{
				for (int j = 0; j < this.Levels[i].Objects.Count; j++)
				{
					PoolManager.Despawn(this.Levels[i].Objects[j].name, this.Levels[i].Objects[j]);
				}
			}
		}
	}

	private Vector3 GetStartPosition()
	{
		Vector3 result = new Vector3(0f, -10f, 0f);
		if (this.random.Next(0, 100) > 50)
		{
			result.x = (float)(this.random.Next(9, 12) * ((this.random.Next(0, 100) <= 50) ? -1 : 1));
			result.z = (float)(this.random.Next(0, 12) * ((this.random.Next(0, 100) <= 50) ? -1 : 1));
		}
		else
		{
			result.x = (float)(this.random.Next(0, 12) * ((this.random.Next(0, 100) <= 50) ? -1 : 1));
			result.z = (float)(this.random.Next(9, 12) * ((this.random.Next(0, 100) <= 50) ? -1 : 1));
		}
		return result;
	}

	[Serializable]
	public struct LevelData
	{
		public int level;

		public List<GameObject> Objects;
	}
}
