using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static BetterList<InputManager.ButtonData> buttons = new BetterList<InputManager.ButtonData>();

    private static BetterList<InputManager.AxisData> axis = new BetterList<InputManager.AxisData>();

    public static InputManager.ButtonDelegate GetButtonDownEvent;

    public static InputManager.ButtonDelegate GetButtonEvent;

    public static InputManager.ButtonDelegate GetButtonUpEvent;

    public static InputManager.AxisDelegate GetAxisEvent;

    private static InputManager instance;

    private void Start()
	{
		InputManager.instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void AddButton(string name, KeyCode key)
	{
		InputManager.ButtonData item = default(InputManager.ButtonData);
		item.name = name;
		item.key = key;
		InputManager.buttons.Add(item);
	}

	private void AddAxis(string name, bool raw)
	{
		InputManager.AxisData item = default(InputManager.AxisData);
		item.name = name;
		item.raw = raw;
		InputManager.axis.Add(item);
	}

	public static void Init()
	{
		if (InputManager.instance == null)
		{
			new GameObject("InputManager").AddComponent<InputManager>();
		}
	}

    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private void Update()
    {
        if(GameObject.Find("Display") != null)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SetButtonDown("Fire");
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SetButtonUp("Fire");
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                SetButtonDown("Aim");
                UIRoot.uiroot.gameObject.SetActive(false);
                UIRoot.uiroot.gameObject.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                SetButtonUp("Aim");
                UIRoot.uiroot.gameObject.SetActive(false);
                UIRoot.uiroot.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                SetButtonDown("Chat");
            }
            if (Input.GetKeyUp(KeyCode.Y))
            {
                SetButtonUp("Chat");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                SetButtonDown("Chat");
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                SetButtonUp("Chat");
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                SetButtonDown("Chat");
            }
            if (Input.GetKeyUp(KeyCode.U))
            {
                SetButtonUp("Chat");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SetButtonDown("Use");
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                SetButtonUp("Use");
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                SetButtonDown("Store");
            }
            if (Input.GetKeyUp(KeyCode.B))
            {
                SetButtonUp("Store");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SetButtonDown("Bomb");
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                SetButtonUp("Bomb");
            }

            if(LevelManager.GetSceneName() == "MainTutorial")
            {
                if(Input.mouseScrollDelta.magnitude > 0.9f)
                {
                    SetButtonDown("SelectWeapon");
                    SetButtonUp("SelectWeapon");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetButtonDown("Reload");
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            SetButtonUp("Reload");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetButtonDown("Jump");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SetButtonUp("Jump");
        }

        SetAxis("Mouse X", Input.GetAxis("Mouse X"));
        SetAxis("Mouse Y", Input.GetAxis("Mouse Y"));
    }
    #endif

    public static void SetButtonDown(string name)
	{
		if (InputManager.GetButtonDownEvent != null)
		{
			InputManager.GetButtonDownEvent(name);
		}
	}

	public static void SetButtonUp(string name)
	{
		if (InputManager.GetButtonUpEvent != null)
		{
			InputManager.GetButtonUpEvent(name);
		}
	}

	public static void SetAxis(string name, float value)
	{
		if (InputManager.GetAxisEvent != null)
		{
			InputManager.GetAxisEvent(name, value);
		}
	}

	public struct ButtonData
	{
		public string name;

		public KeyCode key;
	}

	public struct AxisData
	{
		public string name;

		public bool raw;
	}

	public delegate void ButtonDelegate(string key);

	public delegate void AxisDelegate(string key, float value);
}
