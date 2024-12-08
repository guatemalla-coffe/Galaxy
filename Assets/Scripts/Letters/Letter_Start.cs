using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter_Start : MonoBehaviour
{
    public void DoOnClick()
    {
        UIManager.Instance.radioWindow.radio.StartWork("Answer_0");
        TaskManager.Instance.CompleteTask(21);

    }
}
