using System;
using System.Linq;
using UnityEngine;

public class mChangeName : MonoBehaviour
{

	public void ChangeName()
	{
		if (mPanelManager.select.name == "Weapons" || mPanelManager.select.name == "PlayerSkin")
		{
			return;
		}
		if (!AccountManager.isConnect)
		{
			if (AccountManager.GetLevel() < 3)
			{
				UIToast.Show(Localization.Get("Requires Level", true) + " 3");
			}
			else
			{
				UIToast.Show(Localization.Get("Connection account", true));
			}
			return;
		}
		mPopUp.ShowPopup(Localization.Get("Cost of change name 100 bs coins", true), Localization.Get("ChangeName", true), "Ok", new Action(this.ChangeNameInput), Localization.Get("Back", true), new Action(this.ChangeNameCancel));
	}

	private void ChangeNameInput()
	{
		mPopUp.HideAll();
		if (AccountManager.GetGold() < 100)
		{
			UIToast.Show(Localization.Get("Not enough money", true));
			return;
		}
		mPopUp.ShowInput(AccountManager.instance.Data.AccountName, Localization.Get("ChangeName", true), 12, UIInput.KeyboardType.Default, new Action(this.OnSubmit), new Action(this.OnChange), "Ok", new Action(this.OnYes), Localization.Get("Back", true), new Action(this.ChangeNameCancel));
	}

	private void ChangeNameCancel()
	{
		mPopUp.HideAll();
	}

	private void OnSubmit()
	{
		string text = mPopUp.GetInputText();
		if (text.Length <= 3 || text == "Null" || text[0].ToString() == " " || text[text.Length - 1].ToString() == " ")
		{
			text = "Player " + UnityEngine.Random.Range(0, 99999);
		}
		text = NGUIText.StripSymbols(text);
		mPopUp.SetInputText(text);
	}

	private void OnChange()
	{
		string inputText = mPopUp.GetInputText();
		string text = mChangeName.UpdateSymbols(inputText, true);
		if (inputText != text)
		{
			mPopUp.SetInputText(text);
		}
	}

	private void OnYes()
	{
		string text = mPopUp.GetInputText();
		string text2 = mChangeName.UpdateSymbols(text, true);
		if (text != text2)
		{
			mPopUp.SetInputText(text2);
			return;
		}
		if (text != NGUIText.StripSymbols(text))
		{
			mPopUp.SetInputText(NGUIText.StripSymbols(text));
			return;
		}
		if (text.Length <= 3 || text == "Null" || text[0].ToString() == " " || text[text.Length - 1].ToString() == " ")
		{
			text = "Player " + UnityEngine.Random.Range(0, 99999);
			mPopUp.SetInputText(text);
			return;
		}
		if (AccountManager.instance.Data.AccountName == text)
		{
			return;
		}
		AccountManager.UpdateGold(AccountManager.GetGold() - 100, null, null);
		AccountManager.SetGold(AccountManager.GetGold() - 100);
		mPopUp.HideAll("Menu");
		AccountManager.UpdateName(mPopUp.GetInputText(), new Action<string>(this.SetPlayerNameComplete), new Action<string>(this.SetPlayerNameError));
		mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
		EventManager.Dispatch("AccountUpdate");
	}

	private void SetPlayerNameComplete(string playerName)
	{
		AccountManager.instance.Data.AccountName = playerName;
		mPopUp.HideAll("Menu");
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		EventManager.Dispatch("AccountUpdate");
	}

	private void SetPlayerNameError(string error)
	{
		mPopUp.ShowInput(AccountManager.instance.Data.AccountName, Localization.Get("ChangeName", true), 12, UIInput.KeyboardType.Default, new Action(this.OnSubmit), new Action(this.OnChange), Localization.Get("Back", true), new Action(this.ChangeNameCancel), "Ok", new Action(this.OnYes));
		UIToast.Show(Localization.Get("Error", true) + ": " + error, 3f);
	}

	public static string UpdateSymbols(string text, bool isSymbol)
	{
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			if (mChangeName.CheckSymbol(text[i].ToString(), isSymbol))
			{
				text2 += text[i];
			}
		}
		return text2;
	}

	public static bool CheckSymbols(string text, bool isSymbol)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (!mChangeName.CheckSymbol(text[i].ToString(), isSymbol))
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckSymbol(string symbol, bool isSymbol)
	{
		string[] source = new string[]
		{
			"-",
			"_",
			"'",
			" ",
			"!",
			"@",
			"№",
			";",
			"%",
			"^",
			":",
			"&",
			"?",
			"*"
		};
		string[] source2 = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9"
		};
		string[] source3 = new string[]
		{
			"a",
			"e",
			"i",
			"o",
			"u",
			"y"
		};
		string[] source4 = new string[]
		{
			"b",
			"c",
			"d",
			"f",
			"g",
			"h",
			"j",
			"k",
			"l",
			"m",
			"n",
			"p",
			"q",
			"r",
			"s",
			"t",
			"v",
			"w",
			"x",
			"z"
		};
		symbol = symbol.ToLower();
		if (source.Contains(symbol))
		{
			return isSymbol;
		}
		return source2.Contains(symbol) || source3.Contains(symbol) || source4.Contains(symbol);
	}
}
