using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatSystemDebugTool : MonoBehaviour
{
    [Header("Match Manager")]
    public MatchManager mm;
    public bool isGameStarted;
    public TextMeshProUGUI currentTurn;
    public TextMeshProUGUI coreInfo;

    [Header("Blue Team")]
    public TextMeshProUGUI b_gl;
    public TextMeshProUGUI b_glStats;
    public TextMeshProUGUI b_mf;
    public TextMeshProUGUI b_mfStats;
    public TextMeshProUGUI b_fw;
    public TextMeshProUGUI b_fwStats;

    [Header("Red Team")]
    public TextMeshProUGUI r_gl;
    public TextMeshProUGUI r_glStats;
    public TextMeshProUGUI r_mf;
    public TextMeshProUGUI r_mfStats;
    public TextMeshProUGUI r_fw;
    public TextMeshProUGUI r_fwStats;

    public void MatchReady()
    {
        b_gl.text = mm.teams[0].goalie.name;
        b_mf.text = mm.teams[0].midfielder.name;
        b_fw.text = mm.teams[0].forward.name;

        r_gl.text = mm.teams[1].goalie.name;
        r_mf.text = mm.teams[1].midfielder.name;
        r_fw.text = mm.teams[1].forward.name;

        isGameStarted = true;
    }

    public void CurrentTurnDisplay(Striker stkr)
    {
        currentTurn.text = stkr.name + "'s Turn";
    }

    void Update()
    {
        if (isGameStarted)
        {
            b_glStats.text =
                "Action: " + mm.teams[0].goalie.actionMetter + "\n" +
                "Alive: " + mm.teams[0].goalie.isAlive + "\n" +
                "Evade: " + mm.teams[0].goalie.isEvading + "\n" +
                "Stunned: " + mm.teams[0].goalie.isStunned + "\n" +
                "Recover: " + mm.teams[0].goalie.isRecovered + "\n" +
                "Pos: " + mm.teams[0].goalie.mapPosition + "\n" +
                "HP: " + mm.teams[0].goalie.health + "\n" +
                "Power: " + mm.teams[0].goalie.power + "\n" +
                "Speed: " + mm.teams[0].goalie.speed + "\n" +
                "Primary: " + mm.teams[0].goalie.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[0].goalie.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[0].goalie.ultimate.currentCooldown;

            b_mfStats.text =
                "Action: " + mm.teams[0].midfielder.actionMetter + "\n" +
                "Alive: " + mm.teams[0].midfielder.isAlive + "\n" +
                "Evade: " + mm.teams[0].midfielder.isEvading + "\n" +
                "Stunned: " + mm.teams[0].midfielder.isStunned + "\n" +
                "Recover: " + mm.teams[0].midfielder.isRecovered + "\n" +
                "Pos: " + mm.teams[0].midfielder.mapPosition + "\n" +
                "HP: " + mm.teams[0].midfielder.health + "\n" +
                "Power: " + mm.teams[0].midfielder.power + "\n" +
                "Speed: " + mm.teams[0].midfielder.speed + "\n" +
                "Primary: " + mm.teams[0].midfielder.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[0].midfielder.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[0].midfielder.ultimate.currentCooldown;

            b_fwStats.text =
                "Action: " + mm.teams[0].forward.actionMetter + "\n" +
                "Alive: " + mm.teams[0].forward.isAlive + "\n" +
                "Evade: " + mm.teams[0].forward.isEvading + "\n" +
                "Stunned: " + mm.teams[0].forward.isStunned + "\n" +
                "Recover: " + mm.teams[0].forward.isRecovered + "\n" +
                "Pos: " + mm.teams[0].forward.mapPosition + "\n" +
                "HP: " + mm.teams[0].forward.health + "\n" +
                "Power: " + mm.teams[0].forward.power + "\n" +
                "Speed: " + mm.teams[0].forward.speed + "\n" +
                "Primary: " + mm.teams[0].forward.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[0].forward.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[0].forward.ultimate.currentCooldown;

            r_glStats.text =
                "Action: " + mm.teams[1].goalie.actionMetter + "\n" +
                "Alive: " + mm.teams[1].goalie.isAlive + "\n" +
                "Evade: " + mm.teams[1].goalie.isEvading + "\n" +
                "Stunned: " + mm.teams[1].goalie.isStunned + "\n" +
                "Recover: " + mm.teams[1].goalie.isRecovered + "\n" +
                "Pos: " + mm.teams[1].goalie.mapPosition + "\n" +
                "HP: " + mm.teams[1].goalie.health + "\n" +
                "Power: " + mm.teams[1].goalie.power + "\n" +
                "Speed: " + mm.teams[1].goalie.speed + "\n" +
                "Primary: " + mm.teams[1].goalie.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[1].goalie.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[1].goalie.ultimate.currentCooldown;

            r_mfStats.text =
                "Action: " + mm.teams[1].midfielder.actionMetter + "\n" +
                "Alive: " + mm.teams[1].midfielder.isAlive + "\n" +
                "Evade: " + mm.teams[1].midfielder.isEvading + "\n" +
                "Stunned: " + mm.teams[1].midfielder.isStunned + "\n" +
                "Recover: " + mm.teams[1].midfielder.isRecovered + "\n" +
                "Pos: " + mm.teams[1].midfielder.mapPosition + "\n" +
                "HP: " + mm.teams[1].midfielder.health + "\n" +
                "Power: " + mm.teams[1].midfielder.power + "\n" +
                "Speed: " + mm.teams[1].midfielder.speed + "\n" +
                "Primary: " + mm.teams[1].midfielder.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[1].midfielder.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[1].midfielder.ultimate.currentCooldown;

            r_fwStats.text =
                "Action: " + mm.teams[1].forward.actionMetter + "\n" +
                "Alive: " + mm.teams[1].forward.isAlive + "\n" +
                "Evade: " + mm.teams[1].forward.isEvading + "\n" +
                "Stunned: " + mm.teams[1].forward.isStunned + "\n" +
                "Recover: " + mm.teams[1].forward.isRecovered + "\n" +
                "Pos: " + mm.teams[1].forward.mapPosition + "\n" +
                "HP: " + mm.teams[1].forward.health + "\n" +
                "Power: " + mm.teams[1].forward.power + "\n" +
                "Speed: " + mm.teams[1].forward.speed + "\n" +
                "Primary: " + mm.teams[1].forward.primary.currentCooldown + "\n" +
                "Secondary: " + mm.teams[1].forward.secondary.currentCooldown + "\n" +
                "Ultimate: " + mm.teams[1].forward.ultimate.currentCooldown;

                coreInfo.text = "Position: " + mm.core.position + " Map: " + mm.core.mapPosition;
        }
    }
}
