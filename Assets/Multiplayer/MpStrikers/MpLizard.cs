using UnityEngine;

public class MpLizard : MultiplayerStriker
{
    private int lizardBuff;

    public MpLizard(string nick, MultiplayerMatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, MultiplayerFigurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(5, 0, 2, false);
        ultimate    = new Skill(7, 2, 3, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Lizard;

        lizardBuff = 0;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + lizardBuff + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower * 10 + power };
        skillInformation[3] = new object[] { ultimate.skillPower + lizardBuff + power, ultimate.skillPower * 5 + power, ultimate.corePush };
        skillInformation[4] = new object[] { s_strike.corePush + (lizardBuff / 2) };

        base.PreparePlayerActionAssign();
    }

    protected override void ResetBuffs()
    {
        lizardBuff = 0;
        base.ResetBuffs();
    }

    public override void Strike()
    {
        if (lizardBuff > 0)
        {
            if (m_matchManager.core.mapPosition == mapPosition)
                m_matchManager.PushCore(allyTeam, 1);
        }

        base.Strike();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + lizardBuff + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        actionMetter += secondary.skillPower * 10 + power;
        isEvading = true;
        figurine.SetRenderEvadeClientRpc();

        lizardBuff = secondary.skillPower + power;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + lizardBuff + power, mapPosition);
        m_matchManager.Slow(enemyTeam, ultimate.skillPower * 5 + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, ultimate.corePush);

        base.UltimateSkill();
    }
}
