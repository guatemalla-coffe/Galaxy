using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenWindow : MonoBehaviour
{
    public Image kitchenIcon;
    public Image fillRect_Progress;
    public TextMeshProUGUI textProgress;

    public GameObject chooseMaterialWindow;
    public GameObject chooseMaterialPrefab;
    public GameObject chooseOneMaterialPrefab;
    
    public Kitchen kitchen;
    
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

    private void Awake()
    {
        chooseFuelButton.onClick.AddListener(delegate
        {
            GenerateIcons(GameManager.Instance.fuels, "FuelKitchen", chooseFuelIcon.transform.position);
        });
        chooseMaterialButton.onClick.AddListener(delegate
        {
            GenerateIcons(GameManager.Instance.food, "Material", chooseMaterialButton.transform.position);
        });
        
        UpdateWindow();
    }

    private void FixedUpdate()
    {
        if (kitchen != null)
        {
            if (kitchen.isWorking)
            {
                UpdateWindow();
            }
        }
    }

    public void UpdateWindow()
    {
        kitchenIcon.sprite = kitchen.spriteRenderer.sprite;
        if (kitchen.isWorking)
        {
            fillRect_Progress.fillAmount = kitchen.currentFuseCompletion / 3f;
            chooseFuelHealthIcon.fillAmount = kitchen.currentFuelHealth / 30f;
            textProgress.text = ((int)((kitchen.currentFuseCompletion / 10f) * 100f)).ToString() + "%";
        }
        else
        {
            fillRect_Progress.fillAmount = 0f;
            chooseFuelHealthIcon.fillAmount = 0f;
            textProgress.text = "0%";
        }
        
        if (kitchen.currentFuel.Count > 0)
        {
            chooseFuelIcon.sprite = kitchen.currentFuel[0].GetComponent<SpriteRenderer>().sprite;
            chooseFuelIcon.color = new Color(1f, 1f, 1f, 1f);
            chooseFuelTextCount.text = kitchen.currentFuel.Count.ToString();
            chooseFuelHealthIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            chooseFuelIcon.sprite = null;
            chooseFuelIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseFuelHealthIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseFuelTextCount.text = "";
        }
        
        if (kitchen.currentItem != null)
        {
            chooseMaterialIcon.sprite = kitchen.currentItem.GetComponent<SpriteRenderer>().sprite;
            chooseMaterialIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            chooseMaterialIcon.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    public void GenerateIcons(List<string> iconKeys, string typeWindow, Vector3 pos)
    {
        if (typeWindow == "Material")
        {
            if (UIManager.Instance.KitchenWindow.kitchen.currentItem != null)
            {
                Debug.Log("Generate Icons Icon Clear !!= null");
                UIManager.Instance.KitchenWindow.kitchen.GetFoodBack();
                UIManager.Instance.KitchenWindow.chooseMaterialWindow.SetActive(false);
                UpdateWindow();
                return;
            }
        }
        else
        {
            if (UIManager.Instance.KitchenWindow.kitchen.currentFuel.Count > 0)
            {
                UIManager.Instance.KitchenWindow.kitchen.GetFuelBack();
                UIManager.Instance.KitchenWindow.chooseMaterialWindow.SetActive(false);
                UpdateWindow();
                return;
            }
        }
        DestroyChooseMaterialChilds();
        chooseMaterialWindow.SetActive(true);
        if (typeWindow == "Material")
        {
            foreach (var foodName in GameManager.Instance.food)
            {
                GameObject resourcePrefab = Resources.Load<GameObject>($"Resources/{foodName}");
                if (resourcePrefab != null)
                {
                    GameObject spawnedRes = Instantiate(chooseOneMaterialPrefab, chooseMaterialWindow.transform);
                    ChooseOneMaterial spawnedResM = spawnedRes.GetComponent<ChooseOneMaterial>();
                    spawnedResM.icon.sprite = resourcePrefab.GetComponent<SpriteRenderer>().sprite;
                    spawnedResM.Craftable = resourcePrefab.GetComponent<Craftable>();
                    spawnedResM.type = typeWindow;
                    spawnedResM.GetComponent<UIButtonHover>().GenerateRes(spawnedResM.Craftable.price);
                }

            }

        }
        
        else
        {
            List<ResourceIcon> icons = UIManager.Instance.GetResourceIconList();
            foreach (var resourceIcon in icons)
            {
                if (iconKeys.Contains(resourceIcon.name) && resourceIcon.GetCount() > 0)
                {
                    GameObject spawnedRes = Instantiate(chooseMaterialPrefab, chooseMaterialWindow.transform);
                    ChooseMaterialKitchen spawnedResM = spawnedRes.GetComponent<ChooseMaterialKitchen>();
                    spawnedResM.icon.sprite = resourceIcon.iconImage.sprite;
                    spawnedResM.resourceIcon = resourceIcon;
                    spawnedResM.type = typeWindow;
                }
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

    public void OpenWindow(Kitchen kitchenS)
    {
        kitchen = kitchenS;
        gameObject.SetActive(true);
        UpdateWindow();
    }

    public void CloseWindow()
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
