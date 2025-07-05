using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MultiplayerTeamSelectManager : NetworkBehaviour
{
    [Header("Network")]
    private bool isBlueReady = false;
    private bool isRedReady = false;

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

    [Header("Players")]
    [SerializeField] private PlayerCard goalie;
    [SerializeField] private PlayerCard midfielder;
    [SerializeField] private PlayerCard forward;
    [SerializeField] private int glIndex;
    [SerializeField] private int mfIndex;
    [SerializeField] private int fwIndex;

    [Header("Start Button")]
    [SerializeField] private Button startButton;

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }

    private void Awake()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (IsServer)
            StartServerRpc();
    }

    [ServerRpc]
    private void StartServerRpc()
    {
        StartCoroutine(BeginCoroutine());
    }

    private IEnumerator BeginCoroutine()
    {
        yield return new WaitForSeconds(1f);

        GetPlayerTypeClientRpc();
    }

    public void StartMatch()
    {
        startButton.interactable = false;

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            PlayerMatchReadyServerRpc(
                MultiplayerPlayerData.current.glStriker,
                MultiplayerPlayerData.current.mfStriker,
                MultiplayerPlayerData.current.fwStriker,
                MultiplayerPlayerType.Blue);
        }

        else
        {
            PlayerMatchReadyServerRpc(
                MultiplayerPlayerData.current.glStriker,
                MultiplayerPlayerData.current.mfStriker,
                MultiplayerPlayerData.current.fwStriker,
                MultiplayerPlayerType.Red);
        }
    }

    public void SetGoalieSelectedStriker(int value)
    {
        glIndex += value;

        if (glIndex >= goalie.roster.Length)
            glIndex = 0;
        else if (glIndex < 0)
            glIndex = goalie.roster.Length - 1;

        MultiplayerPlayerData.current.glStriker = goalie.roster[glIndex];

        glImage.sprite = SetStrikerSprite(MultiplayerPlayerData.current.glStriker);

        CheckStartConditions();
    }

    public void SetMidfielderSelectedStriker(int value)
    {
        mfIndex += value;

        if (mfIndex >= midfielder.roster.Length)
            mfIndex = 0;
        else if (mfIndex < 0)
            mfIndex = midfielder.roster.Length - 1;

        MultiplayerPlayerData.current.mfStriker = midfielder.roster[mfIndex];

        mfImage.sprite = SetStrikerSprite(MultiplayerPlayerData.current.mfStriker);

        CheckStartConditions();
    }

    public void SetForwardSelectedStriker(int value)
    {
        fwIndex += value;

        if (fwIndex >= forward.roster.Length)
            fwIndex = 0;
        else if (fwIndex < 0)
            fwIndex = forward.roster.Length - 1;

        MultiplayerPlayerData.current.fwStriker = forward.roster[fwIndex];

        fwImage.sprite = SetStrikerSprite(MultiplayerPlayerData.current.fwStriker);

        CheckStartConditions();
    }

    private void CheckStartConditions()
    {
        if ((MultiplayerPlayerData.current.glStriker == MultiplayerPlayerData.current.mfStriker) ||
            (MultiplayerPlayerData.current.glStriker == MultiplayerPlayerData.current.fwStriker) ||
            (MultiplayerPlayerData.current.mfStriker == MultiplayerPlayerData.current.fwStriker))
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

    private void SetPlayers(string gl, string mf, string fw)
    {
        goalie      = Resources.Load<PlayerCard>("PlayerCards/" + gl);
        midfielder  = Resources.Load<PlayerCard>("PlayerCards/" + mf);
        forward     = Resources.Load<PlayerCard>("PlayerCards/" + fw);

        MultiplayerPlayerData.current.glStriker = goalie.roster[0];
        MultiplayerPlayerData.current.mfStriker = midfielder.roster[0];
        MultiplayerPlayerData.current.fwStriker = forward.roster[0];

        glImage.sprite = SetStrikerSprite(goalie.roster[0]);
        mfImage.sprite = SetStrikerSprite(midfielder.roster[0]);
        fwImage.sprite = SetStrikerSprite(forward.roster[0]);

        PrepareButtons(goalie, glButtons);
        PrepareButtons(midfielder, mfButtons);
        PrepareButtons(forward, fwButtons);

        glName.text = goalie.playerName;
        mfName.text = midfielder.playerName;
        fwName.text = forward.playerName;

        CheckStartConditions();
    }

    #region Rpc
    [ClientRpc]
    private void GetPlayerTypeClientRpc()
    {
        SetPlayers(
            MultiplayerPlayerData.current.goalie,
            MultiplayerPlayerData.current.midfielder,
            MultiplayerPlayerData.current.forward);

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            SetPlayersServerRpc(
                MultiplayerPlayerData.current.playerName,
                MultiplayerPlayerData.current.goalie,
                MultiplayerPlayerData.current.midfielder,
                MultiplayerPlayerData.current.forward,
                MultiplayerPlayerType.Blue);
        }  

        else
        {
            SetPlayersServerRpc(
                MultiplayerPlayerData.current.playerName,
                MultiplayerPlayerData.current.goalie,
                MultiplayerPlayerData.current.midfielder,
                MultiplayerPlayerData.current.forward,
                MultiplayerPlayerType.Red);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayersServerRpc(string playerName, string gl, string mf, string fw, MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
        {
            MultiplayerMatchData.Instance.SetPlayersServerRpc(playerName, gl, mf, fw, MultiplayerPlayerType.Blue);

            Debug.Log("Blue Team Set");
            SetPlayerReadyServerRpc(MultiplayerPlayerType.Blue);
        }

        else if (type == MultiplayerPlayerType.Red)
        {
            MultiplayerMatchData.Instance.SetPlayersServerRpc(playerName, gl, mf, fw, MultiplayerPlayerType.Red);

            Debug.Log("Red Team Set");
            SetPlayerReadyServerRpc(MultiplayerPlayerType.Red);
        }
    }

    [ServerRpc]
    private void SetPlayerReadyServerRpc(MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
            isBlueReady = true;
        else if (type == MultiplayerPlayerType.Red)
            isRedReady = true;

        if (isBlueReady && isRedReady)
        {
            isBlueReady = false;
            isRedReady = false;

            Debug.Log(MultiplayerMatchData.Instance.blue_goalie.Value);
            Debug.Log(MultiplayerMatchData.Instance.blue_midfielder.Value);
            Debug.Log(MultiplayerMatchData.Instance.blue_forward.Value);
            Debug.Log(MultiplayerMatchData.Instance.red_goalie.Value);
            Debug.Log(MultiplayerMatchData.Instance.red_midfielder.Value);
            Debug.Log(MultiplayerMatchData.Instance.red_forward.Value);

            PlayIntroClientRpc(
                MultiplayerMatchData.Instance.blue_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.blue_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.blue_forward.Value.ToString(),
                MultiplayerMatchData.Instance.red_goalie.Value.ToString(),
                MultiplayerMatchData.Instance.red_midfielder.Value.ToString(),
                MultiplayerMatchData.Instance.red_forward.Value.ToString());
        }   
    }

    [ClientRpc]
    private void PlayIntroClientRpc(string b_Gl, string b_Mf, string b_Fw, string r_Gl, string r_Mf, string r_Fw)
    {
        allyTeamDisplay[0].cardName = b_Gl;
        allyTeamDisplay[1].cardName = b_Mf;
        allyTeamDisplay[2].cardName = b_Fw;

        enemyTeamDisplay[0].cardName = r_Gl;
        enemyTeamDisplay[1].cardName = r_Mf;
        enemyTeamDisplay[2].cardName = r_Fw;

        foreach (CardDisplay display in allyTeamDisplay)
            display.DisplayPlayerCard();

        foreach (CardDisplay display in enemyTeamDisplay)
            display.DisplayPlayerCard();

        intro.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerMatchReadyServerRpc(Strikers gl, Strikers mf, Strikers fw, MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
        {
            MultiplayerMatchData.Instance.blue_glStriker.Value = gl;
            MultiplayerMatchData.Instance.blue_mfStriker.Value = mf;
            MultiplayerMatchData.Instance.blue_fwStriker.Value = fw;

            Debug.Log("Blue Team Set");
            isBlueReady = true;
        }

        else if (type == MultiplayerPlayerType.Red)
        {
            MultiplayerMatchData.Instance.red_glStriker.Value = gl;
            MultiplayerMatchData.Instance.red_mfStriker.Value = mf;
            MultiplayerMatchData.Instance.red_fwStriker.Value = fw;

            Debug.Log("Red Team Set");
            isRedReady = true;
        }

        if (isBlueReady && isRedReady)
        {
            StartCoroutine(MatchCoroutine());
        }
    }

    private IEnumerator MatchCoroutine()
    {
        yield return new WaitForSeconds(3f);

        NetworkManager.Singleton.SceneManager.LoadScene(Scenes.MultiplayerMatch.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    #endregion
}
