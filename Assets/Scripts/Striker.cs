using UnityEngine;

public class Striker
{
    //Profile
    public string name;
    public Strikers character;

    //MatchManager
    public MatchManager m_matchManager;
    public int allyTeam;
    public int enemyTeam;

    //State & Positioning
    public int actionMetter;
    public bool isAlive;
    public bool isEvading;
    public bool isStunned;
    public bool isRecovered;
    public int deathCounter;
    public int deathSpeed;
    public Position mapPosition;

    //Stats
    public int baseHealth;
    public int health;

    public int basePower;
    public int buffPower;
    public int power;

    public int baseSpeed;
    public int buffSpeed;
    public int speed;

    //Actions Information
    public object[][] skillInformation = new object[7][];

    //Skills
    public Skill s_strike   = new Skill(1, 1, 0, true);
    public Skill s_evade    = new Skill(5, 0, 0, true);
    public Skill s_move     = new Skill(1, 0, 0, false);

    public Skill primary;
    public Skill secondary;
    public Skill ultimate;

    //Figurine
    public Figurine figurine;
    public Vector3[] movePositions;

    #region Functions
    public virtual void PrepareAction()
    {
        actionMetter = 0;

        if (s_strike.currentCooldown > 0) s_strike.currentCooldown -= 1;
        if (s_evade.currentCooldown > 0) s_evade.currentCooldown -= 1;
        if (s_move.currentCooldown > 0) s_move.currentCooldown -= 1;


        if (primary.currentCooldown > 0) primary.currentCooldown -= 1;
        if (secondary.currentCooldown > 0) secondary.currentCooldown -= 1;
        if (ultimate.currentCooldown > 0) ultimate.currentCooldown -= 1;

        if (isAlive)
        {
            isEvading = false;
            figurine.SetRenderAlive();

            if (isRecovered)
            {
                isStunned = false;
                isRecovered = false;
            }

            if (isStunned)
                isRecovered = true;
        }

        else
        {
            deathCounter--;
            if (deathCounter <= 0) Revive();
        }
    }

    public virtual void PreparePlayerActionAssign()
    {
        skillInformation.CopyTo(m_matchManager.currentStrikerInformation, 0);
        m_matchManager.AssignPlayerActions(this);
    }

    protected virtual void PrepareEndTurn()
    {
        ResetBuffs();
        m_matchManager.EndTurn();
    }

    public virtual void Damage(int ammount)
    {
        Debug.Log(name + " got hurt for " + ammount);
        m_matchManager.logHolders[allyTeam].AddLog(character, " got hurt for " + ammount, allyTeam);

        health -= ammount;
        if (health <= 0)
            Die();
        figurine.SetHealth(health);
    }

    public void Heal(int ammount)
    {
        Debug.Log(name + " got healed for " + ammount);
        m_matchManager.logHolders[allyTeam].AddLog(character, " got healed for " + ammount, allyTeam);

        health += ammount;
        if (health > baseHealth)
            health = baseHealth;
        figurine.SetHealth(health);
    }

    public void Stun()
    {
        Debug.Log(name + " got stunned");
        m_matchManager.logHolders[allyTeam].AddLog(character, " got stunned", allyTeam);

        isStunned = true;
        isRecovered = false;
        figurine.SetRenderStun();
    }

    public void Accelerate(int ammount)
    {
        Debug.Log(name + " got accelerated for " + ammount);
        m_matchManager.logHolders[allyTeam].AddLog(character, " got accelerated for " + ammount, allyTeam);

        actionMetter += ammount;
        figurine.SetAction(actionMetter);
    }

    public void Slow(int ammount)
    {
        Debug.Log(name + " got slowed for " + ammount);
        m_matchManager.logHolders[allyTeam].AddLog(character, " got slowed for " + ammount, allyTeam);

        actionMetter -= ammount;
        if (actionMetter < 0)
            actionMetter = 0;
        figurine.SetAction(actionMetter);
    }

    public void Buff(int ammount, BuffType buff)
    {
        switch(buff)
        {
            case (BuffType.Power):
                Debug.Log(name + " buffed Power for " + ammount);
                m_matchManager.logHolders[allyTeam].AddLog(character, " buffed Power for " + ammount, allyTeam);
                buffPower += ammount;
                break;
            case (BuffType.Speed):
                Debug.Log(name + " buffed Speed for " + ammount);
                m_matchManager.logHolders[allyTeam].AddLog(character, " buffed Speed for " + ammount, allyTeam);
                buffSpeed += ammount;
                break;
        }

        figurine.SetPower(power + buffPower);
        figurine.SetSpeed(speed + buffSpeed);
    }

