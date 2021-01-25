using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

namespace MBaske
{
    public class WalkerAgentTrainImitation : WalkerAgentTrain
    {
        private enum TargetSpeedMode
        {
            OscillatorSpeed, RandomSpeed
        }
        [SerializeField]
        private TargetSpeedMode m_TargetSpeedMode;

        private Oscillator m_Oscillator;
        private Quaternion m_TargetWalkRot;
        private bool m_IsHeuristic;

        public override void Initialize()
        {
            base.Initialize();

            m_Oscillator = GetComponentInChildren<Oscillator>();

            var param = GetComponent<BehaviorParameters>();
            m_IsHeuristic = param.BehaviorType == BehaviorType.HeuristicOnly;

            if (m_IsHeuristic)
            {
                // Uses oscillator.
                m_Activation = Activation.WhenGrounded;
            }
        }

        public override void OnEpisodeBegin()
        {
            m_Oscillator.ManagedReset();

            m_TargetWalkRot = Quaternion.AngleAxis(m_Oscillator.GetTargetAngle(), Vector3.up);
            SetTargetDirections(m_TargetWalkRot * Vector3.forward, Vector3.forward);

            if (m_TargetSpeedMode == TargetSpeedMode.OscillatorSpeed)
            {
                TargetSpeed = m_Oscillator.GetTargetSpeed();
            }
            else
            {
                RandomizeTargetSpeed();
            }

            base.OnEpisodeBegin();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (m_IsHeuristic)
            {
                // Recording demonstration:
                // Look direction is always agent forward.
                var fwd = m_Body.AvgWorldForwardXZ;
                SetTargetDirections(m_TargetWalkRot * fwd, fwd);
            }
            else
            {
                // Look direction is world forward.
                TargetDirectionsToAngles();
            }

            base.CollectObservations(sensor);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            m_Oscillator.ManagedUpdate();

            base.Heuristic(actionsOut);
        }

        protected override void PostAction()
        {
            if (m_IsHeuristic && m_Body.Inclination > 30)
            {
                // Early stop when tweaking oscillator.
                EndEpisode();
            }
        }
    }
}