using UnityEngine;
using System.Collections;

namespace BSCM.Game.Modes.Bomb{
	public class Settings : MonoBehaviour {

#if UNITY_EDITOR
		void OnDrawGizmos() {
			if(name != "BombSettings")
				name = "BombSettings";

			CreateTrigger("Zone A");
			CreateTrigger("Zone B");
			CreateTrigger("Zone Weapon Red");
			CreateTrigger("Zone Weapon Blue");
		}

		void CreateTrigger(string nam){
			Transform go = transform.Find(nam);
			if(go == null){
				go = new GameObject(nam).transform;
				go.SetParent(transform);
			}
			BoxCollider colliderGO = go.GetComponent<BoxCollider>();
			if(colliderGO == null){
				colliderGO = go.gameObject.AddComponent<BoxCollider>();
			}
			colliderGO.isTrigger = true;
		}
#endif
	}
}
