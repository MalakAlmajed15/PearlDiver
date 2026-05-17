using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Creatures to Spawn")]
    public GameObject[] creaturePrefabs;

    [Header("Spawn Settings")]
    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;

    [Header("Spawn Area")]
    public float minX = -35f;
    public float maxX = 35f;
    public float minY = -10f;
    public float maxY = -2f;
    public float spawnZ = -15f; // Match your terrain Z

    void Start()
    {
        ScheduleNextSpawn();
    }

    void ScheduleNextSpawn()
    {
        float randomTime = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke("SpawnCreature", randomTime);
    }

    void SpawnCreature()
    {
        int randomIndex = Random.Range(0, creaturePrefabs.Length);

        
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position;

        float randomX = playerPos.x + Random.Range(-20f, 20f);
        float randomY = playerPos.y + Random.Range(-5f, 5f);
        float randomZ = playerPos.z + Random.Range(-20f, 20f);

        Vector3 spawnPos = new Vector3(randomX, randomY, randomZ);

        Instantiate(creaturePrefabs[randomIndex], spawnPos, Quaternion.identity);

        ScheduleNextSpawn();
    }
}