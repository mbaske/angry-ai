using UnityEngine;

public class RotatingCam : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float offset;

    void LateUpdate()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
        transform.LookAt(Vector3.up * offset);
    }
}
