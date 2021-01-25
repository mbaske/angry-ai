using System;
using UnityEngine;

namespace MBaske
{
    public class BodyCollisionTrigger : MonoBehaviour
    {
        public event Action BulletHitSufferedEvent;
        public event Action<int, bool> CollisionEvent;

        public void InvokeBulletHitSufferedEvent()
        {
            BulletHitSufferedEvent?.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckCollision(other.gameObject.layer, true);

            // CHANGED invoked by bullet.
            //if (other.gameObject.layer == Layer.Bullet)
            //{
            //    if (!transform.CompareTag(other.GetComponent<Bullet>().AgentTag))
            //    {
            //        // Ignores friendly fire.
            //        BulletHitSufferedEvent?.Invoke();
            //    }
            //}
        }

        private void OnTriggerStay(Collider other)
        {
            CheckCollision(other.gameObject.layer, false);
        }

        private void CheckCollision(int layer, bool isEnter)
        {
            if (layer == Layer.Obstacle || layer == Layer.Trigger)
            {
                CollisionEvent?.Invoke(layer, isEnter);
            }
        }
    }
}
