using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpSpeedster : MultiplayerStriker
{
    private bool isFlowState;
    private int speedsterBuff;

    public MpSpeedster(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 1, 2, true);
        secondary   = new Skill(6, 0, 2, false);
        ultimate    = new Skill(6, 1, 4, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Speedster;

        isFlowState = false;
        speedsterBuff = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.skillPower * 3 + power, primary.corePush, 2 };
        skillInformation[2] = new object[] { secondary.skillPower * 7 + power };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.corePush, 4 };
        skillInformation[4] = new object[] { s_strike.corePush, 1 };

        base.PreparePlayerActionAssign();
    }

    protected override void ResetBuffs()
    {
        if (!isAlive)
        {
            isFlowState = false;
            speedsterBuff = 0;

            base.ResetBuffs();
        }

        else
        {
            base.ResetBuffs();

            if (isFlowState)
            {
                Buff(speedsterBuff, BuffType.Speed);
                figurine.SetSpeedClientRpc(speed + buffSpeed);
            }
        }
    }

    public override void Strike()
    {
        if (isFlowState)
            speedsterBuff += 1;

        base.Strike();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        m_matchManager.Slow(enemyTeam, primary.skillPower * 3 + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        if (isFlowState)
            speedsterBuff += 2;

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        Accelerate(secondary.skillPower * 7 + power);

        isFlowState = true;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        m_matchManager.Stun(enemyTeam, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, ultimate.corePush);

        if (isFlowState)
            speedsterBuff += 4;

        base.UltimateSkill();
    }
}
