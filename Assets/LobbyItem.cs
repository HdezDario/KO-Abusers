using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyText;
    [SerializeField] private Button lobbyButton;

    public void PopulateLobbyInfo(string lobbyName, string lobbyId)
    {
        lobbyText.text = lobbyName;

        lobbyButton.onClick.AddListener(() =>
        {
            lobbyButton.interactable = false;
            LobbyManager.Instance.JoinLobbyByLobbyId(lobbyId);
        });
    }
}
