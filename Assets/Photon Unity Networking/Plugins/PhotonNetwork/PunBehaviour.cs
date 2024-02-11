using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Photon
{
	// Token: 0x020002C9 RID: 713
	public class PunBehaviour : MonoBehaviour, IPunCallbacks
	{
		// Token: 0x06001AD9 RID: 6873 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnConnectedToPhoton()
		{
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnLeftRoom()
		{
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnMasterClientSwitched(PhotonPlayer newMasterClient)
		{
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonCreateRoomFailed(object[] codeAndMsg)
		{
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonJoinRoomFailed(object[] codeAndMsg)
		{
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnCreatedRoom()
		{
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnJoinedLobby()
		{
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnLeftLobby()
		{
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnDisconnectedFromPhoton()
		{
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnConnectionFail(DisconnectCause cause)
		{
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
		{
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnReceivedRoomListUpdate()
		{
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnJoinedRoom()
		{
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
		{
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonRandomJoinFailed(object[] codeAndMsg)
		{
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnConnectedToMaster()
		{
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonMaxCccuReached()
		{
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
		{
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnUpdatedFriendList()
		{
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnWebRpcResponse(OperationResponse response)
		{
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnOwnershipRequest(object[] viewAndPlayer)
		{
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnLobbyStatisticsUpdate()
		{
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnPhotonPlayerActivityChanged(PhotonPlayer otherPlayer)
		{
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x0000574F File Offset: 0x0000394F
		public virtual void OnOwnershipTransfered(object[] viewAndPlayers)
		{
		}
	}
}
