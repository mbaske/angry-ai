using UnityEngine;

namespace MBaske
{
    public static class Util
    {
        public static Vector3 GetGroundPos(Vector3 origin)
        {
            if (Physics.Raycast(origin + Vector3.up * 10, Vector3.down,
                out RaycastHit hit, 100, Layer.GroundMask))
            {
                //Debug.DrawLine(origin, hit.point, Color.magenta);
                return hit.point;
            }
            Debug.LogWarning("Ground detection failed");
            origin.y = 0;
            return origin;
        }

        public static float GetHeightAboveGround(Vector3 origin)
        {
            return origin.y - GetGroundPos(origin).y;
        }

        public static bool RandomBool(float probability = 0.5f)
        {
            return Random.value < probability;
        }


        public static Vector3 Position(Matrix4x4 matrix)
        {
            return new Vector3(matrix[0, 3], matrix[1, 3], matrix[2, 3]);
        }

        public static Vector3 Forward(Matrix4x4 matrix)
        {
            return new Vector3(matrix[0, 2], matrix[1, 2], matrix[2, 2]);
        }

        public static Vector3 Localize(Matrix4x4 matrix, Vector3 point)
        {
            return matrix.inverse.MultiplyPoint3x4(point);
        }


        private static readonly Camera s_Cam = Camera.main;

        public static bool ViewportContains(Vector3 pos)
        {
            pos = s_Cam.WorldToViewportPoint(pos);
            return pos.x >= 0 && pos.x <= 1
                && pos.y >= 0 && pos.y <= 1
                && pos.z >= 0;
        }

        public static bool IsInFrontOfCamera(Vector3 pos)
        {
            return s_Cam.WorldToViewportPoint(pos).z > 0;
        }
    }
}