using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Table : Building
{
    public List<GameObject> currentFuel;
    public SpriteRenderer spriteRenderer;
    
    public Material outlineMaterial;
    private Material originalMaterial;

    public Transform eatPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        GetComponent<CommandList>().Commands = new List<Command>() {(new EatCommand(null, this))};
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(13);
        GetComponent<StaticSortingOrder>().Sort();
    }
    
    public void Select()
    {
        if(UIManager.Instance.tableWindow.contextMenuForTableOpened) return;
        Debug.Log($"{gameObject.name} selected.");
        
        UIManager.Instance.contextMenu.SetActive(false);
        
        if (outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    public void Deselect()
    {
        if(UIManager.Instance.tableWindow.contextMenuForTableOpened) return;
        Debug.Log($"{gameObject.name} deselected.");
        spriteRenderer.material = originalMaterial;
    }
    
    private void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if (isPlased && !UIManager.Instance.tableWindow.isActiveAndEnabled)
        {
            UIManager.Instance.tableWindow.CloseWindow();       
            Select();
            OpenMenu();
        }
    }
    
    public override void OpenMenu()
    {
        UIManager.Instance.tableWindow.OpenWindow(this);       
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
            if(resourceIcon.name == "HoneyPastile") TaskManager.Instance.CompleteTask(14);
            resourceIcon.IncreaseAmount(-1);
        }
        UIManager.Instance.tableWindow.UpdateWindow();
    }

    public void GetFuelBack()
    {
        for (int i = 0; i < currentFuel.Count; i++)
        {
            SpawnResource(currentFuel[i]);
        }
        currentFuel.Clear();
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
