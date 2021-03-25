using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    public bool debugClicks = false;
    private void Update()
    {
        if (debugClicks == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log("Clicked on " + hit.transform.gameObject.name);
                }
            }
        }
    }
}
