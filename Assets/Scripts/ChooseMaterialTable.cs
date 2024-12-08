using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMaterialTable : MonoBehaviour
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
            UIManager.Instance.tableWindow.chooseMaterialWindow.SetActive(false);
            UIManager.Instance.tableWindow.UpdateWindow();
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
        UIManager.Instance.tableWindow.table.SetFuel(resourceIcon, currentCount);
    }

    private void OnClickMaterial()
    {
        if (UIManager.Instance.tableWindow.table.currentFuel.Count > 0)
        {
            UIManager.Instance.tableWindow.table.GetFuelBack();
            UIManager.Instance.tableWindow.chooseMaterialWindow.SetActive(false);
            return;
        }
        
        ChooseMaterialTable[] mats = GameObject.FindObjectsByType<ChooseMaterialTable>(FindObjectsSortMode.None);
        foreach (var mat in mats)
        {
            mat.ChooseCountWindowSlider.SetActive(false);
        }
        ChooseCountWindowSlider.SetActive(true);
    }
}
