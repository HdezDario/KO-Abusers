using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    private static MatchData _current;
    public static MatchData current
    {
        get
        {
            if (_current == null)
            {
                _current = new MatchData();
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

    // Rank
    public Rank rank;
    public int reward;

    // Player Team
    public PlayerInfo goalie;
    public PlayerInfo midfielder;
    public PlayerInfo forward;

    // Enemy Team
    public PlayerInfo enemyGoalie;
    public PlayerInfo enemyMidfielder;
    public PlayerInfo enemyForward;

    // Player Lists
    public List<PlayerCard> currentRankGoalies;
    public List<PlayerCard> currentRankMidfielders;
    public List<PlayerCard> currentRankForwards;

    public struct PlayerInfo
    {
        public PlayerCard player;
        public Strikers[] strikers;
        public int[] strikersState;
        public Strikers selectedStriker;
        public int selectedStrikerIndex;
    }
}
