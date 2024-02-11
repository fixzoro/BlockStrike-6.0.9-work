using System;
using UnityEngine;

public class UIElements : MonoBehaviour
{
    public UIElements.Element[] list;

    private static UIElements instance;

    private void Awake()
	{
		UIElements.instance = this;
	}

	public static T Get<T>(string key) where T : Component
	{
		int i = 0;
		while (i < UIElements.instance.list.Length)
		{
			if (UIElements.instance.list[i].key == key)
			{
				if (!(UIElements.instance.list[i].comp == null))
				{
					return UIElements.instance.list[i].comp as T;
				}
				T component = UIElements.instance.list[i].target.GetComponent<T>();
				if (component != null)
				{
					UIElements.instance.list[i].comp = component;
					return component;
				}
				return (T)((object)null);
			}
			else
			{
				i++;
			}
		}
		return (T)((object)null);
	}

	[Serializable]
	public class Element
	{
		public string key;

		public GameObject target;

		[HideInInspector]
		public UnityEngine.Object comp;
	}
}
