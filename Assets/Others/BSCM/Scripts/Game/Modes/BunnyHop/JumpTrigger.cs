using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSCM.Game.Modes.BunnyHop{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class JumpTrigger : MonoBehaviour
    {
        private void Start()
        {
            gameObject.AddComponent<BunnyAutoJump>();
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
