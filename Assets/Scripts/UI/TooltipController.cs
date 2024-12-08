using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip; // Ссылка на объект всплывающего окна
    [SerializeField] private GameObject resourcePrefab; // Ссылка на префаб ресов

    private void Start()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false); // Убеждаемся, что окно скрыто по умолчанию
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(true); // Показываем окно при наведении
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false); // Скрываем окно, когда курсор уходит
        }
    }

    public void GenerateTips(BuildingData data)
    {
        foreach (var res in data.price)
        {
            GameObject spawnedPrefab = Instantiate(resourcePrefab, tooltip.transform);
            Sprite resourceSprite = Resources.Load<Sprite>($"Icons/{res.name}");
            spawnedPrefab.transform.GetChild(0).GetComponent<Image>().sprite = resourceSprite;
            spawnedPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = res.count.ToString();
        }
    }
}