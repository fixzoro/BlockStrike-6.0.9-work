using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIChat : MonoBehaviour
{
    public UILabel label;

    public UIInput input;

    public UISprite background;

    private StringBuilder builder = new StringBuilder();

    private List<int> list = new List<int>();

    private static UIChat instance;

    private float time;

    private float maxTime;

    public static bool actived;

    private static float textShowDuration = 8f;

    private void Start()
	{
		UIChat.instance = this;
		PhotonRPC.AddMessage("PhotonChatNewLine", new PhotonRPC.MessageDelegate(this.PhotonChatNewLine));
		UIChat.textShowDuration = Mathf.Clamp(GameConsole.Load<float>("chat_show_duration", 8f), 1f, 20f);
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void UpdateBackground()
	{
		if (string.IsNullOrEmpty(this.label.text))
		{
			this.background.alpha = 0f;
		}
		else
		{
			this.background.height = this.label.height + 6;
			this.background.alpha = 0.2f;
		}
		this.background.UpdateWidget();
	}

	private void GetButtonDown(string name)
	{
		if (name == "Chat")
		{
			this.Show();
		}
	}

	public static void Add(string text)
	{
		if (UIChat.instance.time + UIChat.instance.maxTime + 2f > Time.time)
		{
			UIChat.instance.maxTime += 1f;
		}
		else
		{
			UIChat.instance.maxTime = 0f;
		}
		UIChat.instance.time = Time.time;
		bool value = text[0] == '.';
		PhotonDataWrite data = PhotonRPC.GetData();
		data.Write(text);
		data.Write(value);
		PhotonRPC.RPC("PhotonChatNewLine", PhotonTargets.All, data);
    }

	[PunRPC]
	private void PhotonChatNewLine(PhotonMessage message)
	{
		string text = message.ReadString();
		bool flag = message.ReadBool();
		PhotonPlayer sender = message.sender;
		if (Settings.FilterChat)
		{
			text = BadWordsManager.Check(NGUIText.StripSymbols(text));
		}
		else
		{
			text = NGUIText.StripSymbols(text);
		}
		if (flag)
		{
			text = text.Remove(0, 1);
			text = UIStatus.GetTeamHexColor("[Team] " + sender.UserId, sender.GetTeam()) + ": " + text;
		}
		else
		{
			text = UIStatus.GetTeamHexColor(sender) + ": " + text;
		}
		if (GameManager.globalChat)
		{
			if (flag)
			{
				if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
				{
					UIChat.NewLine(text);
				}
			}
			else
			{
				UIChat.NewLine(text);
			}
		}
		else if (sender.GetDead())
		{
			text = text.Insert(text.IndexOf("]") + 1, "[" + Localization.Get("Dead", true) + "] ");
			if (PhotonNetwork.player.GetDead())
			{
				if (flag)
				{
					if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
					{
						UIChat.NewLine(text);
					}
				}
				else
				{
					UIChat.NewLine(text);
				}
			}
		}
		else if (flag)
		{
			if (PhotonNetwork.player.GetTeam() == sender.GetTeam())
			{
				UIChat.NewLine(text);
			}
		}
		else
		{
			UIChat.NewLine(text);
		}
	}

	private void Show()
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
		if (GameManager.loadingLevel)
		{
			return;
		}
		if (this.time + this.maxTime > Time.time)
		{
			UIChat.NewLine("Message sending limit " + StringCache.Get(Mathf.CeilToInt(this.time + this.maxTime - Time.time)) + " sec");
			return;
		}
		if (!UICamera.inputHasFocus)
		{
			this.input.value = string.Empty;
			TimerManager.In(0.1f, delegate()
			{
				UICamera.selectedObject = this.input.gameObject;
				UIChat.actived = true;
			});
		}
	}

	public void OnSubmit()
	{
		string text = this.input.value;
		text = text.Replace("\n", string.Empty);
		if (!string.IsNullOrEmpty(text))
		{
			this.input.value = string.Empty;
			this.input.isSelected = false;
			TimerManager.In(0.1f, delegate()
			{
				UIChat.actived = false;
			});
			UIChat.Add(text);
		}
	}

    public static void NewLine(string text)
	{
		nProfiler.BeginSample("UIChat.NewLine");
		if (!Settings.Chat)
		{
			return;
		}
		if (UIChat.instance.builder.Length > 0)
		{
			UIChat.instance.builder.Insert(0, text + "\n");
		}
		else
		{
			UIChat.instance.builder.Append(text);
		}
		UIChat.instance.list.Add(text.Length);
		UIChat.instance.UpdateLabel(true);
		nProfiler.EndSample();
	}

	private void UpdateLabel(bool clear)
	{
		nProfiler.BeginSample("UIChat.UpdateLabel");
		this.label.text = this.builder.ToString();
		TimerManager.In(0.2f, new TimerManager.Callback(this.UpdateBackground));
		if (clear)
		{
			TimerManager.In(UIChat.textShowDuration, new TimerManager.Callback(this.RemoveLabel));
		}
		nProfiler.EndSample();
	}

	private void RemoveLabel()
	{
		if (this.list.Count == 1)
		{
			this.builder.Length = 0;
			this.builder.Capacity = 0;
		}
		else
		{
			this.builder.Remove(this.builder.Length - (this.list[0] + 1), this.list[0] + 1);
		}
		this.list.RemoveAt(0);
		this.UpdateLabel(false);
	}
}
