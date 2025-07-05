public enum Strikers
{
    Hamster,
    Sniper,
    Kicker,
    Ember,
    Witch,
    Boxer,
    Guardian,
    Shadow,
    Lizard,
    Rocket,
    Slime,
    Shield,
    Magician,
    Cat,
    Blade,
    Drone,
    Hook,
    Umbrella,
    Drummer,
    Guitar,
    Speedster
}

public enum Position
{
    Left,
    Middle,
    Right,
    All,
    Null
}

public enum BuffType
{
    Power,
    Speed
}

[System.Serializable]
public enum ActionType
{
    Evade,
    Primary,
    Secondary,
    Ultimate,
    Strike,
    MoveLeft,
    MoveRight
}

public enum Scenes
{
    Menu,
    TeamSelect,
    Match,
    Results,
    Lobby,
    MultiplayerTeamSelect,
    MultiplayerMatch,
    Leaderboard
}

public enum MultiplayerPlayerType
{
    None,
    Blue,
    Red
}