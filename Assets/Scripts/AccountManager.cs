using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using FreeJSON;
using UnityEngine;
using BestHTTP;

public class AccountManager : MonoBehaviour
{
    public AccountData Data = new AccountData();

    public CryptoString[] Links = new CryptoString[0];

    public static bool isConnect;

    public static CryptoString AccountID;

    public static AccountManager instance;

    public static AccountClan Clan = new AccountClan();

    public static CryptoString AccountToken;

    public AccountData DefaultData = new AccountData();

    public static long LastLogin;
    
	private void Awake()
	{
		if (AccountManager.instance == null)
		{
			AccountManager.instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public static void Init()
	{
		new GameObject("AccountManager").AddComponent<AccountManager>();
	}

	public static void Login(string id, Action<bool> complete, Action<string> failed)
	{
		AccountManager.AccountID = id;
        if (string.IsNullOrEmpty(AccountManager.AccountToken))
        {
            Loom.RunAsync(delegate
            {
                AccountManager.AccountToken = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(AccountManager.AccountID);
                Loom.QueueOnMainThread(delegate ()
                {
                    AccountManager.Login(complete, failed);
                });
            });
            return;
        }
        AccountManager.Login(complete, failed);
	}

    private static void Login(Action<bool> complete, Action<string> failed)
    {
        Firebase deviceBansFirebase = new Firebase();
        deviceBansFirebase.Auth = AccountManager.AccountToken;
        deviceBansFirebase.Child("Players").Child("DevicesBans").Child(AndroidNativeFunctions.GetAndroidID2()).GetValue(delegate (string deviceBanResult)
        {
            if (deviceBanResult != "null")
            {
                AccountManager.isConnect = false;
                failed("Device Ban");
                mPopUp.SetActiveWait(false);
                return;
            }
            Firebase firebase = new Firebase();
            firebase.Auth = AccountManager.AccountToken;
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).GetValue(delegate (string result)
            {
                if (result == "null")
                {
                    complete(false);
                    AccountManager.isConnect = false;
                    return;
                }
                if (JsonObject.Parse(result).ContainsKey("Banned"))
                {
                    AccountManager.isConnect = false;
                    failed("Account Ban");
                    mPopUp.SetActiveWait(false);
                    return;
                }
                if (AccountManager.isOldAccountVersion(result))
                {
                    AccountManager.isConnect = false;
                    failed("Account Version");
                    mPopUp.SetActiveWait(false);
                    return;
                }
                AccountManager.instance.DefaultData = AccountConvert.Deserialize(result);
                AccountManager.instance.Data = AccountConvert.Deserialize(result);
                complete(true);
                AccountManager.isConnect = true;
                AccountManager.UpdateLastLogin();
                AccountManager.UpdateSession();
                AccountManager.CheckAndroidEmulator();
                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                AccountManager.SetAvatar(PlayerPrefs.GetString("EditorAvatar", "https://businessmir.kz/wp-content/uploads/2019/01/unnamed.jpg"));
                #else
                AccountManager.SetAvatar(AndroidGoogleSignIn.Account.PhotoUrl);
                #endif
                EventManager.Dispatch("AccountConnected");
            }, delegate (string error)
            {
                firebase = new Firebase();
                firebase.Child("Players").Child("AccountsBans").GetValue(FirebaseParam.Default.OrderByKey().EqualTo(AccountManager.AccountID), delegate (string result)
                {
                    JsonObject jsonObject = JsonObject.Parse(result);
                    if (jsonObject.Length != 0)
                    {
                        failed(jsonObject.Get<string>(AccountManager.AccountID));
                        return;
                    }
                    failed(error);
                }, delegate (string error2)
                {
                    failed(error);
                });
                AccountManager.isConnect = false;
            });
        },
        delegate (string deviceBanError)
        {
            failed(deviceBanError);
            AccountManager.isConnect = false;
        });
    }

    public static void Register(string name, AccountData data, Action complete, Action<string> failed)
    {
        new Firebase { Auth = AccountManager.AccountToken }.Child("Players").Child("NickNames").Child(name.ToUpper()[0].ToString()).Child(name).GetValue(delegate (string result)
        {
            if (result != "null" && result != "{}")
            {
                failed("Name already taken");
                return;
            }

            string json = AccountConvert.Serialize(name, data, true);
            new Firebase { Auth = AccountManager.AccountToken }.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(json, delegate (string result2)
            {
                AccountManager.instance.DefaultData = AccountConvert.Deserialize(result2);
                AccountManager.instance.Data = AccountConvert.Deserialize(result2);
                complete();
                AccountManager.isConnect = true;
                AccountManager.UpdateLastLogin();
                AccountManager.UpdateSession();

                #if UNITY_EDITOR || UNITY_STANDALONE_WIN
                AccountManager.SetAvatar(PlayerPrefs.GetString("EditorAvatar", "https://businessmir.kz/wp-content/uploads/2019/01/unnamed.jpg"));
                #else
                AccountManager.SetAvatar(AndroidGoogleSignIn.Account.PhotoUrl);
                #endif

                JsonObject jsonAccountIDS = new JsonObject();
                jsonAccountIDS.Add(AccountManager.instance.Data.ID.ToString(), name);
                Firebase firebaseAccountIDS = new Firebase();
                firebaseAccountIDS.Auth = AccountManager.AccountToken;
                firebaseAccountIDS.Child("Players").Child("AccountIDS").UpdateValue(jsonAccountIDS.ToString());

                string parentNew = name.ToUpper()[0].ToString();
                Firebase checkNameFirebase = new Firebase();
                checkNameFirebase.Auth = AccountManager.AccountToken;
                JsonObject nameJjson = new JsonObject();
                nameJjson.Add(name, AccountManager.AccountID);
                checkNameFirebase.Child("Players").Child("NickNames").Child(parentNew).UpdateValue(nameJjson.ToString(), delegate (string result3)
                {
                    AccountManager.CheckAndroidEmulator();
                }, delegate (string error2, string json2)
                {
                    failed(error2);
                    AccountManager.isConnect = false;
                    return;
                });
            }, delegate (string error, string json2)
            {
                failed(error);
                AccountManager.isConnect = false;
            });
        }, delegate (string error)
        {
            failed(error);
        });
    }

    public static void UpdateName(string newName, Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        string oldName = AccountManager.instance.Data.AccountName;
        string parentNew = newName.ToUpper()[0].ToString();
        firebase.Child("Players").Child("NickNames").Child(parentNew).Child(newName).GetValue(delegate (string result)
        {
            if (result != "null" && result != "{}")
            {
                failed("Name already taken");
                return;
            }
            JsonObject json = new JsonObject();
            json.Add("AccountName", newName);
            firebase = new Firebase();
            firebase.Auth = AccountManager.AccountToken;
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(json.ToString(), delegate (string result2)
            {
                AccountManager.instance.Data.AccountName = newName;
                complete(newName);

                JsonObject jsonAccountIDS = new JsonObject();
                jsonAccountIDS.Add(AccountManager.instance.Data.ID.ToString(), newName);
                Firebase firebaseAccountIDS = new Firebase();
                firebaseAccountIDS.Auth = AccountManager.AccountToken;
                firebaseAccountIDS.Child("Players").Child("AccountIDS").UpdateValue(jsonAccountIDS.ToString());

                firebase = new Firebase();
                firebase.Auth = AccountManager.AccountToken;
                json = new JsonObject();
                json.Add(newName, AccountManager.AccountID);
                firebase.Child("Players").Child("NickNames").Child(parentNew).UpdateValue(json.ToString(), delegate (string result3)
                {
                    if (!string.IsNullOrEmpty(oldName))
                    {
                        firebase = new Firebase();
                        firebase.Auth = AccountManager.AccountToken;
                        firebase.Child("Players").Child("NickNames").Child(oldName.ToUpper()[0].ToString()).Child(oldName).Delete();
                    }
                }, delegate (string error, string json2)
                {
                    if (!string.IsNullOrEmpty(oldName))
                    {
                        firebase = new Firebase();
                        firebase.Auth = AccountManager.AccountToken;
                        firebase.Child("Players").Child("NickNames").Child(oldName.ToUpper()[0].ToString()).Child(oldName).Delete();
                    }
                });
            }, delegate (string error, string json2)
            {
                failed(error);
            });
        }, delegate (string error)
        {
            failed(error);
        });
    }

    public static void UpdateData(Action failed, Action complete)
    {
        //AccountData oldData = AccountConvert.DeserializeOld(AccountManager.instance.OldData);
        //Firebase firebase = new Firebase();
        //firebase.Auth = AccountManager.AccountToken;
        //firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("LevelXP").Delete();
        //firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("PlayerLevel").Delete();
        //TimerManager.In(1f, delegate ()
        //{
        //    string json = AccountConvert.Serialize(oldData.AccountName, oldData, false);
        //    firebase = new Firebase();
        //    firebase.Auth = AccountManager.AccountToken;
        //    firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(json, delegate (string result)
        //    {
        //        AccountManager.instance.DefaultData = AccountConvert.Deserialize(result);
        //        AccountManager.instance.Data = AccountConvert.Deserialize(result);
        //        AccountManager.isConnect = true;
        //        AccountManager.UpdateLastLogin();
        //        AccountManager.UpdateSession();
        //        mPopUp.HideAll("Menu");
        //        mPopUp.SetActiveWait(false);
        //        EventManager.Dispatch("AccountUpdate");
        //        WeaponManager.UpdateData();
        //    }, delegate (string error, string json2)
        //    {
        //        AccountManager.UpdateData(null);
        //        AccountManager.isConnect = false;
        //    });
        //});
    }

    public static void AddFriend(int id, string nickName, Action complete, Action<string> failed)
    {
        List<int> ids = new List<int>();
        Firebase firebaseFriends = new Firebase();
        firebaseFriends.Auth = AccountManager.AccountToken;
        firebaseFriends.Child("Players").Child("Accounts").Child(AccountParent).Child(AccountManager.AccountID).GetValue(delegate (string result)
        {
            JsonObject idsObject = new JsonObject();
            idsObject = JsonObject.Parse(result);
            ids = idsObject.Get<List<int>>("Friends");
            ids.Add(id);
            Firebase firebase = new Firebase();
            firebase.Auth = AccountManager.AccountToken;
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("Friends", ids);
            firebase.Child("Players").Child("Accounts").Child(AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string udapteResult)
            {
                AccountManager.instance.Data.Friends = ids;
                EventManager.Dispatch("AccountUpdate");
                complete();
            },
            delegate (string error, string updateJson)
            {
                failed(error);
            });
        }, delegate (string error)
        {
            failed(error);
        });
    }

