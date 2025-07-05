using UnityEngine;

public class MpWitch : MultiplayerStriker
{
    public MpWitch(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(3, 1, 2, true);
        secondary   = new Skill(5, 0, 5, false);
        ultimate    = new Skill(8, 2, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Witch;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.skillPower + power, primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower * 3 + power };
        skillInformation[3] = new object[] { ultimate.skillPower + power, ultimate.corePush };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        m_matchManager.Buff(allyTeam, primary.skillPower + power, mapPosition, BuffType.Power);
        m_matchManager.Debuff(enemyTeam, primary.skillPower + power, mapPosition, BuffType.Power);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Accelerate(allyTeam, secondary.skillPower * 3 + power, mapPosition);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        m_matchManager.PushEnemy(enemyTeam, 1, mapPosition);
        m_matchManager.PushCore(allyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
