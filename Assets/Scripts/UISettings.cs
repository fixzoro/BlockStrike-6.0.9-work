using System;
using UnityEngine;

public class UISettings : MonoBehaviour
{
    [Header("General")]
    public UILabel fontLabel;

    private int selectFont;

    public UILabel languageLabel;

    private static int selectLanguage;

    [Header("Control")]
    public UISlider Sensitivity;

    public UILabel SensitivityLabel;

    public UISlider ButtonAlpha;

    public UILabel ButtonAlphaLabel;

    public UIToggle ShiftButton;

    [Header("Sound")]
    public UISlider Volume;

    public UILabel VolumeLabel;

    public UIToggle Sound;

    public UIToggle AmbientSound;

    [Header("Others")]
    public UIToggle FPSMeter;

    public UIToggle Chat;

    public UIToggle Console;

    public UIToggle ShowDamage;

    public UIToggle BulletHole;

    public UIToggle Blood;

    public UIToggle HitMarker;

    public UIToggle HUD;

    public UIToggle ShowWeapon;

    public UIToggle Shell;

    public UIToggle ProjectileEffect;

    public UIToggle FilterChat;

    public UIToggle ShowFirestat;

    public UIToggle ShowStickers;

    public UIToggle ShowAvatars;

    public UIToggle Clouds;

    public UITable table;

    private void Start()
	{
		this.Load();
	}

	private void Load()
	{
		Settings.Load();
		UISettings.UpdateLanguage();
		this.selectFont = Settings.Font;
		this.fontLabel.text = Localization.Get("Font", true) + ": " + UIFontManager.GetFonts()[this.selectFont].name.Split(new char[]
		{
			"-"[0]
		})[1];
		this.languageLabel.text = Localization.Get("Language", true) + ": " + Localization.Get("LanguageType", true);
		this.FPSMeter.value = Settings.FPSMeter;
		this.Chat.value = Settings.Chat;
		this.Console.value = Settings.Console;
		this.UpdateConsole();
		this.ShowDamage.value = Settings.ShowDamage;
		this.BulletHole.value = Settings.BulletHole;
		this.Blood.value = Settings.Blood;
		this.HitMarker.value = Settings.HitMarker;
		this.HUD.value = Settings.HUD;
		this.ShowWeapon.value = Settings.ShowWeapon;
		this.Shell.value = Settings.Shell;
		this.ProjectileEffect.value = Settings.ProjectileEffect;
		this.FilterChat.value = Settings.FilterChat;
		this.ShowFirestat.value = Settings.ShowFirestat;
		this.ShowStickers.value = Settings.ShowStickers;
		this.ShowAvatars.value = Settings.ShowAvatars;
		this.Clouds.value = Settings.Clouds;
		this.UpdateSensitivity();
		this.UpdateButtonAlpha();
		this.ShiftButton.value = Settings.ShiftButton;
		this.UpdateVolume();
		this.Sound.value = Settings.Sound;
		this.AmbientSound.value = Settings.AmbientSound;
	}

	public void Save()
	{
		Settings.Font = this.selectFont;
		Settings.FPSMeter = this.FPSMeter.value;
		Settings.Chat = this.Chat.value;
		Settings.Console = this.Console.value;
		this.UpdateConsole();
		Settings.ShowDamage = this.ShowDamage.value;
		Settings.BulletHole = this.BulletHole.value;
		Settings.Blood = this.Blood.value;
		Settings.HitMarker = this.HitMarker.value;
		Settings.HUD = this.HUD.value;
		Settings.ShowWeapon = this.ShowWeapon.value;
		Settings.Shell = this.Shell.value;
		Settings.ProjectileEffect = this.ProjectileEffect.value;
		Settings.FilterChat = this.FilterChat.value;
		Settings.ShowFirestat = this.ShowFirestat.value;
		Settings.ShowStickers = this.ShowStickers.value;
		Settings.ShowAvatars = this.ShowAvatars.value;
		Settings.Clouds = this.Clouds.value;
		Settings.ShiftButton = this.ShiftButton.value;
		Settings.Sound = this.Sound.value;
		Settings.AmbientSound = this.AmbientSound.value;
		Settings.Save();
		EventManager.Dispatch("OnSettings");
	}

	public void Default()
	{
		Settings.Font = 0;
		Settings.FPSMeter = false;
		Settings.Chat = true;
		Settings.Console = false;
		this.UpdateConsole();
		Settings.ShowDamage = false;
		Settings.BulletHole = true;
		Settings.Blood = true;
		Settings.HitMarker = true;
		Settings.HUD = true;
		Settings.ShowWeapon = true;
		Settings.FilterChat = true;
		Settings.Shell = true;
		Settings.ProjectileEffect = true;
		Settings.ShowFirestat = true;
		Settings.ShowStickers = true;
		Settings.ShowAvatars = true;
		Settings.Clouds = true;
		Settings.Sensitivity = 0.2f;
		Settings.ButtonAlpha = 1f;
		Settings.DynamicJoystick = true;
		Settings.ShiftButton = false;
		Settings.Volume = 0.8f;
		Settings.Sound = true;
		Settings.AmbientSound = true;
		Settings.Save();
		this.Load();
		EventManager.Dispatch("OnSettings");
	}

