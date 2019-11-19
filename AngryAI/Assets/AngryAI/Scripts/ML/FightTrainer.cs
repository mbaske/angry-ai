using UnityEngine;
using System.Collections.Generic;

public class FightTrainer : Fighter
{
    private Queue<Vector3> path;
    private const string bodyTag = "Body";
    
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        path = new Queue<Vector3>();
    }

    public override void AgentReset()
    {
        base.AgentReset();
        path.Clear();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();

        float distance = 0;
        path.Enqueue(body.transform.position);
        if (path.Count > 10)
        {
            distance = (path.Dequeue() - body.transform.position).magnitude;
        }

        if (targetIsLocked)
        {
            AddReward(1f);
        }
        else
        {
            // Motivate bot to move around.
            AddReward(distance * 0.25f);
        }

        // Penalize proximity, bot should not run into stuff.
        AddReward(body.CumlProximity * -0.1f);
        // Penalize falling over.
        AddReward(body.transform.up.y - 1f);

        // if (!AgentUtil.ValidateObservations(this))
        // {
        //     Done();
        // }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);

        if (isDecisionStep)
        {
            if (hasShootAction)
            {
                if (!targetIsLocked)
                {
                    // Penalize wasted ammo - although we're not 
                    // actually firing a shot when target isn't locked.
                    AddReward(-0.05f);
                }
            }
            else if (targetIsLocked)
            {
                // Missed shot.
                AddReward(-0.25f);
            }
        }
    }

    protected override void OnBulletCollision(Collision other)
    {
        if (other.collider.CompareTag(bodyTag))
        {
            BodyFighter opponentBody = other.collider.GetComponent<BodyFighter>();
            opponentBody.Owner.AddReward(-0.5f);
        }
    }
}
