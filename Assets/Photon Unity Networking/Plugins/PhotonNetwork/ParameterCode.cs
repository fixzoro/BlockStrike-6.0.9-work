using System;

// Token: 0x020002AF RID: 687
public class ParameterCode
{
	// Token: 0x04000E8B RID: 3723
	public const byte SuppressRoomEvents = 237;

	// Token: 0x04000E8C RID: 3724
	public const byte EmptyRoomTTL = 236;

	// Token: 0x04000E8D RID: 3725
	public const byte PlayerTTL = 235;

	// Token: 0x04000E8E RID: 3726
	public const byte EventForward = 234;

	// Token: 0x04000E8F RID: 3727
	[Obsolete("Use: IsInactive")]
	public const byte IsComingBack = 233;

	// Token: 0x04000E90 RID: 3728
	public const byte IsInactive = 233;

	// Token: 0x04000E91 RID: 3729
	public const byte CheckUserOnJoin = 232;

	// Token: 0x04000E92 RID: 3730
	public const byte ExpectedValues = 231;

	// Token: 0x04000E93 RID: 3731
	public const byte Address = 230;

	// Token: 0x04000E94 RID: 3732
	public const byte PeerCount = 229;

	// Token: 0x04000E95 RID: 3733
	public const byte GameCount = 228;

	// Token: 0x04000E96 RID: 3734
	public const byte MasterPeerCount = 227;

	// Token: 0x04000E97 RID: 3735
	public const byte UserId = 225;

	// Token: 0x04000E98 RID: 3736
	public const byte ApplicationId = 224;

	// Token: 0x04000E99 RID: 3737
	public const byte Position = 223;

	// Token: 0x04000E9A RID: 3738
	public const byte MatchMakingType = 223;

	// Token: 0x04000E9B RID: 3739
	public const byte GameList = 222;

	// Token: 0x04000E9C RID: 3740
	public const byte Secret = 221;

	// Token: 0x04000E9D RID: 3741
	public const byte AppVersion = 220;

	// Token: 0x04000E9E RID: 3742
	[Obsolete("TCP routing was removed after becoming obsolete.")]
	public const byte AzureNodeInfo = 210;

	// Token: 0x04000E9F RID: 3743
	[Obsolete("TCP routing was removed after becoming obsolete.")]
	public const byte AzureLocalNodeId = 209;

	// Token: 0x04000EA0 RID: 3744
	[Obsolete("TCP routing was removed after becoming obsolete.")]
	public const byte AzureMasterNodeId = 208;

	// Token: 0x04000EA1 RID: 3745
	public const byte RoomName = 255;

	// Token: 0x04000EA2 RID: 3746
	public const byte Broadcast = 250;

	// Token: 0x04000EA3 RID: 3747
	public const byte ActorList = 252;

	// Token: 0x04000EA4 RID: 3748
	public const byte ActorNr = 254;

	// Token: 0x04000EA5 RID: 3749
	public const byte PlayerProperties = 249;

	// Token: 0x04000EA6 RID: 3750
	public const byte CustomEventContent = 245;

	// Token: 0x04000EA7 RID: 3751
	public const byte Data = 245;

	// Token: 0x04000EA8 RID: 3752
	public const byte Code = 244;

	// Token: 0x04000EA9 RID: 3753
	public const byte GameProperties = 248;

	// Token: 0x04000EAA RID: 3754
	public const byte Properties = 251;

	// Token: 0x04000EAB RID: 3755
	public const byte TargetActorNr = 253;

	// Token: 0x04000EAC RID: 3756
	public const byte ReceiverGroup = 246;

	// Token: 0x04000EAD RID: 3757
	public const byte Cache = 247;

	// Token: 0x04000EAE RID: 3758
	public const byte CleanupCacheOnLeave = 241;

	// Token: 0x04000EAF RID: 3759
	public const byte Group = 240;

	// Token: 0x04000EB0 RID: 3760
	public const byte Remove = 239;

	// Token: 0x04000EB1 RID: 3761
	public const byte PublishUserId = 239;

	// Token: 0x04000EB2 RID: 3762
	public const byte Add = 238;

	// Token: 0x04000EB3 RID: 3763
	public const byte Info = 218;

	// Token: 0x04000EB4 RID: 3764
	public const byte ClientAuthenticationType = 217;

	// Token: 0x04000EB5 RID: 3765
	public const byte ClientAuthenticationParams = 216;

	// Token: 0x04000EB6 RID: 3766
	public const byte JoinMode = 215;

	// Token: 0x04000EB7 RID: 3767
	public const byte ClientAuthenticationData = 214;

	// Token: 0x04000EB8 RID: 3768
	public const byte MasterClientId = 203;

	// Token: 0x04000EB9 RID: 3769
	public const byte FindFriendsRequestList = 1;

	// Token: 0x04000EBA RID: 3770
	public const byte FindFriendsResponseOnlineList = 1;

	// Token: 0x04000EBB RID: 3771
	public const byte FindFriendsResponseRoomIdList = 2;

	// Token: 0x04000EBC RID: 3772
	public const byte LobbyName = 213;

	// Token: 0x04000EBD RID: 3773
	public const byte LobbyType = 212;

	// Token: 0x04000EBE RID: 3774
	public const byte LobbyStats = 211;

	// Token: 0x04000EBF RID: 3775
	public const byte Region = 210;

	// Token: 0x04000EC0 RID: 3776
	public const byte UriPath = 209;

	// Token: 0x04000EC1 RID: 3777
	public const byte WebRpcParameters = 208;

	// Token: 0x04000EC2 RID: 3778
	public const byte WebRpcReturnCode = 207;

	// Token: 0x04000EC3 RID: 3779
	public const byte WebRpcReturnMessage = 206;

	// Token: 0x04000EC4 RID: 3780
	public const byte CacheSliceIndex = 205;

	// Token: 0x04000EC5 RID: 3781
	public const byte Plugins = 204;

	// Token: 0x04000EC6 RID: 3782
	public const byte NickName = 202;

	// Token: 0x04000EC7 RID: 3783
	public const byte PluginName = 201;

	// Token: 0x04000EC8 RID: 3784
	public const byte PluginVersion = 200;

	// Token: 0x04000EC9 RID: 3785
	public const byte ExpectedProtocol = 195;

	// Token: 0x04000ECA RID: 3786
	public const byte CustomInitData = 194;

	// Token: 0x04000ECB RID: 3787
	public const byte EncryptionMode = 193;

	// Token: 0x04000ECC RID: 3788
	public const byte EncryptionData = 192;

	// Token: 0x04000ECD RID: 3789
	public const byte RoomOptionFlags = 191;
}
