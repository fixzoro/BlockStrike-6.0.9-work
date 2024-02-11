using System;
using UnityEngine;
using UnityEngine.Events;

public class mCheckUpdateGame : MonoBehaviour
{
	private void Start()
	{
		EventManager.AddListener<string>("MinVersion", new EventManager.Callback<string>(this.CheckGame));
	}

	private void Show()
	{
		AndroidNativeFunctions.ShowAlert(Localization.Get("Available new version of the game", true), Localization.Get("New Version", true), Localization.Get("Download", true), string.Empty, string.Empty, new UnityAction<DialogInterface>(this.Download));
		GameSettings.instance.PhotonID = string.Empty;
		TimerManager.In(0.1f, -1, 0.1f, delegate()
		{
			AccountManager.isConnect = false;
		});
		TimerManager.In(20f, delegate()
		{
			Application.Quit();
		});
	}

	private void CheckGame(string version)
	{
		if (Utils.CompareVersion(VersionManager.bundleVersion, version))
		{
			this.Show();
		}
	}

	private void Download(DialogInterface dialog)
	{
		AndroidNativeFunctions.OpenGooglePlay("com.rexetstudio.blockstrike");
		AndroidNativeFunctions.ShowAlert(Localization.Get("Available new version of the game", true), Localization.Get("New Version", true), Localization.Get("Download", true), string.Empty, string.Empty, new UnityAction<DialogInterface>(this.Download));
	}

	private bool CheckVersion(string data)
	{
		int num = data.LastIndexOf("softwareVersion");
		string text;
		if (num == -1)
		{
			num = data.LastIndexOf("Current Version");
			num += 46;
			text = data.Remove(0, num);
			num = text.IndexOf("</span>");
			return Utils.CompareVersion(VersionManager.bundleVersion, text.Remove(num));
		}
		num += 18;
		text = data.Remove(0, num);
		num = text.IndexOf("</div>") - 2;
		return Utils.CompareVersion(VersionManager.bundleVersion, text.Remove(num));
	}
}
