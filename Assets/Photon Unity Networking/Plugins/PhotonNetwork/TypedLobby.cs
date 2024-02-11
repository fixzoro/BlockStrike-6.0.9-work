using System;

// Token: 0x020002B9 RID: 697
public class TypedLobby
{
	// Token: 0x06001A14 RID: 6676 RVA: 0x000130A9 File Offset: 0x000112A9
	public TypedLobby()
	{
		this.Name = string.Empty;
		this.Type = LobbyType.Default;
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x000130C3 File Offset: 0x000112C3
	public TypedLobby(string name, LobbyType type)
	{
		this.Name = name;
		this.Type = type;
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06001A17 RID: 6679 RVA: 0x000130E5 File Offset: 0x000112E5
	public bool IsDefault
	{
		get
		{
			return this.Type == LobbyType.Default && string.IsNullOrEmpty(this.Name);
		}
	}

	// Token: 0x06001A18 RID: 6680 RVA: 0x00013100 File Offset: 0x00011300
	public override string ToString()
	{
		return string.Format("lobby '{0}'[{1}]", this.Name, this.Type);
	}

	// Token: 0x04000F19 RID: 3865
	public string Name;

	// Token: 0x04000F1A RID: 3866
	public LobbyType Type;

	// Token: 0x04000F1B RID: 3867
	public static readonly TypedLobby Default = new TypedLobby();
}
