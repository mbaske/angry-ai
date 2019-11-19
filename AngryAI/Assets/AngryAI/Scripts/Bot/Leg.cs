using UnityEngine;

public class Leg : MonoBehaviour
{
    [SerializeField]
    private float[] constraints;

    private Transform[] bones;
    private Vector3[] eulers;

    public void Initialize()
    {
        Transform bone = transform;
        bones = new Transform[4];
        eulers = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            bones[i] = bone;
            eulers[i] = bone.localEulerAngles;
            bone = bone.GetChild(0);
        }
    }

    public void StepUpdate(float p0, float p1, float p2, float p3)
    {
        bones[0].localRotation = Quaternion.Euler(
            eulers[0].x + constraints[0] * p0, eulers[0].y, eulers[0].z);
        bones[1].localRotation = Quaternion.Euler(
            eulers[1].x + constraints[1] * p1, eulers[1].y, eulers[1].z);
        bones[2].localRotation = Quaternion.Euler(
            eulers[2].x, eulers[2].y, eulers[2].z + constraints[2] * p2);
        bones[3].localRotation = Quaternion.Euler(
            eulers[3].x, eulers[3].y, eulers[3].z + constraints[3] * p3);
    }
}
