using UnityEngine;

public static class AgentUtil
{
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
