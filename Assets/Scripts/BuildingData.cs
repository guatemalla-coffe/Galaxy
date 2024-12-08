using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public List<ResourceForBD> price = new List<ResourceForBD>();
    public Sprite iconButton;

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

[System.Serializable] 
public class ResourceForBD
{
    public string name;
    public int count;
}