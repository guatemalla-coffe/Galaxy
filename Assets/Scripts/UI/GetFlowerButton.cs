using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GetFlowerButton : MonoBehaviour
{
    public GameObject resource;
    public Image icon;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(delegate
        {
            GetFlowerBack();
        });
    }

    public void GetFlowerBack()
    {
        if (resource != null)
        {
            UIManager.Instance.beehiveWindow.beehive.flowers.Remove(resource);
            SpawnResource(resource);
            UIManager.Instance.beehiveWindow.UpdateWindow();
            UIManager.Instance.beehiveWindow.chooseMaterialWindow.SetActive(false);
        }
    }
    
    private void SpawnResource(GameObject prefab)
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
                    UIManager.Instance.beehiveWindow.beehive.transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, 2f);

                spawnPosition = potentialPosition;
                break;
            }

            GameObject wood = Instantiate(prefab, UIManager.Instance.beehiveWindow.beehive.transform.position, Quaternion.identity);

            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }

}
