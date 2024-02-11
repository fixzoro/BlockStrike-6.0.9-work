using UnityEngine;
using System.IO;
public class VersionManager : MonoBehaviour {

	static bool isCheck;
	static string mProductName;
	static string mBundleIdentifier;
	static string mBundleVersion;
	static int mBundleVersionCode;
	static bool mFullVersion;
	static bool mTestVersion;

	
	public static string productName{
		get{
			CheckTextAsset();
			return mProductName;
		}
	}
	
	public static string bundleIdentifier{
		get{
			CheckTextAsset();
			return mBundleIdentifier;
		}
	}
	
	public static string bundleVersion{
		get{
			CheckTextAsset();
			return mBundleVersion;
		}
	}
	
	public static int bundleVersionCode{
		get{
			CheckTextAsset();
			return mBundleVersionCode;
		}
	}
	
	public static bool fullVersion{
		get{
			CheckTextAsset();
			return mFullVersion;
		}
	}
	
	public static bool testVersion{
		get{
			CheckTextAsset();
			return mTestVersion;
		}
	}
	
	static void CheckTextAsset(){
		if(!isCheck){
			TextAsset w = Resources.Load("VersionManager/VersionInfo")as TextAsset;
			string[] n = w.text.Split("\n"[0]);
			mProductName = n[0];
			mBundleIdentifier = n[1];
			mBundleVersion = n[2];
			mBundleVersionCode = int.Parse(n[3]);
			mFullVersion = bool.Parse(n[4]);
			mTestVersion = bool.Parse(n[5]);
			isCheck = true;
		}
	}
}
