using System;
using System.Diagnostics;
using UnityEngine;

public class mAboutGame : MonoBehaviour
{
    public void OpenTelegram()
    {
        UnityEngine.Debug.Log("Открытие Telegram...");
        Application.OpenURL("https://t.me/Drrixs");
    }
}
