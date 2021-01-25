using UnityEngine;
using System.Collections;

namespace MBaske
{
    public class Body : MonoBehaviour
    {
        public Vector3 WorldPosition => m_Body.transform.position;
        public Vector3 WorldVelocity => m_Body.velocity;
        public Vector3 LocalVelocity => Localize(m_Body.velocity);
        public Vector3 LocalAngularVelocity => Localize(m_Body.angularVelocity);
        
        public Vector3 AvgWorldVelocityXZ { get; private set; }
        public Vector3 AvgLocalVelocityXZ => Localize(AvgWorldVelocityXZ);
        public float AvgSpeed => AvgWorldVelocityXZ.magnitude;

        public Vector3 AvgWorldForwardXZ { get; private set; }
        public Quaternion AvgWorldRotationXZ { get; private set; }

        public Vector3 Gyro => new Vector3(transform.right.y, transform.up.y, transform.forward.y);
        public float Inclination => Vector3.Angle(Vector3.up, transform.up);

        public float AvgHeight { get; private set; }
        public float AvgHeightOffset => AvgHeight - m_DefaultHeight;
        public float CurrentHeight => Util.GetHeightAboveGround(WorldPosition);
        public float TargetHeight => m_DefaultHeight;

        [SerializeField]
        private float m_DefaultHeight;
        [SerializeField]
        private int m_BufferSize = 300;
        private int m_BufferIndex;

        private Vector3[] m_VelocityBuffer;
        private Vector3[] m_ForwardBuffer;
        private float[] m_HeightBuffer;

        private ArticulationBody m_Body;
        private Quaternion m_DefRot;
        private Vector3 m_DefPos;
        private Vector3 m_DefFwd;

        public void Initialize()
        {
            m_Body = GetComponent<ArticulationBody>();
            m_DefRot = transform.rotation;
            m_DefPos = transform.position;
            m_DefFwd = transform.forward;

            m_VelocityBuffer = new Vector3[m_BufferSize];
            m_ForwardBuffer = new Vector3[m_BufferSize];
            m_HeightBuffer = new float[m_BufferSize];
        }

        public void ManagedReset(bool randomizeRotation = false, Vector3 position = default)
        {
            AvgWorldForwardXZ = m_DefFwd;
            AvgWorldRotationXZ = m_DefRot;
            AvgWorldVelocityXZ = Vector3.zero;
            AvgHeight = m_DefaultHeight;
            m_BufferIndex = 0;

            for (int i = 0; i < m_BufferSize; i++)
            {
                m_VelocityBuffer[i] = Vector3.zero;
                m_ForwardBuffer[i] = transform.forward;
                m_HeightBuffer[i] = m_DefaultHeight;
            }

            m_Body.TeleportRoot(
                position.Equals(default) ? m_DefPos : position,
                randomizeRotation
                    ? Quaternion.LookRotation(Random.onUnitSphere, Random.onUnitSphere)
                    : m_DefRot);

            if (!randomizeRotation)
            {
                // Wait for bones to settle.
                m_Body.immovable = true;
                StartCoroutine(UnfreezeDelayed());
            }
        }

        public void ManagedUpdate()
        {
            m_VelocityBuffer[m_BufferIndex] = m_Body.velocity;
            m_ForwardBuffer[m_BufferIndex] = transform.forward;
            m_HeightBuffer[m_BufferIndex] = CurrentHeight;

            m_BufferIndex = ++m_BufferIndex % m_BufferSize;

            AvgWorldVelocityXZ = Vector3.zero;
            AvgWorldForwardXZ = Vector3.zero;
            AvgHeight = 0;

            for (int i = 0; i < m_BufferSize; i++)
            {
                AvgWorldVelocityXZ += m_VelocityBuffer[i];
                AvgWorldForwardXZ += m_ForwardBuffer[i];
                AvgHeight += m_HeightBuffer[i];
            }

            float n = m_BufferSize;
            AvgWorldVelocityXZ = Vector3.ProjectOnPlane(AvgWorldVelocityXZ / n, Vector3.up);
            AvgWorldForwardXZ = Vector3.ProjectOnPlane(AvgWorldForwardXZ / n, Vector3.up).normalized;
            AvgWorldRotationXZ = Quaternion.LookRotation(AvgWorldForwardXZ);
            AvgHeight /= n;

            //Debug.Log($"Speed {AvgSpeed} / Height {AvgHeight}");
        }

        public void AddRandomForce(float strength = 10000)
        {
            m_Body.AddForce(Random.onUnitSphere * strength);
        }

        private IEnumerator UnfreezeDelayed(float delay = 0.2f)
        {
            yield return new WaitForSeconds(delay);
            m_Body.immovable = false;
        }

        private Vector3 Localize(Vector3 v)
        {
            return transform.InverseTransformVector(v);
        }
    }
}