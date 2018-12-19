using MLAgents;
using UnityEngine;

public class WalkerAgentParcours : WalkerAgent
{
    public override void InitializeAgent()
    {
        robot = GetComponent<WalkerBot>();
        base.InitializeAgent();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();
        robot.ResetGoal();
    }
}