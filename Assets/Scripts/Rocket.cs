using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rocket : Building
{
    public bool onWork = false;
    public override async void Place()
    {
        TaskManager.Instance.CompleteTask(25);
        
        GetComponent<SortingOrder>().Sort();

    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if(isPlased) UIManager.Instance.rocketWindow.OpenWindow(this);
    }

    public async void StartWork(string answName, int count)
    {
        onWork = true;
        GetComponent<Animator>().SetBool("isWorking", true);
        UIManager.Instance.rocketWindow.chooseItemWindow.gameObject.SetActive(false);
        UIManager.Instance.rocketWindow.onWorkWindow.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(30f));
        GetComponent<Animator>().SetBool("isWorking", false);
        UIManager.Instance.rocketWindow.onWorkWindow.gameObject.SetActive(false);
        UIManager.Instance.rocketWindow.chooseItemWindow.gameObject.SetActive(true);
        for (int i = 0; i < count; i++)
        {
            SpawnResource(Resources.Load<GameObject>($"Resources/{answName}"));
        }

        onWork = false;
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
}
