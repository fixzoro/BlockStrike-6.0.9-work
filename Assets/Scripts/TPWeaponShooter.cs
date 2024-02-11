using System;
using UnityEngine;

public class TPWeaponShooter : MonoBehaviour
{
    [Header("Data")]
    public TPWeaponShooter.DataClass Data;

    [Header("Weapon Settings")]
    public MeshAtlas[] WeaponAtlas;

    [Header("Muzzle Settings")]
    public TPWeaponShooter.MuzzleSettings Muzzle;

    [Header("Transform Settings")]
    public Vector3 DefaultPosition;

    public Quaternion DefaultRotation;

    [Header("FireStat Settings")]
    public TPWeaponShooter.FireStatSettings FireStat;

    [Header("Stickers")]
    public MeshAtlas[] Stickers;

    [Header("Double Weapon Settings")]
    public TPWeaponShooter.DoubleWeaponClass DoubleWeapon;

    [Header("Others")]
    public bool Sound = true;

    public nTimer Timer;

    private PlayerSkin PlayerSkin;

    private WeaponSound FireSound;

    private WeaponSound ReloadSound;

    private Transform cachedTransform;

    private GameObject cachedGameObject;

    private TimerData MuzzleInvokeData;

    private TimerData RightMuzzleInvokeData;

    private TimerData LeftMuzzleInvokeData;

    public void Init(int weaponID, int weaponSkin, PlayerSkin playerSkin)
	{
		this.cachedGameObject = base.gameObject;
		this.cachedTransform = base.transform;
		this.cachedTransform.localPosition = this.DefaultPosition;
		this.cachedTransform.localRotation = this.DefaultRotation;
		this.PlayerSkin = playerSkin;
		this.Data.weapon = weaponID;
		this.Data.skin = weaponSkin;
		this.FireSound = WeaponManager.GetWeaponData(this.Data.weapon).FireSound;
		this.ReloadSound = WeaponManager.GetWeaponData(this.Data.weapon).ReloadSound;
		this.UpdateSkin();
		this.UpdateDoubleWeapon();
	}

	public void Active()
	{
		this.cachedGameObject.SetActive(true);
		if (this.DoubleWeapon.enabled)
		{
			this.DoubleWeapon.leftWeapon.SetActive(true);
		}
		if (this.Muzzle.enabled)
		{
			if (this.DoubleWeapon.enabled)
			{
				this.DoubleWeapon.leftMuzzle.meshRenderer.enabled = false;
				this.DoubleWeapon.rightMuzzle.meshRenderer.enabled = false;
			}
			else
			{
				this.Muzzle.meshRenderer.enabled = false;
			}
		}
	}

	public void Deactive()
	{
		this.cachedGameObject.SetActive(false);
		if (this.DoubleWeapon.enabled)
		{
			this.DoubleWeapon.leftWeapon.SetActive(false);
		}
	}

	public void Fire(bool isVisible, DecalInfo decalInfo)
	{
		nProfiler.BeginSample("TPWeaponShooter.Fire");
		if (isVisible)
		{
			if (this.DoubleWeapon.enabled)
			{
				if (this.DoubleWeapon.toogle)
				{
					this.DoubleWeapon.rightMuzzle.meshRenderer.enabled = true;
					if (!this.Timer.Contains("RightMuzzle"))
					{
						this.Timer.Create("RightMuzzle", 0.05f, delegate()
						{
							this.DoubleWeapon.rightMuzzle.meshRenderer.enabled = false;
						});
					}
					this.Timer.In("RightMuzzle");
				}
				else
				{
					this.DoubleWeapon.leftMuzzle.meshRenderer.enabled = true;
					if (!this.Timer.Contains("LeftMuzzle"))
					{
						this.Timer.Create("LeftMuzzle", 0.05f, delegate()
						{
							this.DoubleWeapon.leftMuzzle.meshRenderer.enabled = false;
						});
					}
					this.Timer.In("LeftMuzzle");
				}
			}
			else if (this.Muzzle.enabled)
			{
				this.Muzzle.meshRenderer.enabled = true;
				if (!this.Timer.Contains("Muzzle"))
				{
					this.Timer.Create("Muzzle", 0.05f, delegate()
					{
						this.Muzzle.meshRenderer.enabled = false;
					});
				}
				this.Timer.In("Muzzle");
			}
		}
		if (this.DoubleWeapon.enabled)
		{
			this.DoubleWeapon.toogle = !this.DoubleWeapon.toogle;
		}
		if (this.Sound)
		{
			this.PlayerSkin.Sounds.Play(this.FireSound);
		}
		nProfiler.EndSample();
	}

	public void Reload()
	{
		if (this.Sound)
		{
			this.PlayerSkin.Sounds.Play(this.ReloadSound, 0.2f);
		}
	}

