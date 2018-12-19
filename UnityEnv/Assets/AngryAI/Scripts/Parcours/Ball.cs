using UnityEngine;

public class Ball : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Tags.CLEAR))
        {
            Destroy(gameObject);
        }
    }
}
