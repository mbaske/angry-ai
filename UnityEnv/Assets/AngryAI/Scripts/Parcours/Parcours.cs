using System.Collections.Generic;
using UnityEngine;

public class Parcours : MonoBehaviour
{
    // Needs to be considered when determining an agent's relative velocity.
    // The tiles might be moving while the agent is running in place.
    public Vector3 Velocity => avgTileVelocity;
    public Transform Goal => goal;

    [SerializeField]
    private ParcoursSettings settings;

    // Used for triggering interval based items like barriers and balls.
    private int tileCount;
    // Used for animating goal and slope angle.
    private float osc;
    private float slopeAngle;
    // Combined length of all tiles. 
    private float length;
    // Z-pos at which tiles are shifted back.
    private float maxZ;
    private Vector2 forward2D;
    // Needs to be considered when determining the distance an agent has covered.
    private Vector2 accumOffset2D;

    private List<Tile> tiles;
    private Vector3 avgTileVelocity;
    private Transform ramp;
    private ConfigurableJoint joint;
    private Transform goal;
    private Vector3 goalPos;
    private Transform box;
    private Transform wallF;
    private Transform wallB;
    private Transform wallL;
    private Transform wallR;
    private float boxZOffset;
    private float boxYOffset;
    private float boxYSinMult;
    private int targetFrameRate;

    public void Initialize()
    {
        ramp = transform.Find("Ramp");
        joint = ramp.GetComponent<ConfigurableJoint>();
        goal = ramp.Find("Goal");
        goalPos = goal.localPosition;
        forward2D = Util.Flatten(transform.forward);

        box = transform.Find("Box");
        wallF = box.Find("Front");
        wallB = box.Find("Back");
        wallL = box.Find("Left");
        wallR = box.Find("Right");
        boxZOffset = wallF.localPosition.z;
        boxYOffset = wallF.localScale.y / 2f - 3f;
        boxYSinMult = boxYOffset / Mathf.Sin(settings.MAX_SLOPE * Mathf.Deg2Rad);
        targetFrameRate = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;

        UpdateSlope();
        CreateTiles();
    }

    public void ReSet()
    {
        if (settings.AutoSpeed)
        {
            settings.Speed = 0f;
        }
        osc = 0f;
        tileCount = 0;
        accumOffset2D = Vector2.zero;

        ResetGoal(false);
        UpdateSlope();
    }

    public void ResetGoal(bool zOnly = true)
    {
        goal.localPosition = zOnly ? Util.SetZ(goal.localPosition, goalPos.z)
                                   : goalPos;
    }

    public void StepUpdate(Vector3 agentPos)
    {
        if (settings.AutoSpeed)
        {
            agentPos = ramp.InverseTransformPoint(agentPos);
            float s = Mathf.Max(0f, (agentPos.z + 0.2f) * 12f); // TBD
            settings.Speed = Mathf.Lerp(settings.Speed, s, 0.25f);
        }

        // Increment factors are set so that settings.Speed roughly matches avgTileVelocity.
        float incr = settings.Speed * Time.fixedDeltaTime * 4f;
        int i = 0;
        avgTileVelocity = Vector3.zero;
        foreach (Tile tile in tiles)
        {
            if (tile.Active)
            {
                i++;
                avgTileVelocity += tile.Velocity;
            }
            else
            {
                tile.ActivateIfSettled();
            }

            if (!tile.Move(incr, maxZ))
            {
                tile.ShiftBack(length, tileCount++);
            }
        }
        goal.Translate(0f, 0f, -incr);
        avgTileVelocity /= i;
        accumOffset2D += forward2D * incr;

        UpdateGoal();
        UpdateSlope();
        osc += Time.fixedDeltaTime;
    }

    private void CreateTiles()
    {
        Tile tile = transform.GetComponentInChildren<Tile>();
        tiles = new List<Tile>() { tile }; // Template.
        float rampZ = ramp.localScale.z;
        float tileZ = tile.transform.localScale.z;
        float offsetZ = rampZ / -2f;
        int nTiles = Mathf.FloorToInt(rampZ / tileZ);
        length = nTiles * tileZ;
        maxZ = offsetZ + length + 1f; // TBD

        for (int i = 1; i < nTiles; i++)
        {
            tiles.Add(Instantiate(tile, transform));
        }
        for (int i = 0; i < nTiles; i++)
        {
            tiles[i].Initialize(settings, offsetZ + i * tileZ);
        }
    }

    private void UpdateGoal()
    {
        goal.localPosition = Util.SetX(
            goal.localPosition, Mathf.Sin(osc * settings.GoalOscillator * Time.deltaTime
            * targetFrameRate) * 0.45f);
    }

    private void UpdateSlope()
    {
        float deg = (settings.SlopeOscillator > 0f
            ? Mathf.Sin(osc * settings.SlopeOscillator * Time.deltaTime * targetFrameRate)
            : 1f) * settings.SlopeAngle;

        float tmp = slopeAngle;
        slopeAngle = Mathf.Lerp(slopeAngle, deg, 0.05f);

        if (Mathf.Abs(tmp - slopeAngle) > Mathf.Epsilon)
        {
            joint.targetRotation = Quaternion.Euler(slopeAngle, 0f, 0f);

            float rad = Mathf.Abs(slopeAngle * Mathf.Deg2Rad);
            // Box shape follows slope angle.
            box.localPosition = Util.SetY(
                box.localPosition, -boxYOffset + Mathf.Sin(rad) * boxYSinMult);
            wallF.localPosition = Util.SetZ(
                wallF.localPosition, boxZOffset * Mathf.Cos(rad));
            wallB.localPosition = Util.SetZ(
                wallB.localPosition, -wallF.localPosition.z);
            wallL.localScale = Util.SetZ(
                wallL.localScale, (wallF.localPosition.z + wallF.localScale.z / 2f) * 2f);
            wallR.localScale = Util.SetZ(
                wallR.localScale, wallL.localScale.z);
        }
    }

    private void OnValidate()
    {
        settings.MaxBarrierWidth = Mathf.Max(settings.MinBarrierWidth, settings.MaxBarrierWidth);
    }
}
