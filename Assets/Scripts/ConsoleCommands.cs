using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{
	[Console(new string[] {"game_quit"} )]
	private static void OnGameQuit()
	{
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.LeaveRoom(true);
		}
		Application.Quit();
	}

	[Console(new string[] {"set_player_ragdoll_force"} )]
	private static void OnPlayerForceRagdoll(int value)
	{
		int num = Mathf.Clamp(value, 0, 2000);
		PlayerSkinRagdoll.force = (float)num;
	}

	[Console(new string[] {"leave_room"} )]
	private static void OnLeaveRoom()
	{
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.LeaveRoom(true);
		}
	}

	[Console(new string[] {"game_info"} )]
	private static void OnGameInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(VersionManager.bundleVersion);
		stringBuilder.AppendLine(VersionManager.bundleVersionCode.ToString());
		Debug.Log(stringBuilder.ToString());
	}

	[Console(new string[] {"texture_quality"} )]
	private static void OnTextureQuality(int value)
	{
		QualitySettings.globalTextureMipmapLimit = Mathf.Clamp(value, 0, 3);
		Debug.Log("Texture Quality: " + value + " [0-3]");
	}

	[Console(new string[] {"show_all_stats"} )]
	private static void OnShowAllStats(bool value)
	{
		FieldInfo field = typeof(UIFramesPerSecond).GetField("allStats", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("show_all_stats", value);
	}

	[Console(new string[] {"auto_show_error"} )]
	private static void OnAutoShowError(bool value)
	{
		FieldInfo field = typeof(ConsoleManager).GetField("autoErrorShow", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("auto_error_show", value);
	}

	[Console(new string[] {"set_time_day"} )]
	private static void OnSetTimeDay(int value)
	{
		if (LevelManager.GetSceneName() != "Shooting Range")
		{
			Debug.Log("Only level: Shooting Range");
			return;
		}
		SkyboxManager skyboxManager = UnityEngine.Object.FindObjectOfType<SkyboxManager>();
		skyboxManager.TimeDay = 100f / (float)Mathf.Clamp(value, 0, 100);
		skyboxManager.SkyboxMaterial.mainTextureOffset = new Vector2(skyboxManager.TimeDay, 0f);
	}

	[Console(new string[] {"show_player_model"} )]
	private static void OnCustomShowPlayerModel(bool value)
	{
		if (LevelManager.GetSceneName() != "Shooting Range")
		{
			Debug.Log("Only level: Shooting Range");
			return;
		}
		ShootingRangeManager shootingRangeManager = UnityEngine.Object.FindObjectOfType<ShootingRangeManager>();
		shootingRangeManager.SendMessage("ShowPlayerModel", value);
	}

	[Console(new string[] { "custom_player_head_skin", "custom_player_body_skin", "custom_player_legs_skin"} )]
	private static void OnSetCustomPlayerHeadSkins(string value)
	{
		if (LevelManager.GetSceneName() != "Shooting Range")
		{
			Debug.Log("Only level: Shooting Range");
			return;
		}
		ShootingRangeManager shootingRangeManager = UnityEngine.Object.FindObjectOfType<ShootingRangeManager>();
		shootingRangeManager.SendMessage("SetCustomPlayerSkins", value);
	}

	[Console(new string[] {"custom_weapon_skin"} )]
	private static void OnCustomWeaponSkin(string value)
	{
		if (LevelManager.GetSceneName() != "Shooting Range")
		{
			Debug.Log("Only level: Shooting Range");
			return;
		}
		if (GameManager.player == null)
		{
			return;
		}
		FPWeaponShooter script = GameManager.player.PlayerWeapon.GetSelectedWeaponData().Script;
		if (script == null)
		{
			return;
		}
		script.SendMessage("UpdateCustomSkin", value);
	}

	[Console(new string[] {"chat_show_duration"} )]
	private static void OnChatShowDuration(float value)
	{
		value = Mathf.Clamp(value, 1f, 20f);
		FieldInfo field = typeof(UIChat).GetField("textShowDuration", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("chat_show_duration", value);
	}

	[Console(new string[] {"show_player_name"} )]
	private static void OnShowPlayerName(bool value)
	{
		FieldInfo field = typeof(UINameManager).GetField("showPlayerName", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("show_player_name", value);
	}

	[Console(new string[] {"weapon_fov"} )]
	private static void OnWeaponFOV(float value)
	{
		value = Mathf.Clamp(value, -15f, 15f);
		FieldInfo field = typeof(vp_FPWeapon).GetField("customRenderingFieldOfView", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		if (GameManager.player == null)
		{
			return;
		}
		FPWeaponShooter script = GameManager.player.PlayerWeapon.GetSelectedWeaponData().Script;
		if (script == null)
		{
			return;
		}
		script.FPWeapon.SnapZoom();
		GameConsole.Save("weapon_fov", value);
	}

	[Console(new string[] {"fix_touch_look"} )]
	private static void OnFixTouchLook(bool value)
	{
		FieldInfo field = typeof(InputTouchLook).GetField("fixTouch", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("fix_touch_look", value);
	}

	[Console(new string[] {"weapon_bob"} )]
	private static void OnWeaponBob(bool value)
	{
		FieldInfo field = typeof(vp_FPWeapon).GetField("customBob", BindingFlags.Static | BindingFlags.NonPublic);
		field.SetValue(null, value);
		GameConsole.Save("weapon_bob", value);
	}

	[Console(new string[] {"weapon_left_hand"} )]
	private static void OnWeaponLeftHand(bool value)
	{
		WeaponCameraFlip.OnFlip(value);
		GameConsole.Save("weapon_left_hand", value);
	}

	[Console(new string[] {"thread_enable"} )]
	private static void OnThreadEnable(bool value)
	{
		Debug.Log("Restart the game to make the settings work");
		GameConsole.Save("thread_enable", value);
	}
}
