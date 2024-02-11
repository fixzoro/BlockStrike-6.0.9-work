using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;

public class FPWeaponShooter : MonoBehaviour
{
    [Header("Data")]
    public FPWeaponShooter.DataClass Data;

    [Header("Motion Settings")]
    public FPWeaponShooter.MotionClass Motion;

    [Header("Fire Reload Settings")]
    public FPWeaponShooter.FireReloadSettings FireReload;

    [Header("Reload Settings")]
    public FPWeaponShooter.ReloadWeaponClass ReloadWeapon;

    [Header("Knife Settings")]
    public FPWeaponShooter.KnifeSettings Knife;

    [Header("Double Weapon Settings")]
    public FPWeaponShooter.DoubleWeaponClass DoubleWeapon;

    [Header("Inspect Weapon Settings")]
    public FPWeaponShooter.InspectWeaponSettings InspectWeapon;

    [Header("FireStat Settings")]
    public FPWeaponShooter.FireStatSettings FireStat;

    [Header("Stickers Settings")]
    public MeshAtlas[] Stickers;

    [Header("Shell Settings")]
    public FPWeaponShooter.ShellSettings Shell;

    [Header("Bullet Settings")]
    public FPWeaponShooter.BulletPrefab Bullet;

    [Header("Muzzle Settings")]
    public FPWeaponShooter.MuzzleSettings Muzzle;

    [Header("Others")]
    public vp_FPWeapon FPWeapon;

    public CryptoBool SparkEffect = true;

    public MeshAtlas[] WeaponAtlas;

    public MeshAtlas[] HandsAtlas;

    public nTimer Timer;

    private TimerData ReloadInvokeData;

    private TimerData InspectWeaponInvokeData;

    private TimerData MuzzleInvokeData;

    private TimerData FireReloadInvokeData;

    private TimerData FireReloadInvokeData2;

    private void OnDisable()
	{
		this.StopInspectWeapon();
	}

	public void Active()
	{
		if (!this.Knife.Enabled && this.SparkEffect)
		{
			SparkEffectManager.SetParent(this.Muzzle.transform.parent, this.Muzzle.transform.localPosition);
		}
		this.FPWeapon.Activate();
		this.FPWeapon.Wield(true);
		if (this.DoubleWeapon.Enabled)
		{
			this.DoubleWeapon.RightWeapon.Wield();
			this.DoubleWeapon.LeftWeapon.Wield();
		}
		if (!this.Knife.Enabled)
		{
			if (this.DoubleWeapon.Enabled)
			{
				this.DoubleWeapon.LeftMuzzle.meshRenderer.enabled = false;
				this.DoubleWeapon.RightMuzzle.meshRenderer.enabled = false;
			}
			else if (this.Muzzle.enabled)
			{
				this.Muzzle.meshRenderer.enabled = false;
			}
		}
	}

	public void Deactive()
	{
		if (!this.Knife.Enabled && this.SparkEffect)
		{
			SparkEffectManager.ClearParent();
		}
		this.FPWeapon.Deactivate();
	}

	public void Reload()
	{
		this.Reload(WeaponManager.GetWeaponData(this.Data.weapon).ReloadTime);
	}

	public void Reload(float delay)
	{
		if (this.Knife.Enabled)
		{
			return;
		}
		this.FPWeapon.PositionOffset = this.ReloadWeapon.ReloadPosition;
		this.FPWeapon.RotationOffset = this.ReloadWeapon.ReloadRotation;
		this.FPWeapon.Refresh();
		if (!this.Timer.Contains("Reload"))
		{
			this.Timer.Create("Reload", delay, delegate()
			{
				this.FPWeapon.PositionOffset = this.FPWeapon.DefaultPosition;
				this.FPWeapon.RotationOffset = this.FPWeapon.DefaultRotation;
				this.FPWeapon.Refresh();
			});
		}
		this.Timer.In("Reload", delay);
	}

