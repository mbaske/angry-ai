public class Layers
{
    public const int GROUND = 1 << 8;
    public const int WALL = 1 << 9;
    public const int OBSTACLE = 1 << 10;
    public const int BALL = 1 << 11;
    public const int CLEAR = 1 << 12;
    public const int ROBOT = 1 << 13;
    public const int LEG = 1 << 14;
    public const int BULLET = 1 << 15;
    public const int BARRIER = 1 << 16;
    public const int GROUND_AND_OBSTACLE = 1 << 8 | 1 << 10;
}