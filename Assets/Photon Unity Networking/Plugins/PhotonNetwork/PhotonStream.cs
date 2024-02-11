using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004EA RID: 1258
public class PhotonStream
{
	// Token: 0x06002AD0 RID: 10960 RVA: 0x0001DB31 File Offset: 0x0001BD31
	public PhotonStream(bool write)
	{
		this.write = write;
		if (write)
		{
			this.writeData = new PhotonDataWrite(32);
		}
		else
		{
			this.readData = new PhotonDataRead();
		}
        this.data2 = new List<object>();
    }

    bool write = false;
    internal List<object> data2;
    byte currentItem2 = 0; //Used to track the next item to receive.

    /// <summary>Add another piece of data to send it when isWriting is true.</summary>
    public void SendNext(object obj)
    {
        if (!this.write)
        {
            Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
            return;
        }

        this.data2.Add(obj);
    }

    /// <summary>Read next piece of data from the stream when isReading is true.</summary>
    public object ReceiveNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }

        object obj = this.data2[this.currentItem2];
        this.currentItem2++;
        return obj;
    }



    // Token: 0x1700047D RID: 1149
    // (get) Token: 0x06002AD1 RID: 10961 RVA: 0x0001DB63 File Offset: 0x0001BD63
    public bool isWriting
	{
		get
		{
			return this.write;
		}
	}

	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x06002AD2 RID: 10962 RVA: 0x0001DB6B File Offset: 0x0001BD6B
	public bool isReading
	{
		get
		{
			return !this.write;
		}
	}

	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002AD3 RID: 10963 RVA: 0x0001DB76 File Offset: 0x0001BD76
	public int Count
	{
		get
		{
			return (!this.isWriting) ? this.readData.bytes.Length : this.writeData.bytes.size;
		}
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x0001DBA5 File Offset: 0x0001BDA5
	public byte[] ToArray()
	{
		return this.writeData.ToArray();
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x0001DBB2 File Offset: 0x0001BDB2
	public void SetData(byte[] bytes)
	{
		this.readData.Clear();
		this.readData.bytes = bytes;
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x0001DBCB File Offset: 0x0001BDCB
	public void Clear()
	{
		if (this.isWriting)
		{
			this.writeData.Clear();
		}
		else
		{
			this.readData.Clear();
		}
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x0001DBF3 File Offset: 0x0001BDF3
	public void Write(byte value)
	{
		this.writeData.Write((byte)value);
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x0001DC01 File Offset: 0x0001BE01
	public void Write(byte[] value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x0001DC0F File Offset: 0x0001BE0F
	public void Write(bool value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x0001DC1D File Offset: 0x0001BE1D
	public void Write(int value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADB RID: 10971 RVA: 0x0001DC2B File Offset: 0x0001BE2B
	public void Write(int[] value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADC RID: 10972 RVA: 0x0001DC39 File Offset: 0x0001BE39
	public void Write(float value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADD RID: 10973 RVA: 0x0001DC47 File Offset: 0x0001BE47
	public void Write(short value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADE RID: 10974 RVA: 0x0001DC55 File Offset: 0x0001BE55
	public void Write(string value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002ADF RID: 10975 RVA: 0x0001DC63 File Offset: 0x0001BE63
	public void Write(Vector3 value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x0001DC71 File Offset: 0x0001BE71
	public void Write(Vector2 value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002AE1 RID: 10977 RVA: 0x0001DC7F File Offset: 0x0001BE7F
	public void Write(Quaternion value)
	{
		this.writeData.Write(value);
	}

	// Token: 0x06002AE2 RID: 10978 RVA: 0x0001DC8D File Offset: 0x0001BE8D
	public byte ReadByte()
	{
		return (byte)this.readData.ReadByte();
	}

	// Token: 0x06002AE3 RID: 10979 RVA: 0x0001DC9A File Offset: 0x0001BE9A
	public bool ReadBool()
	{
		return this.readData.ReadBool();
	}

	// Token: 0x06002AE4 RID: 10980 RVA: 0x0001DCA7 File Offset: 0x0001BEA7
	public byte[] ReadBytes()
	{
		return this.readData.ReadBytes();
	}

	// Token: 0x06002AE5 RID: 10981 RVA: 0x0001DCB4 File Offset: 0x0001BEB4
	public int ReadInt()
	{
		return this.readData.ReadInt();
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x0001DCC1 File Offset: 0x0001BEC1
	public short ReadShort()
	{
		return this.readData.ReadShort();
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x0001DCCE File Offset: 0x0001BECE
	public float ReadFloat()
	{
		return this.readData.ReadFloat();
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x0001DCDB File Offset: 0x0001BEDB
	public Vector3 ReadVector3()
	{
		return this.readData.ReadVector3();
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x0001DCE8 File Offset: 0x0001BEE8
	public Quaternion ReadQuaternion()
	{
		return this.readData.ReadQuaternion();
	}

	// Token: 0x04001B94 RID: 7060
	public PhotonDataWrite writeData;

	// Token: 0x04001B95 RID: 7061
	public PhotonDataRead readData;

	// Token: 0x04001B96 RID: 7062
	private bool write2;
}