using UnityEngine;

namespace MBaske
{
    public class SpotPistonAnimation : MonoBehaviour
    {
        private static Quaternion rot = Quaternion.Euler(185, 0, 0);
        private void Update()
        {
            transform.localRotation = Quaternion.Inverse(transform.parent.localRotation) * rot;
        }
    }
}