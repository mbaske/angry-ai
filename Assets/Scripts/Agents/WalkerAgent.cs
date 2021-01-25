using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;

namespace MBaske
{
    public abstract class WalkerAgent : ActivatableAgent, IControllableWalker
    {
        public Matrix4x4 Matrix => Matrix4x4.TRS(m_Body.WorldPosition, m_Body.AvgWorldRotationXZ, Vector3.one);
        public Vector3 LocalVelocity => m_Body.AvgLocalVelocityXZ;
        public Vector3 WorldVelocity => m_Body.AvgWorldVelocityXZ;

        public const float MaxSpeed = 5;
        public const float MinSpeed = 0.5f;

        public float NormTargetSpeed
        {
            set { TargetSpeed = (value + 1) * 0.5f * MaxSpeed; }
            get { return m_NormTargetSpeed; }
        }
        protected float TargetSpeed
        {
            set { m_TargetSpeed = value < MinSpeed ? 0 : value; 
                  m_NormTargetSpeed = m_TargetSpeed / MaxSpeed * 2 - 1; }
            get { return m_TargetSpeed; }
        }
        private float m_NormTargetSpeed;
        private float m_TargetSpeed;

        public float NormTargetWalkAngle { set; get; }
        protected float TargetWalkAngle
        {
            set { NormTargetWalkAngle = value / 180f; }
            get { return NormTargetWalkAngle * 180; }
        }

        public float NormTargetLookAngle { set; get; }
        protected float TargetLookAngle
        {
            set { NormTargetLookAngle = value / 180f; }
            get { return NormTargetLookAngle * 180; }
        }


        protected Body m_Body;
        protected Bone[] m_Bones;
        
        protected GroundRayDetection[] m_GroundRayDetectors;
        protected GroundContactDetection[] m_GroundContactDetectors;

        protected enum Activation
        {
            Immediately, ImmediatelyRotate, WhenGrounded
        }
        [SerializeField]
        protected Activation m_Activation;


        public override void Initialize()
        {
            base.Initialize();

            m_GroundRayDetectors = GetComponentsInChildren<GroundRayDetection>();
            m_GroundContactDetectors = GetComponentsInChildren<GroundContactDetection>();

            m_Body = GetComponentInChildren<Body>();
            m_Body.Initialize();

            m_Bones = GetComponentsInChildren<Bone>();
            foreach (var bone in m_Bones)
            {
                bone.Initialize();
            }
        }

        public override void OnEpisodeBegin()
        {
            ResetAgent();
        }

        public virtual void ResetAgent()
        {
            foreach (var detector in m_GroundContactDetectors)
            {
                detector.ManagedReset();
            }

            foreach (var bone in m_Bones)
            {
                bone.ManagedReset();
            }

            m_Body.ManagedReset(m_Activation == Activation.ImmediatelyRotate);

            if (m_Activation == Activation.WhenGrounded)
            {
                StartCoroutine(ActivateWhenGrounded());
            }
            else
            {
                SetAgentActive(true);
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_NormTargetSpeed);
            sensor.AddObservation(NormTargetWalkAngle);
            sensor.AddObservation(NormTargetLookAngle);

            sensor.AddObservation(m_Body.Gyro);
            sensor.AddObservation(Normalization.Sigmoid(m_Body.LocalVelocity));
            sensor.AddObservation(Normalization.Sigmoid(m_Body.LocalAngularVelocity));
            
            for (int i = 0; i < m_Bones.Length; i++)
            {
                sensor.AddObservation(m_Bones[i].NormAngle);
                sensor.AddObservation(m_Bones[i].NormLocalVelocity);
            }

            foreach (var detector in m_GroundRayDetectors)
            {
                sensor.AddObservation(detector.GetGroundDistance());
            }

            foreach (var detector in m_GroundContactDetectors)
            {
                sensor.AddObservation(detector.HasGroundContact);
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var dt = Time.fixedDeltaTime;
            var actions = actionBuffers.ContinuousActions;
            
            for (int i = 0; i < m_Bones.Length; i++)
            {
                m_Bones[i].ManagedUpdate(actions[i], dt);
            }

            m_Body.ManagedUpdate();
        }

        public override void Heuristic(in ActionBuffers actionsOut) 
        {
            var actions = actionsOut.ContinuousActions;

            for (int i = 0; i < m_Bones.Length; i++)
            {
                actions[i] = m_Bones[i].HeuristicAction;
            }
        }

        protected IEnumerator ActivateWhenGrounded()
        {
            yield return new WaitUntil(() => IsGrounded());

            SetAgentActive(true);
        }

        protected bool IsGrounded()
        {
            bool isGrounded = true;
            foreach (var detector in m_GroundContactDetectors)
            {
                isGrounded = isGrounded && detector.HasGroundContact;
            }
            return isGrounded;
        }
    }
}

