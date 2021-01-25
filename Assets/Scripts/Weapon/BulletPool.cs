using UnityEngine;

namespace MBaske
{
    public class BulletPool : Pool<Bullet>
    {
        public void Shoot(Weapon weapon)
        {
            ((Bullet)Spawn(weapon.Position)).Shoot(weapon);
        }
    }
}