using System;
using UnityEngine;

public class BadWordsManager
{
	public static void Init()
	{
		if (BadWordsManager.words.Length == 0)
		{
			BadWordsManager.words = ((TextAsset)Resources.Load("Others/BadWords", typeof(TextAsset))).text.Split(new char[]
			{
				"\n"[0]
			});
		}
	}

	public static string Check(string text)
	{
		if (BadWordsManager.words.Length == 0)
		{
			BadWordsManager.Init();
		}
		bool flag = false;
		string text2 = text.ToLower();
		for (int i = 0; i < BadWordsManager.words.Length; i++)
		{
			if (text2.Contains(BadWordsManager.words[i]))
			{
				flag = true;
				text2 = text2.Replace(BadWordsManager.words[i], BadWordsManager.BlockWord(BadWordsManager.words[i].Length));
			}
		}
		return (!flag) ? text : text2;
	}

	public static bool Contains(string text)
	{
		if (BadWordsManager.words.Length == 0)
		{
			BadWordsManager.Init();
		}
		text = text.ToLower();
		for (int i = 0; i < BadWordsManager.words.Length; i++)
		{
			if (text.Contains(BadWordsManager.words[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static string BlockWord(int length)
	{
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text += "*";
		}
		return text;
	}

	private static string[] words = new string[0];
}
