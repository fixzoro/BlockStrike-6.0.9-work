using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public GameObject Block;

    public int SelectColor;

    public bool BlockDelete;

    public Color32[] Colors;

    public List<PaintObject> BlockList = new List<PaintObject>();

    private int LastID;

    private static PaintManager instance;

    private void Awake()
	{
		PaintManager.instance = this;
	}

	private void Start()
	{
		EventManager.AddListener<RaycastHit>("Paint", new EventManager.Callback<RaycastHit>(this.OnPaint));
	}

	[ContextMenu("Get Data")]
	private void GetData()
	{
		byte[] array = new byte[this.BlockList.Count * 4];
		int num = 0;
		for (int i = 0; i < this.BlockList.Count; i++)
		{
			byte[] data = this.BlockList[i].GetData();
			array[num] = data[0];
			array[num + 1] = data[1];
			array[num + 2] = data[2];
			array[num + 3] = data[3];
			num += 4;
		}
	}

	private void OnPaint(RaycastHit hit)
	{
		if (hit.transform.name == "Color")
		{
			return;
		}
		if (this.BlockDelete)
		{
			if (hit.transform.name != "Plane")
			{
				int id = hit.transform.parent.GetComponent<PaintObject>().ID;
				for (int i = 0; i < this.BlockList.Count; i++)
				{
					if (this.BlockList[i].ID == id)
					{
						this.BlockList[i].Delete();
						this.BlockList.RemoveAt(i);
						break;
					}
				}
			}
		}
		else if (hit.distance > 1.5f)
		{
			if (hit.transform.name == "Plane")
			{
				Vector3 zero = Vector3.zero;
				zero.x = Mathf.Round(hit.point.x);
				zero.y = 0f;
				zero.z = Mathf.Round(hit.point.z);
				PaintManager.Paint(zero);
			}
			else
			{
				PaintManager.Paint(hit.transform.parent.position + hit.normal);
			}
		}
	}

	public static void Paint(Vector3 pos)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(PaintManager.instance.Block, pos, Quaternion.identity);
		gameObject.name = PaintManager.instance.LastID.ToString();
		gameObject.transform.SetParent(PaintManager.instance.transform);
		PaintObject component = gameObject.GetComponent<PaintObject>();
		component.Init(PaintManager.instance.LastID, PaintManager.instance.SelectColor);
		PaintManager.instance.BlockList.Add(component);
		PaintManager.instance.LastID++;
	}

	public static void Paint(int x, int y, int z, int color)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(PaintManager.instance.Block, Vector3.zero, Quaternion.identity);
		gameObject.name = PaintManager.instance.LastID.ToString();
		gameObject.transform.SetParent(PaintManager.instance.transform);
		gameObject.transform.localPosition = new Vector3((float)x, (float)y, (float)z);
		PaintObject component = gameObject.GetComponent<PaintObject>();
		component.Init(PaintManager.instance.LastID, color);
		PaintManager.instance.BlockList.Add(component);
		PaintManager.instance.LastID++;
	}

	public static void SetColor(int color)
	{
		PaintManager.instance.SelectColor = color;
	}

	public static Color32 GetColor(int id)
	{
		return PaintManager.instance.Colors[id];
	}

	public static void Clear()
	{
		for (int i = 0; i < PaintManager.instance.BlockList.Count; i++)
		{
			PaintManager.instance.BlockList[i].Delete(false);
		}
		PaintManager.instance.BlockList.Clear();
	}

	private IEnumerator LoadData(byte[] bytes)
	{
		PaintManager.Clear();
		PlayerInput.instance.SetMove(false);
		yield return new WaitForSeconds(0.2f);
		int length = bytes.Length / 4;
		int count = 0;
		for (int i = 0; i < length; i++)
		{
			PaintManager.Paint((int)bytes[count], (int)bytes[count + 1], (int)bytes[count + 2], (int)bytes[count + 3]);
			count += 4;
			yield return new WaitForEndOfFrame();
		}
		PlayerInput.instance.SetMove(true);
		yield break;
	}
}
