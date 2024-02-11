using System;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    public UILabel Text;

    public UILabel Title;

    public UILabel Button1;

    public UILabel Button2;

    public Action Button1Action;

    public Action Button2Action;

    public static Action ClickButton;

    private static UIPopUp instance;

    private void Start()
	{
		UIPopUp.instance = this;
	}

	public static void ShowPopUp(string text, string title, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		UIPanelManager.ShowPanel("Popup");
		UIPopUp.instance.Text.text = text;
		UIPopUp.instance.Title.text = title;
		UIPopUp.instance.Button1.text = button1Text;
		UIPopUp.instance.Button2.text = button2Text;
		UIPopUp.instance.Button1Action = callbackButton1;
		UIPopUp.instance.Button2Action = callbackButton2;
	}

	public void OnClickButton1()
	{
		this.Button1Action();
		if (UIPopUp.ClickButton != null)
		{
			UIPopUp.ClickButton();
		}
	}

	public void OnClickButton2()
	{
		this.Button2Action();
		if (UIPopUp.ClickButton != null)
		{
			UIPopUp.ClickButton();
		}
	}
}
