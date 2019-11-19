using UnityEngine;

public class Foot : LimbCollider
{
    private RaycastHit hit;
    private float distance;
    private int lmGround;
    private const float thresh = 0.2f;
    
    public override void Initialize(Transform body, int solverIterations)
    {
        base.Initialize(body, solverIterations);
        lmGround = (1 << Layers.GROUND) ^ (1 << Layers.ROCK) ^ (1 << Layers.HILL); 
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
        rb.AddForce(Vector3.up * (distance < thresh ? 2f: -1f), ForceMode.VelocityChange);
    }

    private void CastRay()
    {
        distance = Physics.Raycast(transform.position, Vector3.down, out hit, 50, lmGround) 
                 ? hit.distance : 0;
    }
}
