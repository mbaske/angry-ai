
using UnityEngine;

namespace MBaske
{
    public class VisualFX : Poolable 
    {
        private Detonator m_Detonator;
        private ParticleSystem[] m_Particles;

        private void Awake()
        {
            m_Detonator = GetComponentInChildren<Detonator>();
            m_Particles = GetComponentsInChildren<ParticleSystem>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            if (m_Detonator != null)
            {
                m_Detonator.Explode();
            }
        }
        protected override void OnDiscard()
        {
            base.OnDiscard();

            foreach (var ps in m_Particles)
            {
                ps.Stop();
                ps.Clear();
            }
        }
    }
}