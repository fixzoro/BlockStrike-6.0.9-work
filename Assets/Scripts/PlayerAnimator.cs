using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public WeaponType selectWeapon;

    public PlayerAnimator.WeaponAnimData rifle;

    public PlayerAnimator.WeaponAnimData pistol;

    public PlayerAnimator.WeaponAnimData knife;

    public PlayerAnimator.LegsAnimData legs;

    public Transform root;

    private float cachedRotate;

    private bool cachedGrounded;

    private float cachedMove;

    private bool cachedReload;

    private Vector3 rootPosition = Vector3.zero;

    private bool cacheVisible;

    public float rotate
	{
		get
		{
			return this.cachedRotate;
		}
		set
		{
			if (this.cachedRotate != value)
			{
				this.cachedRotate = value;
				if (!this.cachedReload)
				{
					this.GetSelectWeapon().SetRotate(value, this.visible);
				}
			}
		}
	}

	public bool grounded
	{
		get
		{
			return this.cachedGrounded;
		}
		set
		{
			if (this.cachedGrounded != value)
			{
				this.cachedGrounded = value;
				this.legs.SetGrounded(value);
			}
		}
	}

	public bool reload
	{
		get
		{
			return this.cachedReload;
		}
		set
		{
			if (this.selectWeapon == WeaponType.Knife)
			{
				return;
			}
			if (value)
			{
				if (this.cachedReload)
				{
					this.GetSelectWeapon().StopReload();
				}
				this.cachedReload = value;
				this.GetSelectWeapon().StartReload();
			}
			else
			{
				this.cachedReload = value;
				this.GetSelectWeapon().StopReload();
			}
		}
	}

	public float move
	{
		get
		{
			return this.cachedMove;
		}
		set
		{
			if (value == 0f)
			{
				if (this.cachedMove != 0f && this.cachedGrounded)
				{
					this.legs.UpdateMove(value, this.visible);
					this.cachedMove = value;
				}
			}
			else
			{
				this.cachedMove = value;
			}
		}
	}

	public bool visible
	{
		get
		{
			return this.cacheVisible;
		}
		set
		{
			this.cacheVisible = value;
			if (!this.cachedReload)
			{
				this.GetSelectWeapon().SetRotate(this.cachedRotate, value);
				if (this.grounded)
				{
					this.legs.UpdateMove(this.cachedMove, value);
				}
			}
		}
	}

	public Vector3 rootPos
	{
		get
		{
			return this.rootPosition;
		}
		set
		{
			this.rootPosition = value;
			this.root.localPosition = this.rootPosition;
		}
	}

	private void OnEnable()
	{
		this.rifle.active = true;
		this.pistol.active = true;
		this.knife.active = true;
		this.legs.active = true;
	}

	private void OnDisable()
	{
		this.rifle.active = false;
		this.pistol.active = false;
		this.knife.active = false;
		this.legs.active = false;
	}

	public void OnDefault()
	{
		this.selectWeapon = WeaponType.Knife;
		this.rifle.reloadIndex = 0;
		this.rifle.reloading = false;
		this.rifle.active = false;
		this.rifle.lastRotate = 0f;
		this.pistol.reloadIndex = 0;
		this.pistol.reloading = false;
		this.pistol.active = false;
		this.pistol.lastRotate = 0f;
		this.knife.reloadIndex = 0;
		this.knife.reloading = false;
		this.knife.active = false;
		this.knife.lastRotate = 0f;
		this.legs.moveIndex = 0;
		this.legs.active = false;
	}

	private void LateUpdate()
	{
		if (this.cachedMove != 0f && this.cachedGrounded)
		{
			this.legs.UpdateMove(this.cachedMove, this.visible);
		}
		if (this.cachedReload)
		{
			this.GetSelectWeapon().UpdateReload(this.visible);
			if (!this.GetSelectWeapon().reloading)
			{
				this.GetSelectWeapon().StopReload();
				this.GetSelectWeapon().SetRotate(this.cachedRotate, this.visible);
				this.cachedReload = false;
			}
		}
	}

	private PlayerAnimator.WeaponAnimData GetSelectWeapon()
	{
		switch (this.selectWeapon)
		{
		case WeaponType.Knife:
			return this.knife;
		case WeaponType.Pistol:
			return this.pistol;
		case WeaponType.Rifle:
			return this.rifle;
		default:
			return this.rifle;
		}
	}

	public void SetWeapon(WeaponType type)
	{
		if (type == this.selectWeapon)
		{
			return;
		}
		if (this.GetSelectWeapon().reloading)
		{
			this.GetSelectWeapon().StopReload();
		}
		this.selectWeapon = type;
		this.reload = false;
		switch (type)
		{
		case WeaponType.Knife:
			this.knife.SetDefault();
			break;
		case WeaponType.Pistol:
			this.pistol.SetDefault();
			break;
		case WeaponType.Rifle:
			this.rifle.SetDefault();
			break;
		}
	}

	public void SetDefault()
	{
		this.root.localPosition = this.rootPos;
		this.root.localEulerAngles = Vector3.zero;
		this.SetWeapon(this.selectWeapon);
		this.cachedRotate = 0f;
		this.cachedReload = false;
		this.cachedMove = 0f;
		this.cachedGrounded = true;
		PlayerAnimator.WeaponAnimData weaponAnimData = this.GetSelectWeapon();
		weaponAnimData.SetDefault();
		weaponAnimData.SetRotate(this.rotate, this.visible);
		this.legs.SetGrounded(this.grounded);
	}

	[Serializable]
	public class WeaponAnimData
	{
        public PlayerAnimator.Key[] defaultValue;

        public PlayerAnimator.Key[] maxRotateValue;

        public PlayerAnimator.Key[] minRotateValue;

        public PlayerAnimator.Keys[] reloadValue;

        public int reloadIndex;

        public int maxReloadIndex;

        public bool reloading;

        public bool active;

        public float lastRotate;

        private int defaultValueCount;

        private int maxRotateValueCount;

        private int minRotateValueCount;

        private int reloadValueCount;

        public void SetDefault()
		{
			if (this.defaultValueCount == 0)
			{
				this.defaultValueCount = this.defaultValue.Length;
			}
			for (int i = 0; i < this.defaultValueCount; i++)
			{
				this.defaultValue[i].target.localPosition = this.defaultValue[i].pos;
				this.defaultValue[i].target.localRotation = this.defaultValue[i].rot;
			}
		}

		public void SetRotate(float rotate, bool visible)
		{
			if (!visible)
			{
				return;
			}
			if (this.lastRotate == rotate)
			{
				return;
			}
			this.lastRotate = rotate;
			if (rotate >= 0f)
			{
				if (this.maxRotateValueCount == 0)
				{
					this.maxRotateValueCount = this.maxRotateValue.Length;
				}
				for (int i = 0; i < this.maxRotateValueCount; i++)
				{
					this.maxRotateValue[i].target.localPosition = Vector3.MoveTowards(this.defaultValue[i + 3].pos, this.maxRotateValue[i].pos, rotate);
					this.maxRotateValue[i].target.localRotation = Quaternion.Lerp(this.defaultValue[i + 3].rot, this.maxRotateValue[i].rot, rotate);
				}
			}
			else
			{
				rotate *= -1f;
				if (this.minRotateValueCount == 0)
				{
					this.minRotateValueCount = this.minRotateValue.Length;
				}
				for (int j = 0; j < this.minRotateValueCount; j++)
				{
					this.minRotateValue[j].target.localPosition = Vector3.MoveTowards(this.defaultValue[j + 3].pos, this.minRotateValue[j].pos, rotate);
					this.minRotateValue[j].target.localRotation = Quaternion.Lerp(this.defaultValue[j + 3].rot, this.minRotateValue[j].rot, rotate);
				}
			}
		}

		public void StartReload()
		{
			this.reloadIndex = 0;
			this.reloading = true;
		}

		public void StopReload()
		{
			this.reloadIndex = 0;
			this.reloading = false;
			this.SetDefault();
		}

		public void UpdateReload(bool visible)
		{
			this.reloadIndex++;
			if (this.reloadIndex >= this.maxReloadIndex)
			{
				this.StopReload();
			}
			else
			{
				if (!visible)
				{
					return;
				}
				if (this.reloadValueCount == 0)
				{
					this.reloadValueCount = this.reloadValue.Length;
				}
				for (int i = 0; i < this.reloadValueCount; i++)
				{
					this.reloadValue[i].target.localPosition = this.reloadValue[i].pos[this.reloadIndex];
					this.reloadValue[i].target.localRotation = this.reloadValue[i].rot[this.reloadIndex];
				}
			}
		}
	}

	[Serializable]
	public class LegsAnimData
	{
        public PlayerAnimator.Key[] defaultValue;

        public PlayerAnimator.Key[] groundedValue;

        public PlayerAnimator.Keys[] moveValue;

        public int moveIndex;

        public bool active;

        private int defaultValueCount;

        private int groundedValueCount;

        private int moveValueCount;

        public void SetDefault()
		{
			if (this.defaultValueCount == 0)
			{
				this.defaultValueCount = this.defaultValue.Length;
			}
			for (int i = 0; i < this.defaultValueCount; i++)
			{
				if (this.defaultValue[i].target.localPosition != this.defaultValue[i].pos)
				{
					this.defaultValue[i].target.localPosition = this.defaultValue[i].pos;
				}
				if (this.defaultValue[i].target.localRotation != this.defaultValue[i].rot)
				{
					this.defaultValue[i].target.localRotation = this.defaultValue[i].rot;
				}
			}
		}

		public void SetGrounded(bool grounded)
		{
			if (!grounded)
			{
				if (this.groundedValueCount == 0)
				{
					this.groundedValueCount = this.groundedValue.Length;
				}
				for (int i = 0; i < this.groundedValueCount; i++)
				{
					if (this.defaultValue[i].target.localPosition != this.groundedValue[i].pos)
					{
						this.defaultValue[i].target.localPosition = this.groundedValue[i].pos;
					}
					if (this.defaultValue[i].target.localRotation != this.groundedValue[i].rot)
					{
						this.defaultValue[i].target.localRotation = this.groundedValue[i].rot;
					}
				}
			}
			else
			{
				this.SetDefault();
			}
		}

		public void UpdateMove(float speed, bool visible)
		{
			nProfiler.BeginSample("PlayerAnimator.UpdateMove");
			if (speed >= 0f)
			{
				this.moveIndex++;
				if (this.moveIndex >= this.moveValue[0].pos.Length)
				{
					this.moveIndex = 0;
				}
			}
			else
			{
				this.moveIndex--;
				if (this.moveIndex <= 0)
				{
					this.moveIndex = this.moveValue[0].pos.Length - 1;
				}
				if (speed < 0f)
				{
					speed *= -1f;
				}
			}
			if (speed != 0f && !visible)
			{
				nProfiler.EndSample();
				return;
			}
			if (!this.active)
			{
				nProfiler.EndSample();
				return;
			}
			if (PhotonNetwork.leavingRoom)
			{
				nProfiler.EndSample();
				return;
			}
			if (this.moveValueCount == 0)
			{
				this.moveValueCount = this.moveValue.Length;
			}
			for (int i = 0; i < this.moveValueCount; i++)
			{
				if (visible)
				{
					this.moveValue[i].target.localRotation = Quaternion.Lerp(this.defaultValue[i].rot, this.moveValue[i].rot[this.moveIndex], speed);
				}
				else
				{
					this.moveValue[i].target.localRotation = this.moveValue[i].rot[this.moveIndex];
				}
			}
			nProfiler.EndSample();
		}

		private Vector3 Vector3Lerp(Vector3 from, Vector3 to, float t)
		{
			from.x += (to.x - from.x) * t;
			from.y += (to.y - from.y) * t;
			from.z += (to.z - from.z) * t;
			return from;
		}
	}

	[Serializable]
	public class Key
	{
		public Transform target;

		public Vector3 pos;

		public Quaternion rot;
	}

	[Serializable]
	public class Keys
	{
		public Transform target;

		public Vector3[] pos;

		public Quaternion[] rot;
	}
}
