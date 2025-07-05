public class Skill
{
    // Cooldown
    public int cooldown;
    public int currentCooldown;

    // Stats
    public int corePush;
    public int skillPower;

    // Conditions
    public bool endsTurn;

    public Skill(int cd, int push, int power, bool ends)
    {
        cooldown = cd;
        currentCooldown = 0;
        corePush = push;
        skillPower = power;
        endsTurn = ends;
    }
}
