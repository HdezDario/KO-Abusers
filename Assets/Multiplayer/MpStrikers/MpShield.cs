using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpShield : MultiplayerStriker
{
    private int shieldBuff;

    public MpShield(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth = 20;
        baseSpeed = 10;

        primary = new Skill(2, 2, 2, true);
        secondary = new Skill(4, 2, 3, false);
        ultimate = new Skill(8, 4, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Shield;

        shieldBuff = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.corePush };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    public override void Damage(int ammount)
    {
        if (ammount > shieldBuff)
            ammount -= shieldBuff;

        else
            ammount = 0;

        base.Damage(ammount);
    }

    protected override void ResetBuffs()
    {
        if (shieldBuff > 0)
            shieldBuff = 0;

        base.ResetBuffs();

        if (shieldBuff < 0)
            shieldBuff = secondary.skillPower + power;
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        m_matchManager.Stun(enemyTeam, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        shieldBuff = -1;

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
        if (m_matchManager.core.mapPosition == hitArea)
            m_matchManager.PushCore(allyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
