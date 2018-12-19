using UnityEngine;

[System.Serializable]
public class ParcoursSettings
{
    [Tooltip("Match agent speed")]
    public bool AutoSpeed = true;

    [Tooltip("Current speed")]
    [Range(0f, 4f)]
    public float Speed = 0f;
    
    [Space]
    [Tooltip("Slope angle in degrees")]
    [Range(-20f, 20f)]
    public float SlopeAngle = 0f;
    [Tooltip("Animate slope")]
    [Range(0f, 1f)]
    public float SlopeOscillator = 0f;
    [HideInInspector]
    public float MAX_SLOPE = 20f;

    [Space]
    [Tooltip("Maximum number of steps per tile")]
    [Range(0, 10)] 
    public int MaxStepsNum = 0; 

    [Tooltip("Maximum step height")]
    [Range(0.1f, 1f)]
    public float MaxStepHeight = 0f;

    [Space]
    [Tooltip("Barrier every nth tile")]
    [Range(0, 12)] // TODO
    public int BarrierInterval = 0; 

    [Tooltip("Ratio of parcours width")]
    [Range(0f, 1f)]
    public float MinBarrierWidth = 0f;

    [Tooltip("Ratio of parcours width")]
    [Range(0f, 1f)]
    public float MaxBarrierWidth = 0f;

    [Space]
    [Tooltip("Drop ball every nth tile")]
    [Range(0, 12)] // TODO
    public int BallDropInterval = 0;

    [Space]
    [Tooltip("Move goal horizontally")]
    [Range(0f, 3f)] 
    public float GoalOscillator = 0;
}