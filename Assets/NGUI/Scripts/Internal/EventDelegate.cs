using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x02000097 RID: 151
[Serializable]
public class EventDelegate
{
	// Token: 0x06000431 RID: 1073 RVA: 0x00004734 File Offset: 0x00002934
	public EventDelegate()
	{
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00007BC1 File Offset: 0x00005DC1
	public EventDelegate(EventDelegate.Callback call)
	{
		this.Set(call);
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00007BD0 File Offset: 0x00005DD0
	public EventDelegate(MonoBehaviour target, string methodName)
	{
		this.Set(target, methodName);
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000435 RID: 1077 RVA: 0x00007BF1 File Offset: 0x00005DF1
	// (set) Token: 0x06000436 RID: 1078 RVA: 0x00007BF9 File Offset: 0x00005DF9
	public MonoBehaviour target
	{
		get
		{
			return this.mTarget;
		}
		set
		{
			this.mTarget = value;
			this.mCachedCallback = null;
			this.mRawDelegate = false;
			this.mCached = false;
			this.mMethod = null;
			this.mParameterInfos = null;
			this.mParameters = null;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000437 RID: 1079 RVA: 0x00007C2C File Offset: 0x00005E2C
	// (set) Token: 0x06000438 RID: 1080 RVA: 0x00007C34 File Offset: 0x00005E34
	public string methodName
	{
		get
		{
			return this.mMethodName;
		}
		set
		{
			this.mMethodName = value;
			this.mCachedCallback = null;
			this.mRawDelegate = false;
			this.mCached = false;
			this.mMethod = null;
			this.mParameterInfos = null;
			this.mParameters = null;
		}
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000439 RID: 1081 RVA: 0x00007C67 File Offset: 0x00005E67
	public EventDelegate.Parameter[] parameters
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			return this.mParameters;
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x0600043A RID: 1082 RVA: 0x000322D8 File Offset: 0x000304D8
	public bool isValid
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			return (this.mRawDelegate && this.mCachedCallback != null) || (this.mTarget != null && !string.IsNullOrEmpty(this.mMethodName));
		}
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600043B RID: 1083 RVA: 0x00032334 File Offset: 0x00030534
	public bool isEnabled
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			if (this.mRawDelegate && this.mCachedCallback != null)
			{
				return true;
			}
			if (this.mTarget == null)
			{
				return false;
			}
			MonoBehaviour monoBehaviour = this.mTarget;
			return monoBehaviour == null || monoBehaviour.enabled;
		}
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00007C80 File Offset: 0x00005E80
	private static string GetMethodName(EventDelegate.Callback callback)
	{
		return callback.Method.Name;
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00007C8D File Offset: 0x00005E8D
	private static bool IsValid(EventDelegate.Callback callback)
	{
		return callback != null && callback.Method != null;
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x0003239C File Offset: 0x0003059C
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return !this.isValid;
		}
		if (obj is EventDelegate.Callback)
		{
			EventDelegate.Callback callback = obj as EventDelegate.Callback;
			if (callback.Equals(this.mCachedCallback))
			{
				return true;
			}
			MonoBehaviour y = callback.Target as MonoBehaviour;
			return this.mTarget == y && string.Equals(this.mMethodName, EventDelegate.GetMethodName(callback));
		}
		else
		{
			if (obj is EventDelegate)
			{
				EventDelegate eventDelegate = obj as EventDelegate;
				return this.mTarget == eventDelegate.mTarget && string.Equals(this.mMethodName, eventDelegate.mMethodName);
			}
			return false;
		}
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00007CA4 File Offset: 0x00005EA4
	public override int GetHashCode()
	{
		return EventDelegate.s_Hash;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00032450 File Offset: 0x00030650
	private void Set(EventDelegate.Callback call)
	{
		this.Clear();
		if (call != null && EventDelegate.IsValid(call))
		{
			this.mTarget = (call.Target as MonoBehaviour);
			if (this.mTarget == null)
			{
				this.mRawDelegate = true;
				this.mCachedCallback = call;
				this.mMethodName = null;
			}
			else
			{
				this.mMethodName = EventDelegate.GetMethodName(call);
				this.mRawDelegate = false;
			}
		}
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00007CAB File Offset: 0x00005EAB
	public void Set(MonoBehaviour target, string methodName)
	{
		this.Clear();
		this.mTarget = target;
		this.mMethodName = methodName;
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000324C4 File Offset: 0x000306C4
	private void Cache()
	{
		this.mCached = true;
		if (this.mRawDelegate)
		{
			return;
		}
		if ((this.mCachedCallback == null || this.mCachedCallback.Target as MonoBehaviour != this.mTarget || EventDelegate.GetMethodName(this.mCachedCallback) != this.mMethodName) && this.mTarget != null && !string.IsNullOrEmpty(this.mMethodName))
		{
			Type type = this.mTarget.GetType();
			this.mMethod = null;
			while (type != null)
			{
				try
				{
					this.mMethod = type.GetMethod(this.mMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (this.mMethod != null)
					{
						break;
					}
				}
				catch (Exception)
				{
				}
				type = type.BaseType;
			}
			if (this.mMethod == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Could not find method '",
					this.mMethodName,
					"' on ",
					this.mTarget.GetType()
				}), this.mTarget);
				return;
			}
			if (this.mMethod.ReturnType != typeof(void))
			{
				Debug.LogError(string.Concat(new object[]
				{
					this.mTarget.GetType(),
					".",
					this.mMethodName,
					" must have a 'void' return type."
				}), this.mTarget);
				return;
			}
			this.mParameterInfos = this.mMethod.GetParameters();
			if (this.mParameterInfos.Length == 0)
			{
				this.mCachedCallback = (EventDelegate.Callback)Delegate.CreateDelegate(typeof(EventDelegate.Callback), this.mTarget, this.mMethodName);
				this.mArgs = null;
				this.mParameters = null;
				return;
			}
			this.mCachedCallback = null;
			if (this.mParameters == null || this.mParameters.Length != this.mParameterInfos.Length)
			{
				this.mParameters = new EventDelegate.Parameter[this.mParameterInfos.Length];
				int i = 0;
				int num = this.mParameters.Length;
				while (i < num)
				{
					this.mParameters[i] = new EventDelegate.Parameter();
					i++;
				}
			}
			int j = 0;
			int num2 = this.mParameters.Length;
			while (j < num2)
			{
				this.mParameters[j].expectedType = this.mParameterInfos[j].ParameterType;
				j++;
			}
		}
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x0003273C File Offset: 0x0003093C
	public bool Execute()
	{
		if (!this.mCached)
		{
			this.Cache();
		}
		if (this.mCachedCallback != null)
		{
			this.mCachedCallback();
			return true;
		}
		if (this.mMethod != null)
		{
			if (this.mParameters == null || this.mParameters.Length == 0)
			{
				this.mMethod.Invoke(this.mTarget, null);
			}
			else
			{
				if (this.mArgs == null || this.mArgs.Length != this.mParameters.Length)
				{
					this.mArgs = new object[this.mParameters.Length];
				}
				int i = 0;
				int num = this.mParameters.Length;
				while (i < num)
				{
					this.mArgs[i] = this.mParameters[i].value;
					i++;
				}
				try
				{
					this.mMethod.Invoke(this.mTarget, this.mArgs);
				}
				catch (ArgumentException ex)
				{
					string text = "Error calling ";
					if (this.mTarget == null)
					{
						text += this.mMethod.Name;
					}
					else
					{
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							this.mTarget.GetType(),
							".",
							this.mMethod.Name
						});
					}
					text = text + ": " + ex.Message;
					text += "\n  Expected: ";
					if (this.mParameterInfos.Length == 0)
					{
						text += "no arguments";
					}
					else
					{
						text += this.mParameterInfos[0];
						for (int j = 1; j < this.mParameterInfos.Length; j++)
						{
							text = text + ", " + this.mParameterInfos[j].ParameterType;
						}
					}
					text += "\n  Received: ";
					if (this.mParameters.Length == 0)
					{
						text += "no arguments";
					}
					else
					{
						text += this.mParameters[0].type;
						for (int k = 1; k < this.mParameters.Length; k++)
						{
							text = text + ", " + this.mParameters[k].type;
						}
					}
					text += "\n";
					Debug.LogError(text);
				}
				int l = 0;
				int num2 = this.mArgs.Length;
				while (l < num2)
				{
					if (this.mParameterInfos[l].IsIn || this.mParameterInfos[l].IsOut)
					{
						this.mParameters[l].value = this.mArgs[l];
					}
					this.mArgs[l] = null;
					l++;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00032A4C File Offset: 0x00030C4C
	public void Clear()
	{
		this.mTarget = null;
		this.mMethodName = null;
		this.mRawDelegate = false;
		this.mCachedCallback = null;
		this.mParameters = null;
		this.mCached = false;
		this.mMethod = null;
		this.mParameterInfos = null;
		this.mArgs = null;
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00032A98 File Offset: 0x00030C98
	public override string ToString()
	{
		if (!(this.mTarget != null))
		{
			return (!this.mRawDelegate) ? null : "[delegate]";
		}
		string text = this.mTarget.GetType().ToString();
		int num = text.LastIndexOf('.');
		if (num > 0)
		{
			text = text.Substring(num + 1);
		}
		if (!string.IsNullOrEmpty(this.methodName))
		{
			return text + "/" + this.methodName;
		}
		return text + "/[delegate]";
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00032B28 File Offset: 0x00030D28
	public static void Execute(List<EventDelegate> list)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null)
				{
					try
					{
						eventDelegate.Execute();
					}
					catch (Exception ex)
					{
						if (ex.InnerException != null)
						{
							Debug.LogException(ex.InnerException);
						}
						else
						{
							Debug.LogException(ex);
						}
					}
					if (i >= list.Count)
					{
						break;
					}
					if (list[i] != eventDelegate)
					{
						continue;
					}
					if (eventDelegate.oneShot)
					{
						list.RemoveAt(i);
						continue;
					}
				}
			}
		}
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00032BE0 File Offset: 0x00030DE0
	public static bool IsValid(List<EventDelegate> list)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.isValid)
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00032C28 File Offset: 0x00030E28
	public static EventDelegate Set(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		if (list != null)
		{
			EventDelegate eventDelegate = new EventDelegate(callback);
			list.Clear();
			list.Add(eventDelegate);
			return eventDelegate;
		}
		return null;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00007CC1 File Offset: 0x00005EC1
	public static void Set(List<EventDelegate> list, EventDelegate del)
	{
		if (list != null)
		{
			list.Clear();
			list.Add(del);
		}
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00007CD6 File Offset: 0x00005ED6
	public static EventDelegate Add(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		return EventDelegate.Add(list, callback, false);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00032C54 File Offset: 0x00030E54
	public static EventDelegate Add(List<EventDelegate> list, EventDelegate.Callback callback, bool oneShot)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					return eventDelegate;
				}
				i++;
			}
			EventDelegate eventDelegate2 = new EventDelegate(callback);
			eventDelegate2.oneShot = oneShot;
			list.Add(eventDelegate2);
			return eventDelegate2;
		}
		Debug.LogWarning("Attempting to add a callback to a list that's null");
		return null;
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00007CE0 File Offset: 0x00005EE0
	public static void Add(List<EventDelegate> list, EventDelegate ev)
	{
		EventDelegate.Add(list, ev, ev.oneShot);
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00032CC0 File Offset: 0x00030EC0
	public static void Add(List<EventDelegate> list, EventDelegate ev, bool oneShot)
	{
		if (ev.mRawDelegate || ev.target == null || string.IsNullOrEmpty(ev.methodName))
		{
			EventDelegate.Add(list, ev.mCachedCallback, oneShot);
		}
		else if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(ev))
				{
					return;
				}
				i++;
			}
			EventDelegate eventDelegate2 = new EventDelegate(ev.target, ev.methodName);
			eventDelegate2.oneShot = oneShot;
			if (ev.mParameters != null && ev.mParameters.Length > 0)
			{
				eventDelegate2.mParameters = new EventDelegate.Parameter[ev.mParameters.Length];
				for (int j = 0; j < ev.mParameters.Length; j++)
				{
					eventDelegate2.mParameters[j] = ev.mParameters[j];
				}
			}
			list.Add(eventDelegate2);
		}
		else
		{
			Debug.LogWarning("Attempting to add a callback to a list that's null");
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00032DD0 File Offset: 0x00030FD0
	public static bool Remove(List<EventDelegate> list, EventDelegate.Callback callback)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(callback))
				{
					list.RemoveAt(i);
					return true;
				}
				i++;
			}
		}
		return false;
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00032DD0 File Offset: 0x00030FD0
	public static bool Remove(List<EventDelegate> list, EventDelegate ev)
	{
		if (list != null)
		{
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EventDelegate eventDelegate = list[i];
				if (eventDelegate != null && eventDelegate.Equals(ev))
				{
					list.RemoveAt(i);
					return true;
				}
				i++;
			}
		}
		return false;
	}

	// Token: 0x04000348 RID: 840
	[SerializeField]
	private MonoBehaviour mTarget;

	// Token: 0x04000349 RID: 841
	[SerializeField]
	private string mMethodName;

	// Token: 0x0400034A RID: 842
	[SerializeField]
	private EventDelegate.Parameter[] mParameters;

	// Token: 0x0400034B RID: 843
	public bool oneShot;

	// Token: 0x0400034C RID: 844
	[NonSerialized]
	private EventDelegate.Callback mCachedCallback;

	// Token: 0x0400034D RID: 845
	[NonSerialized]
	private bool mRawDelegate;

	// Token: 0x0400034E RID: 846
	[NonSerialized]
	private bool mCached;

	// Token: 0x0400034F RID: 847
	[NonSerialized]
	private MethodInfo mMethod;

	// Token: 0x04000350 RID: 848
	[NonSerialized]
	private ParameterInfo[] mParameterInfos;

	// Token: 0x04000351 RID: 849
	[NonSerialized]
	private object[] mArgs;

	// Token: 0x04000352 RID: 850
	private static int s_Hash = "EventDelegate".GetHashCode();

	// Token: 0x02000098 RID: 152
	[Serializable]
	public class Parameter
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x00007CEF File Offset: 0x00005EEF
		public Parameter()
		{
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00007D07 File Offset: 0x00005F07
		public Parameter(UnityEngine.Object obj, string field)
		{
			this.obj = obj;
			this.field = field;
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00007D2D File Offset: 0x00005F2D
		public Parameter(object val)
		{
			this.mValue = val;
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000453 RID: 1107 RVA: 0x00032E20 File Offset: 0x00031020
		// (set) Token: 0x06000454 RID: 1108 RVA: 0x00007D4C File Offset: 0x00005F4C
		public object value
		{
			get
			{
				if (this.mValue != null)
				{
					return this.mValue;
				}
				if (!this.cached)
				{
					this.cached = true;
					this.fieldInfo = null;
					this.propInfo = null;
					if (this.obj != null && !string.IsNullOrEmpty(this.field))
					{
						Type type = this.obj.GetType();
						this.propInfo = type.GetProperty(this.field);
						if (this.propInfo == null)
						{
							this.fieldInfo = type.GetField(this.field);
						}
					}
				}
				if (this.propInfo != null)
				{
					return this.propInfo.GetValue(this.obj, null);
				}
				if (this.fieldInfo != null)
				{
					return this.fieldInfo.GetValue(this.obj);
				}
				if (this.obj != null)
				{
					return this.obj;
				}
				if (this.expectedType != null && this.expectedType.IsValueType)
				{
					return null;
				}
				return Convert.ChangeType(null, this.expectedType);
			}
			set
			{
				this.mValue = value;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x00007D55 File Offset: 0x00005F55
		public Type type
		{
			get
			{
				if (this.mValue != null)
				{
					return this.mValue.GetType();
				}
				if (this.obj == null)
				{
					return typeof(void);
				}
				return this.obj.GetType();
			}
		}

		// Token: 0x04000353 RID: 851
		public UnityEngine.Object obj;

		// Token: 0x04000354 RID: 852
		public string field;

		// Token: 0x04000355 RID: 853
		[NonSerialized]
		private object mValue;

		// Token: 0x04000356 RID: 854
		[NonSerialized]
		public Type expectedType = typeof(void);

		// Token: 0x04000357 RID: 855
		[NonSerialized]
		public bool cached;

		// Token: 0x04000358 RID: 856
		[NonSerialized]
		public PropertyInfo propInfo;

		// Token: 0x04000359 RID: 857
		[NonSerialized]
		public FieldInfo fieldInfo;
	}

	// Token: 0x02000099 RID: 153
	// (Invoke) Token: 0x06000457 RID: 1111
	public delegate void Callback();
}
