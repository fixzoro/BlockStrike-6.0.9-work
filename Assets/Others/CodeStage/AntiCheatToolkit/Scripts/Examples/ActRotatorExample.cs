using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// Token: 0x020001AA RID: 426
	[AddComponentMenu("")]
	public class ActRotatorExample : MonoBehaviour
	{
		// Token: 0x06000FA6 RID: 4006 RVA: 0x0000F114 File Offset: 0x0000D314
		private void Update()
		{
			base.transform.Rotate(0f, this.speed * Time.deltaTime, 0f);
		}

		// Token: 0x04000A91 RID: 2705
		[Range(1f, 100f)]
		public float speed = 5f;
	}
}
