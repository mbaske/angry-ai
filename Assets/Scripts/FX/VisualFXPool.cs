using UnityEngine;

namespace MBaske
{
    public class VisualFXPool : Pool<VisualFX>
    {
        [SerializeField]
        private bool m_EnableVisualFX;

        public void MuzzleFlash(Vector3 pos)
        {
            Play(pos, 0);
        }

        public void Smoke(Vector3 pos)
        {
            Play(pos, 1);
        }

        public void BotImpact(Vector3 pos)
        {
            Play(pos, 2);
        }

        public void SmallExplosion(Vector3 pos)
        {
            Play(pos, 3);
        }

        public void BotExplosion(Vector3 pos)
        {
            Play(pos, 4);
        }

        private void Play(Vector3 pos, int index)
        {
            if (m_EnableVisualFX && Util.ViewportContains(pos))
            {
                Spawn(pos, index);
            }
        }
    }
}