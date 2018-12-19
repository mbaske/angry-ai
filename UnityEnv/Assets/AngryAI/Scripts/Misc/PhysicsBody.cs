using UnityEngine;
using System;

public class PhysicsBody : MonoBehaviour
{
    [HideInInspector]
    public event EventHandler<CollisionArgs> RaiseCollisionEvent;

    protected Rigidbody rb;
    protected Vector3 defPosition;
    protected Quaternion defRotation;

    public virtual Rigidbody Initialize()
    {
        rb = GetComponent<Rigidbody>();
        defPosition = transform.localPosition;
        defRotation = transform.localRotation;
        return rb;
    }

    public virtual void ReSet()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = defPosition;
        transform.localRotation = defRotation;
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    protected virtual void OnRaiseCollisionEvent(CollisionArgs e)
    {
        EventHandler<CollisionArgs> handler = RaiseCollisionEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        OnRaiseCollisionEvent(new CollisionArgs(other, CollisionArgs.ENTER));
    }

    // protected virtual void OnCollisionStay(Collision other) 
    // {
    //     OnRaiseCollisionEvent(new CollisionArgs(other, CollisionArgs.STAY));
    // }

    // protected virtual void OnCollisionExit(Collision other) 
    // {
    //     OnRaiseCollisionEvent(new CollisionArgs(other, CollisionArgs.EXIT));
    // }
}
