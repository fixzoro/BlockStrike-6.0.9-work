//using System;
//using FreeJSON;
//using UnityEngine;

//public class AndroidGoogleSignIn : MonoBehaviour
//{
//    public static AndroidGoogleSignInAccount Account = new AndroidGoogleSignInAccount();

//    private static AndroidGoogleSignIn instance;

//    private static Action successCallback;

//    private static Action<string> errorCallback;

//    private void Awake()
//	{
//		if (AndroidGoogleSignIn.instance == null)
//		{
//			AndroidGoogleSignIn.instance = this;
//		}
//		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
//	}

//	private static void Init()
//	{
//		if (AndroidGoogleSignIn.instance != null)
//		{
//			return;
//		}
//		GameObject gameObject = new GameObject("GoogleSignIn");
//		gameObject.AddComponent<AndroidGoogleSignIn>();
//	}

//	public static void SignIn(string webClientId, Action success, Action<string> error)
//	{
//		if (Application.platform != RuntimePlatform.Android)
//		{
//			return;
//		}
//		AndroidGoogleSignIn.Init();
//		AndroidGoogleSignIn.successCallback = success;
//		AndroidGoogleSignIn.errorCallback = error;
//		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//		{
//			using (AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
//			{
//				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("gr.loukaspd.googlesignin.GoogleSignInFragment"))
//				{
//					androidJavaClass2.SetStatic<string>("UnityGameObjectName", AndroidGoogleSignIn.instance.gameObject.name);
//					androidJavaClass2.CallStatic("SignIn", new object[]
//					{
//						@static,
//						webClientId
//					});
//				}
//			}
//		}
//	}

//	public void UnityGoogleSignInSuccessCallback(string googleSignInAccountJson)
//	{
//		googleSignInAccountJson = googleSignInAccountJson.Replace(", ", ",");
//		googleSignInAccountJson = googleSignInAccountJson.Replace(": ", ":");
//		AndroidGoogleSignInAccount androidGoogleSignInAccount = new AndroidGoogleSignInAccount();
//		JsonObject jsonObject = JsonObject.Parse(googleSignInAccountJson);
//		androidGoogleSignInAccount.DisplayName = jsonObject.Get<string>("DisplayName");
//		androidGoogleSignInAccount.Email = jsonObject.Get<string>("Email");
//		androidGoogleSignInAccount.FamilyName = jsonObject.Get<string>("FamilyName");
//		androidGoogleSignInAccount.Id = jsonObject.Get<string>("Id");
//		androidGoogleSignInAccount.PhotoUrl = jsonObject.Get<string>("PhotoUrl");
//		androidGoogleSignInAccount.Token = jsonObject.Get<string>("Token");
//		if (androidGoogleSignInAccount == null)
//		{
//			this.UnityGoogleSignInErrorCallback(string.Empty);
//			return;
//		}
//		if (string.IsNullOrEmpty(androidGoogleSignInAccount.Email))
//		{
//			this.UnityGoogleSignInErrorCallback(string.Empty);
//			return;
//		}
//		AndroidGoogleSignIn.Account = androidGoogleSignInAccount;
//		if (AndroidGoogleSignIn.successCallback != null)
//		{
//			AndroidGoogleSignIn.successCallback();
//		}
//		this.ClearReferences();
//	}

//	public void UnityGoogleSignInErrorCallback(string errorMsg)
//	{
//		if (AndroidGoogleSignIn.errorCallback != null)
//		{
//			AndroidGoogleSignIn.errorCallback(errorMsg);
//		}
//		this.ClearReferences();
//	}

//	private void ClearReferences()
//	{
//		AndroidGoogleSignIn.successCallback = null;
//		AndroidGoogleSignIn.errorCallback = null;
//	}
//}
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using System;
using FreeJSON;
using UnityEngine;

public class AndroidGoogleSignIn : MonoBehaviour
{
    public static string webClientId = "169884397252-vi1dvfc5tk09ior6c4po4a8nfcfobepa.apps.googleusercontent.com";

