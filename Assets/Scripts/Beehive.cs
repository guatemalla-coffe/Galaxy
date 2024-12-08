using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Beehive : Building
{
    public Material outlineMaterial;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;

    public List<GameObject> flowers;
    public List<string> bees;

    public float currentProgress;
    public GameObject honey;

    public GameObject PS_bee;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        Work();
    }

    public override void OpenMenu()
    {
        UIManager.Instance.beehiveWindow.OpenWindow(this);       
    }
    
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if (isPlased && !UIManager.Instance.beehiveWindow.isActiveAndEnabled)
        {
            UIManager.Instance.beehiveWindow.CloseWindow();       
            Select();
            OpenMenu();
        }
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

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(8);
        transform.GetChild(0).GetComponent<StaticSortingOrder>().Sort();
    }
    
    public async UniTask Work()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            foreach (var bee in bees)
            {
                currentProgress += 1f;
            }
            if (Random.value <= 0.01f * flowers.Count)
            {
                if (bees.Count < 9)
                {
                    bees.Add("bee");
                    GameObject spawnedPS = Instantiate(PS_bee);
                    spawnedPS.transform.position = transform.position;
                    Destroy(spawnedPS, 2f);
                }
            }
            if (bees.Count > flowers.Count)
            {
                if (Random.value <= 0.01f)
                {
                    bees.Remove("bee");
                }
            }
            if(UIManager.Instance.beehiveWindow.beehive == this) UIManager.Instance.beehiveWindow.UpdateWindow();
            if (currentProgress >= 100f)
            {
                currentProgress = 0f;
                SpawnResource(honey);
            }
        }    
    }


    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        spriteRenderer.material = originalMaterial;
    }

    public void LoadFlower(GameObject flower)
    {
        flowers.Add(flower);
    }
    
    public void AddFlower(GameObject flower)
    {
        flowers.Add(flower);
        UIManager.Instance.GetResourceIconByName(flower.GetComponent<ResourceToUI>().name).IncreaseAmount(-1);
        UIManager.Instance.beehiveWindow.UpdateWindow();
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
