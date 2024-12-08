using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public int health = 5; // Количество ударов, чтобы дерево исчезло
    public float shakeDuration = 0.5f;  // Длительность тряски
    public float shakeStrength = 0.2f;
    public Transform harvestTransform;
    [SerializeField] protected GameObject harvestSound;
    [SerializeField] protected GameObject harvestEndSound;
    [SerializeField] protected LayerMask playerLayerMask;
    [SerializeField] protected GameObject resourcePrefab;
    [SerializeField] protected float spawnRadius;
    
    public virtual async void TakeDamage(){}
    
    public bool IsDepleted()
    {
        return health <= 0;
    }
    
    protected virtual void Deplete() { }
    
    protected void SpawnResource()
    {
        int woodToSpawn = 1; // Количество досок, которые спавнятся при уроне
        for (int i = 0; i < woodToSpawn; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;

            // Пытаемся найти валидную позицию
            for (int attempt = 0; attempt < 10; attempt++) // Ограничиваем количество попыток
            {
                // Рассчитываем случайное направление
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

                // Рассчитываем потенциальную позицию
                Vector3 potentialPosition = transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, spawnRadius);

                // Проверяем достижимость точки через A*
                if (AstarPath.active != null)
                {
                    var graph = AstarPath.active.data.gridGraph;
                    var node = graph.GetNearest(potentialPosition).node;

                    if (node != null && node.Walkable)
                    {
                        spawnPosition = (Vector3)node.position;
                        validPositionFound = true;
                        break;
                    }
                }
            }

            // Если не нашли валидную позицию, используем позицию дерева
            if (!validPositionFound)
            {
                spawnPosition = transform.position;
            }

            // Создаем префаб доски
            GameObject wood = Instantiate(resourcePrefab, transform.position, Quaternion.identity);
            
            // Используем DOTween для движения доски
            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
