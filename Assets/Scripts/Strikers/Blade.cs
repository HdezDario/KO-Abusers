using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Striker
{
    private int bladeCharges;
    private int bladeDamage;

    public Blade(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        primary     = new Skill(2, 1, 2, true);
        secondary   = new Skill(4, 2, 3, true);
        ultimate    = new Skill(8, 0, 5, true);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Blade;
        figurine.SetStrikerSprite(Strikers.Blade);

        bladeCharges = 0;
        bladeDamage = 2;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush, 2 };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.corePush, 2 };
        skillInformation[3] = new object[] { ultimate.skillPower + power, 5};
        skillInformation[4] = new object[] { s_strike.corePush, bladeCharges, bladeDamage + power };

        base.PreparePlayerActionAssign();
    }

    public override void Strike()
    {
        if (bladeCharges >= 10)
        {
            m_matchManager.Damage(enemyTeam, bladeDamage + power, mapPosition);

            if (allyTeam == 0 && mapPosition != Position.Right) Move(1);
            else if (allyTeam == 1 && mapPosition != Position.Left) Move(-1);

            bladeCharges = 0;
        }

        else bladeCharges++;

        base.Strike();
    }

    public override void PrimarySkill()
    {
        m_matchManager.Damage(enemyTeam, primary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, primary.corePush);

        HitBladeCharges(2, mapPosition);

        base.PrimarySkill();
    }

    public override void SecondarySkill()
    {
        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        HitBladeCharges(2, mapPosition);

        if (allyTeam == 0 && mapPosition != Position.Right) Move(1);
        else if (allyTeam == 1 && mapPosition != Position.Left) Move(-1);

        m_matchManager.Damage(enemyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        HitBladeCharges(2, mapPosition);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        m_matchManager.Damage(enemyTeam, ultimate.skillPower + power, mapPosition);
        isEvading = true;
        figurine.SetRenderEvade();

        HitBladeCharges(5, mapPosition);

        base.UltimateSkill();
    }

    private void HitBladeCharges(int ammount, Position area)
    {
        if (!m_matchManager.IsMapAreaClear(enemyTeam, (int)area))
            bladeCharges += ammount;
    }
}
