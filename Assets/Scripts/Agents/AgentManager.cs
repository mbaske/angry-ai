using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace MBaske
{
    public abstract class AgentManager : MonoBehaviour
    {
        [SerializeField]
        protected bool m_SpawnObstacles;
        protected ObstacleSpawner m_ObstacleSpawner;

        protected int m_AgentCount;
        protected int m_NumAgents;

        protected Dictionary<string, int> m_TeamHitsScoredCount;
        protected Dictionary<string, List<FighterAgent>> m_Agents;

        public IEnumerable<FighterAgent> AllAgents()
        {
            foreach (var team in m_Agents.Values)
            {
                foreach (var agent in team)
                {
                    yield return agent;
                }
            }
        }

        public IEnumerable<FighterAgent> TeamAgents(string tag)
        {
            var team = m_Agents[tag];

            foreach (var other in team)
            {
                yield return other;
            }
        }

        public IEnumerable<FighterAgent> TeamAgents(FighterAgent centerAgent, bool opponents, float radius)
        {
            var team = m_Agents[opponents ? Tag.OpponentTag(centerAgent.tag) : centerAgent.tag];
            var pos = centerAgent.WorldPosition;

            foreach (var other in team)
            {
                if (other != centerAgent && (other.WorldPosition - pos).magnitude <= radius)
                { 
                    yield return other;
                }
            }
        }

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            m_ObstacleSpawner = FindObjectOfType<ObstacleSpawner>();

            m_Agents = new Dictionary<string, List<FighterAgent>>
            {
                { Tag.Spot, new List<FighterAgent>() },
                { Tag.GunBot, new List<FighterAgent>() }
            };

            m_TeamHitsScoredCount = new Dictionary<string, int>
            {
                { Tag.Spot, 0 },
                { Tag.GunBot, 0 }
            };

            var agents = FindObjectsOfType<FighterAgent>();
            m_NumAgents = agents.Length;

            foreach (var agent in agents)
            {
                m_Agents[agent.tag].Add(agent);

                agent.EpisodeBeginEvent += OnEpisodeBegin;
                agent.HealthDepletedEvent += OnHealthDepleted;
                agent.BulletHitScoredEvent += OnBulletHitScored;
            }
        }

        protected virtual void OnEpisodeBegin(FighterAgent agent)
        {
            agent.SetAgentActive(false);

            if (++m_AgentCount == m_NumAgents)
            {
                m_AgentCount = 0;
                OnEpisodeBegin();
            }
        }

        protected virtual void OnEpisodeBegin()
        {
            m_TeamHitsScoredCount[Tag.Spot] = 0;
            m_TeamHitsScoredCount[Tag.GunBot] = 0;

            if (m_SpawnObstacles)
            {
                m_ObstacleSpawner.Spawn(0.5f);
                StartCoroutine(SetAllAgentsActiveDelayed(true, 1));
            }
            else
            {
                StartCoroutine(SetAllAgentsActiveDelayed(true));
            }
        }

        protected IEnumerator SetAllAgentsActiveDelayed(bool active, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            foreach (var agent in AllAgents())
            {
                agent.SetAgentActive(active);
            }
        }

        protected virtual void OnHealthDepleted(FighterAgent agent) { }

        protected virtual void OnBulletHitScored(FighterAgent agent)
        {
            m_TeamHitsScoredCount[agent.tag]++;
        }

        private void OnDestroy()
        {
            foreach (var agent in AllAgents())
            {
                agent.EpisodeBeginEvent -= OnEpisodeBegin;
                agent.HealthDepletedEvent -= OnHealthDepleted;
                agent.BulletHitScoredEvent -= OnBulletHitScored;
            }
        }
    }
}