using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDownLogger : MonoBehaviour
{
    private void OnMouseDown()
    {
        print("OnMouseDown: " + gameObject.name);
    }
}
