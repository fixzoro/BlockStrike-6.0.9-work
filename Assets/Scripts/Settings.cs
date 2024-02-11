using System;
using UnityEngine;

public class Settings
{
    public static int Font;

    public static bool FPSMeter;

    public static bool Console;

    public static bool Chat = true;

    public static bool ShowDamage;

    public static bool BulletHole = true;

    public static bool Blood = true;

    public static bool HitMarker = true;

    public static int ColorCrosshair;

    public static float Sensitivity = 0.2f;

    public static float Volume = 0.8f;

    public static bool Sound = true;

    public static bool AmbientSound = true;

    public static float ButtonAlpha = 1f;

    public static bool HUD = true;

    public static bool ShowWeapon = true;

    public static bool Shell = true;

    public static bool ProjectileEffect = true;

    public static bool FilterChat = true;

    public static bool DynamicJoystick = true;

    public static bool ShiftButton;

    public static bool ShowFirestat = true;

    public static bool ShowStickers = true;

    public static bool ShowAvatars = true;

    public static bool Clouds = true;

    public static void Load()
	{
		Settings.Font = Settings.GetInt("Font", 0);
		Settings.FPSMeter = Settings.GetBool("FPSMeter", false);
		Settings.Console = Settings.GetBool("Console", false);
		Settings.Chat = Settings.GetBool("Chat", true);
		Settings.ShowDamage = Settings.GetBool("ShowDamage", false);
		Settings.BulletHole = Settings.GetBool("BulletHole", true);
		Settings.Blood = Settings.GetBool("Blood", true);
		Settings.HitMarker = Settings.GetBool("HitMarker", true);
		Settings.ColorCrosshair = Settings.GetInt("ColorCrosshair", 0);
		Settings.Sensitivity = Settings.GetFloat("Sensitivity", 0.2f);
		Settings.Volume = Settings.GetFloat("Volume", 0.8f);
		Settings.Sound = Settings.GetBool("Sound", true);
		Settings.AmbientSound = Settings.GetBool("AmbientSound", true);
		Settings.ButtonAlpha = Mathf.Clamp(Settings.GetFloat("ButtonAlpha", 1f), 0.01f, 1f);
		Settings.HUD = Settings.GetBool("HUD", true);
		Settings.ShowWeapon = Settings.GetBool("ShowWeapon", true);
		Settings.Shell = Settings.GetBool("Shell", true);
		Settings.ProjectileEffect = Settings.GetBool("ProjectileEffect", true);
		Settings.FilterChat = Settings.GetBool("FilterChat", true);
		Settings.DynamicJoystick = Settings.GetBool("DynamicJoystick", true);
		Settings.ShiftButton = Settings.GetBool("ShiftButton", false);
		Settings.ShowFirestat = Settings.GetBool("ShowFirestat", true);
		Settings.ShowStickers = Settings.GetBool("ShowStickers", true);
		Settings.ShowAvatars = Settings.GetBool("ShowAvatars", true);
		Settings.Clouds = Settings.GetBool("Clouds", true);
		AudioListener.volume = Settings.Volume;
	}

	public static void Save()
	{
		Settings.SetInt("Font", Settings.Font);
		Settings.SetBool("FPSMeter", Settings.FPSMeter);
		Settings.SetBool("Console", Settings.Console);
		Settings.SetBool("Chat", Settings.Chat);
		Settings.SetBool("ShowDamage", Settings.ShowDamage);
		Settings.SetBool("BulletHole", Settings.BulletHole);
		Settings.SetBool("Blood", Settings.Blood);
		Settings.SetBool("HitMarker", Settings.HitMarker);
		Settings.SetInt("ColorCrosshair", Settings.ColorCrosshair);
		Settings.SetFloat("Sensitivity", Settings.Sensitivity);
		Settings.SetFloat("Volume", Settings.Volume);
		Settings.SetBool("Sound", Settings.Sound);
		Settings.SetBool("AmbientSound", Settings.AmbientSound);
		Settings.SetFloat("ButtonAlpha", Mathf.Clamp(Settings.ButtonAlpha, 0.01f, 1f));
		Settings.SetBool("HUD", Settings.HUD);
		Settings.SetBool("ShowWeapon", Settings.ShowWeapon);
		Settings.SetBool("Shell", Settings.Shell);
		Settings.SetBool("ProjectileEffect", Settings.ProjectileEffect);
		Settings.SetBool("FilterChat", Settings.FilterChat);
		Settings.SetBool("DynamicJoystick", Settings.DynamicJoystick);
		Settings.SetBool("ShiftButton", Settings.ShiftButton);
		Settings.SetBool("ShowFirestat", Settings.ShowFirestat);
		Settings.SetBool("ShowStickers", Settings.ShowStickers);
		Settings.SetBool("ShowAvatars", Settings.ShowAvatars);
		Settings.SetBool("Clouds", Settings.Clouds);
		AudioListener.volume = Settings.Volume;
	}

	private static bool GetBool(string key, bool defaultValue)
	{
		return nPlayerPrefs.GetBool(key, defaultValue);
	}

	private static int GetInt(string key, int defaultValue)
	{
		return nPlayerPrefs.GetInt(key, defaultValue);
	}

	private static float GetFloat(string key, float defaultValue)
	{
		return nPlayerPrefs.GetFloat(key, defaultValue);
	}

	private static void SetBool(string key, bool value)
	{
		nPlayerPrefs.SetBool(key, value);
	}

	private static void SetInt(string key, int value)
	{
		nPlayerPrefs.SetInt(key, value);
	}

	private static void SetFloat(string key, float value)
	{
		nPlayerPrefs.SetFloat(key, value);
	}
}
