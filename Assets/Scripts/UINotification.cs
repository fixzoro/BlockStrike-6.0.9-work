using System;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : MonoBehaviour
{
    public GameObject Button;

    public UILabel ButtonCountLabel;

    public GameObject Panel;

    public UILabel Label;

    private List<UINotification.Data> Datas = new List<UINotification.Data>();

    private UINotification.Data SelectData;

    private bool isShowButton;

    private bool isShowPanel;

    private int NextID = -1;

    private static UINotification instance;

    private void Start()
	{
		UINotification.instance = this;
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	public static int Add(string text, Action positive, Action negative)
	{
		UINotification.Data data = new UINotification.Data();
		UINotification.instance.NextID++;
		data.ID = UINotification.instance.NextID;
		data.Text = text;
		data.PositiveAction = positive;
		data.NegativeAction = negative;
		UINotification.instance.Datas.Add(data);
		if (!UINotification.instance.isShowButton)
		{
			UINotification.instance.SetShowButton(true);
		}
		UINotification.instance.ButtonCountLabel.text = UINotification.instance.Datas.Count.ToString();
		return UINotification.instance.NextID;
	}

	private void GetButtonDown(string button)
	{
		if (button == "Notification")
		{
			this.Show();
		}
	}

	public void Show()
	{
		if (this.isShowPanel)
		{
			return;
		}
		if (this.Datas.Count == 0)
		{
			this.SetShowButton(false);
			this.SetShowPanel(false);
			return;
		}
		this.SelectData = this.Datas[0];
		this.Datas.Remove(this.SelectData);
		if (this.Datas.Count == 0)
		{
			this.SetShowButton(false);
		}
		else
		{
			this.ButtonCountLabel.text = this.Datas.Count.ToString();
		}
		this.SetShowPanel(true);
	}

	private void SetShowPanel(bool active)
	{
		this.isShowPanel = active;
		this.Panel.SetActive(active);
		if (this.isShowPanel)
		{
			this.Label.text = this.SelectData.Text;
		}
	}

	private void SetShowButton(bool active)
	{
		this.isShowButton = active;
		this.Button.SetActive(active);
	}

	public void OnClickPositive()
	{
		if (this.SelectData != null || this.SelectData.PositiveAction != null)
		{
			this.SelectData.PositiveAction();
		}
		UINotification.Close();
	}

	public void OnClickNegative()
	{
		if (this.SelectData != null || this.SelectData.NegativeAction != null)
		{
			this.SelectData.NegativeAction();
		}
		UINotification.Close();
	}

	public static void Close()
	{
		UINotification.instance.SetShowPanel(false);
		if (UINotification.instance.Datas.Count == 0)
		{
			UINotification.instance.SetShowButton(false);
		}
		else
		{
			UINotification.instance.ButtonCountLabel.text = UINotification.instance.Datas.Count.ToString();
		}
		UINotification.instance.SelectData = null;
	}

	public static void Remove(int id)
	{
		if (UINotification.instance.SelectData != null && UINotification.instance.SelectData.ID == id)
		{
			UINotification.Close();
		}
		else
		{
			for (int i = 0; i < UINotification.instance.Datas.Count; i++)
			{
				if (UINotification.instance.Datas[i].ID == id)
				{
					UINotification.instance.Datas.RemoveAt(i);
				}
			}
			if (UINotification.instance.Datas.Count == 0)
			{
				UINotification.instance.SetShowButton(false);
				UINotification.instance.SetShowPanel(false);
			}
			else
			{
				UINotification.instance.ButtonCountLabel.text = UINotification.instance.Datas.Count.ToString();
			}
		}
	}

	[Serializable]
	public class Data
	{
		public int ID;

		public string Text;

		public Action PositiveAction;

		public Action NegativeAction;
	}
}