    private static GoogleSignInConfiguration configuration;

    public static AndroidGoogleSignInAccount Account = new AndroidGoogleSignInAccount();

    private static AndroidGoogleSignIn instance;

    private static Action successCallback;

    private static Action<string> errorCallback;

    private void Awake()
    {
        if (AndroidGoogleSignIn.instance == null)
        {
            AndroidGoogleSignIn.instance = this;
        }
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    public static void OnSignIn()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true,
            RequestProfile = true
        };
        if(GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
        }
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    internal static void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    errorCallback("");
                }
                else
                {
                    errorCallback("");
                }
            }
        }
        else if (task.IsCanceled)
        {
            errorCallback("");
        }
        else
        {
            AndroidGoogleSignInAccount androidGoogleSignInAccount = new AndroidGoogleSignInAccount();
            androidGoogleSignInAccount.DisplayName = task.Result.DisplayName;
            androidGoogleSignInAccount.Email = task.Result.Email;
            androidGoogleSignInAccount.FamilyName = task.Result.FamilyName;
            androidGoogleSignInAccount.Id = task.Result.UserId;
            androidGoogleSignInAccount.PhotoUrl = task.Result.ImageUrl.AbsoluteUri;
            androidGoogleSignInAccount.Token = task.Result.IdToken;
            AndroidGoogleSignIn.Account = androidGoogleSignInAccount;
            successCallback();
            OnSignOut();
        }
    }

    public static void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private static void Init()
    {
        if (AndroidGoogleSignIn.instance != null)
        {
            return;
        }
        GameObject gameObject = new GameObject("GoogleSignIn");
        gameObject.AddComponent<AndroidGoogleSignIn>();
    }

    public static void SignIn(string webClientId, Action success, Action<string> error)
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }
        successCallback = success;
        errorCallback = error;
        OnSignIn();
    }

    public void UnityGoogleSignInSuccessCallback(string googleSignInAccountJson)
    {
        googleSignInAccountJson = googleSignInAccountJson.Replace(", ", ",");
        googleSignInAccountJson = googleSignInAccountJson.Replace(": ", ":");
        AndroidGoogleSignInAccount androidGoogleSignInAccount = new AndroidGoogleSignInAccount();
        JsonObject jsonObject = JsonObject.Parse(googleSignInAccountJson);
        androidGoogleSignInAccount.DisplayName = jsonObject.Get<string>("DisplayName");
        androidGoogleSignInAccount.Email = jsonObject.Get<string>("Email");
        androidGoogleSignInAccount.FamilyName = jsonObject.Get<string>("FamilyName");
        androidGoogleSignInAccount.Id = jsonObject.Get<string>("Id");
        androidGoogleSignInAccount.PhotoUrl = jsonObject.Get<string>("PhotoUrl");
        androidGoogleSignInAccount.Token = jsonObject.Get<string>("Token");
        if (androidGoogleSignInAccount == null)
        {
            this.UnityGoogleSignInErrorCallback(string.Empty);
            return;
        }
        if (string.IsNullOrEmpty(androidGoogleSignInAccount.Email))
        {
            this.UnityGoogleSignInErrorCallback(string.Empty);
            return;
        }
        AndroidGoogleSignIn.Account = androidGoogleSignInAccount;
        if (AndroidGoogleSignIn.successCallback != null)
        {
            AndroidGoogleSignIn.successCallback();
        }
        this.ClearReferences();
    }

    public void UnityGoogleSignInErrorCallback(string errorMsg)
    {
        if (AndroidGoogleSignIn.errorCallback != null)
        {
            AndroidGoogleSignIn.errorCallback(errorMsg);
        }
        this.ClearReferences();
    }

    private void ClearReferences()
    {
        AndroidGoogleSignIn.successCallback = null;
        AndroidGoogleSignIn.errorCallback = null;
    }
}

