using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prog2step.Models;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Canvas canvas;
    public RectTransform canvasTransform;
    //public Dictionary<string, RectTransform> ItemsTransforms;
    public GameObject takeResourceSoundPrefab;
    public GameObject pauseMenu;
    public GameObject contextMenu;
    [SerializeField] private GameObject resourceIconPrefab;
    [SerializeField] private RectTransform resourcePanel;
    
    private Dictionary<string, ResourceIcon> resourceIcons = new();
    [Header("Bear Selection")] 
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI workText;
    public Image iconBear;
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI energyText;

    [Header("Windows")] 
    public BuildingPlacer buildingMenu;

    public FurnaceWindow furnaceWindow;
    public BeehiveWindow beehiveWindow;
    public KitchenWindow KitchenWindow;
    public TableWindow tableWindow;
    public HoneySynthWindow honeySynthWindow;
    public WorkshopWindow workshopWindow;
    public ScienceTableWindow scienceTableWindow;
    public RadioWindow radioWindow;
    public RocketWindow rocketWindow;

    public TextAsset DoNotDelete;
    
    [Header("Debug")]
    public TextMeshProUGUI commandText;
    public TextMeshProUGUI stateText;


    private string saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    

    // Создание JSON с данными по умолчанию
    private void CreateDefaultJson()
    {
        string jsonData = JsonConvert.SerializeObject(container, Formatting.Indented);

        // Записываем JSON в файл
        File.WriteAllText(saveFilePath, jsonData);

        Debug.Log("Default JSON created at: " + saveFilePath);
    }

    public GameObject tooltipPrefab;
    
    public async UniTaskVoid ShopTip(string tipText)
    {
        GameObject spawnedTip = Instantiate(tooltipPrefab, canvas.transform);
        spawnedTip.GetComponent<RectTransform>().position = Input.mousePosition;
        spawnedTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tipText; // Невозможно разместить здание, место занято.
        spawnedTip.GetComponent<TipController>().StartMove();
    }

    public async UniTask ShopTipPlayer(string tipText, Transform plasyer)
    {
        GameObject spawnedTip = Instantiate(tooltipPrefab, canvas.transform);
        spawnedTip.GetComponent<RectTransform>().position =
            Camera.main.WorldToScreenPoint(plasyer.transform.position + new Vector3(0f, 0.5f, 0f));
        spawnedTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tipText; // Невозможно разместить здание, место занято.
        spawnedTip.GetComponent<TipController>().StartMove();
    }
    
    public async UniTask ShopTipPlayer(string tipText, Transform plasyer, Color color, float timeToDestroy)
    {
        GameObject spawnedTip = Instantiate(tooltipPrefab, canvas.transform);
        spawnedTip.GetComponent<Image>().color = color;
        spawnedTip.GetComponent<RectTransform>().position =
            Camera.main.WorldToScreenPoint(plasyer.transform.position + new Vector3(0f, 0.5f, 0f));
        spawnedTip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tipText; // Невозможно разместить здание, место занято.
        spawnedTip.GetComponent<TipController>().timeToDestroy = timeToDestroy;
        spawnedTip.GetComponent<TipController>().StartMove();
    }
    
    
    public Container container = new Container();
    
    public Texture2D customCursor; // Текстура вашего курсора
    public Vector2 hotspot = Vector2.zero;

    public TerrainManager terrainManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Texture2D resizedCursor = ResizeTexture(customCursor, 16, 16);
            Cursor.SetCursor(customCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogError("More than one UIManager!");
            Destroy(gameObject);
        }
    }

    public Texture2D ResizeTexture(Texture2D original, int width, int height)
    {
        // Создаем новую текстуру с нужными размерами
        Texture2D resizedTexture = new Texture2D(width, height);

        // Копируем пиксели с учетом нового размера
        Color[] pixels = original.GetPixels(0, 0, original.width, original.height);
        
        // Масштабируем текстуру с помощью GetPixels и Apply
        resizedTexture.SetPixels(pixels);
        resizedTexture.Apply();

        return resizedTexture;
    }
    
    private async void Start()
    {
        container = new Container() { Name = "example", Resources = new List<object>() };
        // Устанавливаем путь к файлу
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        bool needRes = false;
        container = JsonConvert.DeserializeObject<Container>(File.ReadAllText(saveFilePath));
        print(container.Name + "  :  NAME");
        // Проверяем, существует ли файл, если нет — создаём его
        if (container.Resources == null || container.Resources == new List<object>() || container.Resources.Count == 0)
        {
            needRes = true;
            container.Resources = new List<object>();
            //CreateDefaultJson();
        }
        else
        {
            needRes = false;
            print(container.Resources);
            Debug.Log("JSON file already exists at: " + saveFilePath);
        }
        
        new RequestsManager().CreateShop();
        Load();
        terrainManager.GenerateTerrain(needRes);
        terrainManager.CreateNavMesh();
        new RequestsManager().GetShops();
    }

    #region Resource

    /// <summary>
    /// Обновляет счетчик ресурса. Если иконка еще не создана, она создается.
    /// </summary>
    public void UpdateResourceCount(string resourceName, int amount)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Обновляем счетчик ресурса
        resourceIcons[resourceName].IncreaseAmount(amount);
    }

    /// <summary>
    /// Создает новую иконку ресурса в UI.
    /// </summary>
    private void CreateResourceIcon(string resourceName)
    {
        // Пример создания иконки для ресурса
        GameObject resourceIcon = Instantiate(resourceIconPrefab, resourcePanel.transform);
        resourceIcon.name = resourceName;
        resourceIcons.Add(resourceName, resourceIcon.GetComponent<ResourceIcon>());

        // Инициализация количества ресурса
        resourceIcon.GetComponent<ResourceIcon>().Initialize();
    }

    
    public RectTransform GetResourceIconTarget(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Возвращаем RectTransform иконки ресурса
        return resourceIcons[resourceName].GetComponent<RectTransform>();
    }
    
    public ResourceIcon GetResourceIconByName(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Возвращаем RectTransform иконки ресурса
        return resourceIcons[resourceName].GetComponent<ResourceIcon>();
    }

    public void AddResource(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Обновляем количество ресурса
        resourceIcons[resourceName].GetComponent<ResourceIcon>().IncreaseAmount(1);
    }
    
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (buildingMenu.gameObject.activeSelf)
            {
                buildingMenu.CloseBuildingMenu();
            }
            else
            {
                buildingMenu.OpenBuildingMenu();
            }
        }

        if (BearManager.Instance.selectedBear != null)
        {
            BearController bear = BearManager.Instance.GetSelectedBear();
            iconBear.sprite = bear.GetComponent<SpriteRenderer>().sprite;
            nameText.text = bear.nameBear;
            workText.text = bear.work;
            hungerText.text = "Сытость: "+((int)(bear.currentHunger)).ToString();
        }
    }

    public void LoadScene(int buildIndex)
    {
        Cursor.SetCursor(UIManager.Instance.customCursor, Vector2.zero, CursorMode.Auto);
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        //Load();
        Cursor.SetCursor(UIManager.Instance.customCursor, Vector2.zero, CursorMode.Auto);
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0.00001f;
            pauseMenu.SetActive(true);
        }
    }
    
    public List<ResourceIcon> GetResourceIconList()
    {
        List<ResourceIcon> keys = new List<ResourceIcon>(); 
        foreach (var pri in resourceIcons)
        {
            keys.Add(GetResourceIconByName(pri.Key));
        }

        return keys;
    }

    public void RemoveResourceIconFromList(ResourceIcon resourceIcon)
    {
        resourceIcons.Remove(resourceIcon.name);
    }

    public void LogMessage(string mes)
    {
        new RequestsManager().CreateLog(mes, container.Name, container.Resources);
    }
    
    public void LogMessage(string mes, ResourceToSave resourceToSave)
    {
        Container tempLogContainer = new Container()
            { Name = container.Name, Resources = new List<object>() { resourceToSave } };
        new RequestsManager().CreateLog(mes, tempLogContainer.Name, tempLogContainer.Resources);
    }
    
    
    private string jsonOut;
    public async void SaveCurrentResources()
    {
        string conName = container.Name;
        container = new Container() {Name = conName, Resources = new List<object>()};
        //Resource save
        foreach (var ri in GetResourceIconList())
        {
            ResourceToSave a = new ResourceToSave() {Count = ri.GetCount(), Name = ri.name };
            container.Resources.Add(a);
            print("ri in GetResourceIconList()");
        }

        //Save bears
        foreach (var bear in BearManager.Instance.bearsList)
        {
            BearToSave a = new BearToSave() { Position = new SerializableVector3(bear.transform.position), 
                Hunger = (int)bear.currentHunger };
            container.Resources.Add(a);
        }
        
        //Save furs
        foreach (var fur in GameObject.FindObjectsByType<Furnace>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> fuelRes = new List<ResourceToSave>();
            foreach (var ri in fur.currentFuel)
            {
                ResourceToSave r = new ResourceToSave() {Count = 1, Name = ri.name };
                fuelRes.Add(r);
            }
            List<ResourceToSave> matRes = new List<ResourceToSave>();
            foreach (var ri in fur.currentMaterial)
            {
                ResourceToSave r = new ResourceToSave() {Count = 1, Name = ri.name };
                matRes.Add(r);
            }
            FurnaceToSave a = new FurnaceToSave() { Position = new SerializableVector3(fur.transform.position), 
                Fuel = fuelRes, Material = matRes};
            container.Resources.Add(a);
        }
        
        //Save beehives
        foreach (var beehive in GameObject.FindObjectsByType<Beehive>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> flowersList = new List<ResourceToSave>();
            foreach (var flower in beehive.flowers)
            {
                ResourceToSave f = new ResourceToSave() { Count = 1, Name = flower.name };
                flowersList.Add(f);
            }
            List<string> beesToSaveList = new List<string>();
            foreach (var bee in beehive.bees)
            {
                beesToSaveList.Add(bee);
            }

            BeehiveToSave beehiveToSave = new BeehiveToSave()
                { Position = new SerializableVector3(beehive.transform.position), Bees = beesToSaveList, Flowers = flowersList };
            container.Resources.Add(beehiveToSave);
        }
        
        //Save table
        foreach (var table in GameObject.FindObjectsByType<Table>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> foodList = new List<ResourceToSave>();
            foreach (var eat in table.currentFuel)
            {
                ResourceToSave f = new ResourceToSave() { Count = 1, Name = eat.name };
                foodList.Add(f);
            }

            TableToSave beehiveToSave = new TableToSave()
                { Position = new SerializableVector3(table.transform.position), Food = foodList };
            container.Resources.Add(beehiveToSave);
        }
        
        //Save kitchen
        foreach (var fur in GameObject.FindObjectsByType<Kitchen>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> fuelRes = new List<ResourceToSave>();
            foreach (var ri in fur.currentFuel)
            {
                ResourceToSave r = new ResourceToSave() {Count = 1, Name = ri.name };
                fuelRes.Add(r);
            }
            KitchenToSave a = new KitchenToSave() { Position = new SerializableVector3(fur.transform.position), 
                Fuel = fuelRes};
            container.Resources.Add(a);
        }
        //Save honeySynth
        foreach (var fur in GameObject.FindObjectsByType<HoneySynth>(FindObjectsSortMode.None))
        {
            HoneySynthToSave a = new HoneySynthToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save workshop
        foreach (var table in GameObject.FindObjectsByType<Workshop>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> foodList = new List<ResourceToSave>();
            foreach (var eat in table.workbench.craftList)
            {
                ResourceToSave f = new ResourceToSave() { Count = 1, Name = eat.name };
                foodList.Add(f);
            }

            WorkshopToSave beehiveToSave = new WorkshopToSave()
                { Position = new SerializableVector3(table.transform.position), CraftQueue = foodList };
            container.Resources.Add(beehiveToSave);
        }
        
        //Save science table
        foreach (var table in GameObject.FindObjectsByType<ScienceTable>(FindObjectsSortMode.None))
        {
            List<ResourceToSave> foodList = new List<ResourceToSave>();
            foreach (var eat in table.craftList)
            {
                ResourceToSave f = new ResourceToSave() { Count = 1, Name = eat.name };
                foodList.Add(f);
            }

            ScienceTableToSave beehiveToSave = new ScienceTableToSave()
                { Position = new SerializableVector3(table.transform.position), PlansQueue = foodList };
            container.Resources.Add(beehiveToSave);
        }
        
        //Save radio
        foreach (var fur in GameObject.FindObjectsByType<Radio>(FindObjectsSortMode.None))
        {
            RadioToSave a = new RadioToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save rocket
        foreach (var fur in GameObject.FindObjectsByType<Rocket>(FindObjectsSortMode.None))
        {
            RocketToSave a = new RocketToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save trees
        foreach (var fur in GameObject.FindObjectsByType<TreeResource>(FindObjectsSortMode.None))
        {
            TreeToSave a = new TreeToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save stones
        foreach (var fur in GameObject.FindObjectsByType<StoneResource>(FindObjectsSortMode.None))
        {
            StoneToSave a = new StoneToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save flowers
        foreach (var fur in GameObject.FindObjectsByType<FlowerResource>(FindObjectsSortMode.None))
        {
            FlowerToSave a = new FlowerToSave() { Position = new SerializableVector3(fur.transform.position)};
            container.Resources.Add(a);
        }
        
        //Save berries
        foreach (var berry in GameObject.FindObjectsByType<BerryResource>(FindObjectsSortMode.None))
        {
            BerryToSave a = new BerryToSave() { Position = new SerializableVector3(berry.transform.position) };
            container.Resources.Add(a);
        }
        
        //SaveTerrainData
        if (terrainManager.seed == 0)
        {
            TerrainDataToSave terr = new TerrainDataToSave()
            {
                Seed = terrainManager.GetRandomSeed()
            };
        }
        
        //Save GameData
        GameInfoToSave gi = new GameInfoToSave() { currentTask = TaskManager.Instance.currentTaskIndex, Craftable = GameManager.Instance.craftable, Letters = GameManager.Instance.letters};
        container.Resources.Add(gi);
        
        
        await Save();
        // Печать итогового JSON в консоль
        RequestsManager requestsManager =  new RequestsManager();
        await requestsManager.UpdateResources();
        Debug.Log("Container JSON:\n" + jsonOut);
    }

    public async Task Save()
    {
        jsonOut = JsonConvert.SerializeObject(container, Formatting.Indented);
        await File.WriteAllTextAsync(saveFilePath,jsonOut);
    }

    private void Load()
    {
        if (File.ReadAllText(saveFilePath) == "")
        {
            Debug.Log("File empty");
            return;
        }
        container = JsonConvert.DeserializeObject<Container>(File.ReadAllText(saveFilePath));

        // Печать имени контейнера
        Debug.Log("Container Name: " + container.Name);

        // Десериализация массива ресурсов (включая Table и Furnace)
        List<object> resourcesT = container.Resources;

        int bearCounter = 0;
        if (resourcesT == null) resourcesT = new List<object>();
        foreach (var resource in resourcesT)
        {
            // Определяем тип объекта и десериализуем его
            if (resource is JObject obj)
            {
                try
                {
                    string type = obj["type"].ToString();
                    if (type == "Test")
                    {
                        TableToSave table = obj.ToObject<TableToSave>();
                        Debug.Log("TestObj: " + table.Position + ", Resource: ");
                    }
                    else if (type == "Furnace")
                    {
                        FurnaceToSave furnace = obj.ToObject<FurnaceToSave>();
                        Debug.Log("Found Furnace: " + furnace.Position);
                        GameObject furnaceSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Furnace_0"));
                        furnaceSpawned.transform.position = furnace.Position.ToVector3();
                        Furnace furnaceComp = furnaceSpawned.GetComponent<Furnace>();
                        furnaceComp.Place();
                        furnaceComp.isPlased = true;
                        furnaceComp.currentFuel = new List<GameObject>();
                        foreach (var fuel in furnace.Fuel)
                        {
                            furnaceComp.currentFuel.Add(Resources.Load<GameObject>($"Resources/{fuel.Name}"));   
                        }
                        furnaceComp.currentMaterial = new List<GameObject>();
                        foreach (var mat in furnace.Material)
                        {
                            furnaceComp.currentMaterial.Add(Resources.Load<GameObject>($"Resources/{mat.Name}"));   
                        }
                    }
                    else if (type == "Beehive")
                    {
                        BeehiveToSave beehiveToSave = obj.ToObject<BeehiveToSave>();
                        Debug.Log("Beehive found: " + beehiveToSave.Position);
                        GameObject beehiveSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Beehive_0"));
                        beehiveSpawned.transform.position = beehiveToSave.Position.ToVector3();
                        Beehive beehiveComp = beehiveSpawned.GetComponent<Beehive>();
                        beehiveComp.Place();
                        beehiveComp.isPlased = true;
                        beehiveComp.bees = new List<string>();
                        foreach (var bee in beehiveToSave.Bees)
                        {
                            beehiveComp.bees.Add(bee);
                        }
                        foreach (var flow in beehiveToSave.Flowers)
                        {
                            beehiveComp.LoadFlower(Resources.Load<GameObject>($"Resources/{flow.Name}"));
                        }
                    }
                    else if (type == "Table")
                    {
                        TableToSave tableToSave = obj.ToObject<TableToSave>();
                        Debug.Log("Beehive found: " + tableToSave.Position);
                        GameObject tableSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Table"));
                        tableSpawned.transform.position = tableToSave.Position.ToVector3();
                        Table tableComp = tableSpawned.GetComponent<Table>();
                        tableComp.Place();
                        tableComp.isPlased = true;
                        tableComp.currentFuel = new List<GameObject>();
                        foreach (var item in tableToSave.Food)
                        {
                            tableComp.currentFuel.Add(Resources.Load<GameObject>($"Resources/{item.Name}"));
                        }
                    }
                    else if (type == "Kitchen")
                    {
                        KitchenToSave kitchen = obj.ToObject<KitchenToSave>();
                        Debug.Log("Found Kitchen: " + kitchen.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Kitchen"));
                        kitchenSpawned.transform.position = kitchen.Position.ToVector3();
                        Kitchen kitchenComp = kitchenSpawned.GetComponent<Kitchen>();
                        kitchenComp.Place();
                        kitchenComp.isPlased = true;
                        kitchenComp.currentFuel = new List<GameObject>();
                        foreach (var fuel in kitchen.Fuel)
                        {
                            kitchenComp.currentFuel.Add(Resources.Load<GameObject>($"Resources/{fuel.Name}"));   
                        }
                        kitchenComp.currentItem = null;
                    }
                    else if (type == "HoneySynth")
                    {
                        HoneySynthToSave kitchen = obj.ToObject<HoneySynthToSave>();
                        Debug.Log("Found HoneySynth: " + kitchen.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>("Buildings/HoneySynth"));
                        kitchenSpawned.transform.position = kitchen.Position.ToVector3();
                        
                        kitchenSpawned.GetComponent<HoneySynth>().Place();
                        kitchenSpawned.GetComponent<HoneySynth>().isPlased = true;
                    }
                    else if (type == "Radio")
                    {
                        RadioToSave kitchen = obj.ToObject<RadioToSave>();
                        Debug.Log("Found radio: " + kitchen.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Radio"));
                        kitchenSpawned.transform.position = kitchen.Position.ToVector3();
                        
                        kitchenSpawned.GetComponent<Radio>().Place();
                        kitchenSpawned.GetComponent<Radio>().isPlased = true;
                    }
                    else if (type == "Rocket")
                    {
                        RocketToSave kitchen = obj.ToObject<RocketToSave>();
                        Debug.Log("Found radio: " + kitchen.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Rocket"));
                        kitchenSpawned.transform.position = kitchen.Position.ToVector3();
                        
                        kitchenSpawned.GetComponent<Rocket>().Place();
                        kitchenSpawned.GetComponent<Rocket>().isPlased = true;
                    }
                    else if (type == "Tree")
                    {
                        TreeToSave tree = obj.ToObject<TreeToSave>();
                        Debug.Log("Found tree: " + tree.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>($"Buildings/Tree_{Random.Range(0, 3)}"));
                        kitchenSpawned.transform.position = tree.Position.ToVector3();
                    }
                    else if (type == "Stone")
                    {
                        StoneToSave tree = obj.ToObject<StoneToSave>();
                        Debug.Log("Found tree: " + tree.Position);
                        switch (Random.Range(0, 4))
                        {
                            case 0:
                                GameObject ore = Instantiate(Resources.Load<GameObject>($"Buildings/StoneOre"));
                                ore.transform.position = tree.Position.ToVector3();
                                break;
                            
                            case 1:
                                GameObject oreA = Instantiate(Resources.Load<GameObject>($"Buildings/IronOre"));
                                oreA.transform.position = tree.Position.ToVector3();
                                break;
                            
                            case 2:
                                GameObject oreB = Instantiate(Resources.Load<GameObject>($"Buildings/CopperOre"));
                                oreB.transform.position = tree.Position.ToVector3();
                                break;
                            
                            case 3:
                                GameObject oreC = Instantiate(Resources.Load<GameObject>($"Buildings/CoalOre"));
                                oreC.transform.position = tree.Position.ToVector3();
                                break;
                        }
                    }
                    else if (type == "Flower")
                    {
                        FlowerToSave tree = obj.ToObject<FlowerToSave>();
                        Debug.Log("Found flower: " + tree.Position);
                        GameObject kitchenSpawned = Instantiate(Resources.Load<GameObject>($"Buildings/FlowerResource_0"));
                        kitchenSpawned.transform.position = tree.Position.ToVector3();
                    }
                    else if (type == "Workshop")
                    {
                        WorkshopToSave tableToSave = obj.ToObject<WorkshopToSave>();
                        Debug.Log("Workshop found: " + tableToSave.Position);
                        GameObject tableSpawned = Instantiate(Resources.Load<GameObject>("Buildings/Workshop"));
                        tableSpawned.transform.position = tableToSave.Position.ToVector3();
                        Workshop tableComp = tableSpawned.GetComponent<Workshop>();
                        tableComp.Place();
                        tableComp.isPlased = true;
                        tableComp.workbench = tableComp.GetComponent<Workbench>();
                        tableComp.workbench.craftList = new List<GameObject>();
                        foreach (var item in tableToSave.CraftQueue)
                        {
                            tableComp.workbench.craftList.Add(Resources.Load<GameObject>($"Resources/{item.Name}"));
                        }
                    }
                    else if (type == "ScienceTable")
                    {
                        ScienceTableToSave tableToSave = obj.ToObject<ScienceTableToSave>();
                        Debug.Log("Beehive found: " + tableToSave.Position);
                        GameObject tableSpawned = Instantiate(Resources.Load<GameObject>("Buildings/ScienceTable"));
                        tableSpawned.transform.position = tableToSave.Position.ToVector3();
                        ScienceTable tableComp = tableSpawned.GetComponent<ScienceTable>();
                        tableComp.Place();
                        tableComp.isPlased = true;
                        tableComp.craftList = new List<GameObject>();
                        foreach (var item in tableToSave.PlansQueue)
                        {
                            tableComp.craftList.Add(Resources.Load<GameObject>($"Resources/{item.Name}"));
                        }
                    }
                    else if (type == "TerrainData")
                    {
                        TerrainDataToSave terrainData = obj.ToObject<TerrainDataToSave>();
                        if (terrainData.Seed == 0)
                        {
                            terrainManager.seed = terrainManager.GetRandomSeed();
                        }
                        else
                        {
                            terrainManager.seed = terrainData.Seed;
                        }
                    }
                    else if(type == "Bear")
                    {
                        BearToSave bearC = obj.ToObject<BearToSave>();
                        Debug.Log("Found Bear: " + bearC.Position + ", Hunger: " + bearC.Hunger);
                        BearManager.Instance.bearsList[bearCounter].transform.position = bearC.Position.ToVector3();
                        BearManager.Instance.bearsList[bearCounter].currentHunger = bearC.Hunger;
                        bearCounter++;
                    }
                    else if (type == "Resource")
                    {
                        ResourceToSave res = obj.ToObject<ResourceToSave>();
                        Debug.Log("Found Resource: " + res.Name + ", Count: " + res.Count);
                        GetResourceIconByName(res.Name).SetCount(res.Count);
                    }
                    else if (type == "GameInfo")
                    {
                        GameInfoToSave res = obj.ToObject<GameInfoToSave>();
                        for(int i = 0; i < res.currentTask; i++)
                        {
                            TaskManager.Instance.CompleteTask(i);
                        }

                        GameManager.Instance.craftable = new List<string>();
                        GameManager.Instance.craftable = res.Craftable;
                        GameManager.Instance.letters = res.Letters;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e+ "    \n" + resource.ToString());
                }

            }
        }
    }

    private void OnApplicationQuit()
    {
        //File.Delete(saveFilePath);
    }
}
// Класс для хранения общего контейнера
[Serializable]
public class Container
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("resources")]
    public List<object> Resources { get; set; }
}

// Класс для хранения магазина
[Serializable]
public class ShopContainer
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("resources")]
    public List<ResourceToSave> Resources { get; set; }
}

public class BearToSave
{
    [JsonProperty("type")]
    public string Type => "Bear";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("hunger")]
    public int Hunger { get; set; }
}

public class GameInfoToSave
{
    [JsonProperty("type")]
    public string Type => "GameInfo";

    [JsonProperty("currentTask")] 
    public int currentTask { get; set; }
    
    [JsonProperty("craftable")] 
    public List<string> Craftable { get; set; }
    
    [JsonProperty("letters")]
    public List<string> Letters { get; set; }
    
}

public class TerrainDataToSave
{
    [JsonProperty("type")]
    public string Type => "TerrainData";

    [JsonProperty("seed")]
    public int Seed { get; set; }
}

public class HoneySynthToSave
{
    [JsonProperty("type")]
    public string Type => "HoneySynth";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}

public class RadioToSave
{
    [JsonProperty("type")]
    public string Type => "Radio";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}

public class RocketToSave
{
    [JsonProperty("type")]
    public string Type => "Rocket";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}

public class TableToSave
{
    [JsonProperty("type")]
    public string Type => "Table";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("resource")]
    public List<ResourceToSave> Food { get; set; }
}

public class AllShopsData
{
    [JsonProperty("shops")]
    public List<ShopContainer> Resources;
    
}

public class ScienceTableToSave
{
    [JsonProperty("type")]
    public string Type => "ScienceTable";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("resource")]
    public List<ResourceToSave> PlansQueue { get; set; }
}

