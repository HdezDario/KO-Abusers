using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    string fileName;
    PlayerCard[] playerList;

    [ContextMenu("Write CSV Players")]
    public void WriteCSVPlayers()
    {
        playerList = Resources.LoadAll<PlayerCard>("PlayerCards");

        fileName = Application.dataPath + "/Resources/PlayerList/Players.csv";

        TextWriter tw = new StreamWriter(fileName, false);
        tw.WriteLine("Name, Rank, Role, Striker1, Striker2, Striker3, Striker4, Striker5, Health, Power, Speed, Strike, Action");
        tw.Close();

        tw = new StreamWriter(fileName, true);

        foreach(PlayerCard playerCard in playerList)
        {
            string[] strikers = new string[5];

            for (int i = 0; i < playerCard.roster.Length; i++)
            {
                strikers[i] = playerCard.roster[i].ToString();
            }

            tw.WriteLine(
                playerCard.name + "," +
                playerCard.rank + "," +
                playerCard.role + "," +
                strikers[0] + "," +
                strikers[1] + "," +
                strikers[2] + "," +
                strikers[3] + "," +
                strikers[4] + "," +
                playerCard.health + "," +
                playerCard.power + "," +
                playerCard.speed + "," +
                playerCard.strikePower + "," +
                playerCard.actionMetter);
        }

        tw.Close();

        Debug.Log("File Written");
    }

    [ContextMenu("Check Players Are Valid")]
    public void CheckPlayersAreValid()
    {
        playerList = Resources.LoadAll<PlayerCard>("PlayerCards");
        bool valid = true;

        foreach(PlayerCard player in playerList)
        {
            if (player.picture == null)
            {
                Debug.LogError(player.playerName + " is invalid, Picture wasn't set");
                valid = false;
            }

            switch (player.rank)
            {
                case Rank.Rookie:
                    if (player.roster.Length != 1)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Bronze:
                    if (player.roster.Length != 2)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Silver:
                    if (player.roster.Length != 2)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Gold:
                    if (player.roster.Length != 3)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Platinum:
                    if (player.roster.Length != 3)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Diamond:
                    if (player.roster.Length != 4)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Challenger:
                    if (player.roster.Length != 4)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.Omega:
                    if (player.roster.Length != 5)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
                case Rank.ProLeague:
                    if (player.roster.Length != 5)
                    {
                        Debug.LogError(player.playerName + " is invalid, Roster isn't set");
                        valid = false;
                    }
                    break;
            }

            List<Strikers> playerStrikers = new List<Strikers>();
            foreach (Strikers striker in player.roster)
                playerStrikers.Add(striker);

            if (playerStrikers.Count != playerStrikers.Distinct().Count())
            {
                Debug.LogError(player.playerName + " is invalid, Roster has duplicates");
                valid = false;
            }

            if (player.health == 0 &&
            player.power == 0 &&
            player.speed == 0 &&
            player.strikePower == 0 &&
            player.actionMetter == 0)
            {
                Debug.LogError(player.playerName + " is invalid, Stats weren't set");
                valid = false;
            }
        }

        if (valid)
            Debug.Log("All Players are valid");
        else Debug.LogError("There are invalid players");
    }
}
