using UnityEngine;

public class MpBoxer : MultiplayerStriker
{
    private bool isHuge;
    private int hugeBuff;

    public MpBoxer(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 20;
        baseSpeed   = 10;

        primary = new Skill(4, 2, 3, true);
        secondary = new Skill(6, 2, 5, true);
        ultimate = new Skill(6, 0, 5, false);

        isHuge = false;
        hugeBuff = 0;

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Boxer;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + hugeBuff + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + hugeBuff + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power };
        skillInformation[4] = new object[] { s_strike.corePush, 3 + power };

        base.PreparePlayerActionAssign();
    }
    protected override void ResetBuffs()
    {
        isHuge = false;
        hugeBuff = 0;

        base.ResetBuffs();
    }

    public override void Strike()
    {
        if (isHuge)
        {
            m_matchManager.Damage(enemyTeam, 3 + power, mapPosition);

            if (m_matchManager.core.mapPosition == mapPosition)
                m_matchManager.PushCore(allyTeam, 1);
        }

        base.Strike();
    }

    public override void PrimarySkill()
    {
        if (isHuge)
        {
            m_matchManager.Damage(enemyTeam, primary.skillPower + hugeBuff + power, mapPosition);
        }

        else
        {
            m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        }

        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        if (isHuge)
        {
            m_matchManager.Damage(enemyTeam, secondary.skillPower + hugeBuff + power, mapPosition);
        }

        else
        {
            m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        }

        m_matchManager.Stun(enemyTeam, mapPosition);

        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        Buff(ultimate.skillPower + power, BuffType.Power);
        isHuge = true;
        hugeBuff = 1 + power;

        base.UltimateSkill();
    }
}