	public static void UpdateLanguage()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			UISettings.SetLanguage(PlayerPrefs.GetString("Language"));
		}
		else if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
		{
			UISettings.SetLanguage("Russia");
		}
		else if (Application.systemLanguage == SystemLanguage.English)
		{
			UISettings.SetLanguage("English");
		}
		else if (Application.systemLanguage == SystemLanguage.Korean)
		{
			UISettings.SetLanguage("Korean");
		}
		else if (Application.systemLanguage == SystemLanguage.Spanish)
		{
			UISettings.SetLanguage("Spanish");
		}
		else if (Application.systemLanguage == SystemLanguage.Portuguese)
		{
			UISettings.SetLanguage("Portuguese");
		}
		else if (Application.systemLanguage == SystemLanguage.French)
		{
			UISettings.SetLanguage("French");
		}
		else if (Application.systemLanguage == SystemLanguage.Japanese)
		{
			UISettings.SetLanguage("Japan");
		}
		else if (Application.systemLanguage == SystemLanguage.Polish)
		{
			UISettings.SetLanguage("Polish");
		}
	}

	private static void SetLanguage(string language)
	{
		for (int i = 0; i < Localization.knownLanguages.Length; i++)
		{
			if (Localization.knownLanguages[i] == language)
			{
				Localization.language = Localization.knownLanguages[i];
				UISettings.selectLanguage = i;
				break;
			}
		}
	}

	public void NextLanguage()
	{
		UISettings.selectLanguage++;
		if (UISettings.selectLanguage > Localization.knownLanguages.Length - 1)
		{
			UISettings.selectLanguage = 0;
		}
		UISettings.SetLanguage(Localization.knownLanguages[UISettings.selectLanguage]);
		this.table.repositionNow = true;
		this.languageLabel.text = Localization.Get("Language", true) + ": " + Localization.Get("LanguageType", true);
	}

	public void LastLanguage()
	{
		UISettings.selectLanguage--;
		if (UISettings.selectLanguage < 0)
		{
			UISettings.selectLanguage = Localization.knownLanguages.Length - 1;
		}
		UISettings.SetLanguage(Localization.knownLanguages[UISettings.selectLanguage]);
		this.table.repositionNow = true;
		this.languageLabel.text = Localization.Get("Language", true) + ": " + Localization.Get("LanguageType", true);
	}

	public void NextFont()
	{
		this.selectFont++;
		if (this.selectFont > UIFontManager.GetFonts().Length - 1)
		{
			this.selectFont = 0;
		}
		UIFontManager.SetFont(this.selectFont);
		this.fontLabel.text = Localization.Get("Font", true) + ": " + UIFontManager.GetFonts()[this.selectFont].name.Split(new char[]
		{
			"-"[0]
		})[1];
	}

	public void LastFont()
	{
		this.selectFont--;
		if (this.selectFont < 0)
		{
			this.selectFont = UIFontManager.GetFonts().Length - 1;
		}
		UIFontManager.SetFont(this.selectFont);
		this.fontLabel.text = Localization.Get("Font", true) + ": " + UIFontManager.GetFonts()[this.selectFont].name.Split(new char[]
		{
			"-"[0]
		})[1];
	}

	private void UpdateConsole()
	{
		GameConsole.actived = Settings.Console;
	}

	private void UpdateSensitivity()
	{
		this.Sensitivity.value = Settings.Sensitivity;
		this.SensitivityLabel.text = string.Concat(new object[]
		{
			Localization.Get("Sensitivity", true),
			": ",
			Mathf.RoundToInt(this.Sensitivity.value * 100f),
			"%"
		});
	}

	public void SetSensitivity()
	{
		Settings.Sensitivity = this.Sensitivity.value;
		this.SensitivityLabel.text = string.Concat(new object[]
		{
			Localization.Get("Sensitivity", true),
			": ",
			Mathf.RoundToInt(this.Sensitivity.value * 100f),
			"%"
		});
	}

	private void UpdateButtonAlpha()
	{
		this.ButtonAlpha.value = Settings.ButtonAlpha;
		this.ButtonAlphaLabel.text = string.Concat(new object[]
		{
			Localization.Get("Button Alpha", true),
			": ",
			Mathf.RoundToInt(this.ButtonAlpha.value * 100f),
			"%"
		});
	}

	public void SetButtonAlpha()
	{
		Settings.ButtonAlpha = Mathf.Clamp(this.ButtonAlpha.value, 0.01f, 1f);
		this.ButtonAlphaLabel.text = string.Concat(new object[]
		{
			Localization.Get("Button Alpha", true),
			": ",
			Mathf.RoundToInt(this.ButtonAlpha.value * 100f),
			"%"
		});
	}

	private void UpdateVolume()
	{
		this.Volume.value = Settings.Volume;
		this.VolumeLabel.text = string.Concat(new object[]
		{
			Localization.Get("Volume", true),
			": ",
			Mathf.RoundToInt(this.Volume.value * 100f),
			"%"
		});
	}

	public void SetVolume()
	{
		Settings.Volume = this.Volume.value;
		this.VolumeLabel.text = string.Concat(new object[]
		{
			Localization.Get("Volume", true),
			": ",
			Mathf.RoundToInt(this.Volume.value * 100f),
			"%"
		});
	}

	public void Optimization(UIToggle toggle)
	{
		if (toggle.value)
		{
			UIToast.Show(Localization.Get("Optimization", true) + " +");
		}
	}

	public void OnDefaultButton()
	{
		EventManager.Dispatch("OnDefaultButton");
	}

	public void OnSaveButton()
	{
		EventManager.Dispatch("OnSaveButton");
	}
}
