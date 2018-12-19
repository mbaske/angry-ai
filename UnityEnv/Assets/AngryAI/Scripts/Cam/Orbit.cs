using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform Target;

    [SerializeField]
    private float radius = 20f;
    [SerializeField]
    private float height = 10f;
    [SerializeField]
    private float speed = 0.5f;
    [SerializeField]
    private float outerBound = 50f;

    private Vector3 position;
    private float angle;
    private float itplHeight;
    private int targetFrameRate;
    private bool active = true;

    private void Start()
    {
        targetFrameRate = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                speed += Input.GetAxis("Horizontal") * Time.deltaTime;
            }
            else
            {
                float t = Time.deltaTime * 10f; // TBD
                radius += Input.GetAxis("Horizontal") * t;
                height += Input.GetAxis("Vertical") * t;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                active = !active;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                speed *= -1f;
            }
        }

        angle += speed * Time.deltaTime;

        position = Target.position + new Vector3
        (
            radius * Mathf.Cos(angle),
            25f, // raycast origin height
            radius * Mathf.Sin(angle)
        );

        Vector3 center = Vector3.up * position.y;
        float distance = Vector3.Distance(center, position);
        if (distance > outerBound)
        { 
            position = Vector3.Lerp(center, position, outerBound / distance); 
        }

        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, 50f, Layers.GROUND))
        {
            itplHeight = Mathf.Lerp(itplHeight, hit.point.y + height, 0.1f);
        }
        position.y = itplHeight;
    }

    private void LateUpdate()
    {
        if (active)
        {
            transform.position = Vector3.Lerp(transform.position, position, 
                                              Time.deltaTime * targetFrameRate);
            transform.LookAt(Target);
        }
    }
}