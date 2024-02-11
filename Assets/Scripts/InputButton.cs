using System;
using UnityEngine;

public class InputButton : MonoBehaviour
{
    public string button;

    private bool press;

    private float alpha = 1f;

    private UISprite sprite;

    private void Start()
	{
		this.sprite = base.GetComponent<UISprite>();
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		EventManager.AddListener("OnSaveButton", new EventManager.Callback(this.OnPosition));
		this.OnSettings();
		this.OnPosition();
	}

	private void OnEnable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Combine(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		if (this.sprite != null)
		{
			this.sprite.alpha = this.alpha;
		}
	}

	private void OnDisable()
	{
		UICamera.onPress = (UICamera.BoolDelegate)Delegate.Remove(UICamera.onPress, new UICamera.BoolDelegate(this.OnPress));
		if (this.press)
		{
			InputManager.SetButtonUp(this.button);
		}
	}

	private void OnPress(GameObject go, bool pressed)
	{
		if (this.sprite.cachedGameObject != go)
		{
			return;
		}
		this.press = pressed;
		if (this.press)
		{
			this.sprite.alpha = this.alpha * 0.5f;
			InputManager.SetButtonDown(this.button);
		}
		else
		{
			this.sprite.alpha = this.alpha;
			InputManager.SetButtonUp(this.button);
		}
	}

	private void OnPosition()
	{
		TimerManager.In(0.1f, delegate()
		{
			if (nPlayerPrefs.HasKey("Button_Pos_" + this.button))
			{
				this.sprite.cachedTransform.localPosition = nPlayerPrefs.GetVector3("Button_Pos_" + this.button);
				if (nPlayerPrefs.HasKey("Button_Size_" + this.button))
				{
					Vector2 vector = nPlayerPrefs.GetVector2("Button_Size_" + this.button);
					if (vector != Vector2.zero)
					{
						this.sprite.width = (int)vector.x;
						this.sprite.height = (int)vector.y;
					}
				}
			}
			this.sprite.UpdateWidget();
		});
	}

	private void OnSettings()
	{
		this.alpha = Settings.ButtonAlpha;
		if (!Settings.HUD)
		{
			this.alpha = 1f;
		}
		this.sprite.alpha = this.alpha;
	}
}
