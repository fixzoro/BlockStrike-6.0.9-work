using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// Token: 0x020001AE RID: 430
	internal class VerticalLayout : IDisposable
	{
		// Token: 0x06000FC9 RID: 4041 RVA: 0x0000F2FA File Offset: 0x0000D4FA
		public VerticalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginVertical(options);
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0000F308 File Offset: 0x0000D508
		public VerticalLayout(GUIStyle style)
		{
			GUILayout.BeginVertical(style, new GUILayoutOption[0]);
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0000F2F3 File Offset: 0x0000D4F3
		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
