using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
public interface IPunPrefabPool
{
	// Token: 0x06001AD3 RID: 6867
	GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation);

	// Token: 0x06001AD4 RID: 6868
	void Destroy(GameObject gameObject);
}
