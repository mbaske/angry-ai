using UnityEngine;
using Unity.MLAgents.Sensors;

namespace MBaske
{
    public class WalkerAgentTrainDirection : WalkerAgentTrain
    {
        [SerializeField]
        private int m_RandomizationInterval = 625;
        [SerializeField, Range(0, 0.001f)]
        private float m_RandomKickProbability = 0.0005f;
        [SerializeField, Range(0f, 1f)]
        private float m_WalkLookSyncRatio = 0.5f;

        public override void OnEpisodeBegin()
        {
            RandomizeTargets();

            base.OnEpisodeBegin();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // Random target directions -> observed angles.
            TargetDirectionsToAngles();

            base.CollectObservations(sensor);
        }

        protected override void PostAction() 
        {
            if (m_Body.WorldPosition.y < -10)
            {
                // Fell off terrain.
                EndEpisode();
            }
            else
            {
                if (Util.RandomBool(m_RandomKickProbability))
                {
                    // Train recovering from hits.
                    m_Body.AddRandomForce();
                }

                if (m_Requester.ActionStepCount % m_RandomizationInterval == 0)
                {
                    RandomizeTargets();
                }
            }
        }

        private void RandomizeTargets()
        {
            RandomizeTargetSpeed();

            var walkDirXZ = RandomDirection();
            var lookDirXZ = Util.RandomBool(m_WalkLookSyncRatio) ? walkDirXZ : RandomDirection();
            SetTargetDirections(walkDirXZ, lookDirXZ);
        }

        private Vector3 RandomDirection()
        {
            return Quaternion.AngleAxis(Random.value * 360, Vector3.up) * Vector3.forward;
        }
    }
}