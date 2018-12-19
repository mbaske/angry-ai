using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public class NavigatorAgent : Agent
{
    private int numWaypoints = 100;
    private Queue<Vector2> waypoints;
    private float distanceCovered;
    private float prevAction;
    private FighterBot robot;
    private int stepInterval;
    private bool isTraining;

    public override void InitializeAgent()
    {
        robot = GetComponent<FighterBot>();
        stepInterval = agentParameters.numberOfActionsBetweenDecisions;
        isTraining = brain.brainType == BrainType.External;
        waypoints = new Queue<Vector2>();
    }

    public override void CollectObservations()
    {
        if (isTraining)
        {
            Color col = robot.GetAvgTextureColor();
            float rg = Mathf.Min((col.r + col.g) * 1000f, 1f);
            AddReward(rg - col.b * 2f);
            AddReward(distanceCovered / 10f);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int step = GetStepCount() % stepInterval + 1;
        float t = step / (float)stepInterval;
        float turn = Mathf.Lerp(prevAction, vectorAction[0], t);
        SetGoal(GetDirection(), turn * 180f, 10f);
     
        if (step == stepInterval)
        {
            prevAction = vectorAction[0];
        }
    }

    public void OnWalkerReset()
    {
        waypoints.Clear();
        distanceCovered = 0f;
    }

    private void SetGoal(Vector2 crntDir, float angle, float distance)
    {
        Vector3 pos = Util.Elevate(robot.GetPosition2D()
                    + Util.Rotate(crntDir, angle) * distance, 25f);
        RaycastHit hit;
        pos.y = Physics.Raycast(pos, Vector3.down, out hit, 50f, Layers.GROUND)
              ? hit.point.y + Robot.HEIGHT : robot.GetPosition().y;
        robot.SetGoal(pos);
        // Debug.DrawLine(robot.GetPosition(), pos, Color.magenta);
    }

    private Vector2 GetDirection()
    {
        if (robot.GetTilt() < 0.5f)
        {
            waypoints.Clear();
            distanceCovered = 0f;
            return robot.GetForward2D();
        }

        Vector2 pos = robot.GetPosition2D();
        waypoints.Enqueue(pos);
        if (waypoints.Count > numWaypoints)
        {
            waypoints.Dequeue();
        }

        Vector2 start = waypoints.Peek();
        distanceCovered = Vector2.Distance(start, pos);
        // Debug.DrawLine(Util.Elevate(start), Util.Elevate(pos), Color.cyan);

        if (distanceCovered < 1f)
        {
            return robot.GetForward2D();
        }

        return (pos - start).normalized;
    }

    // private void OnGUI()
    // {
    //     GUI.DrawTexture(new Rect(10, 10, customTexture.width * 4, 
    //                     customTexture.height * 4), customTexture);
    // }
}