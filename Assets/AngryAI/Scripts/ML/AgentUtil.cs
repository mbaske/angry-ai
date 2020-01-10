using UnityEngine;

namespace MBaske.AngryAI
{
    public static class AgentUtil
    {
        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiJ4LygxK2Ficyh4KSkiLCJjb2xvciI6IiMwMDAwMDAifSx7InR5cGUiOjEwMDAsIndpbmRvdyI6WyItMTIiLCIxMiIsIi0xLjIiLCIxLjIiXX1d
        public static float Sigmoid(float val)
        {
            return val / (1f + Mathf.Abs(val));
        }

        public static Vector3 Sigmoid(Vector3 v3)
        {
            v3.x = Sigmoid(v3.x);
            v3.y = Sigmoid(v3.y);
            v3.z = Sigmoid(v3.z);
            return v3;
        }
    }
}