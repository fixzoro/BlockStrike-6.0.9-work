using System;
using UnityEngine;

[Serializable]
public struct CryptoFloatFast : IFormattable, IEquatable<CryptoFloatFast>
{
    [SerializeField]
    private float defaultValue;

    [SerializeField]
    private float hiddenValue;

    [SerializeField]
    private int randomValue;

    [SerializeField]
    private bool inited;

    private CryptoFloatFast(float value)
	{
		this.defaultValue = value;
		this.randomValue = CryptoManager.staticValue;
		this.hiddenValue = value + (float)this.randomValue;
		this.inited = true;
	}

	public void SetValue(float value)
	{
		this.defaultValue = value;
		this.randomValue = CryptoManager.staticValue;
		this.hiddenValue = value + (float)this.randomValue;
	}

	public void CheckValue()
	{
		if (this.defaultValue - (this.hiddenValue - (float)this.randomValue) >= nValue.float001)
		{
			CheckManager.Detected("Controller Error 23");
		}
	}

	public float GetValue()
	{
		if (!this.inited)
		{
			this.defaultValue = 0f;
			this.randomValue = CryptoManager.staticValue;
			this.hiddenValue = this.defaultValue + (float)this.randomValue;
		}
		return this.defaultValue;
	}

	public override bool Equals(object obj)
	{
		return obj is CryptoFloatFast && this.Equals((CryptoFloatFast)obj);
	}

	public bool Equals(CryptoFloatFast obj)
	{
		return this.GetValue() == obj.GetValue();
	}

	public override int GetHashCode()
	{
		return this.GetValue().GetHashCode();
	}

	public override string ToString()
	{
		return this.GetValue().ToString();
	}

	public string ToString(string format)
	{
		return this.GetValue().ToString(format);
	}

	public string ToString(IFormatProvider provider)
	{
		return this.GetValue().ToString(provider);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return this.GetValue().ToString(format, provider);
	}

	public static implicit operator CryptoFloatFast(float value)
	{
		CryptoFloatFast result = new CryptoFloatFast(value);
		return result;
	}

	public static implicit operator float(CryptoFloatFast value)
	{
		return value.GetValue();
	}

	public static CryptoFloatFast operator ++(CryptoFloatFast value)
	{
		value.SetValue(value.GetValue() + 1f);
		return value;
	}

	public static CryptoFloatFast operator --(CryptoFloatFast value)
	{
		value.SetValue(value.GetValue() - 1f);
		return value;
	}
}
