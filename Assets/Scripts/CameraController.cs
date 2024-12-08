using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float keyboardMoveSpeed = 5f; // Скорость движения при использовании клавиатуры
    public float mouseDragSpeed = 0.1f;  // Скорость движения при перетаскивании мышью

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;         // Скорость приближения/отдаления
    public float minZoom = 5f;           // Минимальное приближение
    public float maxZoom = 20f;          // Максимальное отдаление

    [Header("Boundaries")]
    public Vector2 minBoundary; // Минимальные координаты X и Y
    public Vector2 maxBoundary; // Максимальные координаты X и Y

    private Vector3 dragOrigin; // Точка начала перетаскивания камеры
    private Camera cam;         // Ссылка на камеру

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleKeyboardMovement();
        HandleMouseDragMovement();
        HandleMouseZoom();
        ClampCameraPosition();
    }

    /// <summary>
    /// Обрабатывает передвижение камеры с помощью стрелок на клавиатуре.
    /// </summary>
    private void HandleKeyboardMovement()
    {
        float moveX = Input.GetAxis("Horizontal"); // Ввод по оси X (стрелки или WASD)
        float moveY = Input.GetAxis("Vertical");   // Ввод по оси Y (стрелки или WASD)

        Vector3 movement = new Vector3(moveX, moveY, 0) * keyboardMoveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// Обрабатывает передвижение камеры при зажатии средней кнопки мыши.
    /// </summary>
    private void HandleMouseDragMovement()
    {
        if (Input.GetMouseButtonDown(2)) // Нажата средняя кнопка мыши
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(2)) // Удерживается средняя кнопка мыши
        {
            Vector3 difference = dragOrigin - Input.mousePosition;
            transform.position += new Vector3(difference.x * mouseDragSpeed, difference.y * mouseDragSpeed, 0);
            dragOrigin = Input.mousePosition;
        }
    }

    /// <summary>
    /// Обрабатывает масштабирование камеры при прокрутке колеса мыши.
    /// </summary>
    private void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Получаем ввод от колеса мыши
        if (scroll != 0)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    /// <summary>
    /// Ограничивает позицию камеры в пределах заданных границ.
    /// </summary>
    private void ClampCameraPosition()
    {
        float clampedX = Mathf.Clamp(transform.position.x, minBoundary.x, maxBoundary.x);
        float clampedY = Mathf.Clamp(transform.position.y, minBoundary.y, maxBoundary.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
