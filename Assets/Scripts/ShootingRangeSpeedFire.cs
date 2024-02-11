using System;
using DG.Tweening;
using UnityEngine;

public class ShootingRangeSpeedFire : MonoBehaviour
{
    public ShootingRangeSpeedFire.RoomData[] Data;

    public int dead;

    public int index;

    public bool isStarting;

    public void StartRoom()
	{
		if (this.isStarting)
		{
			return;
		}
		this.index = 0;
		this.isStarting = true;
		this.Next();
	}

	public void Dead()
	{
		this.dead++;
		if (this.Data[this.index].Targets.Length == this.dead)
		{
			this.dead = 0;
			this.index++;
			this.Next();
		}
	}

	public void Next()
	{
		MonoBehaviour.print("Next");
		this.Data[this.index].Gate.DOLocalMoveY(-5f, 0.25f, false);
		for (int i = 0; i < this.Data[this.index].Targets.Length; i++)
		{
			this.Data[this.index].Targets[i].SetActive(true);
		}
	}

	[Serializable]
	public class RoomData
	{
		public Transform Gate;

		public ShootingRangeTarget[] Targets;
	}
}
