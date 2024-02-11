using System;
using System.Collections.Generic;
using System.IO;
using FreeJSON;
using UnityEngine;

public class AnimRecord : MonoBehaviour
{
    public bool isRecord;

    public List<AnimRecord.Point> points = new List<AnimRecord.Point>();

    private int recordClip;

    public int maxClip;

    public bool play;

    public int t;

    [ContextMenu("Save")]
	private void Save()
	{
		JsonObject jsonObject = new JsonObject();
		for (int i = 0; i < this.points.Count; i++)
		{
			JsonObject jsonObject2 = new JsonObject();
			if (this.points[i].pos)
			{
				jsonObject2.Add("pos", this.points[i].posList);
			}
			if (this.points[i].rot)
			{
				jsonObject2.Add("rot", this.points[i].rotList);
			}
			jsonObject.Add(this.points[i].target.name, jsonObject2);
		}
		File.WriteAllText(Path.GetDirectoryName(Application.dataPath) + "/Record.txt", jsonObject.ToString());
	}

	[ContextMenu("Snapshot")]
	private void Snapshot()
	{
		JsonObject jsonObject = new JsonObject();
		for (int i = 0; i < this.points.Count; i++)
		{
			JsonObject jsonObject2 = new JsonObject();
			jsonObject2.Add("pos", this.points[i].target.localPosition);
			jsonObject2.Add("rot", this.points[i].target.localEulerAngles);
			jsonObject.Add(this.points[i].target.name, jsonObject2);
		}
		File.WriteAllText(Path.GetDirectoryName(Application.dataPath) + "/Record.txt", jsonObject.ToString());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.isRecord = !this.isRecord;
			if (this.isRecord)
			{
			}
		}
		if (this.isRecord)
		{
			this.recordClip++;
			if (this.recordClip == this.maxClip)
			{
				this.isRecord = false;
			}
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.points[i].pos)
				{
					this.points[i].posList.Add(this.points[i].target.localPosition);
				}
				if (this.points[i].rot)
				{
					this.points[i].rotList.Add(this.points[i].target.localEulerAngles);
				}
			}
		}
		if (this.play)
		{
			this.t++;
			if (this.t >= this.recordClip)
			{
				this.t = 0;
			}
			for (int j = 0; j < this.points.Count; j++)
			{
				if (this.points[j].pos)
				{
					this.points[j].target.localPosition = this.points[j].posList[this.t];
				}
				if (this.points[j].rot)
				{
					this.points[j].target.localEulerAngles = this.points[j].rotList[this.t];
				}
			}
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(this.NewRect(5f, 5f, 40f, 10f), "Next"))
		{
			this.t++;
			if (this.t >= this.recordClip)
			{
				this.t = 0;
			}
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.points[i].pos)
				{
					this.points[i].target.localPosition = this.points[i].posList[this.t];
				}
				if (this.points[i].rot)
				{
					this.points[i].target.localEulerAngles = this.points[i].rotList[this.t];
				}
			}
		}
	}

	private Rect NewRect(float x, float y, float width, float height)
	{
		x = (float)Screen.width * x / 100f;
		y = (float)Screen.height * y / 100f;
		width = (float)Screen.width * width / 100f;
		height = (float)Screen.height * height / 100f;
		Rect result = new Rect(x, y, width, height);
		return result;
	}

	[Serializable]
	public class Point
	{
		public Transform target;

		public bool pos;

		public bool rot;

		public List<Vector3> posList = new List<Vector3>();

		public List<Vector3> rotList = new List<Vector3>();
	}
}
