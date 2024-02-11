using UnityEngine;
using System.Collections.Generic;

namespace BSCM{

	public class DefaultMaterials : ScriptableObject {

		public List<Material> materials;

		private static DefaultMaterials Instance;
		public static DefaultMaterials instance{
			get{
				if(Instance == null)
					Instance = Resources.Load("DefaultMaterials") as DefaultMaterials;
				return Instance;
			}
		}

		public static int Contains(Material material){
			for(int i=0;i<instance.materials.Count;i++){
				if(instance.materials[i] == material){
					return i;
				}
			}
			return -1;
		}
	}
}
