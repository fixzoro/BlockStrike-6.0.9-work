using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public static class vp_Utility
{
	[Obsolete("Please use 'vp_MathUtility.NaNSafeFloat' instead.")]
	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		return vp_MathUtility.NaNSafeFloat(value, prevValue);
	}

	[Obsolete("Please use 'vp_MathUtility.NaNSafeVector2' instead.")]
	public static Vector2 NaNSafeVector2(Vector2 vector, [Optional] Vector2 prevVector)
	{
		return vp_MathUtility.NaNSafeVector2(vector, prevVector);
	}

	[Obsolete("Please use 'vp_MathUtility.NaNSafeVector3' instead.")]
	public static Vector3 NaNSafeVector3(Vector3 vector, [Optional] Vector3 prevVector)
	{
		return vp_MathUtility.NaNSafeVector3(vector, prevVector);
	}

	[Obsolete("Please use 'vp_MathUtility.NaNSafeQuaternion' instead.")]
	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, [Optional] Quaternion prevQuaternion)
	{
		return vp_MathUtility.NaNSafeQuaternion(quaternion, prevQuaternion);
	}

	[Obsolete("Please use 'vp_MathUtility.SnapToZero' instead.")]
	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		return vp_MathUtility.SnapToZero(value, epsilon);
	}

	[Obsolete("Please use 'vp_MathUtility.SnapToZero' instead.")]
	public static float SnapToZero(float value, float epsilon = 0.0001f)
	{
		return vp_MathUtility.SnapToZero(value, epsilon);
	}

	[Obsolete("Please use 'vp_MathUtility.ReduceDecimals' instead.")]
	public static float ReduceDecimals(float value, float factor = 1000f)
	{
		return vp_MathUtility.ReduceDecimals(value, factor);
	}

	[Obsolete("Please use 'vp_3DUtility.HorizontalVector' instead.")]
	public static Vector3 HorizontalVector(Vector3 value)
	{
		return vp_3DUtility.HorizontalVector(value);
	}

	public static string GetErrorLocation(int level = 1, bool showOnlyLast = false)
	{
		StackTrace stackTrace = new StackTrace();
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = stackTrace.FrameCount - 1; i > level; i--)
		{
			if (i < stackTrace.FrameCount - 1)
			{
				text += " --> ";
			}
			StackFrame frame = stackTrace.GetFrame(i);
			if (frame.GetMethod().DeclaringType.ToString() == text2)
			{
				text = string.Empty;
			}
			text2 = frame.GetMethod().DeclaringType.ToString();
			text = text + text2 + ":" + frame.GetMethod().Name;
		}
		if (showOnlyLast)
		{
			try
			{
				text = text.Substring(text.LastIndexOf(" --> "));
				text = text.Replace(" --> ", string.Empty);
			}
			catch
			{
			}
		}
		return text;
	}

	public static string GetTypeAlias(Type type)
	{
		string empty = string.Empty;
		if (!vp_Utility.m_TypeAliases.TryGetValue(type, out empty))
		{
			return type.ToString();
		}
		return empty;
	}

	public static void Activate(GameObject obj, bool activate = true)
	{
		obj.SetActive(activate);
	}

	public static bool IsActive(GameObject obj)
	{
		return obj.activeSelf;
	}

	public static void RandomizeList<T>(this List<T> list)
	{
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			int index = UnityEngine.Random.Range(i, count);
			T value = list[i];
			list[i] = list[index];
			list[index] = value;
		}
	}

	public static T RandomObject<T>(this List<T> list)
	{
		List<T> list2 = new List<T>();
		list2.AddRange(list);
		list2.RandomizeList<T>();
		return list2.FirstOrDefault<T>();
	}

	public static List<T> ChildComponentsToList<T>(this Transform t) where T : Component
	{
		return t.GetComponentsInChildren<T>().ToList<T>();
	}

	public static Component GetParent(Component target)
	{
		if (target == null)
		{
			return null;
		}
		if (target != target.transform)
		{
			return target.transform;
		}
		return target.transform.parent;
	}
    
	public static int UniqueID
	{
		get
		{
			int num;
			for (;;)
			{
				num = UnityEngine.Random.Range(0, 1000000000);
				if (!vp_Utility.m_UniqueIDs.ContainsKey(num))
				{
					break;
				}
				if (vp_Utility.m_UniqueIDs.Count >= 1000000000)
				{
					vp_Utility.ClearUniqueIDs();
					UnityEngine.Debug.LogWarning("Warning (vp_Utility.UniqueID) More than 1 billion unique IDs have been generated. This seems like an awful lot for a game client. Clearing dictionary and starting over!");
				}
			}
			vp_Utility.m_UniqueIDs.Add(num, 0);
			return num;
		}
	}

	public static void ClearUniqueIDs()
	{
		vp_Utility.m_UniqueIDs.Clear();
	}

	[Obsolete("Please use 'vp_AudioUtility.PlayRandomSound' instead.")]
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds, Vector2 pitchRange)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds, pitchRange);
	}

	[Obsolete("Please use 'vp_AudioUtility.PlayRandomSound' instead.")]
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds);
	}

	private static readonly Dictionary<Type, string> m_TypeAliases = new Dictionary<Type, string>
	{
		{
			typeof(void),
			"void"
		},
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(ushort),
			"ushort"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(Vector2),
			"Vector2"
		},
		{
			typeof(Vector3),
			"Vector3"
		},
		{
			typeof(Vector4),
			"Vector4"
		}
	};

	private static Dictionary<int, int> m_UniqueIDs = new Dictionary<int, int>();
}
