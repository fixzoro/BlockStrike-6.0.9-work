using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeFreeFire : MonoBehaviour
{
    public List<ShootingRangeTarget> Targets;

    private List<ShootingRangeTarget> ActivateTarget = new List<ShootingRangeTarget>();

    private List<ShootingRangeTarget> DeactiveTarget = new List<ShootingRangeTarget>();

    public int MaxTargets = 5;

    private bool Activated;

    private void Start()
	{
		for (int i = 0; i < this.Targets.Count; i++)
		{
			this.DeactiveTarget.Add(this.Targets[i]);
		}
		this.SetActive(true);
	}

	private void SetActive(bool active)
	{
		if (this.Activated == active)
		{
			return;
		}
		this.Activated = !this.Activated;
		if (this.Activated)
		{
			for (int i = 0; i < this.MaxTargets; i++)
			{
				ShootingRangeTarget shootingRangeTarget = this.DeactiveTarget[UnityEngine.Random.Range(0, this.DeactiveTarget.Count)];
				shootingRangeTarget.SetActive(true);
				this.DeactiveTarget.Remove(shootingRangeTarget);
				this.ActivateTarget.Add(shootingRangeTarget);
			}
		}
		else
		{
			this.ActivateTarget.Clear();
			this.DeactiveTarget.Clear();
			for (int j = 0; j < this.Targets.Count; j++)
			{
				this.Targets[j].SetActive(false);
				this.DeactiveTarget.Add(this.Targets[j]);
			}
		}
	}

	public void DeadTarget(ShootingRangeTarget target)
	{
		ShootingRangeTarget shootingRangeTarget = this.DeactiveTarget[UnityEngine.Random.Range(0, this.DeactiveTarget.Count)];
		shootingRangeTarget.SetActive(true);
		this.DeactiveTarget.Remove(shootingRangeTarget);
		this.ActivateTarget.Add(shootingRangeTarget);
		if (this.ActivateTarget.Contains(target))
		{
			this.ActivateTarget.Remove(target);
			this.DeactiveTarget.Add(target);
		}
	}
}
