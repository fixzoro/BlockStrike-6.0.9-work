using System;
using UnityEngine;

public class mLanguage : MonoBehaviour
{
    public mLanguage.LanguageClass[] languages;

    public UITexture Button;

    private int selectLanguage;

    private void Awake()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			this.SetLanguage(PlayerPrefs.GetString("Language"));
		}
		else if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian)
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

	private void SetLanguage(string language)
	{
		for (int i = 0; i < this.languages.Length; i++)
		{
			if (this.languages[i].language == language)
			{
				Localization.language = this.languages[i].language;
				this.Button.mainTexture = this.languages[i].Texture;
				this.selectLanguage = i;
				break;
			}
		}
	}

	public void SelectLanguage()
	{
		if (this.selectLanguage < this.languages.Length - 1)
		{
			this.selectLanguage++;
		}
		else
		{
			this.selectLanguage = 0;
		}
		mLanguage.LanguageClass languageClass = this.languages[this.selectLanguage];
		Localization.language = languageClass.language;
		this.Button.mainTexture = languageClass.Texture;
	}

	[Serializable]
	public class LanguageClass
	{
		public string language;

		public Texture2D Texture;
	}
}
