using UnityEngine;

namespace MBaske
{
    public class Bone : MonoBehaviour
    {
        [SerializeField]
        private float m_Speed = 500;

        [Range(-1f, 1f)]
        public float HeuristicAction;

        public float NormLocalVelocity { get; private set; }
        public float NormAngle => (m_CrntAngle - m_Mid) / m_Ext;
        public int Direction => (int)Mathf.Sign(NormLocalVelocity);


        [SerializeField]
        private Vector3Int m_ObservedAxis;

        private ArticulationBody m_Body;
        private ArticulationDrive m_Drive;
        private Transform m_Parent;

        private Quaternion m_DefRot;
        private Quaternion CrntRot => Quaternion.Inverse(transform.rotation) * m_Parent.rotation;
        private Quaternion DeltaRot => Quaternion.Inverse(CrntRot) * m_DefRot;

        private float m_TargetAngle;
        private float m_CrntAngle;
        private float m_PrevAngle;

        private float m_Min;
        private float m_Max;
        private float m_Mid;
        private float m_Ext;

        public void Initialize()
        {
            m_Body = GetComponent<ArticulationBody>();
            m_Drive = m_Body.xDrive;

            FindConnectedParent(transform.parent);
            m_DefRot = CrntRot;

            m_Min = m_Drive.lowerLimit;
            m_Max = m_Drive.upperLimit;
            m_Ext = (m_Max - m_Min) * 0.5f;
            m_Mid = m_Min + m_Ext;
        }

        public void ManagedReset()
        {
            NormLocalVelocity = 0;
            HeuristicAction = 0;
            m_TargetAngle = 0;
            m_CrntAngle = 0;
            m_PrevAngle = 0;
            UpdateDrive();
        }

        public void ManagedUpdate(float action, float deltaTime)
        {
            m_TargetAngle += action * deltaTime * m_Speed;
            m_TargetAngle = Mathf.Clamp(m_TargetAngle, m_Min, m_Max);
            UpdateDrive();

            m_CrntAngle = Mathf.DeltaAngle(0, Vector3.Dot(m_ObservedAxis, DeltaRot.eulerAngles));
            NormLocalVelocity = (m_CrntAngle - m_PrevAngle) / deltaTime / m_Speed;
            m_PrevAngle = m_CrntAngle;
        }

        private void UpdateDrive()
        {
            m_Drive.target = m_TargetAngle;
            m_Body.xDrive = m_Drive;
        }

        private void FindConnectedParent(Transform tf)
        {
            if (tf.GetComponent<ArticulationBody>() != null)
            {
                m_Parent = tf;
            }
            else if (tf != tf.root)
            {
                FindConnectedParent(tf.parent);
            }
            else
            {
                throw new MissingComponentException("ArticulationBody missing in parent");
            }
        }
    }
}
