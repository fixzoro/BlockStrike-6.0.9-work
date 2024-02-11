using UnityEngine;
using System.Collections;

namespace BSCM.Game.Others{
	public class AnimateMaterial : MonoBehaviour {

		public Material material;
		public Vector2 speed;
		private bool visible;
#if UNITY_EDITOR
		private float time;

		[ContextMenu("Run")]
		void OnRun(){
			UnityEditor.EditorApplication.update += Update;
		}

		[ContextMenu("Stop")]
		void OnStop(){
			UnityEditor.EditorApplication.update -= Update;
			material.mainTextureOffset = Vector2.zero;
		}

		void Update(){
			if(material != null){
				time += 0.016f;
				material.mainTextureOffset = speed * time;
			}
		}
#endif
	}
}
