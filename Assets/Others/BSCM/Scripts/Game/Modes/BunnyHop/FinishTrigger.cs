using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSCM.Game.Modes.BunnyHop{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class FinishTrigger : MonoBehaviour
    {
        private void Start()
        {
            gameObject.AddComponent<BunnySpawn>().FinishSpawn = true;
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
