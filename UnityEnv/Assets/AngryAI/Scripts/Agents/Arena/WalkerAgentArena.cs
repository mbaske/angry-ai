using MLAgents;
using UnityEngine;

public class WalkerAgentArena : WalkerAgent
{
    public override void InitializeAgent()
    {
        robot = GetComponent<FighterBot>();
        base.InitializeAgent();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();
        robot.UpdateWalkDirection();
    }
}