    public static void DeleteFriend(int id, Action complete, Action<string> failed)
    {
        List<int> ids = new List<int>();
        Firebase firebaseFriends = new Firebase();
        firebaseFriends.Auth = AccountManager.AccountToken;
        firebaseFriends.Child("Players").Child("Accounts").Child(AccountParent).Child(AccountManager.AccountID).GetValue(delegate (string result)
        {
            JsonObject idsObject = new JsonObject();
            idsObject = JsonObject.Parse(result);
            ids = idsObject.Get<List<int>>("Friends");
            ids.Remove(id);
            Firebase firebase = new Firebase();
            firebase.Auth = AccountManager.AccountToken;
            JsonObject jsonObject = new JsonObject();
            jsonObject.Add("Friends", ids);
            firebase.Child("Players").Child("Accounts").Child(AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate (string udapteResult)
            {
                AccountManager.instance.Data.Friends = ids;
                EventManager.Dispatch("AccountUpdate");
            },
            delegate (string error, string updateJson)
            {
                failed(error);
            });

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

                    List<int> ids2 = new List<int>();
                    Firebase firebaseFriends2 = new Firebase();
                    firebaseFriends2.Auth = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(email);
                    firebaseFriends2.Child("Players").Child("Accounts").Child(email.ToUpper()[0].ToString()).Child(email).GetValue(delegate (string result2)
                    {
                        JsonObject idsObject2 = new JsonObject();
                        idsObject2 = JsonObject.Parse(result2);
                        ids2 = idsObject2.Get<List<int>>("Friends");
                        ids2.Remove(AccountManager.instance.Data.ID);
                        Firebase firebase2 = new Firebase();
                        firebase2.Auth = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(email);
                        JsonObject jsonObject2 = new JsonObject();
                        jsonObject2.Add("Friends", ids2);
                        firebase2.Child("Players").Child("Accounts").Child(email.ToUpper()[0].ToString()).Child(email).UpdateValue(jsonObject2.ToString(), delegate (string udapteResult)
                        {
                            complete();
                        },
                        delegate (string error, string updateJson)
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
            }, delegate (string error)
            {
                failed(error);
            });
        }, delegate (string error)
        {
            failed(error);
        });
    }

    public static void GetFriendsName(int[] ids, Action complete, Action<string> failed)
    {
        GC.Collect();
        int[] accountIds = new int[20];
        for (int i = 0; i < ids.Length; i++)
        {
            accountIds[i] = ids[i];
        }
        bool completed = false;
        bool arequested = false;
        bool brequested = false;
        bool crequested = false;
        bool drequested = false;
        bool frequested = false;
        bool grequested = false;
        bool hrequested = false;
        bool irequested = false;
        bool jrequested = false;
        bool krequested = false;
        bool lrequested = false;
        bool mrequested = false;
        bool nrequested = false;
        bool orequested = false;
        bool prequested = false;
        bool qrequested = false;
        bool rrequested = false;
        bool srequested = false;
        bool trequested = false;
        bool urequested = false;
        string auth = AccountManager.AccountToken;
        if (accountIds[0] == null)
        {
            arequested = true;
        }
        else
        {
            int index = 0;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                arequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[1] == null)
        {
            brequested = true;
        }
        else
        {
            int index = 1;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                brequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[2] == null)
        {
            crequested = true;
        }
        else
        {
            int index = 2;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                crequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[3] == null)
        {
            drequested = true;
        }
        else
        {
            int index = 3;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                drequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[4] == null)
        {
            frequested = true;
        }
        else
        {
            int index = 4;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                frequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[5] == null)
        {
            grequested = true;
        }
        else
        {
            int index = 5;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                grequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[6] == null)
        {
            hrequested = true;
        }
        else
        {
            int index = 6;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                hrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[7] == null)
        {
            irequested = true;
        }
        else
        {
            int index = 7;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                irequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[8] == null)
        {
            jrequested = true;
        }
        else
        {
            int index = 8;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                jrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[9] == null)
        {
            krequested = true;
        }
        else
        {
            int index = 9;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                krequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[10] == null)
        {
            lrequested = true;
        }
        else
        {
            int index = 10;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                lrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[11] == null)
        {
            mrequested = true;
        }
        else
        {
            int index = 11;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                mrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[12] == null)
        {
            nrequested = true;
        }
        else
        {
            int index = 12;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                nrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[13] == null)
        {
            orequested = true;
        }
        else
        {
            int index = 13;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                orequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[14] == null)
        {
            prequested = true;
        }
        else
        {
            int index = 14;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                prequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[15] == null)
        {
            qrequested = true;
        }
        else
        {
            int index = 15;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                qrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[16] == null)
        {
            rrequested = true;
        }
        else
        {
            int index = 16;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                rrequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[17] == null)
        {
            srequested = true;
        }
        else
        {
            int index = 17;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                srequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[18] == null)
        {
            trequested = true;
        }
        else
        {
            int index = 18;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                trequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        if (accountIds[19] == null)
        {
            urequested = true;
        }
        else
        {
            int index = 19;
            string nickname = string.Empty;
            Firebase nicknameFirebase = new Firebase();
            nicknameFirebase.Auth = auth;
            nicknameFirebase.Child("Players").Child("AccountIDS").GetValue(delegate (string value)
            {
                nickname = JsonObject.Parse(value).Get<string>(accountIds[index].ToString());
                CryptoPrefs.SetString("Friend_#" + accountIds[index], nickname);
                urequested = true;
                GC.Collect();
            }, delegate (string error)
            {
                failed(error);
            });
        }
        GC.Collect();
        //while (!completed)
        //{
        //    if (arequested && brequested && crequested && drequested && frequested && grequested && hrequested && irequested && jrequested && krequested && lrequested && mrequested && nrequested && orequested && prequested && qrequested && rrequested && srequested && trequested && urequested)
        //    {
        //        completed = true;
        //    }
        //}
        //complete();
    }

    public static void GetFriendsInfo(int id, Action<string> complete, Action<string> failed)
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
                JsonObject accountJson = new JsonObject();
                Firebase accountFire = new Firebase();
                accountFire.Auth = new FirebaseToken("ZFpWI7JSa5KQJIlo8aTQqcGrwpEjMeT0x4bLIix3").CreateToken(email);
                accountFire.Child("Players").Child("Accounts").Child(email.ToUpper()[0].ToString()).Child(email).GetValue(delegate (string result)
                {
                    accountJson = JsonObject.Parse(result);
                    complete(accountJson.ToString());
                }, delegate (string error)
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

    private static void SetAvatar(string url)
	{
        if (string.IsNullOrEmpty(url))
        {
            return;
        }
        if (CacheManager.Exists(url, "Avatars", true))
        {
            AccountManager.instance.Data.Avatar = new Texture2D(96, 96);
            AccountManager.instance.Data.Avatar.LoadImage(CacheManager.Load<byte[]>(url, "Avatars", true));
            AccountManager.instance.Data.Avatar.Apply();
            AccountManager.instance.Data.AvatarUrl = url;
            EventManager.Dispatch("AvatarUpdate");
            return;
        }
        new HTTPRequest(new Uri(url), delegate (HTTPRequest req, HTTPResponse res)
        {
            if (res.IsSuccess)
            {
                AccountManager.instance.Data.Avatar = new Texture2D(96, 96);
                AccountManager.instance.Data.Avatar.LoadImage(res.Data);
                AccountManager.instance.Data.Avatar.Apply();
                AccountManager.instance.Data.AvatarUrl = url;
                EventManager.Dispatch("AvatarUpdate");
                CacheManager.Save<byte[]>(url, "Avatars", res.Data, true);
            }
        }).Send();
    }

	public static bool CheckVersion()
	{
		return Utils.CompareVersion(VersionManager.bundleVersion, AccountManager.instance.Data.GameVersion);
	}

	public static bool CheckAndroidEmulator()
	{
		if (AndroidEmulatorDetector.isEmulator() && !CryptoPrefs.GetBool("AndroidEmulator", false))
		{
			TimerManager.In(0.2f, false, delegate()
			{
				Application.Quit();
			});
			AndroidNativeFunctions.ShowToast("Android Emulator Detected");
			return true;
		}
		return false;
	}

	public static void UpdateData(Action failed)
	{
		AccountManager.UpdateData(failed, null);
	}

	public static void UpdateRound(JsonObject data, Action<string> complete, Action<string> failed)
	{

	}

	public static void BuyPlayerSkin(int id, BodyParts part, Action complete, Action<string> failed)
	{

	}

	public static void OpenSkinCase(int id, Action<string> complete, Action<string> failed)
	{

	}

	public static void OpenStickerCase(int id, Action<string> complete, Action<string> failed)
	{

	}

	public static void InAppPurchase(JsonObject purchase, Action<string> complete, Action<string> failed)
	{

	}

	public static void Rewarded(GameCurrency currency, Action complete, Action<string> failed)
	{

	}

	public static void SetSticker(int weapon, int skin, int sticker, int pos, Action complete, Action<string> failed)
	{

	}

    public static void DeleteSticker(int weapon, int skin, int pos, Action complete, Action<string> failed)
	{

	}

    public static void DeleteSticker(int id, bool update, Action<bool> complete, Action<string> failed)
    {
        if (AccountManager.GetStickers(id))
        {
            int i = 0;
            while (i < AccountManager.instance.Data.Stickers.Count)
            {
                if (AccountManager.instance.Data.Stickers[i].ID == id)
                {
                    AccountSticker accountSticker = AccountManager.instance.Data.Stickers[i];
                    if (accountSticker.Count == 1)
                    {
                        AccountManager.instance.Data.Stickers.RemoveAt(i);
                        AccountManager.instance.Data.SortStickers();
                        break;
                    }
                    else
                    {
                        accountSticker.Count--;
                        AccountManager.instance.Data.Stickers[i] = accountSticker;
                    }
                    break;
                }
                else
                {
                    i++;
                }
            }
        }
        if (update)
        {
            AccountManager.UpdateDefaultData(null, null);
        }
    }

    public static void BuyWeapon(int id, Action complete, Action<string> failed)
	{

	}

    public static int GetMoney()
	{
        #if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            return 10000;
        }
        #endif
        return AccountManager.instance.Data.Money;
	}

	public static int GetGold()
	{
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 1000;
        }
        #endif
        return AccountManager.instance.Data.Gold;
	}

	public static int GetXP()
	{
		if (AccountManager.GetLevel() == 250)
		{
			return AccountManager.GetMaxXP();
		}
		return AccountManager.instance.Data.XP;
	}

	public static int GetMaxXP()
	{
		return 150 + 150 * AccountManager.GetLevel();
	}

	public static int GetLevel()
	{
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 250;
        }
        #endif
        return AccountManager.instance.Data.Level;
	}

