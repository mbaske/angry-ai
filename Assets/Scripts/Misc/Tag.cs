
namespace MBaske
{
    public static class Tag
    {
        public static string Spot = "Spot";
        public static string GunBot = "GunBot";

        public static string OpponentTag(string tag)
        {
            return tag == Tag.Spot ? Tag.GunBot : Tag.Spot;
        }

        public static bool IsAgentTag(string tag)
        {
            return tag.Equals(Spot) || tag.Equals(GunBot);
        }
    }
}