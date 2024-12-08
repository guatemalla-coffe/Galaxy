using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Kitchen : Building
{
    public List<GameObject> currentFuel;
    public List<Craftable> foodItems;
    public Craftable currentItem;

    public bool isWorking;

    public float currentFuelHealth;
    public float currentFuseCompletion = 0;
    public SpriteRenderer spriteRenderer;
    
    public Material outlineMaterial;
    private Material originalMaterial;

    public List<GameObject> currentResources;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(11);
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
            foreach (var key in currentItem.GetKeyList())
            {
                if (UIManager.Instance.GetResourceIconByName(key).GetCount() < currentItem.GetValue(key))
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
        currentFuelHealth = currentFuel[0].GetComponent<ResourceToUI>().fireHealth;
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
        if(UIManager.Instance.KitchenWindow.kitchen == this) UIManager.Instance.KitchenWindow.UpdateWindow();
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
                for (int i = 0; i < currentItem.price.Count; i++)
                {
                    if (UIManager.Instance.GetResourceIconByName(currentItem.GetKeyList()[i]).GetCount() <
                        currentItem.GetValue(currentItem.GetKeyList()[i]))
                    {
                        StopWork();
                        return;
                    }
                    for (int j = 0; j < currentItem.GetValue(currentItem.GetKeyList()[i]); j++)
                    {
                        currentResources.Add(UIManager.Instance.GetResourceIconByName(currentItem.GetKeyList()[i]).currentResourceGO);
                        UIManager.Instance.GetResourceIconByName(currentItem.GetKeyList()[i]).IncreaseAmount(-1);
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
                currentFuelHealth = currentFuel[0].GetComponent<ResourceToUI>().fireHealth;
            }

            if (currentFuseCompletion >= 3f)
            {
                currentFuseCompletion = 0f;
                SpawnResource(currentItem.gameObject);
                currentResources.Clear();
                foreach (var key in currentItem.GetKeyList())
                {
                    if (currentItem.GetValue(key) <= 0)
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
        UIManager.Instance.KitchenWindow.OpenWindow(this);       
    }
    
    public void SetFuel(GameObject resourceIcon, int count)
    {
        if (currentFuel.Count > 0)
        {       
            print("GetFuelBack");
            GetFuelBack();
        }
        print("SetFuel");
        for (int i = 0; i < count; i++)
        {
            currentFuel.Add(resourceIcon);
            UIManager.Instance.GetResourceIconByName(resourceIcon.name).IncreaseAmount(-1);
        }
        CheckIfCanWork();
        UIManager.Instance.KitchenWindow.UpdateWindow();
    }

    public void GetFuelBack()
    {
        for (int i = 0; i < currentFuel.Count; i++)
        {
            SpawnResource(currentFuel[i]);
        }
        currentFuel.Clear();
        CheckIfCanWork();
    }
    
    public void SetFood(Craftable resourceIcon)
    {
        currentItem = resourceIcon;
        CheckIfCanWork();
        UIManager.Instance.KitchenWindow.UpdateWindow();
    }

    public void GetFoodBack()
    {
        currentItem = null;
        UIManager.Instance.KitchenWindow.chooseMaterialIcon.sprite = null;
        CheckIfCanWork();
        UIManager.Instance.KitchenWindow.UpdateWindow();
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
