using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MpSlime : MultiplayerStriker
{
    public MpSlime(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(4, 2, 2, false);
        ultimate    = new Skill(8, 2, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Slime;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.skillPower * 3 + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.skillPower * 3 + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.skillPower * 2 + power, ultimate.corePush };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        m_matchManager.Slow(enemyTeam, primary.skillPower * 3 + power, mapPosition);

        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        m_matchManager.Slow(enemyTeam, secondary.skillPower * 3 + power, mapPosition);

        isEvading = true;
        figurine.SetRenderEvadeClientRpc();

        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        switch (allyTeam)
        {
            case (0):
                if (mapPosition != Position.Right)
                    Move(1);
                break;

            case (1):
                if (mapPosition != Position.Left)
                    Move(-1);
                break;
        }

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, Position.All);
        m_matchManager.Slow(enemyTeam, ultimate.skillPower * 2 + power, Position.All);
        m_matchManager.PushCore(allyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
