using System;
using UnityEngine;

public class UIControllerList : MonoBehaviour
{
    public UISprite FireSprite;

    public UISprite JumpSprite;

    public UISprite ReloadSprite;

    public UISprite AimSprite;

    public UISprite StatsSprite;

    public UISprite ChatSprite;

    public UISprite SelectWeaponSprite;

    public UISprite UseSprite;

    public UILabel UseLabel;

    public UISprite PauseSprite;

    public UISprite StoreSprite;

    public UISprite BombSprite;

    public UIPanel BuyWeaponsPanel;

    public InputJoystick JoystickInput;

    public InputTouchLook TouchLookInput;

    private static UIControllerList instance;

    public static UISprite Fire
	{
		get
		{
			return UIControllerList.instance.FireSprite;
		}
	}

	public static UISprite Jump
	{
		get
		{
			return UIControllerList.instance.JumpSprite;
		}
	}

	public static UISprite Reload
	{
		get
		{
			return UIControllerList.instance.ReloadSprite;
		}
	}

	public static UISprite Aim
	{
		get
		{
			return UIControllerList.instance.AimSprite;
		}
	}

	public static UISprite Stats
	{
		get
		{
			return UIControllerList.instance.StatsSprite;
		}
	}

	public static UISprite Chat
	{
		get
		{
			return UIControllerList.instance.ChatSprite;
		}
	}

	public static UISprite SelectWeapon
	{
		get
		{
			return UIControllerList.instance.SelectWeaponSprite;
		}
	}

	public static UISprite Use
	{
		get
		{
			return UIControllerList.instance.UseSprite;
		}
	}

	public static UILabel UseText
	{
		get
		{
			return UIControllerList.instance.UseLabel;
		}
	}

	public static UISprite Pause
	{
		get
		{
			return UIControllerList.instance.PauseSprite;
		}
	}

	public static UISprite Store
	{
		get
		{
			return UIControllerList.instance.StoreSprite;
		}
	}

	public static UISprite Bomb
	{
		get
		{
			return UIControllerList.instance.BombSprite;
		}
	}

	public static InputJoystick Joystick
	{
		get
		{
			return UIControllerList.instance.JoystickInput;
		}
	}
	public static InputTouchLook TouchLook
	{
		get
		{
			return UIControllerList.instance.TouchLookInput;
		}
	}

	public static UIPanel BuyWeapons
	{
		get
		{
			return UIControllerList.instance.BuyWeaponsPanel;
		}
	}

	private void Start()
	{
		UIControllerList.instance = this;
	}
}
