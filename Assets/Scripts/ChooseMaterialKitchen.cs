using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMaterialKitchen : MonoBehaviour
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
            UIManager.Instance.KitchenWindow.chooseMaterialWindow.SetActive(false);
            UIManager.Instance.KitchenWindow.UpdateWindow();
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
            case "FuelKitchen":
                UIManager.Instance.KitchenWindow.kitchen.SetFuel(resourceIcon.currentResourceGO, currentCount);
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
                if (UIManager.Instance.KitchenWindow.kitchen.currentItem != null)
                {
                    UIManager.Instance.KitchenWindow.kitchen.GetFoodBack();
                    UIManager.Instance.KitchenWindow.chooseMaterialWindow.SetActive(false);
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
