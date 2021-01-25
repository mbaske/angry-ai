using UnityEngine;

namespace MBaske
{
    public class WalkerDummy : MonoBehaviour, IControllableWalker
    {
        public Matrix4x4 Matrix => Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        public Vector3 LocalVelocity => Localize(m_Rigidbody.velocity);
        public Vector3 WorldVelocity => m_Rigidbody.velocity;

        public float NormTargetWalkAngle
        {
            get { return m_TargetWalkAngle / 180f; }
            set { m_TargetWalkAngle = value * 180f; }
        }
        private float m_TargetWalkAngle;

        public float NormTargetLookAngle
        {
            get { return m_TargetLookAngle / 180f; }
            set { m_TargetLookAngle = value * 180f; }
        }
        private float m_TargetLookAngle;

        public float NormTargetSpeed
        {
            get { return m_TargetSpeed / c_MaxSpeed * 2 - 1; }
            set { m_TargetSpeed = (value + 1) * 0.5f * c_MaxSpeed; }
        }
        private float m_TargetSpeed;
        private const float c_MaxSpeed = 5;

        private const float c_Height = 0.75f;
        private const float c_ForceHeight = 100f;
        private const float c_ResponseTime = 0.5f;
        private const float c_ImpactStrength = 0.6f;
        private const float c_ImpactAttenuation = 0.96f;

        private Rigidbody m_Rigidbody;
        private Quaternion m_DefRot;
        private Vector3 m_DefPos;
        private Vector3 m_Velocity;
        private Vector3 m_VelocityDamp;
        private Vector3 m_AngularVelocity;
        private Vector3 m_AngularVelocityDamp;
        private Vector3 m_ImpactVelocity;
        private bool m_IsActive = true;

        private WalkerControls m_Controls;


        private void Awake()
        {
            m_Rigidbody = GetComponentInChildren<Rigidbody>();
            m_DefRot = m_Rigidbody.rotation;
            m_DefPos = m_Rigidbody.position;

            m_Controls = FindObjectOfType<WalkerControls>();

            if (m_Controls != null)
            {
                m_Controls.WalkUpdateEvent += OnWalkUpdate;
                m_Controls.LookUpdateEvent += OnLookUpdate;

                OnWalkUpdate();
                OnLookUpdate();
            }
        }

        public void ResetAgent()
        {
            m_Rigidbody.rotation = m_DefRot;
            m_Rigidbody.position = m_DefPos;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;

            m_Velocity = Vector3.zero;
            m_VelocityDamp = Vector3.zero;
            m_AngularVelocity = Vector3.zero;
            m_AngularVelocityDamp = Vector3.zero;
            m_ImpactVelocity = Vector3.zero;

            m_TargetWalkAngle = 0;
            m_TargetLookAngle = 0;
            m_TargetSpeed = 0;
        }

        public void SetAgentActive(bool active, int decisionInterval)
        {
            m_IsActive = active;
        }

        private void FixedUpdate()
        {
            if (m_IsActive)
            {
                var target = Quaternion.AngleAxis(m_TargetWalkAngle, Vector3.up) * transform.forward * m_TargetSpeed;
                m_Velocity = Vector3.SmoothDamp(m_Velocity, target, ref m_VelocityDamp, c_ResponseTime);
                m_Velocity.y = (c_Height - Util.GetHeightAboveGround(transform.position)) * c_ForceHeight;
                m_Rigidbody.velocity = m_Velocity + m_ImpactVelocity;

                target = Vector3.up * m_TargetLookAngle;
                m_AngularVelocity = Vector3.SmoothDamp(m_AngularVelocity, target, ref m_AngularVelocityDamp, c_ResponseTime);
                m_Rigidbody.angularVelocity = m_AngularVelocity;

                m_ImpactVelocity = m_ImpactVelocity.magnitude > 1 ? m_ImpactVelocity * c_ImpactAttenuation : Vector3.zero;
            }
        }

        private Vector3 Localize(Vector3 v)
        {
            return transform.InverseTransformVector(v);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Layer.Bullet)
            {
                m_ImpactVelocity = collision.relativeVelocity * c_ImpactStrength;
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