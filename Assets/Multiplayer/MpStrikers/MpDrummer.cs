using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpDrummer : MultiplayerStriker
{
    private bool isPlayingDrumms;
    private int drummerCharges;

    public MpDrummer(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 20;
        baseSpeed   = 10;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(5, 2, 2, true);
        ultimate    = new Skill(7, 0, 4, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Drummer;

        isPlayingDrumms = false;
        drummerCharges = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush, 1 };
        skillInformation[2] = new object[] { secondary.skillPower * 3 + power, secondary.skillPower, secondary.skillPower + power, secondary.corePush, 2 };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.skillPower + 1, 4 };
        skillInformation[4] = new object[] { s_strike.corePush, drummerCharges, 2 + power };

        base.PreparePlayerActionAssign();
    }

    protected override void ResetBuffs()
    {
        base.ResetBuffs();

        if (isPlayingDrumms)
        {
            Debuff(secondary.skillPower, BuffType.Speed);
            isPlayingDrumms = false;
        }
    }

    public override void Strike()
    {
        if (drummerCharges >= 10)
        {
            m_matchManager.Damage(enemyTeam, 2 + power, mapPosition);
            if (m_matchManager.core.mapPosition == mapPosition)
                m_matchManager.PushCore(allyTeam, 1);
            drummerCharges = 0;
        }

        else drummerCharges++;

        base.Strike();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        HitDrummerCharges(1, mapPosition);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Buff(allyTeam, secondary.skillPower * 3 + power, Position.All, BuffType.Speed);
        m_matchManager.Buff(allyTeam, secondary.skillPower + power, Position.All, BuffType.Power);

        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        isPlayingDrumms = true;

        HitDrummerCharges(2, mapPosition);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
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

        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, hitArea);
        m_matchManager.Debuff(enemyTeam, ultimate.skillPower + 1, hitArea, BuffType.Speed);
        m_matchManager.Stun(enemyTeam, hitArea);

        HitDrummerCharges(4, mapPosition);

        base.UltimateSkill();
    }

    private void HitDrummerCharges(int ammount, Position area)
    {
        if (!m_matchManager.IsMapAreaClear(enemyTeam, (int)area))
            drummerCharges += ammount;
    }
}
