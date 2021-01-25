using UnityEngine;

namespace MBaske
{
    public class GizmoAxes : MonoBehaviour
    {
        [SerializeField]
        private bool draw = true;
        [SerializeField]
        private float length = 1;

        [SerializeField]
        private Color right = Color.red;
        [SerializeField]
        private Color up = Color.green;
        [SerializeField]
        private Color forward = Color.blue;


        private void OnDrawGizmos()
        {
            if (draw)
            {
                Gizmos.color = right;
                Gizmos.DrawRay(transform.position, transform.right * length);
                Gizmos.color = up;
                Gizmos.DrawRay(transform.position, transform.up * length);
                Gizmos.color = forward;
                Gizmos.DrawRay(transform.position, transform.forward * length);
            }
        }
    }
}