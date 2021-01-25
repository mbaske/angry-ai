using UnityEngine;

namespace MBaske
{
    public class SimpleCam : MonoBehaviour
    {
        [SerializeField]
        protected Transform m_Target;
        protected Vector3 m_TargetPos;
        protected Vector3 m_LookPos;

        [SerializeField]
        protected float m_Distance = 5;
        [SerializeField]
        protected Vector2 m_CamOffset = Vector2.up;
        [SerializeField]
        protected Vector2 m_LookOffset;

        [Space, SerializeField]
        protected float m_MoveDamp = 0.25f;
        protected Vector3 m_MoveVelo;
        [SerializeField]
        protected float m_LookDamp = 0.25f;
        protected Vector3 m_LookVelo;
        [SerializeField]
        protected float m_MaxDistanceFromCenter;

        protected void Awake()
        {
            Initialize();
        }

        protected void LateUpdate()
        {
            SetTargetPos();
            UpdateCam();
        }

        protected virtual void Initialize()
        {
            if (m_Target == null)
            {
                var body = FindObjectOfType<ArticulationBody>();
                if (body != null)
                {
                    m_Target = body.transform;
                }
                else
                {
                    m_Target = FindObjectOfType<Rigidbody>().transform;
                }
            }

            SetTargetPos();
            m_LookPos = m_TargetPos;
        }

        protected virtual void SetTargetPos()
        {
            m_TargetPos = m_Target.position;
        }

        protected virtual void UpdateCam()
        {
            var camGroundPos = Util.GetGroundPos(transform.position);
            var targetGroundPos = Util.GetGroundPos(m_TargetPos);
            var perp = Vector3.Cross(targetGroundPos - camGroundPos, Vector3.up).normalized;

            var direction = (camGroundPos - targetGroundPos).normalized;
            var newCamPos = targetGroundPos
                + direction * m_Distance
                + perp * m_CamOffset.x 
                + Vector3.up * m_CamOffset.y;

            if (m_MaxDistanceFromCenter > 0 && newCamPos.magnitude > m_MaxDistanceFromCenter)
            {
                newCamPos = newCamPos.normalized * m_MaxDistanceFromCenter;
            }
            transform.position = Vector3.SmoothDamp(transform.position, newCamPos, ref m_MoveVelo, m_MoveDamp);

            var newLookPos = targetGroundPos 
                + perp * m_LookOffset.x 
                + Vector3.up * m_LookOffset.y;

            m_LookPos = Vector3.SmoothDamp(m_LookPos, newLookPos, ref m_LookVelo, m_LookDamp);
            transform.LookAt(m_LookPos);
        }
    }
}