    public void Debuff(int ammount, BuffType buff)
    {
        switch (buff)
        {
            case (BuffType.Power):
                Debug.Log(name + " debuffed Power for " + ammount);
                m_matchManager.logHolders[allyTeam].AddLog(character, " debuffed Power for " + ammount, allyTeam);
                buffPower -= ammount;
                break;
            case (BuffType.Speed):
                Debug.Log(name + " debuffed Speed for " + ammount);
                m_matchManager.logHolders[allyTeam].AddLog(character, " debuffed Speed for " + ammount, allyTeam);
                buffSpeed -= ammount;
                break;
        }

        figurine.SetPower(power + buffPower);
        figurine.SetSpeed(speed + buffSpeed);
    }

    public void Push(int ammount)
    {
        int index = (int)mapPosition;

        index += ammount;

        switch (index)
        {
            case (0):
                mapPosition = Position.Left;
                break;
            case (1):
                mapPosition = Position.Middle;
                break;
            case (2):
                mapPosition = Position.Right;
                break;
        }

        figurine.SetDestination(movePositions[index]);

        Debug.Log(name + " got pushed to " + mapPosition);
        PushMessage();
    }

    protected virtual void ResetBuffs()
    {
        buffPower = 0;
        buffSpeed = 0;

        figurine.SetPower(power + buffPower);
        figurine.SetSpeed(speed + buffSpeed);
    }

    public void Die()
    {
        isAlive = false;
        isEvading = true;
        isStunned = false;
        isRecovered = false;
        deathCounter = 1;

        actionMetter = 0;
        health = 0;

        ResetBuffs();
        figurine.SetHealth(health);
        figurine.SetRenderDead();

        if (allyTeam == 0)
        {
            mapPosition = Position.Left;
            figurine.SetDestination(movePositions[0]);
        }
            
        else
        {
            mapPosition = Position.Right;
            figurine.SetDestination(movePositions[2]);
        }
            
        Debug.Log(name + " died");
        m_matchManager.logHolders[allyTeam].AddLog(character, " died", allyTeam);
    }

    public void Revive()
    {
        health = baseHealth;
        speed = baseSpeed;
        power = basePower;

        isAlive = true;
        isEvading = true;
        isStunned = false;
        isRecovered = false;

        actionMetter = 0;

        ResetBuffs();
        figurine.SetHealth(health);
        figurine.SetRenderEvade();

        Debug.Log(name + " revived");
        m_matchManager.logHolders[allyTeam].AddLog(character, " revived", allyTeam);
    }

    protected void SetStrikerStats(int bonusHealth, int bonusPower, int bonusSpeed)
    {
        baseHealth += bonusHealth;
        health = baseHealth;

        basePower += bonusPower;
        power = basePower;

        baseSpeed += bonusSpeed;
        speed = baseSpeed;

        figurine.SetHealth(health);
        figurine.SetPower(power);
        figurine.SetSpeed(speed);
        figurine.SetAction(actionMetter);
    }
    
    #endregion

    #region Actions
    public virtual void Strike()
    {
        if (m_matchManager.core.mapPosition == mapPosition)
        {
            if (m_matchManager.IsMapAreaClear(enemyTeam, mapPosition))
                m_matchManager.PushCore(allyTeam, s_strike.corePush + 4);
            else m_matchManager.PushCore(allyTeam, s_strike.corePush);
        }

        s_strike.currentCooldown = s_strike.cooldown;
            
        Debug.Log(name + " striked");
        m_matchManager.logHolders[allyTeam].AddLog(character, " striked", allyTeam);

        if (s_strike.endsTurn)
        {
            PrepareEndTurn();
        }
    }

    public void Evade()
    {
        Debug.Log(name + " evaded");
        m_matchManager.logHolders[allyTeam].AddLog(character, " evaded", allyTeam);

        isEvading = true;
        figurine.SetRenderEvade();

        s_evade.currentCooldown = s_evade.cooldown;

        if (s_evade.endsTurn)
        {
            PrepareEndTurn();
        }
    }

