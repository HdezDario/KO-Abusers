using UnityEngine;

public class MpGuardian : MultiplayerStriker
{
    public MpGuardian(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 20;
        baseSpeed   = 10;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(4, 2, 3, true);
        ultimate    = new Skill(8, 0, 6, false);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Guardian;
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
        m_matchManager.PushEnemy(enemyTeam, 1, mapPosition);
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

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        if (!m_matchManager.teams[allyTeam].goalie.isAlive)
            m_matchManager.teams[allyTeam].goalie.Revive();

        if (!m_matchManager.teams[allyTeam].midfielder.isAlive)
            m_matchManager.teams[allyTeam].midfielder.Revive();

        if (!m_matchManager.teams[allyTeam].forward.isAlive)
            m_matchManager.teams[allyTeam].forward.Revive();

        m_matchManager.Heal(allyTeam, ultimate.skillPower + power, Position.All);

        base.UltimateSkill();
    }
}
