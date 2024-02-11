public static class StringCache
{
    private static string[] cacheNumber;

    private static string[] cacheTime;

    private static FastString fastString = new FastString(200);

    public static string Create(params string[] str)
	{
		StringCache.fastString.Clear();
		for (int i = 0; i < str.Length; i++)
		{
			StringCache.fastString.Append(str[i]);
		}
		return StringCache.fastString.ToString();
	}
    
	public static string Get(uint number)
	{
		return StringCache.Get((int)number);
	}
    
	public static string Get(int number)
	{
		if (number < 0)
		{
			return number.ToString();
		}
		if (StringCache.cacheNumber == null)
		{
			StringCache.cacheNumber = new string[1010];
			for (int i = 0; i < 1010; i++)
			{
				StringCache.cacheNumber[i] = i.ToString();
			}
		}
		if (StringCache.cacheNumber.Length - 1 >= number)
		{
			return StringCache.cacheNumber[number];
		}
		return number.ToString();
	}
    
	public static string GetTime(float time)
	{
		return StringCache.GetTime((int)time);
	}
    
	public static string GetTime(int time)
	{
		if (time < 0)
		{
			time *= -1;
		}
		if (StringCache.cacheTime == null)
		{
			StringCache.cacheTime = new string[1000];
			StringCache.cacheTime[0] = "0:00";
			for (int i = 1; i < 1000; i++)
			{
				int num = i / 60;
				int num2 = i - num * 60;
				StringCache.cacheTime[i] = string.Format("{0:0}:{1:00}", num, num2);
			}
		}
		if (StringCache.cacheTime != null && StringCache.cacheTime.Length - 1 >= time)
		{
			return StringCache.cacheTime[time];
		}
		int num3 = time / 60;
		int num4 = time - num3 * 60;
		return string.Format("{0:0}:{1:00}", num3, num4);
	}
}
