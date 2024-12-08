using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform window; // Ссылка на RectTransform окна
    private Vector2 dragOffset; // Смещение между мышью и верхним левым углом окна
    private Rect screenBounds; // Границы экрана

    private void Start()
    {
        // Установим границы экрана, учитывая размеры окна
        screenBounds = new Rect(0, 0, Screen.width, Screen.height);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Определяем смещение между курсором и левым верхним углом окна
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window,
            eventData.position,
            eventData.pressEventCamera,
            out dragOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Переводим позицию мыши в координаты окна
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        // Новая позиция окна
        Vector2 newPosition = localPointerPosition - dragOffset;

        // Ограничиваем окно пределами экрана
        newPosition.x = Mathf.Clamp(newPosition.x, -screenBounds.width/2f + window.rect.width/2f, screenBounds.width/2f - window.rect.width/2f);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBounds.height/2f + window.rect.height/2f, screenBounds.height/2f - window.rect.height/2f);

        // Устанавливаем позицию окна
        window.anchoredPosition = newPosition;
    }
}