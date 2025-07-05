using UnityEngine;

public class MpKicker : MultiplayerStriker
{
    private int dashPunch;

    public MpKicker(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth = 15;
        baseSpeed = 15;

        primary = new Skill(2, 1, 3, true);
        secondary = new Skill(4, 2, 3, false);
        ultimate = new Skill(7, 0, 4, true);

        dashPunch = 0;

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Kicker;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + dashPunch + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    protected override void ResetBuffs()
    {
        dashPunch = 0;
        base.ResetBuffs();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + dashPunch + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        if (dashPunch > 0)
        {
            m_matchManager.PushEnemy(enemyTeam, 1, mapPosition);

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
        }

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        dashPunch = 2 + power;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        m_matchManager.Stun(enemyTeam, mapPosition);

        base.UltimateSkill();
    }
}