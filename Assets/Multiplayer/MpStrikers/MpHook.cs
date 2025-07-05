using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpHook : MultiplayerStriker
{
    public MpHook(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth = 15;
        baseSpeed = 15;

        primary = new Skill(3, 1, 3, true);
        secondary = new Skill(5, 1, 3, false);
        ultimate = new Skill(8, 3, 4, false);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Hook;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.skillPower * 5 + power, secondary.corePush };
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
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        Accelerate(secondary.skillPower * 5 + power);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        m_matchManager.PushEnemy(enemyTeam, -1, mapPosition);

        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(enemyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
