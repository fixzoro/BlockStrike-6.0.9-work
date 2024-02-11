using System;
using UnityEngine;

public class mInAppItem : MonoBehaviour
{
    public mInAppItem.ItemList Item;

    public string Sku;

    public UILabel PriceLabel;

    private GameObject mGameObject;

    public enum ItemList
    {
        Purchase,
        Consume
    }

    private void Start()
	{
		this.PriceLabel.text = InAppManager.GetPrice(this.Sku);
		this.mGameObject = base.gameObject;
	}

	private void OnEnable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Combine(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	private void OnDisable()
	{
		UICamera.onClick = (UICamera.VoidDelegate)Delegate.Remove(UICamera.onClick, new UICamera.VoidDelegate(this.OnClick));
	}

	private void OnClick(GameObject go)
	{
		if (go != this.mGameObject)
		{
			return;
		}
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (this.Item == mInAppItem.ItemList.Purchase)
		{
			InAppManager.Purchase(this.Sku);
		}
		else
		{
			InAppManager.Consume(this.Sku);
		}
	}
}
