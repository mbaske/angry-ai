using UnityEngine;

namespace MBaske
{
    public class GroundRayDetection : MonoBehaviour
    {
        public float GetGroundDistance()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1, Layer.GroundMask))
            {
                //Debug.DrawLine(transform.position, hitInfo.point, Color.red);
                return hitInfo.distance * 2 - 1;
            }
            //else
            //{
            //    Debug.DrawRay(transform.position, Vector3.down, Color.grey);
            //}

            return 1;
        }
    }
}