using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MatchGenerator : Menu
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector intro;
    [SerializeField] private CardDisplay[] allyTeamDisplay;
    [SerializeField] private CardDisplay[] enemyTeamDisplay;

    [Header("Sprites")]
    [SerializeField] private Image glImage;
    [SerializeField] private Image mfImage;
    [SerializeField] private Image fwImage;

    [Header("Buttons")]
    [SerializeField] private Button[] glButtons;
    [SerializeField] private Button[] mfButtons;
    [SerializeField] private Button[] fwButtons;

    [Header("Names")]
    [SerializeField] private TextMeshProUGUI glName;
    [SerializeField] private TextMeshProUGUI mfName;
    [SerializeField] private TextMeshProUGUI fwName;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    private void OnEnable()
    {
        RankDisplay.RankDisplayFinished.AddListener(PrepareMatchDisplay);
        //PrepareMatchDisplay();
    }

    private void PrepareMatchDisplay()
    {
        glImage.sprite = SetStrikerSprite(MatchData.current.goalie.selectedStriker);
        mfImage.sprite = SetStrikerSprite(MatchData.current.midfielder.selectedStriker);
        fwImage.sprite = SetStrikerSprite(MatchData.current.forward.selectedStriker);

        PrepareButtons(MatchData.current.goalie.player, glButtons);
        PrepareButtons(MatchData.current.midfielder.player, mfButtons);
        PrepareButtons(MatchData.current.forward.player, fwButtons);

        glName.text = MatchData.current.goalie.player.playerName;
        mfName.text = MatchData.current.midfielder.player.playerName;
        fwName.text = MatchData.current.forward.player.playerName;

        GenerateEnemyTeam();

        allyTeamDisplay[0].cardName = MatchData.current.goalie.player.playerName;
        allyTeamDisplay[1].cardName = MatchData.current.midfielder.player.playerName;
        allyTeamDisplay[2].cardName = MatchData.current.forward.player.playerName;

        enemyTeamDisplay[0].cardName = MatchData.current.enemyGoalie.player.playerName;
        enemyTeamDisplay[1].cardName = MatchData.current.enemyMidfielder.player.playerName;
        enemyTeamDisplay[2].cardName = MatchData.current.enemyForward.player.playerName;

        foreach (CardDisplay display in allyTeamDisplay)
            display.DisplayPlayerCard();

        foreach (CardDisplay display in enemyTeamDisplay)
            display.DisplayPlayerCard();

        CheckStartConditions();

        intro.Play();
    }

    public void StartMatch()
    {
        SceneManager.LoadScene((int)Scenes.Match);
    }

    public void SetGoalieSelectedStriker(int value)
    {
        MatchData.current.goalie.selectedStrikerIndex += value;

        if (MatchData.current.goalie.selectedStrikerIndex >= MatchData.current.goalie.player.roster.Length)
            MatchData.current.goalie.selectedStrikerIndex = 0;
        else if (MatchData.current.goalie.selectedStrikerIndex < 0)
            MatchData.current.goalie.selectedStrikerIndex = MatchData.current.goalie.player.roster.Length - 1;

        MatchData.current.goalie.selectedStriker = MatchData.current.goalie.player.roster[MatchData.current.goalie.selectedStrikerIndex];

        glImage.sprite = SetStrikerSprite(MatchData.current.goalie.selectedStriker);

        CheckStartConditions();
    }

    public void SetMidfielderSelectedStriker(int value)
    {
        MatchData.current.midfielder.selectedStrikerIndex += value;

        if (MatchData.current.midfielder.selectedStrikerIndex >= MatchData.current.midfielder.player.roster.Length)
            MatchData.current.midfielder.selectedStrikerIndex = 0;
        else if (MatchData.current.midfielder.selectedStrikerIndex < 0)
            MatchData.current.midfielder.selectedStrikerIndex = MatchData.current.midfielder.player.roster.Length - 1;

        MatchData.current.midfielder.selectedStriker = MatchData.current.midfielder.player.roster[MatchData.current.midfielder.selectedStrikerIndex];

        mfImage.sprite = SetStrikerSprite(MatchData.current.midfielder.selectedStriker);

        CheckStartConditions();
    }

    public void SetForwardSelectedStriker(int value)
    {
        MatchData.current.forward.selectedStrikerIndex += value;

        if (MatchData.current.forward.selectedStrikerIndex >= MatchData.current.forward.player.roster.Length)
            MatchData.current.forward.selectedStrikerIndex = 0;
        else if (MatchData.current.forward.selectedStrikerIndex < 0)
            MatchData.current.forward.selectedStrikerIndex = MatchData.current.forward.player.roster.Length - 1;

        MatchData.current.forward.selectedStriker = MatchData.current.forward.player.roster[MatchData.current.forward.selectedStrikerIndex];

        fwImage.sprite = SetStrikerSprite(MatchData.current.forward.selectedStriker);

        CheckStartConditions();
    }

    private void GenerateEnemyTeam()
    {
        PlayerCard[] playerList = Resources.LoadAll<PlayerCard>("PlayerCards");

        MatchData.current.currentRankGoalies.Clear();
        MatchData.current.currentRankMidfielders.Clear();
        MatchData.current.currentRankForwards.Clear();

        foreach (PlayerCard player in playerList)
        {
            if (player.rank == MatchData.current.rank)
                switch (player.role)
                {
                    case (Role.Goalie):
                        MatchData.current.currentRankGoalies.Add(player);
                        break;
                    case (Role.Midfielder):
                        MatchData.current.currentRankMidfielders.Add(player);
                        break;
                    case (Role.Forward):
                        MatchData.current.currentRankForwards.Add(player);
                        break;
                }
        }

        //Debug.Log(MatchData.current.currentRankGoalies.Count);
        //Debug.Log(MatchData.current.currentRankMidfielders.Count);
        //Debug.Log(MatchData.current.currentRankForwards.Count);

        // Remove Selected Players
        if (MatchData.current.currentRankGoalies.Contains(MatchData.current.goalie.player))
            MatchData.current.currentRankGoalies.Remove(MatchData.current.goalie.player);
        if (MatchData.current.currentRankMidfielders.Contains(MatchData.current.midfielder.player))
            MatchData.current.currentRankMidfielders.Remove(MatchData.current.midfielder.player);
        if (MatchData.current.currentRankForwards.Contains(MatchData.current.forward.player))
            MatchData.current.currentRankForwards.Remove(MatchData.current.forward.player);

        int playerIndex; int strikerIndex;

        // Goalie

        playerIndex = Random.Range(0, MatchData.current.currentRankGoalies.Count);
        MatchData.current.enemyGoalie.player = MatchData.current.currentRankGoalies[playerIndex];
        strikerIndex = Random.Range(0, MatchData.current.enemyGoalie.player.roster.Length);

        MatchData.current.enemyGoalie.selectedStriker = MatchData.current.enemyGoalie.player.roster[strikerIndex];
        MatchData.current.enemyGoalie.selectedStrikerIndex = strikerIndex;

        // Midfielder

        playerIndex = Random.Range(0, MatchData.current.currentRankMidfielders.Count);
        MatchData.current.enemyMidfielder.player = MatchData.current.currentRankMidfielders[playerIndex];
        strikerIndex = Random.Range(0, MatchData.current.enemyMidfielder.player.roster.Length);

        MatchData.current.enemyMidfielder.selectedStriker = MatchData.current.enemyMidfielder.player.roster[strikerIndex];
        MatchData.current.enemyMidfielder.selectedStrikerIndex = strikerIndex;

        // Forward

        playerIndex = Random.Range(0, MatchData.current.currentRankForwards.Count);
        MatchData.current.enemyForward.player = MatchData.current.currentRankForwards[playerIndex];
        strikerIndex = Random.Range(0, MatchData.current.enemyForward.player.roster.Length);

        MatchData.current.enemyForward.selectedStriker = MatchData.current.enemyForward.player.roster[strikerIndex];
        MatchData.current.enemyForward.selectedStrikerIndex = strikerIndex;
    }

    private void CheckStartConditions()
    {
        if ((MatchData.current.goalie.selectedStriker == MatchData.current.midfielder.selectedStriker) ||
            (MatchData.current.goalie.selectedStriker == MatchData.current.forward.selectedStriker) ||
            (MatchData.current.midfielder.selectedStriker == MatchData.current.forward.selectedStriker))
            startButton.interactable = false;
        else startButton.interactable = true;
    }

    private void PrepareButtons(PlayerCard player, Button[] buttons)
    {
        if (player.roster.Length > 1)
            foreach (Button button in buttons)
                button.interactable = true;
        else
            foreach (Button button in buttons)
                button.interactable = false;
    }

    private Sprite SetStrikerSprite(Strikers striker)
    {
        return Resources.Load<Sprite>("CharacterIcons/" + striker + "_icon");
    }
}
