using UnityEngine;
using System.Collections;
using BSCM.Game;
namespace BSCM.Game.Modes.DeathRun{
	public class Button : MonoBehaviour {

		public Team team = Team.Red;
		[Range(1,50)]
		public int key;
	}
}
