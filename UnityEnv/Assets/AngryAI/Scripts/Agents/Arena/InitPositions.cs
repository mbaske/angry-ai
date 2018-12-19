using UnityEngine;

[ExecuteInEditMode]
public class InitPositions : MonoBehaviour
{
    [SerializeField]
    private float radius = 50;
    [SerializeField]
    private float height = 1;

    private void OnValidate()
    {
        int n = transform.childCount;
        float incr = Mathf.PI * 2f / (float)n;
        float angle = 0f;

        for (int i = 0; i < n; i++)
        {
            Transform t = transform.GetChild(i);
            t.position = new Vector3
            (
                radius * Mathf.Cos(angle),
                height,
                radius * Mathf.Sin(angle)
            );
            t.LookAt(Vector3.up * height);
            angle += incr;
        }
    }
}