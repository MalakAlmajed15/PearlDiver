using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Pearl Settings")]
    public GameObject pearlPrefab;
    public GameObject goldenPearlPrefab;
    public int numberOfPearls = 10;

    [Header("Spawn Area (relative to player)")]
    public float rangeX = 20f;
    public float rangeZ = 20f;
    public float spawnYOffset = -3f;

    void Start()
    {
        SpawnPearls();
    }

    void SpawnPearls()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("No Player found! Tag your diver as Player.");
            return;
        }

        Vector3 playerPos = player.transform.position;

        // Pick one random index for the golden pearl
        int goldenIndex = Random.Range(0, numberOfPearls);

        for (int i = 0; i < numberOfPearls; i++)
        {
            float randomX = playerPos.x + Random.Range(-rangeX, rangeX);
            float randomZ = playerPos.z + Random.Range(-rangeZ, rangeZ);
            float spawnY = playerPos.y + spawnYOffset;

            Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);

            if (i == goldenIndex && goldenPearlPrefab != null)
            {
                Instantiate(goldenPearlPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("Golden pearl spawned!");
            }
            else
            {
                Instantiate(pearlPrefab, spawnPosition, Quaternion.identity);
            }
        }

        Debug.Log("Spawned " + numberOfPearls + " pearls!");
    }
}