using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Striker
{
    private bool isDroning;
    private Position dronePosition;

    public Drone(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 10;
        baseSpeed   = 20;

        primary     = new Skill(2, 2, 2, true);
        secondary   = new Skill(4, 1, 3, true);
        ultimate    = new Skill(10, 0, 6, false);
        
        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Drone;
        figurine.SetStrikerSprite(Strikers.Drone);

        isDroning = false;
        dronePosition = mapPosition;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power, secondary.skillPower * 5 + power, secondary.skillPower + power, secondary.corePush };
        skillInformation[3] = new object[] { ultimate.skillPower + power };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    protected override void PrepareEndTurn()
    {
        if (isDroning)
        {
            Move(dronePosition);
            isDroning = false;
        }
        
        base.PrepareEndTurn();
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
        m_matchManager.Accelerate(allyTeam, secondary.skillPower * 5 + power, mapPosition);
        m_matchManager.Heal(allyTeam, secondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, secondary.corePush);

        base.SecondarySkill();
    }

    public override void UltimateSkill()
    {
        isEvading = true;
        figurine.SetRenderEvade();

        isDroning = true;
        dronePosition = mapPosition;
        Move(m_matchManager.core.mapPosition);

        m_matchManager.Heal(allyTeam, ultimate.skillPower + power, mapPosition);

        base.UltimateSkill();
    }
}
