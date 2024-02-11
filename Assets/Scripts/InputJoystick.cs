using System;
using UnityEngine;

public class InputJoystick : MonoBehaviour
{
    public Camera uiCamera;

    public UISprite stick;

    public UISprite background;

    public float distance = 0.3f;

    public string xAxisName = "Horizontal";

    public string yAxisName = "Vertical";

    public InputJoystick.JoystickType selectType;

    private int id = -1;

    private Rect touchZone;

    private Vector3 touchPos;

    public static bool shift;

    private float lastTouch;

    private float positionMultiplier;

    private float alpha = 1f;

    public enum JoystickType
    {
        Dynamic,
        Static
    }

    private void Start()
	{
		this.positionMultiplier = 1f / this.distance;
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
        touchZone = new Rect(0f, 0f, (float)Screen.width / 2.5f, (float)(Screen.height / 2));
    }

	private void OnDisable()
	{
		if (this.selectType == InputJoystick.JoystickType.Dynamic)
		{
			this.Hide();
		}
		this.id = -1;
		this.UpdateValue(Vector2.zero);
	}

	private void Update()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began && this.touchZone.Contains(touch.position))
			{
				if (this.lastTouch + 0.25f > Time.time)
				{
					InputJoystick.shift = true;
					this.lastTouch = 0f;
				}
				else
				{
					this.lastTouch = Time.time;
				}
				this.id = touch.fingerId;
				this.touchPos = touch.position;
				this.touchPos.x = Mathf.Clamp01(this.touchPos.x / (float)Screen.width);
				this.touchPos.y = Mathf.Clamp01(this.touchPos.y / (float)Screen.height);
				if (this.selectType == InputJoystick.JoystickType.Dynamic)
				{
					this.background.cachedTransform.position = this.uiCamera.ViewportToWorldPoint(this.touchPos);
				}
				this.stick.cachedTransform.position = this.uiCamera.ViewportToWorldPoint(this.touchPos);
				this.stick.UpdateWidget();
				this.background.UpdateWidget();
				this.Show();
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && this.id == touch.fingerId)
			{
				this.touchPos = touch.position;
				this.touchPos.x = Mathf.Clamp01(this.touchPos.x / (float)Screen.width);
				this.touchPos.y = Mathf.Clamp01(this.touchPos.y / (float)Screen.height);
				this.stick.cachedTransform.position = this.uiCamera.ViewportToWorldPoint(this.touchPos);
				this.stick.cachedTransform.position = this.Clamp(this.background.cachedTransform.position, this.stick.cachedTransform.position);
				this.UpdateValue((this.stick.cachedTransform.position - this.background.cachedTransform.position) * this.positionMultiplier);
				this.stick.UpdateWidget();
			}
			if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && this.id == touch.fingerId)
			{
				InputJoystick.shift = false;
				this.id = -1;
				if (this.selectType == InputJoystick.JoystickType.Dynamic)
				{
					this.Hide();
				}
				else
				{
					this.stick.cachedTransform.position = this.background.cachedTransform.position;
				}
				this.UpdateValue(Vector2.zero);
			}
		}
	}

	private Vector3 Clamp(Vector3 p, Vector3 y)
	{
		if (Vector3.Distance(p, y) > this.distance)
		{
			Vector3 a = y - p;
			a.Normalize();
			return a * this.distance + p;
		}
		return y;
	}

	private void Show()
	{
		this.stick.alpha = this.alpha;
		this.background.alpha = this.alpha;
		this.stick.UpdateWidget();
		this.background.UpdateWidget();
	}

	private void Hide()
	{
		try
		{
			this.stick.alpha = 0f;
			this.background.alpha = 0f;
			this.stick.UpdateWidget();
			this.background.UpdateWidget();
		}
		catch
		{
		}
	}

	private void UpdateValue(Vector2 value)
	{
		InputManager.SetAxis(this.xAxisName, value.x);
		InputManager.SetAxis(this.yAxisName, value.y);
	}

	private void OnSettings()
	{
		this.selectType = ((!Settings.DynamicJoystick) ? InputJoystick.JoystickType.Static : InputJoystick.JoystickType.Dynamic);
		if (this.selectType == InputJoystick.JoystickType.Dynamic)
		{
			//this.touchZone = nPlayerPrefs.GetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, (float)(Screen.height / 2)));
			this.Hide();
		}
		else
		{
			if (nPlayerPrefs.HasKey("Joystick_Pos"))
			{
				this.background.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Joystick_Pos");
			}
			Vector3 vector = this.uiCamera.WorldToViewportPoint(this.background.cachedTransform.position);
			this.touchZone = new Rect(vector.x * (float)Screen.width - (float)(this.background.width / 2), vector.y * (float)Screen.height - (float)(this.background.height / 2), (float)this.background.width, (float)this.background.height);
			this.Show();
			this.stick.cachedTransform.position = this.background.cachedTransform.position;
		}
		this.alpha = Settings.ButtonAlpha;
		this.stick.UpdateWidget();
		this.background.UpdateWidget();
	}
}
