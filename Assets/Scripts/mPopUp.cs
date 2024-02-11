using System;
using UnityEngine;

public class mPopUp : MonoBehaviour
{
    [Header("Popup")]
    public UIPanel PopupPanel;

    public UILabel PopupTitleLabel;

    public UILabel PopupTextLabel;

    public UILabel PopupButton1;

    public UILabel PopupButton2;

    public static bool activePopup;

    [Header("Text Line")]
    public UIPanel TextLinePanel;

    public UILabel TextLineLabel;

    public static bool activeText;

    [Header("Input")]
    public UIPanel InputPanel;

    public UILabel InputTitleLabel;

    public UILabel InputTextLabel;

    public UIInput InputField;

    public UILabel InputButton1;

    public UILabel InputButton2;

    private Action InputAction;

    private Action InputChange;

    public static bool activeInput;

    [Header("Wait")]
    public GameObject WaitSprite;

    public UILabel WaitLabel;

    public GameObject WaitPanel;

    public UILabel WaitPanelLabel;

    private Action Button1Action;

    private Action Button2Action;

    private static mPopUp instance;

    private void Awake()
	{
		mPopUp.instance = this;
	}

	public static void SetActiveWait(bool active)
	{
		mPopUp.SetActiveWait(active, string.Empty);
	}

	public static void SetActiveWait(bool active, string text)
	{
		mPopUp.instance.WaitSprite.SetActive(active);
		if (active)
		{
			mPopUp.instance.WaitLabel.text = text;
		}
	}

	public static void SetActiveWaitPanel(bool active)
	{
		mPopUp.SetActiveWaitPanel(active, string.Empty);
	}

	public static void SetActiveWaitPanel(bool active, string text)
	{
		mPopUp.instance.WaitPanel.SetActive(active);
		if (active)
		{
			mPopUp.instance.WaitPanelLabel.text = text;
		}
	}

	public static void ShowText(string text)
	{
		mPopUp.ShowText(text, 0f, string.Empty);
	}

	public static void ShowText(string text, float duration, string panel)
	{
		mPopUp.Hide();
		mPopUp.activeText = true;
		mPanelManager.Hide();
		mPopUp.instance.TextLinePanel.alpha = 0f;
		mPanelManager.ShowTween(mPopUp.instance.TextLinePanel.cachedGameObject);
		mPopUp.instance.TextLineLabel.text = text;
		if (duration != 0f)
		{
			TimerManager.In(duration, delegate()
			{
				mPopUp.HideAll(panel);
			});
		}
	}

	public static void ShowPopup(string text, string title, string buttonText, Action callbackButton)
	{
		mPopUp.ShowPopup(text, title, string.Empty, null, buttonText, callbackButton);
	}

	public static void ShowPopup(string text, string title, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		mPopUp.Hide();
		mPopUp.activePopup = true;
		mPopUp.instance.PopupPanel.alpha = 0f;
		mPanelManager.ShowTween(mPopUp.instance.PopupPanel.cachedGameObject);
		mPopUp.instance.PopupTextLabel.text = text;
		mPopUp.instance.PopupTitleLabel.text = title;
		if (string.IsNullOrEmpty(button1Text))
		{
			mPopUp.instance.PopupButton1.cachedGameObject.SetActive(false);
		}
		else
		{
			mPopUp.instance.PopupButton1.cachedGameObject.SetActive(true);
			mPopUp.instance.PopupButton1.text = button1Text;
			mPopUp.instance.Button1Action = callbackButton1;
		}
		mPopUp.instance.PopupButton2.text = button2Text;
		mPopUp.instance.Button2Action = callbackButton2;
	}

	public static void SetPopupText(string text)
	{
		mPopUp.instance.PopupTextLabel.text = text;
	}

	public void OnClickButton(int button)
	{
		if (button != 1)
		{
			if (button == 2)
			{
				if (this.Button2Action != null)
				{
					this.Button2Action();
				}
			}
		}
		else if (this.Button1Action != null)
		{
			this.Button1Action();
		}
	}

	public static void ShowInput(string text, string title, int limit, UIInput.KeyboardType keyboardType, Action callbackInput, Action callbackChange, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		mPopUp.Hide();
		mPopUp.activeInput = true;
		mPanelManager.ShowTween(mPopUp.instance.InputPanel.cachedGameObject);
		mPopUp.instance.InputTextLabel.text = text;
		mPopUp.instance.InputTitleLabel.text = title;
		mPopUp.instance.InputField.characterLimit = limit;
		mPopUp.instance.InputField.keyboardType = keyboardType;
		mPopUp.instance.InputAction = callbackInput;
		mPopUp.instance.InputChange = callbackChange;
		mPopUp.instance.InputField.value = text;
		mPopUp.instance.InputButton1.text = button1Text;
		mPopUp.instance.Button1Action = callbackButton1;
		mPopUp.instance.InputButton2.text = button2Text;
		mPopUp.instance.Button2Action = callbackButton2;
	}

	public void OnInputSubmit()
	{
		if (this.InputAction != null)
		{
			this.InputAction();
		}
	}

	public void OnInputChange()
	{
		if (this.InputChange != null)
		{
			this.InputChange();
		}
	}

	public static string GetInputText()
	{
		return mPopUp.instance.InputField.value;
	}

	public static void SetInputText(string text)
	{
		mPopUp.instance.InputField.value = text;
	}

	private static void Hide()
	{
		if (mPopUp.activeText)
		{
			mPanelManager.HideTween(mPopUp.instance.TextLinePanel.cachedGameObject);
		}
		if (mPopUp.activePopup)
		{
			mPanelManager.HideTween(mPopUp.instance.PopupPanel.cachedGameObject);
		}
		if (mPopUp.activeInput)
		{
			mPanelManager.HideTween(mPopUp.instance.InputPanel.cachedGameObject);
		}
		mPopUp.activeText = false;
		mPopUp.activePopup = false;
		mPopUp.activeInput = false;
	}

	public static void HideAll()
	{
		mPopUp.HideAll(string.Empty);
	}

	public static void HideAll(string panel)
	{
		mPopUp.HideAll(panel, true);
	}

	public static void HideAll(string panel, bool playerData)
	{
		if (mPopUp.activeText)
		{
			mPanelManager.HideTween(mPopUp.instance.TextLinePanel.cachedGameObject);
		}
		if (mPopUp.activePopup)
		{
			mPanelManager.HideTween(mPopUp.instance.PopupPanel.cachedGameObject);
		}
		if (mPopUp.activeInput)
		{
			mPanelManager.HideTween(mPopUp.instance.InputPanel.cachedGameObject);
		}
		mPopUp.activeText = false;
		mPopUp.activePopup = false;
		mPopUp.activeInput = false;
		if (!string.IsNullOrEmpty(panel))
		{
			mPanelManager.Show(panel, playerData);
		}
		else if (mPanelManager.select == null)
		{
			if (mPanelManager.last == null)
			{
				mPanelManager.Show("Menu", playerData);
			}
			else
			{
				mPanelManager.Show(mPanelManager.last.name, playerData);
			}
		}
	}
}
