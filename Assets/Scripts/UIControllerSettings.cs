using System;
using UnityEngine;

public class UIControllerSettings : MonoBehaviour
{
    public Transform Anchor;

    public UISprite JoystickRect;

    public UISprite LookRect;

    public UISprite JoystickSprite;

    public static event Action onDefaultEvent;

	public static event Action onSaveEvent;

	private void OnEnable()
	{
		TimerManager.In(0.1f, delegate()
		{
			if (Settings.DynamicJoystick)
			{
				this.JoystickRect.cachedGameObject.SetActive(true);
				this.RectToSprite(this.JoystickRect, nPlayerPrefs.GetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, (float)(Screen.height / 2))));
				this.JoystickSprite.cachedGameObject.SetActive(false);
			}
			else
			{
				this.JoystickRect.cachedGameObject.SetActive(false);
				this.JoystickSprite.cachedGameObject.SetActive(true);
			}
			this.RectToSprite(this.LookRect, nPlayerPrefs.GetRect("Look_Rect", new Rect((float)(Screen.width / 2), 0f, (float)(Screen.width / 2), (float)Screen.height)));
		});
	}

	public void OnBack()
	{
		if (UIControllerSettings.onSaveEvent != null)
		{
			UIControllerSettings.onSaveEvent();
		}
		if (Settings.DynamicJoystick)
		{
			nPlayerPrefs.SetRect("Joystick_Rect", this.SpriteToRect(this.JoystickRect));
		}
		nPlayerPrefs.SetRect("Look_Rect", this.SpriteToRect(this.LookRect));
	}

	public void OnDefault()
	{
		if (UIControllerSettings.onDefaultEvent != null)
		{
			UIControllerSettings.onDefaultEvent();
		}
		nPlayerPrefs.SetRect("Joystick_Rect", new Rect(0f, 0f, (float)Screen.width / 2.5f, (float)(Screen.height / 2)));
		nPlayerPrefs.SetRect("Look_Rect", new Rect((float)(Screen.width / 2), 0f, (float)(Screen.width / 2), (float)Screen.height));
		this.OnEnable();
	}

	private void RectToSprite(UISprite sp, Rect rect)
	{
		if (rect.x != 0f)
		{
			rect.x = rect.x / (float)Screen.width * this.Anchor.localPosition.x;
		}
		if (rect.y != 0f)
		{
			rect.y = rect.y / (float)Screen.height * this.Anchor.localPosition.y;
		}
		if (rect.width != 0f)
		{
			rect.width = rect.width / (float)Screen.width * this.Anchor.localPosition.x;
		}
		if (rect.height != 0f)
		{
			rect.height = rect.height / (float)Screen.height * this.Anchor.localPosition.y;
		}
		Vector3 localPosition = new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, 0f);
		sp.cachedTransform.localPosition = localPosition;
		sp.width = (int)rect.width;
		sp.height = (int)rect.height;
	}

	private Rect SpriteToRect(UISprite sp)
	{
		Rect result = default(Rect);
		result.x = sp.cachedTransform.localPosition.x - (float)sp.width / 2f;
		if (result.x != 0f)
		{
			result.x = result.x / this.Anchor.localPosition.x * (float)Screen.width;
		}
		result.y = sp.cachedTransform.localPosition.y - (float)sp.height / 2f;
		if (result.y != 0f)
		{
			result.y = result.y / this.Anchor.localPosition.y * (float)Screen.height;
		}
		result.width = (float)sp.width;
		if (result.width != 0f)
		{
			result.width = result.width / this.Anchor.localPosition.x * (float)Screen.width;
		}
		result.height = (float)sp.height;
		if (result.height != 0f)
		{
			result.height = result.height / this.Anchor.localPosition.y * (float)Screen.height;
		}
		return result;
	}
}
