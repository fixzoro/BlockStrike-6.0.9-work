using System;
using UnityEngine;

public class MainTutorial : MonoBehaviour
{
    public Transform Spawn;

    [Header("UI")]
    public GameObject CrosshairUI;

    public GameObject JoystickZone;

    public GameObject LookZone;

    [Header("Targets")]
    public TutorialTarget Target1;

    public TutorialTarget Target2;

    public TutorialTarget Target3;

    [Header("Weapons")]
    public GameObject Rifle;

    public GameObject Pistol;

    private void Start()
	{
		TimerManager.In(0.05f, delegate()
		{
			this.UpdateLanguage();
			UIPanelManager.ShowPanel("Display");
			InputManager.Init();
			WeaponManager.Init();
			GameManager.changeWeapons = false;
			this.SetWeapons();
			this.CreatePlayer();
			UIScore.SetActiveScore(false, 0);
			this.StartButtons();
			this.CrosshairUI.SetActive(false);
			this.Tutorial01();
		});
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_1));
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_3));
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_4));
	}

	private void CreatePlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(0);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(this.Spawn.position, this.Spawn.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Knife);
		player.PlayerWeapon.InfiniteAmmo = true;
	}

	private void SetWeapons()
	{
		WeaponManager.SetSelectWeapon(WeaponType.Knife, 0);
		WeaponManager.SetSelectWeapon(WeaponType.Pistol, 0);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, 0);
	}

	private void StartButtons()
	{
		UIControllerList.Fire.cachedGameObject.SetActive(false);
		UIControllerList.Reload.cachedGameObject.SetActive(false);
		UIControllerList.Jump.cachedGameObject.SetActive(false);
		UIControllerList.SelectWeapon.cachedGameObject.SetActive(false);
		UIControllerList.Aim.cachedGameObject.SetActive(false);
		UIControllerList.Chat.cachedGameObject.SetActive(false);
		UIControllerList.Stats.cachedGameObject.SetActive(false);
		UIControllerList.Pause.cachedGameObject.SetActive(false);
		UIControllerList.Joystick.gameObject.SetActive(false);
		UIControllerList.TouchLook.gameObject.SetActive(false);
	}

	private void Tutorial01()
	{
		TimerManager.In(1f, delegate()
		{
			TweenAlpha.Begin(this.JoystickZone, 1f, 0.5f, 0f);
			UIControllerList.Joystick.gameObject.SetActive(true);
			UIToast.Show(Localization.Get("Use the joystick to move", true), 3f);
			this.SetWeapons();
		});
	}

	public void Tutorial02()
	{
		TweenAlpha.Begin(this.JoystickZone, 1f, 0f, 0f);
        TweenAlpha.Begin(this.LookZone, 1f, 0.5f, 0f);
		UIControllerList.TouchLook.gameObject.SetActive(true);
		UIToast.Show(Localization.Get("Use the right part of the screen to look around", true), 3f);
	}

	public void Tutorial03()
	{
		TweenAlpha.Begin(this.LookZone, 1f, 0f, 0f);
		UIControllerList.Jump.cachedGameObject.SetActive(true);
		UIToast.Show(Localization.Get("Use the jump to pass the obstacles", true), 3f);
	}

	public void Tutorial04()
	{
		UIToast.Show(Localization.Get("Climb the ladder", true), 3f);
	}

	public void Tutorial05()
	{
		UIToast.Show(Localization.Get("Go to the weapon to pick up", true), 3f);
	}

	public void Tutorial06()
	{
		this.Rifle.SetActive(false);
		this.Pistol.SetActive(false);
		WeaponManager.SetSelectWeapon(WeaponType.Rifle, 1);
		WeaponCustomData weaponCustomData = new WeaponCustomData();
		weaponCustomData.Skin = 0;
		PlayerInput.instance.PlayerWeapon.UpdateWeapon(WeaponType.Rifle, true, weaponCustomData);
		TimerManager.In(1f, delegate()
		{
			InputManager.SetButtonDown("Reload");
			TimerManager.In(2f, delegate()
			{
				UIControllerList.Fire.cachedGameObject.SetActive(true);
				this.CrosshairUI.SetActive(true);
				UIToast.Show(Localization.Get("Press the button to fire", true), 3f);
				InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_1));
			});
		});
	}

	private void Tutorial06_1(string button)
	{
		if (button != "Fire")
		{
			return;
		}
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_1));
		TimerManager.In(0.5f, delegate()
		{
			UIToast.Show(Localization.Get("Shoot at targets", true), 3f);
			this.Target1.SetActive(true);
		});
	}

	public void DeactivedTarget(int target)
	{
		switch (target)
		{
		case 1:
			this.Target1.SetActive(false);
			this.Target2.SetActive(true);
			break;
		case 2:
			this.Target2.SetActive(false);
			this.Target3.SetActive(true);
			break;
		case 3:
			this.Target3.SetActive(false);
			break;
		}
		if (!this.Target1.GetActive() && !this.Target2.GetActive() && !this.Target3.GetActive())
		{
			this.Tutorial06_2();
		}
	}

	private void Tutorial06_2()
	{
		UIControllerList.Reload.cachedGameObject.SetActive(true);
		UIToast.Show(Localization.Get("Press the button to reload", true), 3f);
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_3));
	}

	private void Tutorial06_3(string button)
	{
		if (button != "Reload")
		{
			return;
		}
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_3));
		TimerManager.In(2.5f, delegate()
		{
			WeaponManager.SetSelectWeapon(WeaponType.Pistol, 2);
			WeaponCustomData weaponCustomData = new WeaponCustomData();
			weaponCustomData.Skin = 0;
			PlayerInput.instance.PlayerWeapon.UpdateWeapon(WeaponType.Pistol, true, weaponCustomData);
			UIToast.Show(Localization.Get("Switch weapons", true), 3f);
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_4));
		});
	}

	private void Tutorial06_4(string button)
	{
		if (button != "SelectWeapon")
		{
			return;
		}
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.Tutorial06_4));
		TimerManager.In(1f, delegate()
		{
			UIToast.Show(Localization.Get("Training completed", true), 3f);
			PlayerPrefs.SetInt("Tutorial", 1);
			TimerManager.In(5f, delegate()
			{
                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                PlayerInput.instance.cursor = true;
                #endif
                PhotonNetwork.LeaveRoom(true);
			});
		});
	}

	public void Skip()
	{
		PlayerPrefs.SetInt("Tutorial", 1);
        TimerManager.In(0.2f, delegate()
		{
            PhotonNetwork.LeaveRoom(true);
		});
	}

	private void UpdateLanguage()
	{
		if (!PlayerPrefs.HasKey("Language"))
		{
			if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
			{
				this.SetLanguage("Russia");
			}
			else if (Application.systemLanguage == SystemLanguage.English)
			{
				this.SetLanguage("English");
			}
			else if (Application.systemLanguage == SystemLanguage.Korean)
			{
				this.SetLanguage("Korean");
			}
			else if (Application.systemLanguage == SystemLanguage.Spanish)
			{
				this.SetLanguage("Spanish");
			}
			else if (Application.systemLanguage == SystemLanguage.Portuguese)
			{
				this.SetLanguage("Portuguese");
			}
			else if (Application.systemLanguage == SystemLanguage.French)
			{
				this.SetLanguage("French");
			}
			else if (Application.systemLanguage == SystemLanguage.Japanese)
			{
				this.SetLanguage("Japan");
			}
			else if (Application.systemLanguage == SystemLanguage.Polish)
			{
				this.SetLanguage("Polish");
			}
		}
	}

	private void SetLanguage(string language)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (Localization.knownLanguages[i] == language)
			{
				Localization.language = Localization.knownLanguages[i];
				break;
			}
		}
	}
}
