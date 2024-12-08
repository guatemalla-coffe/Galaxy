using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isPlased;
    public virtual void OpenMenu() {}

    public virtual void Place() {}

    public async void Start()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        GetComponent<SortingOrder>().Sort();
    }
}
