using System;
using UnityEngine;

public class mOthers : MonoBehaviour
{
    private bool isExit;

    private void Start()
	{
        #if UNITY_EDITOR
        Settings.Load();
        GameConsole.actived = Settings.Console;
        #else
        Application.targetFrameRate = 60;
        #endif
        InputManager.Init();
		if (!string.IsNullOrEmpty(GameManager.leaveRoomMessage))
		{
			TimerManager.In(0.5f, delegate()
			{
				UIToast.Show(GameManager.leaveRoomMessage, 3f);
				GameManager.leaveRoomMessage = string.Empty;
			});
		}
		UICamera.clickUIInput = true;
		RoundPlayersData.Init();
		BadWordsManager.Init();
		WeaponManager.Init();
		UISelectWeapon.AllWeapons = false;
		UISelectWeapon.SelectedUpdateWeaponManager = false;
		TimerManager.In(0.8f, delegate()
		{
			this.isExit = true;
		});
		LocalNotification.CancelNotification(1);
		LocalNotification.SendNotification(1, 259200L, "Block Strike", Localization.Get("NotificationText", true), Color.red, true, false, false, "app_icon", LocalNotification.NotificationExecuteMode.Inexact);
		PoolManager.ClearAll();
		DropWeaponManager.ClearScene();
		if (!AccountManager.isConnect)
		{
			EventManager.AddListener("AccountConnected", new EventManager.Callback(this.AccountConnected));
		}
    }

    private void AccountConnected()
	{
		this.CheckData();
		this.CheckCurrency();
	}

	private void CheckData()
	{
		int weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Rifle);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Rifle)
		{
			AccountManager.SetWeaponSelected(WeaponType.Rifle, 12);
			WeaponManager.SetSelectWeapon(WeaponType.Rifle, 12);
		}
		weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Pistol);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Pistol)
		{
			AccountManager.SetWeaponSelected(WeaponType.Pistol, 3);
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 3);
		}
		weaponSelected = AccountManager.GetWeaponSelected(WeaponType.Knife);
		if (!AccountManager.GetWeapon(weaponSelected) || WeaponManager.GetWeaponData(weaponSelected).Type != WeaponType.Knife)
		{
			AccountManager.SetWeaponSelected(WeaponType.Knife, 4);
			WeaponManager.SetSelectWeapon(WeaponType.Knife, 4);
		}
	}

	private void CheckCurrency()
	{
		if (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0)
		{
			if (WeaponManager.GetWeaponData(AccountManager.GetWeaponSelected(WeaponType.Knife)).Secret)
			{
				AccountManager.SetWeaponSelected(WeaponType.Knife, 4);
			}
			for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
			{
				AccountManager.SetWeaponSkinSelected(GameSettings.instance.Weapons[i].ID, 0);
			}
		}
	}

	public void ExitGame()
	{
		if (!this.isExit)
		{
			return;
		}
		mPopUp.ShowPopup(Localization.Get("ExitText", true), Localization.Get("Exit", true), Localization.Get("Yes", true), delegate()
		{
			Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll("Menu");
		});
	}

	public void ShowOthersGames()
	{
		Application.OpenURL("https://play.google.com/store/apps/dev?id=6363329851677974248");
	}

	public void ComingSoon()
	{
		UIToast.Show(Localization.Get("Coming soon", true));
	}

	public void OpenStore()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		mPanelManager.Show("Store", true);
	}

	public void AccountUpdate()
	{
		AccountManager.UpdateData(null);
	}

	public static void Stream(string url)
	{
		mPopUp.ShowPopup(Localization.Get("Stream Toast", true), Localization.Get("Watch the video", true), Localization.Get("Yes", true), delegate()
		{
			mPopUp.HideAll("Menu");
			Application.OpenURL(url);
		}, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll("Menu");
		});
	}

	public void ShowToast(string text)
	{
		UIToast.Show(text);
	}
}
