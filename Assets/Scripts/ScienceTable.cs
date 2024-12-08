using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScienceTable : Building
{
    public Transform craftPosition; // Точка для спавна готового ресурса
    public Animator animator; // Анимация работы
    public GameObject craftButtonPrefab;
    public Transform layoutGroupTransform;
    public GameObject completeSound;

    public List<GameObject> craftList;
    public GameObject psScience;

    private void Start()
    {
        GetComponent<CommandList>().Commands = new List<Command>() { new ScienceCommand(null, this) };
    }

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(23);
        GetComponent<SortingOrder>().Sort();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if(isPlased) UIManager.Instance.scienceTableWindow.OpenWindow(this);
    }

    public void PlaySound()
    {
        GameObject spawnedSound = Instantiate(completeSound);
        GameObject spawnedPS = Instantiate(psScience);
        spawnedPS.transform.position = transform.position;
        Destroy(spawnedPS, 4f);
        Destroy(spawnedSound, 2f);
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
        TaskManager.Instance.CompleteTask(24);

    }
    
    public void SpawnResource(GameObject prefab)
    {
        int woodToSpawn = 1; 
        for (int i = 0; i < woodToSpawn; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;
            
            spawnPosition = transform.position;

            // Пытаемся найти валидную позицию
            for (int attempt = 0; attempt < 10; attempt++)
            {
                // Рассчитываем случайное направление
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

                // Рассчитываем потенциальную позицию
                Vector3 potentialPosition =
                    transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, 2f);

                spawnPosition = potentialPosition;
                break;
            }

            GameObject wood = Instantiate(prefab, transform.position, Quaternion.identity);

            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void StartAnim()
    {
        GetComponent<Animator>().SetBool("isWorking", true);
    }

    public void StopAnim()
    {
        GetComponent<Animator>().SetBool("isWorking", false);
    }
}
