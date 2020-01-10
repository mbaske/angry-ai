using System;
using UnityEngine;

namespace MBaske.AngryAI
{
    public class Bullet : MonoBehaviour
    {
        [HideInInspector]
        public BodyFighter OwnerBody;
        [HideInInspector]
        public BulletPool Pool;
        public Action<Collision> Callback;

        [SerializeField]
        private ParticleSystem sparksA;
        [SerializeField]
        private ParticleSystem sparksB;

        [SerializeField]
        private float force = 100;
        private Rigidbody rb;
        private TrailRenderer trail;
        private const string bodyTag = "Body";
        private bool isDemo;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            trail = GetComponent<TrailRenderer>();
            trail.enabled = false;

            isDemo = FindObjectOfType<DemoAcademy>() != null;
        }

        public void Shoot(Vector3 direction)
        {
            rb.AddForce(direction * force, ForceMode.VelocityChange);
            trail.Clear();
            trail.enabled = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            Callback?.Invoke(other);

            trail.enabled = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (isDemo && transform.position.y < 10)
            {
                if (other.collider.CompareTag(bodyTag))
                {
                    sparksA.Play();
                }
                else
                {
                    sparksB.Play();
                }

                Invoke("Discard", 0.5f);
            }
            else
            {
                Discard();
            }
        }

        private void Discard()
        {
            Pool.Discard(this);
        }
    }
}