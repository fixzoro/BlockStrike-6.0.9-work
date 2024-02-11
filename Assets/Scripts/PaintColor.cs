using System;
using UnityEngine;

public class PaintColor : MonoBehaviour
{
    public MeshFilter face;

    public int id;

    private void Start()
	{
		Color32 color = PaintManager.GetColor(this.id);
		Color32[] colors = new Color32[]
		{
			color,
			color,
			color,
			color
		};
		this.face.mesh.colors32 = colors;
	}
    
	private void OnPaint()
	{
		PaintManager.SetColor(this.id);
	}
}
