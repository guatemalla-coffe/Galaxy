using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DynamicSortingOrder : SortingOrder
{
    public int frequency;
    public bool doSort = true;

    private void Awake()
    {
        StartSort();
    }

    private async void StartSort()
    {
        while (doSort)
        {
            Sort();
            await UniTask.Delay(TimeSpan.FromSeconds(frequency/60f));
        }
    }
}
