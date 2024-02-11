using System;
using UnityEngine;

public class UIMainStatus : MonoBehaviour
{
    public UILabel label;

    private static UIMainStatus instance;

    private void Start()
	{
		UIMainStatus.instance = this;
		PhotonRPC.AddMessage("PhotonShow", new PhotonRPC.MessageDelegate(this.PhotonShow));
	}

	public static void Add(string text)
	{
		UIMainStatus.Add(text, false, 2f, string.Empty);
	}

	public static void Add(string text, bool local)
	{
		UIMainStatus.Add(text, local, 2f, string.Empty);
	}

	public static void Add(string text, bool local, float duration)
	{
		UIMainStatus.Add(text, local, duration, string.Empty);
	}

	public static void Add(string text, bool local, float duration, string localize)
	{
		if (local)
		{
			UIMainStatus.Show(text, duration);
		}
		else
		{
			PhotonDataWrite data = PhotonRPC.GetData();
			data.Write(text);
			data.Write(duration);
			data.Write(localize);
			PhotonRPC.RPC("PhotonShow", PhotonTargets.All, data);
		}
	}

	[PunRPC]
	private void PhotonShow(PhotonMessage message)
	{
		string text = message.ReadString();
		float duration = message.ReadFloat();
		string text2 = message.ReadString();
		if (string.IsNullOrEmpty(text2))
		{
			UIMainStatus.Show(text, duration);
		}
		else
		{
			text2 = Localization.Get(text2, true);
			text = text.Replace("[@]", text2);
			UIMainStatus.Show(text, duration);
		}
	}

	public static void Show(string text)
	{
		UIMainStatus.Show(text, 5f);
	}

	public static void Show(string text, float duration)
	{
		UIMainStatus.instance.label.text = text;
		TimerManager.Cancel("MainStatus");
		TimerManager.In("MainStatus", duration, new TimerManager.Callback(UIMainStatus.instance.Clear));
	}

	private void Clear()
	{
		UIMainStatus.instance.label.text = string.Empty;
	}	
}
