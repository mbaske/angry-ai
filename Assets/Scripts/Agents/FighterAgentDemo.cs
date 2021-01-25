using UnityEngine;

namespace MBaske
{
    public class FighterAgentDemo : FighterAgent
    {
        private AudioFXPool m_AudioFXPool;
        private VisualFXPool m_VisualFXPool;

        public override void Initialize()
        {
            base.Initialize();

            m_AudioFXPool = FindObjectOfType<AudioFXPool>();
            m_VisualFXPool = FindObjectOfType<VisualFXPool>();
        }

        protected override void OnCollision(int layer, bool isEnter)
        {
            if (isEnter)
            {
                m_AudioFXPool.ImpactThud(WorldPosition);
            }
        }

        protected override void OnHealthDepleted()
        {
            base.OnHealthDepleted();

            var pos = WorldPosition;
            m_AudioFXPool.Explosion(pos);
            m_AudioFXPool.ImpactHard(pos);
            m_VisualFXPool.BotExplosion(pos);

            ResetAgent();
        }
    }
}

