using UnityEngine;
using System.Collections;

namespace BSCM.Game.Modes.DeathRun{
	public class SetActive : MonoBehaviour {
		
		[Range(1,50)]
		public int key = 1;
		public GameObject target;
		public bool value;
		public float delayIn;
		public float delayOut = 3;
	}
}
