using System;
using System.Collections.Generic;
using UnityEngine;

public class MurderModeManager : MonoBehaviour
{
    public GameObject Pistol;

    private bool canPickupPistol = true;

    private static MurderModeManager instance;

    private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Murder)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			MurderModeManager.instance = this;
		}
	}

	private void Start()
	{
		PhotonRPC.AddMessage("PhotonMasterPickupPistol", new PhotonRPC.MessageDelegate(this.PhotonMasterPickupPistol));
		PhotonRPC.AddMessage("PhotonPickupPistol", new PhotonRPC.MessageDelegate(this.PhotonPickupPistol));
		PhotonRPC.AddMessage("PhotonSetPosition", new PhotonRPC.MessageDelegate(this.PhotonSetPosition));
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
	}

	private void StartRound()
	{
		this.Pistol.SetActive(false);
		this.canPickupPistol = true;
	}

	public static void SetRandomPlayerPistol()
	{
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (!PhotonNetwork.playerList[i].GetDead() && PhotonNetwork.playerList[i].ID != MurderMode.Murder)
			{
				list.Add(PhotonNetwork.playerList[i]);
			}
		}
		if (list.Count > nValue.int0)
		{
			PhotonPlayer photonPlayer = list[UnityEngine.Random.Range(nValue.int0, list.Count)];
			PhotonRPC.RPC("PhotonPickupPistol", PhotonTargets.All, photonPlayer.ID);
		}
	}

	public static void DeadPlayer()
	{
		if (PhotonNetwork.player.ID == MurderMode.Detective)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out raycastHit, (float)nValue.int50))
			{
				PhotonDataWrite data = PhotonRPC.GetData();
				data.Write(raycastHit.point);
				data.Write(raycastHit.normal);
				PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
			}
			else
			{
				MurderModeManager.SetRandomPlayerPistol();
			}
		}
	}

	public static void DeadBystander()
	{
		MurderModeManager.instance.canPickupPistol = false;
		TimerManager.In(30f, delegate()
		{
			MurderModeManager.instance.canPickupPistol = true;
		});
		RaycastHit raycastHit;
		if (Physics.Raycast(PlayerInput.instance.PlayerTransform.position, Vector3.down, out raycastHit, (float)nValue.int50))
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(raycastHit.point);
			data.Write(raycastHit.normal);
			PhotonRPC.RPC("PhotonSetPosition", PhotonTargets.All, data);
		}
		else
		{
			MurderModeManager.SetRandomPlayerPistol();
		}
	}

	[PunRPC]
	private void PhotonSetPosition(PhotonMessage message)
	{
		MurderMode.Detective = -1;
		MurderModeManager.SetPosition(message.ReadVector3(), message.ReadVector3());
	}

	public static void SetPosition(Vector3 pos, Vector3 normal)
	{
		MurderModeManager.instance.Pistol.transform.position = pos + normal * nValue.float001;
		MurderModeManager.instance.Pistol.transform.rotation = Quaternion.LookRotation(normal);
		MurderModeManager.instance.Pistol.SetActive(true);
	}

	public static void SetPosition(Vector3 pos)
	{
		MurderModeManager.instance.Pistol.transform.position = pos;
		MurderModeManager.instance.Pistol.transform.rotation = Quaternion.Euler((float)(-(float)nValue.int90), (float)UnityEngine.Random.Range(nValue.int0, nValue.int360), (float)nValue.int0);
		MurderModeManager.instance.Pistol.SetActive(true);
	}

	public void OnTriggerEnterPistol()
	{
		if (GameManager.roundState == RoundState.PlayRound && this.canPickupPistol && PhotonNetwork.player.ID != MurderMode.Murder)
		{
			PhotonRPC.RPC("PhotonMasterPickupPistol", PhotonTargets.MasterClient);
		}
	}

	[PunRPC]
	private void PhotonMasterPickupPistol(PhotonMessage message)
	{
		if (this.Pistol.activeSelf)
		{
			this.Pistol.SetActive(false);
			PhotonRPC.RPC("PhotonPickupPistol", PhotonTargets.All, message.sender.ID);
		}
	}

	[PunRPC]
	private void PhotonPickupPistol(PhotonMessage message)
	{
		this.Pistol.SetActive(false);
		int num = message.ReadInt();
		MurderMode.Detective = num;
		if (PhotonNetwork.player.ID == num)
		{
			WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 2);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, 47);
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.Ammo = 1;
			weaponCustomData.AmmoTotal = 1;
			weaponCustomData.AmmoMax = 99;
			weaponCustomData.BodyDamage = 100;
			weaponCustomData.FaceDamage = 100;
			weaponCustomData.HandDamage = 100;
			weaponCustomData.LegDamage = 100;
			weaponCustomData.Skin = 0;
			weaponCustomData.CustomData = false;
			PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle, null, weaponCustomData, null);
		}
	}
}
