using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
        set
        {
            if (value != null)
            {
                _current = value;
            }
        }
    }

    // Profile
    public string name;

    // Currency
    //public int credits;

    // Saved Team
    public string goalie;
    public string midfielder;
    public string forward;

    // Inventory
    public List<string> unlockedPlayerCards;
    public List<string> unlockedGoalies;
    public List<string> unlockedMidfielders;
    public List<string> unlockedForwards;
    
}