using System;

// Token: 0x020002B4 RID: 692
public enum EventCaching : byte
{
	// Token: 0x04000EF0 RID: 3824
	DoNotCache,
	// Token: 0x04000EF1 RID: 3825
	[Obsolete]
	MergeCache,
	// Token: 0x04000EF2 RID: 3826
	[Obsolete]
	ReplaceCache,
	// Token: 0x04000EF3 RID: 3827
	[Obsolete]
	RemoveCache,
	// Token: 0x04000EF4 RID: 3828
	AddToRoomCache,
	// Token: 0x04000EF5 RID: 3829
	AddToRoomCacheGlobal,
	// Token: 0x04000EF6 RID: 3830
	RemoveFromRoomCache,
	// Token: 0x04000EF7 RID: 3831
	RemoveFromRoomCacheForActorsLeft,
	// Token: 0x04000EF8 RID: 3832
	SliceIncreaseIndex = 10,
	// Token: 0x04000EF9 RID: 3833
	SliceSetIndex,
	// Token: 0x04000EFA RID: 3834
	SlicePurgeIndex,
	// Token: 0x04000EFB RID: 3835
	SlicePurgeUpToIndex
}
