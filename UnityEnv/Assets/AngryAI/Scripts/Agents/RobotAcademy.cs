using MLAgents;
using UnityEngine;

public class RobotAcademy : Academy
{
    public override void InitializeAcademy()
    {
        Application.runInBackground = true;
    }
}