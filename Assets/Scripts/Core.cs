public class Core
{
    public int position;
    private int areaSize;
    public Position mapPosition;

    public Core(Position pos, int size)
    {
        position = 0;
        areaSize = size;
        mapPosition = pos;
    }

    public void CheckCorePosition()
    {
        if (position < -((areaSize - 1) / 2)) mapPosition = Position.Left;
        else if (position > ((areaSize - 1) / 2)) mapPosition = Position.Right;
        else mapPosition = Position.Middle;
    }
}
