using UnityEngine;
using System.Collections;

public class TouchToMouse : MonoBehaviour
{
    //Transform beingMoved;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach (Touch touch in Input.touches)
        {
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) continue;
            if (touch.phase == TouchPhase.Began)
                hit.transform.gameObject.SendMessage("OnMouseDown");
        }
    }
}
