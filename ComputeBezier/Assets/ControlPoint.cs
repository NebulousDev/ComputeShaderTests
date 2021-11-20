using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    private bool down;

    void OnMouseDown()
    {
        if(Input.GetMouseButtonDown(0))
        {
            down = true;
        }
    }

    void OnMouseUp()
    {
        down = false;
    }

    void Update()
    {
        if(down)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        }
    }
}
