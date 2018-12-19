using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Arena : MonoBehaviour
{
    [SerializeField]
    private float scale = 2f;
    [SerializeField]
    private float height = 5f;
    [SerializeField]
    [Range(0.1f, 3f)]
    private float slope = 0.5f;
    [SerializeField]
    private Vector2 offset = Vector2.zero;

    private float cutoff = 62f;

    private void OnValidate()
    {
        GameObject ground = GameObject.FindWithTag(Tags.GROUND);
        if (ground)
        {
            Mesh mesh = ground.GetComponent<MeshFilter>().sharedMesh;
            float r = mesh.bounds.size.x / 2f;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                float d = Vector3.Distance(Vector3.zero, vertices[i]);
                if (d > cutoff)
                {
                    vertices[i] = Vector3.Lerp(Vector3.zero, vertices[i], cutoff / d);
                }
                else
                {
                    d = Mathf.Pow(1f - d / r, slope);
                    float y = Mathf.PerlinNoise(
                        offset.x + vertices[i].x / r * scale,
                        offset.y + vertices[i].z / r * scale
                    );
                    vertices[i].y = y * height * d;
                }
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
}
