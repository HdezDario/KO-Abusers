using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        if (LobbyManager.Instance != null)
            Destroy(LobbyManager.Instance.gameObject);

        if (MultiplayerMatchData.Instance != null)
            Destroy(MultiplayerMatchData.Instance.gameObject);
    }
}
