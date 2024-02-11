using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class UIGameConsole : MonoBehaviour
{
    public UIInput input;

    public UITextList textList;

    private static Dictionary<string, UIGameConsole.CommandData> commands;

    private List<string> commandsContains = new List<string>();

    private int selectCommandsIndex;

    private List<string> commandsHistory = new List<string>();

    private string text = string.Empty;

    private bool showGUI;

    private static UIGameConsole instance;

    public static string lastInvokeCommand;

    public enum Value
    {
        None,
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector3,
        Color
    }

    private void Start()
	{
		this.GetCommands();
	}

	public void OnSubmit()
	{
		string text = this.input.value;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		text = text.Replace("\n", string.Empty);
		UIGameConsole.Log(">" + text);
		this.commandsHistory.Add(text);
		if (text == "list")
		{
			if (UIGameConsole.commands.Count != 0)
			{
				text = string.Empty;
				UIGameConsole.Log("/////////////////////////////////////////////////");
				for (int i = 0; i < UIGameConsole.commands.Count; i++)
				{
					UIGameConsole.Log(UIGameConsole.commands.Keys.ElementAt(i));
				}
				UIGameConsole.Log("/////////////////////////////////////////////////");
			}
		}
		else
		{
			this.OnCommand();
		}
	}

	public void OnClear()
	{
		this.textList.Clear();
	}

	private void GetCommands()
	{
		if (UIGameConsole.commands == null)
		{
			UIGameConsole.commands = new Dictionary<string, UIGameConsole.CommandData>();
			MethodInfo[] methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
			for (int i = 0; i < methods.Length; i++)
			{
				ConsoleAttribute consoleAttribute = (ConsoleAttribute)methods[i].GetCustomAttributes(typeof(ConsoleAttribute), true)[0];
				for (int j = 0; j < consoleAttribute.commands.Length; j++)
				{
					UIGameConsole.CommandData commandData = new UIGameConsole.CommandData();
					commandData.method = methods[i];
					ParameterInfo[] parameters = commandData.method.GetParameters();
					if (parameters != null && parameters.Length == 1)
					{
						commandData.value = UIGameConsole.GetValueType(parameters[0].ParameterType);
					}
					UIGameConsole.commands.Add(consoleAttribute.commands[j], commandData);
				}
			}
		}
	}

	private static UIGameConsole.Value GetValueType(Type type)
	{
		if (type == typeof(int))
		{
			return UIGameConsole.Value.Int;
		}
		if (type == typeof(float))
		{
			return UIGameConsole.Value.Float;
		}
		if (type == typeof(bool))
		{
			return UIGameConsole.Value.Bool;
		}
		if (type == typeof(string))
		{
			return UIGameConsole.Value.String;
		}
		if (type == typeof(Vector2))
		{
			return UIGameConsole.Value.Vector2;
		}
		if (type == typeof(Vector3))
		{
			return UIGameConsole.Value.Vector3;
		}
		if (type == typeof(Color))
		{
			return UIGameConsole.Value.Color;
		}
		return UIGameConsole.Value.None;
	}

	private void OnCommand()
	{
		string[] array = this.text.Split(new char[]
		{
			' '
		}, 2);
		if (array.Length < 3 && UIGameConsole.commands.ContainsKey(array[0]))
		{
			switch (UIGameConsole.commands[array[0]].value)
			{
			case UIGameConsole.Value.None:
				if (array.Length == 1)
				{
					UIGameConsole.lastInvokeCommand = array[0];
					UIGameConsole.commands[array[0]].method.Invoke(this, null);
				}
				else
				{
					UIGameConsole.LogError("Command error");
				}
				break;
			case UIGameConsole.Value.Int:
			case UIGameConsole.Value.Float:
			case UIGameConsole.Value.Bool:
			case UIGameConsole.Value.String:
			case UIGameConsole.Value.Vector2:
			case UIGameConsole.Value.Vector3:
			case UIGameConsole.Value.Color:
			{
				object obj;
				if (array.Length == 2 && this.Parse(array[1], out obj, UIGameConsole.commands[array[0]].value))
				{
					UIGameConsole.lastInvokeCommand = array[0];
					UIGameConsole.commands[array[0]].method.Invoke(true, new object[]
					{
						obj
					});
				}
				else
				{
					UIGameConsole.LogError("Command error");
				}
				break;
			}
			}
		}
		else
		{
			UIGameConsole.LogError("Command not found: " + array[0]);
		}
		this.text = string.Empty;
	}

	private bool Contains(string t, string t2)
	{
		if (t.Length > t2.Length)
		{
			return false;
		}
		for (int i = 0; i < t.Length; i++)
		{
			if (t[i] != t2[i])
			{
				return false;
			}
		}
		return true;
	}

	private string Format(UIGameConsole.Value valueType)
	{
		switch (valueType)
		{
		case UIGameConsole.Value.Int:
			return "[int]";
		case UIGameConsole.Value.Float:
			return "[float]";
		case UIGameConsole.Value.Bool:
			return "[bool]";
		case UIGameConsole.Value.String:
			return "[string]";
		case UIGameConsole.Value.Vector2:
			return "[vector2]";
		case UIGameConsole.Value.Vector3:
			return "[vector3]";
		case UIGameConsole.Value.Color:
			return "[color]";
		default:
			return string.Empty;
		}
	}

	private bool Parse(string value, out object result, UIGameConsole.Value valueType)
	{
		switch (valueType)
		{
		case UIGameConsole.Value.Int:
		{
			int num = 0;
			if (int.TryParse(value, out num))
			{
				result = num;
				return true;
			}
			break;
		}
		case UIGameConsole.Value.Float:
		{
			float num2 = 0f;
			value = value.Replace(",", ".");
			if (float.TryParse(value, out num2))
			{
				result = num2;
				return true;
			}
			break;
		}
		case UIGameConsole.Value.Bool:
		{
			bool flag = false;
			if (value == "1")
			{
				result = true;
				return true;
			}
			if (value == "0")
			{
				result = false;
				return true;
			}
			if (bool.TryParse(value, out flag))
			{
				result = flag;
				return true;
			}
			break;
		}
		case UIGameConsole.Value.String:
			result = value;
			return true;
		case UIGameConsole.Value.Vector2:
		{
			string[] array = value.Split(new char[]
			{
				' '
			});
			if (array.Length == 2)
			{
				float y = 0f;
				float x;
				if (float.TryParse(array[0], out x) && float.TryParse(array[1], out y))
				{
					result = new Vector2(x, y);
					return true;
				}
			}
			break;
		}
		case UIGameConsole.Value.Vector3:
		{
			string[] array2 = value.Split(new char[]
			{
				' '
			});
			if (array2.Length == 3)
			{
				float z = 0f;
				float x2;
				float y2;
				if (float.TryParse(array2[0], out x2) && float.TryParse(array2[1], out y2) && float.TryParse(array2[2], out z))
				{
					result = new Vector3(x2, y2, z);
					return true;
				}
			}
			break;
		}
		case UIGameConsole.Value.Color:
		{
			string[] array3 = value.Split(new char[]
			{
				' '
			});
			if (array3.Length == 4)
			{
				byte a = 0;
				byte r;
				byte g;
				byte b;
				if (byte.TryParse(array3[0], out r) && byte.TryParse(array3[1], out g) && byte.TryParse(array3[2], out b) && byte.TryParse(array3[3], out a))
				{
					result = new Color32(r, g, b, a);
					return true;
				}
			}
			break;
		}
		}
		result = null;
		return false;
	}

	private void UpdateArrowCommand()
	{
		if (this.text != " " && this.commandsContains.Count != 0)
		{
			if (this.selectCommandsIndex > this.commandsContains.Count - 1)
			{
				this.selectCommandsIndex = 0;
			}
			else if (this.selectCommandsIndex < 0)
			{
				this.selectCommandsIndex = this.commandsContains.Count - 1;
			}
			this.text = this.commandsContains[this.selectCommandsIndex];
		}
		else if (this.commandsHistory.Count != 0)
		{
			if (this.selectCommandsIndex > this.commandsHistory.Count - 1)
			{
				this.selectCommandsIndex = 0;
			}
			else if (this.selectCommandsIndex < 0)
			{
				this.selectCommandsIndex = this.commandsHistory.Count - 1;
			}
			this.text = this.commandsHistory[this.selectCommandsIndex];
		}
	}

	public static void Log(string message)
	{
		ConsoleManager.Log(message);
	}

	public static void LogWarning(string message)
	{
		ConsoleManager.LogWarning(message);
	}

	public static void LogError(string message)
	{
		ConsoleManager.LogError(message);
	}

	public static void ShowConsole(bool show)
	{
		UIGameConsole.instance.showGUI = show;
	}

	public class CommandData
	{
		public UIGameConsole.Value value;

		public MethodInfo method;
	}
}
