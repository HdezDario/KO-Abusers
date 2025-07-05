using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Events;
using TMPro;

public class MultiplayerMatchManager : NetworkBehaviour
{
    public static MultiplayerMatchManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private MultiplayerPlayerType localPlayerType;
    public NetworkVariable<MultiplayerPlayerType> currentPlayerType = new NetworkVariable<MultiplayerPlayerType>();
    private bool isBlueReady = false;
    private bool isRedReady = false;

    [Header("Turns")]
    public NetworkVariable<int> turn = new NetworkVariable<int>(0);
    public float turnTime;
    public int actionPoint;
    public NetworkVariable<int> highestActionMetter = new NetworkVariable<int>(0);
    public NetworkVariable<bool> isActionAchieved = new NetworkVariable<bool>(false);
    public TextMeshProUGUI turnText;
    public MultiplayerStriker currentStriker;

    [Header("Conditions")]
    public int areaSize;
    public int pointsToWin;
    public NetworkVariable<bool> isGameFinished = new NetworkVariable<bool>(false);

    [Header("Teams")]
    public MultiplayerTeam[] teams;

    [Header("Core")]
    public MultiplayerCoreAI coreAI;
    public Core core;
    public NetworkVariable<int> corePosition = new NetworkVariable<int>(0);
    public int sideSize;
    public Vector3[] coreMovements;
    public Vector3 allyGoalPosition;
    public Vector3 enemyGoalPosition;

    [Header("Action UI")]
    public Button[] uiButtons;
    public MultiplayerActionButton[] actionButtons;
    public UnityAction[] strikerActions = new UnityAction[7];

    [Header("Skill info lists")]
    public NetworkList<int> emptyInfo;
    public NetworkList<int> primaryInfo;
    public NetworkList<int> secondaryInfo;
    public NetworkList<int> ultimateInfo;
    public NetworkList<int> strikeInfo;

    [Header("Logs")]
    public MultiplayerLogHolder[] logHolders;

    [Header("Visuals")]
    public MultiplayerFigurine[] figs;
    public PlayerCard[] players;
    public MultiplayerActionBar blueTeamActionBar;
    public MultiplayerActionBar redTeamActionBar;
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
    public CinemachineVirtualCamera blueCamera;
    public CinemachineVirtualCamera redCamera;

