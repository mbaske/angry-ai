using System.Collections;
using UnityEngine;

public class Bullet : PhysicsBody
{
    public AudioSource Audio { get; private set; }
    public ParticleSystem Sparks1 { get; private set; }
    public ParticleSystem Sparks2 { get; private set; }

    [SerializeField]
    private float force = 100f;
    [SerializeField]
    private float lifetime = 0.5f;

    private Vector3 offset;
    private TrailRenderer trail;
    private Coroutine crStop;
    
    public override Rigidbody Initialize()
    {
        base.Initialize();
        offset = Vector3.forward * 1.5f;  // TBD

        Audio = GetComponent<AudioSource>();
        Sparks1 = transform.Find("Sparks1").GetComponent<ParticleSystem>();
        Sparks2 = transform.Find("Sparks2").GetComponent<ParticleSystem>();
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
        gameObject.SetActive(false);
        return rb;
    }

    public void Shoot(TargetingSensor sensor)
    {
        transform.position = sensor.StartPosition;
        transform.rotation = sensor.Rotation;

        gameObject.SetActive(true);
        trail.Clear();
        trail.enabled = true;

        transform.Translate(offset, Space.Self);
        rb.AddForce(transform.forward * force, ForceMode.VelocityChange);

        crStop = StartCoroutine(StopBulletCR());
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (crStop != null)
        {
            StopCoroutine(crStop);
        }
        OnStop(other);
    }

    private IEnumerator StopBulletCR()
    {
        yield return new WaitForSeconds(lifetime);
        OnStop(null);
    }

    private void OnStop(Collision other)
    {
        trail.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        OnRaiseCollisionEvent(new CollisionArgs(other));
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }
}
