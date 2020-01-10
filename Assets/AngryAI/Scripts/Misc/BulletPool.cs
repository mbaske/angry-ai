using System.Collections.Generic;
using UnityEngine;

namespace MBaske.AngryAI
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField]
        private Bullet prefab;
        private Stack<Bullet> inactive;

        private void Awake()
        {
            inactive = new Stack<Bullet>();
        }

        public Bullet Shoot(Vector3 spawnPos, Vector3 direction)
        {
            Bullet bullet = Spawn();
            bullet.transform.position = spawnPos;
            bullet.Shoot(direction);
            return bullet;
        }

        public void Discard(Bullet bullet)
        {
            bullet.Callback = null;
            bullet.OwnerBody = null;
            bullet.gameObject.SetActive(false);
            inactive.Push(bullet);
        }

        private Bullet Spawn()
        {
            Bullet bullet;
            if (inactive.Count > 0)
            {
                bullet = inactive.Pop();
                bullet.gameObject.SetActive(true);
                return bullet;
            }
            bullet = Instantiate(prefab, transform).GetComponent<Bullet>();
            bullet.Pool = this;
            return bullet;
        }
    }
}