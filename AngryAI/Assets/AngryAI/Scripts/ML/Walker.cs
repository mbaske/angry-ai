using UnityEngine;
using MLAgents;
using System;

public class Walker : Agent
{
    public Action DoneCallback;

    public float WalkMode
    {
        set { walkMode = value; }
    }
    protected float walkMode;

    public float NormWalkDir
    {
        set { normWalkDir = value; }
    }
    protected float normWalkDir;

    [SerializeField]
    protected BodyWalker body;

    private int interval;
    private float[] actionsLerp;
    private float[] actionsBuffer;
    private const int nActions = 16;
    private Resetter resetter;

    public override void InitializeAgent()
    {
        interval = agentParameters.numberOfActionsBetweenDecisions;
        actionsLerp = new float[nActions];
        actionsBuffer = new float[nActions];

        Transform container = transform.parent;
        resetter = new Resetter(container);
        body.Initialize(container.position);
    }

    public override void AgentReset()
    {
        walkMode = 0;
        normWalkDir = 0;
        System.Array.Clear(actionsLerp, 0, nActions);
        System.Array.Clear(actionsBuffer, 0, nActions);
        body.OnAgentReset();
        resetter.Reset();
    }

    public override void CollectObservations()
    {
        if (body.GetIsOutOfBounds())
        {
            AgentUtil.SetNullObservations(this, 44);
            Done();
        }
        else
        {
            AddVectorObs(actionsBuffer); // 16
            AddVectorObs(body.GetNormalizedObs()); // 26
            AddVectorObs(normWalkDir);
            AddVectorObs(walkMode);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Interpolate action values between decision steps for smoother movements.
        int step = GetStepCount() % interval + 1;
        float t = step / (float)interval;
        for (int i = 0; i < nActions; i++)
        {
            actionsLerp[i] = Mathf.Lerp(actionsBuffer[i], vectorAction[i], t);
        }
        if (step == interval)
        {
            System.Array.Copy(vectorAction, actionsBuffer, nActions);
        }

        body.StepUpdate(actionsLerp);
        if (body.GetIsOutOfBounds())
        {
            Done();
        }
    }

    public override void Done()
    {
        base.Done();
        // Need to reset fighter agent as well.
        DoneCallback?.Invoke();
    }
}
