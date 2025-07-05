using UnityEngine;

public class Hamster : Striker
{
    public Hamster(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 20;
        baseSpeed   = 10;

        primary     = new Skill(2, 2, 1, true);
        secondary   = new Skill(4, 0, 2, false);
        ultimate    = new Skill(8, 5, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Hamster;
        figurine.SetStrikerSprite(Strikers.Hamster);
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power };
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
        m_matchManager.Stun(enemyTeam, mapPosition);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, ultimate.corePush);

        m_matchManager.PushEnemy(enemyTeam, 1, mapPosition);

        base.UltimateSkill();
    }
}
