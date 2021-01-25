using UnityEngine;
using System;

namespace MBaske
{
    public class Weapon : MonoBehaviour
    {
        public string Tag => gameObject.tag;
        public Vector3 Position => transform.position;
        public Vector3 Target { get; private set; }
        public Collider Collider => m_TriggerCollider;

        public event Action BulletHitScoredEvent;

        public void OnBulletHitScored()
        {
            m_HitCount++;
            // Forward event to agent.
            BulletHitScoredEvent?.Invoke();
        }

        [SerializeField]
        private TargetArea m_TargetArea;
        [SerializeField]
        private float m_ReloadTime = 0.25f;
        // Needed for self collision check.
        [SerializeField]
        private Collider m_TriggerCollider;

        private string m_OpponentTag;
        private BulletPool m_Bullets;
        
        private readonly Collider[] m_DetectedColliders = new Collider[18];

        // Stats

        public float ShotsPerSecond => m_ShotCount / (Time.time - m_Time);
        public float HitsPerSecond => m_HitCount / (Time.time - m_Time);
        public float HitRatio => m_ShotCount > 0 ? m_HitCount / (float)m_ShotCount : 0;

        private float m_Time;
        private float m_ShotTime;
        private int m_ShotCount;
        private int m_HitCount;
        
        public void ResetStats()
        {
            m_Time = Time.time;
            m_ShotCount = 0;
            m_HitCount = 0;
        }


        private void Awake()
        {
            m_Bullets = FindObjectOfType<BulletPool>();
            m_OpponentTag = MBaske.Tag.OpponentTag(Tag);
        }

        private void Update()
        {
            // Auto-fire.
            if (CanShoot() && HasTargetLock(out Vector3 lockedPos))
            {
                Target = lockedPos;
                m_Bullets.Shoot(this);
                m_ShotTime = Time.time;
                m_ShotCount++;
            }
        }

        private bool CanShoot()
        {
            //return Time.time - m_ShotTime > m_ReloadTime;
            return transform.up.y > 0.5f && Time.time - m_ShotTime > m_ReloadTime;
        }

        private bool HasTargetLock(out Vector3 lockedPos)
        {
            var result = false;
            lockedPos = default;

            var weaponPos = Position;
            var range = m_TargetArea.Range;
            var n = Physics.OverlapSphereNonAlloc(weaponPos, range, m_DetectedColliders, Layer.TriggerMask);

            if (n > 0)
            {
                var fwdXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                m_TargetArea.Update(weaponPos, fwdXZ);

                for (int i = 0; i < n; i++)
                {
                    var targetPos = m_DetectedColliders[i].transform.position;
                    var delta = targetPos - weaponPos;

                    if (m_DetectedColliders[i].CompareTag(m_OpponentTag))
                    {
                        if (m_TargetArea.Contains(targetPos))
                        { 
                            var distance = delta.magnitude;
                            // TODO Line of sight doesn't consider team members blocking shots.
                            if (!Physics.Raycast(weaponPos, delta, distance, Layer.ObstacleMask))
                            {
                                if (distance < range)
                                {
                                    range = distance;
                                    lockedPos = targetPos;

                                    result = true;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}