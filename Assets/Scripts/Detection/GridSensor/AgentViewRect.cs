using UnityEngine;
using MBaske.Sensors.Grid;
using System.Collections.Generic;

namespace MBaske
{
    public class AgentViewRect : PixelGrid
    {
        private readonly int m_Extent;
        private readonly float m_Radius;
        private readonly string m_OwnerTag;

        private const int c_ColliderMask = Layer.BotMask | Layer.ObstacleMask;
        private const int c_ObservationStacks = 2;
        private int m_StackIndex;

        private static readonly List<Vector3> s_CraterWall = CraterWall();

        public AgentViewRect(string ownerTag, int size = 29) : base(3 * c_ObservationStacks, size, size)
        {
            m_Extent = size / 2;
            m_Radius = Mathf.Sqrt(size * size * 2);
            m_OwnerTag = ownerTag;
        }

        public override void Clear()
        {
            base.Clear();
            m_StackIndex = 0;
        }

        public void Update(Matrix4x4 matrix)
        {
            ClearLayer(m_StackIndex);
            int channel = m_StackIndex * 3;

            // WALL 

            foreach (var wall in s_CraterWall)
            {
                if (Contains(matrix, wall, out Vector2Int gridPos))
                {
                    Write(channel, gridPos, 1);
                }
            }

            // COLLIDERS

            var colliders = Physics.OverlapSphere(Util.Position(matrix), m_Radius, c_ColliderMask);

            foreach (var coll in colliders)
            {
                if (Contains(matrix, coll.transform.position, out Vector2Int gridPos))
                {
                    if (coll.gameObject.layer == Layer.Obstacle)
                    {
                        Write(channel, gridPos, 1);
                    }
                    else if (Tag.IsAgentTag(coll.tag))
                    {
                        var c = channel + (coll.CompareTag(m_OwnerTag) ? 1 : 2);
                        var h = coll.GetComponentInParent<FighterAgent>().Health * 0.5f + 0.5f;

                        Write(c, gridPos, h);
                    }
                }
            }

            m_StackIndex = ++m_StackIndex % c_ObservationStacks;
        }

        private bool Contains(Matrix4x4 matrix, Vector3 pos, out Vector2Int gridPos)
        {
            pos = Util.Localize(matrix, pos);
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.z);

            if (Mathf.Abs(x) <= m_Extent && Mathf.Abs(y) <= m_Extent)
            {
                gridPos = new Vector2Int(x + m_Extent, y + m_Extent);
                return true;
            }

            gridPos = default;
            return false;
        }

        private static List<Vector3> CraterWall()
        {
            const int rMin = 26;
            const int rMax = 29;
            var positions = new List<Vector3>();

            for (int x = -rMax; x <= rMax; x++)
            {
                for (int z = -rMax; z <= rMax; z++)
                {
                    var p = new Vector3(x, 0, z);
                    var r = p.magnitude;

                    if (r >= rMin && r <= rMax)
                    {
                        positions.Add(p);
                    }
                }
            }

            return positions;
        }
    }
}