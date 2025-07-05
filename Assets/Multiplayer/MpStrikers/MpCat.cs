using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpCat : MultiplayerStriker
{
    Position lastPosition;
    int catPush;

    public MpCat(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 1, 2, true);
        secondary   = new Skill(4, 2, 3, false);
        ultimate    = new Skill(9, 2, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Cat;

        lastPosition = mapPosition;
        catPush = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush + catPush };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.corePush };
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
        lastPosition = mapPosition;
        Move(m_matchManager.core.mapPosition);

        catPush = Mathf.Abs((int)mapPosition - (int)lastPosition);

        if (catPush == 0)
            m_matchManager.PushCore(allyTeam, secondary.corePush);
        else
        {
            if (mapPosition > lastPosition)
                m_matchManager.PushCore(0, secondary.corePush + catPush);

            else if (mapPosition < lastPosition)
                m_matchManager.PushCore(1, secondary.corePush + catPush);
        }

        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);

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
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
