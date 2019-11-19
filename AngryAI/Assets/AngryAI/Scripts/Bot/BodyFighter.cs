using System.Collections.Generic;
using UnityEngine;

public class BodyFighter : Body
{
    public Fighter Owner { get; private set; }
    public float CumlProximity { get; private set; }

    [SerializeField]
    private Transform shieldFX;

    private List<float> obs;
    private int lmDetect;
    private bool isDemo;

    public void Initialize(Fighter agent)
    {
        base.Initialize();

        Owner = agent;
        obs = new List<float>(89);
        lmDetect = (1 << Layers.BODY) ^ (1 << Layers.HEAD) ^ (1 << Layers.HILL) ^ (1 << Layers.ROCK);
        isDemo = FindObjectOfType<DemoAcademy>() != null;
    }

    public List<float> GetNormalizedObs()
    {
        obs.Clear();
        // Inclination
        obs.Add(transform.right.y);
        obs.Add(transform.up.y);
        obs.Add(transform.forward.y);
        // Localize movement.
        AddVectorObs(obs, AgentUtil.Sigmoid(transform.InverseTransformVector(rb.velocity)));
        AddVectorObs(obs, AgentUtil.Sigmoid(transform.InverseTransformVector(rb.angularVelocity)));

        CumlProximity = 0;
        Vector3 heading = HeadingXZ;
        for (int angle = -60; angle <= 60; angle += 5)
        {
            CastRay(heading, angle);
        }
        for (int angle = 75; angle <= 285; angle += 15)
        {
            CastRay(heading, angle);
        }
        return obs;
    }

    private void CastRay(Vector3 heading, int angle, float distance = 25f, float proximity = 3f)
    {
        // No detection defaults.
        float normDistance = 1f;
        float objTypeInfo = 1f;
        float normProx = 0f;

        Vector3 dir = Quaternion.Euler(0, angle, 0) * heading;
        if (Physics.SphereCast(transform.position, 1f, dir, out RaycastHit hit, distance, lmDetect))
        {
            normDistance = (hit.distance / distance) * 2f - 1f;

            int layer = hit.collider.gameObject.layer;
            if (layer == Layers.HEAD)
            {
                // Discriminte head and body, so bot can determine if it might be targeted.
                objTypeInfo = -1;
            }
            else if (layer == Layers.BODY)
            {
                objTypeInfo = -0.33f;
            }
            else
            {
                // Obstacles like rocks and hills.
                objTypeInfo = 0.33f;
            }

            if (hit.distance < proximity)
            {
                normProx = 1f - hit.distance / proximity;
            }
        }

        obs.Add(normDistance);
        obs.Add(objTypeInfo);
        CumlProximity += normProx;
    }

    private void OnCollisionEnter(Collision other)
    {
        int layer = other.collider.gameObject.layer;
        if (isDemo)
        {
            if (layer == Layers.BULLET)
            {
                Bullet bullet = other.collider.GetComponent<Bullet>();
                shieldFX.LookAt(bullet.OwnerBody.transform.position);
                shieldFX.gameObject.SetActive(true);
                CancelInvoke();
                Invoke("DeactivateShieldFX", 0.1f);
            }
        }
        else if (layer == Layers.HILL)
        {
            isOutOfBounds = true;
        }
    }

    private void DeactivateShieldFX()
    {
        shieldFX.gameObject.SetActive(false);
    }
}
