using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Sort()
    {
        try
        {
            _spriteRenderer.sortingOrder = (int)-(transform.position.y * 100f);
        }
        catch (Exception e)
        {
            
        }
    }
}
