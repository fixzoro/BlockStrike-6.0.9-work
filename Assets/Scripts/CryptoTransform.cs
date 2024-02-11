using System;
using UnityEngine;

public static class CryptoTransform
{
    private static Vector3 vZero = new Vector3((float)nValue.int0, (float)nValue.int0, (float)nValue.int0);

    private static Vector3 vOne = new Vector3((float)nValue.int1, (float)nValue.int1, (float)nValue.int1);

    public static void HackDetector(this Transform target, nVector position, nVector rotation, nVector scale)
	{
		if (!target.hasChanged)
		{
			return;
		}
		if (CryptoTransform.Detected(target.localPosition, position))
		{
			CheckManager.Detected();
		}
		else if (CryptoTransform.Detected(target.localEulerAngles, rotation))
		{
			CheckManager.Detected();
		}
		else if (CryptoTransform.Detected(target.localScale, scale))
		{
			CheckManager.Detected();
		}
	}

	private static bool Detected(Vector3 v, nVector a)
	{
		if (a != nVector.Zero)
		{
			return a == nVector.One && v != CryptoTransform.vOne;
		}
		return v != CryptoTransform.vZero;
	}
}
