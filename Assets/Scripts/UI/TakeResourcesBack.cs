using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeResourcesBack : MonoBehaviour
{
    public string type;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            AddResource();
            UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
            UIManager.Instance.furnaceWindow.UpdateWindow();
        });

    }
    
    private void AddResource()
    {
        switch (type)
        {
            case "Fuel":
                UIManager.Instance.furnaceWindow.furnace.GetFuelBack();
                break;
            
            case "Material":
                UIManager.Instance.furnaceWindow.furnace.GetMaterialBack();
                break;
        }
    }
}
