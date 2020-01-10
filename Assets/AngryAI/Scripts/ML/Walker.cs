using UnityEngine;
using MLAgents;
using System;

namespace MBaske.AngryAI
{
    public class Walker : Agent
    {
        public float WalkMode
        {
            set { walkMode = value; }
        }
        protected float walkMode;

        public float NormWalkDir
        {
            set { normWalkDir = value; }
        }
        protected float normWalkDir;

        [SerializeField]
        protected Fighter fighter;

        [SerializeField]
        protected BodyWalker body;

        private int interval;
        private float[] actionsLerp;
        private float[] actionsBuffer;
        private const int nActions = 16;
        private Resetter resetter;

        public override void InitializeAgent()
        {
            interval = agentParameters.numberOfActionsBetweenDecisions;
            actionsLerp = new float[nActions];
            actionsBuffer = new float[nActions];

            Transform container = transform.parent;
            resetter = new Resetter(container);
            body.Initialize(container.position);
        }

        public override void AgentReset()
        {
            walkMode = 0;
            normWalkDir = 0;
            System.Array.Clear(actionsLerp, 0, nActions);
            System.Array.Clear(actionsBuffer, 0, nActions);
            body.OnAgentReset();
            resetter.Reset();
        }

        public override void CollectObservations()
        {
            AddVectorObs(actionsBuffer); // 16
            AddVectorObs(body.GetNormalizedObs()); // 26
            AddVectorObs(normWalkDir);
            AddVectorObs(walkMode);

            if (body.GetIsOutOfBounds())
            {
                Done();
                fighter?.Done();
            }
        }

        public override void AgentAction(float[] actions)
        {
            // Interpolate action values between decision steps for smoother movement.
            int step = GetStepCount() % interval + 1;
            float t = step / (float)interval;
            for (int i = 0; i < nActions; i++)
            {
                actionsLerp[i] = Mathf.Lerp(actionsBuffer[i], actions[i], t);
            }
            if (step == interval)
            {
                System.Array.Copy(actions, actionsBuffer, nActions);
            }

            body.StepUpdate(actionsLerp);
        }
    }
}