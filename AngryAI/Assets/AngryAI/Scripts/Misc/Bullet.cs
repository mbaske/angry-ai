using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public BodyFighter OwnerBody;
    public Action<Collision> CollisionCallback;
    
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
        CollisionCallback?.Invoke(other);
        CollisionCallback = null;

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
            Invoke("ReturnToPool", 0.5f);
        }
        else
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        gameObject.Recycle();
    }
}
