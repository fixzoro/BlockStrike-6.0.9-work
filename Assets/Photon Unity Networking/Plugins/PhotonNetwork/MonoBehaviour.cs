using System;
using UnityEngine;

namespace Photon
{
	// Token: 0x020002C8 RID: 712
	public class MonoBehaviour : UnityEngine.MonoBehaviour
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x000136D7 File Offset: 0x000118D7
		public PhotonView photonView
		{
			get
			{
				if (this.pvCache == null)
				{
					this.pvCache = PhotonView.Get(this);
				}
				return this.pvCache;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x000136FC File Offset: 0x000118FC
		[Obsolete("Use a photonView")]
		public new PhotonView networkView
		{
			get
			{
				Debug.LogWarning("Why are you still using networkView? should be PhotonView?");
				return PhotonView.Get(this);
			}
		}

		// Token: 0x04000FAE RID: 4014
		private PhotonView pvCache;
	}
}
