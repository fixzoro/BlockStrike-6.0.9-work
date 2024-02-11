using System;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance
	{
		get
		{
			if (FirebaseManager.instance == null)
			{
				GameObject gameObject = new GameObject("Firebase");
				FirebaseManager.instance = gameObject.AddComponent<FirebaseManager>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
            return FirebaseManager.instance;
		}
	}
    
	public static bool DebugAction;
    
	private static FirebaseManager instance;
}
