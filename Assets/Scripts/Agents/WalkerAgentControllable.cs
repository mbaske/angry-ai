using UnityEngine;

namespace MBaske
{
    public class WalkerAgentControllable : WalkerAgent
    {
        private WalkerControls m_Controls; 

        public override void Initialize()
        {
            base.Initialize();

            m_Controls = FindObjectOfType<WalkerControls>();

            if (m_Controls != null)
            {
                m_Controls.WalkUpdateEvent += OnWalkUpdate;
                m_Controls.LookUpdateEvent += OnLookUpdate;

                OnWalkUpdate();
                OnLookUpdate();
            }
        }

        private void OnWalkUpdate()
        {
            NormTargetSpeed = m_Controls.NormSpeed;
            NormTargetWalkAngle = m_Controls.NormWalkAngle;
        }

        private void OnLookUpdate()
        {
            NormTargetLookAngle = m_Controls.NormLookAngle;
        }

        private void OnDestroy()
        {
            if (m_Controls != null)
            {
                m_Controls.WalkUpdateEvent -= OnWalkUpdate;
                m_Controls.LookUpdateEvent -= OnLookUpdate;
            }
        }
    }
}