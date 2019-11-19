using UnityEngine;

public class RockModifier : MonoBehaviour
{
    [SerializeField]
    private Material[] materials;

    private void Start()
    {
        Search(transform);
    }   

    private void Search(Transform t)
    {
        if (t.childCount == 0)
        {
            t.GetComponent<MeshRenderer>().sharedMaterial = materials[Random.Range(0, 4)];
        }
        else
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Search(t.GetChild(i));
            }
        }
    }
}

