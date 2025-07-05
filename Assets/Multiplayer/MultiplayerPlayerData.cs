using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPlayerData
{
    private static MultiplayerPlayerData _current;
    public static MultiplayerPlayerData current
    {
        get
        {
            if (_current == null)
            {
                _current = new MultiplayerPlayerData();
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

    public string playerName;

    public string goalie;
    public string midfielder;
    public string forward;

    public Strikers glStriker;
    public Strikers mfStriker;
    public Strikers fwStriker;
}
