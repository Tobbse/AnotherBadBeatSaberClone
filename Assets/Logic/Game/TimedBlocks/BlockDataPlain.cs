public class BlockDataPlain
{
    public const string ORIENTATION_LEFT = "orientation_left";
    public const string ORIENTATION_RIGHT = "orientation_right";
    public const string ORIENTATION_UP = "orientation_up";
    public const string ORIENTATION_DOWN = "orientation_down";

    public const int SIDE_LEFT = 0;
    public const int SIDE_RIGHT = 1;

    public const int SIZE_SMALL = 0;
    public const int SIZE_MEDIUM = 1;
    public const int SIZE_LARGE = 2;

    private int _size;
    private float _time;
    private string _orientation;
    private float _speedFactor;
    private int _yPos;
    private int _side;
    private float _spawnDistance;

    public BlockDataPlain(int size, float time, string orientation, float speedFactor, int yPos, int side, float spawnDistance) {
        _size = size;
        _time = time;
        _orientation = orientation;
        _speedFactor = speedFactor;
        _yPos = yPos;
        _side = side;
        _spawnDistance = spawnDistance;
    }

    public float getDistance(float distance)
    {
        return distance / _speedFactor;
    }

    public int Size
    {
        get { return _size; }
    }

    public float Time
    {
        get { return _time; }
    }

    public string Orientation
    {
        get { return _orientation; }
    }

    public float SpeedFactor
    {
        get { return _speedFactor; }
    }

    public int YPos
    {
        get { return _yPos; }
    }

    public int Side
    {
        get { return _side; }
    }

    public float SpawnDistance
    {
        get { return _spawnDistance; }
    }

}
