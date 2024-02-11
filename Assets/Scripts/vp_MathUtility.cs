using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class vp_MathUtility
{
	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		value = ((!double.IsNaN((double)value)) ? value : prevValue);
		return value;
	}

	public static Vector2 NaNSafeVector2(Vector2 vector, [Optional] Vector2 prevVector)
	{
		vector.x = ((!double.IsNaN((double)vector.x)) ? vector.x : prevVector.x);
		vector.y = ((!double.IsNaN((double)vector.y)) ? vector.y : prevVector.y);
		return vector;
	}

	public static Vector3 NaNSafeVector3(Vector3 vector, [Optional] Vector3 prevVector)
	{
		vector.x = ((!double.IsNaN((double)vector.x)) ? vector.x : prevVector.x);
		vector.y = ((!double.IsNaN((double)vector.y)) ? vector.y : prevVector.y);
		vector.z = ((!double.IsNaN((double)vector.z)) ? vector.z : prevVector.z);
		return vector;
	}

	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, [Optional] Quaternion prevQuaternion)
	{
		quaternion.x = ((!double.IsNaN((double)quaternion.x)) ? quaternion.x : prevQuaternion.x);
		quaternion.y = ((!double.IsNaN((double)quaternion.y)) ? quaternion.y : prevQuaternion.y);
		quaternion.z = ((!double.IsNaN((double)quaternion.z)) ? quaternion.z : prevQuaternion.z);
		quaternion.w = ((!double.IsNaN((double)quaternion.w)) ? quaternion.w : prevQuaternion.w);
		return quaternion;
	}

	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		value.x = ((Mathf.Abs(value.x) >= epsilon) ? value.x : ((float)nValue.int0));
		value.y = ((Mathf.Abs(value.y) >= epsilon) ? value.y : ((float)nValue.int0));
		value.z = ((Mathf.Abs(value.z) >= epsilon) ? value.z : ((float)nValue.int0));
		return value;
	}

	public static float SnapToZero(float value, float epsilon = 0.0001f)
	{
		value = ((Mathf.Abs(value) >= epsilon) ? value : ((float)nValue.int0));
		return value;
	}

	public static float ReduceDecimals(float value, float factor = 1000f)
	{
		return Mathf.Round(value * factor) / factor;
	}

	public static float Sinus(float rate, float amp, float offset = 0f)
	{
		return Mathf.Cos((Time.time + offset) * rate) * amp;
	}
}
