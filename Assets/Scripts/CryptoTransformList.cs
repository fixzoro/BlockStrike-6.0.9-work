using System;
using UnityEngine;

public class CryptoTransformList : MonoBehaviour
{
    public CryptoTransformList.TransformData[] transforms = new CryptoTransformList.TransformData[0];

    private static Vector3 vZero = new Vector3((float)nValue.int0, (float)nValue.int0, (float)nValue.int0);

    private static Vector3 vOne = new Vector3((float)nValue.int1, (float)nValue.int1, (float)nValue.int1);

    private void OnEnable()
	{
		this.CheckTransform();
	}

	public void CheckTransform()
	{
		for (int i = 0; i < this.transforms.Length; i++)
		{
			if (!(this.transforms[i].target == null))
			{
				if (this.transforms[i].target.hasChanged)
				{
					if (this.Detected(this.transforms[i].target.localPosition, this.transforms[i].Position))
					{
						CheckManager.Detected();
					}
					else if (this.Detected(this.transforms[i].target.localEulerAngles, this.transforms[i].Rotation))
					{
						CheckManager.Detected();
					}
					else if (this.Detected(this.transforms[i].target.localScale, this.transforms[i].Scale))
					{
						CheckManager.Detected();
					}
				}
			}
		}
	}

	private bool Detected(Vector3 v, CryptoTransformList.Axis a)
	{
		if (a != CryptoTransformList.Axis.Zero)
		{
			return a == CryptoTransformList.Axis.One && v != CryptoTransformList.vOne;
		}
		return v != CryptoTransformList.vZero;
	}

	public enum Axis
	{
		Off,
		Zero,
		One
	}

	[Serializable]
	public class TransformData
	{
		public Transform target;

		public CryptoTransformList.Axis Position;

		public CryptoTransformList.Axis Rotation;

		public CryptoTransformList.Axis Scale = CryptoTransformList.Axis.One;
	}
}
