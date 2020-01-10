using System.Collections.Generic;
using UnityEngine;

namespace MBaske.AngryAI
{
    public class BodyFighter : Body
    {
        public Fighter Owner { get; private set; }
        public float CumlProximity { get; private set; }

        [SerializeField]
        private Transform shieldFX;
        private bool isDemo;
        private List<float> obs;
        private const float detectionRange = 25;
        private const float proximityThresh = 3;
        private readonly int lmDetectable = 
            (1 << Layers.BODY) | (1 << Layers.HEAD) | (1 << Layers.HILL) | (1 << Layers.ROCK);

        public void Initialize(Fighter agent)
        {
            base.Initialize();

            Owner = agent;
            obs = new List<float>(89);
            isDemo = FindObjectOfType<DemoAcademy>() != null;
        }

        public List<float> GetNormalizedObs()
        {
            obs.Clear();
            // Inclination
            obs.Add(transform.right.y);
            obs.Add(transform.up.y);
            obs.Add(transform.forward.y);

            AddVectorObs(obs, AgentUtil.Sigmoid(Localize(rb.velocity)));
            AddVectorObs(obs, AgentUtil.Sigmoid(Localize(rb.angularVelocity)));

            CumlProximity = 0;
            Vector3 pos = transform.position;
            Vector3 fwd = ForwardXZ;
            for (int angle = -60; angle <= 60; angle += 5)
            {
                CastRay(pos, fwd, angle);
            }
            for (int angle = 75; angle <= 285; angle += 15)
            {
                CastRay(pos, fwd, angle);
            }
            return obs;
        }

        private void CastRay(Vector3 pos, Vector3 fwd, int angle)
        {
            // No detection defaults.
            float normDistance = 1f;
            float objTypeInfo = 1f;
            float normProx = 0f;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * fwd;
            if (Physics.SphereCast(pos, 1f, dir, out RaycastHit hit, detectionRange, lmDetectable))
            {
                normDistance = (hit.distance / detectionRange) * 2f - 1f;

                int layer = hit.collider.gameObject.layer;
                if (layer == Layers.HEAD)
                {
                    // Discriminte head and body, so bot can determine if it might be targeted.
                    objTypeInfo = -1;
                }
                else if (layer == Layers.BODY)
                {
                    objTypeInfo = -0.5f;
                }
                else
                {
                    // Obstacles like rocks and hills.
                    objTypeInfo = 0.5f;
                }

                if (hit.distance < proximityThresh)
                {
                    normProx = 1f - hit.distance / proximityThresh;
                }
            }

            obs.Add(normDistance);
            obs.Add(objTypeInfo);
            CumlProximity += normProx;
        }

        private void OnCollisionEnter(Collision other)
        {
            int layer = other.collider.gameObject.layer;
            if (isDemo)
            {
                if (layer == Layers.BULLET)
                {
                    Bullet bullet = other.collider.GetComponent<Bullet>();
                    shieldFX.LookAt(bullet.OwnerBody.transform.position);
                    shieldFX.gameObject.SetActive(true);
                    CancelInvoke();
                    Invoke("DeactivateShieldFX", 0.1f);
                }
            }
            else if (layer == Layers.HILL)
            {
                isOutOfBounds = true;
            }
        }

        private void DeactivateShieldFX()
        {
            shieldFX.gameObject.SetActive(false);
        }
    }
}