	public void StartInspectWeapon()
	{
		if (this.InspectWeapon.Active)
		{
			return;
		}
		if (this.InspectWeapon.List.Length == nValue.int0)
		{
			return;
		}
		this.InspectWeapon.Active = true;
		base.StartCoroutine(this.InspectWeaponCoroutine());
	}

	private IEnumerator InspectWeaponCoroutine()
	{
		if (this.InspectWeapon.HidePrefab != null)
		{
			if (this.InspectWeapon.DefaultPosition == (float)nValue.int0)
			{
				this.InspectWeapon.DefaultPosition = this.InspectWeapon.HidePrefab.localPosition.y;
			}
			this.InspectWeapon.Tween = this.InspectWeapon.HidePrefab.DOLocalMoveY((float)(-(float)nValue.int1), nValue.float05, false);
			if (!this.Timer.Contains("InspectWeapon"))
			{
				this.Timer.Create("InspectWeapon", nValue.float05, delegate()
				{
					if (this.InspectWeapon.Active)
					{
						this.InspectWeapon.HidePrefab.gameObject.SetActive(false);
					}
				});
			}
			this.Timer.In("InspectWeapon");
		}
		for (int i = 0; i < this.InspectWeapon.List.Length; i++)
		{
			if (this.InspectWeapon.Active)
			{
				this.FPWeapon.StopSprings();
				if (this.DoubleWeapon.Enabled)
				{
					this.DoubleWeapon.RightWeapon.StopSprings();
					this.DoubleWeapon.LeftWeapon.StopSprings();
				}
				this.FPWeapon.AddSoftForce(this.InspectWeapon.List[i].Position, this.InspectWeapon.List[i].Rotation, (int)(this.InspectWeapon.List[i].Duration * (float)nValue.int60));
				yield return new WaitForSeconds(this.InspectWeapon.List[i].Duration);
			}
		}
		if (this.InspectWeapon.HidePrefab != null)
		{
			if (this.InspectWeapon.Active)
			{
				yield return new WaitForSeconds(this.InspectWeapon.List[this.InspectWeapon.List.Length - nValue.int1].Duration * nValue.float015 + nValue.float02);
				if (this.InspectWeapon.Active)
				{
					this.InspectWeapon.HidePrefab.gameObject.SetActive(true);
					this.InspectWeapon.Tween = this.InspectWeapon.HidePrefab.DOLocalMoveY(this.InspectWeapon.DefaultPosition, nValue.float02, false);
					yield return new WaitForSeconds(nValue.float02);
					if (this.InspectWeapon.Active)
					{
						this.InspectWeapon.Active = false;
					}
				}
			}
		}
		else
		{
			this.InspectWeapon.Active = false;
		}
		yield break;
	}

	public void StopInspectWeapon()
	{
		if (this.InspectWeapon.Active)
		{
			this.InspectWeapon.Active = false;
			if (this.InspectWeapon.HidePrefab != null)
			{
				this.InspectWeapon.HidePrefab.gameObject.SetActive(true);
				if (this.InspectWeapon.Tween != null && this.InspectWeapon.Tween.IsActive())
				{
					this.InspectWeapon.Tween.Kill(false);
				}
				this.InspectWeapon.HidePrefab.localPosition = new Vector3(this.InspectWeapon.HidePrefab.localPosition.x, this.InspectWeapon.DefaultPosition, this.InspectWeapon.HidePrefab.localPosition.z);
			}
			this.FPWeapon.StopSprings();
		}
	}

