using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ShootingRangeTarget : MonoBehaviour
{
    public ShootingRangeTarget.PositionClass Position;

    public ShootingRangeTarget.RotationClass Rotation;

    public ShootingRangeTarget.RandomPositionClass RandomPosition;

    public int Health = 100;

    public UnityEvent DeadCallback;

    private bool Activated;

    private Transform CacheTransform;

    private void Start()
	{
		this.CacheTransform = base.transform;
	}

	public void SetActive(bool active)
	{
		this.Activated = active;
		this.Health = 100;
		if (this.CacheTransform == null)
		{
			this.CacheTransform = base.transform;
		}
		if (this.Activated)
		{
			if (this.Position.Use)
			{
				this.CacheTransform.position = this.Position.Default;
				if (this.Position.Loop)
				{
					this.CacheTransform.DOMove(this.Position.Activated, this.Position.Duration, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
				}
				else
				{
					this.CacheTransform.DOMove(this.Position.Activated, this.Position.Duration, false);
				}
			}
			if (this.Rotation.Use)
			{
				this.CacheTransform.eulerAngles = this.Rotation.Default;
				this.CacheTransform.DORotate(this.Rotation.Activated, this.Rotation.Duration, RotateMode.Fast);
			}
			if (this.RandomPosition.Use)
			{
				this.CacheTransform.position = this.RandomPosition.Positions[UnityEngine.Random.Range(0, this.RandomPosition.Positions.Length)];
			}
		}
		else
		{
			if (this.Position.Use)
			{
				if (this.Position.Loop)
				{
					this.CacheTransform.DOKill(false);
				}
				else
				{
					this.CacheTransform.DOMove(this.Position.Default, this.Position.Duration, false);
				}
			}
			if (this.Rotation.Use)
			{
				this.CacheTransform.DORotate(this.Rotation.Default, this.Rotation.Duration, RotateMode.Fast);
			}
		}
	}

	public bool GetActive()
	{
		return this.Activated;
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (this.Activated)
		{
			this.Health -= damageInfo.damage;
			this.Health = Mathf.Max(this.Health, 0);
			if (this.Health == 0)
			{
				this.DeadCallback.Invoke();
				this.SetActive(false);
			}
		}
	}

	[Serializable]
	public class PositionClass
	{
		public bool Use;

		public Vector3 Default;

		public Vector3 Activated;

		public float Duration = 1f;

		public bool Loop;

		public Tweener Tween;
	}

	[Serializable]
	public class RotationClass
	{
		public bool Use;

		public Vector3 Default;

		public Vector3 Activated;

		public float Duration = 1f;

		public Tweener Tween;
	}

	[Serializable]
	public class RandomPositionClass
	{
		public bool Use;

		public Vector3[] Positions;
	}
}
