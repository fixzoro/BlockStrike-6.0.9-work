using System;
using Photon;
using UnityEngine;

public class ZombieWavesSpawnManager : Photon.MonoBehaviour
{
    public int count = 5;

    public int maxInScene = 2;

    public int alive;

    public int deads;

    public Transform[] spawns;

    public GameObject zombiePrefab;

    private void Start()
	{
		base.photonView.AddMessage("PhotonCreateZombie", new PhotonView.MessageDelegate(this.PhotonCreateZombie));
		TimerManager.In(1f, -1, 1f, new TimerManager.Callback(this.CheckScene));
		PlayerAI.deadEvent += this.DeadZombie;
		PlayerAI.startEvent += this.StartZombie;
	}

	private void CheckScene()
	{
		if (this.count - this.deads > this.alive && PlayerAI.list.Count < this.maxInScene)
		{
			PhotonNetwork.InstantiateSceneObject("Player/PlayerAI", this.spawns[UnityEngine.Random.Range(0, this.spawns.Length)].position, Quaternion.identity, 0, null);
		}
	}

	private void StartZombie(PlayerAI ai)
	{
		ai.dead = false;
		ai.health = 1000;
		this.alive++;
	}

	private void DeadZombie(PlayerAI ai)
	{
		this.deads++;
		this.alive--;
		if (this.count <= this.deads && this.alive == 0)
		{
			UIToast.Show("Pause 10 sec");
			TimerManager.In(10f, delegate()
			{
				this.count += 5;
				this.maxInScene++;
				this.deads = 0;
			});
		}
	}

	private void CreateZombie()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonDataWrite data = base.photonView.GetData();
			data.Write((byte)UnityEngine.Random.Range(0, this.spawns.Length));
			base.photonView.RPC("PhotonCreateZombie", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonCreateZombie(PhotonMessage message)
	{
		byte b = message.ReadByte();
		PoolManager.Spawn("Zombie", this.zombiePrefab, this.spawns[(int)b].position, Quaternion.identity.eulerAngles);
	}
}
