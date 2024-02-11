using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEngine;

// Token: 0x020002A2 RID: 674
public static class Extensions
{
	// Token: 0x060019BA RID: 6586 RVA: 0x000A9218 File Offset: 0x000A7418
	public static ParameterInfo[] GetCachedParemeters(this MethodInfo mo)
	{
		ParameterInfo[] parameters;
		if (!Extensions.ParametersOfMethods.TryGetValue(mo, out parameters))
		{
			parameters = mo.GetParameters();
			Extensions.ParametersOfMethods[mo] = parameters;
		}
		return parameters;
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x00012EA5 File Offset: 0x000110A5
	public static PhotonView[] GetPhotonViewsInChildren(this GameObject go)
	{
		return go.GetComponentsInChildren<PhotonView>(true);
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x00012EAE File Offset: 0x000110AE
	public static PhotonView GetPhotonView(this GameObject go)
	{
		return go.GetComponent<PhotonView>();
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000A9250 File Offset: 0x000A7450
	public static bool AlmostEquals(this Vector3 target, Vector3 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000A9270 File Offset: 0x000A7470
	public static bool AlmostEquals(this Vector2 target, Vector2 second, float sqrMagnitudePrecision)
	{
		return (target - second).sqrMagnitude < sqrMagnitudePrecision;
	}

	// Token: 0x060019BF RID: 6591 RVA: 0x00012EB6 File Offset: 0x000110B6
	public static bool AlmostEquals(this Quaternion target, Quaternion second, float maxAngle)
	{
		return Quaternion.Angle(target, second) < maxAngle;
	}

	// Token: 0x060019C0 RID: 6592 RVA: 0x00012EC2 File Offset: 0x000110C2
	public static bool AlmostEquals(this float target, float second, float floatDiff)
	{
		return Mathf.Abs(target - second) < floatDiff;
	}

	// Token: 0x060019C1 RID: 6593 RVA: 0x000A9290 File Offset: 0x000A7490
	public static void Merge(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		foreach (object key in addHash.Keys)
		{
			target[key] = addHash[key];
		}
	}

	// Token: 0x060019C2 RID: 6594 RVA: 0x000A9308 File Offset: 0x000A7508
	public static void MergeStringKeys(this IDictionary target, IDictionary addHash)
	{
		if (addHash == null || target.Equals(addHash))
		{
			return;
		}
		foreach (object obj in addHash.Keys)
		{
			if (obj is string)
			{
				target[obj] = addHash[obj];
			}
		}
	}

	// Token: 0x060019C3 RID: 6595 RVA: 0x00012ECF File Offset: 0x000110CF
	public static string ToStringFull(this IDictionary origin)
	{
		return SupportClass.DictionaryToString(origin, false);
	}

	// Token: 0x060019C4 RID: 6596 RVA: 0x000A938C File Offset: 0x000A758C
	public static string ToStringFull(this object[] data)
	{
		if (data == null)
		{
			return "null";
		}
		string[] array = new string[data.Length];
		for (int i = 0; i < data.Length; i++)
		{
			object obj = data[i];
			array[i] = ((obj == null) ? "null" : obj.ToString());
		}
		return string.Join(", ", array);
	}

	// Token: 0x060019C5 RID: 6597 RVA: 0x000A93EC File Offset: 0x000A75EC
	public static ExitGames.Client.Photon.Hashtable StripToStringKeys(this IDictionary original)
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		if (original != null)
		{
			foreach (object obj in original.Keys)
			{
				if (obj is string)
				{
					hashtable[obj] = original[obj];
				}
			}
		}
		return hashtable;
	}

	// Token: 0x060019C6 RID: 6598 RVA: 0x000A946C File Offset: 0x000A766C
	public static void StripKeysWithNullValues(this IDictionary original)
	{
		object[] array = new object[original.Count];
		int num = 0;
		foreach (object obj in original.Keys)
		{
			array[num++] = obj;
		}
		foreach (object key in array)
		{
			if (original[key] == null)
			{
				original.Remove(key);
			}
		}
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x000A9510 File Offset: 0x000A7710
	public static bool Contains(this int[] target, int nr)
	{
		if (target == null)
		{
			return false;
		}
		for (int i = 0; i < target.Length; i++)
		{
			if (target[i] == nr)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000E32 RID: 3634
	public static Dictionary<MethodInfo, ParameterInfo[]> ParametersOfMethods = new Dictionary<MethodInfo, ParameterInfo[]>();
}
