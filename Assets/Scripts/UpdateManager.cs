using System;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private static UpdateManager instance;

    private int updateTimerCount;

    private nTimer[] updateTimer = new nTimer[0];

    private void Awake()
	{
		UpdateManager.instance = this;
	}

	private static void Init()
	{
		if (UpdateManager.instance != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("UpdateManager");
		gameObject.AddComponent<UpdateManager>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public static void Add(nTimer behaviour)
	{
		UpdateManager.Init();
		int num = UpdateManager.instance.updateTimer.Length;
		nTimer[] array = new nTimer[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = UpdateManager.instance.updateTimer[i];
		}
		array[array.Length - 1] = behaviour;
		UpdateManager.instance.updateTimer = array;
		UpdateManager.instance.updateTimerCount = array.Length;
	}

	public static void Remove(nTimer behaviour)
	{
		UpdateManager.Init();
		int num = UpdateManager.instance.updateTimer.Length;
		nTimer[] array = new nTimer[num - 1];
		num--;
		for (int i = 0; i < num; i++)
		{
			if (!(UpdateManager.instance.updateTimer[i] == behaviour))
			{
				array[i] = UpdateManager.instance.updateTimer[i];
			}
		}
		UpdateManager.instance.updateTimer = array;
		UpdateManager.instance.updateTimerCount = array.Length;
	}

	private void Update()
	{
		if (this.updateTimerCount != 0)
		{
			for (int i = 0; i < this.updateTimerCount; i++)
			{
				if (this.updateTimer[i] == null)
				{
				}
			}
		}
	}
}
