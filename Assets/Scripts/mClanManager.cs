using System;
using FreeJSON;
using UnityEngine;

public class mClanManager : MonoBehaviour
{
    public mClanCreate createClan;

    public mClanChat chatClan;

    public mClanPlayers playersClan;

    private bool loadData;

    private void Start()
    {
        EventManager.AddListener("AccountConnected", delegate 
        {
            if (!this.loadData)
            {
                AccountManager.Clan.GetData(new Action<string>(this.OnGetDataComplete), new Action<string>(this.OnGetDataError));
            }
        });
    }

    public void Open()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		mPanelManager.Show("Clan", true);
		ClanManager.Load();
		if (string.IsNullOrEmpty(AccountManager.GetClan()))
		{
			this.createClan.panel.SetActive(true);
			this.chatClan.panel.SetActive(false);
		}
		else
		{
			this.chatClan.panel.SetActive(true);
			this.createClan.panel.SetActive(false);
			this.chatClan.Open();
			if (!this.loadData)
			{
				AccountManager.Clan.GetData(new Action<string>(this.OnGetDataComplete), new Action<string>(this.OnGetDataError));
			}
		}
	}

	public void Close()
	{
		this.chatClan.Close();
	}

	public void OnSubmit()
	{
		this.createClan.OnSubmit();
		this.chatClan.OnSubmit();
	}

	public void OnCreateClan()
	{
		this.createClan.CreateClan(new Action(this.OnCreateClanComplete), new Action<string>(this.OnCreateClanError));
	}

	public void OpenPlayers()
	{
		this.playersClan.panel.SetActive(true);
		this.chatClan.panel.SetActive(false);
		this.playersClan.Open();
	}

	public void SelectPlayer(mClanPlayerElement element)
	{
		this.playersClan.SelectPlayer(element);
	}

	private void OnCreateClanComplete()
	{
		mPopUp.SetActiveWaitPanel(false);
		this.Open();
	}

	private void OnCreateClanError(string error)
	{
		mPopUp.SetActiveWaitPanel(false);
		UIToast.Show(error);
	}

	private void OnGetDataComplete(string data)
    {
        JsonObject jsonObject = JsonObject.Parse(data);
		ClanManager.name = jsonObject.Get<string>("n");
		ClanManager.admin = Convert.ToInt32(jsonObject.Get<string>("a"));
		JsonObject jsonObject2 = jsonObject.Get<JsonObject>("p");
		int[] array = new int[jsonObject2.Length];
		for (int i = 0; i < jsonObject2.Length; i++)
		{
			array[i] = int.Parse(jsonObject2.GetKey(i));
		}
		ClanManager.players = array;
		this.chatClan.nameLabel.text = ClanManager.name;
		ClanManager.Save();
		this.loadData = true;
	}

	private void OnGetDataError(string error)
	{
		UIToast.Show(error);
	}
}
