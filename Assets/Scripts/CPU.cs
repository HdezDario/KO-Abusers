using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    public MatchManager mm;
    public Team team;

    private List<Action> listOfActions;
    private List<Skill> listOfSkills;

    [Header("Action")]
    private Action evadeAction;
    private Action primaryAction;
    private Action secondaryAction;
    private Action ultimateAction;
    private Action strikeAction;
    private Action moveLeftAction;
    private Action moveRightAction;

    [Header("Skills")]
    private Skill evadeSkill;
    private Skill primarySkill;
    private Skill secondarySkill;
    private Skill ultimateSkill;
    private Skill strikeSkill;
    private Skill moveSkill;

    private void OnEnable()
    {
        listOfActions   = new List<Action>();
        listOfSkills    = new List<Skill>();
    }

    public void AssignCPUActions(Striker striker)
    {
        evadeAction = striker.Evade;
        primaryAction = striker.PrimarySkill;
        secondaryAction = striker.SecondarySkill;
        ultimateAction = striker.UltimateSkill;
        strikeAction = striker.Strike;
        moveLeftAction = () => striker.Move(-1);
        moveLeftAction = () => striker.Move(1);

        evadeSkill      = striker.s_evade;
        primarySkill    = striker.primary;
        secondarySkill  = striker.secondary;
        ultimateSkill   = striker.ultimate;
        strikeSkill     = striker.s_strike;
        moveSkill       = striker.s_move;

        if (striker.s_evade.currentCooldown <= 0)
        {
            listOfActions.Add(striker.Evade);
            listOfSkills.Add(striker.s_evade);
        }

        if (striker.primary.currentCooldown <= 0)
        {
            listOfActions.Add(striker.PrimarySkill);
            listOfSkills.Add(striker.primary);
        }

        if (striker.secondary.currentCooldown <= 0)
        {
            listOfActions.Add(striker.SecondarySkill);
            listOfSkills.Add(striker.secondary);
        }
        if (striker.ultimate.currentCooldown <= 0)
        {
            listOfActions.Add(striker.UltimateSkill);
            listOfSkills.Add(striker.ultimate);
        }

        ChooseAction(striker);
    }

    private void ChooseAction(Striker striker)
    {
        int i;

        if (mm.IsTeamOnCore())
        {
            if (mm.IsTeamOutNumberedOnCore())
            {
                if (moveSkill.currentCooldown <= 0)
                {
                    if (striker.mapPosition != mm.core.mapPosition)
                    {
                        if (mm.IsCoreOnTheLeft(striker.mapPosition))
                            moveLeftAction();

                        else if (mm.IsCoreOnTheRight(striker.mapPosition))
                            moveRightAction();

                        StartCoroutine(ActionsCoroutine(striker));
                    }

                    else
                    {
                        if (mm.AreThereValidTargets(striker.mapPosition))
                        {
                            i = UnityEngine.Random.Range(0, listOfActions.Count);

                            if (!listOfSkills[i].endsTurn)
                            {
                                listOfActions[i]();

                                StartCoroutine(ActionsCoroutine(striker));
                            }

                            else listOfActions[i]();
                        }

                        else
                        {
                            if (mm.IsCoreAreaEmpty())
                                strikeAction();

                            else
                            {
                                if (evadeSkill.currentCooldown <= 0)
                                {
                                    i = UnityEngine.Random.Range(0, 4);

                                    if (i == 0)
                                        evadeAction();

                                    else strikeAction();
                                }

                                else strikeAction();
                            }
                        }
                    }
                }

                else
                {
                    if (mm.AreThereValidTargets(striker.mapPosition))
                    {
                        i = UnityEngine.Random.Range(0, listOfActions.Count);

                        if (!listOfSkills[i].endsTurn)
                        {
                            listOfActions[i]();

                            StartCoroutine(ActionsCoroutine(striker));
                        }

                        else listOfActions[i]();
                    }

                    else
                    {
                        if (mm.IsCoreAreaEmpty())
                            strikeAction();

                        else
                        {
                            if (evadeSkill.currentCooldown <= 0)
                            {
                                i = UnityEngine.Random.Range(0, 4);

                                if (i == 0)
                                    evadeAction();

                                else strikeAction();
                            }

                            else strikeAction();
                        }
                    }
                }
            }

            else
            {
                if (mm.IsMapAreaClear(0, striker.mapPosition))
                {
                    if (moveSkill.currentCooldown <= 0)
                    {
                        if (striker.mapPosition != mm.core.mapPosition)
                        {
                            if (mm.IsCoreOnTheLeft(striker.mapPosition))
                                moveLeftAction();

                            else if (mm.IsCoreOnTheRight(striker.mapPosition))
                                moveRightAction();

                            StartCoroutine(ActionsCoroutine(striker));
                        }

                        else
                        {
                            if (mm.AreThereValidTargets(striker.mapPosition))
                            {
                                i = UnityEngine.Random.Range(0, listOfActions.Count);

                                if (!listOfSkills[i].endsTurn)
                                {
                                    listOfActions[i]();

                                    StartCoroutine(ActionsCoroutine(striker));
                                }

                                else listOfActions[i]();
                            }

                            else
                            {
                                if (mm.IsCoreAreaEmpty())
                                    strikeAction();

                                else
                                {
                                    if (evadeSkill.currentCooldown <= 0)
                                    {
                                        i = UnityEngine.Random.Range(0, 4);

                                        if (i == 0)
                                            evadeAction();

                                        else strikeAction();
                                    }

                                    else strikeAction();
                                }
                            }
                        }
                    }

                    else
                    {
                        if (mm.AreThereValidTargets(striker.mapPosition))
                        {
                            i = UnityEngine.Random.Range(0, listOfActions.Count);

                            if (!listOfSkills[i].endsTurn)
                            {
                                listOfActions[i]();

                                StartCoroutine(ActionsCoroutine(striker));
                            }

                            else listOfActions[i]();
                        }

                        else
                        {
                            if (mm.IsCoreAreaEmpty())
                                strikeAction();

                            else
                            {
                                if (evadeSkill.currentCooldown <= 0)
                                {
                                    i = UnityEngine.Random.Range(0, 4);

                                    if (i == 0)
                                        evadeAction();

                                    else strikeAction();
                                }

                                else strikeAction();
                            }
                        }
                    }
                }

                else // Search Targets
                {
                    if (mm.AreThereValidTargets(striker.mapPosition))
                    {
                        i = UnityEngine.Random.Range(0, listOfActions.Count);

                        if (!listOfSkills[i].endsTurn)
                        {
                            listOfActions[i]();

                            StartCoroutine(ActionsCoroutine(striker));
                        }

                        else listOfActions[i]();
                    }

                    else
                    {
                        if (mm.IsCoreAreaEmpty())
                            strikeAction();

                        else
                        {
                            if (evadeSkill.currentCooldown <= 0)
                            {
                                i = UnityEngine.Random.Range(0, 4);

                                if (i == 0)
                                    evadeAction();

                                else strikeAction();
                            }

                            else strikeAction();
                        }
                    }
                }
            }
        }

        else
        {
            if (moveSkill.currentCooldown <= 0)
            {
                if (striker.mapPosition != mm.core.mapPosition)
                {
                    if (mm.IsCoreOnTheLeft(striker.mapPosition))
                        moveLeftAction();

                    else if (mm.IsCoreOnTheRight(striker.mapPosition))
                        moveRightAction();

                    StartCoroutine(ActionsCoroutine(striker));
                }

                else
                {
                    if (mm.AreThereValidTargets(striker.mapPosition))
                    {
                        i = UnityEngine.Random.Range(0, listOfActions.Count);

                        if (!listOfSkills[i].endsTurn)
                        {
                            listOfActions[i]();

                            StartCoroutine(ActionsCoroutine(striker));
                        }

                        else listOfActions[i]();
                    }

                    else
                    {
                        if (mm.IsCoreAreaEmpty())
                            strikeAction();

                        else
                        {
                            if (evadeSkill.currentCooldown <= 0)
                            {
                                i = UnityEngine.Random.Range(0, 4);

                                if (i == 0)
                                    evadeAction();

                                else strikeAction();
                            }

                            else strikeAction();
                        }
                    }
                }
            }

            else
            {
                if (mm.AreThereValidTargets(striker.mapPosition))
                {
                    i = UnityEngine.Random.Range(0, listOfActions.Count);

                    if (!listOfSkills[i].endsTurn)
                    {
                        listOfActions[i]();

                        StartCoroutine(ActionsCoroutine(striker));
                    }

                    else listOfActions[i]();
                }

                else
                {
                    if (mm.IsCoreAreaEmpty())
                        strikeAction();

                    else
                    {
                        if (evadeSkill.currentCooldown <= 0)
                        {
                            i = UnityEngine.Random.Range(0, 4);

                            if (i == 0)
                                evadeAction();

                            else strikeAction();
                        }

                        else strikeAction();
                    }
                }
            }
        }
    }

    private IEnumerator ActionsCoroutine(Striker stkr)
    {
        yield return new WaitForSeconds(0.65f);

        AssignCPUActions(stkr);
    }

    public void RandomCPUActions(Striker stkr)
    {
        List<Action> actions = new List<Action>();
        List<Skill> skills = new List<Skill>();

        if (stkr.s_evade.currentCooldown <= 0)
        {
            actions.Add(stkr.Evade);
            skills.Add(stkr.s_evade);
        }

        if (stkr.primary.currentCooldown <= 0)
        {
            actions.Add(stkr.PrimarySkill);
            skills.Add(stkr.primary);
        }

        if (stkr.secondary.currentCooldown <= 0)
        {
            actions.Add(stkr.SecondarySkill);
            skills.Add(stkr.secondary);
        }
        if (stkr.ultimate.currentCooldown <= 0)
        {
            actions.Add(stkr.UltimateSkill);
            skills.Add(stkr.ultimate);
        }
        if (stkr.s_strike.currentCooldown <= 0)
        {
            actions.Add(stkr.Strike);
            skills.Add(stkr.s_strike);
        }

        if (stkr.s_move.currentCooldown <= 0)
        {
            if (stkr.mapPosition != Position.Left)
                actions.Add(delegate { stkr.Move(-1); });
            else if (stkr.mapPosition != Position.Right)
                actions.Add(delegate { stkr.Move(1); });

            skills.Add(stkr.s_move);
        }

        int i = UnityEngine.Random.Range(0, actions.Count);

        if (!skills[i].endsTurn)
        {
            actions[i]();

            StartCoroutine(RandomActionsCoroutine(stkr));
        }

        else actions[i]();
    }

    private IEnumerator RandomActionsCoroutine(Striker stkr)
    {
        yield return new WaitForSeconds(0.65f);

        RandomCPUActions(stkr);
    }

    //private void DoAction(int i, List<Skill> skills, List<Action> actions)
    //{
    //    if (!skills[i].endsTurn)
    //    {
    //        actions[i]();

    //        actions.Remove(actions[i]);
    //        skills.Remove(skills[i]);

    //        i = UnityEngine.Random.Range(0, actions.Count);

    //        DoAction(i, skills, actions);
    //    }

    //    else actions[i]();
    //}
}
