using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public int width = 100;  // Ширина карты
    public int height = 100; // Высота карты
    public float mapScale = 20f;
    public int seed = 0;
    [Header("Tiles")]
    [Header("Ground")]
    public GameObject grassPrefab;  
    public GameObject waterPrefab;   
    public GameObject stonePrefab;
    [Header("Decorations")]
    public GameObject[] decorPrefabs; 
    
    [Header("Resources")]
    public GameObject[] resourcePrefabs; 
    
    public GridGraph gridGraph;     // Ссылка на граф для Pathfinding
    public Transform parentStatic;
    public Transform parentObstacle;
    public Transform parentDecor;
    public Transform parentResource;

    public List<int> validSeeds = new List<int>()
    {
        336,
        777,
        1984,
        2077,
        3333,
        4567,
        8888,
        9999,
        12345,
        86848
    };

    
    public int c;

    private void Start()
    {
        //seed = validSeeds[Random.Range(0, validSeeds.Count)];
        
    }

    public int GetRandomSeed()
    {
        return validSeeds[Random.Range(0, validSeeds.Count)];
    }

    public void GenerateTerrain(bool spawnResources)
    {
        GameObject tilePrefab = new GameObject();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Генерация шума с использованием Perlin Noise
                float xCoord = (float)x / width * mapScale;
                float yCoord = (float)y / height * mapScale;
                float sample = Mathf.PerlinNoise(xCoord + seed, yCoord + seed);
                
                // Генерация объектов (если нужно, для визуализации)
                
                Transform parent = parentStatic;
                if (sample <= 0.2f) // Generate water
                {
                    tilePrefab = waterPrefab;
                    parent = parentObstacle;
                }
                else if (sample is > 0.2f and <= 1f) // Generate Grass
                {
                    tilePrefab = grassPrefab;
                    parent = parentStatic;
                    if (sample < 0.7f)
                    {
                        if (Random.value is > 0.005f and <= 0.2f)
                        {
                            GameObject spawnedDecor = Instantiate(decorPrefabs[Random.Range(0, decorPrefabs.Length)], new Vector3(x/4f, y/4f, 0) - new Vector3(width/8f, height/8f), Quaternion.identity);
                            spawnedDecor.transform.parent = parentDecor;
                        }

                        if (spawnResources)
                        {
                            float v = Random.value;
                            if (v <= 0.004f && v > 0.0012f)
                            {
                                GameObject spawnedResource = Instantiate(resourcePrefabs[Random.Range(0, 3)],
                                    new Vector3(x / 4f, y / 4f, 0) - new Vector3(width / 8f, height / 8f),
                                    Quaternion.identity);
                                spawnedResource.transform.parent = parentResource;
                            }
                            else if (v <= 0.0002f)
                            {
                                GameObject spawnedResource = Instantiate(resourcePrefabs[3], //Berry bush   
                                    new Vector3(x / 4f, y / 4f, 0) - new Vector3(width / 8f, height / 8f),
                                    Quaternion.identity);
                                spawnedResource.transform.parent = parentResource;
                            }
                        }
                    }
                    else
                    {
                        tilePrefab = stonePrefab;
                        if (spawnResources)
                        {
                            float value = Random.value;
                            if (value <= 0.01f) // Generate resource chuncks
                            {
                                if (value is <= 0.01f && value > 0.002f)
                                {
                                    GameObject spawnedResource = Instantiate(resourcePrefabs[Random.Range(4, 7)],
                                        new Vector3(x / 4f, y / 4f, 0) - new Vector3(width / 8f, height / 8f),
                                        Quaternion.identity);
                                    spawnedResource.transform.parent = parentResource;
                                }

                                if (value is <= 0.002f)
                                {
                                    GameObject spawnedResource = Instantiate(resourcePrefabs[7],
                                        new Vector3(x / 4f, y / 4f, 0) - new Vector3(width / 8f, height / 8f),
                                        Quaternion.identity);
                                    spawnedResource.transform.parent = parentResource;
                                }
                                // else if (value is <= 0.005f and > 0.001f)
                                // {
                                //     GameObject spawnedResource = Instantiate(resourcePrefabs[4], 
                                //         new Vector3(x/4f, y/4f, 0) - new Vector3(width/8f, height/8f), Quaternion.identity);
                                //     spawnedResource.transform.parent = parentResource;
                                // }
                                // else
                                // {
                                //     GameObject spawnedResource = Instantiate(resourcePrefabs[5], 
                                //         new Vector3(x/4f, y/4f, 0) - new Vector3(width/8f, height/8f), Quaternion.identity);
                                //     spawnedResource.transform.parent = parentResource;
                                // }

                            }
                        }
                    }
                }
                if (tilePrefab != null)
                {
                    GameObject spawnedBlock = Instantiate(tilePrefab,
                        new Vector3(x / 4f, y / 4f, 0) - new Vector3(width / 8f, height / 8f), Quaternion.identity);
                    spawnedBlock.transform.parent = parent;
                    c++;
                }
            }
        }
    }
    
    

    public void CreateNavMesh()
    {
        gridGraph = AstarPath.active.data.gridGraph;

        // Перестроим граф пути
        AstarPath.active.Scan();
    }
}
