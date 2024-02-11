using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMessage : MonoBehaviour
{
    public UITexture messageSprite;

    public UILabel messageCountLabel;

    public UISprite pauseMessageSprite;

    public List<UIMessage.MessageClass> Messages = new List<UIMessage.MessageClass>();

    private static UIMessage instance;

    private void Start()
	{
		UIMessage.instance = this;
	}

	public static void Add(string text, string title, Action<bool> callback)
	{
		UIMessage.MessageClass messageClass = new UIMessage.MessageClass();
		messageClass.callback = callback;
		messageClass.title = title;
		messageClass.text = text;
		UIMessage.instance.Messages.Add(messageClass);
		UIMessage.instance.messageSprite.cachedGameObject.SetActive(true);
		UIMessage.instance.messageCountLabel.text = UIMessage.instance.Messages.Count.ToString();
		UIMessage.instance.pauseMessageSprite.cachedGameObject.SetActive(true);
	}

	public static void Add(string text, string title, object data, Action<bool, object> callback)
	{
		UIMessage.MessageClass messageClass = new UIMessage.MessageClass();
		messageClass.data = data;
		messageClass.callback2 = callback;
		messageClass.title = title;
		messageClass.text = text;
		UIMessage.instance.Messages.Add(messageClass);
		UIMessage.instance.messageSprite.cachedGameObject.SetActive(true);
		UIMessage.instance.messageCountLabel.text = UIMessage.instance.Messages.Count.ToString();
		UIMessage.instance.pauseMessageSprite.cachedGameObject.SetActive(true);
	}

	public void Click()
	{
		if (this.Messages.Count == 0)
		{
			this.messageSprite.cachedGameObject.SetActive(false);
			return;
		}
		UIPopUp.ShowPopUp(this.Messages[0].text, this.Messages[0].title, Localization.Get("No", true), new Action(this.No), Localization.Get("Yes", true), new Action(this.Yes));
	}

	private void No()
	{
		UIPanelManager.ShowPanel("Pause");
		if (this.Messages[0].callback != null)
		{
			this.Messages[0].callback(false);
		}
		if (this.Messages[0].callback2 != null)
		{
			this.Messages[0].callback2(false, this.Messages[0].data);
		}
		this.Messages.RemoveAt(0);
		this.messageCountLabel.text = this.Messages.Count.ToString();
		if (this.Messages.Count == 0)
		{
			this.messageSprite.cachedGameObject.SetActive(false);
			this.pauseMessageSprite.cachedGameObject.SetActive(false);
		}
	}

	private void Yes()
	{
		UIPanelManager.ShowPanel("Pause");
		if (this.Messages[0].callback != null)
		{
			this.Messages[0].callback(true);
		}
		if (this.Messages[0].callback2 != null)
		{
			this.Messages[0].callback2(true, this.Messages[0].data);
		}
		this.Messages.RemoveAt(0);
		this.messageCountLabel.text = this.Messages.Count.ToString();
		if (this.Messages.Count == 0)
		{
			this.messageSprite.cachedGameObject.SetActive(false);
			this.pauseMessageSprite.cachedGameObject.SetActive(false);
		}
	}

	public class MessageClass
	{
		public string title;

		public string text;

		public Action<bool> callback;

		public Action<bool, object> callback2;

		public object data;
	}
}
