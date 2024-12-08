using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Furnace : Building
{
    public List<GameObject> currentFuel = new List<GameObject>();
    public List<GameObject> currentMaterial = new List<GameObject>();
    public List<GameObject> currentExit = new List<GameObject>();

    public bool isWorking;

    public float currentFuelHealth;
    public float currentFuseCompletion = 0;
    public SpriteRenderer spriteRenderer;
    
    public Material outlineMaterial;
    private Material originalMaterial;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(6);
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
        if (currentFuel.Count > 0 && currentMaterial.Count > 0)
        {
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
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        if(currentFuelHealth < 0) StopWork();
    }
    
    public void StopWork()
    {
        isWorking = false;
        GetComponent<Animator>().SetBool("isWorking", false);
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);

    }


    private void Update()
    {
        if (isWorking)
        {
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
                int index = GameManager.Instance.canBeFused.IndexOf(currentMaterial[0].name);
                currentExit.Add(Resources.Load<GameObject>($"Resources/{GameManager.Instance.fused[index]}"));
                SpawnResource(currentExit[0]);
                currentExit.Clear();
                currentMaterial.RemoveAt(0);
                if (currentMaterial.Count == 0)
                {
                    StopWork();
                    return;
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
        UIManager.Instance.furnaceWindow.OpenFurnaceWindow(this);       
    }
    
    public void SetFuel(ResourceIcon resourceIcon, int count)
    {
        if (currentFuel.Count > 0)
        {       
            print("GetFuelBack");
            GetFuelBack();
        }
        print("SetFuel");
        for (int i = 0; i < count; i++)
        {
            currentFuel.Add(resourceIcon.currentResourceGO);
            resourceIcon.IncreaseAmount(-1);
        }
        CheckIfCanWork();
        UIManager.Instance.furnaceWindow.UpdateWindow();
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
    
    public void SetMaterial(ResourceIcon resourceIcon, int count)
    {
        if (currentMaterial.Count > 0)
        {       
            print("GetMaterialBack");
            GetMaterialBack();
        }
        print("SetMaterial");
        for (int i = 0; i < count; i++)
        {
            currentMaterial.Add(resourceIcon.currentResourceGO);
            resourceIcon.IncreaseAmount(-1);
        }
        CheckIfCanWork();
        UIManager.Instance.furnaceWindow.UpdateWindow();
    }
    
    public void GetMaterialBack()
    {
        for (int i = 0; i < currentMaterial.Count; i++)
        {
            SpawnResource(currentMaterial[i]);
        }
        currentMaterial.Clear();
        CheckIfCanWork();
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