public class WorkshopToSave
{
    [JsonProperty("type")]
    public string Type => "Workshop";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("resource")]
    public List<ResourceToSave> CraftQueue { get; set; }
}

// Класс для хранения информации о ресурсе (name и count)
public class ResourceToSave
{
    [JsonProperty("type")]
    public string Type => "Resource";
    
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }
}

public class FurnaceToSave
{
    [JsonProperty("type")]
    public string Type => "Furnace";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("fuel")]
    public List<ResourceToSave> Fuel { get; set; }

    [JsonProperty("material")]
    public List<ResourceToSave> Material { get; set; }
}

public class KitchenToSave
{
    [JsonProperty("type")]
    public string Type => "Kitchen";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("fuel")]
    public List<ResourceToSave> Fuel { get; set; }
}

public class BeehiveToSave
{
    [JsonProperty("type")]
    public string Type => "Beehive";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }

    [JsonProperty("fuel")]
    public List<string> Bees { get; set; }

    [JsonProperty("material")]
    public List<ResourceToSave> Flowers { get; set; }
}

public class TreeToSave
{
    [JsonProperty("type")]
    public string Type => "Tree";
    
    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}
public class StoneToSave
{
    [JsonProperty("type")]
    public string Type => "Stone";
    

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}
public class FlowerToSave
{
    [JsonProperty("type")] 
    public string Type => "Flower";
    
    
    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}
public class BerryToSave
{
    [JsonProperty("type")]
    public string Type => "Berry";

    [JsonProperty("position")]
    public SerializableVector3 Position { get; set; }
}

public class SerializableVector3
{
    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("z")]
    public float Z { get; set; }

    public SerializableVector3(Vector3 vector)
    {
        X = vector.x;
        Y = vector.y;
        Z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }
}

public class ResourceInventory
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();
}


