using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private void Start()
	{
		InAppManager.instance = this;
		InAppManager.Init();
	}
    
	private void OnEnable()
	{
		
	}
    
	private void OnDisable()
	{
		
	}
    
	public static void Init()
	{
		
	}
    
	private void billingSupportedEvent()
	{
		this.UpdateQueryInventory();
	}
    
	private void UpdateQueryInventory()
	{
		string[] skus = new string[]
		{
			"com.rexetstudio.blockstrike.g100",
			"com.rexetstudio.blockstrike.g250",
			"com.rexetstudio.blockstrike.g600",
			"com.rexetstudio.blockstrike.g1000",
			"com.rexetstudio.blockstrike.m5000",
			"com.rexetstudio.blockstrike.m10000",
			"com.rexetstudio.blockstrike.m20000",
			"com.rexetstudio.blockstrike.m30000"
		};
	}
    
	private void billingNotSupportedEvent(string error)
	{
		MonoBehaviour.print("billingNotSupportedEvent: " + error);
	}
    
	public static string GetPrice(string sku)
	{
		if (Application.isEditor)
		{
			return "0,00$";
		}
		return /*string.Empty*/ "0,00$";
	}
    
	public static void Purchase(string sku)
	{
		
	}

	private void purchaseFailedEvent(string error, int response)
	{
		UIToast.Show(Localization.Get("Error", true) + ": " + error);
	}
    
	public static void Consume(string sku)
	{
		
	}
    
	private void consumePurchaseFailedEvent(string error)
	{
		this.UpdateQueryInventory();
	}

	private static InAppManager instance;
}