    public void Move(int ammount)
    {
        int index = (int)mapPosition;

        index += ammount;

        switch(index)
        {
            case (0):
                mapPosition = Position.Left;
                break;
            case (1):
                mapPosition = Position.Middle;
                break;
            case (2):
                mapPosition = Position.Right;
                break;
        }

        figurine.SetDestination(movePositions[index]);

        Debug.Log(name + " moved to " + mapPosition);
        MoveMessage();

        s_move.currentCooldown = s_move.cooldown;

        if (s_move.endsTurn)
        {
            PrepareEndTurn();
        }

        else
        {
            if (m_matchManager.isPlayerAction)
                PreparePlayerActionAssign();
        }
    }

    public void Move(Position position)
    {
        int index = (int)position;

        mapPosition = position;

        figurine.SetDestination(movePositions[index]);

        Debug.Log(name + " moved to " + mapPosition);
        MoveMessage();

        s_move.currentCooldown = s_move.cooldown;

        if (s_move.endsTurn)
        {
            PrepareEndTurn();
        }

        else
        {
            if (m_matchManager.isPlayerAction)
                PreparePlayerActionAssign();
        }
    }

    private void MoveMessage()
    {
        switch (mapPosition)
        {
            case (Position.Left):
                m_matchManager.logHolders[allyTeam].AddLog(character, "moves to Left", allyTeam);
                break;
            case (Position.Middle):
                m_matchManager.logHolders[allyTeam].AddLog(character, "moves to Middle", allyTeam);
                break;
            case (Position.Right):
                m_matchManager.logHolders[allyTeam].AddLog(character, "moves to Right", allyTeam);
                break;
        }
    }

    private void PushMessage()
    {
        switch (mapPosition)
        {
            case (Position.Left):
                m_matchManager.logHolders[allyTeam].AddLog(character, "pushed to Left", allyTeam);
                break;
            case (Position.Middle):
                m_matchManager.logHolders[allyTeam].AddLog(character, "pushed to Middle", allyTeam);
                break;
            case (Position.Right):
                m_matchManager.logHolders[allyTeam].AddLog(character, "pushed to Right", allyTeam);
                break;
        }
    }

    public virtual void PrimarySkill() {
        Debug.Log(name + " used Primary");
        m_matchManager.logHolders[allyTeam].AddLog(character, " used Primary", allyTeam);

        primary.currentCooldown = primary.cooldown;
        if (primary.endsTurn)
        {
            PrepareEndTurn();
        }
            
        else
        {
            if (m_matchManager.isPlayerAction)
                PreparePlayerActionAssign();
        }
            
    }

    public virtual void SecondarySkill() {
        Debug.Log(name + " used Secondary");
        m_matchManager.logHolders[allyTeam].AddLog(character, " used Secondary", allyTeam);

        secondary.currentCooldown = secondary.cooldown;
        if (secondary.endsTurn)
        {
            PrepareEndTurn();
        }
            
        else
        {
            if (m_matchManager.isPlayerAction)
                PreparePlayerActionAssign();
        }
    }

    public virtual void UltimateSkill() {
        Debug.Log(name + " used Ultimate");
        m_matchManager.logHolders[allyTeam].AddLog(character, " used Ultimate", allyTeam);

        ultimate.currentCooldown = ultimate.cooldown;
        if (ultimate.endsTurn)
        {
            PrepareEndTurn();
        }

        else
        {
            if (m_matchManager.isPlayerAction)
                PreparePlayerActionAssign();
        }
    }
    #endregion

    // Constructor
    public Striker(string nick, MatchManager matchManager, int bonusHealth, int bonusPower, int bonusSpeed, 
        int bonusStrikePower, int baseActionMetter, Figurine chip, Vector3[] positions)
    {
        name = nick;

        m_matchManager = matchManager;

        //s_strike.corePush += bonusStrikePower;

        isAlive = true;
        isStunned = false;
        isEvading = false;
        isRecovered = false;

        actionMetter = baseActionMetter;

        figurine = chip;
        figurine.SetPlayer(name);
        movePositions = positions;

        deathSpeed = 10;

        for (int i = 0; i < skillInformation.Length; i++)
        {
            skillInformation[i] = new object[3];
        }
    }
}
