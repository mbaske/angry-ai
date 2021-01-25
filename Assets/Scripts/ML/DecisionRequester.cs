using UnityEngine;
using Unity.MLAgents;

namespace MBaske
{
    [RequireComponent(typeof(Agent))]
    public class DecisionRequester : MonoBehaviour
    {
        [Range(1, 20)]
        public int DecisionInterval = 1;

        public bool PerStepActions = false;

        public bool Active
        {
            get { return m_Active; }
            set { ToggleActive(value);  }
        }
        [SerializeField, ReadOnly]
        private bool m_Active;

        public int DecisionStepCount { get; private set; } // first: 1
        public int ActionStepCount { get; private set; } // first: 1

        private int m_EventStepCount; // first: 0
        private Agent m_Agent;

        private void Awake()
        {
            m_Agent = gameObject.GetComponent<Agent>();
        }

        private void OnDestroy()
        {
            if (m_Active && Academy.IsInitialized)
            {
                Academy.Instance.AgentPreStep -= OnAgentPreStep;
            }
        }

        private void ToggleActive(bool active)
        {
            if (active)
            {
                if (!m_Active)
                {
                    Academy.Instance.AgentPreStep += OnAgentPreStep;
                }
            }
            else
            {
                if (m_Active)
                {
                    Academy.Instance.AgentPreStep -= OnAgentPreStep;
                }
            }

            m_EventStepCount = 0;
            DecisionStepCount = 0;
            ActionStepCount = 0;

            m_Active = active;
        }

        private void OnAgentPreStep(int academyStepCount)
        {
            if (m_EventStepCount % DecisionInterval == 0)
            {
                DecisionStepCount++;
                ActionStepCount++;
                m_Agent.RequestDecision();
            }
            else if (PerStepActions)
            {
                ActionStepCount++;
                m_Agent.RequestAction();
            }

            m_EventStepCount++;
        }
    }
}
