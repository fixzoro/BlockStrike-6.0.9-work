using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSCM.Game.Others{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class Climb : MonoBehaviour
    {
        private void Start()
        {
            base.gameObject.AddComponent<ClimbSystem>();
        }
#if UNITY_EDITOR
        private BoxCollider boxCollider;
		
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
