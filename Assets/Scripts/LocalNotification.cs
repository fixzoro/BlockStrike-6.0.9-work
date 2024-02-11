using System;
using UnityEngine;

internal class LocalNotification
{
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";

    private static string mainActivityClassName = "com.prime31.UnityPlayerNativeActivity";

    public enum NotificationExecuteMode
    {
        Inexact,
        Exact,
        ExactAndAllowWhileIdle
    }

    public static void SendNotification(int id, TimeSpan delay, string title, string message)
	{
		LocalNotification.SendNotification(id, (long)((int)delay.TotalSeconds), title, message, Color.white, true, true, true, string.Empty, LocalNotification.NotificationExecuteMode.Inexact);
	}
    
	public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", LocalNotification.NotificationExecuteMode executeMode = LocalNotification.NotificationExecuteMode.Inexact)
	{
		//AndroidJavaClass androidJavaClass = new AndroidJavaClass(LocalNotification.fullClassName);
		//if (androidJavaClass != null)
		//{
		//	androidJavaClass.CallStatic("SetNotification", new object[]
		//	{
		//		id,
		//		delay * 1000L,
		//		title,
		//		message,
		//		message,
		//		(!sound) ? 0 : 1,
		//		(!vibrate) ? 0 : 1,
		//		(!lights) ? 0 : 1,
		//		bigIcon,
		//		"notify_icon_small",
		//		(int)bgColor.r * 65536 + (int)bgColor.g * 256 + (int)bgColor.b,
		//		(int)executeMode,
		//		LocalNotification.mainActivityClassName
		//	});
		//}
	}
    
	public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(LocalNotification.fullClassName);
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetRepeatingNotification", new object[]
			{
				id,
				delay * 1000L,
				title,
				message,
				message,
				timeout * 1000L,
				(!sound) ? 0 : 1,
				(!vibrate) ? 0 : 1,
				(!lights) ? 0 : 1,
				bigIcon,
				"notify_icon_small",
				(int)bgColor.r * 65536 + (int)bgColor.g * 256 + (int)bgColor.b,
				LocalNotification.mainActivityClassName
			});
		}
	}
    
	public static void CancelNotification(int id)
	{
		//AndroidJavaClass androidJavaClass = new AndroidJavaClass(LocalNotification.fullClassName);
		//if (androidJavaClass != null)
		//{
		//	androidJavaClass.CallStatic("CancelNotification", new object[]
		//	{
		//		id
		//	});
		//}
	}
}
