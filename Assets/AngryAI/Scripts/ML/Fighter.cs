using UnityEngine;
using MLAgents;

namespace MBaske.AngryAI
{
    public class Fighter : Agent
    {
        [SerializeField]
        protected Walker walker;
        [SerializeField]
        protected BodyFighter body;
        [SerializeField]
        protected Head head;

        protected bool hasTargetLock;

        private int interval;
        private float[] actionsLerp;
        private float[] actionsBuffer;
        private const int nActions = 6;

        public override void InitializeAgent()
        {
            interval = agentParameters.numberOfActionsBetweenDecisions;
            actionsLerp = new float[nActions];
            actionsBuffer = new float[nActions];
            head.Initialize();
            body.Initialize(this);
        }

        public override void AgentReset()
        {
            System.Array.Clear(actionsLerp, 0, nActions);
            System.Array.Clear(actionsBuffer, 0, nActions);
            hasTargetLock = false;
        }

        public override void CollectObservations()
        {
            AddVectorObs(actionsBuffer[0]);
            AddVectorObs(actionsBuffer[1]);
            AddVectorObs(actionsBuffer[2]);
            AddVectorObs(body.GetNormalizedObs()); // 89

            RaycastResult result = head.CastRay();
            hasTargetLock = result.HasHit;

            if (hasTargetLock)
            {
                AddVectorObs(-1f);
                AddVectorObs(result.NormDistance);

                Vector3 v = body.Localize(result.OpponentBody.VelocityXZ);
                AddVectorObs(AgentUtil.Sigmoid(v.x));
                AddVectorObs(AgentUtil.Sigmoid(v.z));
            }
            else
            {
                AddVectorObs(1f);
                AddVectorObs(1f);
                AddVectorObs(0f);
                AddVectorObs(0f);
            }
        }

        public override void AgentAction(float[] actions)
        {
            // Interpolate action values between decision steps for smoother movement.
            int step = GetStepCount() % interval + 1;
            float t = step / (float)interval;
            actionsLerp[0] = Mathf.Lerp(actionsBuffer[0], actions[0], t);
            actionsLerp[1] = Mathf.Lerp(actionsBuffer[1], actions[1], t);
            actionsLerp[2] = Mathf.Lerp(actionsBuffer[2], actions[2], t);
            head.StepUpdate(actionsLerp[0], actionsLerp[1], actionsLerp[2]);

            if (step == 1)
            {
                walker.WalkMode = Mathf.Clamp(Mathf.Round(actions[3] * 1.5f), -1, 1);
                walker.NormWalkDir = actions[4];

                if (actions[5] > 0)
                {
                    Bullet bullet = head.ShootBullet();
                    bullet.OwnerBody = body;
                    bullet.Callback = OnBulletCollision;
                }
            }
            else if (step == interval)
            {
                System.Array.Copy(actions, actionsBuffer, nActions);
            }
        }

        protected virtual void OnBulletCollision(Collision other)
        {
        }
    }
}