	public static List<string> GetInAppPurchase()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < AccountManager.instance.Data.InAppPurchase.Count; i++)
		{
			list.Add(AccountManager.instance.Data.InAppPurchase[i]);
		}
		return list;
	}

	public static int GetDeaths()
	{
		return AccountManager.instance.Data.Deaths;
	}

	public static int GetKills()
	{
		return AccountManager.instance.Data.Kills;
	}

	public static int GetHeadshot()
	{
		return AccountManager.instance.Data.Headshot;
	}

	public static void SetWeaponSelected(WeaponType weaponType, int weaponID)
	{
		switch (weaponType)
		{
		case WeaponType.Knife:
			if ((AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0) && WeaponManager.GetWeaponData(weaponID).Secret)
			{
				return;
			}
			AccountManager.instance.Data.SelectedKnife = weaponID;
			break;
		case WeaponType.Pistol:
			AccountManager.instance.Data.SelectedPistol = weaponID;
			break;
		case WeaponType.Rifle:
			AccountManager.instance.Data.SelectedRifle = weaponID;
			break;
		}
		AccountManager.instance.Data.UpdateSelectedWeapon = true;
	}

	public static int GetWeaponSelected(WeaponType weaponType)
	{
		switch (weaponType)
		{
		case WeaponType.Knife:
			return AccountManager.instance.Data.SelectedKnife;
		case WeaponType.Pistol:
			return AccountManager.instance.Data.SelectedPistol;
		case WeaponType.Rifle:
			return AccountManager.instance.Data.SelectedRifle;
		default:
			return 0;
		}
	}

	public static void SetPlayerSkinSelected(int id, BodyParts part)
	{
        switch (part)
        {
            case BodyParts.Head:
                AccountManager.instance.Data.PlayerSkin.Select[0] = id;
                break;
            case BodyParts.Body:
                AccountManager.instance.Data.PlayerSkin.Select[1] = id;
                break;
            case BodyParts.Legs:
                AccountManager.instance.Data.PlayerSkin.Select[2] = id;
                break;
        }
        AccountManager.instance.Data.UpdateSelectedPlayerSkin = true;
	}

    #if UNITY_EDITOR
    public static bool developerSkin;
    #endif

    public static int GetPlayerSkinSelected(BodyParts part)
	{
            #if UNITY_EDITOR
            if(developerSkin)
            {
                switch (part)
                {
                    case BodyParts.Head:
                        return 98;
                    case BodyParts.Body:
                        return 98;
                    case BodyParts.Legs:
                        return 98;
                    default:
                        return -1;
                }
            }
            else
            {
                switch (part)
			    {
		    	case BodyParts.Head:
				    return AccountManager.instance.Data.PlayerSkin.Select[0];
		    	case BodyParts.Body:
			    	return AccountManager.instance.Data.PlayerSkin.Select[1];
			    case BodyParts.Legs:
			    	return AccountManager.instance.Data.PlayerSkin.Select[2];
			    default:
			     	return -1;
		    	}
            }
            return -1;
            #else
            switch (part)
		    {
		  	case BodyParts.Head:
			    return AccountManager.instance.Data.PlayerSkin.Select[0];
		   	case BodyParts.Body:
			   	return AccountManager.instance.Data.PlayerSkin.Select[1];
		    case BodyParts.Legs:
			    return AccountManager.instance.Data.PlayerSkin.Select[2];
			default:
			    return -1;
		    }
            #endif
	}

    public static void UpdatePlayerSkin(Action<string> result, Action<string> error)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObjectPlayerSkin = new JsonObject();
        jsonObjectPlayerSkin.Add("Select", (instance.Data.PlayerSkin.Select[0] + "," + instance.Data.PlayerSkin.Select[1] + "," + instance.Data.PlayerSkin.Select[2]));
        jsonObjectPlayerSkin.Add("Head", instance.Data.PlayerSkin.Head);
        jsonObjectPlayerSkin.Add("Body", instance.Data.PlayerSkin.Body);
        jsonObjectPlayerSkin.Add("Legs", instance.Data.PlayerSkin.Legs);
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("PlayerSkin").UpdateValue(jsonObjectPlayerSkin.ToString(), delegate (string r)
        {
            result(r);
        }, delegate (string e, string json)
        {
            error(e);
        });
    }

    public static void SetPlayerSkin(int id, BodyParts part)
    {
        if (!AccountManager.GetPlayerSkin(id, part))
        {
            switch (part)
            {
                case BodyParts.Head:
                    AccountManager.instance.Data.PlayerSkin.Head.Add(id);
                    return;
                case BodyParts.Body:
                    AccountManager.instance.Data.PlayerSkin.Body.Add(id);
                    return;
                case BodyParts.Legs:
                    AccountManager.instance.Data.PlayerSkin.Legs.Add(id);
                    break;
                default:
                    return;
            }
        }
    }

	public static bool GetPlayerSkin(int id, BodyParts part)
	{
		if (id == 0)
		{
			return true;
		}
		switch (part)
		{
		case BodyParts.Head:
			return AccountManager.instance.Data.PlayerSkin.Head.Contains(id);
		case BodyParts.Body:
			return AccountManager.instance.Data.PlayerSkin.Body.Contains(id);
		case BodyParts.Legs:
			return AccountManager.instance.Data.PlayerSkin.Legs.Contains(id);
		default:
			return false;
		}
	}

	public static bool GetWeapon(int id)
	{
		if (id == nValue.int12 || id == nValue.int3 || id == nValue.int4)
		{
			return true;
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].ID == id)
			{
				if (GameSettings.instance.Weapons[i].Secret)
				{
					for (int j = 0; j < AccountManager.instance.Data.Weapons.Count; j++)
					{
						if (AccountManager.instance.Data.Weapons[j].ID == id)
						{
							return AccountManager.instance.Data.Weapons[j].Skins != null && AccountManager.instance.Data.Weapons[j].Skins.Count != 0;
						}
					}
				}
				else
				{
					for (int k = 0; k < AccountManager.instance.Data.Weapons.Count; k++)
					{
						if (AccountManager.instance.Data.Weapons[k].ID == id)
						{
							return AccountManager.instance.Data.Weapons[k].Buy;
						}
					}
				}
			}
		}
		return false;
	}

	public static void SetWeaponSkin(int id, int skin)
	{
		int i = 0;
		while (i < AccountManager.instance.Data.Weapons.Count)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				if (!AccountManager.instance.Data.Weapons[i].Skins.Contains(skin))
				{
					AccountManager.instance.Data.Weapons[i].Skins.Add(skin);
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public static bool GetWeaponSkin(int id, int skin)
	{
		if (skin == 0)
		{
			return true;
		}
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				return AccountManager.instance.Data.Weapons[i].Skins.Contains((CryptoInt)skin);
			}
		}
		return false;
	}

	public static void SetWeaponSkinSelected(int id, int skin)
	{
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				AccountManager.instance.Data.Weapons[i].LastSkin = AccountManager.instance.Data.Weapons[i].Skin;
				AccountManager.instance.Data.Weapons[i].Skin = skin;
				return;
			}
		}
	}

	public static int GetWeaponSkinSelected(int id)
	{
		if (id == 0)
		{
			return 0;
		}
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				return AccountManager.instance.Data.Weapons[i].Skin;
			}
		}
		return 0;
	}

	public static void SetFireStat(int id, int skin)
	{
		if (skin == 0)
		{
			return;
		}
		int i = 0;
		while (i < AccountManager.instance.Data.Weapons.Count)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				if (AccountManager.instance.Data.Weapons[i].FireStats.Count <= skin)
				{
					for (int j = AccountManager.instance.Data.Weapons[i].FireStats.Count - 1; j < skin; j++)
					{
						AccountManager.instance.Data.Weapons[i].FireStats.Add(-1);
					}
					AccountManager.instance.Data.Weapons[i].FireStats[AccountManager.instance.Data.Weapons[i].FireStats.Count - 1] = 0;
					return;
				}
				if (AccountManager.instance.Data.Weapons[i].FireStats[skin] < 0)
				{
					AccountManager.instance.Data.Weapons[i].FireStats[skin] = 0;
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public static bool GetFireStat(int id, int skin)
	{
		if (skin == 0)
		{
			return false;
		}
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				return AccountManager.instance.Data.Weapons[i].FireStats.Count > skin && AccountManager.instance.Data.Weapons[i].FireStats[skin] != -1;
			}
		}
		return false;
	}

	public static void SetFireStatCounter(int id, int skin, int value)
	{
		if (skin == 0)
		{
			return;
		}
		int i = 0;
		while (i < AccountManager.instance.Data.Weapons.Count)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				if (AccountManager.instance.Data.Weapons[i].FireStats.Count > skin)
				{
					AccountManager.instance.Data.Weapons[i].FireStats[skin] = value;
					return;
				}
				for (int j = AccountManager.instance.Data.Weapons[i].FireStats.Count - 1; j < skin; j++)
				{
					AccountManager.instance.Data.Weapons[i].FireStats.Add(-1);
				}
				AccountManager.instance.Data.Weapons[i].FireStats[AccountManager.instance.Data.Weapons[i].FireStats.Count - 1] = value;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	public static int GetFireStatCounter(int id, int skin)
	{
		if (skin == 0)
		{
			return -1;
		}
		int i = 0;
		while (i < AccountManager.instance.Data.Weapons.Count)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				if (AccountManager.instance.Data.Weapons[i].FireStats.Count > skin)
				{
					return AccountManager.instance.Data.Weapons[i].FireStats[skin];
				}
				return -1;
			}
			else
			{
				i++;
			}
		}
		return -1;
	}

	public static string GetClan()
	{
		return AccountManager.instance.Data.Clan;
	}

	public static void SetStickers(int id)
	{
		if (AccountManager.GetStickers(id))
		{
			for (int i = 0; i < AccountManager.instance.Data.Stickers.Count; i++)
			{
				if (AccountManager.instance.Data.Stickers[i].ID == id)
				{
                    AccountManager.instance.Data.Stickers[i].Count++;
                    return;
				}
			}
			return;
		}
		AccountSticker accountSticker = new AccountSticker();
		accountSticker.ID = id;
		accountSticker.Count = 1;
		AccountManager.instance.Data.Stickers.Add(accountSticker);
		AccountManager.instance.Data.SortStickers();
	}

	private static void DeleteSticker(int id)
	{
		if (AccountManager.GetStickers(id))
		{
			int i = 0;
			while (i < AccountManager.instance.Data.Stickers.Count)
			{
				if (AccountManager.instance.Data.Stickers[i].ID == id)
				{
					AccountSticker accountSticker = AccountManager.instance.Data.Stickers[i];
					if (AccountManager.instance.Data.Stickers[i].Count == 0)
					{
						AccountManager.instance.Data.Stickers.RemoveAt(i);
						AccountManager.instance.Data.SortStickers();
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	public static bool GetStickers(int id)
	{
		for (int i = 0; i < AccountManager.instance.Data.Stickers.Count; i++)
		{
			if (AccountManager.instance.Data.Stickers[i].ID == id)
			{
				return true;
			}
		}
		return false;
	}

	public static int[] GetStickers()
	{
		int[] array = new int[AccountManager.instance.Data.Stickers.Count];
		for (int i = 0; i < AccountManager.instance.Data.Stickers.Count; i++)
		{
			array[i] = AccountManager.instance.Data.Stickers[i].ID;
		}
		return array;
	}

	public static int GetStickerCount(int id)
	{
		for (int i = 0; i < AccountManager.instance.Data.Stickers.Count; i++)
		{
			if (AccountManager.instance.Data.Stickers[i].ID == id)
			{
				return AccountManager.instance.Data.Stickers[i].Count;
			}
		}
		return 0;
	}

	public static void SetWeaponSticker(int weapon, int skin, int pos, int sticker)
	{
		AccountWeaponStickers weaponStickers = AccountManager.GetWeaponStickers(weapon, skin);
		if (AccountManager.HasWeaponSticker(weapon, skin, pos))
		{
			for (int i = 0; i < weaponStickers.StickerData.Count; i++)
			{
				if (weaponStickers.StickerData[i].Index == pos)
				{
					weaponStickers.StickerData[i].StickerID = sticker;
					return;
				}
			}
			return;
		}
		AccountWeaponStickerData accountWeaponStickerData = new AccountWeaponStickerData();
		accountWeaponStickerData.Index = pos;
		accountWeaponStickerData.StickerID = sticker;
		weaponStickers.StickerData.Add(accountWeaponStickerData);
		weaponStickers.SortWeaponStickerData();
	}

	public static void DeleteWeaponSticker(int weapon, int skin, int pos)
	{
		AccountWeaponStickers weaponStickers = AccountManager.GetWeaponStickers(weapon, skin);
		if (weaponStickers == null)
		{
			return;
		}
		for (int i = 0; i < weaponStickers.StickerData.Count; i++)
		{
			if (weaponStickers.StickerData[i].Index == pos)
			{
				weaponStickers.StickerData.RemoveAt(i);
				weaponStickers.SortWeaponStickerData();
				return;
			}
		}
	}

	public static bool HasWeaponSticker(int weapon, int skin, int pos)
	{
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == weapon)
			{
				for (int j = 0; j < AccountManager.instance.Data.Weapons[i].Stickers.Count; j++)
				{
					if (AccountManager.instance.Data.Weapons[i].Stickers[j].SkinID == skin)
					{
						for (int k = 0; k < AccountManager.instance.Data.Weapons[i].Stickers[j].StickerData.Count; k++)
						{
							if (AccountManager.instance.Data.Weapons[i].Stickers[j].StickerData[k].Index == pos)
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	public static int GetWeaponSticker(int weapon, int skin, int pos)
	{
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == weapon)
			{
				for (int j = 0; j < AccountManager.instance.Data.Weapons[i].Stickers.Count; j++)
				{
					if (AccountManager.instance.Data.Weapons[i].Stickers[j].SkinID == skin)
					{
						for (int k = 0; k < AccountManager.instance.Data.Weapons[i].Stickers[j].StickerData.Count; k++)
						{
							if (AccountManager.instance.Data.Weapons[i].Stickers[j].StickerData[k].Index == pos)
							{
								return AccountManager.instance.Data.Weapons[i].Stickers[j].StickerData[k].StickerID;
							}
						}
					}
				}
			}
		}
		return -1;
	}

	public static AccountWeaponStickers GetWeaponStickers(int weapon, int skin)
	{
		for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == weapon)
			{
				for (int j = 0; j < AccountManager.instance.Data.Weapons[i].Stickers.Count; j++)
				{
					if (AccountManager.instance.Data.Weapons[i].Stickers[j].SkinID == skin)
					{
						return AccountManager.instance.Data.Weapons[i].Stickers[j];
					}
				}
				AccountWeaponStickers accountWeaponStickers = new AccountWeaponStickers();
				accountWeaponStickers.SkinID = skin;
				AccountManager.instance.Data.Weapons[i].Stickers.Add(accountWeaponStickers);
				return AccountManager.instance.Data.Weapons[i].Stickers[AccountManager.instance.Data.Weapons[i].Stickers.Count - 1];
			}
		}
		return new AccountWeaponStickers();
	}

	private void OnApplicationQuit()
	{
		
	}

	public static bool isOldAccountVersion(string text)
	{
		return JsonObject.Parse(text).Get<int>("OS") != 4;
	}

    public static void UpdateLastLogin()
	{
		if (!AccountManager.isConnect)
		{
			return;
		}
		TimerManager.In(UnityEngine.Random.value, delegate()
		{
			new Firebase
			{
				Auth = AccountManager.AccountToken
			}.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("LastLogin").SetValue(JsonObject.Parse(Firebase.GetTimeStamp()).ToString(), delegate(string result)
			{
				long.TryParse(result, out AccountManager.LastLogin);
			}, null);
		});
	}

	public static void UpdateSession()
	{
		TimerManager.In(UnityEngine.Random.value, delegate()
		{
			Firebase firebase = new Firebase();
			firebase.Auth = AccountManager.AccountToken;
			JsonObject jsonObject = new JsonObject();
			jsonObject.Add("Session", AccountManager.instance.Data.Session + 1);
			firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
			{
				
			}, null);
		});
	}

	public static void SetGold(int gold)
	{
		AccountManager.SetGold(gold, false, null, null);
	}

	public static void CheckSession(Action<bool> action)
	{
		new Firebase
		{
			Auth = AccountManager.AccountToken
		}.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Session").GetValue(delegate(string result)
		{
			if (result == AccountManager.instance.Data.Session.ToString())
			{
				action(true);
				return;
			}
			action(false);
			if (AccountManager.isConnect)
			{
				UIToast.Show(Localization.Get("Session is already outdated", true));
				UIToast.Show(Localization.Get("Restart the game", true));
				AccountManager.isConnect = false;
				PhotonNetwork.Disconnect();
			}
		}, delegate(string error)
		{
			action(false);
			if (AccountManager.isConnect)
			{
				UIToast.Show(Localization.Get("Session is already outdated", true));
				UIToast.Show(Localization.Get("Restart the game", true));
				AccountManager.isConnect = false;
				PhotonNetwork.Disconnect();
			}
		});
	}

	public static void UpdateDefaultData(Action<bool> complete, Action<string, string> failed)
	{
		JsonObject json = AccountConvert.CompareDefaultValue(AccountManager.instance.DefaultData, AccountManager.instance.Data);
		if (json.Length == 0)
		{
			return;
		}
		if (complete != null)
		{
			complete(false);
		}
		AccountManager.CheckSession(delegate(bool session)
		{
			if (!session || !AccountManager.isConnect)
			{
				return;
			}
			AccountData data = AccountConvert.Copy(AccountManager.instance.Data);
			new Firebase
			{
				Auth = AccountManager.AccountToken
			}.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(json.ToString(), delegate(string result)
			{
				AccountConvert.CopyDefaultValue(data, AccountManager.instance.DefaultData);
				if (complete != null)
				{
					complete(true);
				}
			}, delegate(string error, string json2)
			{
				if (failed != null)
				{
					failed(error, json2);
				}
			});
		});
	}

	public static void SetGold(int gold, bool update)
	{
		AccountManager.SetGold(gold, update, null, null);
	}

	public static void SetGold(int gold, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Gold = Mathf.Min(gold, 1000000);
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetGold1(int gold)
	{
		AccountManager.SetGold1(gold, false, null, null);
	}

	public static void SetGold1(int gold, bool update)
	{
		AccountManager.SetGold1(gold, update, null, null);
	}

	public static void SetGold1(int gold, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.SetGold(AccountManager.GetGold() + gold, update, complete, failed);
	}

	public static string AccountParent
	{
		get
		{
			return AccountManager.AccountID.ToString().ToUpper()[0].ToString();
		}
	}

	public static void Register(string playerName, Action complete, Action<string> failed)
	{
		AccountManager.Register(playerName, new AccountData
		{
			Weapons = 
			{
				new AccountWeapon
				{
					ID = 12
				},
				new AccountWeapon
				{
					ID = 4
				},
				new AccountWeapon
				{
					ID = 3
				}
			}
		}, complete, failed);
	}

	public static void SetXP(int xp)
	{
		AccountManager.SetXP(xp, false, null, null);
	}

	public static void SetXP(int xp, bool update)
	{
		AccountManager.SetXP(xp, update, null, null);
	}

	public static void SetXP(int xp, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.XP = xp;
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetXP1(int xp)
	{
		int num = AccountManager.GetLevel();
		int num2 = AccountManager.GetXP() + xp;
		int num3 = AccountManager.GetMaxXP();
		if (num2 >= num3)
		{
			if (num == 250)
			{
				num2 = num3;
			}
			else
			{
				num++;
				num2 -= num3;
				num2 = Mathf.Max(num2, 0);
				num3 = 150 + 150 * num;
				AccountManager.SetLevel(num);
				if (LevelManager.GetSceneName() == "Menu")
				{
					UIToast.Show(Localization.Get("New Level", true) + " " + num);
					AccountManager.SetGold1(10);
				}
			}
		}
		AccountManager.SetXP(num2);
	}

	public static void SetLevel(int level)
	{
		AccountManager.SetLevel(level, false, null, null);
	}

	public static void SetLevel(int level, bool update)
	{
		AccountManager.SetLevel(level, update, null, null);
	}

	public static void SetLevel(int level, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Level = level;
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetKills(int kills)
	{
		AccountManager.SetKills(kills, false, null, null);
	}

	public static void SetKills(int kills, bool update)
	{
		AccountManager.SetKills(kills, update, null, null);
	}

	public static void SetKills(int kills, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Kills = kills;
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetKills1(int kills)
	{
		AccountManager.SetKills1(kills, false, null, null);
	}

	public static void SetKills1(int kills, bool update)
	{
		AccountManager.SetKills1(kills, update, null, null);
	}

	public static void SetKills1(int kills, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.SetKills(AccountManager.GetKills() + kills, update, complete, failed);
	}

	public static void SetDeaths(int deaths)
	{
		AccountManager.SetDeaths(deaths, false, null, null);
	}

	public static void SetDeaths(int deaths, bool update)
	{
		AccountManager.SetDeaths(deaths, update, null, null);
	}

	public static void SetDeaths(int deaths, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Deaths = deaths;
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetDeaths1(int deaths)
	{
		AccountManager.SetDeaths1(deaths, false, null, null);
	}

	public static void SetDeaths1(int deaths, bool update)
	{
		AccountManager.SetDeaths1(deaths, update, null, null);
	}

	public static void SetDeaths1(int deaths, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.SetDeaths(AccountManager.GetDeaths() + deaths, update, complete, failed);
	}

	public static void SetHeadshot(int headshot)
	{
		AccountManager.SetHeadshot(headshot, false, null, null);
	}

	public static void SetHeadshot(int headshot, bool update)
	{
		AccountManager.SetHeadshot(headshot, update, null, null);
	}

	public static void SetHeadshot(int headshot, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Headshot = headshot;
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetHeadshot1(int headshot)
	{
		AccountManager.SetHeadshot1(headshot, false, null, null);
	}

	public static void SetHeadshot1(int headshot, bool update)
	{
		AccountManager.SetHeadshot1(headshot, update, null, null);
	}

	public static void SetHeadshot1(int headshot, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.SetHeadshot(AccountManager.GetHeadshot() + headshot, update, complete, failed);
	}

	public static void UpdateGold(int gold, Action complete, Action<string> failed)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("Gold", gold);
		firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
            if(complete != null)
            {
                complete();
            }
		}, delegate(string error, string json)
        {
            if(failed != null)
            {
                failed(error);
            }
        });
	}

	public static void SetFirebaseWeaponsSelected(Action<string> complete, Action<string> failed)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("SelectWeapons", (AccountManager.GetWeaponSelected(WeaponType.Rifle) + "," + AccountManager.GetWeaponSelected(WeaponType.Pistol) + "," + AccountManager.GetWeaponSelected(WeaponType.Knife)));
		firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
			
		}, null);
	}

    public static void SetWeaponSkinSelected2(int weaponid, int id, Action<string> complete, Action<string> failed)
	{
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add("Skin", id);
        if (weaponid > 9)
        {
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).UpdateValue(jsonObject.ToString(), delegate (string result)
            {
            }, null);
            return;
        }
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child("0" + weaponid.ToString()).UpdateValue(jsonObject.ToString(), delegate (string result)
        {
        }, null);
    }

	public static void SetClanFirebase(string clan)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
		AccountManager.Clan.SendMessage(AccountManager.instance.Data.AccountName + " joined to the the clan", false);
		jsonObject.Add("Clan", clan);
		firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
			
		}, null);
	}

	public static void SetFirebaseWeaponsBuy(int id, Action<string> complete, Action<string> failed)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("Buy", 1);
		if (id < 10)
		{
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child("0" + id.ToString()).UpdateValue(jsonObject.ToString(), delegate (string success)
            {
                if (success.Contains("Buy"))
                {
                    complete(success);
                }
            }, delegate (string error, string json)
            {
                failed(error);
            });
        }
		else
		{
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(id.ToString()).UpdateValue(jsonObject.ToString(), delegate(string success) 
            {
                if (success.Contains("Buy"))
                {
                    complete(success);
                }
            }, delegate(string error, string json) 
            {
                failed(error);
            });
		}
    }

    public static void UpdateWeaponSkins(bool secretWeapon, int weaponId, Action<string> complete, Action<string> failed)
	{
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        if (secretWeapon)
        {
            jsonObject.Add("Buy", 1);
        }
        List<string> list = new List<string>();
        for (int i = 0; i < AccountManager.instance.Data.Weapons.Count; i++)
        {
            if (AccountManager.instance.Data.Weapons[i].ID == weaponId)
            {
                for (int j = 0; j < AccountManager.instance.Data.Weapons[i].Skins.Count; j++)
                {
                    if (AccountManager.instance.Data.Weapons[i].Skins[j] != 0)
                    {
                        list.Add(AccountManager.instance.Data.Weapons[i].Skins[j].ToString());
                    }
                }
            }
        }
        jsonObject.Add("Skins", string.Join(",", list.ToArray()));
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponId.ToString("D2")).UpdateValue(jsonObject.ToString(), delegate (string result)
        {
            complete(result);
        }, delegate(string error, string json) 
        {
            failed(error);
        });
    }

	public static void UpdateWeaponsData(Action<bool> complete, Action<string, string> failed)
	{
		JsonObject json = AccountConvert.CompareWeaponValue(AccountManager.instance.DefaultData, AccountManager.instance.Data);
		if (json.Length == 0)
		{
			return;
		}
		if (complete != null)
		{
			complete(false);
		}
		AccountManager.CheckSession(delegate(bool session)
		{
			if (!session || !AccountManager.isConnect)
			{
				return;
			}
			AccountData data = AccountConvert.Copy(AccountManager.instance.Data);
			new Firebase
			{
				Auth = AccountManager.AccountToken
			}.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").UpdateValue(json.Get<string>("Weapons"), delegate(string result)
			{
				AccountConvert.CopyWeaponsValue(data, AccountManager.instance.DefaultData);
				if (complete != null)
				{
					complete(true);
				}
			}, delegate(string error, string json2)
			{
				if (failed != null)
				{
					failed(error, json2);
				}
			});
		});
	}

    public static void UpdateStickersFirebase(Action<string> complete, Action<string> failed)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        for (int i = 0; i < AccountManager.instance.Data.Stickers.Count; i++)
        {
            jsonObject.Add(AccountManager.instance.Data.Stickers[i].ID.ToString("D2"), AccountManager.instance.Data.Stickers[i].Count);
        }
        firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Stickers").SetValue(jsonObject.ToString(), delegate(string result)
        {
            complete(result);
        }, delegate(string error, string json) 
        {
            failed(error);
        });
    }

    public static void DeleteWeaponStickerFirebase(int weaponid, int skinid, int position)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        new JsonObject();
        if (weaponid > 9)
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).Child(0 + position.ToString()).Delete();
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).Child(0 + position.ToString()).Delete();
            return;
        }
        else
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).Child(0 + position.ToString()).Delete();
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).Child(0 + position.ToString()).Delete();
            return;
        }
    }

    public static void AddWeaponStickerFirebase(int weaponid, int skinid, int stickerid, int position)
    {
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Add(0 + position.ToString(), stickerid);
        if (weaponid > 9)
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).UpdateValue(jsonObject.ToString());
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).UpdateValue(jsonObject.ToString());
            return;
        }
        else
        {
            if (skinid > 9)
            {
                firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(skinid.ToString()).UpdateValue(jsonObject.ToString());
                return;
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(0 + weaponid.ToString()).Child("Stickers").Child(0 + skinid.ToString()).UpdateValue(jsonObject.ToString());
            return;
        }
    }

    public static void SetFireStatFireBase(bool isCase, int weaponId, int skinId, int firestat)
    {
        if (skinId == 0)
        {
            return;
        }
        if (isCase && AccountManager.GetFireStatCounter(weaponId, skinId) > 0)
        {
            return;
        }
        Firebase firebase = new Firebase();
        firebase.Auth = AccountManager.AccountToken;
        JsonObject jsonObject = new JsonObject();
        TimerManager.In(2f, delegate ()
        {
            if (isCase)
            {
                jsonObject.Add(skinId.ToString("D2"), 0);
            }
            else
            {
                jsonObject.Add(skinId.ToString("D2"), firestat);
            }
            firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Weapons").Child(weaponId.ToString("D2")).Child("FireStats").UpdateValue(jsonObject.ToString());
        });
    }

    public static void SetClan(string text)
	{
		AccountManager.instance.Data.Clan = text;
	}

	public static void SetWeapon(int id)
	{
		AccountManager.SetWeapon(id, false, null, null);
	}

	public static void SetWeapon(int id, bool update)
	{
		AccountManager.SetWeapon(id, update, null, null);
	}

	public static void SetWeapon(int id, bool update, Action<bool> complete, Action<string, string> failed)
	{
		int i = 0;
		while (i < AccountManager.instance.Data.Weapons.Count)
		{
			if (AccountManager.instance.Data.Weapons[i].ID == id)
			{
				AccountManager.instance.Data.Weapons[i].Buy = true;
				if (update)
				{
					AccountManager.UpdateWeaponsData(complete, failed);
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public static void UpdateMoney(int money, Action<string> complete, Action<string> failed)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("Money", money);
		firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
			
		}, null);
	}

	public static void SetMoney(int money)
	{
		AccountManager.SetMoney(money, false, null, null);
	}

	public static void SetMoney(int money, bool update)
	{
		AccountManager.SetMoney(money, update, null, null);
	}

	public static void SetMoney(int money, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.instance.Data.Money = Mathf.Min(money, 1000000);
		if (update)
		{
			AccountManager.UpdateDefaultData(complete, failed);
		}
	}

	public static void SetMoney1(int money)
	{
		AccountManager.SetMoney1(money, false, null, null);
	}

    public static void SetTime(long time)
    {
        AccountManager.instance.Data.Time = time;
    }

    public static void SetTime1(long time)
    {
        AccountManager.SetTime(AccountManager.GetTime() + time);
    }

    public static long GetTime()
    {
        #if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return 1337;
        }
        #endif
        return AccountManager.instance.Data.Time;
    }

    public static void SetMoney1(int money, bool update)
	{
		AccountManager.SetMoney1(money, update, null, null);
	}

	public static void SetMoney1(int money, bool update, Action<bool> complete, Action<string, string> failed)
	{
		AccountManager.SetMoney(AccountManager.GetMoney() + money, update, complete, failed);
	}

	public static void UpdatePlayerRoundData(int xp, int kills, int money, int deaths, int headshots, long time, int level, Action<string> complete, Action<string> failed)
	{
		Firebase firebase = new Firebase();
		firebase.Auth = AccountManager.AccountToken;
		JsonObject jsonObject = new JsonObject();
        JsonObject jsonObjectRound = new JsonObject();
        jsonObjectRound.Add("XP", xp);
        jsonObjectRound.Add("Kills", kills);
        jsonObjectRound.Add("Deaths", deaths);
        jsonObjectRound.Add("Head", headshots);
        jsonObjectRound.Add("Time", time);
        jsonObjectRound.Add("Level", level);
        jsonObject.Add("Round", jsonObjectRound);
        jsonObject.Add("Money", money);
		firebase.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).UpdateValue(jsonObject.ToString(), delegate(string result)
		{
			
		}, null);
	}
}
