using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSCM.Game.Others{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class Teleport : MonoBehaviour
    {
		public Vector3 to;

        private void Start()
        {
            TriggerTeleport triggerTeleport = base.gameObject.AddComponent<TriggerTeleport>();
            triggerTeleport.Position = this.to;
        }
#if UNITY_EDITOR
        private BoxCollider boxCollider;

		void OnDrawGizmosSelected() {
			Gizmos.color = new Color32(0,0,200,160);
			Gizmos.DrawLine(transform.position,to);
			Gizmos.DrawCube(to,Vector3.one);
		}
		
		void Update(){
			if(boxCollider == null){
				boxCollider = GetComponent<BoxCollider>();
				if(boxCollider == null)
					boxCollider = gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.isTrigger = true;
		}
#endif
	}
}
