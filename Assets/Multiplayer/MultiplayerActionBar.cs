using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.Netcode;

public class MultiplayerActionBar : NetworkBehaviour
{
    [Header("Team")]
    [SerializeField] private int t;
    public Image goalie;
    public Image midfielder;
    public Image forward;
    public MultiplayerTeam team;

    [Space(10)]
    public NetworkVariable<Vector3> goaliePos = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> midfielderPos = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> forwardPos = new NetworkVariable<Vector3>();

    [Header("Variables")]
    public float time;
    public float size;
    public float step;

    void Start()
    {
        size = this.GetComponent<RectTransform>().sizeDelta.x / 2;
        step = size / 50;
    }

    public override void OnNetworkSpawn()
    {
        SetTeamBars(t);
        base.OnNetworkSpawn();
    }

    public void MoveAllyActionBar()
    {
        //Goalie
        if (team.goalie.actionMetter > 100)
            goaliePos.Value = new Vector3(size, goaliePos.Value.y, goaliePos.Value.z);
        else
            goaliePos.Value = new Vector3(-size + step * team.goalie.actionMetter, goaliePos.Value.y, goaliePos.Value.z);

        //Midfielder
        if (team.midfielder.actionMetter > 100)
            midfielderPos.Value = new Vector3(size, midfielderPos.Value.y, midfielderPos.Value.z);
        else
            midfielderPos.Value = new Vector3(-size + step * team.midfielder.actionMetter, midfielderPos.Value.y, midfielderPos.Value.z);

        //Forward
        if (team.forward.actionMetter > 100)
            forwardPos.Value = new Vector3(size, forwardPos.Value.y, forwardPos.Value.z);
        else
            forwardPos.Value = new Vector3(-size + step * team.forward.actionMetter, forwardPos.Value.y, forwardPos.Value.z);


        goalie.transform.DOLocalMove(goaliePos.Value, time);
        midfielder.transform.DOLocalMove(midfielderPos.Value, time);
        forward.transform.DOLocalMove(forwardPos.Value, time);
    }

    public void MoveEnemyActionBar()
    {
        //Goalie
        if (team.goalie.actionMetter > 100)
            goaliePos.Value = new Vector3(-size, goaliePos.Value.y, goaliePos.Value.z);
        else
            goaliePos.Value = new Vector3(size - step * team.goalie.actionMetter, goaliePos.Value.y, goaliePos.Value.z);

        //Midfielder
        if (team.midfielder.actionMetter > 100)
            midfielderPos.Value = new Vector3(-size, midfielderPos.Value.y, midfielderPos.Value.z);
        else
            midfielderPos.Value = new Vector3(size - step * team.midfielder.actionMetter, midfielderPos.Value.y, midfielderPos.Value.z);

        //Forward
        if (team.forward.actionMetter > 100)
            forwardPos.Value = new Vector3(-size, forwardPos.Value.y, forwardPos.Value.z);
        else
            forwardPos.Value = new Vector3(size - step * team.forward.actionMetter, forwardPos.Value.y, forwardPos.Value.z);


        goalie.transform.DOLocalMove(goaliePos.Value, time);
        midfielder.transform.DOLocalMove(midfielderPos.Value, time);
        forward.transform.DOLocalMove(forwardPos.Value, time);
    }

    private void SetTeamBars(int t) 
    {
        if (t == 0)
        {
            goaliePos.Value = new Vector3(-250, 37.5f, 0);
            midfielderPos.Value = new Vector3(-250, 37.5f, 0);
            forwardPos.Value = new Vector3(-250, 37.5f, 0);
        }

        else
        {
            goaliePos.Value = new Vector3(250, 37.5f, 0);
            midfielderPos.Value = new Vector3(250, 37.5f, 0);
            forwardPos.Value = new Vector3(250, 37.5f, 0);
        }
        
        goalie.transform.localPosition = goaliePos.Value;
        midfielder.transform.localPosition = midfielderPos.Value;
        forward.transform.localPosition = forwardPos.Value;
    }
    
    [ClientRpc]
    public void SetUpActionBarClientRpc(Strikers goalieStriker, Strikers midfielderStriker, Strikers forwardStriker)
    {
        goalie.sprite = Resources.Load<Sprite>("CharacterIcons/" + goalieStriker + "_icon");
        midfielder.sprite = Resources.Load<Sprite>("CharacterIcons/" + midfielderStriker + "_icon");
        forward.sprite = Resources.Load<Sprite>("CharacterIcons/" + forwardStriker + "_icon");

        Debug.Log("Setting ActionBar");
    }
}
