using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeehiveWindow : MonoBehaviour
{
    public Beehive beehive;

    public GameObject chooseMaterialWindow;
    public GameObject materialPrefab;
    public GameObject chooseMaterialPrefab;
    public GameObject chooseMaterialButtonPrefab;

    public GameObject beeCellPrefab;
    
    public Transform flowersLayoutGroup;
    public Transform beesLayoutGroup;

    public Image progressBar;
    public TextMeshProUGUI progressBarTMPro;

    
    private void Awake()
    {
        UpdateWindow();
    }
    
    public void OpenWindow(Beehive hive)
    {
        beehive = hive;
        gameObject.SetActive(true);
        UpdateWindow();
    }
    
    public void CloseWindow()
    {
        DestroyChooseMaterialChilds();
        Beehive[] hives = GameObject.FindObjectsByType<Beehive>(FindObjectsSortMode.None);
        foreach (var hive in hives)
        {
            hive.Deselect();
        }  
        gameObject.SetActive(false);
        chooseMaterialWindow.SetActive(false);
    }
    
    public void GenerateIcons(List<string> iconKeys, Vector3 pos)
    {
        DestroyChooseMaterialChilds();
        chooseMaterialWindow.SetActive(true);
        List<ResourceIcon> icons = UIManager.Instance.GetResourceIconList();
        foreach (var resourceIcon in icons)
        {
            if (iconKeys.Contains(resourceIcon.name) && resourceIcon.GetCount() > 0)
            {
                GameObject spawnedRes = Instantiate(chooseMaterialPrefab, chooseMaterialWindow.transform);
                ChooseOneMaterial spawnedResM = spawnedRes.GetComponent<ChooseOneMaterial>();
                spawnedResM.resourceIcon = resourceIcon;
                spawnedResM.icon.sprite = resourceIcon.iconImage.sprite;
                
            }
        }
        chooseMaterialWindow.transform.position = pos;
    }
    
    public void DestroyChooseMaterialChilds()
    {
        for (int i = 0; i < chooseMaterialWindow.transform.childCount; i++)
        {
            Destroy(chooseMaterialWindow.transform.GetChild(i).gameObject);
        } 
    }

    public void GenerateFlowers()
    {
        for (int i = 0; i < flowersLayoutGroup.childCount; i++)
        {
            Destroy(flowersLayoutGroup.GetChild(i).gameObject);
        }
        foreach (var flow in beehive.flowers)
        {
            GameObject spawnedMat = Instantiate(materialPrefab, flowersLayoutGroup);
            GetFlowerButton getFlowerButton = spawnedMat.GetComponent<GetFlowerButton>();
            getFlowerButton.resource = flow;
            getFlowerButton.icon.sprite = Resources.Load<Sprite>($"Icons/{flow.GetComponent<ResourceToUI>().name}");
        }

        if (beehive.flowers.Count < 9)
        {
            GameObject spawnedChooseMat = Instantiate(chooseMaterialButtonPrefab, flowersLayoutGroup);
            spawnedChooseMat.GetComponent<Button>().onClick.AddListener(delegate
            {
                GenerateIcons(GameManager.Instance.flowers, spawnedChooseMat.transform.position);
            });
        }
    }
    
    public void GenerateBees()
    {
        for (int i = 0; i < beesLayoutGroup.childCount; i++)
        {
            Destroy(beesLayoutGroup.GetChild(i).gameObject);
        }
        foreach (var bee in beehive.bees)
        {
            Instantiate(beeCellPrefab, beesLayoutGroup);
        }
    }
    
    public void UpdateWindow()
    {
        GenerateFlowers();
        GenerateBees();
        progressBar.fillAmount = beehive.currentProgress / 100f;
        progressBarTMPro.text = (Mathf.Clamp((int)beehive.currentProgress, 0, 100)).ToString() + "%";
    }
}
