using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    [SerializeField] private List<TaskData> tasks; // Список задач
    [SerializeField] private TextMeshProUGUI taskTitle; // Поле для заголовка задачи
    [SerializeField] private TextMeshProUGUI taskDescription; // Поле для описания задачи
    [SerializeField] private AudioSource victorySound; // Звук завершения задачи

    private Image image;

    public int currentTaskIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        image = GetComponent<Image>();
    }

    private void Start()
    {
        if (tasks == null || tasks.Count == 0)
        {
            Debug.LogError("Список задач пуст! Добавьте задачи в инспекторе.");
            return;
        }

        for(int i = 0; i < tasks.Count; i++)
        {
            tasks[i].index = i;
        }
        
        if (taskTitle == null || taskDescription == null)
        {
            Debug.LogError("Не назначены текстовые поля для заголовка и описания задачи!");
            return;
        }

        if (victorySound == null)
        {
            Debug.LogError("Не назначен AudioSource для победного звука!");
            return;
        }

        ShowFirst();
        // Инициализируем первую задачу
        UpdateUI();
    }

    public async UniTask ShowFirst()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        if(currentTaskIndex == 0) TutorialManager.Instance.Show(0); 
    }

    public void CompleteTask(int index)
    {
        if (currentTaskIndex >= tasks.Count)
        {
            Debug.Log("Все задачи выполнены!");
            return;
        }
        
        if(index != currentTaskIndex) return;
        
        if (index == 5)
        {
            TutorialManager.Instance.Show(1);
        }
        if (index == 8)
        {
            TutorialManager.Instance.Show(2);
        }
        
        TaskData currentTask = tasks[currentTaskIndex];
        if (currentTask.IsCompleted)
        {
            Debug.Log("Текущая задача уже была выполнена.");
            return;
        }

        // Завершаем текущую задачу
        currentTask.IsCompleted = true;
        Debug.Log($"Задача выполнена: {currentTask.Title}");
        
        victorySound.Play();
        
        if (currentTaskIndex < tasks.Count - 1)
        {
            currentTaskIndex++;
            UpdateUI();
        }
        else
        {
            Debug.Log("Все задачи завершены!");
        }
    }

    private void UpdateUI()
    {
        // Обновляем UI для текущей задачи
        ColorTask();
        TaskData currentTask = tasks[currentTaskIndex];
        taskTitle.text = currentTask.Title;
        taskDescription.text = currentTask.Description;
        Debug.Log($"Текущая задача обновлена: {currentTask.Title}");
    }

    public bool IsTaskCurrent(TaskData task)
    {
        // Проверяет, является ли задача текущей
        return tasks.IndexOf(task) == currentTaskIndex;
    }

    public async UniTask ColorTask()
    {
        image.color = Color.white;
        await image.DOColor(new Color(0.1f, 0.1f, 0.1f, 0.6f), 2f).ToUniTask();
    }
    
    [Serializable]
    public class TaskData
    {
        public string Title; // Заголовок задачи
        public string Description; // Описание задачи
        public int index;
        public bool IsCompleted; // Флаг выполнения задачи

        public TaskData(string title, string description, int index)
        {
            Title = title;
            Description = description;
            IsCompleted = false;
            this.index = index;
        }
    }

}