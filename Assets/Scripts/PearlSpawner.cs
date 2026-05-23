using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [Header("Pearl Settings")]
    public GameObject pearlPrefab;
    public int numberOfPearls = 7;

    [Header("Golden Pearl")]
    public GameObject goldenPearlPrefab;

    [Header("Pearl Sound")]
    public AudioClip collectSound;

    [Header("Spawn Area")]
    public float minX = 400f;
    public float maxX = 530f;
    public float minZ = 400f;
    public float maxZ = 550f;
    public float spawnY = -6f;

    void Start()
    {
        SpawnPearls();
        SpawnGoldenPearl();
    }

    void SpawnPearls()
    {
        for (int i = 0; i < numberOfPearls; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);


            // Assign collect sound to each spawned pearl
            PearlManager pm = pearl.GetComponent<PearlManager>();
            if (pm != null && collectSound != null)
            {
                pm.collectSound = collectSound;
            }
        }
        Debug.Log("Spawned " + numberOfPearls + " pearls!");
    }

    void SpawnGoldenPearl()
    {
        if (goldenPearlPrefab == null)
        {
            Debug.Log("No golden pearl prefab assigned!");
            return;
        }

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);

        Instantiate(goldenPearlPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Golden pearl spawned at: " + spawnPosition);
    }
}