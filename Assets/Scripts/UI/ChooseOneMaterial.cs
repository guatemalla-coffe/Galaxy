using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChooseOneMaterial : MonoBehaviour
{
    public Image icon;

    public ResourceIcon resourceIcon;
    public Craftable Craftable;

    public string type;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            AddResource();
        });
    }
    

    private void AddResource()
    {
        switch (type)
        {
            case "Flower":
                UIManager.Instance.beehiveWindow.beehive.AddFlower(resourceIcon.currentResourceGO);
                UIManager.Instance.beehiveWindow.chooseMaterialWindow.SetActive(false);
                TaskManager.Instance.CompleteTask(9);
                break;
            
            case "Material":
                UIManager.Instance.KitchenWindow.kitchen.SetFood(Craftable);
                UIManager.Instance.KitchenWindow.chooseMaterialWindow.SetActive(false);
                //TaskManager.Instance.CompleteTask(9);
                break;
        }
    }
    
}

