using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool Active => cldr.enabled;
    public Vector3 Velocity => rb.velocity;

    [SerializeField]
    private Step stepPrefab;
    [SerializeField]
    private Transform ballPrefab;
    private Vector3Int ballDropPos = new Vector3Int(0, 2, -1);

    private ParcoursSettings settings;
    private Vector3 jointPos;
    private ConfigurableJoint joint;
    private Collider cldr;
    private Renderer rend;
    private Rigidbody rb;
    private Barrier barA;
    private Barrier barB;
    private Transform steps; // Container
    private int tileCount;

    public void Initialize(ParcoursSettings settings, float initZPos)
    {
        this.settings = settings;
        cldr = GetComponent<Collider>();
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();
        steps = transform.Find("Steps");
        barA = transform.Find("BarrierA").GetComponent<Barrier>();
        barB = transform.Find("BarrierB").GetComponent<Barrier>();
        SetPosition(initZPos);
    }

    public bool Move(float incr, float maxZ)
    {
        SetPosition(jointPos.z + incr);
        return jointPos.z < maxZ;
    }

    public void ShiftBack(float offset, int n)
    {
        tileCount = n;
        SetActive(false);
        SetPosition(jointPos.z - offset);
    }

    public void ActivateIfSettled()
    {
        // TODO force threshold?
        if (joint.currentForce.sqrMagnitude < 2000000)
        {
            SetActive(true);
        }
    }

    private void SetPosition(float z)
    {
        jointPos.z = z;
        joint.targetPosition = jointPos;
    }

    private void SetActive(bool active)
    {
        if (active)
        {
            // Balls.
            if (settings.BallDropInterval > 0 && tileCount % settings.BallDropInterval == 0)
            {
                Transform ball = Instantiate(ballPrefab,
                    transform.TransformPoint(ballDropPos),
                    Quaternion.identity);
                ball.GetComponent<ConstantForce>().force = transform.forward * -500f;
                ball.GetComponent<Rigidbody>().AddForce(
                    transform.right * (Random.value - 0.5f) * 25f,
                    ForceMode.VelocityChange);
            }

            UpdateSteps();
        }

        // Barriers.
        if (active && settings.BarrierInterval > 0 && settings.MinBarrierWidth > 0f)
        {
            if (tileCount % settings.BarrierInterval == 0)
            {
                UpdateBarriers(Random.Range(settings.MinBarrierWidth, settings.MaxBarrierWidth));
            }
        }
        else
        {
            barA.Active = false;
            barB.Active = false;
        }

        steps.gameObject.SetActive(active);
        cldr.enabled = active;
        rend.enabled = active;
    }

    private void UpdateSteps()
    {
        if (steps.childCount < settings.MaxStepsNum)
        {
            int n = settings.MaxStepsNum - steps.childCount;
            for (int i = 0; i < n; i++)
            {
                Instantiate(stepPrefab, steps);
            }
        }

        for (int i = 0; i < steps.childCount; i++)
        {
            steps.GetChild(i).GetComponent<Step>().Randomize(
                i < settings.MaxStepsNum, settings.MaxStepHeight);
        }
    }

    private void UpdateBarriers(float width)
    {
        if (Util.RndBool(0.75f))
        {
            barA.Active = true;
            barB.Active = false;
            float range = (1f - width) / 2f;
            barA.Width = width;
            barA.X = Random.Range(-range, range);
        }
        else
        {
            barA.Active = true;
            barB.Active = true;
            float wA = (Random.value * 0.8f + 0.1f) * width;
            float wB = width - wA;
            barA.Width = wA;
            barB.Width = wB;
            barA.X = -0.5f + wA / 2f;
            barB.X = 0.5f - wB / 2f;
        }
    }
}
