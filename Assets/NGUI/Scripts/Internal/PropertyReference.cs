using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

// Token: 0x020000AA RID: 170
[Serializable]
public class PropertyReference
{
	// Token: 0x06000562 RID: 1378 RVA: 0x00004734 File Offset: 0x00002934
	public PropertyReference()
	{
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00008674 File Offset: 0x00006874
	public PropertyReference(Component target, string fieldName)
	{
		this.mTarget = target;
		this.mName = fieldName;
	}

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x06000565 RID: 1381 RVA: 0x0000869B File Offset: 0x0000689B
	// (set) Token: 0x06000566 RID: 1382 RVA: 0x000086A3 File Offset: 0x000068A3
	public Component target
	{
		get
		{
			return this.mTarget;
		}
		set
		{
			this.mTarget = value;
			this.mProperty = null;
			this.mField = null;
		}
	}

	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000567 RID: 1383 RVA: 0x000086BA File Offset: 0x000068BA
	// (set) Token: 0x06000568 RID: 1384 RVA: 0x000086C2 File Offset: 0x000068C2
	public string name
	{
		get
		{
			return this.mName;
		}
		set
		{
			this.mName = value;
			this.mProperty = null;
			this.mField = null;
		}
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000569 RID: 1385 RVA: 0x000086D9 File Offset: 0x000068D9
	public bool isValid
	{
		get
		{
			return this.mTarget != null && !string.IsNullOrEmpty(this.mName);
		}
	}

	// Token: 0x17000074 RID: 116
	// (get) Token: 0x0600056A RID: 1386 RVA: 0x0003E610 File Offset: 0x0003C810
	public bool isEnabled
	{
		get
		{
			if (this.mTarget == null)
			{
				return false;
			}
			MonoBehaviour monoBehaviour = this.mTarget as MonoBehaviour;
			return monoBehaviour == null || monoBehaviour.enabled;
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x0003E654 File Offset: 0x0003C854
	public Type GetPropertyType()
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty != null)
		{
			return this.mProperty.PropertyType;
		}
		if (this.mField != null)
		{
			return this.mField.FieldType;
		}
		return typeof(void);
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x0003E6C4 File Offset: 0x0003C8C4
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return !this.isValid;
		}
		if (obj is PropertyReference)
		{
			PropertyReference propertyReference = obj as PropertyReference;
			return this.mTarget == propertyReference.mTarget && string.Equals(this.mName, propertyReference.mName);
		}
		return false;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x000086FD File Offset: 0x000068FD
	public override int GetHashCode()
	{
		return PropertyReference.s_Hash;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00008704 File Offset: 0x00006904
	public void Set(Component target, string methodName)
	{
		this.mTarget = target;
		this.mName = methodName;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x00008714 File Offset: 0x00006914
	public void Clear()
	{
		this.mTarget = null;
		this.mName = null;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x00008724 File Offset: 0x00006924
	public void Reset()
	{
		this.mField = null;
		this.mProperty = null;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00008734 File Offset: 0x00006934
	public override string ToString()
	{
		return PropertyReference.ToString(this.mTarget, this.name);
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0003E720 File Offset: 0x0003C920
	public static string ToString(Component comp, string property)
	{
		if (!(comp != null))
		{
			return null;
		}
		string text = comp.GetType().ToString();
		int num = text.LastIndexOf('.');
		if (num > 0)
		{
			text = text.Substring(num + 1);
		}
		if (!string.IsNullOrEmpty(property))
		{
			return text + "." + property;
		}
		return text + ".[property]";
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0003E784 File Offset: 0x0003C984
	[DebuggerHidden]
	[DebuggerStepThrough]
	public object Get()
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty != null)
		{
			if (this.mProperty.CanRead)
			{
				return this.mProperty.GetValue(this.mTarget, null);
			}
		}
		else if (this.mField != null)
		{
			return this.mField.GetValue(this.mTarget);
		}
		return null;
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0003E80C File Offset: 0x0003CA0C
	[DebuggerStepThrough]
	[DebuggerHidden]
	public bool Set(object value)
	{
		if (this.mProperty == null && this.mField == null && this.isValid)
		{
			this.Cache();
		}
		if (this.mProperty == null && this.mField == null)
		{
			return false;
		}
		if (value == null)
		{
			try
			{
				if (this.mProperty == null)
				{
					this.mField.SetValue(this.mTarget, null);
					return true;
				}
				if (this.mProperty.CanWrite)
				{
					this.mProperty.SetValue(this.mTarget, null, null);
					return true;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
		if (!this.Convert(ref value))
		{
			if (Application.isPlaying)
			{
				UnityEngine.Debug.LogError(string.Concat(new object[]
				{
					"Unable to convert ",
					value.GetType(),
					" to ",
					this.GetPropertyType()
				}));
			}
		}
		else
		{
			if (this.mField != null)
			{
				this.mField.SetValue(this.mTarget, value);
				return true;
			}
			if (this.mProperty.CanWrite)
			{
				this.mProperty.SetValue(this.mTarget, value, null);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0003E96C File Offset: 0x0003CB6C
	[DebuggerStepThrough]
	[DebuggerHidden]
	private bool Cache()
	{
		if (this.mTarget != null && !string.IsNullOrEmpty(this.mName))
		{
			Type type = this.mTarget.GetType();
			this.mField = type.GetField(this.mName);
			this.mProperty = type.GetProperty(this.mName);
		}
		else
		{
			this.mField = null;
			this.mProperty = null;
		}
		return this.mField != null || this.mProperty != null;
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x0003E9F8 File Offset: 0x0003CBF8
	private bool Convert(ref object value)
	{
		if (this.mTarget == null)
		{
			return false;
		}
		Type propertyType = this.GetPropertyType();
		Type from;
		if (value == null)
		{
			if (!propertyType.IsClass)
			{
				return false;
			}
			from = propertyType;
		}
		else
		{
			from = value.GetType();
		}
		return PropertyReference.Convert(ref value, from, propertyType);
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0003EA4C File Offset: 0x0003CC4C
	public static bool Convert(Type from, Type to)
	{
		object obj = null;
		return PropertyReference.Convert(ref obj, from, to);
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00008747 File Offset: 0x00006947
	public static bool Convert(object value, Type to)
	{
		if (value == null)
		{
			value = null;
			return PropertyReference.Convert(ref value, to, to);
		}
		return PropertyReference.Convert(ref value, value.GetType(), to);
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0003EA64 File Offset: 0x0003CC64
	public static bool Convert(ref object value, Type from, Type to)
	{
		if (to.IsAssignableFrom(from))
		{
			return true;
		}
		if (to == typeof(string))
		{
			value = ((value == null) ? "null" : value.ToString());
			return true;
		}
		if (value == null)
		{
			return false;
		}
		if (to == typeof(int))
		{
			if (from == typeof(string))
			{
				int num;
				if (int.TryParse((string)value, out num))
				{
					value = num;
					return true;
				}
			}
			else
			{
				if (from == typeof(float))
				{
					value = Mathf.RoundToInt((float)value);
					return true;
				}
				if (from == typeof(double))
				{
					value = (int)Math.Round((double)value);
				}
			}
		}
		else if (to == typeof(float))
		{
			if (from == typeof(string))
			{
				float num2;
				if (float.TryParse((string)value, out num2))
				{
					value = num2;
					return true;
				}
			}
			else if (from == typeof(double))
			{
				value = (float)((double)value);
			}
			else if (from == typeof(int))
			{
				value = (float)((int)value);
			}
		}
		else if (to == typeof(double))
		{
			if (from == typeof(string))
			{
				double num3;
				if (double.TryParse((string)value, out num3))
				{
					value = num3;
					return true;
				}
			}
			else if (from == typeof(float))
			{
				value = (double)((float)value);
			}
			else if (from == typeof(int))
			{
				value = (double)((int)value);
			}
		}
		return false;
	}

	// Token: 0x040003C0 RID: 960
	[SerializeField]
	private Component mTarget;

	// Token: 0x040003C1 RID: 961
	[SerializeField]
	private string mName;

	// Token: 0x040003C2 RID: 962
	private FieldInfo mField;

	// Token: 0x040003C3 RID: 963
	private PropertyInfo mProperty;

	// Token: 0x040003C4 RID: 964
	private static int s_Hash = "PropertyBinding".GetHashCode();
}
