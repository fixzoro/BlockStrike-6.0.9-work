using System;
using UnityEngine;
using FreeJSON;

[Serializable]
public class mClanCreate
{
    public GameObject panel;

    public UIInput tag;

    public UIInput name;

    public void OnSubmit()
	{
		UIInput current = UIInput.current;
		if (current == this.tag)
		{
			this.tag.value = mChangeName.UpdateSymbols(this.tag.value, true);
			if (this.tag.value.Length > 4 || this.tag.value.Length < 1)
			{
				this.tag.value = Utils.NameGenerator(3);
			}
		}
		if (current == this.name)
		{
			this.name.value = mChangeName.UpdateSymbols(this.name.value, true);
			if (this.name.value.Length > 15 || this.name.value.Length < 2)
			{
				this.name.value = Utils.NameGenerator(UnityEngine.Random.Range(5, 12));
			}
		}
	}

	public void CreateClan(Action complete, Action<string> error)
	{
		if (AccountManager.GetGold() < 250)
		{
			UIToast.Show(Localization.Get("Not enough money", true));
			return;
		}
        if(this.tag.value.ToUpper() == "DEV")
        {
            return;
        }
        mPopUp.ShowText(Localization.Get("Please wait", true) + "...");
        Firebase check = new Firebase();
        check.Auth = AccountManager.AccountToken;
        check.Child("Clans").Child(this.tag.value.ToUpper()).GetValue(delegate (string result)
        {
            if (result == "null")
            {
                this.tag.value = mChangeName.UpdateSymbols(this.tag.value.ToUpper(), true);
                this.name.value = mChangeName.UpdateSymbols(this.name.value, true);
                if (this.tag.value.Length > 4 || this.tag.value.Length < 2)
                {
                    return;
                }
                if (this.name.value.Length > 12 || this.tag.value.Length < 2)
                {
                    return;
                }
                AccountManager.SetClan(this.tag.value.ToUpper());
                AccountManager.SetClanFirebase(this.tag.value.ToUpper());
                AccountManager.UpdateGold(AccountManager.GetGold() - 250, null, null);
                AccountManager.SetGold(AccountManager.GetGold() - 250);

                JsonObject players = new JsonObject();
                players.Add(AccountManager.instance.Data.ID.ToString(), 0);

                JsonObject jsonObject = new JsonObject();
                jsonObject.Add("a", AccountManager.instance.Data.ID.ToString());
                jsonObject.Add("n", this.name.value);
                jsonObject.Add("t", this.tag.value.ToUpper());
                jsonObject.Add("p", players);

                Firebase firebase = new Firebase();
                firebase.Auth = AccountManager.AccountToken;
                firebase.Child("Clans").Child(AccountManager.GetClan()).SetValue(jsonObject.ToString());

                EventManager.Dispatch("AccountUpdate");
                mPopUp.HideAll();
                mPanelManager.Hide();
                mPanelManager.Show("Menu", true);
            }
            else
            {
                mPopUp.HideAll();
                mPanelManager.Hide();
                mPanelManager.Show("Menu", true);
                error("Tag already taken");
            }
        }, delegate (string fail)
        {
            UIToast.Show("Create Clan Error: " + fail);
        });
	}
}
