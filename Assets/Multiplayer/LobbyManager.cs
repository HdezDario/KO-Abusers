using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.Events;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Economy;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private string playerName;

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private const string LEADERBOARD = "Leaderboard";

    [SerializeField] private int playerScore;

    public UnityEvent OnLobbyJoined;
    public UnityEvent OnLobbyExited;
    public UnityEvent OnLobbyKicked;

    public UnityEvent OnClientJoined;
    public UnityEvent OnClientLeave;
    public UnityEvent<string> OnClientReady;

    public Lobby joinedLobby;
    private float heartBeatTimer;

    public static LobbyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            if (OnLobbyJoined == null)
                OnLobbyJoined = new UnityEvent();
            if (OnLobbyExited == null)
                OnLobbyExited = new UnityEvent();
            if (OnLobbyKicked == null)
                OnLobbyKicked = new UnityEvent();
            if (OnClientJoined == null)
                OnClientJoined = new UnityEvent();
            if (OnClientLeave == null)
                OnClientLeave = new UnityEvent();
            if (OnClientReady == null)
                OnClientReady = new UnityEvent<string>();

            InitializeUnityAuthentication();
        }
    }

    public void Disconnect()
    {
        if (joinedLobby != null)
            DeleteLobby();
        try
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
                NetworkManager.Singleton.Shutdown();
        }
        catch
        {
            Debug.Log("Host or Client werent initiliazed");
        }
        
    }

    public override void OnDestroy()
    {
        Disconnect();

        base.OnDestroy();
    }

    private async void InitializeUnityAuthentication()
    {
        playerName = SaveData.current.name;

        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                // Only for Local Testing
                //InitializationOptions options = new InitializationOptions();
                //playerName = Random.Range(0, 1000000).ToString();
                //options.SetProfile(Random.Range(0, 1000000).ToString());

                await UnityServices.InitializeAsync(); // Insert Parameter options if testing locally

                Debug.Log("Initializing Unity Services");

                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
                };

                AuthenticationService.Instance.SignedOut += () =>
                {
                    Debug.Log("Signed out");
                };
            }

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
    }

    private async void HandleLobbyHeartBeat()
    {
        if (joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatMaxTimer = 15f;
                heartBeatTimer = heartBeatMaxTimer;

                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);

                    Debug.Log("beat");
                } catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
                
            }
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            return allocation;
        } catch(RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try {
            string allocationJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return allocationJoinCode;
        } catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
        
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
        
    }

    [ContextMenu("CreateLobby")]
    public async void CreateLobby()
    {
        try
        {
            string lobbyName = playerName + "'s Lobby";
            int maxPlayers = 2;
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions {
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartHost();

            OnLobbyJoined.Invoke();
            Debug.Log("Created Lobby! " + joinedLobby.Name + " " + joinedLobby.MaxPlayers);
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        try {
            if (joinedLobby != null)
            {
                KickPlayer();

                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
                NetworkManager.Singleton.Shutdown();

                OnLobbyExited.Invoke();
                Debug.Log("Lobby Deleted");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void KickPlayer()
    {
        try
        {
            GetKickedClientRpc();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task<QueryResponse> ListAvailableLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

            Debug.Log("Lobbies Found: " + queryResponse.Results.Count);

            return queryResponse;
        }  catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void JoinLobbyByLobbyId(string lobbyId)
    {
        try
        {
            //QueryResponse query = await Lobbies.Instance.QueryLobbiesAsync();

            JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };

            joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinOptions);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.OnClientStarted += Singleton_OnClientStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientStopped += Singleton_OnClientStopped;

            NetworkManager.Singleton.StartClient();

            OnLobbyJoined.Invoke();
            
            Debug.Log("Joined Lobby");
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        ClientJoinedServerRpc();
    }

    private void Singleton_OnClientStarted()
    {
        ClientJoinedServerRpc();
    }

    private void Singleton_OnClientStopped(bool isDisconected)
    {
        ClientLeftServerRpc();
    }

    public async void LeaveLobby()
    {
        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;

            await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);

            joinedLobby = null;
            
            NetworkManager.Singleton.Shutdown();

            NetworkManager.Singleton.OnClientStarted -= Singleton_OnClientStarted;
            NetworkManager.Singleton.OnClientStopped -= Singleton_OnClientStopped;
            NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;

            OnLobbyExited.Invoke();
            ClientLeftServerRpc();
            Debug.Log("Left Lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    #region LeaderBoards
    public class ScoreMetadata
    {
        public string name;

        public string goalie;
        public string midfielder;
        public string forward;

        public Strikers goalieStriker;
        public Strikers midfielderStriker;
        public Strikers forwardStriker;
    }

    public async void AddScore(int score, string playerName, string gl, string mf, string fw, Strikers s1, Strikers s2, Strikers s3)
    {
        try
        {
            LeaderboardEntry entry = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(
                LEADERBOARD,
                new GetPlayerScoreOptions { IncludeMetadata = true }
            );

            var scoreMetadata = new ScoreMetadata
            {
                name = playerName,
                goalie = gl,
                midfielder = mf,
                forward = fw,
                goalieStriker = s1,
                midfielderStriker = s2,
                forwardStriker = s3
            };

            if (score < 0)
            {
                if (entry.Score - 10 <= 0)
                {
                    await LeaderboardsService.Instance
                    .AddPlayerScoreAsync(LEADERBOARD, -entry.Score,
                        new AddPlayerScoreOptions { Metadata = scoreMetadata }
                    );
                }

                else
                {
                    await LeaderboardsService.Instance
                    .AddPlayerScoreAsync(LEADERBOARD, score,
                        new AddPlayerScoreOptions { Metadata = scoreMetadata }
                    );
                }
            }

            else
            {
                await LeaderboardsService.Instance
                .AddPlayerScoreAsync(LEADERBOARD, score,
                    new AddPlayerScoreOptions { Metadata = scoreMetadata }
                );
            }

            //SaveData.current.credits += 50;
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("CREDITS", 50);
        }
        catch
        {
            var scoreMetadata = new ScoreMetadata
            {
                name = playerName,
                goalie = gl,
                midfielder = mf,
                forward = fw,
                goalieStriker = s1,
                midfielderStriker = s2,
                forwardStriker = s3
            };

            await LeaderboardsService.Instance
                .AddPlayerScoreAsync(LEADERBOARD, 0,
                    new AddPlayerScoreOptions { Metadata = scoreMetadata }
                );
        }
    }


    [ContextMenu("AddScore")]
    public async void AddScore()
    {
        var scoreMetadata = new ScoreMetadata { 
            name = playerName,
            goalie = "Jorge",
            midfielder = "Miguel",
            forward = "Pepe",
            goalieStriker = Strikers.Hamster,
            midfielderStriker = Strikers.Sniper,
            forwardStriker = Strikers.Kicker
        };

        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(LEADERBOARD, playerScore,
                new AddPlayerScoreOptions { Metadata = scoreMetadata }
            );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }
    #endregion

    #region RPC
    [ClientRpc]
    public void InitializeClientDataClientRpc()
    {
        MultiplayerPlayerData.current = new MultiplayerPlayerData();

        MultiplayerPlayerData.current.playerName = SaveData.current.name;

        MultiplayerPlayerData.current.goalie = SaveData.current.goalie;
        MultiplayerPlayerData.current.midfielder = SaveData.current.midfielder;
        MultiplayerPlayerData.current.forward = SaveData.current.forward;
    }

    [ClientRpc]
    private void GetKickedClientRpc()
    {
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
        {
            LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            OnLobbyKicked.Invoke();
        }    
    }

    [ClientRpc]
    private void ClientJoinedClientRpc()
    {
        OnClientJoined.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClientJoinedServerRpc()
    {
        Debug.Log("Client Joined");
        ClientJoinedClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClientJoinedSetNameServerRpc(string name)
    {
        OnClientReady.Invoke(name);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClientLeftServerRpc()
    {
        Debug.Log("Client Left");
        OnClientLeave.Invoke();
    }
    #endregion
}
