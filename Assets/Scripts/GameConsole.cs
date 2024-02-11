using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameConsole : MonoBehaviour
{
    private KeyCode key = KeyCode.Escape;

    private GUIStyle style = new GUIStyle();

    private GUIStyle styleButton = new GUIStyle();

    private GUIStyle styleButton2 = new GUIStyle();

    private GUIStyle styleText = new GUIStyle();

    private Texture2D background;

    private Texture2D background2;

    private Vector2 scroll;

    private static Dictionary<string, GameConsole.CommandData> commands;

    private List<string> commandsContains = new List<string>();

    private int selectCommandsIndex;

    private List<string> commandsHistory = new List<string>();

    private string text = string.Empty;

    private bool showGUI;

    private float dpi;

    private Rect[] rects;

    private static GameConsole instance;

    public static string lastInvokeCommand;

    public static bool actived
	{
		set
		{
			if (value)
			{
				if (GameConsole.instance != null)
				{
					return;
				}
				GameObject gameObject = new GameObject("GameConsole");
				gameObject.AddComponent<GameConsole>();
				if (!ConsoleManager.isCreated)
				{
					gameObject.AddComponent<ConsoleManager>();
				}
				if (GameConsole.commands == null)
				{
					GameConsole.commands = new Dictionary<string, GameConsole.CommandData>();
					MethodInfo[] methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
					for (int i = 0; i < methods.Length; i++)
					{
						ConsoleAttribute consoleAttribute = (ConsoleAttribute)methods[i].GetCustomAttributes(typeof(ConsoleAttribute), true)[0];
						for (int j = 0; j < consoleAttribute.commands.Length; j++)
						{
							GameConsole.CommandData commandData = new GameConsole.CommandData();
							commandData.method = methods[i];
							ParameterInfo[] parameters = commandData.method.GetParameters();
							if (parameters != null && parameters.Length == 1)
							{
								commandData.value = GameConsole.GetValueType(parameters[0].ParameterType);
							}
							GameConsole.commands.Add(consoleAttribute.commands[j], commandData);
						}
					}
				}
			}
			else if (GameConsole.instance != null)
			{
				UnityEngine.Object.Destroy(GameConsole.instance.gameObject);
			}
		}
	}

	private void Awake()
	{
		GameConsole.instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		this.rects = new Rect[6];
		this.rects[0] = this.xRect(2f, 2f, 96f, 88f);
		this.rects[1] = this.xRect(2f, 91f, 40f, 8f);
		this.rects[2] = this.xRect(57f, 91f, 20f, 8f);
		this.rects[3] = this.xRect(78f, 91f, 20f, 8f);
		this.rects[4] = this.xRect(43f, 91f, 6f, 8f);
		this.rects[5] = this.xRect(50f, 91f, 6f, 8f);
		this.background = this.CreateTexture2D(new Color32(0, 0, 0, 180));
		this.background2 = this.CreateTexture2D(new Color32(100, 100, 100, byte.MaxValue));
		int num = (int)(13f * (float)Screen.width / 800f);
		this.style.normal.textColor = Color.white;
		this.style.fontSize = num;
		this.style.padding.left = 4;
		this.styleButton.normal.textColor = Color.white;
		this.styleButton.normal.background = this.background;
		this.styleButton.fontSize = num;
		this.styleButton.padding.left = 4;
		this.styleButton.alignment = TextAnchor.MiddleCenter;
		this.styleText.normal.textColor = Color.white;
		this.styleText.normal.background = this.background;
		this.styleText.fontSize = num;
		this.styleText.padding.left = 4;
		this.styleText.alignment = TextAnchor.MiddleLeft;
		this.styleButton2.normal.textColor = Color.white;
		this.styleButton2.normal.background = this.background2;
		this.styleButton2.fontSize = num - 1;
		this.styleButton2.padding.left = 4;
		this.styleButton2.contentOffset = new Vector2(15f, 0f);
		this.styleButton2.alignment = TextAnchor.MiddleLeft;
		this.dpi = Screen.dpi / 100f;
		if (this.dpi == 0f)
		{
			this.dpi = 1.6f;
		}
	}

	public static void Save(string command, int value)
	{
		PlayerPrefs.SetInt("console:" + command, value);
	}

	public static void Save(string command, float value)
	{
		PlayerPrefs.SetFloat("console:" + command, value);
	}

	public static void Save(string command, bool value)
	{
		PlayerPrefs.SetInt("console:" + command, (!value) ? 0 : 1);
	}

	public static void Save(string command, Color32 value)
	{
		string value2 = string.Concat(new object[]
		{
			value.r,
			"|",
			value.g,
			"|",
			value.b,
			"|",
			value.a
		});
		PlayerPrefs.SetString("console:" + command, value2);
	}

	public static T Load<T>(string command)
	{
		return GameConsole.Load<T>(command, default(T));
	}

	public static T Load<T>(string command, T defaultValue)
	{
		if (typeof(T) == typeof(int))
		{
			return (T)((object)PlayerPrefs.GetInt("console:" + command, (int)((object)defaultValue)));
		}
		if (typeof(T) == typeof(float))
		{
			return (T)((object)PlayerPrefs.GetFloat("console:" + command, (float)((object)defaultValue)));
		}
		if (typeof(T) == typeof(bool))
		{
			return (T)((object)(PlayerPrefs.GetInt("console:" + command, (!(bool)((object)defaultValue)) ? 0 : 1) == 1));
		}
		if (typeof(T) == typeof(Color32))
		{
			Color32 color = (Color32)((object)defaultValue);
			string @string = PlayerPrefs.GetString("console:" + command, string.Concat(new object[]
			{
				color.r,
				"|",
				color.g,
				"|",
				color.b,
				"|",
				color.a
			}));
			string[] array = @string.Split(new char[]
			{
				"|"[0]
			});
			color.r = byte.Parse(array[0]);
			color.g = byte.Parse(array[1]);
			color.b = byte.Parse(array[2]);
			color.a = byte.Parse(array[3]);
			return (T)((object)color);
		}
		return defaultValue;
	}

	private static GameConsole.Value GetValueType(Type type)
	{
		if (type == typeof(int))
		{
			return GameConsole.Value.Int;
		}
		if (type == typeof(float))
		{
			return GameConsole.Value.Float;
		}
		if (type == typeof(bool))
		{
			return GameConsole.Value.Bool;
		}
		if (type == typeof(string))
		{
			return GameConsole.Value.String;
		}
		if (type == typeof(Vector2))
		{
			return GameConsole.Value.Vector2;
		}
		if (type == typeof(Vector3))
		{
			return GameConsole.Value.Vector3;
		}
		if (type == typeof(Color))
		{
			return GameConsole.Value.Color;
		}
		return GameConsole.Value.None;
	}

	private void Update()
	{
		if (Input.GetKeyDown(this.key))
		{
			this.showGUI = !this.showGUI;
		}
		if (Input.touchCount == 1)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Moved)
			{
				this.scroll += touch.deltaPosition / this.dpi;
			}
		}
	}

	private void OnGUI()
	{
		if (!this.showGUI)
		{
			return;
		}
		GUI.DrawTextureWithTexCoords(this.rects[0], this.background, new Rect(0f, 0f, 1f, 1f), true);
		GUILayout.BeginArea(this.rects[0]);
		this.scroll = GUILayout.BeginScrollView(this.scroll, new GUILayoutOption[0]);
		GUILayout.Space(2f);
		for (int i = 0; i < ConsoleManager.list.Count; i++)
		{
			GUILayout.Label(ConsoleManager.list[i], this.style, new GUILayoutOption[0]);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUI.changed = false;
		this.text = GUI.TextField(this.rects[1], this.text, 300, this.styleText);
		if (GUI.changed)
		{
			this.selectCommandsIndex = 0;
			this.text = this.text.Replace("\n", string.Empty);
			this.commandsContains.Clear();
			if (this.text != string.Empty)
			{
				for (int j = 0; j < GameConsole.commands.Count; j++)
				{
					if (this.Contains(this.text, GameConsole.commands.Keys.ElementAt(j)))
					{
						this.commandsContains.Add(GameConsole.commands.Keys.ElementAt(j));
					}
				}
			}
		}
		for (int k = 0; k < this.commandsContains.Count; k++)
		{
			if (GUI.Button(this.xRect(2.5f, (float)(85 - k * 6), 30f, 6f), this.commandsContains[k] + " " + this.Format(GameConsole.commands[this.commandsContains[k]].value), this.styleButton2))
			{
				this.text = this.commandsContains[k];
				this.commandsContains.Clear();
				break;
			}
		}
		if (GUI.Button(this.rects[4], "<<", this.styleButton))
		{
			this.selectCommandsIndex--;
			this.UpdateArrowCommand();
		}
		if (GUI.Button(this.rects[5], ">>", this.styleButton))
		{
			this.selectCommandsIndex++;
			this.UpdateArrowCommand();
		}
		if (GUI.Button(this.rects[2], "Submit", this.styleButton) && !string.IsNullOrEmpty(this.text))
		{
			this.text = this.text.Replace("\n", string.Empty);
			GameConsole.Log(">" + this.text);
			this.commandsHistory.Add(this.text);
			if (this.text == "list")
			{
				if (GameConsole.commands.Count != 0)
				{
					this.text = string.Empty;
					GameConsole.Log("/////////////////////////////////////////////////");
					for (int l = 0; l < GameConsole.commands.Count; l++)
					{
						GameConsole.Log(GameConsole.commands.Keys.ElementAt(l));
					}
					GameConsole.Log("/////////////////////////////////////////////////");
				}
			}
			else
			{
				this.OnCommand();
			}
		}
		if (GUI.Button(this.rects[3], "Clear", this.styleButton))
		{
			ConsoleManager.list.Clear();
		}
	}

	private void OnCommand()
	{
		string[] array = this.text.Split(new char[]
		{
			' '
		}, 2);
		if (array.Length < 3 && GameConsole.commands.ContainsKey(array[0]))
		{
			switch (GameConsole.commands[array[0]].value)
			{
			case GameConsole.Value.None:
				if (array.Length == 1)
				{
					GameConsole.lastInvokeCommand = array[0];
					GameConsole.commands[array[0]].method.Invoke(this, null);
				}
				else
				{
					GameConsole.LogError("Command error");
				}
				break;
			case GameConsole.Value.Int:
			case GameConsole.Value.Float:
			case GameConsole.Value.Bool:
			case GameConsole.Value.String:
			case GameConsole.Value.Vector2:
			case GameConsole.Value.Vector3:
			case GameConsole.Value.Color:
			{
				object obj;
				if (array.Length == 2 && this.Parse(array[1], out obj, GameConsole.commands[array[0]].value))
				{
					GameConsole.lastInvokeCommand = array[0];
					GameConsole.commands[array[0]].method.Invoke(true, new object[]
					{
						obj
					});
				}
				else
				{
					GameConsole.LogError("Command error");
				}
				break;
			}
			}
		}
		else
		{
			GameConsole.LogError("Command not found: " + array[0]);
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

	private string Format(GameConsole.Value valueType)
	{
		switch (valueType)
		{
		case GameConsole.Value.Int:
			return "[int]";
		case GameConsole.Value.Float:
			return "[float]";
		case GameConsole.Value.Bool:
			return "[bool]";
		case GameConsole.Value.String:
			return "[string]";
		case GameConsole.Value.Vector2:
			return "[vector2]";
		case GameConsole.Value.Vector3:
			return "[vector3]";
		case GameConsole.Value.Color:
			return "[color]";
		default:
			return string.Empty;
		}
	}

	private bool Parse(string value, out object result, GameConsole.Value valueType)
	{
		switch (valueType)
		{
		case GameConsole.Value.Int:
		{
			int num = 0;
			if (int.TryParse(value, out num))
			{
				result = num;
				return true;
			}
			break;
		}
		case GameConsole.Value.Float:
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
		case GameConsole.Value.Bool:
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
		case GameConsole.Value.String:
			result = value;
			return true;
		case GameConsole.Value.Vector2:
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
		case GameConsole.Value.Vector3:
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
		case GameConsole.Value.Color:
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
		GameConsole.instance.showGUI = show;
	}

	private Rect xRect(Rect rect)
	{
		return this.xRect(rect.x, rect.y, rect.width, rect.height);
	}

	private Rect xRect(float x, float y, float width, float height)
	{
		x = (float)Screen.width * x / 100f;
		y = (float)Screen.height * y / 100f;
		width = (float)Screen.width * width / 100f;
		height = (float)Screen.height * height / 100f;
		Rect result = new Rect(x, y, width, height);
		return result;
	}

	private Texture2D CreateTexture2D(Color color)
	{
		Texture2D texture2D = new Texture2D(2, 2);
		for (int i = 0; i < texture2D.width; i++)
		{
			for (int j = 0; j < texture2D.height; j++)
			{
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}

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

	public class CommandData
	{
		public GameConsole.Value value;

		public MethodInfo method;
	}
}
