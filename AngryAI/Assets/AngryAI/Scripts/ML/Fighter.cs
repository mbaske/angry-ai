using UnityEngine;
using MLAgents;

public class Fighter : Agent
{
    [SerializeField]
    protected Walker walker;
    [SerializeField]
    protected BodyFighter body;
    [SerializeField]
    protected Head head;

    protected bool targetIsLocked;
    protected bool hasShootAction;
    protected bool isDecisionStep;

    private int interval;
    private float[] actionsLerp;
    private float[] actionsBuffer;
    private const int nActions = 6;

    public override void InitializeAgent()
    {
        interval = agentParameters.numberOfActionsBetweenDecisions;
        actionsLerp = new float[nActions];
        actionsBuffer = new float[nActions];
        head.Initialize();
        body.Initialize(this);
    }

    public override void AgentReset()
    {
        System.Array.Clear(actionsLerp, 0, nActions);
        System.Array.Clear(actionsBuffer, 0, nActions);
        targetIsLocked = false;
    }

    public override void CollectObservations()
    {
        AddVectorObs(actionsBuffer[0]);
        AddVectorObs(actionsBuffer[1]);
        AddVectorObs(actionsBuffer[2]);
        AddVectorObs(body.GetNormalizedObs()); // 89

        RaycastResult result = head.CastRay();
        targetIsLocked = result.HasHit;

        if (targetIsLocked)
        {
            AddVectorObs(-1f);
            AddVectorObs(result.NormDistance);
            // Observe opponent movement.
            // TODO project before or after localizing velocity?
            // Vector3 v = body.transform.InverseTransformVector(result.OpponentBody.Velocity);
            // v = Vector3.ProjectOnPlane(v, Vector3.up);
            Vector3 v = body.transform.InverseTransformVector(result.OpponentBody.VelocityXZ);
            AddVectorObs(AgentUtil.Sigmoid(v.x));
            AddVectorObs(AgentUtil.Sigmoid(v.z));
        }
        else
        {
            AddVectorObs(1f);
            AddVectorObs(1f);
            AddVectorObs(0f);
            AddVectorObs(0f);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        // Interpolate action values between decision steps for smoother movements.
        int step = GetStepCount() % interval + 1;
        float t = step / (float)interval;
        for (int i = 0; i < 3; i++)
        {
            actionsLerp[i] = Mathf.Lerp(actionsBuffer[i], vectorAction[i], t);
        }
        if (step == interval)
        {
            System.Array.Copy(vectorAction, actionsBuffer, nActions);
        }
        head.StepUpdate(actionsLerp[0], actionsLerp[1], actionsLerp[2]);

        isDecisionStep = step == 1;
        if (isDecisionStep)
        {
            float walkMode = vectorAction[3];
            if (walkMode > 0.33f)
            {
                walker.WalkMode = 1; // forward
            }
            else if (walkMode < -0.33f)
            {
                walker.WalkMode = -1; // backward
            }
            else
            {
                walker.WalkMode = 0; // pause
            }

            walker.NormWalkDir = vectorAction[4];

            hasShootAction = vectorAction[5] > 0;
            if (hasShootAction && targetIsLocked)
            {
                Bullet bullet = head.ShootBullet();
                bullet.OwnerBody = body;
                bullet.CollisionCallback = OnBulletCollision;
            }
        }
    }

    protected virtual void OnBulletCollision(Collision other)
    {
    }
}
