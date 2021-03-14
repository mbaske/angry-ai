using UnityEngine;

namespace MBaske
{
    public class AudioFXPool : Pool<AudioFX>
    {
        [SerializeField]
        private bool m_EnableAudioFX;

        public void Shot(Vector3 pos)
        {
            Play(pos, 0);
        }

        public void Stray(Vector3 pos)
        {
            Play(pos, 1);
        }

        public void ImpactGround(Vector3 pos)
        {
            Play(pos, 2);
        }

        public void ImpactLight(Vector3 pos)
        {
            Play(pos, 3);
        }

        public void ImpactMedium(Vector3 pos)
        {
            Play(pos, 4);
        }

        public void ImpactHard(Vector3 pos)
        {
            Play(pos, 5);
        }

        public void ImpactThud(Vector3 pos)
        {
            Play(pos, 6);
        }

        public void Explosion(Vector3 pos)
        {
            Play(pos, 7);
        }

        private void Play(Vector3 pos, int index)
        {
            if (m_EnableAudioFX && Util.IsInFrontOfCamera(pos))
            {
                Spawn(pos, index);
            }
        }
    }
}