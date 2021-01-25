using UnityEngine;

public class GroundContactDetection : MonoBehaviour
{
    public bool HasGroundContact { get; private set; }

    private const int c_Layer = 3;

    public void ManagedReset()
    {
        HasGroundContact = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == c_Layer)
        {
            HasGroundContact = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == c_Layer)
        {
            HasGroundContact = false;
        }
    }
}
