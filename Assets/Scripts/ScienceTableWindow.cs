using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScienceTableWindow : MonoBehaviour
{
    public GameObject craftButtonPrefab; // Префаб кнопки для списка
    public Transform craftsPanel; // Родительский объект для списка кнопок
    public Transform queuePanel; // Родительский объект для списка кнопок
    public ScienceTable scienceTable; // Ссылка на Workshop

    private void Start()
    {
        UpdateWindow();
    }

    // Обновление интерфейса окна
    public void UpdateWindow()
    {
        ClearButtons();

        if (scienceTable != null)
        {
            LoadCrafts();
            LoadQueue();
        }
    }

    // Метод создания кнопки
    private void CreateButton(ResourceIcon craftable)
    {
        var buttonInstance = Instantiate(craftButtonPrefab, craftsPanel);
        var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
        switch (craftable.name)
        {
            case "Plan_Engine":
                buttonText.text = "Чертеж двигателя";
                break;
            
            case "Plan_Wings":
                buttonText.text = "Чертеж крыльев";
                break;
            
            case "Plan_Body":
                buttonText.text = "Чертеж корпуса";
                break;
            
            case "Plan_ControlPanel":
                buttonText.text = "Чертеж платы";
                break;
            
            case "Plan_FuelTank":
                buttonText.text = "Чертеж топливного бака";
                break;
            
            default:
                buttonText.text = craftable.name;
                break;
        }

        var button = buttonInstance.GetComponent<Button>();
        button.onClick.AddListener(() => OnCraftButtonClicked(craftable.currentResourceGO));
    }
    
    private void LoadCrafts()
    {
        foreach (var craft in UIManager.Instance.GetResourceIconList())
        {
            if (craft.canBeExplored)
            {
                ResourceIcon resourcePrefab = craft;
                if (resourcePrefab != null)
                {
                    var buttonInstance = Instantiate(craftButtonPrefab, craftsPanel);
                    var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
                    //buttonText.text = craft.name;
                    switch (craft.name)
                    {
                        case "Plan_Engine":
                            buttonText.text = "Чертеж двигателя";
                            break;
            
                        case "Plan_Wings":
                            buttonText.text = "Чертеж крыльев";
                            break;
            
                        case "Plan_Body":
                            buttonText.text = "Чертеж корпуса";
                            break;
            
                        case "Plan_ControlPanel":
                            buttonText.text = "Чертеж платы";
                            break;
            
                        case "Plan_FuelTank":
                            buttonText.text = "Чертеж топливного бака";
                            break;
            
                        default:
                            buttonText.text = craft.name;
                            break;
                    }
                    buttonInstance.transform.GetChild(0).GetComponent<Image>().sprite =
                        Resources.Load<Sprite>($"Icons/{craft.name}");
                    var button = buttonInstance.GetComponent<Button>();

                    button.onClick.AddListener(() => AddToQueue(resourcePrefab));
                }
            }
        }

    }
    
    private void LoadQueue()
    {
        foreach (var craft in scienceTable.craftList)
        {
            GameObject resourcePrefab = craft;
            if (resourcePrefab != null)
            {
                var buttonInstance = Instantiate(craftButtonPrefab, queuePanel);
                var buttonText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = resourcePrefab.name;
                switch (craft.name)
                {
                    case "Plan_Engine":
                        buttonText.text = "Чертеж двигателя";
                        break;
            
                    case "Plan_Wings":
                        buttonText.text = "Чертеж крыльев";
                        break;
            
                    case "Plan_Body":
                        buttonText.text = "Чертеж корпуса";
                        break;
            
                    case "Plan_ControlPanel":
                        buttonText.text = "Чертеж платы";
                        break;
            
                    case "Plan_FuelTank":
                        buttonText.text = "Чертеж топливного бака";
                        break;
            
                    default:
                        buttonText.text = craft.name;
                        break;
                }
                buttonInstance.transform.GetChild(0).GetComponent<Image>().sprite =
                    Resources.Load<Sprite>($"Icons/{craft.name}");
                var button = buttonInstance.GetComponent<Button>();

                button.onClick.AddListener(() => RemoveFromQueue(resourcePrefab));
            }
        }

    }

    private void AddToQueue(ResourceIcon resourceIcon)
    {
        print("SetTemplate");
        resourceIcon.IncreaseAmount(-1);
        UIManager.Instance.scienceTableWindow.UpdateWindow();
        print($"AddToQUeue: {resourceIcon}");
        scienceTable.craftList.Add(resourceIcon.currentResourceGO);
        UpdateWindow();
    }

    public void RemoveFromQueue(GameObject resource)
    {
        print($"RemovedFromQueue: {resource}");
        scienceTable.craftList.Remove(resource);
        scienceTable.SpawnResource(resource);
        UpdateWindow();
    }
    
    private void OnCraftButtonClicked(GameObject craftable)
    {
        scienceTable.craftList.Add(craftable);
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

    public void OpenWindow(ScienceTable science)
    {
        this.scienceTable = science;
        gameObject.SetActive(true);
        UpdateWindow();
    }
}
