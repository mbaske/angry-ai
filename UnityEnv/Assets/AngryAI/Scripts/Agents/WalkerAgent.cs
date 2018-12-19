using MLAgents;
using UnityEngine;
using System;
using System.Collections.Generic;

public class WalkerAgent : Agent
{
    protected Robot robot;
    private int stepInterval;
    private bool isTraining;
    private float[] prevActions;

    public override void InitializeAgent()
    {
        stepInterval = agentParameters.numberOfActionsBetweenDecisions;
        isTraining = brain.brainType == BrainType.External;
        robot.Initialize();
    }

    public override void AgentReset()
    {
        robot.ReSet();
        prevActions = new float[16];
    }

    public override void CollectObservations()
    {
        AddVectorObs(robot.GetTilt()); // 1
        AddVectorObs(robot.GetWalkDirection()); // 3
        AddVectorObs(robot.GetVelocity()); // 3
        AddVectorObs(robot.GetAngularVelocity()); // 3
        AddVectorObs(robot.GetDistanceToGround()); // 1
        AddVectorObs(robot.GetLegObs()); // 20

        if (isTraining)
        {
            // Might need to tweak multipliers & exponents during training.
            float heading = Vector3.Dot(robot.GetWalkDirection(), Vector3.forward);
            heading = Mathf.Abs(Mathf.Pow(heading, 4f)) * Mathf.Sign(heading); // focus
            SetReward(heading
                    + robot.GetTilt()
                    - robot.GetGoalDistanceDelta());
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (robot.IsOutOfBounds())
        {
            Done();
        }
        else
        {
            int step = GetStepCount() % stepInterval + 1;
            float t = step / (float)stepInterval;
            for (int i = 0; i < 4; i++)
            {
                int n = i * 4;
                float[] itplTargets = new float[4];
                // Interpolate between decision steps, 2 x rotation + 2 x position.
                for (int j = 0; j < 4; j++)
                {
                    itplTargets[j] = Mathf.Lerp(prevActions[n + j], vectorAction[n + j], t);
                }
                robot.GetLeg(i).StepUpdate(itplTargets);
            }
            robot.StepUpdate();

            if (step == stepInterval)
            {
                System.Array.Copy(vectorAction, prevActions, vectorAction.Length);
            }
        }
    }
}