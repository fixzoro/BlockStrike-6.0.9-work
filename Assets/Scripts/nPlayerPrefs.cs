using System;
using System.IO;
using FreeJSON;
using UnityEngine;

public static class nPlayerPrefs
{
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static bool GetBool(string key)
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt(key + "bool2021h"));
    }

    public static bool GetBool(string key, bool defaultValue)
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt(key + "bool2021h", Convert.ToInt32(defaultValue)));
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key + "bool2021h", Convert.ToInt32(value));
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static string GetString(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }


    public static Vector3 GetVector3(string key)
    {
        string cvecString = PlayerPrefs.GetString(key);
        string[] vecString = cvecString.Split('?');
        return new Vector3((float)Convert.ToDouble(vecString[0]), (float)Convert.ToDouble(vecString[1]), (float)Convert.ToDouble(vecString[2]));
    }

    public static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        string cvecString = PlayerPrefs.GetString(key);
        if (cvecString.Length < 3)
        {
            return defaultValue;
        }
        string[] vecString = cvecString.Split('?');
        return new Vector3((float)Convert.ToDouble(vecString[0]), (float)Convert.ToDouble(vecString[1]), (float)Convert.ToDouble(vecString[2]));
    }

    public static void SetVector3(string key, Vector3 value)
    {
        string cvecString = value.x.ToString() + "?" + value.y.ToString() + "?" + value.z.ToString();
        PlayerPrefs.SetString(key, cvecString);
    }

    public static Vector2 GetVector2(string key)
    {
        string cvecString = PlayerPrefs.GetString(key);
        string[] vecString = cvecString.Split('?');
        return new Vector2((float)Convert.ToDouble(vecString[0]), (float)Convert.ToDouble(vecString[1]));
    }

    public static Vector2 GetVector2(string key, Vector2 defaultValue)
    {
        string cvecString = PlayerPrefs.GetString(key);
        if (cvecString.Length < 3)
        {
            return defaultValue;
        }
        string[] vecString = cvecString.Split('?');
        return new Vector2((float)Convert.ToDouble(vecString[0]), (float)Convert.ToDouble(vecString[1]));
    }

    public static void SetVector2(string key, Vector2 value)
    {
        string cvecString = value.x.ToString() + "?" + value.y.ToString();
        PlayerPrefs.SetString(key, cvecString);
    }

    public static Rect GetRect(string key)
    {
        throw new NotImplementedException();
        return /*PlayerPrefs.Get<Rect>(key)*/ new Rect();
    }

    public static Rect GetRect(string key, Rect defaultValue)
    {
        throw new NotImplementedException();
        return /*PlayerPrefs.Get<Rect>(key, defaultValue)*/ new Rect();
    }

    public static void SetRect(string key, Rect value)
    {
        throw new NotImplementedException();
        //PlayerPrefs.Add(key, value);
    }
}
