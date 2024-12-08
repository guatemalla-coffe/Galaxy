using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ResourceToUI : MonoBehaviour
{
    public string name;
    [SerializeField] private float moveDuration = 0.8f; // Длительность анимации
    [SerializeField] private Ease moveEase = Ease.InOutQuad; // Тип easing для анимации
    private Transform targetUI; // Целевая позиция в UI
    private Canvas canvas;
    public float feedAmount;
    public float fireHealth;

    private async void Awake()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        MoveToUIAndDestroy();
    }

    public async void MoveToUIAndDestroy()
    {
        // Получаем цель в UI (иконку ресурса)
        RectTransform targetUI = UIManager.Instance.GetResourceIconTarget(name);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(targetUI.GetComponent<ResourceIcon>().iconImage.transform.position);
        
        // Анимация перемещения ресурса в UI
        await transform.DOMove(worldPos, moveDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        // Добавляем ресурс в интерфейс после анимации
        UIManager.Instance.AddResource(name);
        
        // Удаляем ресурс из мира
        Destroy(gameObject);
    }


    public void Explore()
    {
        if (name == "Plan_Engine")
        {
            GameManager.Instance.craftable.Add("Engine");
        }
        else if (name == "Plan_Wings")
        {
            GameManager.Instance.craftable.Add("Wings");
        }
        else if (name == "Plan_Body")
        {
            GameManager.Instance.craftable.Add("Body");
        }
        else if (name == "Plan_ControlPanel")
        {
            GameManager.Instance.craftable.Add("ControlPanel");
        }
        else if (name == "Plan_FuelTank")
        {
            GameManager.Instance.craftable.Add("FuelTank");
        }
    }


}