using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionBar : MonoBehaviour
{
    [Header("Team")]
    public Image goalie;
    public Image midfielder;
    public Image forward;
    public Team team;

    [Space(10)]
    public Vector3 goaliePos;
    public Vector3 midfielderPos;
    public Vector3 forwardPos;

    [Header("Variables")]
    public float time;
    public float size;
    public float step;

    void Start()
    {
        size = this.GetComponent<RectTransform>().sizeDelta.x / 2;
        step = size / 50;
    }

    public void MoveAllyActionBar()
    {
        //Goalie
        if (team.goalie.actionMetter > 100)
            goaliePos.x = size;
        else
            goaliePos.x = -size + step * team.goalie.actionMetter;

        //Midfielder
        if (team.midfielder.actionMetter > 100)
            midfielderPos.x = size;
        else
            midfielderPos.x = -size + step * team.midfielder.actionMetter;

        //Forward
        if (team.forward.actionMetter > 100)
            forwardPos.x = size;
        else
            forwardPos.x = -size + step * team.forward.actionMetter;


        goalie.transform.DOLocalMove(goaliePos, time);
        midfielder.transform.DOLocalMove(midfielderPos, time);
        forward.transform.DOLocalMove(forwardPos, time);
    }

    public void MoveEnemyActionBar()
    {
        //Goalie
        if (team.goalie.actionMetter > 100)
            goaliePos.x = -size;
        else
            goaliePos.x = size - step * team.goalie.actionMetter;

        //Midfielder
        if (team.midfielder.actionMetter > 100)
            midfielderPos.x = -size;
        else
            midfielderPos.x = size - step * team.midfielder.actionMetter;

        //Forward
        if (team.forward.actionMetter > 100)
            forwardPos.x = -size;
        else
            forwardPos.x = size - step * team.forward.actionMetter;


        goalie.transform.DOLocalMove(goaliePos, time);
        midfielder.transform.DOLocalMove(midfielderPos, time);
        forward.transform.DOLocalMove(forwardPos, time);
    }

    public void SetStrikerIcons()
    {
        goalie.sprite = Resources.Load<Sprite>("CharacterIcons/" + team.goalie.character + "_icon");
        midfielder.sprite = Resources.Load<Sprite>("CharacterIcons/" + team.midfielder.character + "_icon");
        forward.sprite = Resources.Load<Sprite>("CharacterIcons/" + team.forward.character + "_icon");
    }
}
