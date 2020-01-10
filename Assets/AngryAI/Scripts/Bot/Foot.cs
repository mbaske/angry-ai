using UnityEngine;

namespace MBaske.AngryAI
{
    public class Foot : LimbCollider
    {
        private RaycastHit hit;
        private float distance;
        private const float groundThresh = 0.2f;
        private const float detectionRange = 25;
        private readonly int lmGround = (1 << Layers.GROUND) | (1 << Layers.ROCK) | (1 << Layers.HILL);
        
        public override void Initialize(Transform body, int solverIterations)
        {
            base.Initialize(body, solverIterations);
            CastRay();
        }

        public float GetNormalizedGroundDistance()
        {
            return Mathf.Min(2f, distance) - 1f;
        }

        public override void StepUpdate()
        {
            base.StepUpdate();
            CastRay();
            // Apply up force in case foot slips through ground mesh collider.
            // Otherwise down force for stabilizing overall bot posture.
            rb.AddForce(Vector3.up * (distance < groundThresh ? 2f : -1f), ForceMode.VelocityChange);
        }

        private void CastRay()
        {
            distance = Physics.Raycast(transform.position, Vector3.down, out hit, detectionRange, lmGround)
                     ? hit.distance : 0;
        }
    }
}