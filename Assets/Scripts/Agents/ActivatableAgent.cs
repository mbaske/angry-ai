using Unity.MLAgents;
using Unity.MLAgents.Policies;

namespace MBaske
{
    public abstract class ActivatableAgent : Agent
    {
        protected string m_BehaviorName;
        protected DecisionRequester m_Requester;
        protected bool IsActive => m_Requester.Active;

        public override void Initialize()
        {
            m_BehaviorName = GetComponent<BehaviorParameters>().BehaviorName;
            m_Requester = GetComponent<DecisionRequester>();

            if (m_Requester == null)
            {
                m_Requester = gameObject.AddComponent<DecisionRequester>();
            }
        }

        public virtual void SetAgentActive(bool active, int decisionInterval = 0)
        {
            m_Requester.Active = active;
            m_Requester.DecisionInterval = decisionInterval > 0 ? decisionInterval : m_Requester.DecisionInterval;
            m_Requester.PerStepActions = m_Requester.DecisionInterval > 1;
        }
    }
}

