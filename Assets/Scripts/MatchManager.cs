using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    //[SerializeField] private GameState gameState;
    public CombatSystemDebugTool devTool;

    [Header("Turns")]
    public int turn;
    public float turnTime;
    public int actionPoint;
    public int highestActionMetter;
    public bool isActionAchieved;
    public bool isPlayerAction;
    public Striker currentStriker;

    [Header("Conditions")]
    public int areaSize;
    public int pointsToWin;
   
    public bool isGameFinished;

    [Header("Teams")]
    public Team[] teams;
    public CPU cpu;

    [Header("Core")]
    public CoreAI coreAI;
    public Core core;
    public int corePosition;
    public int sideSize;
    public Vector3[] coreMovements;
    public Vector3 allyGoalPosition;
    public Vector3 enemyGoalPosition;

    [Header("Action UI")]
    public ActionButton[] actionButtons;
    public object[][] currentStrikerInformation = new object[7][];

    [Header("Visuals")]
    public Figurine[] figs;

    [Header("Logs")]
    public LogHolder[] logHolders;

    public PlayerCard[] players;
    public ActionBar blueTeamActionBar;
    public ActionBar redTeamActionBar;
    public Image mapImage;
    public Sprite[] mapSprites;

    [Header("Positions")]
    public Vector3[] b_GoaliePos;
    public Vector3[] b_MidPos;
    public Vector3[] b_ForwardPos;
    public Vector3[] r_GoaliePos;
    public Vector3[] r_MidPos;
    public Vector3[] r_ForwardPos;

    [Header("Cameras & Visual UI")]
    public GameObject visualUI;
    public CinemachineVirtualCamera winCamera;
    public CinemachineVirtualCamera loseCamera;

    void Start()
    {
        PrepareGame();        
    }

    #region Game Set
    private void PrepareGame()
    {
        core = new Core(Position.Middle, areaSize);

        if (areaSize % 2 == 0)
            areaSize++;

        pointsToWin = areaSize + ((areaSize - 1) / 2);

        coreMovements = new Vector3[areaSize * 3 + 2];
        int aux = sideSize / (pointsToWin + 1);
        for(int i = 0; i < areaSize * 3 + 2; i++)
        {
            coreMovements[i] = new Vector3(-sideSize + aux * i, 0f, -5f);
        }
        allyGoalPosition = new Vector3(-sideSize - aux, 0f, 0f);
        enemyGoalPosition = new Vector3(sideSize + aux, 0f, 0f);

        // Player Team
        players[0] = MatchData.current.goalie.player;
        players[1] = MatchData.current.midfielder.player;
        players[2] = MatchData.current.forward.player;

        Strikers p1 = MatchData.current.goalie.selectedStriker;
        Strikers p2 = MatchData.current.midfielder.selectedStriker;
        Strikers p3 = MatchData.current.forward.selectedStriker;

        // CPU Team
        players[3] = MatchData.current.enemyGoalie.player;
        players[4] = MatchData.current.enemyMidfielder.player;
        players[5] = MatchData.current.enemyForward.player;

        Strikers p4 = MatchData.current.enemyGoalie.selectedStriker;
        Strikers p5 = MatchData.current.enemyMidfielder.selectedStriker;
        Strikers p6 = MatchData.current.enemyForward.selectedStriker;

        teams = new Team[2];

        // Player Team
        teams[0] = new Team(p1, p2, p3, players[0], players[1], players[2], figs[0], figs[1], figs[2], b_GoaliePos, b_MidPos, b_ForwardPos, 0, 1, this);
        
        // CPU Team
        teams[1] = new Team(p4, p5, p6, players[3], players[4], players[5], figs[3], figs[4], figs[5], r_GoaliePos, r_MidPos, r_ForwardPos, 1, 0, this);

        blueTeamActionBar.team = teams[0];
        redTeamActionBar.team = teams[1];
        blueTeamActionBar.SetStrikerIcons();
        redTeamActionBar.SetStrikerIcons();

        mapImage.sprite = mapSprites[(int)core.mapPosition];

        UnassignPlayerActions();
        devTool.MatchReady();
        StartCoroutine(PlayTurnCoroutine());
    }

    private void PlayTurn()
    {
        //gameState = GameState.Running;
        turn++;
        corePosition = core.position;

        teams[0].Turn();  //Player Team
        teams[1].Turn();  //CPU Team

        blueTeamActionBar.MoveAllyActionBar(); // Player Team
        redTeamActionBar.MoveEnemyActionBar(); // CPU Team

        highestActionMetter = actionPoint - 1;
        isActionAchieved = false;
        isPlayerAction = false;

        CheckActionMetters(0);  //Player Team
        CheckActionMetters(1);  //CPU Team

        if (isActionAchieved)
        {
            //gameState = GameState.Action;

            logHolders[currentStriker.allyTeam].AddLog(currentStriker.character, currentStriker.name + " turn", currentStriker.allyTeam);
            Debug.Log(currentStriker.name + " turn");
            devTool.CurrentTurnDisplay(currentStriker);

            currentStriker.PrepareAction();

            if (currentStriker.isAlive && !currentStriker.isStunned)
            {
                if (isPlayerAction)
                {
                    currentStriker.PreparePlayerActionAssign();
                }

                else cpu.RandomCPUActions(currentStriker);
            }

            else
            {
                logHolders[currentStriker.allyTeam].AddLog(currentStriker.character, " is dead or stunned", currentStriker.allyTeam);
                Debug.Log(currentStriker.name + " is dead or stunned");
                StartCoroutine(PlayTurnCoroutine());
            }
        }

        else StartCoroutine(PlayTurnCoroutine());
    }

    private IEnumerator PlayTurnCoroutine()
    {
        yield return new WaitForSeconds(turnTime);

        PlayTurn();
    }

    private void CheckActionMetters(int i)
    {
        if (teams[i].goalie.actionMetter > actionPoint && teams[i].goalie.actionMetter > highestActionMetter)
        {
            SetCurrentStriker(teams[i].goalie, i);
        }

        if (teams[i].midfielder.actionMetter > actionPoint && teams[i].midfielder.actionMetter > highestActionMetter)
        {
            SetCurrentStriker(teams[i].midfielder, i);
        }

        if (teams[i].forward.actionMetter > actionPoint && teams[i].forward.actionMetter > highestActionMetter)
        {
            SetCurrentStriker(teams[i].forward, i);
        }
    }

    private void SetCurrentStriker(Striker stkr, int i)
    {
        currentStriker = stkr;
        highestActionMetter = stkr.actionMetter;
        isActionAchieved = true;

        if (isActionAchieved)
        {
            if (i == 0) isPlayerAction = true;
            else isPlayerAction = false;
        }
    }

    public void EndTurn()
    {
        core.CheckCorePosition();
        mapImage.sprite = mapSprites[(int)core.mapPosition];

        if (!isGameFinished)
        {
            blueTeamActionBar.MoveAllyActionBar();
            redTeamActionBar.MoveEnemyActionBar();
        }
        
        Debug.Log(currentStriker.name + " turn end");

        if (isPlayerAction)
            UnassignPlayerActions();
        if (!isGameFinished)
            StartCoroutine(PlayTurnCoroutine());
    }

    public IEnumerator PlayerWin()
    {
        winCamera.gameObject.SetActive(true);
        visualUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        GiveRankedReward();
    }

    private async void GiveRankedReward()
    {
        int matchReward = 20 * (8 - (int)MatchData.current.rank);

        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("CREDITS", matchReward);

        MatchData.current.reward += matchReward;
        MatchData.current.rank = GetNextRank(MatchData.current.rank);

        if (MatchData.current.rank == Rank.Rookie)
        {
            //MatchData.current.rank = Rank.ProLeague;
            SceneManager.LoadScene((int)Scenes.Results);
        }
        else
            SceneManager.LoadScene((int)Scenes.TeamSelect);
    }

    public IEnumerator PlayerLose()
    {
        loseCamera.gameObject.SetActive(true);
        visualUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene((int)Scenes.Results);
    }

    private Rank GetNextRank(Rank rank)
    {
        switch (rank)
        {
            case (Rank.Bronze):
                return Rank.Silver;
            case (Rank.Silver):
                return Rank.Gold;
            case (Rank.Gold):
                return Rank.Platinum;
            case (Rank.Platinum):
                return Rank.Diamond;
            case (Rank.Diamond):
                return Rank.Challenger;
            case (Rank.Challenger):
                return Rank.Omega;
            case (Rank.Omega):
                return Rank.ProLeague;

            default:
                return Rank.Rookie;
        }
    }

    public void AssignPlayerActions(Striker striker)
    {
        UnassignPlayerActions();

        actionButtons[0].AssignStrikerAction(striker.character, striker.Evade, striker.s_evade, ActionType.Evade, currentStrikerInformation);
        actionButtons[1].AssignStrikerAction(striker.character, striker.PrimarySkill, striker.primary, ActionType.Primary, currentStrikerInformation[1]);
        actionButtons[2].AssignStrikerAction(striker.character, striker.SecondarySkill, striker.secondary, ActionType.Secondary, currentStrikerInformation[2]);
        actionButtons[3].AssignStrikerAction(striker.character, striker.UltimateSkill, striker.ultimate, ActionType.Ultimate, currentStrikerInformation[3]);
        actionButtons[4].AssignStrikerAction(striker.character, striker.Strike, striker.s_strike, ActionType.Strike, currentStrikerInformation[4]);

        if (striker.mapPosition != Position.Left)
            actionButtons[5].AssignStrikerAction(striker.character, delegate { striker.Move(-1); }, striker.s_move, ActionType.MoveLeft, currentStrikerInformation);

        if (striker.mapPosition != Position.Right)
            actionButtons[6].AssignStrikerAction(striker.character, delegate { striker.Move(1); }, striker.s_move, ActionType.MoveRight, currentStrikerInformation);
    }

    private void UnassignPlayerActions()
    {
        foreach(ActionButton bt in actionButtons)
        {
            bt.UnassignAction();
        }
    }

    #endregion

    #region Actions
    public void Damage(int team, int ammount, Position area)
    {
        if (area == Position.All)
        {
            if (!teams[team].goalie.isEvading) teams[team].goalie.Damage(ammount);
            if (!teams[team].midfielder.isEvading) teams[team].midfielder.Damage(ammount);
            if (!teams[team].forward.isEvading) teams[team].forward.Damage(ammount);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area && !teams[team].goalie.isEvading)
            {
                teams[team].goalie.Damage(ammount);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area && !teams[team].midfielder.isEvading)
            {
                teams[team].midfielder.Damage(ammount);
            }

            // Forward
            if (teams[team].forward.mapPosition == area && !teams[team].forward.isEvading)
            {
                teams[team].forward.Damage(ammount);
            }
        }
    }

    public void Heal(int team, int ammount, Position area)
    {
        if (area == Position.All)
        {
            teams[team].goalie.Heal(ammount);
            teams[team].midfielder.Heal(ammount);
            teams[team].forward.Heal(ammount);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area)
            {
                teams[team].goalie.Heal(ammount);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area)
            {
                teams[team].midfielder.Heal(ammount);
            }

            // Forward
            if (teams[team].forward.mapPosition == area)
            {
                teams[team].forward.Heal(ammount);
            }
        }
    }

    public void Stun(int team, Position area)
    {
        if (area == Position.All)
        {
            if (!teams[team].goalie.isEvading) teams[team].goalie.Stun();
            if (!teams[team].midfielder.isEvading) teams[team].midfielder.Stun();
            if (!teams[team].forward.isEvading) teams[team].forward.Stun();
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area && !teams[team].goalie.isEvading)
            {
                teams[team].goalie.Stun();
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area && !teams[team].midfielder.isEvading)
            {
                teams[team].midfielder.Stun();
            }

            // Forward
            if (teams[team].forward.mapPosition == area && !teams[team].forward.isEvading)
            {
                teams[team].forward.Stun();
            }
        }
    }

    public void Accelerate(int team, int ammount, Position area)
    {
        if (area == Position.All)
        {
            teams[team].goalie.Accelerate(ammount);
            teams[team].midfielder.Accelerate(ammount);
            teams[team].forward.Accelerate(ammount);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area)
            {
                teams[team].goalie.Accelerate(ammount);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area)
            {
                teams[team].midfielder.Accelerate(ammount);
            }

            // Forward
            if (teams[team].forward.mapPosition == area)
            {
                teams[team].forward.Accelerate(ammount);
            }
        }
    }

    public void Slow(int team, int ammount, Position area)
    {
        if (area == Position.All)
        {
            if (!teams[team].goalie.isEvading) teams[team].goalie.Slow(ammount);
            if (!teams[team].midfielder.isEvading) teams[team].midfielder.Slow(ammount);
            if (!teams[team].forward.isEvading) teams[team].forward.Slow(ammount);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area && !teams[team].goalie.isEvading)
            {
                teams[team].goalie.Slow(ammount);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area && !teams[team].midfielder.isEvading)
            {
                teams[team].midfielder.Slow(ammount);
            }

            // Forward
            if (teams[team].forward.mapPosition == area && !teams[team].forward.isEvading)
            {
                teams[team].forward.Slow(ammount);
            }
        }
    }

    public void Debuff(int team, int ammount, Position area, BuffType buff)
    {
        if (area == Position.All)
        {
            if (!teams[team].goalie.isEvading) teams[team].goalie.Debuff(ammount, buff);
            if (!teams[team].midfielder.isEvading) teams[team].midfielder.Debuff(ammount, buff);
            if (!teams[team].forward.isEvading) teams[team].forward.Debuff(ammount, buff);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area && !teams[team].goalie.isEvading)
            {
                teams[team].goalie.Debuff(ammount, buff);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area && !teams[team].midfielder.isEvading)
            {
                teams[team].midfielder.Debuff(ammount, buff);
            }

            // Forward
            if (teams[team].forward.mapPosition == area && !teams[team].forward.isEvading)
            {
                teams[team].forward.Debuff(ammount, buff);
            }
        }
    }

    public void Buff(int team, int ammount, Position area, BuffType buff)
    {
        if (area == Position.All)
        {
            teams[team].goalie.Buff(ammount, buff);
            teams[team].midfielder.Buff(ammount, buff);
            teams[team].forward.Buff(ammount, buff);
        }

        else
        {
            // Goalie
            if (teams[team].goalie.mapPosition == area)
            {
                teams[team].goalie.Buff(ammount, buff);
            }

            // Midfielder
            if (teams[team].midfielder.mapPosition == area)
            {
                teams[team].midfielder.Buff(ammount, buff);
            }

            // Forward
            if (teams[team].forward.mapPosition == area)
            {
                teams[team].forward.Buff(ammount, buff);
            }
        }
    }

    public bool IsMapAreaClear(int team, Position area)
    {
        if ((teams[team].goalie.mapPosition == area && teams[team].goalie.isAlive)
            || (teams[team].midfielder.mapPosition == area && teams[team].midfielder.isAlive)
            || (teams[team].forward.mapPosition == area && teams[team].forward.isAlive))
        {
            return false;
        }

        return true;
    }

    public bool IsMapAreaClear(int team, int index)
    {
        Position area = GetAreaByIndex(index);

        if ((teams[team].goalie.mapPosition == area && teams[team].goalie.isAlive && !teams[team].goalie.isEvading)
            || (teams[team].midfielder.mapPosition == area && teams[team].midfielder.isAlive && !teams[team].midfielder.isEvading)
            || (teams[team].forward.mapPosition == area && teams[team].forward.isAlive && !teams[team].forward.isEvading))
        {
            return false;
        }

        return true;
    }

    public Position GetAreaByIndex(int index)
    {
        switch (index)
        {
            case (0):
                return Position.Left;

            case (1):
                return Position.Middle;

            case (2):
                return Position.Right;

            default:
                return Position.Left;
        }
    }

    public void PushEnemy(int team, int direction, Position area)
    {
        switch (team)
        {
            case (0):
                if (teams[0].goalie.mapPosition == area && !teams[0].goalie.isEvading)
                    if ((direction == 1 && teams[0].goalie.mapPosition != Position.Left) || (direction == -1 && teams[0].goalie.mapPosition != Position.Right))
                        teams[0].goalie.Push(-1 * direction);
                if (teams[0].midfielder.mapPosition == area && !teams[0].midfielder.isEvading)
                    if ((direction == 1 && teams[0].midfielder.mapPosition != Position.Left) || (direction == -1 && teams[0].midfielder.mapPosition != Position.Right))
                        teams[0].midfielder.Push(-1 * direction);
                if (teams[0].forward.mapPosition == area && !teams[0].forward.isEvading)
                    if ((direction == 1 && teams[0].forward.mapPosition != Position.Left) || (direction == -1 && teams[0].forward.mapPosition != Position.Right))
                        teams[0].forward.Push(-1 * direction);
                break;
            case (1):
                if (teams[1].goalie.mapPosition == area && !teams[1].goalie.isEvading)
                    if ((direction == 1 && teams[1].goalie.mapPosition != Position.Right) || direction == -1 && teams[1].goalie.mapPosition != Position.Left)
                        teams[1].goalie.Push(1 * direction);
                if (teams[1].midfielder.mapPosition == area && !teams[1].midfielder.isEvading && teams[1].midfielder.mapPosition != Position.Right)
                    if ((direction == 1 && teams[1].midfielder.mapPosition != Position.Right) || direction == -1 && teams[1].midfielder.mapPosition != Position.Left)
                        teams[1].midfielder.Push(1 * direction);
                if (teams[1].forward.mapPosition == area && !teams[1].forward.isEvading && teams[1].forward.mapPosition != Position.Right)
                    if ((direction == 1 && teams[1].forward.mapPosition != Position.Right) || direction == -1 && teams[1].forward.mapPosition != Position.Left)
                        teams[1].forward.Push(1 * direction);
                break;
        }
    }

    public void PushCore(int team, int ammount)
    {
        switch(team)
        {
            case (0):
                core.position += ammount;
                break;

            case (1):
                core.position -= ammount;
                break;
        }

        corePosition = core.position;

        if (corePosition < -pointsToWin)
        {
            coreAI.SetDestination(allyGoalPosition);

            Debug.Log("CPU Wins");
            isGameFinished = true;

            StartCoroutine(PlayerLose());
        }

        else if (corePosition > pointsToWin)
        {
            coreAI.SetDestination(enemyGoalPosition);

            Debug.Log("Player Wins");
            isGameFinished = true;

            StartCoroutine(PlayerWin());
        }

        else
            coreAI.SetDestination(coreMovements[(pointsToWin + 1) + corePosition]);
    }
    #endregion

    #region CPU
    public bool IsTeamOnCore()
    {
        if (teams[1].goalie.mapPosition == core.mapPosition)
            return false;
        else if (teams[1].midfielder.mapPosition == core.mapPosition)
            return false;
        else if (teams[1].forward.mapPosition == core.mapPosition)
            return false;
        else return true;
    }

    public bool IsCoreAreaEmpty()
    {
        if (IsMapAreaClear(0, core.mapPosition))
            return true;
        else return false;
    }

    public bool IsTeamOutNumberedOnCore()
    {
        int[] strikersOnCore = new int[2];

        for (int i = 0; i < 2; i++)
        {
            if (teams[i].goalie.mapPosition == core.mapPosition && teams[i].goalie.isAlive)
                strikersOnCore[i]++;

            if (teams[i].midfielder.mapPosition == core.mapPosition && teams[i].midfielder.isAlive)
                strikersOnCore[i]++;

            if (teams[i].forward.mapPosition == core.mapPosition && teams[i].forward.isAlive)
                strikersOnCore[i]++;
        }

        if (strikersOnCore[0] > strikersOnCore[1])
            return true;
        else return false;
    }

    public bool AreThereValidTargets(Position pos)
    {
        if (teams[0].goalie.mapPosition == pos && teams[0].goalie.isAlive && !teams[0].goalie.isEvading)
            return true;
        else if (teams[0].midfielder.mapPosition == pos && teams[0].midfielder.isAlive && !teams[0].midfielder.isEvading)
            return true;
        else if (teams[0].forward.mapPosition == pos && teams[0].forward.isAlive && !teams[0].forward.isEvading)
            return true;
        else return false;
    }

    public bool IsCoreOnTheLeft(Position pos)
    {
        if ((int)core.mapPosition < (int)pos)
            return true;
        else return false;
    }

    public bool IsCoreOnTheRight(Position pos)
    {
        if ((int)core.mapPosition > (int)pos)
            return true;
        else return false;
    }
    #endregion

    [System.Serializable]
    private enum GameState
    {
        Starting,
        Running,
        Action,
        Finished
    }
}