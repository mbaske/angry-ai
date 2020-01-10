using UnityEngine;

namespace MBaske.AngryAI
{
    public class LimbCollider : MonoBehaviour
    {
        public Vector3 Velocity => rb.velocity;

        protected Rigidbody rb;
        private ConfigurableJoint joint;

        [SerializeField]
        private Transform bone;
        private Transform body;

        public virtual void Initialize(Transform body, int solverIterations)
        {
            this.body = body;
            rb = GetComponent<Rigidbody>();
            rb.solverIterations = solverIterations;
            joint = GetComponent<ConfigurableJoint>();
        }

        public virtual void StepUpdate()
        {
            joint.targetPosition = Quaternion.Inverse(body.rotation) * (body.position - bone.position);
        }
    }
}