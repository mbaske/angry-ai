using UnityEngine;

public class Barrier : MonoBehaviour
{
    public bool Active
    {
        set { gameObject.SetActive(value); }
        get { return gameObject.activeSelf; }
    }

    public float Width
    {
        set { transform.localScale = Util.SetX(transform.localScale, value); }
        get { return transform.localScale.x; }
    }

    public float X
    {
        set { transform.localPosition = Util.SetX(transform.localPosition, value); }
        get { return transform.localPosition.x; }
    }
}
