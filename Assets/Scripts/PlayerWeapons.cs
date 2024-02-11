using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public WeaponType SelectedWeapon;

    public bool CanFire = true;

    public bool isDebug;

    [Disabled]
    public bool isFire;

    [Disabled]
    public bool isScope;

    [Disabled]
    public bool isReload;

    [Disabled]
    public bool Wielded;

    private float DropWeaponTime = -1f;

    public bool InfiniteAmmo;

    private bool isDryFire;

    private CryptoVector2 FireAccuracy;

    private Ray FireRay;

    private RaycastHit FireRaycastHit;

    private Transform FireRayTransform;

    private DamageInfo FireDamageInfo = default(DamageInfo);

    [Header("Weapons Data")]
    public PlayerWeapons.PlayerWeaponData KnifeData = new PlayerWeapons.PlayerWeaponData();

    public PlayerWeapons.PlayerWeaponData PistolData = new PlayerWeapons.PlayerWeaponData();

    public PlayerWeapons.PlayerWeaponData RifleData = new PlayerWeapons.PlayerWeaponData();

    private bool isUpdateWeaponData;

    [Header("RigidBody")]
    public bool PushRigidbody;

    public float PushRigidbodyForce = 1000f;

    [Header("Sounds")]
    public PlayerSounds Sounds;

    [Header("Others")]
    public LayerMask FireLayers;

    public Camera PlayerCamera;

    public Camera WeaponCamera;

    public nTimer Timer;

    private Dictionary<string, GameObject> WeaponObjects = new Dictionary<string, GameObject>();

    private void Start()
	{
		EventManager.AddListener<DamageInfo>("KillPlayer", new EventManager.Callback<DamageInfo>(this.KillPlayer));
		this.Timer.In((float)nValue.int2, true, new TimerDelegate(this.UpdateValue));
	}

	[ContextMenu("UpdateValue")]
	private void UpdateValue()
	{
		this.FireAccuracy.UpdateValue();
		this.UpdateValueWeaponData(this.GetSelectedWeaponData());
	}

	private void UpdateValueWeaponData(PlayerWeapons.PlayerWeaponData data)
	{
		data.ID.UpdateValue();
		data.FaceDamage.UpdateValue();
		data.BodyDamage.UpdateValue();
		data.HandDamage.UpdateValue();
		data.LegDamage.UpdateValue();
		data.FireRate.UpdateValue();
		data.Accuracy.UpdateValue();
		data.FireAccuracy.UpdateValue();
		data.FireBullets.UpdateValue();
		data.ReloadTime.UpdateValue();
		data.Distance.UpdateValue();
		data.Mass.UpdateValue();
		data.ScopeSize.UpdateValue();
		data.ScopeSensitivity.UpdateValue();
		data.ScopeRecoil.UpdateValue();
		data.ScopeAccuracy.UpdateValue();
		data.Scope2Size.UpdateValue();
		data.Scope2Sensitivity.UpdateValue();
		data.Ammo.UpdateValue();
		data.AmmoTotal.UpdateValue();
		data.AmmoMax.UpdateValue();
		data.Skin.UpdateValue();
		data.FireStat.UpdateValue();
	}

	private void OnEnable()
	{
		UICrosshair.SetActiveCrosshair(true);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
	}

	private void OnDisable()
	{
		UICrosshair.SetActiveCrosshair(false);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
		this.isReload = false;
		this.isFire = false;
		this.Timer.Cancel("ReloadTime");
		this.Timer.Cancel("WieldedTime");
	}

	private void GetButtonDown(string name)
	{
        switch (name)
        {
            case "Fire":
            case "FireSniper":
                isFire = true;
                break;
            case "Aim":
                ScopeWeapon(check: true);
                break;
            case "Reload":
                ReloadWeapon();
                break;
            case "Pause":
            case "Statistics":
                DeactiveScope();
                break;
            case "SelectWeapon":
                if (DropWeaponManager.enable)
                {
                    DropWeaponTime = 0f;
                }
                else
                {
                    UpdateSelectWeapon();
                }
                break;
        }
    }

	private void GetButtonUp(string name)
	{
        switch (name)
        {
            case "Fire":
            case "FireSniper":
                isFire = false;
                isDryFire = false;
                break;
            case "SelectWeapon":
                if (DropWeaponManager.enable && DropWeaponTime >= 0f)
                {
                    UpdateSelectWeapon();
                    DropWeaponTime = -1f;
                }
                break;
        }
    }

	private void Update()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.mouseScrollDelta.magnitude > 0.9f && GameObject.Find("Display") != null)
        {
            this.UpdateSelectWeapon();
        }
        if ((Input.GetKey(KeyCode.Alpha1) && GameObject.Find("Display") != null))
        {
            SetWeapon(WeaponType.Rifle, true);
        }
        if ((Input.GetKey(KeyCode.Alpha2) && GameObject.Find("Display") != null))
        {
            SetWeapon(WeaponType.Pistol, true);
        }
        if ((Input.GetKey(KeyCode.Alpha3) && GameObject.Find("Display") != null))
        {
            SetWeapon(WeaponType.Knife, true);
        }
        #endif
        if (this.isFire)
		{
			this.FireWeapon();
		}
		if (DropWeaponManager.enable && this.DropWeaponTime >= 0f)
		{
			if (this.DropWeaponTime >= 0.5f)
			{
				this.DropWeapon(true);
				this.DropWeaponTime = -1f;
			}
			this.DropWeaponTime += Time.deltaTime;
		}
	}

	private void UpdateSelectWeapon()
	{
		if (this.SelectedWeapon == WeaponType.Knife)
		{
			if (this.RifleData.Enabled)
			{
				this.SetWeapon(WeaponType.Rifle);
			}
			else if (this.PistolData.Enabled)
			{
				this.SetWeapon(WeaponType.Pistol);
			}
		}
		else if (this.SelectedWeapon == WeaponType.Pistol)
		{
			if (this.KnifeData.Enabled)
			{
				this.SetWeapon(WeaponType.Knife);
			}
			else if (this.RifleData.Enabled)
			{
				this.SetWeapon(WeaponType.Rifle);
			}
		}
		else if (this.SelectedWeapon == WeaponType.Rifle)
		{
			if (this.PistolData.Enabled)
			{
				this.SetWeapon(WeaponType.Pistol);
			}
			else if (this.KnifeData.Enabled)
			{
				this.SetWeapon(WeaponType.Knife);
			}
		}
	}

	public PlayerWeapons.PlayerWeaponData GetSelectedWeaponData()
	{
		return this.GetWeaponData(this.SelectedWeapon);
	}

	public PlayerWeapons.PlayerWeaponData GetWeaponData(WeaponType weapon)
	{
		switch (weapon)
		{
		case WeaponType.Knife:
			return this.KnifeData;
		case WeaponType.Pistol:
			return this.PistolData;
		case WeaponType.Rifle:
			return this.RifleData;
		default:
			return null;
		}
	}

	public PlayerWeapons.PlayerWeaponData GetWeaponData(int weaponID)
	{
		if (this.KnifeData.ID == weaponID)
		{
			return this.KnifeData;
		}
		if (this.PistolData.ID == weaponID)
		{
			return this.PistolData;
		}
		if (this.RifleData.ID == weaponID)
		{
			return this.RifleData;
		}
		return null;
	}

	public void UpdateWeaponAmmoAll()
	{
		this.UpdateWeaponAmmo(WeaponType.Pistol);
		this.UpdateWeaponAmmo(WeaponType.Rifle);
		PlayerWeapons.PlayerWeaponData selectedWeaponData = this.GetSelectedWeaponData();
		UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, this.InfiniteAmmo, selectedWeaponData.ReloadWarning);
	}

	public void UpdateWeaponAmmo(WeaponType type)
	{
		if (type == WeaponType.Knife)
		{
			return;
		}
		PlayerWeapons.PlayerWeaponData weaponData = this.GetWeaponData(type);
		if (weaponData.Enabled)
		{
			WeaponData weaponData2 = WeaponManager.GetWeaponData(weaponData.ID);
			weaponData.Ammo = weaponData2.Ammo;
			weaponData.AmmoTotal = weaponData2.Ammo;
			weaponData.AmmoMax = weaponData2.MaxAmmo;
		}
	}

	public void UpdateWeaponAll()
	{
		this.UpdateWeaponAll(WeaponManager.DefaultWeaponType);
	}

	public void UpdateWeaponAll(WeaponType selectWeapon)
	{
		this.UpdateWeaponAll(selectWeapon, null, null, null);
	}

	public void UpdateWeaponAll(WeaponType selectWeapon, WeaponCustomData dataKnife, WeaponCustomData dataPistol, WeaponCustomData dataRifle)
	{
		this.isUpdateWeaponData = true;
		this.WeaponCamera.cullingMask = 0;
		this.DeactiveScope();
		this.DeactiveAll();
		this.UpdateWeaponData(WeaponType.Knife, dataKnife);
		this.UpdateWeaponData(WeaponType.Pistol, dataPistol);
		this.UpdateWeaponData(WeaponType.Rifle, dataRifle);
		TimerManager.In(nValue.float005, delegate()
		{
			this.SetWeapon(selectWeapon, false);
			this.isUpdateWeaponData = false;
			this.WeaponCamera.cullingMask = int.MinValue;
		});
		this.UpdateShowButtons();
	}

	public void UpdateWeaponAll(WeaponType selectWeapon, int knife, int pistol, int rifle, WeaponCustomData dataKnife, WeaponCustomData dataPistol, WeaponCustomData dataRifle)
	{
		this.isUpdateWeaponData = true;
		this.WeaponCamera.cullingMask = 0;
		this.DeactiveScope();
		this.DeactiveAll();
		this.UpdateWeaponData(knife, dataKnife);
		this.UpdateWeaponData(pistol, dataPistol);
		this.UpdateWeaponData(rifle, dataRifle);
		TimerManager.In(nValue.float005, delegate()
		{
			this.SetWeapon(selectWeapon, false);
			this.isUpdateWeaponData = false;
			this.WeaponCamera.cullingMask = int.MinValue;
		});
		this.UpdateShowButtons();
	}

	public void UpdateWeapon(WeaponType type, bool isSelectWeapon)
	{
		this.UpdateWeapon(type, isSelectWeapon, null);
	}

	public void UpdateWeapon(WeaponType type, bool isSelectWeapon, WeaponCustomData customData)
	{
		int weaponID = WeaponManager.GetSelectWeapon(type);
		TimerManager.In(nValue.float005, delegate()
		{
			this.isUpdateWeaponData = true;
			this.DeactiveAll();
			WeaponManager.SetSelectWeapon(type, weaponID);
			this.UpdateWeaponData(type, customData);
			TimerManager.In(nValue.float005, delegate()
			{
				if (isSelectWeapon)
				{
					this.SetWeapon(type, false);
				}
				else
				{
					this.SetWeapon(this.SelectedWeapon, false);
				}
				this.isUpdateWeaponData = false;
			});
			this.UpdateShowButtons();
		});
	}

	public void UpdateWeapon(int weaponID, bool isSelectWeapon)
	{
		this.UpdateWeapon(weaponID, isSelectWeapon, null);
	}

	public void UpdateWeapon(int weaponID, bool isSelectWeapon, WeaponCustomData customData)
	{
		TimerManager.In(nValue.float005, delegate()
		{
			this.isUpdateWeaponData = true;
			this.DeactiveAll();
			this.UpdateWeaponData(weaponID, customData);
			TimerManager.In(nValue.float005, delegate()
			{
				if (isSelectWeapon)
				{
					this.SetWeapon(WeaponManager.GetWeaponData(weaponID).Type, false);
				}
				else
				{
					this.SetWeapon(this.SelectedWeapon, false);
				}
				this.isUpdateWeaponData = false;
			});
			this.UpdateShowButtons();
		});
	}

	private void UpdateShowButtons()
	{
		int num = nValue.int0;
		if (this.KnifeData.Enabled)
		{
			num++;
		}
		if (this.PistolData.Enabled)
		{
			num++;
		}
		if (this.RifleData.Enabled)
		{
			num++;
		}
		if (num >= nValue.int2)
		{
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(true);
		}
		else
		{
			UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
		}
	}

	private void UpdateWeaponData(WeaponType weaponType)
	{
		this.UpdateWeaponData(weaponType, null);
	}

	private void UpdateWeaponData(int weaponID)
	{
		this.UpdateWeaponData(weaponID, null);
	}

	private void UpdateWeaponData(WeaponType weaponType, WeaponCustomData customData)
	{
		WeaponData data = null;
		switch (weaponType)
		{
		case WeaponType.Knife:
			if (WeaponManager.HasSelectWeapon(WeaponType.Knife))
			{
				data = WeaponManager.GetSelectWeaponData(WeaponType.Knife);
			}
			break;
		case WeaponType.Pistol:
			if (WeaponManager.HasSelectWeapon(WeaponType.Pistol))
			{
				data = WeaponManager.GetSelectWeaponData(WeaponType.Pistol);
			}
			break;
		case WeaponType.Rifle:
			if (WeaponManager.HasSelectWeapon(WeaponType.Rifle))
			{
				data = WeaponManager.GetSelectWeaponData(WeaponType.Rifle);
			}
			break;
		}
		this.UpdateWeaponData(weaponType, data, true, customData);
	}

	private void UpdateWeaponData(int weaponID, WeaponCustomData customData)
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(weaponID);
		this.UpdateWeaponData(weaponData.Type, weaponData, true, customData);
	}

	private void UpdateWeaponData(WeaponType weaponType, WeaponData data, bool isUpgrade, WeaponCustomData customData)
	{
		PlayerWeapons.PlayerWeaponData playerWeaponData = new PlayerWeapons.PlayerWeaponData();
		if (data != null && !this.WeaponObjects.ContainsKey(data.Name) && data.FpsPrefab != null)
		{
			GameObject gameObject = data.FpsPrefab;
			gameObject = Utils.AddChild(gameObject, GameManager.player.FPCamera.transform, default(Vector3), default(Quaternion));
			this.WeaponObjects.Add(data.Name, gameObject);
			gameObject.SetActive(true);
		}
		if (data != null)
		{
			playerWeaponData.Enabled = true;
			playerWeaponData.CustomData = false;
			playerWeaponData.ID = data.ID;
			playerWeaponData.CanFire = data.CanFire;
			playerWeaponData.Name = data.Name;
			playerWeaponData.FaceDamage = data.FaceDamage;
			playerWeaponData.BodyDamage = data.BodyDamage;
			playerWeaponData.HandDamage = data.HandDamage;
			playerWeaponData.LegDamage = data.LegDamage;
			playerWeaponData.FireRate = data.FireRate;
			playerWeaponData.Accuracy = data.Accuracy;
			playerWeaponData.FireAccuracy = data.FireAccuracy;
			playerWeaponData.Ammo = data.Ammo;
			playerWeaponData.AmmoTotal = data.Ammo;
			playerWeaponData.AmmoMax = data.MaxAmmo;
			playerWeaponData.Mass = data.Mass;
			playerWeaponData.FireBullets = data.FireBullets;
			playerWeaponData.ReloadTime = data.ReloadTime;
			playerWeaponData.ReloadWarning = data.ReloadWarning;
			playerWeaponData.Distance = data.Distance;
			playerWeaponData.Scope = data.Scope;
			playerWeaponData.ScopeSize = data.ScopeSize;
			playerWeaponData.ScopeSensitivity = data.ScopeSensitivity;
			playerWeaponData.ScopeRecoil = data.ScopeRecoil;
			playerWeaponData.ScopeAccuracy = data.ScopeAccuracy;
			playerWeaponData.Scope2 = data.Scope2;
			playerWeaponData.Scope2Size = data.Scope2Size;
			playerWeaponData.Scope2Sensitivity = data.Scope2Sensitivity;
			playerWeaponData.FireSound = data.FireSound;
			playerWeaponData.ReloadSound = data.ReloadSound;
			playerWeaponData.Skin = AccountManager.GetWeaponSkinSelected(playerWeaponData.ID);
			playerWeaponData.FireStat = AccountManager.GetFireStatCounter(playerWeaponData.ID, playerWeaponData.Skin);
			playerWeaponData.Stickers = this.ConvertArray(AccountManager.GetWeaponStickers(playerWeaponData.ID, playerWeaponData.Skin).ToArray());
			playerWeaponData.LastFire = 0f;
			playerWeaponData.Script = this.WeaponObjects[data.Name].GetComponent<FPWeaponShooter>();
			if (customData != null)
			{
				playerWeaponData.CustomData = customData.CustomData;
				if (customData.FaceDamage != -nValue.int1)
				{
					playerWeaponData.FaceDamage = customData.FaceDamage;
				}
				if (customData.BodyDamage != -nValue.int1)
				{
					playerWeaponData.BodyDamage = customData.BodyDamage;
				}
				if (customData.HandDamage != -nValue.int1)
				{
					playerWeaponData.HandDamage = customData.HandDamage;
				}
				if (customData.LegDamage != -nValue.int1)
				{
					playerWeaponData.LegDamage = customData.LegDamage;
				}
				if (customData.Ammo != -nValue.int1)
				{
					playerWeaponData.Ammo = customData.Ammo;
				}
				if (customData.AmmoMax != -nValue.int1)
				{
					playerWeaponData.AmmoMax = customData.AmmoMax;
				}
				if (customData.AmmoTotal != -nValue.int1)
				{
					playerWeaponData.AmmoTotal = customData.AmmoTotal;
				}
				if (customData.Mass != -nValue.float1)
				{
					playerWeaponData.Mass = customData.Mass;
				}
				if (customData.Skin != -nValue.int1)
				{
					playerWeaponData.Skin = customData.Skin;
				}
				if (customData.FireStatCounter >= 0)
				{
					playerWeaponData.FireStat = customData.FireStatCounter;
				}
				else if (playerWeaponData.CustomData)
				{
					playerWeaponData.FireStat = -1;
				}
				if (customData.Stickers != null && customData.Stickers.Length != 0)
				{
					playerWeaponData.Stickers = customData.Stickers;
				}
				else if (playerWeaponData.CustomData)
				{
					playerWeaponData.Stickers = new CryptoInt[0];
				}
			}
			playerWeaponData.Script.UpdateWeaponData(playerWeaponData);
		}
		else
		{
			playerWeaponData.Enabled = false;
		}
		switch (weaponType)
		{
		case WeaponType.Knife:
			this.KnifeData = playerWeaponData;
			return;
		case WeaponType.Pistol:
			this.PistolData = playerWeaponData;
			return;
		case WeaponType.Rifle:
			this.RifleData = playerWeaponData;
			return;
		default:
			return;
		}
	}

	public void SetWeapon(WeaponType weapon)
	{
		this.SetWeapon(weapon, true);
	}

	public void SetWeapon(WeaponType weapon, bool checkSelectedWeapon)
	{
		if (checkSelectedWeapon && this.SelectedWeapon == weapon)
		{
			return;
		}
		if (!this.GetWeaponData(weapon).Enabled)
		{
			return;
		}
		this.DeactiveScope();
		this.DeactiveAll();
		this.SelectedWeapon = weapon;
		PlayerWeapons.PlayerWeaponData selectedWeaponData = this.GetSelectedWeaponData();
		UICrosshair.SetAccuracy(selectedWeaponData.Accuracy);
		UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, this.InfiniteAmmo, selectedWeaponData.ReloadWarning);
		UIControllerList.Aim.cachedGameObject.SetActive(selectedWeaponData.Scope || selectedWeaponData.Scope2);
		GameManager.player.SetPlayerSpeed(selectedWeaponData.Mass);
		this.Timer.Cancel("ReloadTime");
		this.isReload = false;
		this.Sounds.Stop();
		this.Timer.Cancel("WieldedTime");
		this.Wielded = true;
		if (!this.Timer.Contains("WieldedTime"))
		{
			this.Timer.Create("WieldedTime", nValue.float05, delegate()
			{
				this.Wielded = false;
			});
		}
		this.Timer.In("WieldedTime");
		switch (weapon)
		{
		case WeaponType.Knife:
			this.KnifeData.Script.Active();
			GameManager.controller.SetWeapon(this.KnifeData);
			break;
		case WeaponType.Pistol:
			this.PistolData.Script.Active();
			GameManager.controller.SetWeapon(this.PistolData);
			break;
		case WeaponType.Rifle:
			this.RifleData.Script.Active();
			GameManager.controller.SetWeapon(this.RifleData);
			break;
		}
	}

	private void DeactiveAll()
	{
		if (this.KnifeData.Script != null)
		{
			this.KnifeData.Script.Deactive();
		}
		if (this.PistolData.Script != null)
		{
			this.PistolData.Script.Deactive();
		}
		if (this.RifleData.Script != null)
		{
			this.RifleData.Script.Deactive();
		}
	}

	public void FireWeapon()
	{
		nProfiler.BeginSample("PlayerWeapons.FireWeapon");
		if (!this.CanFire || this.isReload || GameManager.roundState == RoundState.EndRound)
		{
			return;
		}
		switch (this.SelectedWeapon)
		{
		case WeaponType.Knife:
			this.Fire(this.KnifeData);
			break;
		case WeaponType.Pistol:
			this.Fire(this.PistolData);
			break;
		case WeaponType.Rifle:
			this.Fire(this.RifleData);
			break;
		}
		nProfiler.EndSample();
	}

	private void Fire(PlayerWeapons.PlayerWeaponData weapon)
	{
		nProfiler.BeginSample("PlayerWeapons.Fire");
		if (this.Wielded)
		{
			return;
		}
		if (weapon.FireUp && this.isFire)
		{
			return;
		}
		if (weapon.LastFire > Time.time)
		{
			return;
		}
		if (this.isUpdateWeaponData || !weapon.CanFire)
		{
			return;
		}
		if (weapon.Script.Knife.Enabled)
		{
			weapon.LastFire = Time.time + weapon.FireRate;
			this.Sounds.Play(weapon.FireSound);
			weapon.Script.Fire();
			TimerManager.In(weapon.Script.Knife.Delay, delegate()
			{
				this.FireData(weapon);
			});
		}
		else if (weapon.Ammo > 0)
		{
			PlayerWeapons.PlayerWeaponData weapon2 = weapon;
			weapon2.Ammo = --weapon2.Ammo;
			weapon.LastFire = Time.time + weapon.FireRate;
			this.Sounds.Play(weapon.FireSound);
			weapon.Script.Fire();
			this.FireData(weapon);
			if (this.isScope && weapon.Scope && weapon.ScopeRecoil != (float)nValue.int0)
			{
				GameManager.player.FPCamera.Pitch -= weapon.ScopeRecoil;
				float num = weapon.ScopeRecoil / (float)nValue.int2;
				GameManager.player.FPCamera.Yaw += UnityEngine.Random.Range(-num, num);
			}
		}
		else if (weapon.AmmoMax == 0)
		{
			this.DryFire(weapon);
		}
		else
		{
			this.Reload(weapon);
		}
		nProfiler.EndSample();
	}

	private void FireData(PlayerWeapons.PlayerWeaponData weapon)
	{
		nProfiler.BeginSample("PlayerWeapons.FireData");
		UIAmmo.SetAmmo(weapon.Ammo, weapon.AmmoMax, this.InfiniteAmmo, weapon.ReloadWarning);
		DecalInfo decalInfo = DecalInfo.Get();
		if (this.SelectedWeapon == WeaponType.Knife)
		{
			decalInfo.isKnife = true;
		}
		for (int i = nValue.int0; i < weapon.FireBullets; i++)
		{
			if (this.isScope && weapon.Scope)
			{
				this.FireAccuracy = UICrosshair.Fire(weapon.ScopeAccuracy);
			}
			else
			{
				this.FireAccuracy = UICrosshair.Fire(weapon.FireAccuracy);
			}
			this.FireAccuracy.x = UnityEngine.Random.Range(-this.FireAccuracy.x, this.FireAccuracy.x);
			this.FireAccuracy.y = UnityEngine.Random.Range(-this.FireAccuracy.y, this.FireAccuracy.y);
			this.FireRay = this.PlayerCamera.ViewportPointToRay(new Vector3(nValue.float05 + this.FireAccuracy.x, nValue.float05 + this.FireAccuracy.y, (float)nValue.int0));
			if (nRaycast.RaycastFire(this.FireRay, weapon.Distance, this.FireLayers))
			{
				nProfiler.BeginSample("nRaycast.RaycastFire => True");
				nProfiler.BeginSample("1");
				if ((int)decalInfo.BloodDecal == nValue.int200)
				{
					decalInfo.BloodDecal = (byte)decalInfo.Points.size;
					decalInfo.Points.Add(nRaycast.hitPoint);
					decalInfo.Normals.Add(Vector3.zero);
				}
				nProfiler.EndSample();
				nProfiler.BeginSample("2");
				this.FireDamageInfo.Set(weapon.BodyDamage, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTeam, weapon.ID, weapon.Skin, PhotonNetwork.player.ID, false);
				nProfiler.EndSample();
				nProfiler.BeginSample("3");
				nRaycast.hitBoxCollider.playerDamage.Damage(this.FireDamageInfo);
				nProfiler.EndSample();
				nProfiler.BeginSample("SparkEffectManager.Fire");
				if (!this.isScope)
				{
					SparkEffectManager.Fire(nRaycast.hitPoint, nRaycast.hitDistance);
				}
				nProfiler.EndSample();
			}
			else if (Physics.Raycast(this.FireRay, out this.FireRaycastHit, weapon.Distance, this.FireLayers))
			{
				this.FireRayTransform = this.FireRaycastHit.transform;
				if (!this.FireRayTransform.CompareTag("IgnoreDecal") && !this.FireRayTransform.CompareTag("PlayerSkin"))
				{
					if (this.FireRayTransform.CompareTag("RigidbodyObject") && this.PushRigidbody)
					{
						this.FireRaycastHit.transform.GetComponent<RigidbodyObject>().Force(this.PlayerCamera.transform.forward * this.PushRigidbodyForce);
					}
					else if (this.FireRayTransform.CompareTag("DamageObject"))
					{
						this.FireDamageInfo = DamageInfo.Get(weapon.BodyDamage, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTeam, weapon.ID, weapon.Skin, PhotonNetwork.player.ID, false);
						this.FireRaycastHit.transform.SendMessage("Damage", this.FireDamageInfo, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						decalInfo.Points.Add(this.FireRaycastHit.point);
						decalInfo.Normals.Add(this.FireRaycastHit.normal);
						if (!this.isScope)
						{
							SparkEffectManager.Fire(this.FireRaycastHit.point, this.FireRaycastHit.distance);
						}
					}
				}
			}
		}
		GameManager.controller.FireWeapon(decalInfo);
		nProfiler.EndSample();
	}

	private void DryFire(PlayerWeapons.PlayerWeaponData weapon)
	{
		if (this.isDryFire)
		{
			return;
		}
		this.isDryFire = true;
		weapon.Script.DryFire();
		this.Sounds.Play(WeaponSound.AmmoEmpty);
	}

	public void ReloadWeapon()
	{
		if (this.isReload)
		{
			return;
		}
		switch (this.SelectedWeapon)
		{
		case WeaponType.Knife:
			if (this.KnifeData.Script != null)
			{
				this.KnifeData.Script.StartInspectWeapon();
			}
			break;
		case WeaponType.Pistol:
			this.Reload(this.PistolData);
			break;
		case WeaponType.Rifle:
			this.Reload(this.RifleData);
			break;
		}
	}

	private void Reload(PlayerWeapons.PlayerWeaponData weapon)
	{
		if (!weapon.Enabled)
		{
			return;
		}
		if (this.Wielded)
		{
			return;
		}
		if (this.isScope)
		{
			this.DeactiveScope();
		}
		if (weapon.Ammo == weapon.AmmoTotal || weapon.AmmoMax == nValue.int0)
		{
			weapon.Script.StartInspectWeapon();
			return;
		}
		this.isReload = true;
		this.Sounds.Play(weapon.ReloadSound);
		weapon.Script.Reload(weapon.ReloadTime);
		GameManager.controller.ReloadWeapon();
		if (!this.Timer.Contains("ReloadTime"))
		{
			this.Timer.Create("ReloadTime", weapon.ReloadTime + nValue.float05, delegate()
			{
				PlayerWeapons.PlayerWeaponData selectedWeaponData = this.GetSelectedWeaponData();
				this.isReload = false;
				if (this.InfiniteAmmo)
				{
					selectedWeaponData.Ammo = selectedWeaponData.AmmoTotal;
				}
				else if (selectedWeaponData.AmmoMax > selectedWeaponData.AmmoTotal)
				{
					PlayerWeapons.PlayerWeaponData playerWeaponData = selectedWeaponData;
					playerWeaponData.AmmoMax -= selectedWeaponData.AmmoTotal - selectedWeaponData.Ammo;
					selectedWeaponData.Ammo = selectedWeaponData.AmmoTotal;
				}
				else
				{
					int num = selectedWeaponData.Ammo;
					PlayerWeapons.PlayerWeaponData playerWeaponData2 = selectedWeaponData;
					playerWeaponData2.Ammo += selectedWeaponData.AmmoMax;
					selectedWeaponData.Ammo = Mathf.Min(selectedWeaponData.AmmoTotal, selectedWeaponData.Ammo);
					PlayerWeapons.PlayerWeaponData playerWeaponData3 = selectedWeaponData;
					playerWeaponData3.AmmoMax -= selectedWeaponData.Ammo - num;
					selectedWeaponData.AmmoMax = Mathf.Max(nValue.int0, selectedWeaponData.AmmoMax);
				}
				UIAmmo.SetAmmo(selectedWeaponData.Ammo, selectedWeaponData.AmmoMax, this.InfiniteAmmo, selectedWeaponData.ReloadWarning);
			});
		}
		this.Timer.In("ReloadTime", weapon.ReloadTime + nValue.float05);
	}

	private void ScopeWeapon(bool check)
	{
		if (check && this.isReload)
		{
			return;
		}
		if (!this.GetSelectedWeaponData().Scope && !this.GetSelectedWeaponData().Scope2 && check)
		{
			return;
		}
		this.isScope = !this.isScope;
		LODObject.isScope = this.isScope;
		DOTween.Kill("Scope", false);
		DOTween.Kill("Scope2", false);
		if (this.isScope)
		{
			if (this.GetSelectedWeaponData().Scope)
			{
				this.PlayerCamera.DOFieldOfView((float)this.GetSelectedWeaponData().ScopeSize, nValue.float02).id = "Scope";
				this.WeaponCamera.fieldOfView = (float)nValue.int1;
				UICrosshair.SetActiveScope(true);
			}
			else if (this.GetSelectedWeaponData().Scope2)
			{
				this.PlayerCamera.DOFieldOfView((float)this.GetSelectedWeaponData().Scope2Size, nValue.float02).id = "Scope";
				this.WeaponCamera.DOFieldOfView((float)this.GetSelectedWeaponData().Scope2Size, nValue.float02).id = "Scope2";
			}
		}
		else
		{
			DOTween.Kill("Scope", false);
			DOTween.Kill("Scope2", false);
			this.PlayerCamera.DOFieldOfView(nValue.float60, nValue.float02).id = "Scope";
			if (this.GetSelectedWeaponData().Scope2)
			{
				this.WeaponCamera.DOFieldOfView(nValue.float60, nValue.float02).id = "Scope2";
			}
			else
			{
				this.WeaponCamera.fieldOfView = nValue.float60;
			}
			UICrosshair.SetActiveScope(false);
		}
		this.GetSelectedWeaponData().Script.ScopeRifle();
	}

	public void DeactiveScope()
	{
		if (this.isScope)
		{
			this.ScopeWeapon(false);
		}
		if (GameManager.player.Dead)
		{
			UICrosshair.SetActiveCrosshair(false);
		}
	}

	private void KillPlayer(DamageInfo damageInfo)
	{
		if (LevelManager.customScene)
		{
			return;
		}
		PlayerWeapons.PlayerWeaponData playerWeaponData = null;
		if (this.PistolData.Enabled && this.PistolData.ID == damageInfo.weapon && this.PistolData.Script.FireStat.Enabled)
		{
			playerWeaponData = this.PistolData;
		}
		else if (this.RifleData.Enabled && this.RifleData.ID == damageInfo.weapon && this.RifleData.Script.FireStat.Enabled)
		{
			playerWeaponData = this.RifleData;
		}
		if (playerWeaponData == null)
		{
			return;
		}
		if (playerWeaponData.CustomData)
		{
			return;
		}
		int num = AccountManager.GetFireStatCounter(damageInfo.weapon, damageInfo.weaponSkin) + nValue.int1;
		playerWeaponData.Script.UpdateFireStat(num);
		PlayerRoundManager.SetFireStat1(damageInfo.weapon, damageInfo.weaponSkin);
		AccountManager.SetFireStatCounter(damageInfo.weapon, damageInfo.weaponSkin, num);
        AccountManager.SetFireStatFireBase(false, damageInfo.weapon, damageInfo.weaponSkin, num);
        GameManager.controller.UpdateFireStatValue(playerWeaponData.ID);
	}

	public void DropWeapon()
	{
		this.DropWeapon(this.SelectedWeapon, false);
	}

	public void DropWeapon(bool updateWeapon)
	{
		this.DropWeapon(this.SelectedWeapon, updateWeapon);
	}

	public void DropWeapon(WeaponType type)
	{
		this.DropWeapon(type, false);
	}

	public void DropWeapon(WeaponType type, bool updateWeapon)
	{
		if (type == WeaponType.Knife)
		{
			return;
		}
		PlayerWeapons.PlayerWeaponData weaponData = this.GetWeaponData(type);
		if (!weaponData.Enabled)
		{
			return;
		}
		bool flag = DropWeaponManager.CreateWeapon(false, (byte)weaponData.ID, (byte)weaponData.Skin, true, GameManager.player.PlayerTransform.position, GameManager.player.PlayerTransform.eulerAngles, this.StickersToByteArray(weaponData.Stickers), weaponData.FireStat, weaponData.Ammo, weaponData.AmmoMax);
		if (updateWeapon && flag)
		{
			weaponData.Enabled = false;
			this.UpdateSelectWeapon();
		}
	}

	private byte[] StickersToByteArray(CryptoInt[] stickers)
	{
		byte[] array = new byte[stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)stickers[i];
		}
		return array;
	}

	public byte[] SerializeWeapons()
	{
		byte[] array = new byte[6];
		array[0] = (byte)this.GetSelectedWeaponData().ID;
		array[1] = (byte)this.GetSelectedWeaponData().Skin;
		byte b = 2;
		if (this.SelectedWeapon != WeaponType.Rifle)
		{
			array[(int)b] = (byte)this.RifleData.ID;
			array[(int)(b + 1)] = (byte)this.RifleData.Skin;
			b += 2;
		}
		if (this.SelectedWeapon != WeaponType.Pistol)
		{
			array[(int)b] = (byte)this.PistolData.ID;
			array[(int)(b + 1)] = (byte)this.PistolData.Skin;
			b += 2;
		}
		if (this.SelectedWeapon != WeaponType.Knife)
		{
			array[(int)b] = (byte)this.KnifeData.ID;
			array[(int)(b + 1)] = (byte)this.KnifeData.Skin;
		}
		for (int i = 0; i < array.Length; i++)
		{
			MonoBehaviour.print(array[i]);
		}
		return array;
	}

	public CryptoInt[] ConvertArray(int[] array)
	{
		if (array == null)
		{
			return new CryptoInt[0];
		}
		CryptoInt[] array2 = new CryptoInt[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i];
		}
		return array2;
	}

	public int[] ConvertArray(CryptoInt[] array)
	{
		if (array == null)
		{
			return new int[0];
		}
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array[i];
		}
		return array2;
	}

	[Serializable]
	public class PlayerWeaponData
	{
		public bool Enabled;

		public bool CustomData;

		public CryptoInt ID;

		public bool CanFire;

		public bool FireUp;

		public CryptoString Name;

		public CryptoInt FaceDamage;

		public CryptoInt BodyDamage;

		public CryptoInt HandDamage;

		public CryptoInt LegDamage;

		public CryptoFloat FireRate;

		public CryptoFloat Accuracy;

		public CryptoFloat FireAccuracy;

		public CryptoInt FireBullets;

		public CryptoFloat ReloadTime;

		public CryptoInt ReloadWarning;

		public CryptoFloat Distance;

		public CryptoFloat Mass;

		public bool Scope;

		public CryptoInt ScopeSize;

		public CryptoFloat ScopeSensitivity;

		public CryptoFloat ScopeRecoil;

		public CryptoFloat ScopeAccuracy;

		public bool Scope2;

		public CryptoInt Scope2Size;

		public CryptoFloat Scope2Sensitivity;

		public WeaponSound FireSound;

		public WeaponSound ReloadSound;

		public CryptoInt Ammo;

		public CryptoInt AmmoTotal;

		public CryptoInt AmmoMax;

		public CryptoInt Skin;

		public CryptoInt FireStat;

		public CryptoInt[] Stickers;

		public float LastFire;

		public FPWeaponShooter Script;
	}
}
