using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Workbench : MonoBehaviour
{
    public List<GameObject> craftList = new List<GameObject>();
    public Transform craftPosition;
    public string type;

    public bool HasResource(string resourceName, int count)
    {
        return UIManager.Instance.GetResourceIconByName(resourceName).GetCount() >= count;
    }

    public void UseResource(string resourceName, int count)
    {
        if (HasResource(resourceName, count))
        {
            UIManager.Instance.GetResourceIconByName(resourceName).IncreaseAmount(-count);
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

    public void StartAnim()
    {
        GetComponent<Animator>().SetBool("isWorking", true);
    }

    public void StopAnim()
    {
        GetComponent<Animator>().SetBool("isWorking", false);
    }
}