using System;

public struct FirebaseParam
{
	public FirebaseParam(string param)
	{
		this.Param = param;
	}
    
	public static FirebaseParam Default
	{
		get
		{
			return default(FirebaseParam);
		}
	}
    
	public FirebaseParam Add(string param)
	{
		if (!string.IsNullOrEmpty(this.Param))
		{
			this.Param += "&";
		}
		this.Param += param;
		return this;
	}
    
	public FirebaseParam Add(string header, string value)
	{
		return this.Add(header, value, true);
	}
    
	public FirebaseParam Add(string header, string value, bool quoted)
	{
		if (quoted)
		{
			return this.Add(header + "=\"" + value + "\"");
		}
		return this.Add(header + "=" + value);
	}
    
	public FirebaseParam Add(string header, int value)
	{
		return this.Add(header + "=" + value);
	}

	public FirebaseParam Add(string header, float value)
	{
		return this.Add(header + "=" + value);
	}
    
	public FirebaseParam Add(string header, long value)
	{
		return this.Add(header + "=" + value);
	}
    
	public FirebaseParam Add(string header, bool value)
	{
		return this.Add(header + "=" + value);
	}
    
	public FirebaseParam Auth(string auth)
	{
		return this.Add("auth", auth, false);
	}
    
	public FirebaseParam OrderBy(string key)
	{
		return this.Add("orderBy", key);
	}
    
	public FirebaseParam OrderByKey()
	{
		return this.Add("orderBy", "$key");
	}
    
	public FirebaseParam OrderByValue()
	{
		return this.Add("orderBy", "$value");
	}
    
	public FirebaseParam OrderByPriority()
	{
		return this.Add("orderBy", "$priority");
	}
    
	public FirebaseParam LimitToFirst(int limit)
	{
		return this.Add("limitToFirst", limit);
	}
    
	public FirebaseParam LimitToLast(int limit)
	{
		return this.Add("limitToLast", limit);
	}
    
	public FirebaseParam StartAt(string start)
	{
		return this.Add("startAt", start);
	}
    
	public FirebaseParam StartAt(int start)
	{
		return this.Add("startAt", start);
	}
    
	public FirebaseParam StartAt(float start)
	{
		return this.Add("startAt", start);
	}
    
	public FirebaseParam StartAt(long start)
	{
		return this.Add("startAt", start);
	}
    
	public FirebaseParam StartAt(bool start)
	{
		return this.Add("startAt", start);
	}
    
	public FirebaseParam EndAt(string end)
	{
		return this.Add("endAt", end);
	}
    
	public FirebaseParam EndAt(int end)
	{
		return this.Add("endAt", end);
	}
    
	public FirebaseParam EndAt(float end)
	{
		return this.Add("endAt", end);
	}
    
	public FirebaseParam EndAt(long end)
	{
		return this.Add("endAt", end);
	}
    
	public FirebaseParam EndAt(bool end)
	{
		return this.Add("endAt", end);
	}
    
	public FirebaseParam EqualTo(string at)
	{
		return this.Add("equalTo", at);
	}
    
	public FirebaseParam EqualTo(int at)
	{
		return this.Add("equalTo", at);
	}
    
	public FirebaseParam EqualTo(float at)
	{
		return this.Add("equalTo", at);
	}
    
	public FirebaseParam EqualTo(long at)
	{
		return this.Add("equalTo", at);
	}
    
	public FirebaseParam EqualTo(bool at)
	{
		return this.Add("equalTo", at);
	}
    
	public FirebaseParam PrintPretty()
	{
		return this.Add("print=pretty");
	}
    
	public FirebaseParam PrintSilent()
	{
		return this.Add("print=silent");
	}
    
	public FirebaseParam Shallow()
	{
		return this.Add("shallow=true");
	}
    
	public override string ToString()
	{
		return this.Param;
	}
    
	private string Param;
}
