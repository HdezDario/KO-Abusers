using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private TextMeshProUGUI hostPlayerText;
    [SerializeField] private TextMeshProUGUI clientPlayerText;

    [Header("Buttons")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button kickButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [Header("Lobby Prefab")]
    [SerializeField] private GameObject lobbyHolder;
    [SerializeField] private GameObject lobbyPrefab;

    [Header("UI Menus")]
    [SerializeField] private GameObject listUI;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject kickObject;
    [SerializeField] private GameObject startObject;

    private void Start()
    {
        hostButton.onClick.AddListener( () =>
        {
            hostButton.interactable = false;
            LobbyManager.Instance.CreateLobby();
        });

        refreshButton.onClick.AddListener(() =>
        {
            RefreshList();
        });

        leaveButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost)
                LobbyManager.Instance.DeleteLobby();
            else
                LobbyManager.Instance.LeaveLobby();
        });

        kickButton.onClick.AddListener(() =>
        {
            kickButton.interactable = false;
            startButton.interactable = false;
            LobbyManager.Instance.KickPlayer();
        });

        startButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.InitializeClientDataClientRpc();

            Debug.Log("Starting Match");

            NetworkManager.Singleton.SceneManager.LoadScene(Scenes.MultiplayerTeamSelect.ToString(), LoadSceneMode.Single);
        });
        
        exitButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.Disconnect();
            SceneManager.LoadScene(Scenes.Menu.ToString());
        });

        LobbyManager.Instance.OnLobbyJoined.AddListener(OpenLobbyUI);
        LobbyManager.Instance.OnLobbyExited.AddListener(OpenListUI);
        LobbyManager.Instance.OnLobbyKicked.AddListener(OpenListUI);
        LobbyManager.Instance.OnClientJoined.AddListener(ClientJoined);
        LobbyManager.Instance.OnClientLeave.AddListener(ClientLeft);
        LobbyManager.Instance.OnClientReady.AddListener(ClientReady);

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        

        RefreshList();
    }

    private void OnDisable()
    {
        LobbyManager.Instance.OnLobbyJoined.RemoveAllListeners();
        LobbyManager.Instance.OnLobbyExited.RemoveAllListeners();
        LobbyManager.Instance.OnLobbyKicked.RemoveAllListeners();
        LobbyManager.Instance.OnClientJoined.RemoveAllListeners();
        LobbyManager.Instance.OnClientLeave.RemoveAllListeners();
        LobbyManager.Instance.OnClientReady.RemoveAllListeners();
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log(LobbyManager.Instance.joinedLobby.Players.Count);

        hostPlayerText.text = LobbyManager.Instance.joinedLobby.Players[0].Data["PlayerName"].Value;

        if (LobbyManager.Instance.joinedLobby.Players.Count > 1)
        {
            clientPlayerText.text = LobbyManager.Instance.joinedLobby.Players[1].Data["PlayerName"].Value;
            LobbyManager.Instance.ClientJoinedSetNameServerRpc(LobbyManager.Instance.joinedLobby.Players[1].Data["PlayerName"].Value);
        }

        Debug.Log("Players Loaded");
    }

    private async void RefreshList()
    {
        if (lobbyHolder.transform.childCount > 0)
            for (int i = lobbyHolder.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(lobbyHolder.transform.GetChild(i).gameObject);
            }

        try
        {
            QueryResponse query = await LobbyManager.Instance.ListAvailableLobbies();

            foreach (Lobby lobby in query.Results)
            {
                GameObject newLobby = Instantiate(lobbyPrefab, lobbyHolder.transform);
                LobbyItem item = newLobby.GetComponent<LobbyItem>();

                item.PopulateLobbyInfo(lobby.Name, lobby.Id);
            }
        }
        catch
        {
            Debug.Log("No lobbies found");
        }
    }

    private void OpenLobbyUI()
    {
        listUI.SetActive(false);
        lobbyUI.SetActive(true);

        if (NetworkManager.Singleton.IsHost)
        {
            kickObject.SetActive(true);
            startObject.SetActive(true);

            hostPlayerText.text = LobbyManager.Instance.joinedLobby.Players[0].Data["PlayerName"].Value;
            clientPlayerText.text = "???";
        }
            
        else
        {
            kickObject.SetActive(false);
            startObject.SetActive(false);
        }
    }

    private void OpenListUI()
    {
        hostButton.interactable = true;
        kickObject.SetActive(false);
        startObject.SetActive(false);

        lobbyUI.SetActive(false);
        listUI.SetActive(true);

        RefreshList();
    }

    private void ClientJoined()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            kickButton.interactable = true;
            startButton.interactable = true;

            Debug.Log(LobbyManager.Instance.joinedLobby.Players.Count);

            if (LobbyManager.Instance.joinedLobby.Players.Count > 1)
            {
                clientPlayerText.text = LobbyManager.Instance.joinedLobby.Players[1].Data["PlayerName"].Value;
            }
        }
    }

    private void ClientLeft()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            kickButton.interactable = false;
            startButton.interactable = false;

            hostPlayerText.text = LobbyManager.Instance.joinedLobby.Players[0].Data["PlayerName"].Value;
            clientPlayerText.text = "???";
        }
    }
    
    private void ClientReady(string name)
    {
        clientPlayerText.text = name;
    }
}
