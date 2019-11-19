using System.Collections.Generic;
using UnityEngine;

public class BodyWalker : Body
{
    [SerializeField]
    private Leg[] legs;
    [SerializeField]
    private LimbCollider[] knees;
    [SerializeField]
    private Foot[] feet;

    private List<float> obs;
    private Vector3 offset;
    private int lmGround;

    public void Initialize(Vector3 offset)
    {
        base.Initialize();
        this.offset = offset;
        lmGround = (1 << Layers.GROUND) ^ (1 << Layers.ROCK) ^ (1 << Layers.HILL); 

        obs = new List<float>(26);
        for (int i = 0; i < 4; i++)
        {
            legs[i].Initialize();
            knees[i].Initialize(transform, 12);
            feet[i].Initialize(transform, 64); 
        }
    }

    public bool GetIsOutOfBounds()
    {
        Vector3 pos = transform.position - offset;

        if (Mathf.Abs(pos.x) > 50) 
        {
            return true;
        }
        if (Mathf.Abs(pos.y) > 25)
        {
            return true;
        }
        if (Mathf.Abs(pos.z) > 50)
        {
            return true;
        }
        return isOutOfBounds;
    }

    public override void StepUpdate(float[] actions)
    {
        for (int i = 0, j = 0; i < 4; i++)
        {
            legs[i].StepUpdate(actions[j++], actions[j++], actions[j++], actions[j++]);
            knees[i].StepUpdate();
            feet[i].StepUpdate();
        }
    }

    public List<float> GetNormalizedObs()
    {
        obs.Clear();
        // Inclination
        obs.Add(transform.right.y);
        obs.Add(transform.up.y);
        obs.Add(transform.forward.y);
        obs.Add(GetNormalizedGroundDistance());
        // Localize movement.
        AddVectorObs(obs, AgentUtil.Sigmoid(transform.InverseTransformVector(rb.velocity)));
        AddVectorObs(obs, AgentUtil.Sigmoid(transform.InverseTransformVector(rb.angularVelocity)));

        for (int i = 0; i < 4; i++)
        {
            AddVectorObs(obs, AgentUtil.Sigmoid(transform.InverseTransformVector(feet[i].Velocity)));
            obs.Add(feet[i].GetNormalizedGroundDistance());
        }
        return obs;
    }

    private float GetNormalizedGroundDistance()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2, lmGround))
        {
            return hit.distance - 1;
        }
        return 1;
    }
}
