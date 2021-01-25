using UnityEngine;

namespace MBaske
{
    public class FXBullet : Bullet
    {
        private TrailRenderer m_Trail;
        private AudioFXPool m_AudioFXPool;
        private VisualFXPool m_VisualFXPool;

        protected override void Initialize()
        {
            base.Initialize();

            m_Trail = GetComponent<TrailRenderer>();
            m_Trail.enabled = false;

            m_AudioFXPool = FindObjectOfType<AudioFXPool>();
            m_VisualFXPool = FindObjectOfType<VisualFXPool>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            m_Trail.Clear();
            m_Trail.enabled = true;

            m_AudioFXPool.Shot(transform.position);
            m_VisualFXPool.MuzzleFlash(transform.position);
        }

        protected override void OnDiscard()
        {
            if (m_IsActive)
            {
                // Didn't hit anything.
                m_AudioFXPool.Stray(transform.position);
            }

            base.OnDiscard();

            m_Trail.enabled = false;
        }

        protected override void OnCollision(Collision collision)
        {
            m_Trail.enabled = false;
            var pos = transform.position;

            switch (collision.gameObject.layer)
            {
                case Layer.Ground:
                    m_AudioFXPool.ImpactGround(pos);
                    m_VisualFXPool.Smoke(pos);
                    break;

                case Layer.Obstacle:
                    m_AudioFXPool.ImpactLight(pos);
                    m_VisualFXPool.SmallExplosion(pos);
                    m_VisualFXPool.Smoke(pos);
                    break;

                case Layer.Bot:
                    if (collision.impulse.magnitude < 20)
                    {
                        m_AudioFXPool.ImpactLight(pos);
                    }
                    else
                    {
                        m_AudioFXPool.ImpactMedium(pos);
                    }
                    m_VisualFXPool.BotImpact(pos);
                    m_VisualFXPool.Smoke(pos);
                    break;
            }
        }
    }
}