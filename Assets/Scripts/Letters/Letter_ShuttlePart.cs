using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter_ShuttlePart : MonoBehaviour
{
    public string ansName;
    public void DoOnClick()
    {
        UIManager.Instance.radioWindow.radio.StartWork(ansName);
    }
}
