using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

namespace MBaske
{
    public class FighterAgentTrain : FighterAgent
    {
        [SerializeField]
        protected int m_TBStatsInterval = 12;
        protected StatsRecorder m_TBStats;

        protected float m_Time;
        protected int m_CollisionCount;

        protected int m_CrntSpeedRewardFactor;
        protected const int c_MaxSpeedRewardFactor = 30;

        public override void Initialize()
        {
            base.Initialize();

            m_TBStats = Academy.Instance.StatsRecorder;
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();

            m_Time = Time.time;
            m_CollisionCount = 0;
            m_CrntSpeedRewardFactor = c_MaxSpeedRewardFactor;
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            base.OnActionReceived(actionBuffers);

            var speed = m_Walker.WorldVelocity.magnitude;
            AddReward(speed * m_CrntSpeedRewardFactor / (WalkerAgent.MaxSpeed * c_MaxSpeedRewardFactor));

            if (m_IsPreDecisionStep)
            {
                m_CrntSpeedRewardFactor = Mathf.Max(1, m_CrntSpeedRewardFactor - 1);

                if (m_Requester.DecisionStepCount % m_TBStatsInterval == 0)
                {
                    m_TBStats.Add(m_BehaviorName + "/Speed", speed);
                    m_TBStats.Add(m_BehaviorName + "/Collisions Per Second", m_CollisionCount / (Time.time - m_Time));
                    m_TBStats.Add(m_BehaviorName + "/Weapon Shots Per Second", m_Weapon.ShotsPerSecond);
                    m_TBStats.Add(m_BehaviorName + "/Weapon Hits Per Second", m_Weapon.HitsPerSecond);
                    m_TBStats.Add(m_BehaviorName + "/Weapon Hit Ratio", m_Weapon.HitRatio);
                }
            }
        }

        protected override void OnCollision(int layer, bool isEnter)
        {
            // Registers collision enter and stay events.
            m_CollisionCount++;
            AddReward(-0.5f);
        }

        protected override void OnBulletHitScored()
        {
            m_CrntSpeedRewardFactor = c_MaxSpeedRewardFactor;
            base.OnBulletHitScored();
        }

        protected override void OnBulletHitSuffered()
        {
            m_CrntSpeedRewardFactor = Mathf.Max(1, m_CrntSpeedRewardFactor / 2);
            base.OnBulletHitSuffered();
        }
    }
}

