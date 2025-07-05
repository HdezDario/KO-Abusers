using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerMatchReady : MonoBehaviour
{
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // Player not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            
        }
    }
}
