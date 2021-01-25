using System;
using UnityEngine;

namespace MBaske
{
    public class BoneMotionStats
    {
        private int m_CrntDirection;
        private int m_PrevPhaseChangeStep;

        private readonly int[] m_PhaseDurations;
        private int m_Index;
        private bool skipFirst;

        public BoneMotionStats(int bufferSize = 8)
        {
            m_PhaseDurations = new int[bufferSize];
        }

        public void Reset()
        {
            Array.Clear(m_PhaseDurations, 0, m_PhaseDurations.Length);

            m_Index = 0;
            m_CrntDirection = 0;
            m_PrevPhaseChangeStep = 0;
            skipFirst = true;
        }

        public void Update(Bone bone, int step)
        {
            if (bone.Direction != m_CrntDirection)
            {
                if (m_CrntDirection != 0)
                {
                    if (skipFirst)
                    {
                        // partial phase
                        skipFirst = false;
                    }
                    else
                    {
                        m_PhaseDurations[m_Index] = step - m_PrevPhaseChangeStep;
                        m_Index = ++m_Index % m_PhaseDurations.Length;
                    }
                }

                m_CrntDirection = bone.Direction;
                m_PrevPhaseChangeStep = step;
            }
        }

        public bool HasValues(out float phaseDurationAvg, out float phaseDurationStD, bool relative = true)
        {
            phaseDurationAvg = 0;
            phaseDurationStD = 0;

            int n = 0;
            float mean = 0;
            float sum = 0;

            foreach (var val in m_PhaseDurations)
            {
                if (val > 0)
                {
                    n++;
                    float delta = val - mean;
                    mean += delta / n;
                    sum += delta * (val - mean);
                    phaseDurationAvg += val;
                }
            }

            if (n > 1)
            {
                phaseDurationAvg /= n;
                phaseDurationStD = Mathf.Sqrt(sum / (n - 1));

                if (relative)
                {
                    phaseDurationStD /= phaseDurationAvg;
                }
            }

            return n > 0;
        }
    }
}