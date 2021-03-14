using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using MBaske.Sensors.Grid;
using System;

namespace MBaske
{
    public abstract class FighterAgent : ActivatableAgent, IPixelGridProvider
    {
        public Vector3 WorldPosition => Util.Position(m_Walker.Matrix);
        public Vector3 WordForward => Util.Forward(m_Walker.Matrix);

        public event Action<FighterAgent> EpisodeBeginEvent;
        public event Action<FighterAgent> BulletHitScoredEvent;
        public event Action<FighterAgent> BulletHitSufferedEvent;
        public event Action<FighterAgent> HealthDepletedEvent;

        public float Health { get; private set; }

        [SerializeField, Range(0.001f, 0.5f)]
        protected float m_HealthDecrement = 0.05f;

        protected IControllableWalker m_Walker;
        protected BodyCollisionTrigger m_Trigger;
        protected Weapon m_Weapon;

        protected float[] m_PrevActions;
        protected bool m_IsPreDecisionStep;

        protected AgentViewRect m_ViewRect;
        protected GridSensorComponentBase m_SensorComp;
        protected GridSensor m_Sensor;

        public override void Initialize()
        {
            base.Initialize();

            m_Walker = GetComponentInChildren<IControllableWalker>();
            m_Weapon = GetComponentInChildren<Weapon>();
            m_Weapon.BulletHitScoredEvent += OnBulletHitScored;

            m_Trigger = GetComponentInChildren<BodyCollisionTrigger>();
            m_Trigger.BulletHitSufferedEvent += OnBulletHitSuffered;
            m_Trigger.CollisionEvent += OnCollision;

            m_SensorComp = GetComponent<GridSensorComponentBase>();
            m_SensorComp.PixelGridProvider = this;
        }

        public override void OnEpisodeBegin()
        {
            if (m_Sensor == null)
            {
                m_Sensor = m_SensorComp.Sensor;
                m_Sensor.UpdateEvent += OnSensorUpdate;
            }

            ResetAgent();
            m_Weapon.ResetStats();
            EpisodeBeginEvent?.Invoke(this);
        }

        public virtual void ResetAgent()
        {
            Health = 1;
            m_Walker.ResetAgent();
            m_PrevActions = new float[3];
        }

        public override void SetAgentActive(bool active, int decisionInterval = -1)
        {
            base.SetAgentActive(active, decisionInterval);
            m_Walker.SetAgentActive(active);
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(m_PrevActions);
            sensor.AddObservation(Health * 2 - 1);

            var v = Normalization.Sigmoid(m_Walker.LocalVelocity);
            sensor.AddObservation(v.x);
            sensor.AddObservation(v.z);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            var actions = actionBuffers.ContinuousActions.Array;
            var step = m_Requester.ActionStepCount % m_Requester.DecisionInterval;
            m_IsPreDecisionStep = step == 0;

            if (m_IsPreDecisionStep)
            {
                Array.Copy(actions, 0, m_PrevActions, 0, actions.Length);
            }
            else
            {
                float t = step / (float)m_Requester.DecisionInterval;
                for (int i = 0; i < actions.Length; i++)
                {
                    actions[i] = Mathf.Lerp(m_PrevActions[i], actions[i], t);
                }
            }

            m_Walker.NormTargetSpeed = actions[0];
            m_Walker.NormTargetWalkAngle = actions[1];
            m_Walker.NormTargetLookAngle = actions[2];
        }

        public override void Heuristic(in ActionBuffers actionsOut) { }

       
        // EVENTS 

        protected virtual void OnBulletHitScored()
        {
            BulletHitScoredEvent?.Invoke(this);
        }

        protected virtual void OnBulletHitSuffered()
        {
            Health -= m_HealthDecrement; 
            BulletHitSufferedEvent?.Invoke(this);

            if (Health <= 0)
            {
                Health = 0;
                OnHealthDepleted();
            }
        }

        protected virtual void OnHealthDepleted()
        {
            HealthDepletedEvent?.Invoke(this);
        }

        protected virtual void OnCollision(int layer, bool isEnter) { }


        // GRID SENSOR

        protected void OnSensorUpdate()
        {
            m_ViewRect.Update(m_Walker.Matrix);
        }

        public PixelGrid GetPixelGrid()
        {
            m_ViewRect ??= new AgentViewRect(gameObject.tag);
            return m_ViewRect;
        }


        private void OnDestroy()
        {
            m_Sensor.UpdateEvent -= OnSensorUpdate;
            m_Weapon.BulletHitScoredEvent -= OnBulletHitScored;
            m_Trigger.BulletHitSufferedEvent -= OnBulletHitSuffered;
            m_Trigger.CollisionEvent -= OnCollision;
        }
    }
}

