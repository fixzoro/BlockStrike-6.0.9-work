using System;

// Token: 0x020002CF RID: 719
public class SceneManagerHelper
{
	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06001B01 RID: 6913 RVA: 0x00013784 File Offset: 0x00011984
	public static string ActiveSceneName
	{
		get
		{
			return LevelManager.GetSceneName();
		}
	}
}
