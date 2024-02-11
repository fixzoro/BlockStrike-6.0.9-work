using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class mAccount : MonoBehaviour
{
	// Token: 0x06001560 RID: 5472 RVA: 0x00077EB6 File Offset: 0x000760B6
	public void Start()
	{
		if (!AccountManager.isConnect)
		{
			this.Start2();
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000047F6 File Offset: 0x000029F6
	public static void Reconnect()
	{
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x00077EC8 File Offset: 0x000760C8
	private void Start2()
	{
		string playerID = string.Empty;
		string androidID = AndroidNativeFunctions.GetAndroidID();
		playerID = androidID;
		playerID = playerID.Replace(".", "*");
		mPopUp.SetActiveWait(true, Localization.Get("Connect to the account", true));
		playerID = playerID.ToLower();
		TimerManager.In(0.5f, delegate ()
		{
			AccountManager.Login(playerID, new Action<bool>(this.Login), new Action<string>(this.LoginError));
		});
		TimerManager.In(1.5f, delegate ()
		{
			AccountManager.isConnect = true;
		});
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x00077F78 File Offset: 0x00076178
	private void Login(bool isCreated)
	{
		if (isCreated)
		{
			mPopUp.SetActiveWait(false);
			EventManager.Dispatch("AccountUpdate");
			WeaponManager.UpdateData();
			if (string.IsNullOrEmpty(AccountManager.instance.Data.AccountName) || AccountManager.instance.Data.AccountName.ToString() == " ")
			{
				this.SetPlayerName(delegate (string playerName)
				{
					AccountManager.UpdateName(playerName, new Action<string>(this.SetPlayerNameComplete), new Action<string>(this.SetPlayerNameError));
					mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
				});
				return;
			}
		}
		else
		{
			mPopUp.SetActiveWait(false);
			mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
			this.SetPlayerName(delegate (string playerName)
			{
				AccountManager.Register(playerName, new Action(this.RegisterComplete), new Action<string>(this.RegisterError));
				mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
			});
		}
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00078024 File Offset: 0x00076224
	private void LoginError(string error)
	{
		mPopUp.ShowPopup(Localization.Get("Error", true) + ": " + error, "Account", Localization.Get("Connect", true), new Action(this.LoginErrorTry), Localization.Get("Exit", true), new Action(mAccount.Exit));
		mPopUp.SetActiveWait(false);
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00078088 File Offset: 0x00076288
	private void LoginErrorTry()
	{
		if (mPopUp.activePopup)
		{
			mPopUp.HideAll("Menu");
		}
		AccountManager.Login(AccountManager.AccountID, new Action<bool>(this.Login), new Action<string>(this.LoginError));
		string text = AccountManager.AccountID;
		text = text.Remove(text.LastIndexOf("@"));
		mPopUp.SetActiveWait(true, Localization.Get("Connect to the account", true) + ": " + text);
	}

	// Token: 0x06001566 RID: 5478 RVA: 0x00078106 File Offset: 0x00076306
	private void RegisterComplete()
	{
		EventManager.Dispatch("AccountUpdate");
		WeaponManager.UpdateData();
		mPopUp.HideAll("Menu");
	}

	// Token: 0x06001567 RID: 5479 RVA: 0x00078124 File Offset: 0x00076324
	private void RegisterError(string error)
	{
		mPopUp.ShowPopup(Localization.Get("Error", true) + ": " + error, "Account", Localization.Get("Connect", true), new Action(this.RegisterErrorTry), Localization.Get("Exit", true), new Action(mAccount.Exit));
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x0007817F File Offset: 0x0007637F
	private void RegisterErrorTry()
	{
		if (mPopUp.activePopup)
		{
			mPopUp.HideAll("Menu");
		}
		this.SetPlayerName(delegate (string playerName)
		{
			AccountManager.Register(playerName, new Action(this.RegisterComplete), new Action<string>(this.RegisterError));
			mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
		});
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x000781A4 File Offset: 0x000763A4
	private void SetPlayerName(Action<string> action)
	{
		mPopUp.SetActiveWait(false);
		mPopUp.ShowInput(string.Empty, Localization.Get("ChangeName", true), 12, UIInput.KeyboardType.Default, new Action(this.SetPlayerNameSubmit), new Action(this.SetPlayerNameChange), Localization.Get("Back", true), null, "Ok", delegate
		{
			this.SetPlayerNameSave(action);
		});
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00078218 File Offset: 0x00076418
	private void SetPlayerNameSave(Action<string> action)
	{
		string text = NGUIText.StripSymbols(mPopUp.GetInputText());
		string text2 = mChangeName.UpdateSymbols(text, true);
		if (text != text2)
		{
			mPopUp.SetInputText(text2);
			mAccount.Name = text;
			return;
		}
		if (text.Length > 3 && !(text == "Null") && !(text[0].ToString() == " ") && !(text[text.Length - 1].ToString() == " "))
		{
			if (action != null)
			{
				mAccount.Name = text;
				action(text);
			}
			return;
		}
		text = "Player " + UnityEngine.Random.Range(0, 99999).ToString();
		mPopUp.SetInputText(text);
		mAccount.Name = text;
	}

	// Token: 0x0600156B RID: 5483 RVA: 0x000782DC File Offset: 0x000764DC
	private void SetPlayerNameSubmit()
	{
		string text = mPopUp.GetInputText();
		if (text.Length <= 3 || text == "Null" || text[0].ToString() == " " || text[text.Length - 1].ToString() == " ")
		{
			text = "Player " + UnityEngine.Random.Range(0, 99999).ToString();
		}
		text = NGUIText.StripSymbols(text);
		mPopUp.SetInputText(text);
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00078370 File Offset: 0x00076570
	private void SetPlayerNameChange()
	{
		string inputText = mPopUp.GetInputText();
		string text = mChangeName.UpdateSymbols(inputText, true);
		if (inputText != text)
		{
			mPopUp.SetInputText(text);
		}
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00078106 File Offset: 0x00076306
	private void SetPlayerNameComplete(string playerName)
	{
		EventManager.Dispatch("AccountUpdate");
		WeaponManager.UpdateData();
		mPopUp.HideAll("Menu");
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x00078398 File Offset: 0x00076598
	private void SetPlayerNameError(string error)
	{
		this.SetPlayerName(delegate (string playerName)
		{
			AccountManager.UpdateName(playerName, new Action<string>(this.SetPlayerNameComplete), new Action<string>(this.SetPlayerNameError));
			mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
		});
		string text = error;
		if (text == "Name already taken")
		{
			text = Localization.Get("Name already taken", true);
		}
		UIToast.Show(Localization.Get("Error", true) + ": " + text, 3f);
	}

	// Token: 0x0600156F RID: 5487 RVA: 0x0005070B File Offset: 0x0004E90B
	public static void Exit()
	{
		Application.Quit();
	}

	// Token: 0x04000EC8 RID: 3784
	public CryptoString WebClientID;

	// Token: 0x04000EC9 RID: 3785
	public static int NameSetted;

	// Token: 0x04000ECA RID: 3786
	public static string Name;

	// Token: 0x04000ECB RID: 3787
	public static bool isAdmin = true;
}
