using UnityEngine;

public class MouseControls : MonoBehaviour
{
    [SerializeField]
    private float turn = 5f;
    [SerializeField]
    private float pan = -2f;
    [SerializeField]
    private float zoom = 10f;

    private Vector3 pos;
    private Camera cam;
    private float lockX = 100; // flag no lock
    private float lockY = 100; // flag no lock

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.ScreenToViewportPoint(Input.mousePosition - pos);

        if (Input.GetKeyDown(KeyCode.X))
        {
            lockX = delta.x;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            lockX = 100;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            lockY = delta.y;
        }
        else if (Input.GetKeyUp(KeyCode.Y))
        {
            lockY = 100;
        }

        delta.x = lockX < 100 ? lockX : delta.x;
        delta.y = lockY < 100 ? lockY : delta.y;

        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(delta, transform.right, -delta.y * turn);
            transform.RotateAround(delta, Vector3.up, delta.x * turn);
        }
        else if (Input.GetMouseButton(1))
        {
            transform.Translate(delta * pan, Space.Self);
        }
        else
        {
            pos = Input.mousePosition;
            transform.Translate(Input.GetAxis("Mouse ScrollWheel") 
                                * transform.forward * zoom, Space.World);
        }
    }
}