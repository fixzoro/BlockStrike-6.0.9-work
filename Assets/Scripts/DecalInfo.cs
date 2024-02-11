using System;
using System.Collections.Generic;
using UnityEngine;

public class DecalInfo
{
    private static PhotonDataWrite writeData = new PhotonDataWrite();

    private static PhotonDataRead readData = new PhotonDataRead();

    public byte BloodDecal = 200;

    public bool isKnife;

    public BetterList<Vector3> Points = new BetterList<Vector3>();

    public BetterList<Vector3> Normals = new BetterList<Vector3>();

    private static List<DecalInfo> PoolList = new List<DecalInfo>();

    public static DecalInfo Get()
	{
		nProfiler.BeginSample("DecalInfo.Get");
		if (DecalInfo.PoolList.Count != 0)
		{
			DecalInfo result = DecalInfo.PoolList[0];
			DecalInfo.PoolList.RemoveAt(0);
			nProfiler.EndSample();
			return result;
		}
		nProfiler.EndSample();
		return new DecalInfo();
	}

	public void Dispose()
	{
		this.BloodDecal = 200;
		this.isKnife = false;
		this.Points.Clear();
		this.Normals.Clear();
		DecalInfo.PoolList.Add(this);
	}

	public byte[] ToArray()
	{
		if (this.BloodDecal == 200 && this.Points.size == 0)
		{
			return new byte[]
			{
				199
			};
		}
		DecalInfo.writeData.Clear();
		DecalInfo.writeData.Write(this.BloodDecal);
		DecalInfo.writeData.Write(this.isKnife);
		DecalInfo.writeData.Write((byte)this.Points.size);
		for (int i = 0; i < this.Points.size; i++)
		{
			DecalInfo.writeData.Write(this.Points[i]);
		}
		DecalInfo.writeData.Write((byte)this.Normals.size);
		for (int j = 0; j < this.Normals.size; j++)
		{
			DecalInfo.writeData.Write(this.Normals[j]);
		}
		return DecalInfo.writeData.ToArray();
	}

	public static DecalInfo SetData(byte[] bytes)
	{
		nProfiler.BeginSample("DecalInfo.SetData");
		DecalInfo decalInfo = DecalInfo.Get();
		if (bytes.Length > 1)
		{
			DecalInfo.readData.Clear();
			DecalInfo.readData.bytes = bytes;
			decalInfo.BloodDecal = DecalInfo.readData.ReadByte();
			decalInfo.isKnife = DecalInfo.readData.ReadBool();
			byte b = DecalInfo.readData.ReadByte();
			for (int i = 0; i < (int)b; i++)
			{
				decalInfo.Points.Add(DecalInfo.readData.ReadVector3());
			}
			byte b2 = DecalInfo.readData.ReadByte();
			for (int j = 0; j < (int)b2; j++)
			{
				decalInfo.Normals.Add(DecalInfo.readData.ReadVector3());
			}
		}
		nProfiler.EndSample();
		return decalInfo;
	}
}
