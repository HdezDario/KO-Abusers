using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Card", menuName = "Player Card", order = 0)]
[System.Serializable]
public class PlayerCard : ScriptableObject
{
    [Header("Info")]
    public string playerName;
    public Sprite picture;
    public Rank rank;
    public Role role;

    [Header("Strikers")]
    public Strikers[] roster;


    [Header("Stats")]
    public int health;
    public int power;
    public int speed;
    public int strikePower;
    public int actionMetter;

    private void OnEnable()
    {
        playerName = name;
    }

    public Sprite GetStrikerIcon(Strikers stkr)
    {
        return Resources.Load<Sprite>("CharacterIcons/" + stkr + "_icon");
    }

    public Sprite GetRankedIcon(Rank rank)
    {
        return Resources.Load<Sprite>("RankedIcons/" + rank + "_icon");
    }

    public Sprite GetRankedCard(Rank rank)
    {
        return Resources.Load<Sprite>("RankedCards/" + rank + "_Card");
    }

    public Sprite GetRoleIcon(Role role)
    {
        return Resources.Load<Sprite>("Icons/Area_" + role);
    }
}

[System.Serializable]
public enum Rank
{
    ProLeague,
    Omega,
    Challenger,
    Diamond,
    Platinum,
    Gold,
    Silver,
    Bronze,
    Rookie
}

[System.Serializable]
public enum Role
{
    Goalie,
    Midfielder,
    Forward
}