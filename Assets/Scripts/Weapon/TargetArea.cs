using UnityEngine;
using System;

namespace MBaske
{
    [Serializable]
    public struct TargetArea
    {
        public float Range;
        public float Angle;
        public float Extent;

        private Vector3 aL, bL, aR, bR;

        public bool Contains(Vector3 p)
        {
            var dL = (p.x - aL.x) * (bL.z - aL.z) - (p.z - aL.z) * (bL.x - aL.x);
            var dR = (p.x - aR.x) * (bR.z - aR.z) - (p.z - aR.z) * (bR.x - aR.x);

            return dL > 0 && dR < 0;
        }

        public void Update(Vector3 pos, Vector3 fwdXZ)
        {
            var perp = Vector3.Cross(Vector3.up, fwdXZ);
            aL = pos - perp * Extent;
            aR = pos + perp * Extent;

            bL = aL + Quaternion.AngleAxis(-Angle, Vector3.up) * fwdXZ * Range;
            bR = aR + Quaternion.AngleAxis(Angle, Vector3.up) * fwdXZ * Range;
        }

        public void Draw()
        {
            Debug.DrawLine(aL, bL, Color.cyan);
            Debug.DrawLine(aR, bR, Color.magenta);
            Debug.DrawLine(bL, bR, Color.white);
        }
    }
}
