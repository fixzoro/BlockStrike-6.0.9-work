using System;
using UnityEngine;

public class UIDefuseKit : MonoBehaviour
{
    private static bool DefuseKit;

    public UIWidget DefuseKitButton;

    private static UISprite DefuseKitIcon;

    public static bool defuseKit
	{
		get
		{
			return UIDefuseKit.DefuseKit;
		}
		set
		{
			UIDefuseKit.DefuseKit = value;
			if (UIDefuseKit.DefuseKitIcon == null)
			{
				UIDefuseKit.DefuseKitIcon = UIElements.Get<UISprite>("BombIcon");
			}
			UIDefuseKit.DefuseKitIcon.cachedGameObject.SetActive(value);
			if (value)
			{
				UIDefuseKit.DefuseKitIcon.spriteName = "Pilers";
			}
		}
	}

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		this.DefuseKitButton.cachedGameObject.SetActive(GameManager.team == Team.Blue);
		this.DefuseKitButton.alpha = ((!UIDefuseKit.DefuseKit) ? 1f : 0.5f);
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UIDefuseKit.DefuseKit = false;
	}

	private void OnDestroy()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
		UIDefuseKit.DefuseKit = false;
	}

	private void OnClick(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go != base.gameObject)
		{
			return;
		}
		if (UIDefuseKit.defuseKit)
		{
			return;
		}
		if (UIBuyWeapon.Money < 400)
		{
			UIToast.Show(Localization.Get("Not enough money", true));
			return;
		}
		UIBuyWeapon.Money -= 400;
		UIDefuseKit.defuseKit = true;
		this.DefuseKitButton.alpha = 0.5f;
	}
}