	public void UpdateSkin()
	{
		string text = this.Data.weapon + "-" + this.Data.skin;
		for (int i = 0; i < this.WeaponAtlas.Length; i++)
		{
			if (this.WeaponAtlas[i].spriteName != text)
			{
				MeshAtlas meshAtlas = this.WeaponAtlas[i];
				meshAtlas.spriteName = text;
			}
		}
	}

	public void SetFireStat(int firestat)
	{
		if (!Settings.ShowFirestat)
		{
			return;
		}
		if (firestat < -1)
		{
			return;
		}
		this.FireStat.enabled = true;
		this.FireStat.target.SetActive(true);
		this.UpdateFireStat(firestat);
	}

	public void UpdateFireStat1()
	{
		if (this.FireStat.enabled)
		{
			this.UpdateFireStat(this.FireStat.value + 1);
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (this.FireStat.enabled && this.FireStat.value != counter)
		{
			this.FireStat.value = counter;
			string text = counter.ToString("D6");
			for (int i = 0; i < this.FireStat.counters.Length; i++)
			{
				this.FireStat.counters[i].spriteName = "f" + text[i];
			}
		}
	}

	public void SetParent(Transform p1, Transform p2)
	{
		if (this.cachedTransform.parent != p1)
		{
			this.cachedTransform.SetParent(p1);
			this.cachedTransform.localPosition = this.DefaultPosition;
			this.cachedTransform.localRotation = this.DefaultRotation;
		}
		if (this.DoubleWeapon.enabled && this.DoubleWeapon.leftWeapon.transform.parent != p2)
		{
			this.DoubleWeapon.leftWeapon.transform.SetParent(p2);
			this.DoubleWeapon.leftWeapon.transform.localPosition = this.DoubleWeapon.leftWeaponPosition;
			this.DoubleWeapon.leftWeapon.transform.localEulerAngles = this.DoubleWeapon.leftWeaponRotation;
		}
	}

	public void UpdateDoubleWeapon()
	{
		if (this.DoubleWeapon.enabled)
		{
			this.DoubleWeapon.leftWeapon.transform.SetParent(this.PlayerSkin.PlayerTwoWeaponRoot);
			this.DoubleWeapon.leftWeapon.transform.localPosition = this.DoubleWeapon.leftWeaponPosition;
			this.DoubleWeapon.leftWeapon.transform.localEulerAngles = this.DoubleWeapon.leftWeaponRotation;
		}
	}

	public void SetStickers(byte[] stickers)
	{
		if (!Settings.ShowStickers)
		{
			return;
		}
		if (stickers == null || stickers.Length == 0)
		{
			for (int i = 0; i < this.Stickers.Length; i++)
			{
				this.Stickers[i].cachedGameObject.SetActive(false);
			}
			return;
		}
		if (this.CheckStickers(this.Data.stickers, stickers))
		{
			for (int j = 0; j < this.Stickers.Length; j++)
			{
				this.Stickers[j].cachedGameObject.SetActive(false);
			}
		}
		int num = 0;
		for (int k = 0; k < stickers.Length / 2; k++)
		{
			this.Stickers[(int)(stickers[num] - 1)].cachedGameObject.SetActive(true);
			this.Stickers[(int)(stickers[num] - 1)].spriteName = stickers[num + 1].ToString();
			num += 2;
		}
		this.Data.stickers = stickers;
	}

	private bool CheckStickers(byte[] a1, byte[] a2)
	{
		if (a1 == null || a2 == null)
		{
			return false;
		}
		if (a1.Length != a2.Length)
		{
			return false;
		}
		for (int i = 0; i < a1.Length; i++)
		{
			if (a1[i] != a2[i])
			{
				return false;
			}
		}
		return true;
	}

	public int[] GetStickers()
	{
		int[] array = new int[this.Stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (this.Stickers[i].cachedGameObject.activeSelf)
			{
				try
				{
					array[i] = int.Parse(this.Stickers[i].spriteName);
				}
				catch
				{
					array[i] = -1;
				}
			}
			else
			{
				array[i] = -1;
			}
		}
		return array;
	}

	public Transform GetCachedTransform()
	{
		if (this.cachedTransform == null)
		{
			this.cachedTransform = base.transform;
		}
		return this.cachedTransform;
	}

	[Serializable]
	public class DataClass
	{
		public int weapon;

		public int skin;

		public byte[] stickers;

		public int firestat;
	}

	[Serializable]
	public class FireStatSettings
	{
		[Disabled]
		public bool enabled;

		public GameObject target;

		public MeshAtlas[] counters;

		public int value;
	}

	[Serializable]
	public class MuzzleSettings
	{
		public bool enabled = true;

		public Transform transform;

		public MeshRenderer meshRenderer;
	}

	[Serializable]
	public class DoubleWeaponClass
	{
		public bool enabled;

		public GameObject leftWeapon;

		public TPWeaponShooter.MuzzleSettings rightMuzzle;

		public TPWeaponShooter.MuzzleSettings leftMuzzle;

		public Vector3 leftWeaponPosition;

		public Vector3 leftWeaponRotation;

		[Disabled]
		public bool toogle;
	}
}
