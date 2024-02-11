using System;
using UnityEngine;

// Token: 0x0200056D RID: 1389
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class UIUpdateManager : MonoBehaviour
{
	// Token: 0x06002EBE RID: 11966 RVA: 0x000207A4 File Offset: 0x0001E9A4
	public UIUpdateManager()
	{
		UIUpdateManager.instance = this;
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x000207CA File Offset: 0x0001E9CA
	public static void AddItem(UIMonoBehaviour behaviour)
	{
		UIUpdateManager.instance.AddItemToArray(behaviour);
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000207D7 File Offset: 0x0001E9D7
	public static void RemoveSpecificItem(UIMonoBehaviour behaviour)
	{
		UIUpdateManager.instance.RemoveSpecificItemFromArray(behaviour);
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000207E4 File Offset: 0x0001E9E4
	public static void RemoveSpecificItemAndDestroyIt(UIMonoBehaviour behaviour)
	{
		UIUpdateManager.instance.RemoveSpecificItemFromArray(behaviour);
		UnityEngine.Object.Destroy(behaviour.gameObject);
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x0010E110 File Offset: 0x0010C310
	private void AddItemToArray(UIMonoBehaviour behaviour)
	{
		if (behaviour.GetType().GetMethod("nUpdate").DeclaringType != typeof(UIMonoBehaviour))
		{
			this.regularArray = this.ExtendAndAddItemToArray(this.regularArray, behaviour);
			this.regularUpdateArrayCount++;
		}
		if (behaviour.GetType().GetMethod("nLateUpdate").DeclaringType == typeof(UIMonoBehaviour))
		{
			return;
		}
		this.lateArray = this.ExtendAndAddItemToArray(this.lateArray, behaviour);
		this.lateUpdateArrayCount++;
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x0010E1A8 File Offset: 0x0010C3A8
	public UIMonoBehaviour[] ExtendAndAddItemToArray(UIMonoBehaviour[] original, UIMonoBehaviour itemToAdd)
	{
		int num = original.Length;
		UIMonoBehaviour[] array = new UIMonoBehaviour[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = original[i];
		}
		array[array.Length - 1] = itemToAdd;
		return array;
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x0010E1E4 File Offset: 0x0010C3E4
	private void RemoveSpecificItemFromArray(UIMonoBehaviour behaviour)
	{
		if (this.CheckIfArrayContainsItem(this.regularArray, behaviour))
		{
			this.regularArray = this.ShrinkAndRemoveItemToArray(this.regularArray, behaviour);
			this.regularUpdateArrayCount--;
		}
		if (!this.CheckIfArrayContainsItem(this.lateArray, behaviour))
		{
			return;
		}
		this.lateArray = this.ShrinkAndRemoveItemToArray(this.lateArray, behaviour);
		this.lateUpdateArrayCount--;
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x0010E258 File Offset: 0x0010C458
	public bool CheckIfArrayContainsItem(UIMonoBehaviour[] arrayToCheck, UIMonoBehaviour objectToCheckFor)
	{
		int num = arrayToCheck.Length;
		for (int i = 0; i < num; i++)
		{
			if (objectToCheckFor == arrayToCheck[i])
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x0010E28C File Offset: 0x0010C48C
	public UIMonoBehaviour[] ShrinkAndRemoveItemToArray(UIMonoBehaviour[] original, UIMonoBehaviour itemToRemove)
	{
		int num = original.Length;
		UIMonoBehaviour[] array = new UIMonoBehaviour[num - 1];
		for (int i = 0; i < num; i++)
		{
			if (!(original[i] == itemToRemove))
			{
				array[i] = original[i];
			}
		}
		return array;
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x0010E2D4 File Offset: 0x0010C4D4
	private void Update()
	{
		this.frameCount = Time.frameCount;
		if (this.regularUpdateArrayCount == 0)
		{
			return;
		}
		for (int i = 0; i < this.regularUpdateArrayCount; i++)
		{
			if (!(this.regularArray[i] == null))
			{
				this.regularArray[i].nUpdate(this.frameCount);
			}
		}
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x0010E33C File Offset: 0x0010C53C
	private void LateUpdate()
	{
		if (this.lateUpdateArrayCount == 0)
		{
			return;
		}
		for (int i = 0; i < this.lateUpdateArrayCount; i++)
		{
			if (!(this.lateArray[i] == null))
			{
				this.lateArray[i].nLateUpdate(this.frameCount);
			}
		}
	}

	// Token: 0x04001E0A RID: 7690
	private static UIUpdateManager instance;

	// Token: 0x04001E0B RID: 7691
	private int regularUpdateArrayCount;

	// Token: 0x04001E0C RID: 7692
	private int lateUpdateArrayCount;

	// Token: 0x04001E0D RID: 7693
	private UIMonoBehaviour[] regularArray = new UIMonoBehaviour[0];

	// Token: 0x04001E0E RID: 7694
	private UIMonoBehaviour[] lateArray = new UIMonoBehaviour[0];

	// Token: 0x04001E0F RID: 7695
	private int frameCount;
}
