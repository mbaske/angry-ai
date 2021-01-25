using UnityEngine;
using System.Collections.Generic;

namespace MBaske
{
    public static class Colors
    {
        public static string Red = "red";
        public static string Cyan = "cyan";
        public static string Blue = "blue";
        public static string Darkblue = "darkblue";
        public static string Lightblue = "lightblue";
        public static string Purple = "purple";
        public static string Yellow = "yellow";
        public static string Lime = "lime";
        public static string Fuchsia = "fuchsia";
        public static string White = "white";
        public static string Silver = "silver";
        public static string Grey = "grey";
        public static string Black = "black";
        public static string Orange = "orange";
        public static string Brown = "brown";
        public static string Maroon = "maroon";
        public static string Green = "green";
        public static string Olive = "olive";
        public static string Navy = "navy";
        public static string Teal = "teal";
        public static string Aqua = "aqua";
        public static string Magenta = "magenta";

        public static Color Parse(string color)
        {
            Color result = Color.white;

            if (ColorUtility.TryParseHtmlString("#" + color, out result))
            {
                return result;
            }
            if (ColorUtility.TryParseHtmlString(color, out result))
            {
                return result;
            }

            Debug.LogWarningFormat("Invalid color", color);
            return result;
        }

        // startHue inclusive, endHue exclusive
        // TODO
        public static List<Color> Palette(int length, float brightness = 1f,
            float saturation = 0.75f, float startHue = 0, float endHue = 1f)
        {
            var colors = new List<Color>();
            for (int i = 0; i < length; i++)
            {
                float hue = Mathf.Lerp(startHue, endHue, i / (float)length);
                colors.Add(Color.HSVToRGB(hue, saturation, brightness));
            }
            return colors;
        }
    }
}