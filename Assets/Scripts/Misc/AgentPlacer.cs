using UnityEngine;
using System.Collections.Generic;

namespace MBaske
{
    public class AgentPlacer : MonoBehaviour
    {
        [SerializeField]
        private float m_Radius;
        [SerializeField]
        private float m_Spacing;
        [SerializeField]
        private float m_Height;
        [SerializeField]
        private bool m_Spot;

        private void OnValidate()
        {
            var agents = FindObjectsOfType<FighterAgent>();
            var transforms = new List<Transform>();

            foreach (var agent in agents)
            {
                if (agent.name.Contains(m_Spot ? "Spot" : "GunBot"))
                {
                    transforms.Add(agent.transform);
                }
            }

            var dir = Vector3.forward * m_Radius;
            var arc = m_Spacing * (transforms.Count - 1) * -0.5f;

            for (int i = 0; i < transforms.Count; i++)
            {
                var rot = Quaternion.AngleAxis(arc + m_Spacing * i + (m_Spot ? 180 : 0), Vector3.up);
                var pos = rot * dir;
                pos.y = Util.GetGroundPos(pos).y + m_Height;
                transforms[i].position = pos;
                transforms[i].rotation = rot * Quaternion.AngleAxis(180, Vector3.up);
            }
        }
    }
}