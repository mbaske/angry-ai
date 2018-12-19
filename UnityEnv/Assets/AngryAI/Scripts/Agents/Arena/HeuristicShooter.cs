using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class HeuristicShooter : MonoBehaviour, Decision
{
    private List<float> empty = new List<float>();
    private float[] shoot = new float[]{1};
    private float[] idle = new float[]{0};

    public float[] Decide(
            List<float> vectorObs,
            List<Texture2D> visualObs,
            float reward,
            bool done,
            List<float> memory)
    {
        // Excl. obstacles.
        return Util.RndBool(vectorObs[0]) ? shoot : idle;
        // Incl. obstacles.
        // return Util.RndBool(Mathf.Abs(vectorObs[0])) ? shoot : idle;
    }

    public List<float> MakeMemory(
            List<float> vectorObs,
            List<Texture2D> visualObs,
            float reward,
            bool done,
            List<float> memory)
    {
        return empty;
    }
}