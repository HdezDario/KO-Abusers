using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class MultiplayerMatchData : NetworkBehaviour
{
    public static MultiplayerMatchData Instance { get; private set; }

    // Blue Team
    public NetworkVariable<FixedString64Bytes> bluePlayer = new NetworkVariable<FixedString64Bytes>("");

    public NetworkVariable<FixedString64Bytes> blue_goalie = new NetworkVariable<FixedString64Bytes>("");
    public NetworkVariable<FixedString64Bytes> blue_midfielder = new NetworkVariable<FixedString64Bytes>("");
    public NetworkVariable<FixedString64Bytes> blue_forward = new NetworkVariable<FixedString64Bytes>("");

    public NetworkVariable<Strikers> blue_glStriker = new NetworkVariable<Strikers>(Strikers.Hamster);
    public NetworkVariable<Strikers> blue_mfStriker = new NetworkVariable<Strikers>(Strikers.Sniper);
    public NetworkVariable<Strikers> blue_fwStriker = new NetworkVariable<Strikers>(Strikers.Kicker);

    // Red Team
    public NetworkVariable<FixedString64Bytes> redPlayer = new NetworkVariable<FixedString64Bytes>("");

    public NetworkVariable<FixedString64Bytes> red_goalie = new NetworkVariable<FixedString64Bytes>("");
    public NetworkVariable<FixedString64Bytes> red_midfielder = new NetworkVariable<FixedString64Bytes>("");
    public NetworkVariable<FixedString64Bytes> red_forward = new NetworkVariable<FixedString64Bytes>("");

    public NetworkVariable<Strikers> red_glStriker = new NetworkVariable<Strikers>(Strikers.Hamster);
    public NetworkVariable<Strikers> red_mfStriker = new NetworkVariable<Strikers>(Strikers.Hamster);
    public NetworkVariable<Strikers> red_fwStriker = new NetworkVariable<Strikers>(Strikers.Hamster);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayersServerRpc(string playerName, string gl, string mf, string fw, MultiplayerPlayerType type)
    {
        if (type == MultiplayerPlayerType.Blue)
        {
            bluePlayer.Value = new FixedString64Bytes(playerName);

            blue_goalie.Value = new FixedString64Bytes(gl);
            blue_midfielder.Value = new FixedString64Bytes(mf);
            blue_forward.Value = new FixedString64Bytes(fw);
        }

        else if (type == MultiplayerPlayerType.Red)
        {
            redPlayer.Value = new FixedString64Bytes(playerName);

            red_goalie.Value = new FixedString64Bytes(gl);
            red_midfielder.Value = new FixedString64Bytes(mf);
            red_forward.Value = new FixedString64Bytes(fw);
        }

        Debug.Log(gl + " " + mf + " " + fw);
    }
}
