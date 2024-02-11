using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    private static bool autoErrorShow = false;

    public static bool isCreated;

    public static Action<string, string, LogType> LogCallback;

    public static List<string> list = new List<string>();

    private void Start()
	{
		ConsoleManager.autoErrorShow = GameConsole.Load<bool>("auto_error_show", false);
		Application.RegisterLogCallback(new Application.LogCallback(this.OnLogCallback));
	}

	private void OnEnable()
	{
		ConsoleManager.isCreated = true;
	}

	private void OnDisable()
	{
		ConsoleManager.isCreated = false;
	}

	public static void Log(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=grey>Log:</color> " + message);
		ConsoleManager.list.Add(stringBuilder.ToString());
	}

	public static void LogWarning(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=yellow>Warning:</color> " + message);
		ConsoleManager.list.Add(stringBuilder.ToString());
	}

	public static void LogError(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<color=red>Error:</color> " + message);
		ConsoleManager.list.Add(stringBuilder.ToString());
	}

	private void OnLogCallback(string message, string stackTrace, LogType type)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (type)
		{
		case LogType.Error:
			stringBuilder.AppendLine("<color=red>Error:</color> " + message);
			if (ConsoleManager.autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error", true));
			}
			break;
		case LogType.Assert:
			stringBuilder.AppendLine("<color=red>Assert:</color> " + message);
			if (ConsoleManager.autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error", true));
			}
			break;
		case LogType.Warning:
			stringBuilder.AppendLine("<color=yellow>Warning:</color> " + message);
			break;
		case LogType.Log:
			stringBuilder.AppendLine("<color=grey>Log:</color> " + message);
			break;
		case LogType.Exception:
			stringBuilder.AppendLine("<color=red>Exception:</color> " + message);
			if (ConsoleManager.autoErrorShow)
			{
				UIToast.Show(Localization.Get("Error", true));
			}
			break;
		}
		stringBuilder.Append("<color=grey>" + stackTrace + "</color>");
		ConsoleManager.list.Add(stringBuilder.ToString());
		if (ConsoleManager.LogCallback != null)
		{
			ConsoleManager.LogCallback(message, stackTrace, type);
		}
	}
}
