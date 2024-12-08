using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMaterial : MonoBehaviour
{
    public Image icon;
    public GameObject ChooseCountWindowSlider;
    public Slider slider;
    public TextMeshProUGUI currentCountText;

    public ResourceIcon resourceIcon;

    public int maxCount;
    private int currentCount;

    public string type;

    public Button applyButton;

    private void Awake()
    {
        applyButton.onClick.AddListener(delegate
        {
            AddResource();
            UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
            UIManager.Instance.furnaceWindow.UpdateWindow();
        });
        GetComponent<Button>().onClick.AddListener(delegate
        {
            OnClickMaterial();
        });
    }


    public void OnSliderChange()
    {
        maxCount = resourceIcon.GetCount();
        currentCount = (int)(slider.value*maxCount);
        currentCountText.text = currentCount.ToString();
    }

    private void AddResource()
    {
        switch (type)
        {
            case "Fuel":
                UIManager.Instance.furnaceWindow.furnace.SetFuel(resourceIcon, currentCount);
                break;
            
            case "Material":
                UIManager.Instance.furnaceWindow.furnace.SetMaterial(resourceIcon, currentCount);
                break;
            
            case "FuelKitchen":
                UIManager.Instance.KitchenWindow.kitchen.SetFuel(resourceIcon.currentResourceGO, currentCount);
                break;
            
            case "FoodTable":
                UIManager.Instance.tableWindow.table.SetFuel(resourceIcon, currentCount);
                break;
        }
    }

    private void OnClickMaterial()
    {
        switch (type)
        {
            case "Fuel":
                if (UIManager.Instance.furnaceWindow.furnace.currentFuel.Count > 0)
                {
                    UIManager.Instance.furnaceWindow.furnace.GetFuelBack();
                    UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
                    return;
                }
                break;
            
            case "Material":
                if (UIManager.Instance.furnaceWindow.furnace.currentMaterial.Count > 0)
                {
                    UIManager.Instance.furnaceWindow.furnace.GetMaterialBack();
                    UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
                    return;
                }
                break;
        }
        
        ChooseMaterial[] mats = GameObject.FindObjectsByType<ChooseMaterial>(FindObjectsSortMode.None);
        foreach (var mat in mats)
        {
            mat.ChooseCountWindowSlider.SetActive(false);
        }
        ChooseCountWindowSlider.SetActive(true);
    }
}
