using System.Collections.Generic;
using UnityEngine;

namespace MBaske
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField]
        private HealthBar m_Prefab;
        private Dictionary<FighterAgent, HealthBar> m_Bars;

        private void Awake()
        {
            var agents = FindObjectsOfType<FighterAgent>();
            m_Bars = new Dictionary<FighterAgent, HealthBar>(agents.Length);

            foreach (var agent in agents)
            {
                var bar = Instantiate(m_Prefab, transform);
                agent.BulletHitSufferedEvent += bar.OnBulletHitSuffered;
                m_Bars.Add(agent, bar);
            }
        }

        private void OnDestroy()
        {
            foreach (var kvp in m_Bars)
            {
                kvp.Key.BulletHitSufferedEvent -= kvp.Value.OnBulletHitSuffered;
            }
        }
    }
}