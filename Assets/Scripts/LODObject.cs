using System;
using UnityEngine;

[ExecuteInEditMode]
public class LODObject : MonoBehaviour
{
    public bool isEditor;

    public Renderer[] Originals;

    public Renderer[] LODs;

    private bool isLOD;

    private Transform mTransform;

    private int id;

    public nTimer Timer;

    public static bool isScope;

    private static float distance = 15f;

    public static Transform Target;

    private Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}
    
	private void Start()
	{
		if (LODObject.Target == null)
		{
			LODObject.Target = Camera.main.transform;
		}
		this.Timer.In(UnityEngine.Random.Range(0.1f, 0.15f), true, new TimerDelegate(this.CheckLOD));
	}
    
	private void CheckLOD()
	{
		if (LODObject.Target == null)
		{
			if (!(Camera.main != null))
			{
				return;
			}
			LODObject.Target = Camera.main.transform;
		}
		if (Vector3.Distance(this.cachedTransform.position, LODObject.Target.position) < ((!LODObject.isScope) ? LODObject.distance : 40f))
		{
			if (this.isLOD)
			{
				this.isLOD = false;
				for (int i = 0; i < this.Originals.Length; i++)
				{
					this.Originals[i].enabled = true;
				}
				for (int j = 0; j < this.LODs.Length; j++)
				{
					this.LODs[j].enabled = false;
				}
			}
		}
		else if (!this.isLOD)
		{
			this.isLOD = true;
			for (int k = 0; k < this.Originals.Length; k++)
			{
				this.Originals[k].enabled = false;
			}
			for (int l = 0; l < this.LODs.Length; l++)
			{
				this.LODs[l].enabled = true;
			}
		}
	}
    
	public static void SetDistance(float dis)
	{
		LODObject.distance = 10f + dis * 40f;
	}
}
