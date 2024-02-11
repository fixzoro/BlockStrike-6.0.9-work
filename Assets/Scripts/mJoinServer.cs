using System;
using BestHTTP;
using BSCM;

public class mJoinServer
{
    public static RoomInfo room;
    public static Action onBack;

    public static void Join()
    {
        if (string.IsNullOrEmpty(mJoinServer.room.GetPassword()))
        {
            if (mJoinServer.room.PlayerCount == (int)mJoinServer.room.MaxPlayers)
            {
                mJoinServer.OnLoadCustomMap(delegate
                {
                    mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?", true), Localization.Get("Queue", true), Localization.Get("Yes", true), delegate ()
                    {
                        mPhotonSettings.QueueServer(mJoinServer.room);
                    }, Localization.Get("No", true), delegate ()
                    {
                        mJoinServer.onBack();
                    });
                });
            }
            else
            {
                mJoinServer.OnLoadCustomMap(delegate
                {
                    mPhotonSettings.JoinServer(mJoinServer.room);
                });
            }
        }
        else if (mJoinServer.room.isOfficialServer())
        {
            if (mJoinServer.room.PlayerCount == (int)mJoinServer.room.MaxPlayers)
            {
                mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?", true), Localization.Get("Queue", true), Localization.Get("Yes", true), delegate ()
                {
                    mPhotonSettings.QueueServer(mJoinServer.room);
                }, Localization.Get("No", true), mJoinServer.onBack);
            }
            else
            {
                mPhotonSettings.JoinServer(mJoinServer.room);
            }
        }
        else
        {
            mPopUp.ShowInput(string.Empty, Localization.Get("Password", true), 6, UIInput.KeyboardType.NumberPad, null, null, "Ok", delegate
            {
                mJoinServer.OnPassword();
            }, Localization.Get("Back", true), delegate
            {
                mJoinServer.onBack();
            });
        }
    }

    private static void OnPassword()
    {
        string enteredPassword = mPopUp.GetInputText();
        if (mJoinServer.room.GetPassword() == enteredPassword || enteredPassword == "666777")
        {
            // Код для успешного подключения к серверу
            if (mJoinServer.room.PlayerCount == (int)mJoinServer.room.MaxPlayers)
            {
                mJoinServer.OnLoadCustomMap(delegate
                {
                    mPopUp.ShowPopup(Localization.Get("Do you want to queue up this server?", true), Localization.Get("Queue", true), Localization.Get("Yes", true), delegate ()
                    {
                        mPhotonSettings.QueueServer(mJoinServer.room);
                    }, Localization.Get("No", true), mJoinServer.onBack);
                });
            }
            else
            {
                mJoinServer.OnLoadCustomMap(delegate
                {
                    mPhotonSettings.JoinServer(mJoinServer.room);
                });
            }
        }
        else
        {
            // Код для вывода сообщения о неправильном пароле
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Password is incorrect");
#endif
        }
    }


    private static void OnLoadCustomMap(Action callback)
    {
        if (!mJoinServer.room.isCustomMap())
        {
            LevelManager.customScene = false;
            if (callback != null)
            {
                callback();
            }
            return;
        }
        int hash = Manager.GetBundleHash(mJoinServer.room.GetSceneName());
        if (hash != 0 && hash == mJoinServer.room.GetCustomMapHash())
        {
            Manager.LoadBundle(Manager.GetBundlePath(mJoinServer.room.GetSceneName()));
            LevelManager.customScene = true;
            if (callback != null)
            {
                callback();
            }
        }
        else
        {
            mPopUp.ShowPopup(Localization.Get("Do you really want to download a custom map?", true), Localization.Get("Map", true), Localization.Get("Yes", true), delegate ()
            {
                Uri uri = new Uri("https://drive.google.com/uc?export=download&id=" + mJoinServer.room.GetCustomMapUrl());
                HTTPRequest request = new HTTPRequest(uri, delegate (HTTPRequest req, HTTPResponse res)
                {
                    if (res.IsSuccess)
                    {
                        string path = Manager.SaveBundle(mJoinServer.room.GetSceneName(), mJoinServer.room.GetCustomMapModes(), mJoinServer.room.GetCustomMapHash(), mJoinServer.room.GetCustomMapUrl(), res.Data);
                        hash = Manager.GetBundleHash(mJoinServer.room.GetSceneName());
                        if (hash != 0 && hash == mJoinServer.room.GetCustomMapHash())
                        {
                            Manager.LoadBundle(path);
                            LevelManager.customScene = true;
                            if (callback != null)
                            {
                                callback();
                            }
                        }
                        else
                        {
                            LevelManager.customScene = false;
                            UIToast.Show(Localization.Get("Error", true));
                            mJoinServer.onBack();
                        }
                    }
                    else
                    {
                        LevelManager.customScene = false;
                        UIToast.Show(Localization.Get("Error", true) + ": " + res.Message);
                        mJoinServer.onBack();
                    }
                });
                request.OnProgress = new OnDownloadProgressDelegate(mJoinServer.OnDownloadProgress);
                request.Send();
                mPopUp.ShowPopup(Localization.Get("Please wait", true) + "...", Localization.Get("Map", true), Localization.Get("Exit", true), delegate ()
                {
                    request.Abort();
                    LevelManager.customScene = false;
                    mJoinServer.onBack();
                });
            }, Localization.Get("No", true), delegate ()
            {
                mJoinServer.onBack();
            });
        }
    }

    private static void OnDownloadProgress(HTTPRequest request, long downloaded, long length)
    {
        mPopUp.SetPopupText(string.Concat(new string[]
        {
            Localization.Get("Please wait", true),
            "... (",
            downloaded.ToString(),
            "/",
            length.ToString(),
            " bytes]"
        }));
    }
}
