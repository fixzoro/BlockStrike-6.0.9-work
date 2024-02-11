using System;
using UnityEngine;

public class SelectedWeaponAttribute : PropertyAttribute
{
	public SelectedWeaponAttribute(WeaponType weapon)
	{
		this.weaponType = weapon;
	}
    
	public SelectedWeaponAttribute()
	{
		this.AllWeapons = true;
	}
    
	public bool AllWeapons;
    
	public WeaponType weaponType;
    
	public int selected;
}
