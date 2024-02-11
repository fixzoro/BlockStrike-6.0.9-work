using System;
using UnityEngine;

public class GlassObject : MonoBehaviour
{
    public GameObject Glass;

    public GameObject BrokenGlass;

    public Transform[] Elements;

    private float[] Speed;

    public Vector2 MinMaxSpeed = new Vector2(30f, 40f);

    private float StartTime;

    private bool isActive;

    [ContextMenu("Active")]
	public void Active()
	{
		this.Glass.SetActive(false);
		this.BrokenGlass.SetActive(true);
		this.Speed = new float[this.Elements.Length];
		for (int i = 0; i < this.Elements.Length; i++)
		{
			this.Speed[i] = UnityEngine.Random.Range(this.MinMaxSpeed.x, this.MinMaxSpeed.y);
		}
		this.StartTime = Time.time;
		this.isActive = true;
	}

	private void Update()
	{
		if (this.isActive)
		{
			for (int i = 0; i < this.Elements.Length; i++)
			{
				this.Elements[i].localPosition -= new Vector3(0f, this.Speed[i] * Time.deltaTime, 0f);
			}
			if (this.StartTime + 1f < Time.time)
			{
				this.isActive = false;
				this.BrokenGlass.SetActive(false);
				for (int j = 0; j < this.Elements.Length; j++)
				{
					this.Elements[j].localPosition = Vector3.zero;
				}
			}
		}
	}
}
