using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmoBoxCollider : MonoBehaviour {

	public bool isSelected = false;
	public bool isWire = false;
	public Color color = new Color(1,0,0,0.5f);
	public string iconName;
	public bool iconAllowScaling = false;
	private BoxCollider boxCollider;

	void OnDrawGizmosSelected(){
		if(isSelected)
			OnShow();
	}

	void OnDrawGizmos(){
		if(!isSelected)
			OnShow();
	}

	void OnShow(){
		if(boxCollider == null){
			boxCollider = GetComponent<BoxCollider>();
			if(boxCollider == null)
				boxCollider = gameObject.AddComponent<BoxCollider>();
		}

		Gizmos.color = color;
		if(isWire){
			Gizmos.DrawWireCube(boxCollider.center + transform.localPosition,boxCollider.size);
		}else{
			Gizmos.DrawCube(boxCollider.center + transform.localPosition,boxCollider.size);
		}
		if(!string.IsNullOrEmpty(iconName)){
			Gizmos.DrawIcon(boxCollider.center + transform.localPosition,iconName,iconAllowScaling);
		}
	}
}
