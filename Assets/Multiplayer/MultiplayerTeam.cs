using UnityEngine;

public class MultiplayerTeam
{
    public MultiplayerMatchManager mm;

    public MultiplayerStriker goalie;
    public MultiplayerStriker midfielder;
    public MultiplayerStriker forward;

    public MultiplayerTeam(Strikers goalieStriker, Strikers midfielderStriker, Strikers forwardStriker,
        PlayerCard goaliePlayer, PlayerCard midfielderPlayer, PlayerCard forwardPlayer,
        MultiplayerFigurine goalieChip, MultiplayerFigurine midfielderChip, MultiplayerFigurine forwardChip, 
        Vector3[] goaliePos, Vector3[] midfielderPos, Vector3[] forwardPos, int allyTeamNumber, int enemyTeamNumber, MultiplayerMatchManager match)
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

            goalie.figurine.SetDestinationClientRpc(goalie.movePositions[0]);
            midfielder.figurine.SetDestinationClientRpc(midfielder.movePositions[1]);
            forward.figurine.SetDestinationClientRpc(forward.movePositions[2]);
        }

        else
        {
            goalie.mapPosition = Position.Right;
            midfielder.mapPosition = Position.Middle;
            forward.mapPosition = Position.Left;

            goalie.figurine.SetDestinationClientRpc(goalie.movePositions[2]);
            midfielder.figurine.SetDestinationClientRpc(midfielder.movePositions[1]);
            forward.figurine.SetDestinationClientRpc(forward.movePositions[0]);
        }
    }

    public void Turn()
    {
        goalie.actionMetter     += GetTurnSpeed(goalie);
        midfielder.actionMetter += GetTurnSpeed(midfielder);
        forward.actionMetter    += GetTurnSpeed(forward);

        goalie.figurine.SetActionClientRpc(goalie.actionMetter);
        midfielder.figurine.SetActionClientRpc(midfielder.actionMetter);
        forward.figurine.SetActionClientRpc(forward.actionMetter);
    }

    private int GetTurnSpeed(MultiplayerStriker striker)
    {
        if (striker.isAlive)
            return striker.speed + Random.Range(-2, 3) + (mm.turn.Value / 20);
        else return striker.deathSpeed;
    }

    public MultiplayerStriker CreatePlayerStriker(Strikers striker, string name, int health, int power, int speed, int strikePower, 
        int actionMetter, MultiplayerFigurine chip, Vector3[] positions)
    {
        switch (striker)
        {
            case (Strikers.Hamster):
                return new MpHamster(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Sniper):
                return new MpSniper(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Kicker):
                return new MpKicker(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Ember):
                return new MpEmber(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Witch):
                return new MpWitch(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Boxer):
                return new MpBoxer(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Guardian):
                return new MpGuardian(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Shadow):
                return new MpShadow(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Lizard):
                return new MpLizard(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Rocket):
                return new MpRocket(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Slime):
                return new MpSlime(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Shield):
                return new MpShield(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Magician):
                return new MpMagician(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Cat):
                return new MpCat(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Blade):
                return new MpBlade(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Drone):
                return new MpDrone(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Hook):
                return new MpHook(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Umbrella):
                return new MpUmbrella(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Drummer):
                return new MpDrummer(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Guitar):
                return new MpGuitar(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
            case (Strikers.Speedster):
                return new MpSpeedster(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);

            default:
                return new MpKicker(name, mm, health, power, speed, strikePower, actionMetter, chip, positions);
        }
    }
}
