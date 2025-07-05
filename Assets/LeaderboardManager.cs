using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    private const string LEADERBOARD = "Leaderboard";
    [SerializeField] private string playerName; 
    private LeaderboardEntry myScore;

    [SerializeField] private LeaderboardItemDisplay myItem;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private GameObject leaderboardItem;

    private void Awake()
    {
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                //InitializationOptions options = new InitializationOptions();
                ////playerName = Random.Range(0, 1000000).ToString();
                //options.SetProfile(Random.Range(0, 1000000).ToString());

                await UnityServices.InitializeAsync();
                Debug.Log("Initializing Unity Services");

                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Singed in " + AuthenticationService.Instance.PlayerId);
                };

                AuthenticationService.Instance.SignedOut += () =>
                {
                    Debug.Log("Signed out");
                };
            }

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
            Debug.Log("Couldnt Sign in");
        }

        myScore = await GetPlayerScore();
        myItem.ScoreDisplay(myScore);

        LeaderboardScoresPage board = await GetLeaderboardScores();

        foreach (LeaderboardEntry entry in board.Results)
        {
            GameObject LBItem = Instantiate(leaderboardItem, leaderboard.transform);
            LeaderboardItemDisplay itemDisplay = LBItem.GetComponent<LeaderboardItemDisplay>();

            itemDisplay.ScoreDisplay(entry);
        }
    }

    private async Task<LeaderboardEntry> GetPlayerScore()
    {
        try
        {
            LeaderboardEntry scoreResponse = await LeaderboardsService.Instance
            .GetPlayerScoreAsync(
                LEADERBOARD,
                new GetPlayerScoreOptions { IncludeMetadata = true }
            );
            Debug.Log("Rank obtained");
            return scoreResponse;
        } catch
        {
            return await AddNewEntry();            
        }
        
    }

    private async Task<LeaderboardScoresPage> GetLeaderboardScores()
    {
        try
        {
            LeaderboardScoresPage scoreResponse = await LeaderboardsService.Instance
            .GetScoresAsync(
                LEADERBOARD,
                new GetScoresOptions { Limit = 25, IncludeMetadata = true }
            );
            Debug.Log("Leaderboard Obtained");
            return scoreResponse;
        } catch
        {
            Debug.Log("Failed retrieving leaderboard");
            return default;
        }
        
    }

    public async Task<LeaderboardEntry> AddNewEntry()
    {
        try
        {
            PlayerCard goalie = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.goalie);
            PlayerCard midfielder = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.midfielder);
            PlayerCard forward = Resources.Load<PlayerCard>("PlayerCards/" + SaveData.current.forward);

            var scoreMetadata = new ScoreMetadata
            {
                name = SaveData.current.name,
                goalie = goalie.name,
                midfielder = midfielder.name,
                forward = forward.name,
                goalieStriker = goalie.roster[0],
                midfielderStriker = midfielder.roster[0],
                forwardStriker = forward.roster[0]
            };

            LeaderboardEntry playerEntry = await LeaderboardsService.Instance
                .AddPlayerScoreAsync(LEADERBOARD, 0, new AddPlayerScoreOptions { Metadata = scoreMetadata });

            return playerEntry;
        }
        catch
        {
            Debug.Log("Failed writing info");
            return default;
        }
        
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene((int)Scenes.Menu);
    }

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
}
