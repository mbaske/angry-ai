using UnityEngine;

public class Step : MonoBehaviour
{
    private Color col;
    private Material mat;

    public void Randomize(bool active, float maxHeight)
    {
        gameObject.SetActive(active && Util.RndBool());

        if (gameObject.activeSelf)
        {
            if (mat == null)
            {
                mat = GetComponent<Renderer>().material;
                col = mat.color;
            }

            Vector3 s = new Vector3
            (
                Random.Range(0.1f, 0.5f),
                Random.Range(0.01f, maxHeight),
                Random.Range(0.1f, 1.2f)
            );
            transform.localScale = s;

            transform.localPosition = new Vector3
            (
                Random.Range(-0.5f + s.x / 2f, 0.5f - s.x / 2f),
                0.5f + s.y / 2f,
                Random.Range(-0.25f, 0.25f)
            );

            mat.color = col * (s.y / maxHeight);
        }
    }
}
