using System;
using UnityEngine;

public class MaxScoreManager : MonoBehaviour
{
    public MaxScoreManager.MaxScoreData[] modes;

    private static MaxScoreManager instance;

    private void Awake()
	{
		MaxScoreManager.instance = this;
	}

	public static int Get(GameMode mode)
	{
		for (int i = 0; i < MaxScoreManager.instance.modes.Length; i++)
		{
			if (MaxScoreManager.instance.modes[i].mode == mode)
			{
				return MaxScoreManager.instance.modes[i].score;
			}
		}
		Debug.Log("No Find Score: " + mode.ToString());
		return 0;
	}

	[Serializable]
	public struct MaxScoreData
	{
		public MaxScoreData(GameMode m, int s)
		{
			this.mode = m;
			this.score = s;
		}

		public GameMode mode;

		public CryptoInt score;
	}
}
