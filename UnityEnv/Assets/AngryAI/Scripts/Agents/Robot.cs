using System.Collections.Generic;
using UnityEngine;
using MLAgents;

// WalkerBot and FighterBot inherit from Robot.
public class Robot : MonoBehaviour
{
    public const float HEIGHT = 1.25f;

    protected Vector3 position => body.position;
    protected Leg[] legs;
    protected Transform body;
    protected Rigidbody rb;
    protected Transform fixedParts;
    protected Transform movingParts;
    // Relative to body forward axis.
    protected Vector3 walkDirection;
    protected Vector3 goalPosition;
    protected float prevGoalDistance;

    public virtual void Initialize()
    {
        body = transform.Find("Body");
        fixedParts = transform.Find("Fixed");
        movingParts = transform.Find("Moving");

        SetHeightAboveGround();

        rb = GetBody().Initialize();
        legs = transform.GetComponentsInChildren<Leg>();
        foreach (Leg leg in legs)
        {
            leg.Initialize();
        }
    }

    public virtual void ReSet()
    {
        foreach (Leg leg in legs)
        {
            leg.ReSet();
        }
        GetBody().ReSet();
        ResetGoal();
    }

    public virtual void StepUpdate()
    {
    }

    public void UpdateWalkDirection()
    {
        if (goalPosition == position)
        {
            walkDirection = Vector3.forward;
        }
        else
        {
            Quaternion rot = Quaternion.LookRotation(goalPosition - position, Vector3.up);
            walkDirection = rot * Quaternion.Inverse(body.rotation) * Vector3.forward;
        }
    }

    public Vector3 GetWalkDirection()
    {
        return walkDirection;
    }

    public virtual void ResetGoal()
    {
        SetGoal(position);
        UpdateWalkDirection();
    }

    public void SetGoal(Vector3 goal)
    {
        goalPosition = goal;
        prevGoalDistance = GetDistanceToGoal();
    }

    public void SetGoal(Transform goal)
    {
        SetGoal(goal.position);
    }

    public virtual float GetDistanceToGoal()
    {
        return Vector3.Distance(position, goalPosition);
    }

    public virtual float GetGoalDistanceDelta()
    {
        float distance = GetDistanceToGoal();
        float delta = distance - prevGoalDistance;
        prevGoalDistance = distance;
        return delta;
    }

    public virtual float GetDistanceToGround(bool normalize = true)
    {
        return Util.GetGroundDistance(body, normalize);
    }

    public virtual Vector3 GetVelocity(bool normalize = true)
    {
        float div = normalize ? 10f : 1f; // TBD: div 10 -> result in -1/+1 range.
        return body.InverseTransformVector(rb.velocity) / div;
    }

    public virtual Vector3 GetAngularVelocity(bool normalize = true)
    {
        float div = normalize ? 10f : 1f; // TBD: div 10 -> result in -1/+1 range.
        return body.InverseTransformVector(rb.angularVelocity) / div;
    }

    public virtual Vector3 GetPosition()
    {
        return position;
    }

    public virtual Vector2 GetPosition2D()
    {
        return Util.Flatten(position);
    }

    public PhysicsBody GetBody()
    {
        return body.GetComponent<PhysicsBody>();
    }

    public Leg GetLeg(int index)
    {
        return legs[index];
    }

    public List<float> GetLegObs()
    {
        List<float> obs = new List<float>();
        foreach (Leg leg in legs)
        {
            obs.AddRange(leg.GetNormalizedObs());
        }
        return obs;
    }

    public float GetTilt()
    {
        return body.up.y;
    }

    public Vector2 GetForward2D()
    {
        return Util.Flatten(body.forward);
    }

    public virtual bool IsOutOfBounds()
    {
        // TODO https://forum.unity.com/threads/the-dreaded-invalid-aabb-errors.463144/
        return float.IsNaN(position.x) || position.y < -25f;
    }

    protected virtual void OnUpdate()
    {
        fixedParts.position = position;
        fixedParts.rotation = body.rotation;
    }

    protected virtual void SetHeightAboveGround()
    {
        Vector3 pos = transform.position;
        pos.y += GetDistanceToGround(false) + HEIGHT;
        transform.position = pos;
    }
}