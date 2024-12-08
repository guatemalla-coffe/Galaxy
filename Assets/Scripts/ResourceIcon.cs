using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText; // Текст для количества
    public Image iconImage; // Изображение ресурса

    private int count;

    private Tween scaleTween;

    public GameObject currentResourceGO;

    public float fireHealth = -1f;

    public Craftable craftable;

    public float feedAmount = 0f;

    public bool canBeExplored;

    /// <summary>
    /// Инициализация иконки ресурса.
    /// </summary>
    public void Initialize()
    {
        count = 0;
        countText.text = count.ToString();

        // Установим иконку ресурса по имени
        Sprite resourceSprite = Resources.Load<Sprite>($"Icons/{name}");
        if (resourceSprite != null)
        {
            iconImage.sprite = resourceSprite;
        }
        else
        {
            Debug.LogWarning($"Иконка для ресурса '{name}' не найдена.");
        }
        
        // Установим префаб ресурса по имени
        GameObject resourcePrefab = Resources.Load<GameObject>($"Resources/{name}");
        if (resourcePrefab != null)
        {
            currentResourceGO = resourcePrefab;
        }
        else
        {
            Debug.LogWarning($"Префаб для ресурса '{name}' не найден.");
        }

        switch (name)
        {
            case "Wood":
                fireHealth = 20f;
                break;
            case "Coal":
                fireHealth = 90f;
                break;
            case "HoneyPastile":
                feedAmount = 30f;
                break;
        }
        
        if(GameManager.Instance.food.Contains(name))
        {
            craftable = currentResourceGO.GetComponent<Craftable>();
        }

        List<string> explored = new List<string>()
            { "Plan_Engine", "Plan_Wings", "Plan_ControlPanel", "Plan_Body", "Plan_FuelTank" };
        if (explored.Contains(name))
        {
            canBeExplored = true;
        }
    }
    
    public bool HasResource(int requiredAmount)
    {
        return GetCount() >= requiredAmount;
    }

    public int GetCount()
    {
        return count;
    }
    
    public void SetCount(int amount)
    {
        count = amount;
        UpdateAmountText();
    }

    public void IncreaseAmount(int amount)
    {
        GameObject spawnedSound = Instantiate(UIManager.Instance.takeResourceSoundPrefab);
        Destroy(spawnedSound, 0.5f);
        count += amount; // Увеличиваем количество
        UpdateAmountText(); // Обновляем текстовое поле
        ScaleTask();
        CheckCompletion();
        
        if (GetCount() <= 0)
        {
            UIManager.Instance.RemoveResourceIconFromList(this);
            Destroy(gameObject);
        }
        
        UIManager.Instance.SaveCurrentResources();
        UIManager.Instance.Save();
        ResourceToSave logResource = new ResourceToSave() { Name = name, Count = count };
        UIManager.Instance.LogMessage($"Изменение ресурса: {name}, разница: {amount}, общее кол-во: {GetCount()}", logResource);
    }
    

    private void CheckCompletion()
    {
        try
        {
            if (name == "Stone") TaskManager.Instance.CompleteTask(3);
            if (name == "Coal") TaskManager.Instance.CompleteTask(4);
            if (name == "Iron")
            {
                TaskManager.Instance.CompleteTask(5);
            }

            if (name == "IronIngot") TaskManager.Instance.CompleteTask(7);
            if (name == "Honey") TaskManager.Instance.CompleteTask(10);
            if(name == "HoneyPastile") TaskManager.Instance.CompleteTask(12);
            if(name == "EHoney") TaskManager.Instance.CompleteTask(17);
            if(name == "Gear") TaskManager.Instance.CompleteTask(19);


        }
        catch(Exception e)
        {
            
        }
    }

    private async UniTask ScaleTask()
    {
        if (scaleTween == null)
        {
            scaleTween = iconImage.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
            await scaleTween;
            scaleTween = iconImage.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
            await scaleTween;
            scaleTween = null;
        }
    }
    
    // Обновление текстового поля с количеством
    private void UpdateAmountText()
    {
        countText.text = count.ToString();
    }
    
    public void Explore()
    {

    }
}