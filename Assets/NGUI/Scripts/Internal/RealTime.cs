using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class RealTime : MonoBehaviour
{
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x0600057B RID: 1403 RVA: 0x0000876A File Offset: 0x0000696A
	public static float time
	{
		get
		{
			return Time.unscaledTime;
		}
	}

	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600057C RID: 1404 RVA: 0x00008771 File Offset: 0x00006971
	public static float deltaTime
	{
		get
		{
			return Time.unscaledDeltaTime;
		}
	}
}
