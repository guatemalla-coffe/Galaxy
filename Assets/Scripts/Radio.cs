using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Radio : Building
{

    public override void Place()
    {
        TaskManager.Instance.CompleteTask(20);
        GetComponent<SortingOrder>().Sort();

    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Click on UI, ignoring OnMouseDown");
            return;
        }
        if(isPlased) UIManager.Instance.radioWindow.OpenWindow(this);
    }

    public async void StartWork(string answName)
    {
        if (UIManager.Instance.GetResourceIconByName("EHoney").GetCount() >= 1)
        {
            UIManager.Instance.GetResourceIconByName("EHoney").IncreaseAmount(-1);

            UIManager.Instance.radioWindow.chooseLetterWin.gameObject.SetActive(false);
            UIManager.Instance.radioWindow.awaitForSendingLetterWin.gameObject.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(3f));

            UIManager.Instance.radioWindow.awaitForSendingLetterWin.gameObject.SetActive(false);
            Instantiate(Resources.Load<GameObject>($"Answers/{answName}"), UIManager.Instance.radioWindow.answerWin);


            UIManager.Instance.radioWindow.answerWin.gameObject.SetActive(true);
        }
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
