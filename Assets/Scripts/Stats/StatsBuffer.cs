
namespace MBaske
{
    public class StatsBuffer : TimedQueue<float>
    {
        public bool IsEnabled { get; set; } = true;

        public float Min => Values().Min();
        public float Max => Values().Max();
        public float Range => Max - Min;

        public float Average => Values().Average();
        public float MAD => Values().MAD();

        public float RelStdDev => Values().StdDev(true);
        public float StdDev => Values().StdDev();

        public float Start => First.Time;
        public float End => Last.Time;

        public float Current => Last.Value;

        public StatsBuffer(int initCapacity, string name = "Value") : base(initCapacity, name) { }
    }
}