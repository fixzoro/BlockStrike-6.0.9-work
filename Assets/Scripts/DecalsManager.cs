using System;
using UnityEngine;

public class DecalsManager : MonoBehaviour
{
    public float FrontOfWall = 0.02f;

    public MeshFilter bulletMeshFilter;

    public int bulletMaxCount = 50;

    private int bulletCount;

    private Mesh bulletMesh;

    public Transform BloodEffect;

    private int BloodEffectTimerID;

    private static DecalsManager instance;

    private Vector3[] vertices = new Vector3[50];

    private int[] triangles = new int[0];

    private Vector3[] normals;

    private Vector2[] uv;

    private static Vector3 quadNormal = new Vector3(0f, 0f, -1f);

    private static Vector3[] quadVertices = new Vector3[]
    {
        new Vector3(-0.05f, -0.05f, 0f),
        new Vector3(0.05f, 0.05f, 0f),
        new Vector3(0.05f, -0.05f, 0f),
        new Vector3(-0.05f, 0.05f, 0f)
    };

    private void Awake()
	{
		DecalsManager.instance = this;
	}

	private void Start()
	{
		this.CreateBulletMesh();
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.UpdateSettings));
		this.UpdateSettings();
	}

	private void CreateBulletMesh()
	{
		this.bulletMesh = new Mesh();
		this.bulletMeshFilter.sharedMesh = this.bulletMesh;
		this.vertices = new Vector3[this.bulletMaxCount * 4];
		this.triangles = new int[this.bulletMaxCount * 6];
		this.normals = new Vector3[this.bulletMaxCount * 4];
		this.uv = new Vector2[this.bulletMaxCount * 4];
		Vector2[] array = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f)
		};
		for (int i = 0; i < this.bulletMaxCount; i++)
		{
			this.vertices[i * 4] = DecalsManager.quadVertices[0] + Vector3.left * 1000f;
			this.vertices[i * 4 + 1] = DecalsManager.quadVertices[1] + Vector3.left * 1000f;
			this.vertices[i * 4 + 2] = DecalsManager.quadVertices[2] + Vector3.left * 1000f;
			this.vertices[i * 4 + 3] = DecalsManager.quadVertices[3] + Vector3.left * 1000f;
			this.triangles[i * 6] = 0 + i * 4;
			this.triangles[i * 6 + 1] = 1 + i * 4;
			this.triangles[i * 6 + 2] = 2 + i * 4;
			this.triangles[i * 6 + 3] = 1 + i * 4;
			this.triangles[i * 6 + 4] = 0 + i * 4;
			this.triangles[i * 6 + 5] = 3 + i * 4;
			this.normals[i * 4] = DecalsManager.quadNormal;
			this.normals[i * 4 + 1] = DecalsManager.quadNormal;
			this.normals[i * 4 + 2] = DecalsManager.quadNormal;
			this.normals[i * 4 + 3] = DecalsManager.quadNormal;
			this.uv[i * 4] = array[0];
			this.uv[i * 4 + 1] = array[1];
			this.uv[i * 4 + 2] = array[2];
			this.uv[i * 4 + 3] = array[3];
		}
		this.bulletMesh.vertices = this.vertices;
		this.bulletMesh.triangles = this.triangles;
		this.bulletMesh.normals = this.normals;
		this.bulletMesh.uv = this.uv;
		this.bulletMesh.RecalculateBounds();
	}

	public static void FireWeapon(DecalInfo decalInfo)
	{
		nProfiler.BeginSample("DecalsManager.FireWeapon");
		for (int i = 0; i < decalInfo.Points.size; i++)
		{
			if ((int)decalInfo.BloodDecal == i)
			{
				if (Settings.Blood)
				{
					DecalsManager.instance.CreateBloodEffect(decalInfo.Points[i]);
				}
			}
			else if (Settings.BulletHole && !decalInfo.isKnife)
			{
				DecalsManager.instance.CreateBulletHole(decalInfo.Points[i], decalInfo.Normals[i]);
			}
		}
		decalInfo.Dispose();
		nProfiler.EndSample();
	}

	public void CreateBulletHole(Vector3 point, Vector3 normal)
	{
		nProfiler.BeginSample("DecalsManager.CreateBulletHole");
		if (point == Vector3.zero && normal == Vector3.zero)
		{
			return;
		}
		Vector3 b = point + normal * DecalsManager.instance.FrontOfWall;
		Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.back, normal).eulerAngles;
		Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, UnityEngine.Random.value * 360f);
		this.vertices[this.bulletCount * 4] = DecalsManager.X(rotation, DecalsManager.quadVertices[0]) + b;
		this.vertices[this.bulletCount * 4 + 1] = DecalsManager.X(rotation, DecalsManager.quadVertices[1]) + b;
		this.vertices[this.bulletCount * 4 + 2] = DecalsManager.X(rotation, DecalsManager.quadVertices[2]) + b;
		this.vertices[this.bulletCount * 4 + 3] = DecalsManager.X(rotation, DecalsManager.quadVertices[3]) + b;
		this.bulletMesh.vertices = this.vertices;
		this.bulletMesh.RecalculateBounds();
		this.bulletCount++;
		if (this.bulletCount >= this.bulletMaxCount)
		{
			this.bulletCount = 0;
		}
		nProfiler.EndSample();
	}

	public static void ClearBulletHoles()
	{
		for (int i = 0; i < DecalsManager.instance.bulletMaxCount; i++)
		{
			DecalsManager.instance.vertices[i * 4] = DecalsManager.quadVertices[0] + Vector3.left * 1000f;
			DecalsManager.instance.vertices[i * 4 + 1] = DecalsManager.quadVertices[1] + Vector3.left * 1000f;
			DecalsManager.instance.vertices[i * 4 + 2] = DecalsManager.quadVertices[2] + Vector3.left * 1000f;
			DecalsManager.instance.vertices[i * 4 + 3] = DecalsManager.quadVertices[3] + Vector3.left * 1000f;
		}
		DecalsManager.instance.bulletCount = 0;
	}

	public void CreateBloodEffect(Vector3 pos)
	{
		nProfiler.BeginSample("DecalsManager.CreateBloodEffect");
		Transform activeCamera = CameraManager.ActiveCamera;
		if (activeCamera == null)
		{
			return;
		}
		if (TimerManager.IsActive(this.BloodEffectTimerID))
		{
			TimerManager.Cancel(this.BloodEffectTimerID);
		}
		this.BloodEffect.position = pos;
		this.BloodEffect.LookAt(activeCamera.position);
		Vector3 eulerAngles = this.BloodEffect.eulerAngles;
		this.BloodEffect.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, UnityEngine.Random.value * 360f);
		this.BloodEffectTimerID = TimerManager.In(0.05f, delegate()
		{
			this.BloodEffect.position = Vector3.right * 5000f;
		});
		nProfiler.EndSample();
	}

	private void UpdateSettings()
	{
		if (!Settings.BulletHole)
		{
			DecalsManager.ClearBulletHoles();
		}
	}

	private static Vector3 X(Quaternion rotation, Vector3 point)
	{
		float num = rotation.x * 2f;
		float num2 = rotation.y * 2f;
		float num3 = rotation.z * 2f;
		float num4 = rotation.x * num;
		float num5 = rotation.y * num2;
		float num6 = rotation.z * num3;
		float num7 = rotation.x * num2;
		float num8 = rotation.x * num3;
		float num9 = rotation.y * num3;
		float num10 = rotation.w * num;
		float num11 = rotation.w * num2;
		float num12 = rotation.w * num3;
		Vector3 result;
		result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
		result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
		result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
		return result;
	}
}
