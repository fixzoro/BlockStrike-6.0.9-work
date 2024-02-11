using System;

[Serializable]
public class WeaponUpgradeData
{
	public GameCurrency Currency;

	public CryptoInt Price;

	public CryptoInt FaceDamage = 50;
    
	public CryptoInt BodyDamage = 20;
    
	public CryptoInt HandDamage = 10;
    
	public CryptoInt LegDamage = 5;

	public CryptoFloat FireRate = 0.1f;
    
	public CryptoFloat Accuracy = 10f;

	public CryptoFloat FireAccuracy = 10f;
    
	public CryptoInt Ammo = 20;
    
	public CryptoInt MaxAmmo = 100;

	public CryptoFloat Mass = 0.01f;
}
