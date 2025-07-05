using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Striker
{
    public Rocket(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        primary     = new Skill(2, 1, 2, true);
        secondary   = new Skill(5, 2, 3, true);
        ultimate    = new Skill(10, 0, 5, true);

        character = Strikers.Rocket;
        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        figurine.SetStrikerSprite(Strikers.Rocket);
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
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

        Move(hitArea);

        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, hitArea);
        m_matchManager.Stun(enemyTeam, hitArea);
        if (m_matchManager.core.mapPosition == hitArea)
            m_matchManager.PushCore(allyTeam, secondary.corePush);
        
        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, Position.All);

        base.UltimateSkill();
    }
}
