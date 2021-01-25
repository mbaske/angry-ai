using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MBaske
{
    public class AudioFX : Poolable
    {
        protected static AudioClip GetRandomClip(int index, string directory)
        {
            if (!s_Clips.TryGetValue(index, out AudioClip[] clips))
            {
                clips = Array.ConvertAll(
                    Resources.LoadAll(directory, typeof(AudioClip)),
                    clip => (AudioClip)clip);

                s_Clips.Add(index, clips);
            }

            return clips[Random.Range(0, clips.Length)];
        }

        protected static Dictionary<int, AudioClip[]> 
            s_Clips = new Dictionary<int, AudioClip[]>();

        [SerializeField]
        protected string m_Directory;
        protected AudioSource m_Audio;

        private void Awake()
        {
            m_Audio = GetComponent<AudioSource>();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            m_Audio.PlayOneShot(GetRandomClip(Index, m_Directory));
        }
    }
}