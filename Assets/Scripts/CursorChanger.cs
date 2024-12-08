using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour
{
    public Texture2D customCursor; // Текстура курсора, которую вы хотите использовать
    public Vector2 hotspot = new Vector2(0, 0); // Точка отсчета для курсора (обычно центр или угол)

    // При наведении на объект
    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Оn UI, ignoring OnMouseDown");
            return;
        }
        // Устанавливаем кастомный курсор
        Cursor.SetCursor(customCursor, new Vector2(0, 0), CursorMode.Auto);
    }

    // Когда мышь покидает объект
    private void OnMouseExit()
    {
        // Возвращаем стандартный курсор
        Cursor.SetCursor(UIManager.Instance.customCursor, Vector2.zero, CursorMode.Auto);
    }
}