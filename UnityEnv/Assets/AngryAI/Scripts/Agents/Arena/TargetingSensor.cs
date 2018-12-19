using UnityEngine;

public class TargetingSensor
{
    public bool IsLocked => Target != null;
    public bool HasObstacle { get; private set; }
    public Vector3 StartPosition => origin.position;
    public Vector3 EndPosition { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Direction { get; private set; }
    public Transform Target { get; private set; }
    public float Range { get; private set; }
    public float MaxAngle { get; private set; }
    public float Angle { get; private set; } 
    public float Alignment { get; private set; } 
    public float Proximity { get; private set; } 
    public float TargetDistance { get; private set; }
    public float ObstacleDistance { get; private set; }
    
    private Transform origin; // Robot body
    private int layerMask;

    public TargetingSensor(Transform origin, float range, float maxAngle)
    {
        this.origin = origin;
        Range = range;
        MaxAngle = maxAngle;
        layerMask = Layers.OBSTACLE | Layers.ROBOT;
        LookAt(null);
    }

    public void Sweep()
    {
        Transform tmpTarget = null;
        float minDistance = Mathf.Infinity;
        Collider[] c = Physics.OverlapSphere(origin.position, Range, Layers.ROBOT);
        for (int i = 0; i < c.Length; i++)
        {
            Transform t = c[i].transform;
            if (t != origin)
            {
                LookAt(t);
                if (Angle <= MaxAngle && TargetDistance < minDistance)
                {
                    tmpTarget = t;
                    minDistance = TargetDistance;
                }
            }
        }
        LookAt(tmpTarget);

        RaycastHit hit;
        HasObstacle = Physics.Raycast(origin.position, Direction, out hit, Range, layerMask) 
                      && (hit.collider.CompareTag(Tags.OBSTACLE));
        ObstacleDistance = HasObstacle ? Vector3.Distance(origin.position, hit.point) 
                                       : Mathf.Infinity;
        EndPosition = hit.point;
        Alignment = Vector3.SignedAngle(origin.forward, Direction, Vector3.up) / MaxAngle;
        Proximity = IsLocked ? 1f - TargetDistance / Range : 0f;
        Proximity = HasObstacle ? -(1f - ObstacleDistance / Range) : Proximity;

        // Color col = IsLocked ? (HasObstacle ? Color.yellow : Color.magenta) 
        //                      : (HasObstacle ? Color.cyan : Color.blue);
        // Debug.DrawRay(origin.position, Direction * Range, col);
    }

    private void LookAt(Transform t)
    {
        Target = t;
        if (t != null)
        {
            Rotation = Quaternion.LookRotation(t.position - origin.position, Vector3.up);
            TargetDistance = Vector3.Distance(origin.position, t.position);
        }
        else
        {
            Rotation = origin.rotation;
            TargetDistance = Mathf.Infinity;
        }
        Direction = Rotation * Vector3.forward;
        Angle = Vector3.Angle(origin.forward, Direction);
    }
}