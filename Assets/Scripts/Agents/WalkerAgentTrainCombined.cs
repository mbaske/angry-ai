using UnityEngine;
using Unity.MLAgents.Sensors;

namespace MBaske
{
    // This class should be used for training walker and fighter agents in tandem.
    public class WalkerAgentTrainCombined : WalkerAgentTrain
    {
        public override void CollectObservations(VectorSensor sensor)
        {
            // Observed target angles are set by fighter agent, 
            // directions are needed for error calculation.
            TargetAnglesToDirections();

            base.CollectObservations(sensor);
        }
    }
}