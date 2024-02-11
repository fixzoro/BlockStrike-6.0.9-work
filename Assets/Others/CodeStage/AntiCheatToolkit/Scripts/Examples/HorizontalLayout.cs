using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// Token: 0x020001AD RID: 429
	internal class HorizontalLayout : IDisposable
	{
		// Token: 0x06000FC7 RID: 4039 RVA: 0x0000F2E5 File Offset: 0x0000D4E5
		public HorizontalLayout(params GUILayoutOption[] options)
		{
			GUILayout.BeginHorizontal(options);
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x0000F2F3 File Offset: 0x0000D4F3
		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}
