using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private Transform mTransform;

    #if UNITY_EDITOR
    public Team team;
    #endif

    public Transform cachedTransform
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

	public Vector3 spawnPosition
	{
		get
		{
			Vector3 position = this.cachedTransform.position;
			position.x += UnityEngine.Random.Range(-this.spawnScale.x / 2f, this.spawnScale.x / 2f);
			position.z += UnityEngine.Random.Range(-this.spawnScale.z / 2f, this.spawnScale.z / 2f);
			return position;
		}
		set
		{
			this.cachedTransform.position = value;
		}
	}

	public Vector3 spawnRotation
	{
		get
		{
			return this.cachedTransform.eulerAngles;
		}
		set
		{
			this.cachedTransform.eulerAngles = value;
		}
	}

	public Vector3 spawnScale
	{
		get
		{
			return this.cachedTransform.localScale;
		}
		set
		{
			this.cachedTransform.localScale = value;
		}
	}

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(team == Team.Red)
        {
            Gizmos.color = Color.red - new Color(0f, 0f, 0f, 0.2f);
        }
        if(team == Team.Blue)
        {
            Gizmos.color = Color.blue - new Color(0f, 0f, 0f, 0.2f);
        }
        if (team == Team.None)
        {
            Gizmos.color = Color.green - new Color(0f, 0f, 0f, 0.3f);
        }
        Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 5f);
        Gizmos.DrawCube(base.transform.position, spawnScale);
    }
    #endif
}
