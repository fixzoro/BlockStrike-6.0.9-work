using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class DropWeaponManager : Photon.MonoBehaviour
{
    public bool actived;

    public static float lastDropTime;

    public static float lastPickupTime;

    private List<DropWeapon> list = new List<DropWeapon>();

    private static DropWeaponManager instance;

    public static bool enable
	{
		get
		{
			return DropWeaponManager.instance.actived;
		}
		set
		{
			DropWeaponManager.instance.actived = value;
		}
	}

	private void Awake()
	{
		DropWeaponManager.instance = this;
	}

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonCreateWeapon", new PhotonRPC.MessageDelegate(this.PhotonCreateWeapon));
		PhotonRPC.AddMessage("PhotonMasterPickupWeapon", new PhotonRPC.MessageDelegate(this.PhotonMasterPickupWeapon));
		PhotonRPC.AddMessage("PhotonPickupWeapon", new PhotonRPC.MessageDelegate(this.PhotonPickupWeapon));
	}

	public static void ClearScene()
	{
		if (DropWeaponManager.instance == null)
		{
			return;
		}
		for (int i = 0; i < DropWeaponManager.instance.list.Count; i++)
		{
			if (DropWeaponManager.instance.list[i].Weapon.activeSelf)
			{
				DropWeaponManager.instance.list[i].DestroyWeapon();
			}
		}
	}

	public static bool CreateWeapon(bool local, byte weaponID, byte weaponSkin, bool checkPos, Vector3 pos, Vector3 rot, byte[] stickers, int firestat, int ammo, int ammoMax)
	{
		if (checkPos)
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(pos, Vector3.down, out raycastHit, 50f))
			{
				return false;
			}
			pos = raycastHit.point + raycastHit.normal * 0.01f;
			rot = Quaternion.LookRotation(raycastHit.normal).eulerAngles;
		}
		DropWeaponManager.lastDropTime = Time.time;
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(weaponID);
		data.Write(weaponSkin);
		data.Write(pos);
		data.Write(rot);
		data.Write(stickers);
		data.Write(firestat);
		data.Write(ammo);
		data.Write(ammoMax);
		if (local)
		{
			PhotonRPC.RPC("PhotonCreateWeapon", PhotonNetwork.player, data);
		}
		else
		{
			PhotonRPC.RPC("PhotonCreateWeapon", PhotonTargets.All, data);
		}
		return true;
	}

	[PunRPC]
	private void PhotonCreateWeapon(PhotonMessage message)
	{
		byte b = message.ReadByte();
		byte value = message.ReadByte();
		Vector3 position = message.ReadVector3();
		Vector3 eulerAngles = message.ReadVector3();
		byte[] stickers = message.ReadBytes();
		int value2 = message.ReadInt();
		int value3 = message.ReadInt();
		int value4 = message.ReadInt();
		if (WeaponManager.GetWeaponData((int)b).Type == WeaponType.Knife)
		{
			return;
		}
		GameObject gameObject = PoolManager.Spawn(b + "-Drop", GameSettings.instance.Weapons[(int)(b - 1)].DropPrefab);
		DropWeapon component = gameObject.GetComponent<DropWeapon>();
		gameObject.transform.position = position;
		gameObject.transform.eulerAngles = eulerAngles;
		component.ID = (int)(message.timestamp * 100.0);
		component.Data.Ammo = value3;
		component.Data.AmmoMax = value4;
		component.Data.Skin = (int)value;
		component.Data.FireStatCounter = value2;
		component.Data.Stickers = this.ConvertStickers(stickers);
		component.DestroyDrop = true;
		component.CustomData = true;
		component.UpdateWeapon();
		if (!this.list.Contains(component))
		{
			this.list.Add(component);
		}
		EventManager.Dispatch<DropWeapon>("DropWeapon", component);
	}

	public static void PickupWeapon(int id)
	{
		DropWeaponManager.lastPickupTime = Time.time;
		PhotonRPC.RPC("PhotonMasterPickupWeapon", PhotonTargets.MasterClient, id);
	}

	[PunRPC]
	private void PhotonMasterPickupWeapon(PhotonMessage message)
	{
		int num = message.ReadInt();
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].ID == num && this.list[i].Weapon.activeSelf)
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(message.sender.ID);
				data.Write(num);
				PhotonRPC.RPC("PhotonPickupWeapon", PhotonTargets.All, data);
				break;
			}
		}
	}

	[PunRPC]
	private void PhotonPickupWeapon(PhotonMessage message)
	{
		int num = message.ReadInt();
		int id = message.ReadInt();
		DropWeapon weapon = this.GetWeapon(id);
		if (weapon == null)
		{
			return;
		}
		if (PhotonNetwork.player.ID == num)
		{
			weapon.PickupWeapon();
		}
		else
		{
			weapon.DestroyWeapon();
		}
	}

	private DropWeapon GetWeapon(int id)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].ID == id)
			{
				return this.list[i];
			}
		}
		return null;
	}

	private CryptoInt[] ConvertStickers(byte[] stickers)
	{
		if (stickers == null)
		{
			return new CryptoInt[0];
		}
		CryptoInt[] array = new CryptoInt[stickers.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (stickers[i] == 255)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = (int)stickers[i];
			}
		}
		return array;
	}
}
