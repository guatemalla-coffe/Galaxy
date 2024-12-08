using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FlowerResource : ResourceObject
{
    private void Awake()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestResourceCommand(null, this))};
    }
    
    private void Start()
    {
        DestroyNearby();
    }

    private async void DestroyNearby()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        if (Physics2D.OverlapBox(new Vector2(harvestTransform.position.x, harvestTransform.position.y), new Vector2(0.2f, 0.2f), 0f, gameObject.layer))
        {
            print("DESTROY STONE AT POS: " + transform.position);
            Destroy(gameObject);
        }
    }

    public override async void TakeDamage()
    {
        health--;
        Debug.Log($"Цветок {name} получил урон. Осталось здоровья: {health}");
        //Spawn resource
        //UIManager.Instance.woodText.text = (Convert.ToInt32(UIManager.Instance.woodText.text) + 1).ToString();
        //Добавить дерево игроку
        GameObject spanedHarvestSound = Instantiate(harvestSound);
        Destroy(spanedHarvestSound, 1f);

        if (health <= 0)
        {
            Deplete();
        }
    }
    
    
    protected override void Deplete()
    {
        Debug.Log($"Камень {name} уничтожен.");
        gameObject.layer = 0;

        SpawnResource(resourcePrefab);
        Destroy(gameObject, 0.1f);
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
                    transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, 2f);

                spawnPosition = potentialPosition;
                break;
            }

            GameObject wood = Instantiate(prefab, transform.position, Quaternion.identity);

            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector2(harvestTransform.position.x, harvestTransform.position.y), new Vector2(0.2f, 0.2f));
    }
}