    #region Network
    public override void OnNetworkSpawn()
    {
        Debug.Log("Spawned " + NetworkManager.Singleton.LocalClientId);
        if (IsServer)
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Se desconectó");

        if (IsServer)
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
    }
    #endregion

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        emptyInfo       = new NetworkList<int>();
        primaryInfo     = new NetworkList<int>();
        secondaryInfo   = new NetworkList<int>();
        ultimateInfo    = new NetworkList<int>();
        strikeInfo      = new NetworkList<int>();
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        BeginMatchServerRpc();
    }

    [ServerRpc]
    public void BeginMatchServerRpc()
    {
        StartCoroutine(PrepareGame());
    }

    #region Game Set
    private IEnumerator PrepareGame()
    {
        yield return new WaitForSeconds(0.25f);

        SetPlayerTypeClientRpc();
    }

    private void ContinuePreparingGame()
    {
        core = new Core(Position.Middle, areaSize);

        if (areaSize % 2 == 0)
            areaSize++;

        pointsToWin = areaSize + ((areaSize - 1) / 2);

        coreMovements = new Vector3[areaSize * 3 + 2];
        int aux = sideSize / (pointsToWin + 1);
        for (int i = 0; i < areaSize * 3 + 2; i++)
        {
            coreMovements[i] = new Vector3(-sideSize + aux * i, 0f, -5f);
        }
        allyGoalPosition = new Vector3(-sideSize - aux, 0f, 0f);
        enemyGoalPosition = new Vector3(sideSize + aux, 0f, 0f);

        Debug.Log(MultiplayerMatchData.Instance.blue_goalie);
        Debug.Log(MultiplayerMatchData.Instance.red_forward);

        // Blue Team
        players[0] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.blue_goalie.Value);
        players[1] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.blue_midfielder.Value);
        players[2] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.blue_forward.Value);

        //Strikers p1 = MatchData.current.goalie.selectedStriker;
        //Strikers p2 = MatchData.current.midfielder.selectedStriker;
        //Strikers p3 = MatchData.current.forward.selectedStriker;

        // Red Team
        players[3] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.red_goalie.Value);
        players[4] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.red_midfielder.Value);
        players[5] = Resources.Load<PlayerCard>("PlayerCards/" + MultiplayerMatchData.Instance.red_forward.Value);

        //Strikers p4 = MatchData.current.enemyGoalie.selectedStriker;
        //Strikers p5 = MatchData.current.enemyMidfielder.selectedStriker;
        //Strikers p6 = MatchData.current.enemyForward.selectedStriker;

        Strikers p1 = MultiplayerMatchData.Instance.blue_glStriker.Value;
        Strikers p2 = MultiplayerMatchData.Instance.blue_mfStriker.Value;
        Strikers p3 = MultiplayerMatchData.Instance.blue_fwStriker.Value;
        Strikers p4 = MultiplayerMatchData.Instance.red_glStriker.Value;
        Strikers p5 = MultiplayerMatchData.Instance.red_mfStriker.Value;
        Strikers p6 = MultiplayerMatchData.Instance.red_fwStriker.Value;

        teams = new MultiplayerTeam[2];

        // Blue Team
        teams[0] = new MultiplayerTeam(p1, p2, p3, players[0], players[1], players[2], figs[0], figs[1], figs[2], b_GoaliePos, b_MidPos, b_ForwardPos, 0, 1, this);
        teams[0].goalie.figurine.SetStrikerSpriteClientRpc(teams[0].goalie.character);
        teams[0].midfielder.figurine.SetStrikerSpriteClientRpc(teams[0].midfielder.character);
        teams[0].forward.figurine.SetStrikerSpriteClientRpc(teams[0].forward.character);

        // Red Team
        teams[1] = new MultiplayerTeam(p4, p5, p6, players[3], players[4], players[5], figs[3], figs[4], figs[5], r_GoaliePos, r_MidPos, r_ForwardPos, 1, 0, this);
        teams[1].goalie.figurine.SetStrikerSpriteClientRpc(teams[1].goalie.character);
        teams[1].midfielder.figurine.SetStrikerSpriteClientRpc(teams[1].midfielder.character);
        teams[1].forward.figurine.SetStrikerSpriteClientRpc(teams[1].forward.character);

        blueTeamActionBar.team = teams[0];
        redTeamActionBar.team = teams[1];

        blueTeamActionBar.SetUpActionBarClientRpc(teams[0].goalie.character, teams[0].midfielder.character, teams[0].forward.character);
        redTeamActionBar.SetUpActionBarClientRpc(teams[1].goalie.character, teams[1].midfielder.character, teams[1].forward.character);

        ////blueTeamActionBar.SetStrikerIcons();
        ////redTeamActionBar.SetStrikerIcons();
        //blueTeamActionBar.SetStrikerIconsClientRpc();
        //redTeamActionBar.SetStrikerIconsClientRpc();

        SetMapImageClientRpc((int)core.mapPosition);

        UnassignPlayerActionsClientRpc();
        //devTool.MatchReady();
        StartCoroutine(PlayTurnCoroutine());
    }

    private void PlayTurn()
    {
        //gameState = GameState.Running;
        turn.Value++;
        //turn++;
        corePosition.Value = core.position;

        teams[0].Turn();  // Blue Team
        teams[1].Turn();  // Red Team

        blueTeamActionBar.MoveAllyActionBar(); // Blue Team
        redTeamActionBar.MoveEnemyActionBar(); // Red Team

        highestActionMetter.Value = actionPoint - 1;
        isActionAchieved.Value = false;

        CheckActionMetters(0);  // Blue Team
        CheckActionMetters(1);  // Red Team

        if (isActionAchieved.Value)
        {

            logHolders[currentStriker.allyTeam].AddLogClientRpc(currentStriker.character, currentStriker.name + " turn", currentStriker.allyTeam);
            Debug.Log(currentStriker.name + " turn");

            currentStriker.PrepareAction();

            if (currentStriker.isAlive && !currentStriker.isStunned)
            {
                currentStriker.PreparePlayerActionAssign();

                DisplayPlayerTurnClientRpc(currentPlayerType.Value);
            }

            else
            {
                SetCurrentPlayerTypeServerRpc(MultiplayerPlayerType.None);
                DisplayPlayerTurnClientRpc(currentPlayerType.Value);

                logHolders[currentStriker.allyTeam].AddLogClientRpc(currentStriker.character, " is dead or stunned", currentStriker.allyTeam);
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
        //if (teams[i].goalie.actionMetter > actionPoint && teams[i].goalie.actionMetter > highestActionMetter.Value)
        //{
        //    SetCurrentStriker(teams[i].goalie, i);
        //}

        //if (teams[i].midfielder.actionMetter > actionPoint && teams[i].midfielder.actionMetter > highestActionMetter.Value)
        //{
        //    SetCurrentStriker(teams[i].midfielder, i);
        //}

        //if (teams[i].forward.actionMetter > actionPoint && teams[i].forward.actionMetter > highestActionMetter.Value)
        //{
        //    SetCurrentStriker(teams[i].forward, i);
        //}

        CheckPlayerMetter(teams[i].goalie);
        CheckPlayerMetter(teams[i].midfielder);
        CheckPlayerMetter(teams[i].forward);
    }

    private void CheckPlayerMetter(MultiplayerStriker striker)
    {
        if (striker.actionMetter > actionPoint)
        {
            if (striker.actionMetter > highestActionMetter.Value)
                SetCurrentStriker(striker, striker.allyTeam);

            else if (striker.actionMetter == highestActionMetter.Value)
            {
                int coin = Random.Range(0, 2);
                if (coin == 0)
                    SetCurrentStriker(striker, striker.allyTeam);
            }
        }
    }

    private void SetCurrentStriker(MultiplayerStriker stkr, int i)
    {
        currentStriker = stkr;
        highestActionMetter.Value = stkr.actionMetter;
        isActionAchieved.Value = true;

        if (isActionAchieved.Value)
        {
            if (i == 0) SetCurrentPlayerTypeServerRpc(MultiplayerPlayerType.Blue);
            else SetCurrentPlayerTypeServerRpc(MultiplayerPlayerType.Red);
        }
    }

    public void EndTurn()
    {
        core.CheckCorePosition();
        SetMapImageClientRpc((int)core.mapPosition);

        if (!isGameFinished.Value)
        {
            blueTeamActionBar.MoveAllyActionBar();
            redTeamActionBar.MoveEnemyActionBar();
        }

        Debug.Log(currentStriker.name + " turn end");

        UnassignPlayerActionsClientRpc();

        StartCoroutine(PlayTurnCoroutine());
    }

    private IEnumerator EndGameCoroutine()
    {
        //LobbyManager.Instance.AddScore(-300);

        yield return new WaitForSeconds(3f);

        NetworkManager.Singleton.SceneManager.LoadScene(Scenes.Menu.ToString(), LoadSceneMode.Single);
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
                return Rank.ProLeague;
        }
    }

    public void AssignPlayerActions(MultiplayerStriker striker)
    {
        UnassignPlayerActionsClientRpc();

        strikerActions[0] = striker.Evade;
        strikerActions[1] = striker.PrimarySkill;
        strikerActions[2] = striker.SecondarySkill;
        strikerActions[3] = striker.UltimateSkill;
        strikerActions[4] = striker.Strike;
        strikerActions[5] = delegate { striker.Move(-1); };
        strikerActions[6] = delegate { striker.Move(1); };

        actionButtons[0].AssignStrikerActionClientRpc(striker.character, ActionType.Evade, striker.s_evade.endsTurn);
        actionButtons[1].AssignStrikerActionClientRpc(striker.character, ActionType.Primary, striker.primary.endsTurn);
        actionButtons[2].AssignStrikerActionClientRpc(striker.character, ActionType.Secondary, striker.secondary.endsTurn);
        actionButtons[3].AssignStrikerActionClientRpc(striker.character, ActionType.Ultimate, striker.ultimate.endsTurn);
        actionButtons[4].AssignStrikerActionClientRpc(striker.character, ActionType.Strike, striker.s_strike.endsTurn);
        actionButtons[5].AssignStrikerActionClientRpc(striker.character, ActionType.MoveLeft, striker.s_move.endsTurn);
        actionButtons[6].AssignStrikerActionClientRpc(striker.character, ActionType.MoveRight, striker.s_move.endsTurn);

        EnableActionsClientRpc(currentPlayerType.Value, striker.s_evade.currentCooldown <= 0, 0);
        EnableActionsClientRpc(currentPlayerType.Value, striker.primary.currentCooldown <= 0, 1);
        EnableActionsClientRpc(currentPlayerType.Value, striker.secondary.currentCooldown <= 0, 2);
        EnableActionsClientRpc(currentPlayerType.Value, striker.ultimate.currentCooldown <= 0, 3);
        EnableActionsClientRpc(currentPlayerType.Value, striker.s_strike.currentCooldown <= 0, 4);
        EnableActionsClientRpc(currentPlayerType.Value, striker.s_move.currentCooldown <= 0 && striker.mapPosition != Position.Left, 5);
        EnableActionsClientRpc(currentPlayerType.Value, striker.s_move.currentCooldown <= 0 && striker.mapPosition != Position.Right, 6);
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
        switch (team)
        {
            case (0):
                core.position += ammount;
                break;

            case (1):
                core.position -= ammount;
                break;
        }

        corePosition.Value = core.position;

        if (corePosition.Value < -pointsToWin)
        {
            coreAI.SetDestinationClientRpc(allyGoalPosition);

            Debug.Log("Red Wins");
            isGameFinished.Value = true;

            UpdatePlayerLeaderboardClientRpc(MultiplayerPlayerType.Red);

            RedWinClientRpc();
            StartCoroutine(EndGameCoroutine());
        }

        else if (corePosition.Value > pointsToWin)
        {
            coreAI.SetDestinationClientRpc(enemyGoalPosition);

            Debug.Log("Blue Wins");
            isGameFinished.Value = true;

            UpdatePlayerLeaderboardClientRpc(MultiplayerPlayerType.Blue);

            BlueWinClientRpc();
            StartCoroutine(EndGameCoroutine());
        }

        else
            coreAI.SetDestinationClientRpc(coreMovements[(pointsToWin + 1) + corePosition.Value]);
    }

    public void PlayAction(int i)
    {
        PlayActionServerRpc(i);
    }
    #endregion

    #region RPCs
    [ServerRpc(RequireOwnership = false)]
    private void PlayActionServerRpc(int i)
    {
        strikerActions[i]();
    }

    [ClientRpc]
    private void BlueWinClientRpc()
    {
        blueCamera.gameObject.SetActive(true);
        visualUI.gameObject.SetActive(false);
    }
    
    [ClientRpc]
    private void RedWinClientRpc()
    {
        redCamera.gameObject.SetActive(true);
        visualUI.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void SetMapImageClientRpc(int i)
    {
        mapImage.sprite = mapSprites[i];
    }

    [ClientRpc]
    private void SetPlayerTypeClientRpc()
    {
        if (NetworkManager.Singleton.LocalClientId == 0 || IsServer)
        {
            localPlayerType = MultiplayerPlayerType.Blue;

            Debug.Log("Server is" + localPlayerType.ToString());

            SetPlayerReadyServerRpc(MultiplayerPlayerType.Blue);
        }

        else if (IsClient)
        {
            localPlayerType = MultiplayerPlayerType.Red;

            Debug.Log("Client is" + localPlayerType.ToString());

            SetPlayerReadyServerRpc(MultiplayerPlayerType.Red);
        }  
    }

    [ClientRpc]
    private void UnassignPlayerActionsClientRpc()
    {
        foreach (MultiplayerActionButton bt in actionButtons)
        {
            bt.DisableActionButton();
        }
    }

    [ServerRpc]
    public void ClearInfoListsServerRpc()
    {
        primaryInfo.Clear();
        secondaryInfo.Clear();
        ultimateInfo.Clear();
        strikeInfo.Clear();
    }

    [ServerRpc]
    private void SetCurrentPlayerTypeServerRpc(MultiplayerPlayerType type)
    {
        currentPlayerType.Value = type;
    }

    [ServerRpc]
    public void AddPrimaryInfoServerRpc(int data)
    {
        primaryInfo.Add(data);
    }

    [ServerRpc]
    public void AddSecondaryInfoServerRpc(int data)
    {
        secondaryInfo.Add(data);
    }

    [ServerRpc]
    public void AddUltimateInfoServerRpc(int data)
    {
        ultimateInfo.Add(data);
    }

    [ServerRpc]
    public void AddStrikeInfoServerRpc(int data)
    {
        strikeInfo.Add(data);
    }

    [ClientRpc]
    private void DisplayPlayerTurnClientRpc(MultiplayerPlayerType type)
    {
        if (type != MultiplayerPlayerType.None)
        {
            if (localPlayerType == type)
                turnText.text = "Choose an Action";
            else
                turnText.text = "Wait for the Opponent";
        }
        else turnText.text = "";

    }

    [ClientRpc]
    private void EnableActionsClientRpc(MultiplayerPlayerType type, bool isReady, int actionIndex)
    {
        if (localPlayerType == type)
        {
            if (isReady) actionButtons[actionIndex].EnableActionButton();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerAndStrikersServerRpc(string gl, string mf, string fw, MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
        {
            MultiplayerMatchData.Instance.blue_goalie.Value = gl;
            MultiplayerMatchData.Instance.blue_midfielder.Value = mf;
            MultiplayerMatchData.Instance.blue_forward.Value = fw;

            Debug.Log("Blue Team Set");
            SetPlayerReadyServerRpc(MultiplayerPlayerType.Blue);
        }

        else if (type == MultiplayerPlayerType.Red)
        {
            MultiplayerMatchData.Instance.red_goalie.Value = gl;
            MultiplayerMatchData.Instance.red_midfielder.Value = mf;
            MultiplayerMatchData.Instance.red_forward.Value = fw;
            
            Debug.Log("Red Team Set");
            SetPlayerReadyServerRpc(MultiplayerPlayerType.Red);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
            isBlueReady = true;
        else if (type == MultiplayerPlayerType.Red)
            isRedReady = true;

        if (isBlueReady && isRedReady)
            ContinuePreparingGame();
    }

    [ClientRpc]
    private void UpdatePlayerLeaderboardClientRpc(MultiplayerPlayerType winnerPlayer)
    {
        if (winnerPlayer == MultiplayerPlayerType.Blue)
        {
            if (localPlayerType == winnerPlayer)
            {
                LobbyManager.Instance.AddScore(
                20,
                MultiplayerMatchData.Instance.bluePlayer.Value.ToString(),
                MultiplayerMatchData.Instance.blue_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.blue_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.blue_forward.Value.ToString(),
                MultiplayerMatchData.Instance.blue_glStriker.Value,
                MultiplayerMatchData.Instance.blue_mfStriker.Value,
                MultiplayerMatchData.Instance.blue_fwStriker.Value);
            }
            else
            {
                LobbyManager.Instance.AddScore(
                -10,
                MultiplayerMatchData.Instance.redPlayer.Value.ToString(),
                MultiplayerMatchData.Instance.red_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.red_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.red_forward.Value.ToString(),
                MultiplayerMatchData.Instance.red_glStriker.Value,
                MultiplayerMatchData.Instance.red_mfStriker.Value,
                MultiplayerMatchData.Instance.red_fwStriker.Value);
            }
        }

        else if (winnerPlayer == MultiplayerPlayerType.Red)
        {
            if (localPlayerType == winnerPlayer)
            {
                LobbyManager.Instance.AddScore(
                20,
                MultiplayerMatchData.Instance.redPlayer.Value.ToString(),
                MultiplayerMatchData.Instance.red_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.red_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.red_forward.Value.ToString(),
                MultiplayerMatchData.Instance.red_glStriker.Value,
                MultiplayerMatchData.Instance.red_mfStriker.Value,
                MultiplayerMatchData.Instance.red_fwStriker.Value);
            }
            else
            {
                LobbyManager.Instance.AddScore(
                -10,
                MultiplayerMatchData.Instance.bluePlayer.Value.ToString(),
                MultiplayerMatchData.Instance.blue_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.blue_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.blue_forward.Value.ToString(),
                MultiplayerMatchData.Instance.blue_glStriker.Value,
                MultiplayerMatchData.Instance.blue_mfStriker.Value,
                MultiplayerMatchData.Instance.blue_fwStriker.Value);
            }
        }
    }
    #endregion
}
