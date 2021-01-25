using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

namespace MBaske
{
    public abstract class WalkerAgentTrain : WalkerAgent
    {
        [SerializeField]
        protected bool m_DrawGUIStats;
        protected GUIStats m_GUIStats;

        [SerializeField]
        protected int m_TBStatsInterval = 60;
        protected StatsRecorder m_TBStats;

        [SerializeField, MinMaxSlider(0f, 5f)]
        protected MinMax m_TargetSpeedRange = new MinMax(0f, 5f);

        protected Vector3 m_TargetWalkDirectionXZ;
        protected Vector3 m_TargetLookDirectionXZ;

        public override void Initialize()
        {
            base.Initialize();

            m_TBStats = Academy.Instance.StatsRecorder;
            m_GUIStats = GetComponent<GUIStats>();

            if (m_DrawGUIStats && m_GUIStats == null)
            {
                m_GUIStats = gameObject.AddComponent<GUIStats>();
            }
        }

        public override void ResetAgent()
        {
            base.ResetAgent();

            if (m_DrawGUIStats)
            {
                m_GUIStats.Clear();
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            base.OnActionReceived(actionBuffers);

            if (m_Requester.DecisionStepCount % m_TBStatsInterval == 0)
            {
                AddTensorboardStats();
            }

            if (m_DrawGUIStats)
            {
                DrawGUIStats();
            }

            SetRewards();
            PostAction();
        }

        protected virtual void SetRewards()
        {
            AddReward(GetHeightReward());
            AddReward(GetInclinationReward());
            AddReward(GetWalkDirectionReward());
            AddReward(GetLookDirectionReward());
            AddReward(GetSpeedErrorPenalty());
        }

        protected virtual void PostAction() { }


        // POSTURE

        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiIoKGNvcyh4KjAuMDE3NDUzMjkpLTAuNSkqMileNCIsImNvbG9yIjoiIzAwMDAwMCJ9LHsidHlwZSI6MTAwMCwid2luZG93IjpbIi05MC41MTI5MTI3MjA0NDE3IiwiOTguNjYxOTg1MjQ4MzI3MTQiLCItMC42NjUiLCIxLjMzNSJdfV0-
        protected float GetInclinationReward(float strength = 0.5f, float exp = 4)
        {
            float norm = Mathf.Max(0, m_Body.transform.up.y - 0.5f) * 2;
            return Mathf.Pow(norm, exp) * strength;
        }

        protected float GetNormHeightError()
        {
            return Mathf.Clamp01(Mathf.Abs(m_Body.AvgHeightOffset));
        }

        protected float GetHeightReward(float strength = 0.5f, float exp = 4)
        {
            return Mathf.Pow(1 - GetNormHeightError(), exp) * strength;
        }


        // DIRECTIONS

        protected void SetTargetPositions(Vector3 walkPos, Vector3 lookPos)
        {
            Vector3 pos = m_Body.WorldPosition;
            SetTargetDirections(
                Vector3.ProjectOnPlane(walkPos - pos, Vector3.up),
                Vector3.ProjectOnPlane(lookPos - pos, Vector3.up));
        }
        
        protected void SetTargetDirections(Vector3 walkDirXZ, Vector3 lookDirXZ)
        {
            m_TargetWalkDirectionXZ = walkDirXZ.normalized;
            m_TargetLookDirectionXZ = lookDirXZ.normalized;
            TargetDirectionsToAngles();
        }

        protected void TargetDirectionsToAngles()
        {
            Vector3 fdw = m_Body.AvgWorldForwardXZ;
            TargetWalkAngle = Vector3.SignedAngle(fdw, m_TargetWalkDirectionXZ, Vector3.up);
            TargetLookAngle = Vector3.SignedAngle(fdw, m_TargetLookDirectionXZ, Vector3.up);
        }

        protected void TargetAnglesToDirections()
        {
            Vector3 fdw = m_Body.AvgWorldForwardXZ;
            m_TargetWalkDirectionXZ = Quaternion.AngleAxis(TargetWalkAngle, Vector3.up) * fdw;
            m_TargetLookDirectionXZ = Quaternion.AngleAxis(TargetLookAngle, Vector3.up) * fdw;
        }

        

        protected float GetNormWalkDirectionError()
        {
            return Vector3.Angle(m_Body.AvgWorldVelocityXZ, m_TargetWalkDirectionXZ) / 180f;
        }

        protected float GetWalkDirectionReward(float strength = 1, float exp = 8)
        {
            return Mathf.Pow(1 - GetNormWalkDirectionError(), exp) * strength;
        }

        protected float GetNormLookDirectionError()
        {
            return Mathf.Abs(NormTargetLookAngle);
        }

        protected float GetLookDirectionReward(float strength = 1, float exp = 8)
        {
            return Mathf.Pow(1 - GetNormLookDirectionError(), exp) * strength;
        }


        // SPEED

        protected void RandomizeTargetSpeed()
        {
            TargetSpeed = Random.Range(m_TargetSpeedRange.Min, m_TargetSpeedRange.Max);
        }

        protected float GetForwardSpeed()
        {
            return Vector3.Dot(m_Body.AvgWorldVelocityXZ, m_Body.AvgWorldForwardXZ);
        }

        protected float GetDirectionalSpeed()
        {
            return Vector3.Dot(m_Body.AvgWorldVelocityXZ, m_TargetWalkDirectionXZ);
        }

        protected float GetSpeedError()
        {
            return Mathf.Min(Mathf.Abs(GetDirectionalSpeed() - TargetSpeed), MaxSpeed);
        }

        // TBD error/(2+error)*1.4 (assuming max speed = 5)
        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiJ4LygyK3gpKjEuNCIsImNvbG9yIjoiIzAwMDAwMCJ9LHsidHlwZSI6MTAwMH1d
        protected float GetSpeedErrorPenalty(float strength = 1)
        {
            if (TargetSpeed < MinSpeed)
            {
                // Penalize any movement.
                return -m_Body.WorldVelocity.magnitude;
            }

            float error = GetSpeedError();
            float norm = error / (2 + error) * 1.4f;
            return norm * -strength;
        }


        // STATS

        protected virtual void AddTensorboardStats()
        {
            m_TBStats.Add(m_BehaviorName + "/Avg. Height", m_Body.AvgHeight);
            m_TBStats.Add(m_BehaviorName + "/Inclination", m_Body.Inclination);
            m_TBStats.Add(m_BehaviorName + "/Speed Error", GetSpeedError());
            m_TBStats.Add(m_BehaviorName + "/Look Error", GetNormLookDirectionError());
            m_TBStats.Add(m_BehaviorName + "/Walk Error", GetNormWalkDirectionError());
        }

        protected virtual void DrawGUIStats()
        {
            m_GUIStats.Add(TargetSpeed, "Speed", "Target", Colors.Orange);
            m_GUIStats.Add(m_Body.AvgSpeed, "Speed", "Measured Avg.", Colors.Lightblue);

            m_GUIStats.Add(m_Body.TargetHeight, "Height", "Target", Colors.Orange);
            m_GUIStats.Add(m_Body.AvgHeight, "Height", "Measured Avg.", Colors.Lightblue);

            m_GUIStats.Add(m_Body.Inclination / 180f, "Inclination", "", Colors.Orange);

            m_GUIStats.Add(GetNormWalkDirectionError(), "Direction Errors", "Walk Direction", Colors.Orange);
            m_GUIStats.Add(GetNormLookDirectionError(), "Direction Errors", "Look Direction", Colors.Lightblue);

            var palette = Colors.Palette(4, 1, 0.5f, 0.2f, 0.8f);

            float sum =
                m_GUIStats.Add(GetHeightReward(), "Rewards", "Height", palette[0]) +
                m_GUIStats.Add(GetInclinationReward(), "Rewards", "Inclination", palette[1]) +
                m_GUIStats.Add(GetWalkDirectionReward(), "Rewards", "Walk Direction", palette[2]) +
                m_GUIStats.Add(GetLookDirectionReward(), "Rewards", "Look Direction", palette[3]);

            sum += m_GUIStats.Add(GetSpeedErrorPenalty(), "Penalties", "Speed Error", Colors.Orange);

            m_GUIStats.Add(sum, "Reward Sum", "", Colors.Lightblue);
        }
    }
}