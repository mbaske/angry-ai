using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MBaske
{
    public class DemoCam : SimpleCam
    {
        private struct SortableAgent : IComparable
        {
            public FighterAgent Agent;
            public float Distance;

            public int CompareTo(object other)
            {
                return Distance.CompareTo(((SortableAgent)other).Distance);
            }
        }
        private List<SortableAgent> m_Sorted;

        private AgentManager m_AgentManager;
        private FighterAgent m_TargetAgent;

        [SerializeField, MinMaxSlider(2f, 20f)]
        private MinMax m_DistanceRange = new MinMax(3f, 20f);
        [SerializeField, MinMaxSlider(0.25f, 10f)]
        private MinMax m_HeightRange = new MinMax(1f, 10f);

        private int m_RepeatSelectCount;
        private float m_Height;
        private readonly Collider[] m_Obstacles = new Collider[20];

        protected override void Initialize()
        {
            m_AgentManager = FindObjectOfType<AgentManager>();
            m_Sorted = new List<SortableAgent>();

            m_TargetAgent = FindObjectOfType<FighterAgent>();
            m_TargetPos = m_TargetAgent.transform.position;
            m_LookPos = m_TargetPos;

            InvokeRepeating("CheckState", 1, 1);
        }

        protected override void SetTargetPos()
        {
            //m_TargetPos = m_TargetAgent.WorldPosition;

            var centroid = m_TargetAgent.WorldPosition;
            var agents = m_AgentManager.TeamAgents(m_TargetAgent, true, 6);
            int n = 1;

            foreach (var agent in agents)
            {
                n++;
                centroid += agent.WorldPosition;
            }
            m_TargetPos = centroid / n;
        }

        protected override void UpdateCam()
        {
            // Dodge obstacles.
            var pos = transform.position;
            var dodge = Mathf.Infinity;
            var n = Physics.OverlapSphereNonAlloc(pos, 2, m_Obstacles);

            for (int i = 0; i < n; i++)
            {
                dodge = Mathf.Min(dodge, (m_Obstacles[i].transform.position - pos).sqrMagnitude);
            }
            m_CamOffset.y = Mathf.Max(m_Height, Util.GetGroundPos(pos).y + 1 / dodge * 50);

            base.UpdateCam();
        }

        private void CheckState()
        {
            const int maxRepeat = 10;

            if (m_RepeatSelectCount >= maxRepeat || 
                !HasLineOfSight() || 
                !HasOpponentInVicinity(m_TargetAgent))
            {
                TryTargetSwitch();
            }
        }

        private void TryTargetSwitch()
        {
            if (HasNewTarget(out FighterAgent targetAgent))
            {
                m_TargetAgent = targetAgent;
                m_RepeatSelectCount = 0;
            }
            else
            {
                m_RepeatSelectCount++;
            }
            // Randomize anyway.
            m_Distance = Random.Range(m_DistanceRange.Min, m_DistanceRange.Max);
            m_Height = Random.Range(m_HeightRange.Min, m_HeightRange.Max);
        }

        private bool HasNewTarget(out FighterAgent targetAgent)
        {
            m_Sorted.Clear();

            var pos = transform.position;
            var agents = m_AgentManager.AllAgents();

            foreach (var agent in agents)
            {
                if (HasLineOfSight(agent))
                {
                    m_Sorted.Add(new SortableAgent
                    {
                        Agent = agent,
                        Distance = (pos - agent.WorldPosition).magnitude
                    });
                }
            }
            m_Sorted.Sort();

            foreach (var obj in m_Sorted)
            {
                if (HasOpponentInVicinity(obj.Agent))
                {
                    targetAgent = obj.Agent;
                    return true;
                }
            }

            // None found.
            targetAgent = m_TargetAgent;
            return false;
        }

        private bool HasOpponentInVicinity(FighterAgent agent)
        {
            var team = m_AgentManager.TeamAgents(agent, true, 10);
            return team.ToArray().Length > 0;
        }

        private bool HasLineOfSight(FighterAgent agent)
        {
            var delta = agent.WorldPosition - transform.position;
            return !Physics.Raycast(transform.position, delta, delta.magnitude, Layer.ObstacleMask);
        }

        private bool HasLineOfSight()
        {
            var delta = m_TargetPos - transform.position;
            return !Physics.Raycast(transform.position, delta, delta.magnitude, Layer.ObstacleMask);
        }
    }
}
