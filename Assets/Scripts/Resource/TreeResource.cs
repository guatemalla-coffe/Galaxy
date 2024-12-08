using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

public class TreeResource : ResourceObject
{
    private void Awake()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestResourceCommand(null, this))};
        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(2f, 2f), 0f, playerLayerMask))
        {
            print("DESTROY TREE AT POS: " + transform.position);
            Destroy(gameObject);
        }
    }

    
    public override async void TakeDamage()
    {
        health--;
        Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");

        GameObject spawnedSound = Instantiate(harvestSound);

        SpawnResource();

        await ShakeTree();
        Destroy(spawnedSound);

        // Если здоровье упало до нуля
        if (health <= 0)
        {
            GameObject spawnedEndSound = Instantiate(harvestEndSound);
            Destroy(spawnedEndSound, 3f);

            Deplete();
        }
    }

    
    protected override void Deplete()
    {
        Debug.Log($"Дерево {name} уничтожено.");
        gameObject.layer = 0;

        //AstarPath.active.Scan();
        //AstarPath.active.data.gridGraph.FloodFill();
        CustomFloodFill(transform.position, 3f);
        Destroy(gameObject, 0.1f);
    }
    
    private async UniTask ShakeTree()
    {
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        transform.GetChild(3).GetComponent<ParticleSystem>().Play();
        
        Vector3 originalPosition = transform.position;
        
        transform.DOShakePosition(shakeDuration, shakeStrength, 20, 90, false, true);
        
        await UniTask.Delay((int)(shakeDuration * 1000));
        
        transform.position = originalPosition;
    }
    
    public void CustomFloodFill(Vector3 position, float radius)
    {
        GridGraph gridGraph = AstarPath.active.data.gridGraph;
        int centerX = Mathf.FloorToInt(position.x / gridGraph.nodeSize);
        int centerY = Mathf.FloorToInt(position.y / gridGraph.nodeSize);
    
        // Перебираем все узлы в радиусе изменения
        for (int x = centerX - 5; x < centerX + 5; x++)
        {
            for (int y = centerY - 5; y < centerY + 5; y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth) continue;
                GridNodeBase node = gridGraph.GetNode(x, y);

                // Если узел в радиусе действия, то обновляем его
                if (Vector3.Distance(position, (Vector3)node.position) < radius)
                {
                    node.Walkable = !IsObstructed(node);  // Пример: проверяем, проходим ли этот узел
                }
            }
        }

        // Пересчитываем путь после обновления
        AstarPath.active.Scan();
    }

    private bool IsObstructed(GridNodeBase node)
    {
        // Здесь логика для проверки, заблокирован ли этот узел (например, если на нем есть объект)
        return false;  // Пример для простоты
    }
    
}