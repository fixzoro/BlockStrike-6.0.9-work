using System;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public CryptoInt ID;

    public CryptoBool Lock;

    public CryptoBool CanFire;

    public CryptoBool Secret;

    public WeaponType Type;

    public WeaponType Animation;

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

    public CryptoInt Ammo;

    public CryptoInt MaxAmmo;

    public CryptoFloat Distance;

    public CryptoFloat Mass;

    public CryptoBool Scope;

    public CryptoInt ScopeSize;

    public CryptoFloat ScopeSensitivity;

    public CryptoFloat ScopeRecoil;

    public CryptoFloat ScopeAccuracy;

    public CryptoBool Scope2;

    public CryptoInt Scope2Size;

    public CryptoFloat Scope2Sensitivity;

    public WeaponSound FireSound;

    public WeaponSound ReloadSound;

    public GameObject FpsPrefab;

    public GameObject TpsPrefab;

    public GameObject DropPrefab;

    public static WeaponData Copy(WeaponData data)
	{
		WeaponData weaponData = new WeaponData();
		weaponData.ID = data.ID;
		weaponData.Lock = data.Lock;
		weaponData.CanFire = data.CanFire;
		weaponData.Secret = data.Secret;
		weaponData.Type = data.Type;
		weaponData.Animation = data.Animation;
		weaponData.Name = data.Name;
		weaponData.FaceDamage = data.FaceDamage;
		weaponData.BodyDamage = data.BodyDamage;
		weaponData.HandDamage = data.HandDamage;
		weaponData.LegDamage = data.LegDamage;
		weaponData.FireRate = data.FireRate;
		weaponData.Accuracy = data.Accuracy;
		weaponData.FireAccuracy = data.FireAccuracy;
		weaponData.FireBullets = data.FireBullets;
		weaponData.ReloadTime = data.ReloadTime;
		weaponData.Ammo = data.Ammo;
		weaponData.MaxAmmo = data.MaxAmmo;
		weaponData.Distance = data.Distance;
		weaponData.Mass = data.Mass;
		weaponData.Scope = data.Scope;
		weaponData.ScopeSize = data.ScopeSize;
		weaponData.ScopeSensitivity = data.ScopeSensitivity;
		weaponData.ScopeRecoil = data.ScopeRecoil;
		weaponData.ScopeAccuracy = data.ScopeAccuracy;
		weaponData.FpsPrefab = data.FpsPrefab;
		weaponData.TpsPrefab = data.TpsPrefab;
		weaponData.FireSound = data.FireSound;
		weaponData.ReloadSound = data.ReloadSound;
		return data;
	}
}
