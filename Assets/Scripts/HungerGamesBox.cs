using System;
using System.Collections.Generic;
using UnityEngine;

public class HungerGamesBox : MonoBehaviour
{
    [Range(1f, 50f)]
    public int ID = 1;

    public bool Used;

    public List<HungerGamesBox.WeaponData> Weapons = new List<HungerGamesBox.WeaponData>();

    public DropWeaponStatic dropWeapon;

    private GameObject cachedGameObject;

    private void Start()
	{
		this.cachedGameObject = base.gameObject;
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener<int, int>("EventPickupBox", new EventManager.Callback<int, int>(this.Pickup));
		this.dropWeapon.onDropWeaponEvent += this.OnDropWeapon;
	}

	private void StartRound()
	{
		this.cachedGameObject.SetActive(true);
		this.dropWeapon.weaponID = this.Weapons[UnityEngine.Random.Range(0, this.Weapons.Count)].Weapon;
	}

	private void SelectWeapon()
	{
		this.Used = true;
		HungerGames.SetWeapon(this.dropWeapon.weaponID);
	}

	private void Pickup(int id, int pickupPlayer)
	{
		if (id == this.ID)
		{
			this.cachedGameObject.SetActive(false);
			if (pickupPlayer == PhotonNetwork.player.ID)
			{
				this.SelectWeapon();
			}
		}
	}

	public void OnDropWeapon()
	{
		HungerGames.PickupBox(this.ID);
	}

	[Serializable]
	public class WeaponData
	{
		[SelectedWeapon]
		public int Weapon;
	}
}
