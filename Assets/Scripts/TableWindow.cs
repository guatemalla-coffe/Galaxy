using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableWindow : MonoBehaviour
{
    public GameObject chooseMaterialWindow;
    public GameObject chooseMaterialPrefab;
    
    public Table table;
    public bool contextMenuForTableOpened = false;


    [Header("Choose Material Buttons")]
    [Space(10)]
    [Header("Food")]
    public Button chooseFuelButton;
    public Image chooseFuelIcon;
    public TextMeshProUGUI chooseFuelTextCount;

    private void Awake()
    {
        chooseFuelButton.onClick.AddListener(delegate
        {
            GenerateIcons(GameManager.Instance.food, "FoodTable", chooseFuelIcon.transform.position);
        });
        
        UpdateWindow();
    }

    private void FixedUpdate()
    {
        if (table != null)
        { 
            UpdateWindow();
        }
    }

    public void UpdateWindow()
    {
        if (table.currentFuel.Count > 0)
        {
            chooseFuelIcon.sprite = Resources.Load<Sprite>($"Icons/{table.currentFuel[0].name}");
            chooseFuelIcon.color = new Color(1f, 1f, 1f, 1f);
            chooseFuelTextCount.text = table.currentFuel.Count.ToString();
        }
        else
        {
            chooseFuelIcon.sprite = null;
            chooseFuelIcon.color = new Color(0f, 0f, 0f, 0f);
            chooseFuelTextCount.text = "";
        }
    }

    public void GenerateIcons(List<string> iconKeys, string typeWindow, Vector3 pos)
    {
        if (UIManager.Instance.tableWindow.table.currentFuel.Count > 0)
        {
                UIManager.Instance.tableWindow.table.GetFuelBack();
                UIManager.Instance.tableWindow.chooseMaterialWindow.SetActive(false);
                UpdateWindow();
                return;
        }
        DestroyChooseMaterialChilds();
        chooseMaterialWindow.SetActive(true);
        List<ResourceIcon> icons = UIManager.Instance.GetResourceIconList();
        foreach (var resourceIcon in icons)
        {
            if (iconKeys.Contains(resourceIcon.name) && resourceIcon.GetCount() > 0)
            {
                GameObject spawnedRes = Instantiate(chooseMaterialPrefab, chooseMaterialWindow.transform);
                ChooseMaterialTable spawnedResM = spawnedRes.GetComponent<ChooseMaterialTable>();
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

    public void OpenWindow(Table tableS)
    {
        if (!contextMenuForTableOpened)
        {
            table = tableS;
            gameObject.SetActive(true);
            UpdateWindow();
        }
    }

    public void CloseWindow()
    {
        DestroyChooseMaterialChilds();
        Table[] furs = GameObject.FindObjectsByType<Table>(FindObjectsSortMode.None);
        foreach (var fur in furs)
        {
            fur.Deselect();
        }  
        gameObject.SetActive(false);
        chooseMaterialWindow.SetActive(false);
    }
}
