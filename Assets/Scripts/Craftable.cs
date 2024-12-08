using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craftable : MonoBehaviour
{
    public List<ResourceForBD> price = new List<ResourceForBD>();
    public ResourceIcon resource;
    public float craftDuration = 1f;

    public List<string> GetKeyList()
    {
        List<string> keys = new List<string>(); 
        foreach (var pri in price)
        {
            keys.Add(pri.name);
        }

        return keys;
    }

    public int GetValue(string key)
    {
        foreach (var pri in price)
        {
            if (pri.name == key)
            {
                return pri.count;
            }
        }

        return -1;
    }
}
