using UnityEngine;

public class Shadow : Striker
{
    private Position shadowPosition;

    public Shadow(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(4, 1, 2, true);
        ultimate    = new Skill(8, 0, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Shadow;
        figurine.SetStrikerSprite(Strikers.Shadow);

        shadowPosition = Position.Middle;
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
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        isEvading = true;
        figurine.SetRenderEvade();

        Position lastPosition = mapPosition;
        Move(shadowPosition);
        shadowPosition = lastPosition;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);

        //Goalie
        if (m_matchManager.teams[enemyTeam].goalie.isAlive && !m_matchManager.teams[enemyTeam].goalie.isEvading && m_matchManager.teams[enemyTeam].goalie.mapPosition == mapPosition)
        {
            m_matchManager.teams[enemyTeam].goalie.isEvading = true;
            m_matchManager.teams[enemyTeam].goalie.isStunned = true;
            m_matchManager.teams[enemyTeam].goalie.figurine.SetRenderEvade();
        }

        //Midfielder
        if (m_matchManager.teams[enemyTeam].midfielder.isAlive && !m_matchManager.teams[enemyTeam].midfielder.isEvading && m_matchManager.teams[enemyTeam].midfielder.mapPosition == mapPosition)
        {
            m_matchManager.teams[enemyTeam].midfielder.isEvading = true;
            m_matchManager.teams[enemyTeam].midfielder.isStunned = true;
            m_matchManager.teams[enemyTeam].midfielder.figurine.SetRenderEvade();

        }

        //Forward
        if (m_matchManager.teams[enemyTeam].forward.isAlive && !m_matchManager.teams[enemyTeam].forward.isEvading && m_matchManager.teams[enemyTeam].forward.mapPosition == mapPosition)
        {
            m_matchManager.teams[enemyTeam].forward.isEvading = true;
            m_matchManager.teams[enemyTeam].forward.isStunned = true;
            m_matchManager.teams[enemyTeam].forward.figurine.SetRenderEvade();
        }

        base.UltimateSkill();
    }
}
