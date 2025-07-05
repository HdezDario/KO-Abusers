using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Umbrella : Striker
{
    private bool isOpenForm;
    private int isOpen;

    private Skill closedPrimary;
    private Skill closedSecondary;
    private Skill openPrimary;
    private Skill openSecondary;

    public Umbrella(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions) : base(nick, matchManager, bonusHealth, bonusPower, bonusSpeed, bonusStrikePower, baseActionMetter, chip, positions)
    {
        baseHealth  = 15;
        baseSpeed   = 15;

        closedPrimary   = new Skill(3, 1, 2, true);
        closedSecondary = new Skill(5, 1, 3, true);
        openPrimary     = new Skill(3, 2, 3, true);
        openSecondary   = new Skill(5, 0, 5, true);
        ultimate        = new Skill(1, 0, 0, false);

        SetStrikerStats(bonusHealth, bonusPower, bonusSpeed);

        character = Strikers.Umbrella;
        figurine.SetStrikerSprite(Strikers.Umbrella);

        isOpenForm = false;
        isOpen = 0;
        primary     = closedPrimary;
        secondary   = closedSecondary;
    }

    public override void PreparePlayerActionAssign()
    {
        skillInformation[1] = new object[] { primary.skillPower + power, primary.corePush };
        skillInformation[2] = new object[] { secondary.skillPower + power , closedSecondary.corePush };
        skillInformation[3] = new object[] { isOpen };
        skillInformation[4] = new object[] { s_strike.corePush };

        base.PreparePlayerActionAssign();
    }

    public override void PrepareAction()
    {
        if (closedPrimary.currentCooldown > 0) closedPrimary.currentCooldown -= 1;
        if (openPrimary.currentCooldown > 0) openPrimary.currentCooldown -= 1;
        if (closedSecondary.currentCooldown > 0) closedSecondary.currentCooldown -= 1;
        if (openSecondary.currentCooldown > 0) openSecondary.currentCooldown -= 1;

        base.PrepareAction();
    }

    public override void Strike()
    {
        if (!isOpenForm)
        {
            if (m_matchManager.core.mapPosition == mapPosition)
                m_matchManager.PushCore(allyTeam, 1);
        }

        base.Strike();
    }

    public override void PrimarySkill()
    {
        if (isOpenForm)
            OpenPrimary();
        else
            ClosedPrimary();

        base.PrimarySkill();
    }

    private void ClosedPrimary()
    {
        m_matchManager.Damage(enemyTeam, closedPrimary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, closedPrimary.corePush);

        Debug.Log(name + " used Closed Primary");
    }

    private void OpenPrimary()
    {
        m_matchManager.Damage(enemyTeam, openPrimary.skillPower + power, mapPosition);
        m_matchManager.PushEnemy(enemyTeam, 1, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, openPrimary.corePush);

        openPrimary.currentCooldown = openPrimary.cooldown;

        Debug.Log(name + " used Open Primary");
    }

    public override void SecondarySkill()
    {
        if (isOpenForm)
            OpenSecondary();
        else
            ClosedSecondary();

        base.SecondarySkill();
    }

    private void ClosedSecondary()
    {
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

        m_matchManager.Damage(enemyTeam, closedSecondary.skillPower + power, mapPosition);
        if (m_matchManager.core.mapPosition == mapPosition)
            m_matchManager.PushCore(allyTeam, closedSecondary.corePush);

        Debug.Log(name + " used Closed Secondary");
    }

    private void OpenSecondary()
    {
        switch (allyTeam)
        {
            case (0):
                if (mapPosition != Position.Left)
                    Move(-1);
                break;

            case (1):
                if (mapPosition != Position.Right)
                    Move(1);
                break;
        }

        m_matchManager.Damage(enemyTeam, openSecondary.skillPower + power, mapPosition);
        
        isEvading = true;
        figurine.SetRenderEvade();

        openSecondary.currentCooldown = openSecondary.cooldown;

        Debug.Log(name + " used Open Secondary");
    }

    public override void UltimateSkill()
    {
        isOpenForm = !isOpenForm;

        if (!isOpenForm)
        {
            Debug.Log(name + " closed umbrella");
            primary = closedPrimary;
            secondary = closedSecondary;
            isOpen = 0;
        }
        else
        {
            Debug.Log(name + " opened umbrella");
            primary = openPrimary;
            secondary = openSecondary;
            isOpen = 1;
        }

        base.UltimateSkill();
    }
}
