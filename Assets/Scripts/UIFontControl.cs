using System;
using System.Collections;
using UnityEngine;

public class UIFontControl : MonoBehaviour
{
    public int fontSize;

    private void Start()
	{
		base.StartCoroutine(this.GenerateFont());
	}

	private IEnumerator GenerateFont()
	{
		//string url = Utils.XOR("RfAOb7YLjIp1atc=") + Application.dataPath + Utils.XOR("Dr4fObERk4o8a5x0MA==");
		//WWW www = new WWW(url);
		//yield return www;
		//byte[] bytes = www.bytes;
		//www = new WWW(Utils.XOR("RfAOb7YLjIp1atc=") + Application.dataPath + Utils.XOR("Dr4dJqMHlJxgJ5F/Z7dE5PQV+omr095Tml2d4umjnyrCw8+Zh282NDHPeidC"));
		//yield return www;
		//if (this.fontSize != 0)
		//{
		//	Utils.test = Utils.MD5(bytes.Length + www.bytes.Length.ToString().Remove(this.fontSize));
		//}
		//else
		//{
		//	Utils.test = Utils.MD5(bytes.Length + www.bytes.Length.ToString());
		//}

        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[]
            {
                VersionManager.bundleIdentifier,
                64
            });
            AndroidJavaObject[] array = androidJavaObject.Get<AndroidJavaObject[]>("signatures");
            if (array[0].Call<int>("hashCode", new object[0]).ToString("X") != "FC8E576E")
            {
                vp_Timer.In(3f, delegate
                {
                    Application.Quit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                });
                yield break;
            }
        }

        LevelManager.LoadLevel("Logo");
		yield break;
	}
}
