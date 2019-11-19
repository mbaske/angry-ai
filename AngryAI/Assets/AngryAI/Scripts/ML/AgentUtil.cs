using MLAgents;
using UnityEngine;

public static class AgentUtil
{
    // Observations are normalized to -1/+1 range, rewards normally shouldn't go much beyond that.
    // There are some situations where combined rewards might spike though, so a value of 10 
    // is on the safe side.
    private const float max = 10;

    // Depending on "Solver Iterations", "Position Spring" and "Maximum Force" joint settings,
    // an agent's random actions might slam colliders into each other with too much force. 
    // This can "break" the physics, resulting in very large or NaN values in the associated
    // rigidbodies or transform components.
    // The ValidateObservations method makes sure these values aren't observed during training 
    // and returns a bool so that an agent can reset if things go crazy.
    // While this fixes the issue during in-editor training, it doesn't seem to work for builds.
    public static bool ValidateObservations(Agent agent)
    {
        AgentInfo info = agent.Info;

        bool isValid = !float.IsNaN(info.reward) && Mathf.Abs(info.reward) < max;
        int length = info.vectorObservation.Count;

        if (isValid)
        {
            for (int i = 0; i < length; i++)
            {
                if (float.IsNaN(info.vectorObservation[i]) || Mathf.Abs(info.reward) > max)
                {
                    isValid = false;
                    break;
                }
            }
        }

        if (!isValid)
        {
            SetNullObservations(agent, length);
        }

        return isValid;
    }

    public static void SetNullObservations(Agent agent, int length)
    {
        AgentInfo info = agent.Info;
        info.vectorObservation.Clear();
        for (int i = 0; i < length; i++)
        {
            info.vectorObservation.Add(0);
        }
        info.reward = 0;
        agent.Info = info;
    }

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
