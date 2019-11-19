using UnityEngine;

// Trainer is used in combination with fighter agent, who provides the walk direction.

public class WalkTrainerB : Walker
{
    public override void CollectObservations()
    {
        base.CollectObservations();

        if (!IsDone())
        {
            AddReward(-Mathf.Abs(normWalkDir));

            Vector3 dirXZ = Quaternion.Euler(0, normWalkDir * 180f, 0) * body.HeadingXZ;
            float speed = Vector3.Dot(body.VelocityXZ, dirXZ) * 0.1f;

            switch (walkMode)
            {
                case 1:
                    AddReward(speed); // forward
                    break;
                case -1:
                    AddReward(-speed); // backward
                    break;
                case 0:
                    AddReward(-Mathf.Abs(speed)); // pause
                    break;
            }
        }

        // if (!AgentUtil.ValidateObservations(this))
        // {
        //     Done();
        // }
    }
}
