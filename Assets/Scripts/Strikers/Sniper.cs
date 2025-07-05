using UnityEngine;

public class Sniper : Striker
{
    public Sniper(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        primary     = new Skill(2, 1, 3, true);
        secondary   = new Skill(4, 0, 4, false);
        ultimate    = new Skill(7, 2, 2, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Sniper;
        figurine.SetStrikerSprite(Strikers.Sniper);
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.skillPower * 5 + power };
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
        Move(m_matchManager.core.mapPosition);

        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        m_matchManager.Slow(enemyTeam, secondary.skillPower * 5 + power, mapPosition);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, Position.All);
        m_matchManager.PushCore(allyTeam, primary.corePush);

        base.UltimateSkill();
    }
}
