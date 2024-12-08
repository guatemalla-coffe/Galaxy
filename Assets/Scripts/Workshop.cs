using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Workshop : Building
{
    public Transform craftPosition; // Точка для спавна готового ресурса
    public Animator animator; // Анимация работы
    public GameObject craftButtonPrefab;
    public Transform layoutGroupTransform;
    public Workbench workbench;

    private void Start()
    {
        workbench = GetComponent<Workbench>();
        
        // Загрузка объектов из GameManager.Instance.craftable
        //LoadCraftableItems();
        
        GetComponent<CommandList>().Commands = new List<Command>() {new CraftCommand(null, workbench)};
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(18);
        GetComponent<SortingOrder>().Sort();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if(isPlased) UIManager.Instance.workshopWindow.OpenWindow(this);
    }

    // Метод загрузки доступных для крафта объектов
    private void LoadCraftableItems()
    {
        foreach (var craftableName in GameManager.Instance.craftable)
        {
            var prefab = Resources.Load<GameObject>($"Resources/{craftableName}");
            if (prefab != null)
            {
                workbench.craftList.Add(prefab);
            }
            else
            {
                Debug.LogWarning($"Не удалось загрузить объект для крафта: {craftableName}");
            }
        }
    }

    // Метод начала крафта
    public void StartCrafting(GameObject craftable)
    {
        Debug.Log($"Начинается крафт: {craftable.name}");
        animator.SetBool("isWorking", true);

        // Добавить логику спавна объекта после анимации
        SpawnCraftedItem(craftable);
    }

    public void StopCrafting()
    {
        Debug.Log("Крафт завершён.");
        animator.SetBool("isWorking", false);
    }

    // Метод спавна объекта
    private void SpawnCraftedItem(GameObject craftable)
    {
        Instantiate(craftable, craftPosition.position, Quaternion.identity);
        Debug.Log($"Скрафчен объект: {craftable.name}");
    }
}