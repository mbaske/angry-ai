using UnityEngine;

namespace MBaske
{
    public class Bullet : Poolable
    {
        [SerializeField]
        protected float m_Force = 25;

        protected Rigidbody m_Rigidbody;
        protected Weapon m_Owner;
        protected bool m_IsActive;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public void Shoot(Weapon weapon)
        {
            m_Owner = weapon;
            m_IsActive = true;

            m_Rigidbody.velocity = (weapon.Target - weapon.Position).normalized * m_Force;
        }

        // We only count bullet hits to BodyCollisionTrigger.
        private void OnTriggerEnter(Collider other)
        {
            if (m_IsActive && other != m_Owner.Collider)
            {
                // Ignore friendly fire hits.
                if (!other.CompareTag(m_Owner.Tag))
                {
                    m_Owner.OnBulletHitScored();

                    // CHANGED replaces OnTriggerEnter on opponent collider.
                    other.GetComponent<BodyCollisionTrigger>().InvokeBulletHitSufferedEvent();
                }

                // Deactivate bullet after first hit, regardless of team.
                Deactivate();
            }
            // else: bullet inactive or self-collision -> ignore.
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (m_IsActive)
            //{
                // Handle FX in subclass.
                OnCollision(collision);
                
                if (collision.gameObject.layer != Layer.Bot)
                {
                    // Ground or obstacle.
                    Deactivate();
                }
            //}
        }

        private void Deactivate()
        {
            m_IsActive = false;
            DiscardAfter(0.1f);
        }

        protected override void OnDiscard()
        {
            base.OnDiscard();

            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Rigidbody.Sleep();
            m_IsActive = false;
            m_Owner = null;
        }

        protected virtual void OnCollision(Collision collision) { }
    }
}