using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoneySynth : Building
{
    public List<ResourceIcon> currentFuel;
    public List<Craftable> foodItems;
    public GameObject currentItem;

    public bool isWorking;

    public float currentFuelHealth;
    public float currentFuseCompletion = 0;
    public SpriteRenderer spriteRenderer;
    
    public Material outlineMaterial;
    private Material originalMaterial;

    public List<GameObject> currentResources;
    private Workbench workbench;

    public GameObject honeyPrefab;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        workbench = GetComponent<Workbench>();
        for (int i = 0; i < 4000; i++)
        {
            workbench.craftList.Add(honeyPrefab);
        }
        GetComponent<CommandList>().Commands = new List<Command>() {new CraftCommand(null, workbench)};
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(16);
    }
    
    public void Select()
    {
        Debug.Log($"{gameObject.name} selected.");
        
        UIManager.Instance.contextMenu.SetActive(false);
        
        if (outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        spriteRenderer.material = originalMaterial;
    }

    public void CheckIfCanWork()
    {
        if (currentFuel.Count > 0 && currentItem != null)
        {
            foreach (var key in currentItem.GetComponent<Craftable>().GetKeyList())
            {
                if (UIManager.Instance.GetResourceIconByName(key).GetCount() < currentItem.GetComponent<Craftable>().GetValue(key))
                {
                    StopWork();
                    return;
                }
            }
            StartWork();
        }
        else
        {
            StopWork();
        }
    }
    
    public void StartWork()
    {
        isWorking = true;
        currentFuelHealth = currentFuel[0].fireHealth;
        GetComponent<Animator>().SetBool("isWorking", true);
        transform.GetChild(0).gameObject.SetActive(true);
        if(currentFuelHealth < 0) StopWork();
    }
    
    public void StopWork()
    {
        isWorking = false;
        GetComponent<Animator>().SetBool("isWorking", false);
        transform.GetChild(0).gameObject.SetActive(false);
        currentFuseCompletion = 0f;
        DropResources();
        if(UIManager.Instance.honeySynthWindow.honeySynth == this) UIManager.Instance.honeySynthWindow.UpdateWindow();
    }

    public void DropResources()
    {
        foreach (var res in currentResources)
        {
            SpawnResource(res);
        }
        currentResources.Clear();
    }


    private void Update()
    {
        if (isWorking)
        {
            if (currentResources.Count == 0)
            {
                for (int i = 0; i < currentItem.GetComponent<Craftable>().price.Count; i++)
                {
                    if (UIManager.Instance.GetResourceIconByName(currentItem.GetComponent<Craftable>().GetKeyList()[i]).GetCount() <
                        currentItem.GetComponent<Craftable>().GetValue(currentItem.GetComponent<Craftable>().GetKeyList()[i]))
                    {
                        StopWork();
                        return;
                    }
                    for (int j = 0; j < currentItem.GetComponent<Craftable>().GetValue(currentItem.GetComponent<Craftable>().GetKeyList()[i]); j++)
                    {
                        currentResources.Add(UIManager.Instance.GetResourceIconByName(currentItem.GetComponent<Craftable>().GetKeyList()[i]).currentResourceGO);
                        UIManager.Instance.GetResourceIconByName(currentItem.GetComponent<Craftable>().GetKeyList()[i]).IncreaseAmount(-1);
                    }
                }
            }
            currentFuelHealth -= Time.deltaTime;
            currentFuseCompletion += Time.deltaTime;
            if (currentFuelHealth <= 0)
            {
                currentFuel.Remove(currentFuel[0]);
                
                if (currentFuel.Count == 0)
                {
                    StopWork();
                    return;
                }
                currentFuelHealth = currentFuel[0].fireHealth;
            }

            if (currentFuseCompletion >= 3f)
            {
                currentFuseCompletion = 0f;
                SpawnResource(currentItem.gameObject);
                currentResources.Clear();
                foreach (var key in currentItem.GetComponent<Craftable>().GetKeyList())
                {
                    if (currentItem.GetComponent<Craftable>().GetValue(key) <= 0)
                    {
                        StopWork();
                        return;
                    }
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if (isPlased && !UIManager.Instance.furnaceWindow.isActiveAndEnabled)
        {
            UIManager.Instance.furnaceWindow.CloseFurnaceWindow();       
            Select();
            OpenMenu();
        }
    }
    
    public override void OpenMenu()
    {
        UIManager.Instance.honeySynthWindow.OpenWindow(this);       
    }
    
    private void SpawnResource(GameObject prefab)
    {
        int woodToSpawn = 1; 
        for (int i = 0; i < woodToSpawn; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;
            
            spawnPosition = transform.position;

            // Пытаемся найти валидную позицию
            for (int attempt = 0; attempt < 10; attempt++)
            {
                // Рассчитываем случайное направление
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

                // Рассчитываем потенциальную позицию
                Vector3 potentialPosition =
                    transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, 2f);

                spawnPosition = potentialPosition;
                break;
            }

            GameObject wood = Instantiate(prefab, transform.position, Quaternion.identity);

            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
