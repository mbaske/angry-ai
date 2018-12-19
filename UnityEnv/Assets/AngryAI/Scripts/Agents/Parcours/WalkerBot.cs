using UnityEngine;

public class WalkerBot : Robot
{
    private Parcours parcours;

    public override void Initialize()
    {
        base.Initialize();
        parcours = transform.parent.GetComponentInChildren<Parcours>();
        parcours.Initialize();
    }

    public override void ReSet()
    {
        parcours.ReSet();
        base.ReSet();

        // TODO transform.position should better not change after initialization.
        // Need to adjust y-pos in case default slope angle was changed in inspector during runtime.
        SetHeightAboveGround();
    }

    public override void StepUpdate()
    {
        parcours.StepUpdate(body.position);
        base.StepUpdate();
    }

    public override void ResetGoal()
    {
        parcours.ResetGoal();
        SetGoal(parcours.Goal);
        UpdateWalkDirection();
    }

    public override float GetGoalDistanceDelta()
    {
        float delta = GetDistanceToGoal() - prevGoalDistance;
        ResetGoal();
        return delta;
    }

    public override Vector3 GetVelocity(bool normalize = true)
    {
        float div = normalize ? 10f : 1f; // TBD: div 10 -> result in -1/+1 range.
        return body.InverseTransformVector(rb.velocity - parcours.Velocity) / div;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        movingParts.position = body.position;
        movingParts.rotation = Quaternion.Lerp(movingParts.rotation, body.rotation, 0.25f);
    }

    private void Update()
    {
        OnUpdate();
    }
}