	public void Fire()
	{
		nProfiler.BeginSample("FPWeaponShooter.Fire");
		if (this.InspectWeapon.Active)
		{
			this.InspectWeapon.Active = false;
			this.FPWeapon.StopSprings();
			if (this.DoubleWeapon.Enabled)
			{
				this.DoubleWeapon.RightWeapon.StopSprings();
				this.DoubleWeapon.LeftWeapon.StopSprings();
			}
			if (this.InspectWeapon.HidePrefab != null)
			{
				this.InspectWeapon.HidePrefab.gameObject.SetActive(true);
				if (this.InspectWeapon.Tween != null && this.InspectWeapon.Tween.IsActive())
				{
					this.InspectWeapon.Tween.Kill(false);
				}
				this.InspectWeapon.HidePrefab.localPosition = new Vector3(this.InspectWeapon.HidePrefab.localPosition.x, this.InspectWeapon.DefaultPosition, this.InspectWeapon.HidePrefab.localPosition.z);
			}
		}
		if (this.Knife.Enabled)
		{
			this.FireKnife();
		}
		else
		{
			this.FireWeapon();
		}
		nProfiler.EndSample();
	}

	private void FireWeapon()
	{
		nProfiler.BeginSample("FPWeaponShooter.FireWeapon");
		if (this.Muzzle.enabled && UnityEngine.Random.value > nValue.float02)
		{
			FPWeaponShooter.MuzzleSettings muzzleSettings = (!this.DoubleWeapon.Enabled) ? this.Muzzle : ((!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightMuzzle : this.DoubleWeapon.LeftMuzzle);
			muzzleSettings.transform.localEulerAngles = new Vector3(muzzleSettings.transform.localEulerAngles.x, muzzleSettings.transform.localEulerAngles.y, UnityEngine.Random.value * (float)nValue.int360);
			muzzleSettings.meshRenderer.enabled = true;
			if (!this.Timer.Contains("Muzzle"))
			{
				this.Timer.Create("Muzzle", nValue.float002, delegate()
				{
					if (this.DoubleWeapon.Enabled)
					{
						if (!this.DoubleWeapon.Toogle)
						{
							this.DoubleWeapon.LeftMuzzle.meshRenderer.enabled = false;
						}
						else
						{
							this.DoubleWeapon.RightMuzzle.meshRenderer.enabled = false;
						}
					}
					else
					{
						this.Muzzle.meshRenderer.enabled = false;
					}
				});
			}
			this.Timer.In("Muzzle");
		}
		if (this.Shell.Enable && Settings.Shell)
		{
			if (this.Shell.DoubleWeapon)
			{
				this.Shell.Prefabs[(!this.DoubleWeapon.Toogle) ? 0 : 1].Emit(nValue.int1);
			}
			else
			{
				this.Shell.Prefabs[0].Emit(nValue.int1);
			}
		}
		this.FPWeapon.ResetSprings(nValue.float05, nValue.float05, (float)nValue.int1, (float)nValue.int1);
		if (this.Motion.RecoilRotation.z == (float)nValue.int0)
		{
			if (this.DoubleWeapon.Enabled)
			{
				FPWeaponTwoHanded fpweaponTwoHanded = (!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightWeapon : this.DoubleWeapon.LeftWeapon;
				fpweaponTwoHanded.AddForce2(this.Motion.RecoilPosition, this.Motion.RecoilRotation);
				this.DoubleWeapon.Toogle = !this.DoubleWeapon.Toogle;
			}
			else
			{
				this.FPWeapon.AddForce2(this.Motion.RecoilPosition, this.Motion.RecoilRotation);
			}
		}
		else if (this.DoubleWeapon.Enabled)
		{
			FPWeaponTwoHanded fpweaponTwoHanded2 = (!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightWeapon : this.DoubleWeapon.LeftWeapon;
			fpweaponTwoHanded2.AddForce2(this.Motion.RecoilPosition, Vector3.Scale(this.Motion.RecoilRotation, Vector3.one + Vector3.back) + ((UnityEngine.Random.value >= 0.5f) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(this.Motion.RecoilRotation.z * 0.5f, this.Motion.RecoilRotation.z));
			this.DoubleWeapon.Toogle = !this.DoubleWeapon.Toogle;
		}
		else
		{
			this.FPWeapon.AddForce2(this.Motion.RecoilPosition, Vector3.Scale(this.Motion.RecoilRotation, Vector3.one + Vector3.back) + ((UnityEngine.Random.value >= 0.5f) ? Vector3.back : Vector3.forward) * UnityEngine.Random.Range(this.Motion.RecoilRotation.z * 0.5f, this.Motion.RecoilRotation.z));
		}
		if (this.FireReload.Enabled)
		{
			if (!this.Timer.Contains("FireReload"))
			{
				this.Timer.Create("FireReload", this.FireReload.Delay, delegate()
				{
					this.FPWeapon.AddSoftForce(this.FireReload.Position, this.FireReload.Rotation, this.FireReload.Force);
					if (!this.Timer.Contains("FireReload2"))
					{
						this.Timer.Create("FireReload2", this.FireReload.Duration, delegate()
						{
							this.FPWeapon.StopSprings();
							if (this.DoubleWeapon.Enabled)
							{
								this.DoubleWeapon.RightWeapon.StopSprings();
								this.DoubleWeapon.LeftWeapon.StopSprings();
							}
						});
					}
					this.Timer.In("FireReload2");
				});
			}
			this.Timer.In("FireReload");
		}
		nProfiler.EndSample();
	}

	private void FireKnife()
	{
		if (this.Knife.Delay != (float)nValue.int0)
		{
			if (this.DoubleWeapon.Enabled)
			{
				FPWeaponTwoHanded fpweaponTwoHanded = (!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightWeapon : this.DoubleWeapon.LeftWeapon;
				fpweaponTwoHanded.AddSoftForceKnifeDelay();
			}
			else
			{
				this.FPWeapon.AddSoftForce(this.Knife.DelayPosition, this.Knife.DelayRotation, this.Knife.DelayForce);
			}
		}
		if (!this.Timer.Contains("FireKnife"))
		{
			this.Timer.Create("FireKnife", this.Knife.Delay, delegate()
			{
				if (this.DoubleWeapon.Enabled)
				{
					FPWeaponTwoHanded fpweaponTwoHanded2 = (!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightWeapon : this.DoubleWeapon.LeftWeapon;
					fpweaponTwoHanded2.StopSprings();
					fpweaponTwoHanded2.AddSoftForceKnifeAttack();
					if (this.DoubleWeapon.Toogle)
					{
						if (!this.Timer.Contains("FireKnifeLeft"))
						{
							this.Timer.Create("FireKnifeLeft", this.Knife.AttackDuration, delegate()
							{
								this.DoubleWeapon.LeftWeapon.StopSprings();
							});
						}
						this.Timer.In("FireKnifeLeft");
					}
					else
					{
						if (!this.Timer.Contains("FireKnifeRight"))
						{
							this.Timer.Create("FireKnifeRight", this.Knife.AttackDuration, delegate()
							{
								this.DoubleWeapon.RightWeapon.StopSprings();
							});
						}
						this.Timer.In("FireKnifeRight");
					}
					this.DoubleWeapon.Toogle = !this.DoubleWeapon.Toogle;
				}
				else
				{
					this.FPWeapon.StopSprings();
					this.FPWeapon.AddSoftForce(this.Knife.AttackPosition, this.Knife.AttackRotation, this.Knife.AttackForce);
					if (!this.Timer.Contains("FireKnife2"))
					{
						this.Timer.Create("FireKnife2", this.Knife.AttackDuration, delegate()
						{
							this.FPWeapon.StopSprings();
						});
					}
					this.Timer.In("FireKnife2");
					if (this.Bullet.Enabled)
					{
						GameObject gameObject = PhotonNetwork.Instantiate(this.Bullet.Prefab, PlayerInput.instance.FPCamera.Transform.position + this.Bullet.Position, Quaternion.identity, 0);
						Rigidbody component = gameObject.GetComponent<Rigidbody>();
						component.AddForce(PlayerInput.instance.FPCamera.Transform.forward * this.Bullet.Force);
					}
				}
			});
		}
		this.Timer.In("FireKnife");
	}

	public void DryFire()
	{
		if (this.Knife.Enabled)
		{
			return;
		}
		if (this.DoubleWeapon.Enabled)
		{
			FPWeaponTwoHanded fpweaponTwoHanded = (!this.DoubleWeapon.Toogle) ? this.DoubleWeapon.RightWeapon : this.DoubleWeapon.LeftWeapon;
			fpweaponTwoHanded.AddForce2(this.Motion.RecoilPosition * -nValue.float01, this.Motion.RecoilRotation * -nValue.float01);
			this.DoubleWeapon.Toogle = !this.DoubleWeapon.Toogle;
		}
		else
		{
			this.FPWeapon.AddForce2(this.Motion.RecoilPosition * -nValue.float01, this.Motion.RecoilRotation * -nValue.float01);
		}
	}

	public void ScopeRifle()
	{
		if (this.InspectWeapon.Active)
		{
			this.FPWeapon.StopSprings();
			this.StopInspectWeapon();
		}
	}

	public void UpdateWeaponData(PlayerWeapons.PlayerWeaponData weaponData)
	{
		int[] array = new int[weaponData.Stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = weaponData.Stickers[i];
		}
		this.UpdateWeaponData(weaponData.ID, weaponData.Skin, array, weaponData.FireStat);
	}

	public void UpdateWeaponData(int weapon, int skin, int[] stickers, int firestat)
	{
		this.UpdateHandAtlas();
		this.UpdateWeaponAtlas(weapon, skin);
		this.UpdateStickers(stickers);
		if (!this.Knife.Enabled && firestat >= 0)
		{
			this.FireStat.Enabled = true;
			this.FireStat.Target.SetActive(true);
			this.UpdateFireStat(firestat);
		}
		else if (this.FireStat.Enabled)
		{
			this.FireStat.Enabled = false;
			this.FireStat.Target.SetActive(false);
		}
	}

	public void UpdateHandAtlas()
	{
		this.UpdateHandAtlas(PhotonNetwork.player.GetTeam(), AccountManager.GetPlayerSkinSelected(BodyParts.Body));
	}

	public void UpdateHandAtlas(Team team, int id)
	{
		this.Data.team = team;
		this.Data.handSkin = id;
		UIAtlas atlas = (team != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue;
		string spriteName = "1-" + id;
		for (int i = 0; i < this.HandsAtlas.Length; i++)
		{
			this.HandsAtlas[i].atlas = atlas;
			this.HandsAtlas[i].spriteName = spriteName;
		}
	}

	public void UpdateWeaponAtlas(int weaponID, int weaponSkin)
	{
		this.Data.weapon = weaponID;
		this.Data.skin = weaponSkin;
		for (int i = 0; i < this.WeaponAtlas.Length; i++)
		{
			this.WeaponAtlas[i].spriteName = weaponID + "-" + weaponSkin;
		}
	}

	public void UpdateFireStat(int counter)
	{
		if (this.FireStat.Enabled)
		{
			this.Data.firestat = counter;
			counter = Mathf.Min(counter, 999999);
			string text = counter.ToString("D6");
			for (int i = 0; i < text.Length; i++)
			{
				this.FireStat.Counters[i].spriteName = "f" + text[i];
			}
		}
	}

	private void UpdateStickers(int[] stickers)
	{
		for (int i = 0; i < this.Stickers.Length; i++)
		{
			this.Stickers[i].cachedGameObject.SetActive(false);
		}
		for (int j = 0; j < stickers.Length; j++)
		{
			if (stickers[j] != -1)
			{
				this.Stickers[j].cachedGameObject.SetActive(true);
				this.Stickers[j].spriteName = stickers[j].ToString();
			}
		}
	}

	private void UpdateCustomSkin(object value)
	{
		if (!PhotonNetwork.offlineMode)
		{
			return;
		}
		string str = string.Empty;
		if (Application.isEditor)
		{
			str = Directory.GetParent(Application.dataPath).FullName + "/Others/Custom Weapon Skins";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/files/Custom Weapon Skins"))
				{
					Directory.CreateDirectory(text + "/files/Custom Weapon Skins");
				}
				str = text + "/files/Custom Weapon Skins";
			}
			else
			{
				str = Application.dataPath;
			}
		}
		string path = str + "/" + (string)value;
		if (!File.Exists(path))
		{
			MonoBehaviour.print("No found file");
			return;
		}
		byte[] data = File.ReadAllBytes(path);
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		texture2D.filterMode = FilterMode.Point;
		if (texture2D.width > 128 || texture2D.height > 128)
		{
			MonoBehaviour.print("Maximum texture size 128x128");
			return;
		}
		Material material = new Material(Shader.Find("MADFINGER/Diffuse/Simple"));
		material.mainTexture = texture2D;
		for (int i = 0; i < this.WeaponAtlas.Length; i++)
		{
			this.WeaponAtlas[i].meshFilter.sharedMesh = this.WeaponAtlas[i].originalMesh;
			this.WeaponAtlas[i].meshRenderer.material = material;
			UnityEngine.Object.Destroy(this.WeaponAtlas[i]);
		}
		this.WeaponAtlas = new MeshAtlas[0];
	}

	[Serializable]
	public class DataClass
	{
		public int weapon;

		public int skin;

		public int[] stickers;

		public int firestat;

		public Team team;

		public int handSkin;
	}

	[Serializable]
	public class MotionClass
	{
		public CryptoVector3 RecoilPosition = new Vector3(0f, 0f, -0.035f);

		public CryptoVector3 RecoilRotation = new Vector3(-10f, 0f, 0f);
	}

	[Serializable]
	public class FireReloadSettings
	{
		public CryptoBool Enabled;

		public CryptoFloat Delay = 0.2f;

		public CryptoFloat Duration = 0.5f;

		public CryptoInt Force = 60;

		public CryptoVector3 Position;

		public CryptoVector3 Rotation;
	}

	[Serializable]
	public class KnifeSettings
	{
		public CryptoBool Enabled;

		public CryptoFloat Delay = 0.1f;

		public CryptoInt DelayForce = 50;

		public CryptoVector3 DelayPosition;

		public CryptoVector3 DelayRotation;

		public CryptoFloat AttackDuration = 0.2f;

		public CryptoInt AttackForce = 50;

		public CryptoVector3 AttackPosition;

		public CryptoVector3 AttackRotation;
	}

	[Serializable]
	public class FireStatSettings
	{
		[Disabled]
		public bool Enabled;

		public GameObject Target;

		public MeshAtlas[] Counters;
	}

	[Serializable]
	public class InspectWeaponSettings
	{
		[Disabled]
		public bool Active;

		public Transform HidePrefab;

		public FPWeaponShooter.InspectWeaponSettings.InspectWeaponList[] List;

		[Disabled]
		public float DefaultPosition;

		public Tweener Tween;

		[Serializable]
		public class InspectWeaponList
		{

			public float Duration = 0.5f;

			public Vector3 Position;

			public Vector3 Rotation;
		}
	}

	[Serializable]
	public class ShellSettings
	{
		public bool Enable;

		public bool DoubleWeapon;

		public ParticleSystem[] Prefabs;
	}

	[Serializable]
	public class DoubleWeaponClass
	{
		public CryptoBool Enabled;

		public FPWeaponTwoHanded RightWeapon;

		public FPWeaponTwoHanded LeftWeapon;

		public FPWeaponShooter.MuzzleSettings RightMuzzle;

		public FPWeaponShooter.MuzzleSettings LeftMuzzle;

		[Disabled]
		public bool Toogle;
	}

	[Serializable]
	public class ReloadWeaponClass
	{
		public CryptoVector3 ReloadPosition;

		public CryptoVector3 ReloadRotation;
	}

	[Serializable]
	public class BulletPrefab
	{
		public bool Enabled;

		public string Prefab;

		public Vector3 Position;

		public float Force = 150f;
	}

	[Serializable]
	public class MuzzleSettings
	{
		public bool enabled = true;

		public Transform transform;

		public MeshRenderer meshRenderer;
	}
}
