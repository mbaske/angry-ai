using System.Collections.Generic;
using UnityEngine;

namespace MBaske
{
    public class GUIStats : MonoBehaviour
    {
        [SerializeField]
        private int m_BufferSize = 128;

        private Dictionary<string, StatsBuffer> m_Buffers;
        private GraphFactory m_GraphFactory;

        private void Awake()
        {
            m_Buffers = new Dictionary<string, StatsBuffer>();
            m_GraphFactory = FindObjectOfType<GraphFactory>(true);
        }

        public void Clear()
        {
            foreach (var buffer in m_Buffers.Values)
            {
                buffer.Clear();
            }
        }

        public float Add(float value, string graphName, string valueLabel, string color, bool showValue = true)
        {
            return Add(value, graphName, valueLabel, Colors.Parse(color), showValue);
        }

        public float Add(float value, string graphName, string valueLabel, Color color, bool showValue = true)
        {
            var key = graphName + valueLabel;

            if (!m_Buffers.TryGetValue(key, out StatsBuffer buffer))
            {
                buffer = new StatsBuffer(m_BufferSize, valueLabel);
                m_Buffers.Add(key, buffer);

                if (!m_GraphFactory.HasGraph(graphName, out Graph graph))
                {
                    graph = m_GraphFactory.AddGraph(graphName);
                }

                graph.Add(buffer, color, showValue);
            }

            buffer.Add(value);
            return value;
        }
    }
}
