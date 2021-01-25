using UnityEngine;
using System;

namespace MBaske
{
    public class Oscillator : MonoBehaviour
    {
        [Serializable]
        private struct OscBone
        {
            public int Phase;
            public Bone Bone;
        }

        [Serializable]
        private struct OscGroup
        {
            public float Scale;
            public OscBone[] Bones;
        }

        [Serializable]
        private struct OscMode
        {
            public float TargetAngle;
            public float MeasuredSpeed;
            public float Frequency;
            public OscGroup[] Groups;
        }

        [SerializeField]
        private OscMode[] m_Modes;

        [SerializeField, Tooltip("Set to -1 for cycling modes")] 
        private int m_ModeIndex = -1;
        private bool m_CycleModes;

        private float m_Time;
        private const float c_PI2 = Mathf.PI * 2;

        private void Awake()
        {
            m_CycleModes = m_ModeIndex == -1;
        }

        public float GetTargetAngle()
        {
            return m_Modes[m_ModeIndex].TargetAngle;
        }

        public float GetTargetSpeed()
        {
            return m_Modes[m_ModeIndex].MeasuredSpeed;
        }

        public void ManagedReset()
        {
            m_Time = 0;

            if (m_CycleModes)
            {
                m_ModeIndex = ++m_ModeIndex % m_Modes.Length;
            }
        }

        public void ManagedUpdate()
        {
            var mode = m_Modes[m_ModeIndex];

            m_Time += Time.fixedDeltaTime * mode.Frequency;
            float cos = Mathf.Cos(m_Time * c_PI2);

            foreach (var group in mode.Groups)
            {
                float amp = cos * group.Scale;
                foreach (var bone in group.Bones)
                {
                    bone.Bone.HeuristicAction = amp * bone.Phase;
                }
            }
        }
    }
}