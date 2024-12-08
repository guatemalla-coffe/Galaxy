using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    public List<string> fuels;
    public List<string> canBeFused;
    public List<string> fused;
    public List<string> flowers;
    public List<string> food;
    public List<string> craftable;
    public List<string> letters;
    //public List<string> rocketCanFlew;

    public List<ShopContainer> shopConts = new List<ShopContainer>();

    public bool shopsAcitve = false;

    public Light2D globalLight2D;  // Ссылка на компонент GlobalLight2D
    public TextMeshProUGUI timeText;                // Ссылка на текстовый компонент для отображения времени
    public float intensityMin = 0.2f;    // Минимальная интенсивность света (ночью)
    public float intensityMax = 1f;      // Максимальная интенсивность света (днем)
    public float dayDuration = 6f;       // Длительность одного дня в минутах (6 минут = 1 день)

    public float currentTime = 12f;           // Текущее виртуальное время в минутах
    public float timePerSecond;         // Время в минутах, которое проходит за 1 секунду

    private void Start()
    {
        // В 1 секунду проходит определенная доля дня. Например, если 1 день = 6 минутам (360 секунд),
        // то каждую секунду мы будем увеличивать текущее время на (6 минут / 360 секунд).
        timePerSecond = dayDuration / 360f;
    }

    private void Update()
    {
        // Увеличиваем текущее время
        currentTime += timePerSecond * Time.deltaTime;

        // Если время превышает 1 день (6 минут), сбрасываем его обратно в начало
        if (currentTime >= dayDuration)
        {
            currentTime -= dayDuration;
        }

        // Расчет времени в формате часов и минут (с округлением)
        int hour = Mathf.FloorToInt(currentTime);
        int minute = Mathf.FloorToInt((currentTime - hour) * 60);
        string timeFormatted = string.Format("{0:D2}:{1:D2}", hour, minute);
        timeText.text = timeFormatted;  // Обновляем текст на экране

        // Расчет интенсивности с максимальной интенсивностью в 12:00 и минимальной в 00:00
        // Мы используем нормализацию времени, чтобы получить плавное изменение интенсивности.
        float normalizedTime = currentTime / 24f; // Нормализуем время от 0 до 1

        // Получаем значение интенсивности, изменяющееся от 1 (в 12:00) до 0 (в 00:00)
        //float intensity = Mathf.Sin((normalizedTime + 0.5f) * Mathf.PI);  // Сдвиг на 0.5, чтобы максимальная интенсивность была в 12:00

        // Интерполируем интенсивность от минимальной до максимальной
        if (currentTime <= 3f)
        {
            globalLight2D.intensity = 0.1f;
        }        
        else if (currentTime <= 6f)
        {
            globalLight2D.intensity = 0.2f;
        }
        else if (currentTime <= 9f)
        {
            globalLight2D.intensity = 0.4f;
        }
        else if (currentTime <= 14f)
        {
            globalLight2D.intensity = 0.6f;
        }
        else if (currentTime <= 18f)
        {
            globalLight2D.intensity = 0.4f;
        }
        else if (currentTime <= 21f)
        {
            globalLight2D.intensity = 0.2f;
        }
        else if (currentTime <= 24f)
        {
            globalLight2D.intensity = 0.1f;
        }
        
    }
    
    public ShopContainer GetShopByName(string name)
    {
        shopConts = new List<ShopContainer>()
        {
            new ShopContainer()
            {
                Name = "Radio",
                Resources = new List<ResourceToSave>()
                {
                    new ResourceToSave(){Count = 5, Name = "Plan_Engine"},
                    new ResourceToSave(){Count = 5, Name = "Plan_Wings"},
                    new ResourceToSave(){Count = 5, Name = "Plan_Body"},
                    new ResourceToSave(){Count = 5, Name = "Plan_FuelTank"},
                    new ResourceToSave(){Count = 5, Name = "Plan_ControlPanel"}
                }
            },
            new ShopContainer()
            {
                Name = "Rocket",
                Resources = new List<ResourceToSave>()
                {
                    new ResourceToSave(){Count = 5, Name = "Wood"},
                    new ResourceToSave(){Count = 5, Name = "Iron"},
                    new ResourceToSave(){Count = 5, Name = "Stone"},
                    new ResourceToSave(){Count = 5, Name = "Coal"},
                    new ResourceToSave(){Count = 5, Name = "Copper"}
                }
            }
        };
        foreach (var shop in shopConts)
        {
            if (shop.Name == name)
            {
                return shop;
            }
        }

        return null;
    }

    public bool isBuilding()
    {
        return UIManager.Instance.buildingMenu.gameObject.activeSelf;
    }
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one GameManager!");
            Destroy(gameObject);
        }
    }
    
    
}
