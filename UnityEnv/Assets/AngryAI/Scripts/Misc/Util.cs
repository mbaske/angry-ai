using UnityEngine;

public static class Util
{
    public static bool RndBool(float f = 0.5f)
    {
        return Random.value < f;
    }

    public static float RndFloat(float f = 0.5f)
    {
        return Random.value < f ? 1f : -1f;
    }

    public static T RndItem<T>(T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static float ClampAngle(float angle)
    {
        angle = ((angle % 360) + 360) % 360;
        return angle > 180 ? angle - 360 : angle;
    }

    public static Vector2 Flatten(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 Elevate(Vector2 v, float y = 0f)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }


    public static Vector3 SetX(Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector2 SetX(Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector3 SetY(Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector2 SetY(Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector3 SetZ(Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }


    public static float GetGroundDistance(Transform tf, bool normalize = true, float range = 5f)
    {
        RaycastHit hit;
        float div = normalize ? range : 1f;
        if (Physics.Raycast(tf.position, -tf.up, out hit, range, Layers.GROUND_AND_OBSTACLE))
        {
            // Debug.DrawLine(tf.position, hit.point, Color.cyan);
            return Vector3.Distance(tf.position, hit.point) / -div;
        }
        else if (Physics.Raycast(tf.position, tf.up, out hit, range, Layers.GROUND_AND_OBSTACLE))
        {
            // Debug.DrawLine(tf.position, hit.point, Color.magenta);
            return Vector3.Distance(tf.position, hit.point) / div;
        }
        return 0f;
    }
}