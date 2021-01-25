
namespace MBaske
{
    public static class Layer
    {
        public const int Ground = 3;
        public const int Bot = 6;
        public const int Obstacle = 7;
        public const int Bullet = 8;
        public const int Trigger = 9;

        public const int GroundMask = 1 << 3;
        public const int BotMask = 1 << 6;
        public const int ObstacleMask = 1 << 7;
        public const int BulletMask = 1 << 8;
        public const int TriggerMask = 1 << 9;
    }
}