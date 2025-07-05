using UnityEngine;

public class Team
{
    public MatchManager mm;

    public Striker goalie;
    public Striker midfielder;
    public Striker forward;

    public Team(Strikers goalieStriker, Strikers midfielderStriker, Strikers forwardStriker,
        PlayerCard goaliePlayer, PlayerCard midfielderPlayer, PlayerCard forwardPlayer,
        Figurine goalieChip, Figurine midfielderChip, Figurine forwardChip, 
        Vector3[] goaliePos, Vector3[] midfielderPos, Vector3[] forwardPos,int allyTeamNumber, int enemyTeamNumber, MatchManager match)
    {
        mm = match;

        //Goalie
        goalie = CreatePlayerStriker(goalieStriker, goaliePlayer.playerName, goaliePlayer.health, goaliePlayer.power, goaliePlayer.speed, goaliePlayer.strikePower, goaliePlayer.actionMetter, goalieChip, goaliePos);
        
        //Midfielder
        midfielder = CreatePlayerStriker(midfielderStriker, midfielderPlayer.playerName, midfielderPlayer.health, midfielderPlayer.power, midfielderPlayer.speed, midfielderPlayer.strikePower, midfielderPlayer.actionMetter, midfielderChip, midfielderPos);

        //Forward
        forward = CreatePlayerStriker(forwardStriker, forwardPlayer.playerName, forwardPlayer.health, forwardPlayer.power, forwardPlayer.speed, forwardPlayer.strikePower, forwardPlayer.actionMetter, forwardChip, forwardPos);

        goalie.allyTeam         = allyTeamNumber;
        midfielder.allyTeam     = allyTeamNumber;
        forward.allyTeam        = allyTeamNumber;

        goalie.enemyTeam        = enemyTeamNumber;
        midfielder.enemyTeam    = enemyTeamNumber;
        forward.enemyTeam       = enemyTeamNumber;

        if (allyTeamNumber == 0)
        {
            goalie.mapPosition = Position.Left;
            midfielder.mapPosition = Position.Middle;
            forward.mapPosition = Position.Right;

            goalie.figurine.SetDestination(goalie.movePositions[0]);
            midfielder.figurine.SetDestination(midfielder.movePositions[1]);
            forward.figurine.SetDestination(forward.movePositions[2]);
        }

        else
        {
            goalie.mapPosition = Position.Right;
            midfielder.mapPosition = Position.Middle;
            forward.mapPosition = Position.Left;

            goalie.figurine.SetDestination(goalie.movePositions[2]);
            midfielder.figurine.SetDestination(midfielder.movePositions[1]);
            forward.figurine.SetDestination(forward.movePositions[0]);
        }
    }

    public void Turn()
    {

        goalie.actionMetter     += GetTurnSpeed(goalie);
        midfielder.actionMetter += GetTurnSpeed(midfielder);
        forward.actionMetter    += GetTurnSpeed(forward);

        goalie.figurine.SetAction(goalie.actionMetter);
        midfielder.figurine.SetAction(midfielder.actionMetter);
        forward.figurine.SetAction(forward.actionMetter);
    }

    private int GetTurnSpeed(Striker striker)
    {
        if (striker.isAlive)
            return striker.speed + Random.Range(-2, 3) + (mm.turn / 20);
        else return striker.deathSpeed;
    }

    public Striker CreatePlayerStriker(Strikers striker, string name, int health, int power, int speed, int strikePower, 
        int actionMetter, Figurine chip, Vector3[] positions)
    {
        switch (striker)
        {
            case (Strikers.Hamster):
                return new Hamster(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);

            case (Strikers.Sniper):
                return new Sniper(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Kicker):
                return new Kicker(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Ember):
                return new Ember(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Witch):
                return new Witch(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Boxer):
                return new Boxer(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Guardian):
                return new Guardian(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Shadow):
                return new Shadow(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Lizard):
                return new Lizard(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Rocket):
                return new Rocket(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Slime):
                return new Slime(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Shield):
                return new Shield(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Magician):
                return new Magician(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Cat):
                return new Cat(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Blade):
                return new Blade(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Drone):
                return new Drone(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Hook):
                return new Hook(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Umbrella):
                return new Umbrella(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Drummer):
                return new Drummer(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Guitar):
                return new Guitar(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Speedster):
                return new Speedster(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);

            default:
                return new Kicker(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);

        }
    }
}
