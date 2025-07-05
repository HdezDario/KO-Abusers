using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    [SerializeField] private string player;

    [Header("Starting Roster")]
    [SerializeField] private string initialGoalie;
    [SerializeField] private string initialMidfielder;
    [SerializeField] private string initialForward;

    [Header("Sections")]
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject teamSelectObject;
    [SerializeField] private GameObject inventoryObject;
    [SerializeField] private GameObject gachaObject;
    [SerializeField] private GameObject newPlayerObject;

    void Start()
    {
        PrepareGameStart();
    }

    private async void PrepareGameStart()
    {
        await InitializeUnityAuthentication();

        if (SaveSystem.Load(Application.persistentDataPath + "/SaveFiles/SaveData.ko") == null)
            NewGame();

        else
        {
            LoadGame();
            mainMenuObject.SetActive(true);
            CheckUnlockedPlayers();
        }
    }

    private async Task<bool> InitializeUnityAuthentication()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
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

            return true;
        }

        catch
        {
            Debug.Log("Couldnt Connect");
            return default;
        }
    }

    private async void NewGame()
    {
        SaveData.current = new SaveData();

        SaveData.current.name = "";
        //SaveData.current.credits = 1000;
        SaveData.current.unlockedPlayerCards    = new List<string>();
        SaveData.current.unlockedGoalies        = new List<string>();
        SaveData.current.unlockedMidfielders    = new List<string>();
        SaveData.current.unlockedForwards       = new List<string>();

        UnlockPlayerCard(initialGoalie);
        UnlockPlayerCard(initialMidfielder);
        UnlockPlayerCard(initialForward);

        //UnlockPlayerCard("Cooler Dax");

        SetTeamMember(initialGoalie, Role.Goalie);
        SetTeamMember(initialMidfielder, Role.Midfielder);
        SetTeamMember(initialForward, Role.Forward);

        int creditsAmmount = 75;
        await EconomyService.Instance.PlayerBalances.SetBalanceAsync("CREDITS", creditsAmmount);

        mainMenuObject.SetActive(false);
        newPlayerObject.SetActive(true);
    }

    private void CheckUnlockedPlayers()
    {
        List<string> avaiblePlayers = new List<string>();
        List<string> updatedPlayers = new List<string>();
        PlayerCard[] playerCards = Resources.LoadAll<PlayerCard>("PlayerCards");

        foreach (PlayerCard playerCard in playerCards)
        {
            avaiblePlayers.Add(playerCard.name);
        }

        foreach (string player in SaveData.current.unlockedPlayerCards)
        {
            if (avaiblePlayers.Contains(player) && !updatedPlayers.Contains(player))
                updatedPlayers.Add(player);
        }

        SaveData.current.unlockedPlayerCards.Clear();
        SaveData.current.unlockedGoalies.Clear();
        SaveData.current.unlockedMidfielders.Clear();
        SaveData.current.unlockedForwards.Clear();

        foreach(string player in updatedPlayers)
        {
            UnlockPlayerCard(player);
        }

        SaveGame();
    }

    [ContextMenu("Save Game")]
    public void SaveGameProgress()
    {
        SaveGame();
        Debug.Log("Game Saved Manually");
    }

    [ContextMenu("Unlock Player Card")]
    public void UnlockFromMenu()
    {
        UnlockPlayerCard(player);
        SaveGame();
    }


    [ContextMenu("Unlock ALL Players")]
    public void UnlockAllPlayers()
    {
        PlayerCard[] playerCards = Resources.LoadAll<PlayerCard>("PlayerCards");

        foreach (PlayerCard playerCard in playerCards)
        {
            UnlockPlayerCard(playerCard.name);
        }
    }

    public void RestartProgress()
    {
        DeleteGame();
        NewGame();
    }

    public void StartRanked()
    {
        SaveGame();

        MatchData.current = new MatchData();
        
        // Goalie
        MatchData.current.goalie.player = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.goalie);
        MatchData.current.goalie.strikers = new Strikers[MatchData.current.goalie.player.roster.Length];
        MatchData.current.goalie.player.roster.CopyTo(MatchData.current.goalie.strikers, 0);
        MatchData.current.goalie.strikersState = new int[MatchData.current.goalie.strikers.Length];
        MatchData.current.goalie.selectedStriker = MatchData.current.goalie.strikers[0];
        MatchData.current.goalie.selectedStrikerIndex = 0;

        // Midfielder
        MatchData.current.midfielder.player = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.midfielder);
        MatchData.current.midfielder.strikers = new Strikers[MatchData.current.midfielder.player.roster.Length];
        MatchData.current.midfielder.player.roster.CopyTo(MatchData.current.midfielder.strikers, 0);
        MatchData.current.midfielder.strikersState = new int[MatchData.current.midfielder.strikers.Length];
        MatchData.current.midfielder.selectedStriker = MatchData.current.midfielder.strikers[0];
        MatchData.current.midfielder.selectedStrikerIndex = 0;

        // Forward
        MatchData.current.forward.player = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.forward);
        MatchData.current.forward.strikers = new Strikers[MatchData.current.forward.player.roster.Length];
        MatchData.current.forward.player.roster.CopyTo(MatchData.current.forward.strikers, 0);
        MatchData.current.forward.strikersState = new int[MatchData.current.forward.strikers.Length];
        MatchData.current.forward.selectedStriker = MatchData.current.forward.strikers[0];
        MatchData.current.forward.selectedStrikerIndex = 0;

        // Player Lists
        MatchData.current.currentRankGoalies = new List<PlayerCard>();
        MatchData.current.currentRankMidfielders = new List<PlayerCard>();
        MatchData.current.currentRankForwards = new List<PlayerCard>();

        // Rank
        MatchData.current.rank = Rank.Bronze;
        MatchData.current.reward = 0;

        SceneManager.LoadScene((int)Scenes.TeamSelect);
    }

    public void OpenTeamSelect()
    {
        mainMenuObject.SetActive(false);
        teamSelectObject.SetActive(true);
    }

    public void OpenInventory()
    {
        mainMenuObject.SetActive(false);
        inventoryObject.SetActive(true);
    }

    public void OpenGacha()
    {
        mainMenuObject.SetActive(false);
        gachaObject.SetActive(true);
    }

    public void CloseTeamSelect()
    {
        SaveGame();

        mainMenuObject.SetActive(true);
        teamSelectObject.SetActive(false);
    }

    public void CloseInventory()
    {
        mainMenuObject.SetActive(true);
        inventoryObject.SetActive(false);
    }

    public void CloseGacha()
    {
        mainMenuObject.SetActive(true);
        gachaObject.SetActive(false);
    }

    public void OpenMultiplayer()
    {
        SceneManager.LoadScene(Scenes.Lobby.ToString());
    }
    
    public void OpenLeaderboard()
    {
        SceneManager.LoadScene(Scenes.Leaderboard.ToString());
    }
}
