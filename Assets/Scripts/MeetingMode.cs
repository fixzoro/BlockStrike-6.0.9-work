using System;
using UnityEngine;

public class MeetingMode : MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.offlineMode)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (PhotonNetwork.room.GetGameMode() != GameMode.Meeting)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		GameManager.roundState = RoundState.PlayRound;
		GameManager.startDamageTime = (float)nValue.int2;
		UIScore.SetActiveScore(false);
		UIPanelManager.ShowPanel("Display");
		UIPlayerStatistics.isOnlyBluePanel = true;
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(nValue.float05, delegate()
		{
			GameManager.team = Team.Blue;
			this.OnRevivalPlayer();
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
	}

	private void OnRevivalPlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(nValue.int100);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(SpawnManager.GetTeamSpawn().spawnPosition, SpawnManager.GetTeamSpawn().spawnRotation);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, nValue.int0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, nValue.int0);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		player.PlayerWeapon.CanFire = false;
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		UIDeathScreen.Show(damageInfo);
		UIStatus.Add(damageInfo);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.player.PlayerTransform.position, damageInfo.position);
		CameraManager.SetType(CameraType.Dead, new object[]
		{
			GameManager.player.FPCamera.Transform.position,
			GameManager.player.FPCamera.Transform.eulerAngles,
			ragdollForce * (float)nValue.int100
		});
		GameManager.controller.DeactivePlayer(ragdollForce, damageInfo.headshot);
		TimerManager.In((float)nValue.int3, delegate()
		{
			this.OnRevivalPlayer();
		});
	}

	public void OnClimb(bool active)
	{
		PlayerInput.instance.SetClimb(active);
	}

	public void OnIce(bool active)
	{
		PlayerInput.instance.SetMoveIce(active);
	}

	public void OnTrampoline(float force)
	{
		PlayerInput.instance.FPController.AddForce(new Vector3(0f, force, 0f));
	}

	public void OnSize(float size)
	{
		PlayerSkin[] array = UnityEngine.Object.FindObjectsOfType<PlayerSkin>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Controller.transform.localScale = Vector3.one * size;
		}
	}

	public void GiveKnife(int weaponID)
	{
		WeaponManager.SetSelectWeapon(WeaponType.Knife, weaponID);
		PlayerInput player = GameManager.player;
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
	}
}
