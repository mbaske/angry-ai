using UnityEngine;
using System.Collections.Generic;

namespace MBaske.AngryAI
{
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

            float pathLength = UpdatePath();

            if (hasTargetLock)
            {
                AddReward(1f);
            }
            else
            {
                // Motivate agent to move around while it isn't targeting opponents.
                AddReward(pathLength * 0.25f);
            }

            // Penalize proximity, bot should not run into stuff.
            AddReward(body.CumlProximity * -0.1f);
            // Penalize falling over.
            AddReward(body.transform.up.y - 1f);
        }

        protected override void OnBulletCollision(Collision other)
        {
            if (other.collider.CompareTag(bodyTag))
            {
                AddReward(0.5f);

                BodyFighter opponentBody = other.collider.GetComponent<BodyFighter>();
                opponentBody.Owner.AddReward(-0.25f);
            }
            else
            {
                AddReward(-0.5f);
            }
        }

        private float UpdatePath()
        {
            Vector3 pos = body.transform.position;
            path.Enqueue(pos);
            return ((path.Count > 10 ? path.Dequeue() : path.Peek()) - pos).magnitude;
        }
    }
}