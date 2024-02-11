using System;
using FreeJSON;
using UnityEngine;

public class AccountClan
{
	public void Create(string tag, string name, Action complete, Action<string> failed)
	{

	}

	public void AddPlayer(int id, Action complete, Action<string> failed)
	{
        string nickname = string.Empty;
        string email = string.Empty;
        Firebase nicknameFirebase = new Firebase();
        nicknameFirebase.Auth = AccountManager.AccountToken;
        nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
        {
            nickname = JsonObject.Parse(value).Get<string>(id.ToString());
            Firebase emailFirebase = new Firebase();
            emailFirebase.Auth = AccountManager.AccountToken;
            emailFirebase.Child("Players").Child("NickNames").Child(nickname.ToUpper()[0].ToString()).GetValue(delegate (string value2)
            {
                email = JsonObject.Parse(value2).Get<string>(nickname);
                Firebase accFirebase = new Firebase();
                accFirebase.Auth = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(email);
                accFirebase.Child("Players").Child("Accounts").Child(email.ToUpper()[0].ToString()).Child(email).GetValue(delegate (string result) 
                {
                    JsonObject accJson = JsonObject.Parse(result);
                    accJson.Add("Clan", AccountManager.instance.Data.Clan);
                    Firebase setClanAccFirebase = new Firebase();
                    setClanAccFirebase.Auth = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(email);
                    setClanAccFirebase.Child("Players").Child("Accounts").Child(email.ToUpper()[0].ToString()).Child(email).UpdateValue(accJson.ToString(), delegate (string setResult)
                    {
                        SendMessage(nickname + " joined to the the clan", false);
                        Firebase firebase = new Firebase();
                        firebase.Auth = AccountManager.AccountToken;
                        int[] players = new int[ClanManager.players.Length + 1];
                        for (int i = 0; i < ClanManager.players.Length; i++)
                        {
                            players[i] = ClanManager.players[i];
                        }
                        players[ClanManager.players.Length] = id;
                        ClanManager.players = players;
                        JsonObject playersJson = new JsonObject();
                        JsonObject playersArrayJson = new JsonObject();
                        for (int i = 0; i < ClanManager.players.Length; i++)
                        {
                            playersArrayJson.Add(ClanManager.players[i].ToString(), i);
                        }
                        playersJson.Add("p", playersArrayJson);
                        firebase.Child("Clans").Child(AccountManager.GetClan()).UpdateValue(playersJson.ToString(), delegate (string setPlayersResult)
                        {
                            complete();
                        }, delegate (string error, string json)
                        {
                            failed(error);
                        });
                    },
                    delegate (string error, string json)
                    {
                        failed(error);
                    });
                }, 
                delegate (string error) 
                {
                    failed(error);
                });
            }, delegate (string error)
            {
                failed(error);
            });
        }, delegate (string error)
        {
            failed(error);
        });
    }

	public void GetData(Action<string> complete, Action<string> failed)
	{
		this.GetData(AccountManager.instance.Data.Clan, complete, failed);
	}

	public void GetData(string tag, Action<string> complete, Action<string> failed)
	{
        if(tag == string.Empty)
        {
            return;
        }
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        firebase.Child("Clans").Child(tag).GetValue(delegate (string result)
        {
            if (result == "null")
            {
                failed("404");
                return;
            }
            complete(result);

        }, delegate (string error)
        {
            failed(error);
        });
	}

	public void DeletePlayer(int playerID, string tag, Action complete, Action<string> failed)
	{

	}

	public void SendMessage(string message, bool nickName)
	{
        long epochTicks = new DateTime(1970, 1, 1).Ticks;
        long unixTime = ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
        Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		new AccountData();
		JsonObject jsonObject = new JsonObject();
        if(nickName)
        {
            jsonObject.Add("m", AccountManager.instance.Data.AccountName + ": " + message);
        }
        else
        {
            jsonObject.Add("m", message);
        }
		jsonObject.Add("n", string.Empty);
		jsonObject.Add("t", JsonObject.Parse(Firebase.GetTimeStamp()));
		firebase.Child("Clans").Child(AccountManager.GetClan()).Child("c").Child(unixTime.ToString()).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
		}, null);
	}
}
