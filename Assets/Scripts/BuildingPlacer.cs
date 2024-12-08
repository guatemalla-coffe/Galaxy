using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using Pathfinding.Util;
using TMPro;
using Unity.VisualScripting;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building Settings")]
    public LayerMask walkableLayer;
    public LayerMask buildingLayer; // Слой построенных зданий
    //public float buildAreaSize = 3f;
    public GameObject[] buildingPrefabs;

    [Header("UI Settings")]
    public GameObject buttonPrefab;
    public Transform uiPanel;

    private GameObject currentBuilding;
    private GameObject selectedBuilding; // Выбранное здание для удаления или перемещения
    private GridGraph gridGraph;

    private Color originalColor;
    private SpriteRenderer currentBuildingRenderer;

    public GameObject TestPoint;
    public List<GameObject> testP;
    public BoxCollider2D currentBuildingBoxCollider;
    public float sizeX;
    public float sizeY;
    
    public GameObject CannottBuildTip_Prefab;
    public GameObject psBuild;



    private void Start()
    {
        testP = new List<GameObject>();
        gridGraph = AstarPath.active.data.gridGraph;
        GenerateBuildingButtons();
    }

    private void Update()
    {
        HandleBuildingMovement();
        HandleBuildingPlacement();
        HandleBuildingDeselection();
        HandleBuildingInteraction();
    }

    private void HandleBuildingMovement()
    {
        if (currentBuilding == null) return;

        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 gridPosition = GetSnapToGridPosition(mousePosition);
        currentBuilding.transform.position = gridPosition;
    }

    private Vector3 GetSnapToGridPosition(Vector3 worldPosition)
    {
        // int gridX = Mathf.FloorToInt(worldPosition.x / gridGraph.nodeSize);
        // int gridY = Mathf.FloorToInt(worldPosition.y / gridGraph.nodeSize);
        //return new Vector3(gridX * gridGraph.nodeSize, gridY * gridGraph.nodeSize, 0);
        float buildingGridSize = 0.5f;
        int gridX = Mathf.FloorToInt(worldPosition.x / buildingGridSize);
        int gridY = Mathf.FloorToInt(worldPosition.y / buildingGridSize);
        return new Vector3(gridX * buildingGridSize, gridY * buildingGridSize, 0);
        
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    private void HandleBuildingPlacement()
    {
        if (currentBuilding == null) return;
        
        if (CanPlaceBuilding(currentBuilding.transform.position) && !IsPlayerInArea(currentBuilding.transform.position))
        {
            if (currentBuildingRenderer != null)
            {
                originalColor = currentBuildingRenderer.color;
                currentBuildingRenderer.color = Color.green; // Подсветка
            }
        }
        else
        {
            if (currentBuildingRenderer != null)
            {
                originalColor = currentBuildingRenderer.color;
                currentBuildingRenderer.color = Color.red; // Подсветка
            }
        }
        
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            if (IsPlayerInArea(currentBuilding.transform.position))
            {
                DrawTip("Слишком близко к игроку или другоу зданию!");
                Debug.Log("Невозможно разместить здание, объект игрока находится в зоне строительства.");
                return;
            }

            if (CanPlaceBuilding(currentBuilding.transform.position))
            {
                if (CanBuy(currentBuilding))
                {
                    PlaceBuilding(currentBuilding, currentBuilding.transform.position);
                }
                else
                {
                    DrawTip("Недостаточно ресурсов");

                    Debug.Log("Not enough resorces");
                }
            }
            else
            {
                DrawTip("Невозможно разместить здание, место занято.");
                Debug.Log("Невозможно разместить здание, место занято.");
            }
        }
    }
    
    private bool IsPlayerInArea(Vector3 position)
    {
        // Размер проверки соответствует зоне строительства
        Vector2 size = new Vector2(currentBuildingBoxCollider.size.x, currentBuildingBoxCollider.size.y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, 0);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Building"))
            {
                return true;
            }
        }

        return false;
    }

    private void DrawTip(string textTip)
    {
        GameObject spawnedTip = Instantiate(CannottBuildTip_Prefab, transform);
        spawnedTip.GetComponent<RectTransform>().position =
            Camera.main.WorldToScreenPoint(currentBuilding.transform.position + new Vector3(0f, 0.5f, 0f));
        spawnedTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = textTip; // Невозможно разместить здание, место занято.
        spawnedTip.GetComponent<TipController>().StartMove();
    }
    
    private void HandleBuildingDeselection()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            CancelBuilding();
        }
    }

    private void HandleBuildingInteraction()
    {
        if (Input.GetMouseButtonDown(0) && currentBuilding == null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePosition, buildingLayer);

            if (hit != null)
            {
                selectedBuilding = hit.gameObject;

                // Включаем режим перемещения
                Debug.Log($"Выбрано здание: {selectedBuilding.name}");
                StartBuilding(selectedBuilding);
            }
        }
        else if (Input.GetMouseButtonDown(1) && selectedBuilding != null)
        {
            // Удаляем здание при ПКМ
            RemoveBuilding(selectedBuilding);
            selectedBuilding = null;
        }
    }

    private bool CanPlaceBuilding(Vector3 position)
    {
        DrawCubes(position);
        int startX = Mathf.FloorToInt(((position.x - sizeX*0.25f/2f) - gridGraph.center.x) / gridGraph.nodeSize) + (gridGraph.width / 2);
        int startY = Mathf.FloorToInt(((position.y-sizeY*0.25f/2f) - gridGraph.center.y) / gridGraph.nodeSize) + (gridGraph.depth / 2);

        for (int x = startX; x < startX + Mathf.FloorToInt(sizeX); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(sizeY); y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    return false;
                
                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                if (nodeBase == null || !nodeBase.Walkable)
                    return false;
            }
        }

        return true;
    }

    private void DrawCubes(Vector3 position)
    {
        DeleteTestPoints();
        int startX = Mathf.FloorToInt(((position.x - sizeX*0.25f/2f) - gridGraph.center.x) / gridGraph.nodeSize) + (gridGraph.width / 2);
        int startY = Mathf.FloorToInt(((position.y - sizeY*0.25f/2f) - gridGraph.center.y) / gridGraph.nodeSize) + (gridGraph.depth / 2);

        for (int x = startX; x < startX + Mathf.FloorToInt(sizeX); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(sizeY); y++)
            {
                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                
                GameObject t = Instantiate(TestPoint, (Vector3)nodeBase.position, Quaternion.identity);
                testP.Add(t);

                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                {
                    t.GetComponent<SpriteRenderer>().color = Color.red;
                    continue;
                }
                
                if (nodeBase == null || !nodeBase.Walkable)
                {
                    t.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    private void PlaceBuilding(GameObject buildingPrefab, Vector3 position)
    {
        GameObject spawnedBuilding = Instantiate(buildingPrefab, position, Quaternion.identity);
        spawnedBuilding.tag = "Building";
        spawnedBuilding.GetComponent<Building>().isPlased = true;
        buildingPrefab.GetComponent<SpriteRenderer>().color = Color.white;
        spawnedBuilding.GetComponent<SpriteRenderer>().color = Color.white;
        spawnedBuilding.AddComponent<StaticSortingOrder>();
        spawnedBuilding.GetComponent<StaticSortingOrder>().Sort();
        spawnedBuilding.GetComponent<Building>().Place();
        spawnedBuilding.gameObject.layer = 8;

        GameObject part = Instantiate(psBuild);
        part.transform.position = spawnedBuilding.transform.position;
        Destroy(part, 1.5f);

        int startX = Mathf.FloorToInt(position.x / gridGraph.nodeSize);
        int startY = Mathf.FloorToInt(position.y / gridGraph.nodeSize);

        for (int x = startX; x < startX + Mathf.FloorToInt(sizeX); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(sizeY); y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    continue;

                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                GridNode node = nodeBase as GridNode;
                if (node != null)
                {
                    node.Walkable = false;
                }
            }
        }
        DeleteTestPoints();
        

        AstarPath.active.Scan();

        Destroy(currentBuilding);
        currentBuilding = null;
    }

    private void StartBuilding(GameObject buildingPrefab)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }
        
        currentBuilding = Instantiate(buildingPrefab);
        currentBuilding.GetComponent<Building>().isPlased = false;
        currentBuilding.gameObject.layer = 0;
        currentBuildingRenderer = currentBuilding.GetComponent<SpriteRenderer>();
        currentBuildingBoxCollider = currentBuilding.GetComponent<BoxCollider2D>();
        sizeX = currentBuildingBoxCollider.size.x / gridGraph.nodeSize;
        sizeY = currentBuildingBoxCollider.size.y / gridGraph.nodeSize;
    }

    public void CancelBuilding()
    {
        DeleteTestPoints();
        if (currentBuilding != null)
        {
            if (currentBuildingRenderer != null)
                currentBuildingRenderer.color = originalColor; // Сбрасываем цвет

            Destroy(currentBuilding);
            currentBuilding = null;
        }
        
    }

    private void RemoveBuilding(GameObject building)
    {
        if (building != null)
        {
            Destroy(building);
            Debug.Log($"Здание {building.name} удалено.");
        }
    }

    private void DeleteTestPoints()
    {
        List<GameObject> d = new List<GameObject>(testP);
        foreach (var da in d)
        {
            
            Destroy(da.gameObject);
        }
        testP.Clear();
    }

    private void GenerateBuildingButtons()
    {
        foreach (var prefab in buildingPrefabs)
        {
            GameObject button = Instantiate(buttonPrefab, uiPanel);
            Sprite iconSprite = prefab.GetComponent<BuildingData>().iconButton;
            
            if (iconSprite != null)
            {
                button.transform.GetChild(0).GetComponent<Image>().sprite = iconSprite;
            }
            else
            {
                button.transform.GetChild(0).GetComponent<Image>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            }
            

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartBuilding(prefab);
            });

            button.transform.GetComponent<TooltipController>().GenerateTips(prefab.GetComponent<BuildingData>());
        }
    }

    public void CloseBuildingMenu()
    {
        CancelBuilding();
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void OpenBuildingMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 1f;
        GameEvents.BuildingMenuOpen();
    }

    public bool CanBuy(GameObject buildingPrefab)
    {
        BuildingData data = buildingPrefab.GetComponent<BuildingData>();
        List<string> keyList = data.GetKeyList();
        bool canBuy = false;
        foreach (var key in keyList)
        {
            ResourceIcon resourceIcon = UIManager.Instance.GetResourceIconByName(key);
            if (resourceIcon != null)
            {
                int pr = 0;
                if (data.GetValue(key) > 0)
                {
                    pr = data.GetValue(key);
                    if (resourceIcon.GetCount() < pr)
                    {
                        canBuy = false;
                    }
                    else
                    {
                        canBuy = true;
                    }
                }
            }
        }
        if (canBuy)
        {
            foreach (var key in keyList)
            {
                ResourceIcon resourceIcon = UIManager.Instance.GetResourceIconByName(key);
                if (resourceIcon != null)
                {
                    int pr = 0;
                    if (data.GetValue(key) > 0)
                    {
                        pr = data.GetValue(key);
                        if (resourceIcon.GetCount() < pr)
                        {
                            Debug.LogError("Ошибка в цикле for в BuildingPlacer.CanBuy()");
                            return false;
                        }
                        else
                        {
                            resourceIcon.IncreaseAmount(-pr);
                        }
                    }
                }
            }

            return true;
        }
        return false;
        
    }
}
