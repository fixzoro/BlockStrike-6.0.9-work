using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Firebase
{
    public string Key;

    public string Auth;

    public string FullKey;

    public Firebase Parent;

    public string DataBase;

    private static bool isServerCertificate;

    public Firebase()
	{
		this.Key = string.Empty;
		this.FullKey = string.Empty;
		this.Parent = null;
		this.DataBase = "slivchik-8951a-default-rtdb.firebaseio.com";
	}
    
	public Firebase(string databaseURL)
	{
		this.DataBase = databaseURL;
	}
    
	public Firebase(string databaseURL, string auth)
	{
		this.DataBase = databaseURL;
		this.Auth = auth;
	}
    
	private Firebase(Firebase parent, string key, string auth)
	{
		this.Parent = parent;
		this.Key = key;
		this.Auth = auth;
		this.FullKey = parent.FullKey + "/" + key;
		this.DataBase = parent.DataBase;
	}
    
	public string FullURL
	{
		get
		{
			return "https://" + this.DataBase + this.FullKey + ".json";
		}
	}
    
	public Firebase Child(string key)
	{
		return new Firebase(this, key, this.Auth);
	}
    
	public Firebase Copy()
	{
		return new Firebase
		{
			Key = this.Key,
			Auth = this.Auth,
			FullKey = this.FullKey,
			Parent = this.Parent,
			DataBase = this.DataBase
		};
	}
    
	public void SetTimeStamp(string key)
	{
		this.Child(key).SetValue(Firebase.GetTimeStamp());
	}
    
	public static string GetTimeStamp()
	{
		return "{\".sv\": \"timestamp\"}";
	}
    
	public void GetValue()
	{
		this.GetValue(string.Empty, null, null);
	}
    
	public void GetValue(Action<string> success, Action<string> failed)
	{
		this.GetValue(string.Empty, success, failed);
	}
    
	public void GetValue(FirebaseParam param)
	{
		this.GetValue(param.ToString(), null, null);
	}
    
	public void GetValue(FirebaseParam param, Action<string> success, Action<string> failed)
	{
		this.GetValue(param.ToString(), success, failed);
	}
    
	public void GetValue(string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(this.Auth))
		{
			FirebaseParam firebaseParam = new FirebaseParam(param);
			param = firebaseParam.Auth(this.Auth).ToString();
        }
		string text = this.FullURL;
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		MovementEffects.Timing.RunCoroutine(this.GetValueCoroutine(text, success, failed));
	}
    
	private IEnumerator<float> GetValueCoroutine(string url, Action<string> success, Action<string> failed)
	{
		WWW www = new WWW(url);
		yield return MovementEffects.Timing.WaitUntilDone(www);
		if (string.IsNullOrEmpty(www.error))
		{
			if (success != null)
			{
				success(www.text);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnGetSuccess");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.text);
			}
		}
		else
		{
			if (failed != null)
			{
				failed(www.error);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnGetFailed");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.error);
			}
		}
		yield break;
	}
    
	public void SetValue(string json)
	{
		this.SetValue(json, string.Empty, null, null);
	}
    
	public void SetValue(string json, Action<string> success, Action<string, string> failed)
	{
		this.SetValue(json, string.Empty, success, failed);
	}
    
	public void SetValue(string json, FirebaseParam param)
	{
		this.SetValue(json, param.ToString(), null, null);
	}
    
	public void SetValue(string json, FirebaseParam param, Action<string> success, Action<string, string> failed)
	{
		this.SetValue(json, param.ToString(), success, failed);
	}
    
	public void SetValue(string json, string param, Action<string> success, Action<string, string> failed)
	{
		if (!string.IsNullOrEmpty(this.Auth))
		{
			FirebaseParam firebaseParam = new FirebaseParam(param);
			param = firebaseParam.Auth(this.Auth).ToString();
		}
		string text = this.FullURL;
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
        MovementEffects.Timing.RunCoroutine(this.SetValueCoroutine(text, json, success, failed));
	}
    
	private IEnumerator<float> SetValueCoroutine(string url, string json, Action<string> success, Action<string, string> failed)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Content-Type", "application/json");
		dictionary.Add("X-HTTP-Method-Override", "PUT");
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		WWW www = new WWW(url, bytes, dictionary);
		yield return MovementEffects.Timing.WaitUntilDone(www);
        if (string.IsNullOrEmpty(www.error))
		{
			if (success != null)
			{
				success(www.text);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnSetSuccess");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.text);
			}
		}
		else
		{
			if (failed != null)
			{
				failed(www.error, json);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnSetFailed");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.error);
			}
		}
		yield break;
	}
    
	public void UpdateValue(string json)
	{
		this.UpdateValue(json, string.Empty, null, null);
	}
    
	public void UpdateValue(string json, Action<string> success, Action<string, string> failed)
	{
		this.UpdateValue(json, string.Empty, success, failed);
	}
    
	public void UpdateValue(string json, FirebaseParam param)
	{
		this.UpdateValue(json, param.ToString(), null, null);
	}
    
	public void UpdateValue(string json, FirebaseParam param, Action<string> success, Action<string, string> failed)
	{
		this.UpdateValue(json, param.ToString(), success, failed);
	}
    
	public void UpdateValue(string json, string param, Action<string> success, Action<string, string> failed)
	{
		if (!string.IsNullOrEmpty(this.Auth))
		{
			FirebaseParam firebaseParam = new FirebaseParam(param);
			param = firebaseParam.Auth(this.Auth).ToString();
		}
		string text = this.FullURL;
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
        MovementEffects.Timing.RunCoroutine(this.UpdateValueCoroutine(text, json, success, failed));
	}
    
	private IEnumerator<float> UpdateValueCoroutine(string url, string json, Action<string> success, Action<string, string> failed)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Content-Type", "application/json");
		dictionary.Add("X-HTTP-Method-Override", "PATCH");
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		WWW www = new WWW(url, bytes, dictionary);
		yield return MovementEffects.Timing.WaitUntilDone(www);
        if (string.IsNullOrEmpty(www.error))
		{
			if (success != null)
			{
				success(www.text);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnUpdateSuccess");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.text);
			}
		}
		else
		{
			if (failed != null)
			{
				failed(www.error, json);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnUpdateFailed");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.error);
			}
		}
		yield break;
	}
    
	public void Push(string json)
	{
		this.Push(json, string.Empty, null, null);
	}
    
	public void Push(string json, Action<string> success, Action<string> failed)
	{
		this.Push(json, string.Empty, success, failed);
	}
    
	public void Push(string json, FirebaseParam param)
	{
		this.Push(json, param.ToString(), null, null);
	}
    
	public void Push(string json, FirebaseParam param, Action<string> success, Action<string> failed)
	{
		this.Push(json, param.ToString(), success, failed);
	}
    
	public void Push(string json, string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(this.Auth))
		{
			FirebaseParam firebaseParam = new FirebaseParam(param);
			param = firebaseParam.Auth(this.Auth).ToString();
		}
		string text = this.FullURL;
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		FirebaseManager.Instance.StartCoroutine(this.PushCoroutine(text, json, success, failed));
	}
    
	private IEnumerator PushCoroutine(string url, string json, Action<string> success, Action<string> failed)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(json);
		WWW www = new WWW(url, bytes);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (success != null)
			{
				success(www.text);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnPushSuccess");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.text);
			}
		}
		else
		{
			if (failed != null)
			{
				failed(www.error);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnPushFailed");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.error);
			}
		}
		yield break;
	}
    
	public void Delete()
	{
		this.Delete(string.Empty, null, null);
	}
    
	public void Delete(Action<string> success, Action<string> failed)
	{
		this.Delete(string.Empty, success, failed);
	}
    
	public void Delete(FirebaseParam param)
	{
		this.Delete(param.ToString(), null, null);
	}
    
	public void Delete(FirebaseParam param, Action<string> success, Action<string> failed)
	{
		this.Delete(param.ToString(), success, failed);
	}
    
	public void Delete(string param, Action<string> success, Action<string> failed)
	{
		if (!string.IsNullOrEmpty(this.Auth))
		{
			FirebaseParam firebaseParam = new FirebaseParam(param);
			param = firebaseParam.Auth(this.Auth).ToString();
		}
		string text = this.FullURL;
		if (!string.IsNullOrEmpty(param))
		{
			text = text + "?" + param;
		}
		FirebaseManager.Instance.StartCoroutine(this.DeleteCoroutine(text, success, failed));
	}
    
	private IEnumerator DeleteCoroutine(string url, Action<string> success, Action<string> failed)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Content-Type", "application/json");
		dictionary.Add("X-HTTP-Method-Override", "DELETE");
		byte[] bytes = Encoding.UTF8.GetBytes("{ \"dummy\" : \"dummies\"}");
		WWW www = new WWW(url, bytes, dictionary);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (success != null)
			{
				success(www.text);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnDeleteSuccess");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.text);
			}
		}
		else
		{
			if (failed != null)
			{
				failed(www.error);
			}
			if (FirebaseManager.DebugAction)
			{
				Debug.Log("OnDeleteFailed");
				Debug.Log("Firebase: " + this.FullURL);
				Debug.Log("Json: " + www.error);
			}
		}
		yield break;
	}
}
