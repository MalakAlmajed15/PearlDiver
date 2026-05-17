using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Pearl Settings")]
    public GameObject pearlPrefab;
    public int numberOfPearls = 10;

    [Header("Spawn Area")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;
    public float spawnY = -5f;

    void Start()
    {
        SpawnPearls();
    }

    void SpawnPearls()
    {
        for (int i = 0; i < numberOfPearls; i++)
        {
            // Pick a random position within the spawn area
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);

            // Spawn the pearl at that position
            Instantiate(pearlPrefab, spawnPosition, Quaternion.identity);
        }
        Debug.Log("Spawned " + numberOfPearls + " pearls!");
    }
}