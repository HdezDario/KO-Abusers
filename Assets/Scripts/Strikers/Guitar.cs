using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitar : Striker
{
    private bool isPlayingGuitar;
    private bool isTeleporting;

    public Guitar(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(5, 0, 3, false);
        ultimate    = new Skill(10, 2, 10, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Guitar;
        figurine.SetStrikerSprite(Strikers.Guitar);

        isPlayingGuitar = false;
        isTeleporting = false;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.skillPower * 8 + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power };
        skillInformation[3] = new object[] { ultimate.skillPower + power };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    public override void Damage(int ammount)
    {
        if (isPlayingGuitar)
        {
            if (ammount >= health)
                health = ammount + 1;
        }

        base.Damage(ammount);
    }

    public override void PrepareAction()
    {
        if (isPlayingGuitar)
        {
            m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
            isPlayingGuitar = false;
        }

        base.PrepareAction();
    }

    protected override void PrepareEndTurn()
    {
        if (isTeleporting)
        {
            Move(m_matchManager.core.mapPosition);

            m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
            m_matchManager.Stun(enemyTeam, mapPosition);

            isTeleporting = false;
        }

        base.PrepareEndTurn();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        m_matchManager.Slow(enemyTeam, primary.skillPower * 8 + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        isTeleporting = true;

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        isPlayingGuitar = true;

        base.UltimateSkill();
    }
}
