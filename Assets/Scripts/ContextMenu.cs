using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    
    private Camera mainCamera;
    [SerializeField] private RectTransform canvasRectTransform;  // Ссылка на RectTransform Canvas
    [SerializeField] private RectTransform menuRectTransform; 
    
    public GameObject menuPanel; // Панель контекстного меню
    public Button buttonPrefab;  // Префаб кнопки

    private void OnEnable()
    {
        GameEvents.OnBuildingMenuOpen += CloseMenu;
    }

    private void OnDisable()
    {
        GameEvents.OnBuildingMenuOpen -= CloseMenu;
    }

    private void GenerateMenu(Vector3 position, List<Command> commands)
    {
        // Очищаем старые кнопки
        foreach (Transform child in menuPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        if (commands == null || commands.Count == 0)
        {
            Debug.LogError("Список команд пуст или не передан в GenerateMenu!");
            return;
        }
        
        // Создаём кнопки для каждой команды
        foreach (Command command in commands)
        {
            Button newButton = Instantiate(buttonPrefab, menuPanel.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = command.commandName;

            // Привязываем действие к кнопке
            newButton.onClick.AddListener(delegate
            {
                if(BearManager.Instance.GetSelectedBear().currentCommand != null) BearManager.Instance.GetSelectedBear().currentCommand.Cancel();
                command.bear = BearManager.Instance.GetSelectedBear();
                command.ExecuteAsync();
                UIManager.Instance.tableWindow.contextMenuForTableOpened = false;
                CloseMenu();
            });
        }
        // Показываем меню в позиции курсора
        menuPanel.SetActive(true);
        menuPanel.GetComponent<RectTransform>().position = ClampToScreen(position);
        
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        menuPanel.SetActive(false); // Прячем меню по умолчанию
    }

    void Update()
    {
        if (BearManager.Instance.GetSelectedBear() != null)
        {
            if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                if(GameManager.Instance.isBuilding()) return;
                
                Vector3 mousePosition = Input.mousePosition;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

                GameObject clickedObject = hit.collider != null ? hit.collider.gameObject : null;
                CommandList commandList;
                if (clickedObject != null && clickedObject.TryGetComponent<CommandList>(out commandList))
                {
                    List<Command> viewedCommands = new List<Command>() {new MoveCommand(BearManager.Instance.GetSelectedBear(),mainCamera.ScreenToWorldPoint(mousePosition))};
                    foreach (var command in commandList.Commands)
                    {
                        viewedCommands.Add(command);
                    }
                    if(BearManager.Instance.GetSelectedBear().currentCommand != null) BearManager.Instance.GetSelectedBear().currentCommand.Cancel();
                    viewedCommands[1].bear = BearManager.Instance.GetSelectedBear();
                    viewedCommands[1].ExecuteAsync();
                    print("clickedObject != null && clickedObject.TryGetComponent<CommandList>(out commandList)");
                    //GenerateMenu(mousePosition, viewedCommands);
                }
                else
                {
                    if(BearManager.Instance.GetSelectedBear().currentCommand != null) BearManager.Instance.GetSelectedBear().currentCommand.Cancel();
                    new MoveCommand(BearManager.Instance.GetSelectedBear(),
                        mainCamera.ScreenToWorldPoint(mousePosition)).ExecuteAsync();
                    //GenerateMenu(mousePosition, new List<Command>(){new MoveCommand(BearManager.Instance.GetSelectedBear(),mainCamera.ScreenToWorldPoint(mousePosition))});
                }
            }
        }
    }
    
    
    /// <summary>
    /// Ограничивает позицию меню в пределах экрана и размещает справа снизу от мыши.
    /// </summary>
    private Vector3 ClampToScreen(Vector3 position)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Учитываем размеры меню
        float menuWidth = menuRectTransform.rect.width;
        float menuHeight = menuRectTransform.rect.height;

        // Смещение: размещение справа снизу от мыши
        position.x += menuWidth / 2f;  // Сдвигаем вправо
        position.y -= menuHeight / 2f; // Сдвигаем вниз

        // Ограничение по горизонтали
        if (position.x < 0 + menuWidth/2f)
            position.x = menuWidth/2f;
        else if (position.x > screenWidth - menuWidth/2f)
            position.x = screenWidth - menuWidth/2f;

        // Ограничение по вертикали
        if (position.y < 0)
            position.y = 0;
        else if (position.y + menuHeight > screenHeight)
            position.y = screenHeight - menuHeight;

        return position;
    }

}
