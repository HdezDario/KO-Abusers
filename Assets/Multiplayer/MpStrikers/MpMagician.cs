using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpMagician : MultiplayerStriker
{
    private int magicianBuff;

    public MpMagician(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 2, 4, true);
        secondary   = new Skill(4, 1, 2, false);
        ultimate    = new Skill(10, 3, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Magician;

        magicianBuff = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + magicianBuff + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power, -ultimate.corePush };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    protected override void ResetBuffs()
    {
        magicianBuff = 0;

        base.ResetBuffs();
    }

    public override void PrimarySkill()
    {
        Position hitArea = Position.Null;

        switch (allyTeam)
        {
            case (0):
                hitArea = Position.Right;
                for (int i = (int)mapPosition; i < 3; i++)
                {
                    if (!m_matchManager.IsMapAreaClear(enemyTeam, i))
                    {
                        hitArea = m_matchManager.GetAreaByIndex(i);
                        break;
                    }
                }
                break;

            case (1):
                hitArea = Position.Left;
                for (int i = (int)mapPosition; i >= 0; i--)
                {
                    if (!m_matchManager.IsMapAreaClear(enemyTeam, i))
                    {
                        hitArea = m_matchManager.GetAreaByIndex(i);
                        break;
                    }
                }
                break;
        }

        m_matchManager.Damage(enemyTeam, primary.skillPower + magicianBuff + power, hitArea);
        if (m_matchManager.core.mapPosition == hitArea)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        if (!m_matchManager.IsMapAreaClear(enemyTeam, mapPosition))
            magicianBuff = 2 + power;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, m_matchManager.core.mapPosition);
        m_matchManager.Stun(enemyTeam, m_matchManager.core.mapPosition);

        m_matchManager.PushCore(enemyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
