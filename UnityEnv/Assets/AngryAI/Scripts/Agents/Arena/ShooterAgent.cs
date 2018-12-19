using UnityEngine;
using MLAgents;

public class ShooterAgent : Agent
{
    private FighterBot robot;
    private int stepInterval;
    private bool isTraining;

    public override void InitializeAgent()
    {
        robot = GetComponent<FighterBot>();
        stepInterval = agentParameters.numberOfActionsBetweenDecisions;
        isTraining = brain.brainType == BrainType.External;
    }

    public override void CollectObservations()
    {
        robot.UpdateSensor();
        AddVectorObs(robot.Sensor.Proximity);
        // TODO Use visual obs, train aim rather than using hardcoded target lock.
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (GetStepCount() % stepInterval == 0)
        {
            int action = Mathf.FloorToInt(vectorAction[0]);

            if (isTraining)
            {
                switch (action)
                {
                    case 0:
                        AddReward(robot.Sensor.IsLocked ? -1f : 0f);
                        break;
                    case 1:
                        AddReward(robot.Sensor.IsLocked || robot.Sensor.HasObstacle ? 
                                  Mathf.Abs(robot.Sensor.Proximity) : -1f);

                        break;
                }
            }

            if (action == 1)
            {
                robot.Shoot();
            }
        }
    }

    // private void Shoot()
    // {
    //     Bullet bullet = robot.Shoot();
    //     bullet.RaiseCollisionEvent += HandleCollisionEvent;
    // }

    // private void HandleCollisionEvent(object sender, CollisionArgs e)
    // {
    //     Bullet bullet = (Bullet)sender;
    //     bullet.RaiseCollisionEvent -= HandleCollisionEvent;
    //     Debug.Log(e.Tag);
    // }
}