using UnityEngine;
using System;

public class Leg : MonoBehaviour
{
    // Full rotation and motion range is from -range to +range.
    private float rotRangeA = 30f;
    private float posRangeA = 0.3f;
    private float rotRangeB = 25f; 
    private float posRangeB = 0.4f;

    private Transform inner;
    private Transform outer;
    private Transform body;
    private ConfigurableJoint innerJoint;
    private ConfigurableJoint outerJoint;
    private Vector3 link;
    private float offset;
    private float[] obs;

    public void Initialize()
    {
        obs = new float[5];

        inner = transform.Find("LegA");
        innerJoint = inner.GetComponent<ConfigurableJoint>();
        GetInnerLeg().Initialize();

        outer = transform.Find("LegB");
        outerJoint = outer.GetComponent<ConfigurableJoint>();
        GetOuterLeg().Initialize();

        body = innerJoint.connectedBody.transform;
        link = Vector3.forward * inner.localScale.z * 0.5f;
        offset = Vector3.Distance(body.position, inner.position);
    }

    public void ReSet()
    {
        StepUpdate(new float[4]);
        GetInnerLeg().ReSet();
        GetOuterLeg().ReSet();
    }

    public void StepUpdate(float[] targets)
    {
        try
        {
            innerJoint.targetRotation = Quaternion.Euler(0f, 0f, targets[0] * rotRangeA);
            innerJoint.targetPosition = Vector3.back * targets[1] * posRangeA;
            outerJoint.targetRotation = Quaternion.Euler(targets[2] * rotRangeB, 0f, 0f);
            outerJoint.targetPosition = Vector3.up * targets[3] * posRangeB;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
    }

    public PhysicsBody GetInnerLeg()
    {
        return inner.GetComponent<PhysicsBody>();
    }

    public PhysicsBody GetOuterLeg()
    {
        return outer.GetComponent<PhysicsBody>();
    }

    public float[] GetNormalizedObs()
    {
        // Inner leg rotation.
        obs[0] = Mathf.Clamp(
            Vector3.SignedAngle(inner.up, body.up, body.forward) / rotRangeA,
            -1f, 1f);
        // Inner leg position.
        obs[1] = Mathf.Clamp(
            (Vector3.Distance(body.position, inner.position) - offset) / posRangeA,
            -1f, 1f);
        // Outer leg rotation.
        obs[2] = Mathf.Clamp(
            Vector3.SignedAngle(outer.forward, inner.forward, -inner.up) / rotRangeB,
            -1f, 1f);
        // Outer leg position.
        // TODO
        // Need sign to determine if outer leg is moving up or down relative to inner leg.
        // Can't use localPosition since they are sibling objects.
        Vector3 p = inner.TransformPoint(link);
        float sign = Mathf.Sign(outer.InverseTransformPoint(p).y);
        obs[3] = Mathf.Clamp(
            Vector3.Distance(p, outer.position) / posRangeB * sign,
            -1f, 1f);

        obs[4] = Util.GetGroundDistance(outer);
        // Debug.Log(string.Join(", ", obs));
        return obs;
    }
}
