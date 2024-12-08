using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnaceWindow : MonoBehaviour
{
    public Image furnaceIcon;
    public Image fillRect_Progress;
    public TextMeshProUGUI textProgress;

    public GameObject chooseMaterialWindow;
    public GameObject chooseMaterialPrefab;
    
    public Furnace furnace;


    [Header("Choose Material Buttons")]
    [Space(10)]
    [Header("Fuel")]
    public Button chooseFuelButton;
    public Image chooseFuelIcon;
    public Image chooseFuelHealthIcon;
    public TextMeshProUGUI chooseFuelTextCount;
    [Header("Material")]
    public Button chooseMaterialButton;
    public Image chooseMaterialIcon;
    public TextMeshProUGUI chooseMaterialTextCount;

    private void Awake()
    {
        chooseFuelButton.onClick.AddListener(delegate
        {
            GenerateIcons(GameManager.Instance.fuels, "Fuel", chooseFuelIcon.transform.position);
        });
        chooseMaterialButton.onClick.AddListener(delegate
        {
            GenerateIcons(GameManager.Instance.canBeFused, "Material", chooseMaterialButton.transform.position);
        });
        
        UpdateWindow();
    }

    private void FixedUpdate()
    {
        if (furnace != null)
        {
            if (furnace.isWorking)
            {
                UpdateWindow();
            }
        }
    }

    public void UpdateWindow()
    {
        furnaceIcon.sprite = furnace.spriteRenderer.sprite;
        if (furnace.isWorking)
        {
            fillRect_Progress.fillAmount = furnace.currentFuseCompletion / 3f;
            chooseFuelHealthIcon.fillAmount = furnace.currentFuelHealth / 30f;
            textProgress.text = ((int)((furnace.currentFuseCompletion / 3f) * 100f)).ToString() + "%";
        }
        else
        {
            fillRect_Progress.fillAmount = 0f;
            chooseFuelHealthIcon.fillAmount = 0f;
            textProgress.text = "0%";
        }
        
        if (furnace.currentFuel.Count > 0)
        {
            chooseFuelIcon.sprite = furnace.currentFuel[0].GetComponent<SpriteRenderer>().sprite;
            chooseFuelIcon.color = new Color(1f, 1f, 1f, 1f);
            chooseFuelTextCount.text = furnace.currentFuel.Count.ToString();
            chooseFuelHealthIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            chooseFuelIcon.sprite = null;
            chooseFuelIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseFuelHealthIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseFuelTextCount.text = "";
        }
        
        if (furnace.currentMaterial.Count > 0)
        {
            chooseMaterialIcon.sprite = furnace.currentMaterial[0].GetComponent<SpriteRenderer>().sprite;
            chooseMaterialIcon.color = new Color(1f, 1f, 1f, 1f);
            chooseMaterialTextCount.text = furnace.currentMaterial.Count.ToString();
        }
        else
        {
            chooseMaterialIcon.sprite = null;
            chooseMaterialIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseMaterialTextCount.text = "";
        }
    }

    public void GenerateIcons(List<string> iconKeys, string typeWindow, Vector3 pos)
    {
        if (typeWindow == "Material")
        {
            if (UIManager.Instance.furnaceWindow.furnace.currentMaterial.Count > 0)
            {
                UIManager.Instance.furnaceWindow.furnace.GetMaterialBack();
                UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
                UpdateWindow();
                return;
            }
        }
        else
        {
            if (UIManager.Instance.furnaceWindow.furnace.currentFuel.Count > 0)
            {
                UIManager.Instance.furnaceWindow.furnace.GetFuelBack();
                UIManager.Instance.furnaceWindow.chooseMaterialWindow.SetActive(false);
                UpdateWindow();
                return;
            }
        }
        DestroyChooseMaterialChilds();
        chooseMaterialWindow.SetActive(true);
        List<ResourceIcon> icons = UIManager.Instance.GetResourceIconList();
        foreach (var resourceIcon in icons)
        {
            if (iconKeys.Contains(resourceIcon.name) && resourceIcon.GetCount() > 0)
            {
                GameObject spawnedRes = Instantiate(chooseMaterialPrefab, chooseMaterialWindow.transform);
                ChooseMaterial spawnedResM = spawnedRes.GetComponent<ChooseMaterial>();
                spawnedResM.icon.sprite = resourceIcon.iconImage.sprite;
                spawnedResM.resourceIcon = resourceIcon;
                spawnedResM.type = typeWindow;
            }
        }

        chooseMaterialWindow.transform.position = pos;
    }
    
    private void DestroyChooseMaterialChilds()
    {
        for (int i = 0; i < chooseMaterialWindow.transform.childCount; i++)
        {
            Destroy(chooseMaterialWindow.transform.GetChild(i).gameObject);
        }       
    }

    public void OpenFurnaceWindow(Furnace furnaceS)
    {
        furnace = furnaceS;
        gameObject.SetActive(true);
        UpdateWindow();
    }

    public void CloseFurnaceWindow()
    {
        DestroyChooseMaterialChilds();
        Furnace[] furs = GameObject.FindObjectsByType<Furnace>(FindObjectsSortMode.None);
        foreach (var fur in furs)
        {
            fur.Deselect();
        }  
        gameObject.SetActive(false);
        chooseMaterialWindow.SetActive(false);
        
    }
}
