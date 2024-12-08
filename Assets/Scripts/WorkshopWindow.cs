using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopWindow : MonoBehaviour
{
    public GameObject craftButtonPrefab; // Префаб кнопки для списка
    public Transform craftsPanel; // Родительский объект для списка кнопок
    public Transform queuePanel; // Родительский объект для списка кнопок
    public Workshop workshop; // Ссылка на Workshop
    public TextMeshProUGUI headerText; // Заголовок окна
    public GameObject craftRecipePrefab;

    private void Start()
    {
        UpdateWindow();
    }

    // Обновление интерфейса окна
    public void UpdateWindow()
    {
        ClearButtons();

        if (workshop != null)
        {
            LoadCrafts();
            LoadQueue();
        }
    }

    // Метод создания кнопки
    private void CreateButton(GameObject craftable)
    {
        var buttonInstance = Instantiate(craftButtonPrefab, craftsPanel);
        var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = craftable.name;

        var button = buttonInstance.GetComponent<Button>();
        button.onClick.AddListener(() => OnCraftButtonClicked(craftable));
    }
    
    private void LoadCrafts()
    {
        foreach (var craft in GameManager.Instance.craftable)
        {
            GameObject resourcePrefab = Resources.Load<GameObject>($"Resources/{craft}");
            if (resourcePrefab != null)
            {
                var buttonInstance = Instantiate(craftButtonPrefab, craftsPanel);
                var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
                switch (craft)
                {
                    case "Gear":
                        buttonText.text = "Шестерня";
                        break;
                    
                    case "Wires":
                        buttonText.text = "Медный провод";
                        break;
                    
                    case "Engine":
                        buttonText.text = "Двигатель шатла";
                        break;
                    
                    case "Body":
                        buttonText.text = "Корпус шатла";
                        break;
                    
                                        
                    case "FuelTank":
                        buttonText.text = "Энерготопливный бак шатла";
                        break;
                    
                    case "ControlPanel":
                        buttonText.text = "Панель управления шатла";
                        break;
                    
                                        
                    case "Wings":
                        buttonText.text = "Крылья шатла";
                        break;
                    
                    default:
                        buttonText.text = craft;
                        break;
                }

                var button = buttonInstance.GetComponent<Button>();
                buttonInstance.transform.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>($"Icons/{craft}");
                foreach (var a in resourcePrefab.GetComponent<Craftable>().price)
                {
                    GameObject spawnedRec = Instantiate(craftRecipePrefab, buttonInstance.transform.GetChild(2));
                    spawnedRec.transform.GetChild(0).GetComponent<Image>().sprite =
                        Resources.Load<Sprite>($"Icons/{a.name}");
                    spawnedRec.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = a.count.ToString();
                }
                button.onClick.AddListener(() => AddToQueue(resourcePrefab));
            }
        }

    }
    
    private void LoadQueue()
    {
        foreach (var craft in workshop.workbench.craftList)
        {
            GameObject resourcePrefab = craft;
            if (resourcePrefab != null)
            {
                var buttonInstance = Instantiate(craftButtonPrefab, queuePanel);
                var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = craft.GetComponent<Craftable>().name;
            
                var button = buttonInstance.GetComponent<Button>();
                buttonInstance.transform.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>($"Icons/{craft.GetComponent<ResourceToUI>().name}");
                foreach (var a in resourcePrefab.GetComponent<Craftable>().price)
                {
                    GameObject spawnedRec = Instantiate(craftRecipePrefab, buttonInstance.transform.GetChild(2));
                    spawnedRec.transform.GetChild(0).GetComponent<Image>().sprite =
                        Resources.Load<Sprite>($"Icons/{a.name}");
                    spawnedRec.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = a.count.ToString();
                }
                switch (craft.name)
                {
                    case "Gear":
                        buttonText.text = "Шестерня";
                        break;
                    
                    case "Wires":
                        buttonText.text = "Медный провод";
                        break;
                    
                    case "Engine":
                        buttonText.text = "Двигатель шатла";
                        break;
                    
                    case "Body":
                        buttonText.text = "Корпус шатла";
                        break;
                    
                                        
                    case "FuelTank":
                        buttonText.text = "Энерготопливный бак шатла";
                        break;
                    
                    case "ControlPanel":
                        buttonText.text = "Панель управления шатла";
                        break;
                    
                                        
                    case "Wings":
                        buttonText.text = "Крылья шатла";
                        break;
                    
                    default:
                        buttonText.text = craft.name;
                        break;
                }
                button.onClick.AddListener(() => RemoveFromQueue(resourcePrefab));
                
            }
        }

    }

    private void AddToQueue(GameObject resource)
    {
        print($"AddToQUeue: {resource}");
        workshop.workbench.craftList.Add(resource);
        UpdateWindow();
    }

    public void RemoveFromQueue(GameObject resource)
    {
        print($"RemovedFromQueue: {resource}");
        workshop.workbench.craftList.Remove(resource);
        UpdateWindow();
    }
    
    private void OnCraftButtonClicked(GameObject craftable)
    {
        var craftableComponent = craftable.GetComponent<Craftable>();
        if (craftableComponent == null)
        {
            Debug.LogError($"Объект {craftable.name} не содержит компонент Craftable!");
            return;
        }

        // Проверка доступности всех необходимых ресурсов
        foreach (var resource in craftableComponent.price)
        {
            if (!UIManager.Instance.GetResourceIconByName(resource.name).HasResource(resource.count))
            {
                Debug.LogWarning($"Недостаточно ресурса {resource.name} для крафта {craftable.name}!");
                return;
            }
        }
        
        workshop.workbench.craftList.Add(craftable);
        UpdateWindow();
    }


    // Очистка кнопок из окна
    private void ClearButtons()
    {
        foreach (Transform child in craftsPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in queuePanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public void OpenWindow(Workshop workshopS)
    {
        this.workshop = workshopS;
        gameObject.SetActive(true);
        UpdateWindow();
    }
}